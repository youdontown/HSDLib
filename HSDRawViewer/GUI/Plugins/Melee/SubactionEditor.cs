﻿using System;
using System.Windows.Forms;
using HSDRaw;
using System.Collections.Generic;
using HSDRaw.Melee.Pl;
using WeifenLuo.WinFormsUI.Docking;
using System.Drawing;
using HSDRawViewer.Tools;
using System.Linq;
using HSDRawViewer.Rendering;
using HSDRaw.Melee.Cmd;
using System.IO;
using HSDRawViewer.GUI.Extra;

namespace HSDRawViewer.GUI.Plugins.Melee
{
    public partial class SubactionEditor : DockContent, EditorBase
    {
        public DockState DefaultDockState => DockState.Document;

        public Type[] SupportedTypes => new Type[] { typeof(SBM_FighterCommandTable), typeof(SBM_FighterSubactionData), typeof(SBM_ItemSubactionData), typeof(SBM_ColorSubactionData) };

        public DataNode Node
        {
            get => _node;
            set
            {
                _node = value;
                if (value.Accessor is SBM_FighterSubactionData sudata)
                {
                    if (value.Accessor is SBM_ItemSubactionData)
                        SubactionGroup = SubactionGroup.Item;

                    if (value.Accessor is SBM_ColorSubactionData)
                        SubactionGroup = SubactionGroup.Color;

                    SBM_FighterCommand[] su = new SBM_FighterCommand[]
                    {
                        new SBM_FighterCommand()
                        {
                            SubAction = sudata,
                            Name = "Script"
                        }
                    };

                    LoadActions(su);
                    RefreshActionList();

                    // disable fighter only stuff
                    loadPlayerFilesToolStripMenuItem.Enabled = false;
                    propertyGrid1.Visible = false;
                }
                else
                if(value.Accessor is SBM_FighterCommandTable SubactionTable)
                {
                    LoadActions(SubactionTable.Commands);
                    RefreshActionList();

                    if (Node.Parent is DataNode parent)
                    {
                        if (parent.Accessor is SBM_FighterData plDat)
                        {
                            ModelScale = plDat.Attributes.ModelScale;

                            if (plDat.Hurtboxes != null)
                                Hurtboxes.AddRange(plDat.Hurtboxes.Hurtboxes);
                            
                            ECB = plDat.EnvironmentCollision;

                            ResetModelVis();
                        }
                    }
                }
            }
        }

        public SBM_FighterData FighterData
        {
            get
            {
                if (_node.Accessor is SBM_FighterCommandTable SubactionTable)
                    if (Node.Parent is DataNode parent)
                        if (parent.Accessor is SBM_FighterData plDat)
                            return plDat;

                return null;
            }
        }

        private DataNode _node;

        public SubactionGroup SubactionGroup = SubactionGroup.Fighter;

        /// <summary>
        /// 
        /// </summary>
        public SubactionEditor()
        {
            InitializeComponent();

            renderPanel.Visible = false;

            DoubleBuffered = true;

            viewport = new ViewportControl();
            viewport.Dock = DockStyle.Fill;
            viewport.AnimationTrackEnabled = true;
            viewport.AddRenderer(this);
            viewport.EnableFloor = true;
            previewBox.Controls.Add(viewport);
            viewport.RefreshSize();
            viewport.BringToFront();

            toolStripComboBox1.SelectedIndex = 0;

            clickTimer = new Timer();
            clickTimer.Interval = 500;
            clickTimer.Tick += timer_Tick;

            FormClosing += (sender, args) =>
            {
                SaveFile();
                JOBJManager.CleanupRendering();
                viewport.Dispose();
                _animEditor.CloseOnExit = true;
                _animEditor.Dispose();
            };

            _animEditor.FormClosing += (sender, args) =>
            {
                // quietly save changes to animation
                if (_animEditor.MadeChanges)
                {
                   // MessageBox.Show("Changes Made");
                    //if (MessageBox.Show("Save Changes to Animation?", "Save Animation", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    SaveAnimation();
                }
            };
            
            SubactionProcess.UpdateVISMethod = SetModelVis;
            SubactionProcess.AnimateMaterialMethod = AnimateMaterial;
            SubactionProcess.AnimateModelMethod = AnimateModel;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshActionList()
        {
            actionList.Items.Clear();
            subActionList.Items.Clear();
            var actionIndex = 0;
            var routineIndex = 0;
            foreach (var sa in AllActions)
            {
                if(sa.Subroutine)
                    sa.Index = routineIndex++;
                else
                    sa.Index = actionIndex++;
                actionList.Items.Add(sa);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="script"></param>
        private void SelectAction(Action script)
        {
            propertyGrid1.SelectedObject = script;

            ClearUndoStack();

            // gather all references to this script
            var references = AllActions.FindAll(e=>e._struct.References.ContainsValue(script._struct));

            cbReference.Items.Clear();
            foreach(var r in references)
            {
                cbReference.Items.Add(r);
            }

            renderPanel.Visible = (cbReference.Items.Count > 0);

            if (cbReference.Items.Count > 0)
                cbReference.SelectedIndex = 0;
            
            RefreshSubactionList(script);

            LoadAnimation(script.Symbol);

            ResetModelVis();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="script"></param>
        private void RefreshSubactionList(Action script)
        {
            // set the script for the subaction processer for rendering
            SubactionProcess.SetStruct(script._struct, SubactionGroup);

            // begin filling the subaction list
            subActionList.BeginUpdate();
            subActionList.Items.Clear();
            var scripts = GetScripts(script);
            foreach (var s in scripts)
                subActionList.Items.Add(s);
            subActionList.EndUpdate();
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadPlayerFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_node.Text.Contains("Fighter"))
                LoadFighterAnimationFiles();
            else
            if (_node.Text.Contains("Demo"))
                LoadDemoAnimationFiles();
        }


        #region Undo
        private Stack<byte[]> UndoDataStack = new Stack<byte[]>();
        private Stack<Dictionary<int, HSDStruct>> UndoReferenceStack = new Stack<Dictionary<int, HSDStruct>>();

        private void ClearUndoStack()
        {
            UndoDataStack.Clear();
            UndoReferenceStack.Clear();
        }

        private void AddActionToUndo()
        {
            if (actionList.SelectedItem is Action a)
            {
                UndoDataStack.Push(a._struct.GetData());
                UndoReferenceStack.Push(new Dictionary<int, HSDStruct>(a._struct.References));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Undo()
        {
            if (UndoDataStack.Count > 0 && actionList.SelectedItem is Action a)
            {
                a._struct.SetData(UndoDataStack.Pop());
                
                a._struct.References.Clear();
                var prevRef = UndoReferenceStack.Pop();
                foreach (var v in prevRef)
                    a._struct.References.Add(v.Key, v.Value);

                RefreshSubactionList(a);
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        private void SaveFile()
        {
            //SaveFighterAnimationFile();
            //MessageBox.Show("This feature has not yet been implemented");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void actionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(actionList.SelectedItem is Action a)
            {
                SelectAction(a);
            }
            else
            {
                subActionList.Items.Clear();
                cbReference.Items.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_DoubleClick(object sender, EventArgs e)
        {
            EditSubAction();
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var data = new byte[] { 0, 0, 0, 0 };
            var action = new Action()
            {
                _struct = new HSDStruct(data)
            };

            var index = -1;// actionList.SelectedIndex;

            if (index == -1 || index > ActionCount)
                index = ActionCount;

            AllActions.Insert(index, action);
            RefreshActionList();
            SaveAllActionChanges();
            actionList.SelectedItem = action;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createNewSubroutineToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var data = new byte[] { 0x18, 0, 0, 0 };
            var action = new Action()
            {
                _struct = new HSDStruct(data),
                Subroutine = true
            };
            AllActions.Insert(actionList.Items.Count, action);
            RefreshActionList();
            actionList.SelectedItem = action;

            SaveAllActionChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteSelectedActionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(actionList.SelectedIndex != -1)
            {
                AllActions.RemoveAt(actionList.SelectedIndex);
                RefreshActionList();

                SaveAllActionChanges();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var ac = new SubActionScript(SubactionGroup)
            {
                data = new byte[] { 0, 0, 0, 0 }
            };
            subActionList.Items.Insert(subActionList.SelectedIndex + 1, ac);
            subActionList.SelectedItem = null;
            subActionList.SelectedItem = ac;

            SaveSelectedActionChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            RemoveSelected();
        }

        /// <summary>
        /// 
        /// </summary>
        private void RemoveSelected()
        {
            subActionList.BeginUpdate();
            if (subActionList.SelectedItems.Count != 0)
            {
                var list = new List<object>();
                foreach (var v in subActionList.SelectedItems)
                    list.Add(v);
                foreach (var v in list)
                    subActionList.Items.Remove(v);
            }
            subActionList.EndUpdate();

            SaveSelectedActionChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            EditSubAction();
        }

        /// <summary>
        /// 
        /// </summary>
        private void EditSubAction()
        {
            var selectedIndex = subActionList.SelectedIndex;

            if (subActionList.SelectedItem is SubActionScript sa)
            {
                using (SubActionPanel p = new SubActionPanel(AllActions))
                {
                    p.LoadData(sa.data, sa.Reference, SubactionGroup);
                    if (p.ShowDialog() == DialogResult.OK)
                    {
                        sa.data = p.Data;

                        if (sa.data.Length > 0 && SubactionManager.GetSubaction((byte)(sa.data[0]), SubactionGroup).HasPointer)
                            sa.Reference = p.Reference;
                        else
                            sa.Reference = null;

                        subActionList.Items[selectedIndex] = subActionList.SelectedItem;

                        SaveSelectedActionChanges();

                        subActionList.Invalidate();
                    }
                }

                // redraw item
                var item = subActionList.SelectedItem;
                var itempos = subActionList.SelectedIndex;
                subActionList.Items.Remove(item);
                subActionList.Items.Insert(itempos, item);

                subActionList.SelectedIndex = selectedIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RedrawActionItems()
        {
            var items = new List<object>();
            foreach (var i in subActionList.Items)
                items.Add(i);

            subActionList.Items.Clear();
            foreach(var i in items)
                subActionList.Items.Add(i);
        }

        #region ToolStrip

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGoto_Click(object sender, EventArgs e)
        {
            if(cbReference.SelectedItem is Action a)
            {
                actionList.SelectedIndex = actionList.Items.IndexOf(a);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if(e.Index != -1 && subActionList.Items[e.Index] is SubActionScript script)
            {
                var length = script.GetParamsAsString(null).Count();
                e.ItemHeight = subActionList.Font.Height * (toolStripComboBox1.SelectedIndex != 0 ? 1 : length + 1);
                e.ItemHeight = Math.Min(e.ItemHeight, 255); // limit
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if(e.Index != -1)
            {
                if(subActionList.Items[e.Index] is SubActionScript script)
                {
                    var sa = SubactionManager.GetSubaction(script.data[0], SubactionGroup);
                    e.Graphics.DrawString(e.Index + ". " + script.Name + (toolStripComboBox1.SelectedIndex == 2 ? "(" + string.Join(", ", script.GetParamsAsString(null)) + ")" : ""), e.Font, new SolidBrush(sa.IsCustom ? Color.DarkOrange : Color.DarkBlue), e.Bounds);
                    int i = 1;
                    if (toolStripComboBox1.SelectedIndex == 0)
                        foreach (var v in script.GetParamsAsString(null))
                        {
                            if (e.Bounds.Y + e.Font.Height * i >= e.Bounds.Y + e.Bounds.Height)
                                break;
                            var bottomRect = new Rectangle(new Point(e.Bounds.X, e.Bounds.Y + e.Font.Height * i), new Size(e.Bounds.Width, e.Bounds.Height));
                            e.Graphics.DrawString("\t" + v, e.Font, new SolidBrush(e.ForeColor), bottomRect);
                            i++;
                        }
                }
                else
                    e.Graphics.DrawString(subActionList.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds);

            }
        }

        // move up
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            subActionList.BeginUpdate();
            int[] indexes = subActionList.SelectedIndices.Cast<int>().ToArray();
            if (indexes.Length > 0 && indexes[0] > 0)
            {
                for (int i = 0; i < subActionList.Items.Count; ++i)
                {
                    if (indexes.Contains(i))
                    {
                        object moveItem = subActionList.Items[i];
                        subActionList.Items.Remove(moveItem);
                        subActionList.Items.Insert(i - 1, moveItem);
                        subActionList.SetSelected(i - 1, true);
                    }
                }
            }
            subActionList.EndUpdate();
            SaveSelectedActionChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDown_Click(object sender, EventArgs e)
        {
            subActionList.BeginUpdate();
            int[] indexes = subActionList.SelectedIndices.Cast<int>().ToArray();
            if (indexes.Length > 0 && indexes[indexes.Length - 1] < subActionList.Items.Count - 1)
            {
                for (int i = subActionList.Items.Count - 1; i > -1; --i)
                {
                    if (indexes.Contains(i))
                    {
                        object moveItem = subActionList.Items[i];
                        subActionList.Items.Remove(moveItem);
                        subActionList.Items.Insert(i + 1, moveItem);
                        subActionList.SetSelected(i + 1, true);
                    }
                }
            }
            subActionList.EndUpdate();
            SaveSelectedActionChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCopy_Click(object sender, EventArgs e)
        {
            CopySelected();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPaste_Click(object sender, EventArgs e)
        {
            Paste();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReplacePaste()
        {
            var selectedIndex = subActionList.SelectedIndex;
            if (selectedIndex != -1)
            {
                RemoveSelected();
                subActionList.SelectedIndex = selectedIndex - 1;
                Paste();
                subActionList.SelectedIndex = selectedIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CopySelected()
        {
            // Lets say its my data format
            Clipboard.Clear();

            // get collections of selected scripts
            var scripts = new List<SubActionScript>();
            foreach (SubActionScript scr in subActionList.SelectedItems)
                scripts.Add(scr);

            // Put data into clipboard
            Clipboard.SetData(typeof(List<SubActionScript>).FullName, scripts);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Paste()
        {
            // get insert index
            var index = subActionList.SelectedIndex + 1;
            if (index == -1)
                index = 0;

            // Get data object from the clipboard
            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null)
            {
                // Check if a collection of Slides is in the clipboard
                string dataFormat = typeof(List<SubActionScript>).FullName;
                if (dataObject.GetDataPresent(dataFormat))
                {
                    // Retrieve slides from the clipboard
                    List<SubActionScript> scripts = dataObject.GetData(dataFormat) as List<SubActionScript>;
                    if (scripts != null)
                    {
                        // insert scripts
                        scripts.Reverse();
                        foreach (var v in scripts)
                            // only paste subactions the belong to this group
                            if(v.GetGroup() == SubactionGroup)
                                subActionList.Items.Insert(index, v.Clone());

                        SaveSelectedActionChanges();
                    }
                }
            }
            
        }

        /// <summary>
        /// Hot keys
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Oemplus)
                buttonAdd_Click(null, null);

            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.OemMinus)
                RemoveSelected();

            if (e.Control)
            {
                if (e.KeyCode == Keys.X)
                {
                    CopySelected();
                    RemoveSelected();
                }

                if (e.KeyCode == Keys.C)
                    CopySelected();

                if (e.KeyCode == Keys.V)
                {
                    if (e.Shift)
                        ReplacePaste();
                    else
                        Paste();
                }

                if (e.KeyCode == Keys.Z)
                    Undo();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCut_Click(object sender, EventArgs e)
        {
            CopySelected();
            RemoveSelected();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RedrawActionItems();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleScriptViewToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            RedrawActionItems();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonReplace_Click(object sender, EventArgs e)
        {
            ReplacePaste();
        }

        #endregion

        #region Special Functions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearAllActionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure?\nThis operation cannot be undone", "Clear All Scripts", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                foreach(var script in AllActions)
                {
                    script._struct.References.Clear();
                    script._struct.SetData(new byte[4]);
                }

                actionList.SelectedIndex = 0;
            }
        }

        #endregion

        #region Drag and Drop

        private Timer clickTimer;
        private Point MousePoint;

        private void timer_Tick(object sender, EventArgs e)
        {
            if (MousePoint == Cursor.Position && subActionList.SelectedItems.Count == 1)
            {
                subActionList.DoDragDrop(subActionList.SelectedItems, DragDropEffects.Move);
                clickTimer.Stop();
            }
            MousePoint = Cursor.Position;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_MouseDown(object sender, MouseEventArgs e)
        {
            if (subActionList.SelectedItem == null) return;

            MousePoint = Cursor.Position;
            clickTimer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_DragDrop(object sender, DragEventArgs e)
        {
            Point point = subActionList.PointToClient(new Point(e.X, e.Y));
            int index = subActionList.IndexFromPoint(point);
            if (index < 0) index = subActionList.Items.Count - 1;

            List<object> data = new List<object>();

            foreach (var i in subActionList.SelectedItems)
                data.Add(i);

            foreach(var i in data)
                subActionList.Items.Remove(i);

            foreach(var i in data)
            {
                subActionList.Items.Insert(index, i);
            }

            SaveSelectedActionChanges();

            subActionList.SelectedIndex = index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subActionList_MouseUp(object sender, MouseEventArgs e)
        {
            clickTimer.Stop();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            SaveSelectedActionChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void actionList_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                e.DrawBackground();

                var brush = ApplicationSettings.SystemWindowTextColorBrush;

                var itemText = ((ListBox)sender).Items[e.Index].ToString();

                if (!itemText.StartsWith("Subroutine") && !itemText.StartsWith("Custom"))
                {
                    var indText = e.Index.ToString() + ".";

                    var indSize = TextRenderer.MeasureText(indText, e.Font);
                    var indexBound = new Rectangle(e.Bounds.X, e.Bounds.Y, indSize.Width, indSize.Height);
                    var textBound = new Rectangle(e.Bounds.X + indSize.Width, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);

                    e.Graphics.DrawString(indText, e.Font, ApplicationSettings.SystemGrayTextColorBrush, indexBound, StringFormat.GenericDefault);
                    e.Graphics.DrawString(itemText, e.Font, brush, textBound, StringFormat.GenericDefault);
                }
                else
                    e.Graphics.DrawString(itemText, e.Font, brush, e.Bounds, StringFormat.GenericDefault);

                e.DrawFocusRectangle();
            }
            catch
            {

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var f = FileIO.SaveFile("Text File (*.txt)|*.txt", "cmd_script.txt");

            if(f != null)
            {
                using (FileStream stream = new FileStream(f, FileMode.Create))
                using (StreamWriter w = new StreamWriter(stream))
                    foreach (var v in AllActions)
                    {
                        w.WriteLine($"[Symbol = \"{v.Symbol}\"]");
                        w.WriteLine($"[AnimOffset = 0x{v.AnimOffset}]");
                        w.WriteLine($"[AnimSize = 0x{v.AnimSize}]");
                        w.WriteLine($"[Flags = 0x{v.Flags.ToString("X")}]");

                        var scripts = GetScripts(v);
                        w.WriteLine(v.ToString() + "()");
                        w.WriteLine("{");
                        foreach (var s in scripts)
                            w.WriteLine($"\t{s.Serialize(this)};");
                        w.WriteLine("}");
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importFromTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = FileIO.OpenFile("Text File (*.txt)|*.txt");

            if (f != null)
            {
                using (FileStream stream = new FileStream(f, FileMode.Create))
                using (StreamReader w = new StreamReader(stream))
                {
                    var script = new Action();
                    while(!w.EndOfStream)
                    {
                        var line = w.ReadLine();
                        
                        // process attribute




                        // process script

                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void savePlayerRenderingFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importFigatreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (actionList.SelectedItem is Action a)
            {
                var f = FileIO.OpenFile(ApplicationSettings.HSDFileFilter);

                if (f != null)
                {
                    // check valid dat file
                    var file = new HSDRawFile(f);

                    // grab symbol
                    var symbol = file.Roots[0].Name;

                    // check if symbol exists and ok to overwrite
                    if(SymbolToAnimation.ContainsKey(symbol))
                    {
                        if(MessageBox.Show($"Symbol \"{symbol}\" already exists.\nIs it okay to overwrite?", "Overwrite Symbol", MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                            return;

                        SymbolToAnimation[symbol] = File.ReadAllBytes(f);
                    }
                    else
                        SymbolToAnimation.Add(symbol, File.ReadAllBytes(f));
                        
                    // set action symbol
                    a.Symbol = symbol;

                    // reselect action
                    LoadAnimation(symbol);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportFigatreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(actionList.SelectedItem is Action a)
            {
                if(a.Symbol != null && SymbolToAnimation.ContainsKey(a.Symbol))
                {
                    var f = FileIO.SaveFile(ApplicationSettings.HSDFileFilter, a.Symbol + ".dat");

                    if (f != null)
                        File.WriteAllBytes(f, SymbolToAnimation[a.Symbol]);
                }
            }
        }

        private PopoutJointAnimationEditor _animEditor = new PopoutJointAnimationEditor(false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void popoutEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _animEditor.Show();
            _animEditor.Visible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAnimationChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAnimation();
        }
    }
}

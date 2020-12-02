﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HSDRaw.MEX;
using HSDRaw.Common;
using HSDRaw.MEX.Characters;
using HSDRaw;
using System.IO;
using HSDRaw.Melee.Pl;
using HSDRawViewer.Tools;
using HSDRaw.Melee;
using System.Collections.Generic;
using HSDRawViewer.GUI.MEX.Tools;

namespace HSDRawViewer.GUI.MEX.Controls
{
    public partial class MEXFighterControl : UserControl, IMEXControl
    {
        /// <summary>
        /// 
        /// </summary>
        public MEX_Data MexData
        {
            get
            {
                var c = Parent;
                while (c != null && !(c is MexDataEditor)) c = c.Parent;
                if (c is MexDataEditor e) return e._data;
                return null;
            }
        }

        public MexDataEditor Editor
        {
            get
            {
                var c = Parent;
                while (c != null && !(c is MexDataEditor)) { c = c.Parent; Console.WriteLine(c.GetType()); }
                if (c is MexDataEditor e) return e;
                return null;
            }
        }

        public MEXFighterEntry SelectedEntry { get => fighterList.SelectedItem as MEXFighterEntry; }

        public int SelectedIndex { get => fighterList.SelectedIndex; }

        /// <summary>
        /// 
        /// </summary>
        public int NumberOfEntries
        {
            get => FighterEntries.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        public BindingList<MEXFighterEntry> FighterEntries = new BindingList<MEXFighterEntry>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetControlName()
        {
            return "Fighter";
        }

        /// <summary>
        /// 
        /// </summary>
        public MEXFighterControl()
        {
            InitializeComponent();

            fighterList.DataSource = FighterEntries;

            FighterEntries.ListChanged += (sender, args) =>
            {
                MEXConverter.internalIDValues.Clear();
                MEXConverter.internalIDValues.Add("None");
                MEXConverter.internalIDValues.AddRange(FighterEntries.Select(e => e.NameText));
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public void CheckEnable(MexDataEditor editor)
        {
            var sym = editor.GetSymbol("ftLoadCommonData");

            if (DummyBoneTable == null && sym != null && sym is ftLoadCommonData plco)
                LoadPlCO(plco);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void LoadData(MEX_Data data)
        {
            FighterEntries.Clear();
            for (int i = 0; i < data.MetaData.NumOfInternalIDs; i++)
            {
                FighterEntries.Add(new MEXFighterEntry().LoadData(data, i, MEXIdConverter.ToExternalID(i, data.MetaData.NumOfInternalIDs)));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SaveData(MEX_Data d)
        {
            d.MetaData.NumOfExternalIDs = NumberOfEntries;
            d.MetaData.NumOfInternalIDs = NumberOfEntries;
            d.FighterData.NameText.Array = new HSD_String[NumberOfEntries];
            d.FighterData.CharFiles.Array = new MEX_CharFileStrings[NumberOfEntries];
            d.FighterData.CostumeIDs.Array = new MEX_CostumeIDs[NumberOfEntries];
            d.FighterData.CostumeFileSymbols.Array = new MEX_CostumeFileSymbolTable[NumberOfEntries];
            d.FighterData.AnimFiles.Array = new HSD_String[NumberOfEntries];
            d.FighterData.AnimCount.Array = new MEX_AnimCount[NumberOfEntries];
            d.FighterData.InsigniaIDs.Array = new HSD_Byte[NumberOfEntries];
            d.FighterData.ResultAnimFiles.Array = new HSD_String[NumberOfEntries];
            d.FighterData.ResultScale.Array = new HSD_Float[NumberOfEntries];
            d.FighterData.VictoryThemeIDs.Array = new HSD_Int[NumberOfEntries];
            d.FighterData.FtDemo_SymbolNames.Array = new MEX_FtDemoSymbolNames[NumberOfEntries];
            d.FighterData.AnnouncerCalls.Array = new HSD_Int[NumberOfEntries];
            d.FighterData.SSMFileIDs.Array = new MEX_CharSSMFileID[NumberOfEntries];
            d.FighterData.EffectIDs.Array = new HSD_Byte[NumberOfEntries];
            d.FighterData.CostumePointers.Array = new MEX_CostumeRuntimePointers[NumberOfEntries];
            d.FighterData.DefineIDs.Array = new MEX_CharDefineIDs[NumberOfEntries];
            d.FighterData.WallJump.Array = new HSD_Byte[NumberOfEntries];
            d.FighterData.RstRuntime.Array = new MEX_RstRuntime[NumberOfEntries];
            d.FighterData.FighterItemLookup.Array = new MEX_ItemLookup[NumberOfEntries];
            //d.FighterData.FighterEffectLookup.Array = new MEX_EffectTypeLookup[0];
            d.FighterData.TargetTestStageLookups.Array = new HSD_UShort[NumberOfEntries];
            d.FighterData.FighterSongIDs.Array = new MEX_FighterSongID[NumberOfEntries];
            d.FighterData.VIFiles.Array = new HSD_String[0];
            d.FighterData.EndAdventureFiles.Array = new HSD_String[0];
            d.FighterData.EndAllStarFiles.Array = new HSD_String[0];
            d.FighterData.EndClassicFiles.Array = new HSD_String[0];
            d.FighterData.EndMovieFiles.Array = new HSD_String[0];

            d.FighterFunctions.OnLoad.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.OnDeath.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.OnUnknown.Array = new HSD_UInt[NumberOfEntries];

            if (d.MetaData.Flags.HasFlag(MexFlags.ContainMoveLogic))
                d.FighterFunctions.MoveLogic.Array = new HSDArrayAccessor<MEX_MoveLogic>[NumberOfEntries];
            else
                d.FighterFunctions.MoveLogicPointers = new HSDArrayAccessor<HSD_UInt>();

            d.FighterFunctions.SpecialN.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.SpecialNAir.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.SpecialHi.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.SpecialHiAir.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.SpecialLw.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.SpecialLwAir.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.SpecialS.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.SpecialSAir.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.OnAbsorb.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.onItemPickup.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.onMakeItemInvisible.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.onMakeItemVisible.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.onItemDrop.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.onItemCatch.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.onUnknownItemRelated.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.onUnknownCharacterModelFlags1.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.onUnknownCharacterModelFlags2.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.onHit.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.onUnknownEyeTextureRelated.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.onFrame.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.onActionStateChange.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.onRespawn.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.onModelRender.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.onShadowRender = new HSDArrayAccessor<HSD_UInt>();
            d.FighterFunctions.onUnknownMultijump = new HSDArrayAccessor<HSD_UInt>();
            d.FighterFunctions.onActionStateChangeWhileEyeTextureIsChanged.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.onTwoEntryTable = new HSDArrayAccessor<HSD_UInt>();
            d.FighterFunctions.onLand = new HSDArrayAccessor<HSD_UInt>();
            d.FighterFunctions.onExtRstAnim = new HSDArrayAccessor<HSD_UInt>();
            d.FighterFunctions.onIndexExtResultAnim = new HSDArrayAccessor<HSD_UInt>();
            d.FighterFunctions.DemoMoveLogic = new HSDArrayAccessor<HSD_UInt>();
            d.FighterFunctions.getTrailData = new HSDArrayAccessor<HSD_UInt>();
            d.FighterFunctions.onThrowFw = new HSDArrayAccessor<HSD_UInt>();
            d.FighterFunctions.onThrowBk = new HSDArrayAccessor<HSD_UInt>();
            d.FighterFunctions.onThrowHi = new HSDArrayAccessor<HSD_UInt>();
            d.FighterFunctions.onThrowLw = new HSDArrayAccessor<HSD_UInt>();

            d.FighterFunctions.onSmashDown.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.onSmashUp.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.onSmashForward.Array = new HSD_UInt[NumberOfEntries];

            d.FighterFunctions.enterFloat.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.enterSpecialDoubleJump.Array = new HSD_UInt[NumberOfEntries];
            d.FighterFunctions.enterTether.Array = new HSD_UInt[NumberOfEntries];

            d.KirbyData.CapFiles.Array = new MEX_KirbyCapFiles[0];
            d.KirbyData.KirbyCostumes.Array = new MEX_KirbyCostume[0];
            d.KirbyData.KirbyEffectIDs.Array = new HSD_Byte[0];
            d.KirbyFunctions.OnAbilityGain.Array = new HSD_UInt[0];
            d.KirbyFunctions.OnAbilityLose.Array = new HSD_UInt[0];
            d.KirbyFunctions.KirbySpecialN.Array = new HSD_UInt[0];
            d.KirbyFunctions.KirbySpecialNAir.Array = new HSD_UInt[0];
            d.KirbyFunctions.KirbyOnHit.Array = new HSD_UInt[0];
            d.KirbyFunctions.KirbyOnItemInit.Array = new HSD_UInt[0];

            // runtime fighter pointer struct
            d.FighterData._s.GetReference<HSDAccessor>(0x40)._s.Resize(NumberOfEntries * 8);

            // kirby runtimes
            d.KirbyData.CapFileRuntime._s = new HSDStruct(4 * NumberOfEntries);
            d.KirbyData.CostumeRuntime._s = new HSDStruct(4);
            d.KirbyData.CapFtCmdRuntime._s = new HSDStruct(4 * NumberOfEntries);
            d.KirbyData.MoveLogicRuntime._s = new HSDStruct(4 * NumberOfEntries);
            //d.KirbyData.KirbyNState = new HSDArrayAccessor<HSD_UShort>();
            //d.KirbyData.KirbyNStateAir = new HSDArrayAccessor<HSD_UShort>();

            // dump data
            int index = 0;
            foreach (var v in FighterEntries)
            {
                v.SaveData(d, index, MEXIdConverter.ToExternalID(index, NumberOfEntries));
                index++;
            }

            SavePlCo();
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void ResetDataBindings()
        {
            FighterEntries.ResetBindings();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="internalID"></param>
        public void RemoveFighterEntry(int internalID)
        {
            FighterEntries.RemoveAt(internalID);
        }

        /// <summary>
        /// Adds new entry to fighter list
        /// </summary>
        /// <param name="e"></param>
        /// <returns>internal ID</returns>
        public int AddEntry(MEXFighterEntry e)
        {
            FighterEntries.Insert(FighterEntries.Count - 6, e);
            return FighterEntries.Count - 6;
        }

        /// <summary>
        /// Checks if effect index is currently in use by fighter
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool EffectInUse(int index)
        {
            return FighterEntries.Any(e => e.EffectIndex == index || e.KirbyEffectID == index);
        }

        /// <summary>
        /// Checks if effect index is currently in use by fighter
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool ItemInUse(int index)
        {
            return FighterEntries.Any(e => e.MEXItems.Any(r=>r.Value == index));
        }

        /// <summary>
        /// Returns true if any fighter already uses given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool NameExists(string name)
        {
            foreach (var v in FighterEntries)
            {
                if (v.NameText.Equals(name))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if fighter at this index is an extended fighter
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsExtendedFighter(int index)
        {
            return (index >= 0x21 - 6 && index < FighterEntries.Count - 6);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsSpecialFighter(int index)
        {
            return (index >= FighterEntries.Count - 6);
        }



        #region Events
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void propertyGrid2_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void fighterPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            FighterEntries.ResetBindings();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cloneButton_Click(object sender, EventArgs e)
        {
            if (Editor.GetFile("PlCo.dat") == null)
            {
                MessageBox.Show("Please load PlCo.dat before cloning a fighter");
                return;
            }
            if (fighterList.SelectedItem is MEXFighterEntry me)
            {
                var clone = ObjectExtensions.Copy(me);
                // give unique name
                int clnIndex = 0;
                if (NameExists(clone.NameText))
                {
                    while (NameExists(clone.NameText + " " + clnIndex.ToString())) clnIndex++;
                    clone.NameText = clone.NameText + " " + clnIndex;
                }
                AddEntry(clone);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportFighter_Click(object sender, EventArgs e)
        {
            if (fighterList.SelectedItem is MEXFighterEntry fighter)
            {
                var f = FileIO.SaveFile("YAML (*.yaml)|*.yaml", fighter.NameText + ".yaml");
                if (f != null)
                {
                    FighterPackage package = new FighterPackage();

                    package.Fighter = fighter;

                    // add fighter items
                    foreach(var itemIndex in fighter.MEXItems)
                        package.Items.Add(Editor.ItemControl.GetItem(itemIndex.Value));

                    // add fighter effect
                    if(fighter.EffectIndex != -1)
                        package.Effect = Editor.EffectControl.Effects[fighter.EffectIndex];
                    if(fighter.KirbyEffectID != -1)
                        package.KirbyEffect = Editor.EffectControl.Effects[fighter.KirbyEffectID];
                    

                    package.Serialize(f);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importFighter_Click(object sender, EventArgs e)
        {
            if(Editor.GetFile("PlCo.dat") == null)
            {
                MessageBox.Show("Please load PlCo.dat before importing a fighter");
                return;
            }
            var f = FileIO.OpenFile("YAML (*.yaml)|*.yaml");
            if (f != null)
            {
                var package = FighterPackage.DeserializeFile(f);

                var fighter = package.Fighter;

                // load items
                if(package.Items != null)
                {
                    fighter.MEXItems = new HSD_UShort[package.Items.Count];
                    for(int i = 0; i < fighter.MEXItems.Length; i++)
                        fighter.MEXItems[i] = new HSD_UShort() { Value = (ushort)Editor.ItemControl.AddMEXItem(package.Items[i]) };
                }

                // load effects
                if(package.Effect != null)
                    fighter.EffectIndex = Editor.EffectControl.AddMEXEffectFile(package.Effect);

                // load kirby effect
                if (package.KirbyEffect != null)
                    fighter.KirbyEffectID = Editor.EffectControl.AddMEXEffectFile(package.KirbyEffect);

                // insert new fighter
                FighterEntries.Insert(FighterEntries.Count - 6, fighter);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteFighter_Click(object sender, EventArgs e)
        {
            var selected = fighterList.SelectedIndex;

            if (!IsExtendedFighter(selected))
                MessageBox.Show("Unable to delete base game fighters", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else 
            if(MessageBox.Show("Permanently delete this Fighter?", "Delete Fighter", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                DeleteFighter(SelectedEntry);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fighter"></param>
        public void DeleteFighter(MEXFighterEntry fighter)
        {
            // remove fighter entry
            FighterEntries.Remove(fighter);

            // remove items
            var it = fighter.MEXItems.Select(e => e.Value).ToList();
            it.Sort();
            it.Reverse();
            foreach (var i in it)
            {
                Editor.ItemControl.SaveRemoveMexItem(i);
            }

            // remove effect file
            bool removed = false;
            if (fighter.EffectIndex >= 0)
                removed = Editor.EffectControl.SafeRemoveEffectFile(fighter.EffectIndex);

            // adjust effect index if one above it was removed above ^
            if (fighter.KirbyEffectID >= 0)
                if(removed && fighter.KirbyEffectID > fighter.EffectIndex)
                    Editor.EffectControl.SafeRemoveEffectFile(fighter.KirbyEffectID - 1);
                else
                    Editor.EffectControl.SafeRemoveEffectFile(fighter.KirbyEffectID);

            MessageBox.Show($"Removed {fighter.NameText}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fighterList_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                e.DrawBackground();

                var brush = ApplicationSettings.SystemWindowTextColorBrush;

                if (IsExtendedFighter(e.Index))
                    brush = Brushes.DarkViolet;

                if (IsSpecialFighter(e.Index))
                    brush = ApplicationSettings.SystemWindowTextRedColorBrush;

                e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(),
                e.Font, brush, e.Bounds, StringFormat.GenericDefault);

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
        private void buttonCopyMoveLogic_Click(object sender, EventArgs e)
        {
            if (fighterList.SelectedItem is MEXFighterEntry fighter)
            {
                var moveLogic = fighter.Functions.MoveLogic;

                if (moveLogic == null)
                {
                    MessageBox.Show("This MxDt file does not contains move logic", "Nothing to copy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                var ftDataFile = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), fighter.FighterDataPath);

                SBM_FighterData fighterData = null;

                if (File.Exists(ftDataFile))
                    fighterData = new HSDRawFile(ftDataFile).Roots[0].Data as SBM_FighterData;

                StringBuilder table = new StringBuilder();

                int index = 341;
                foreach (var m in moveLogic)
                {
                    table.AppendLine($"\t// State: {index} - " + (fighterData != null && m.AnimationID != -1 && fighterData.FighterCommandTable.Commands[m.AnimationID].Name != null ? System.Text.RegularExpressions.Regex.Replace(fighterData.FighterCommandTable.Commands[m.AnimationID].Name.Replace("_figatree", ""), @"Ply.*_Share_ACTION_", "") : "Animation: " + m.AnimationID.ToString("X")));
                    index++;
                    table.AppendLine(string.Format(
                        "\t{{" +
                        "\n\t\t{0, -12}// AnimationID" +
                        "\n\t\t0x{1, -10}// StateFlags" +
                        "\n\t\t0x{2, -10}// AttackID" +
                        "\n\t\t0x{3, -10}// BitFlags" +
                        "\n\t\t0x{4, -10}// AnimationCallback" +
                        "\n\t\t0x{5, -10}// IASACallback" +
                        "\n\t\t0x{6, -10}// PhysicsCallback" +
                        "\n\t\t0x{7, -10}// CollisionCallback" +
                        "\n\t\t0x{8, -10}// CameraCallback" +
                        "\n\t}},",
                m.AnimationID + ",",
                m.StateFlags.ToString("X") + ",",
                m.AttackID.ToString("X") + ",",
                m.BitFlags.ToString("X") + ",",
                m.AnimationCallBack.ToString("X") + ",",
                m.IASACallBack.ToString("X") + ",",
                m.PhysicsCallback.ToString("X") + ",",
                m.CollisionCallback.ToString("X") + ",",
                m.CameraCallback.ToString("X") + ","
                ));
                }

                Clipboard.SetText(
                    @"__attribute__((used))
static struct MoveLogic move_logic[] = {
" + table.ToString() + @"}; ");

                MessageBox.Show("Move Logic Table Copied to Clipboard");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fighterList_SelectedIndexChanged(object sender, EventArgs e)
        {
            fighterPropertyGrid.SelectedObject = fighterList.SelectedItem;
            functionPropertyGrid.SelectedObject = (fighterList.SelectedItem as MEXFighterEntry).Functions;
            boneTablePropertyGrid.SelectedObject = (fighterList.SelectedItem as MEXFighterEntry).BoneTable;
        }

        #endregion

        
        private SBM_BoneLookupTable DummyBoneTable;
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ftData"></param>
        private void LoadPlCO(ftLoadCommonData ftData)
        {
            var tables = ftData.BoneTables.Array;
            var unktables = ftData.FighterTable.Array;

            if (tables.Length != FighterEntries.Count + 1)
                MessageBox.Show($"PlCo is missing entries\nExpected {FighterEntries.Count} Found {tables.Length - 1}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            int fIndex = 0;
            foreach(var fighter in FighterEntries)
            {
                if (fIndex < tables.Length)
                    fighter.BoneTable = tables[fIndex];
                else
                    fighter.BoneTable = new SBM_BoneLookupTable();

                if (fIndex < unktables.Length)
                    fighter.UnkTable = unktables[fIndex];
                else
                    fighter.UnkTable = new SBM_PlCoUnknownFighterTable();

                fIndex++;
            }

            DummyBoneTable = tables[tables.Length - 1];
        }

        /// <summary>
        /// 
        /// </summary>
        public void SavePlCo()
        {
            var PlCo = Editor.GetFile("PlCo.dat");

            if (PlCo == null)
                return;
            
            List<SBM_BoneLookupTable> tables = new List<SBM_BoneLookupTable>();

            foreach (var f in FighterEntries)
            {
                if (f.BoneTable != null)
                    tables.Add(f.BoneTable);
                else
                    tables.Add(new SBM_BoneLookupTable());
            }

            tables.Add(DummyBoneTable);

            if (PlCo.Roots[0].Data is ftLoadCommonData ftData)
            {
                ftData.BoneTables.Array = tables.ToArray();
                var ft = FighterEntries.Select(e => e.UnkTable).ToList();
                ft.Add(null);
                ftData.FighterTable.Array = ft.ToArray();
            }
        }
    }
}

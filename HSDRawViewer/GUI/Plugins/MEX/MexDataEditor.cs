﻿using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Melee.Mn;
using HSDRaw.MEX;
using HSDRaw.MEX.Characters;
using HSDRawViewer.Rendering;
using HSDRawViewer.Sound;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HSDRawViewer.GUI.Plugins.MEX
{
    public partial class MexDataEditor : DockContent, EditorBase, IDrawable
    {
        public MexDataEditor()
        {
            InitializeComponent();

            //GUITools.SetIconFromBitmap(this, Properties.Resources.doc_kabii);

            fighterList.DataSource = FighterEntries;

            JOBJManager.RenderBones = false;

            viewport = new ViewportControl();
            viewport.Dock = DockStyle.Fill;
            viewport.AnimationTrackEnabled = false;
            viewport.AddRenderer(this);
            viewport.EnableFloor = true;
            groupBox2.Controls.Add(viewport);
            viewport.RefreshSize();
            viewport.BringToFront();
            //viewport.Visible = false;

            musicDSPPlayer.ReplaceButtonVisible = false;

            menuPlaylistEditor.DoubleClickedNode += menuPlaylistEditor_DoubleClick;

            musicListEditor.DoubleClickedNode += musicListEditor_DoubleClicked;
            musicListEditor.ArrayUpdated += musicListEditor_ArrayUpdated;
            musicListEditor.EnablePropertyViewDescription = false;

            commonItemEditor.DisableAllControls();
            fighterItemEditor.DisableAllControls();
            pokemonItemEditor.DisableAllControls();
            stageItemEditor.DisableAllControls();

            commonItemEditor.TextOverrides.AddRange(DefaultItemNames.CommonItemNames);
            fighterItemEditor.TextOverrides.AddRange(DefaultItemNames.FighterItemNames);
            pokemonItemEditor.TextOverrides.AddRange(DefaultItemNames.PokemonItemNames);
            stageItemEditor.TextOverrides.AddRange(DefaultItemNames.StageItemNames);

            FighterEntries.ListChanged += (sender, args) =>
            {
                MEXConverter.internalIDValues.Clear();
                MEXConverter.internalIDValues.Add("None");
                MEXConverter.internalIDValues.AddRange(FighterEntries.Select(e=>e.NameText));
            };

            effectEditor.ArrayUpdated += (sender, args) =>
            {
                MEXConverter.effectValues.Clear();
                MEXConverter.effectValues.AddRange(Effects.Select(e=>e.FileName));
            };

            FormClosing += (sender, args) =>
            {
                JOBJManager.ClearRenderingCache();
                viewport.Dispose();
            };
        }

        public DockState DefaultDockState => DockState.Document;

        public Type[] SupportedTypes => new Type[] { typeof(MEX_Data) };

        public DataNode Node
        {
            get => _node; set
            {
                _node = value;
                LoadData();
            }
        }
        private DataNode _node;
        private MEX_Data _data { get { if (_node.Accessor is MEX_Data data) return data; else return null; } }

        private BindingList<MEXEntry> FighterEntries = new BindingList<MEXEntry>();

        public MEX_EffectEntry[] Effects { get; set; }
        public MEX_Effect[] MEX_Effects { get; set; }

        public MEX_CSSIconEntry[] Icons { get; set; }
        
        public HSD_String[] Music { get; set; }
        public MEXPlaylistEntry[] MenuPlaylist { get; set; }

        public MEX_Item[] ItemCommon { get; set; }
        public MEX_Item[] ItemFighter { get; set; }
        public MEX_Item[] ItemPokemon { get; set; }
        public MEX_Item[] ItemStage { get; set; }
        public MEX_Item[] ItemMEX { get; set; }

        private int MEXItemOffset = 0;

        public DrawOrder DrawOrder => DrawOrder.Last;

        private JOBJManager JOBJManager = new JOBJManager();

        private ViewportControl viewport;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void LoadData()
        {
            // Effects------------------------------------
            Effects = new MEX_EffectEntry[_data.EffectFiles.Length];
            for (int i = 0; i < Effects.Length; i++)
            {
                Effects[i] = new MEX_EffectEntry()
                {
                    FileName = _data.EffectFiles[i].FileName,
                    Symbol = _data.EffectFiles[i].Symbol,
                };
            }
            effectEditor.SetArrayFromProperty(this, "Effects");

            MEX_Effects = _data.MEXEffects.Array;
            mEXEffectEditor.SetArrayFromProperty(this, "MEX_Effects");


            // Fighters------------------------------------
            FighterEntries.Clear();
            for(int i = 0; i < _data.MetaData.NumOfInternalIDs; i++)
            {
                FighterEntries.Add(new MEXEntry().LoadData(_data, i, MEXIdConverter.ToExternalID(i, _data.MetaData.NumOfInternalIDs)));
            }


            // CSS------------------------------------
            Icons = new MEX_CSSIconEntry[_data.CSSIconData.Icons.Length];
            for(int i = 0; i < Icons.Length; i++)
            {
                Icons[i] = MEX_CSSIconEntry.FromIcon(_data.CSSIconData.Icons[i]);
            }
            cssIconEditor.SetArrayFromProperty(this, "Icons");


            // Music------------------------------------
            Music = _data.MusicTable.BackgroundMusicStrings.Array;
            musicListEditor.SetArrayFromProperty(this, "Music");

            MenuPlaylist = new MEXPlaylistEntry[_data.MusicTable.MenuPlayListCount];
            for (int i = 0; i < MenuPlaylist.Length; i++)
            {
                var e = _data.MusicTable.MenuPlaylist[i];
                MenuPlaylist[i] = new MEXPlaylistEntry()
                {
                    MusicID = e.HPSID,
                    PlayChance = e.ChanceToPlay.ToString() + "%"
                };
            }
            menuPlaylistEditor.SetArrayFromProperty(this, "MenuPlaylist");


            // Items------------------------------------
            MEXItemOffset = 0;

            ItemCommon = _data.ItemTable.CommonItems.Array;
            commonItemEditor.SetArrayFromProperty(this, "ItemCommon");
            MEXItemOffset += ItemCommon.Length;

            ItemFighter = _data.ItemTable.FighterItems.Array;
            fighterItemEditor.SetArrayFromProperty(this, "ItemFighter");
            fighterItemEditor.ItemIndexOffset = MEXItemOffset;
            MEXItemOffset += ItemFighter.Length;

            ItemPokemon = _data.ItemTable.Pokemon.Array;
            pokemonItemEditor.SetArrayFromProperty(this, "ItemPokemon");
            pokemonItemEditor.ItemIndexOffset = MEXItemOffset;
            MEXItemOffset += ItemPokemon.Length;

            ItemStage = _data.ItemTable.Stages.Array;
            stageItemEditor.SetArrayFromProperty(this, "ItemStage");
            stageItemEditor.ItemIndexOffset = MEXItemOffset;
            MEXItemOffset += ItemStage.Length;

            ItemMEX = _data.ItemTable.MEXItems.Array;
            mexItemEditor.SetArrayFromProperty(this, "ItemMEX");
            mexItemEditor.ItemIndexOffset = MEXItemOffset;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveFighterData()
        {
            var d = _data;
            int index = 0;
            d.MetaData.NumOfExternalIDs = FighterEntries.Count;
            d.MetaData.NumOfInternalIDs = FighterEntries.Count;
            d.FighterData.NameText.Array = new HSD_String[0];
            d.FighterData.CharFiles.Array = new MEX_CharFileStrings[0];
            d.FighterData.CostumeIDs.Array = new MEX_CostumeIDs[0];
            d.FighterData.CostumeFileSymbols.Array = new MEX_CostumeFileSymbolTable[0];
            d.FighterData.AnimFiles.Array = new HSD_String[0];
            d.FighterData.AnimCount.Array = new MEX_AnimCount[0];
            d.FighterData.InsigniaIDs.Array = new HSD_Byte[0];
            d.FighterData.ResultAnimFiles.Array = new HSD_String[0];
            d.FighterData.ResultScale.Array = new HSD_Float[0];
            d.FighterData.VictoryThemeIDs.Array = new HSD_Int[0];
            d.FighterData.FtDemo_SymbolNames.Array = new MEX_FtDemoSymbolNames[0];
            d.FighterData.AnnouncerCalls.Array = new HSD_Int[0];
            d.FighterData.SSMFileIDs.Array = new MEX_CharSSMFileID[0];
            d.FighterData.EffectIDs.Array = new HSD_Byte[0];
            d.FighterData.CostumePointers.Array = new MEX_CostumeRuntimePointers[0];
            d.FighterData.DefineIDs.Array = new MEX_CharDefineIDs[0];
            d.FighterData.WallJump.Array = new HSD_Byte[0];
            d.FighterData.RstRuntime.Array = new MEX_RstRuntime[0];
            d.FighterData.FighterItems.Array = new MEX_FighterItem[0];

            d.FighterFunctions.OnLoad.Array = new HSD_UInt[0];
            d.FighterFunctions.OnDeath.Array = new HSD_UInt[0];
            d.FighterFunctions.OnUnknown.Array = new HSD_UInt[0];
            d.FighterFunctions.MoveLogic.Array = new HSDArrayAccessor<MEX_MoveLogic>[0];
            d.FighterFunctions.SpecialN.Array = new HSD_UInt[0];
            d.FighterFunctions.SpecialNAir.Array = new HSD_UInt[0];
            d.FighterFunctions.SpecialHi.Array = new HSD_UInt[0];
            d.FighterFunctions.SpecialHiAir.Array = new HSD_UInt[0];
            d.FighterFunctions.SpecialLw.Array = new HSD_UInt[0];
            d.FighterFunctions.SpecialLwAir.Array = new HSD_UInt[0];
            d.FighterFunctions.SpecialS.Array = new HSD_UInt[0];
            d.FighterFunctions.SpecialSAir.Array = new HSD_UInt[0];
            d.FighterFunctions.OnAbsorb.Array = new HSD_UInt[0];
            d.FighterFunctions.onItemPickup.Array = new HSD_UInt[0];
            d.FighterFunctions.onMakeItemInvisible.Array = new HSD_UInt[0];
            d.FighterFunctions.onMakeItemVisible.Array = new HSD_UInt[0];
            d.FighterFunctions.onItemDrop.Array = new HSD_UInt[0];
            d.FighterFunctions.onItemCatch.Array = new HSD_UInt[0];
            d.FighterFunctions.onUnknownItemRelated.Array = new HSD_UInt[0];
            d.FighterFunctions.onUnknownCharacterModelFlags1.Array = new HSD_UInt[0];
            d.FighterFunctions.onUnknownCharacterModelFlags2.Array = new HSD_UInt[0];
            d.FighterFunctions.onHit.Array = new HSD_UInt[0];
            d.FighterFunctions.onUnknownEyeTextureRelated.Array = new HSD_UInt[0];
            d.FighterFunctions.onFrame.Array = new HSD_UInt[0];
            d.FighterFunctions.onActionStateChange.Array = new HSD_UInt[0];
            d.FighterFunctions.onRespawn.Array = new HSD_UInt[0];
            d.FighterFunctions.onModelRender.Array = new HSD_UInt[0];
            d.FighterFunctions.onShadowRender.Array = new HSD_UInt[0];
            d.FighterFunctions.onUnknownMultijump.Array = new HSD_UInt[0];
            d.FighterFunctions.onActionStateChangeWhileEyeTextureIsChanged.Array = new HSD_UInt[0];
            d.FighterFunctions.onTwoEntryTable.Array = new HSD_UInt[0];
            d.FighterFunctions.onLand.Array = new HSD_UInt[0];

            d.FighterFunctions.onSmashDown.Array = new HSD_UInt[0];
            d.FighterFunctions.onSmashUp.Array = new HSD_UInt[0];
            d.FighterFunctions.onSmashForward.Array = new HSD_UInt[0];

            d.FighterFunctions.enterFloat.Array = new HSD_UInt[0];
            d.FighterFunctions.enterSpecialDoubleJump.Array = new HSD_UInt[0];
            d.FighterFunctions.enterTether.Array = new HSD_UInt[0];

            d.KirbyTable.CapFiles.Array = new MEX_KirbyCapFiles[0];
            d.KirbyTable.KirbyCostumes.Array = new MEX_KirbyCostume[0];
            d.KirbyTable.EffectIDs.Array = new HSD_Byte[0];
            d.KirbyTable.KirbyHatFunctions.Array = new MEX_KirbyHatLoad[0];
            d.KirbyTable.KirbySpecialN.Array = new HSD_UInt[0];
            d.KirbyTable.KirbySpecialNAir.Array = new HSD_UInt[0];
            d.KirbyTable.KirbyOnHit.Array = new HSD_UInt[0];
            d.KirbyTable.KirbyOnItemInit.Array = new HSD_UInt[0];

            // runtime fighter pointer struct
            d.FighterData._s.GetReference<HSDAccessor>(0x40)._s.Resize(FighterEntries.Count * 8);

            // kirby runtimes
            d.KirbyTable.CapFileRuntime._s = new HSDStruct(4 * FighterEntries.Count);
            d.KirbyTable.CostumeRuntime._s = new HSDStruct(4);
            
            // dump data
            foreach (var v in FighterEntries)
            {
                v.SaveData(d, index, MEXIdConverter.ToExternalID(index, FighterEntries.Count));
                index++;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void SaveEffectData()
        {
            _data.MetaData.NumOfEffects = Effects.Length;
            _data.EffectFiles = new HSDArrayAccessor<MEX_EffectFiles>();
            foreach(var v in Effects)
            {
                _data.EffectFiles.Add(new MEX_EffectFiles()
                {
                    FileName = v.FileName,
                    Symbol = v.Symbol
                });
            }

            _data.MEXEffects.Array = MEX_Effects;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveMusicData()
        {
            _data.MetaData.NumOfMusic = Music.Length;
            _data.MusicTable.BackgroundMusicStrings.Array = new HSD_String[0];
            foreach (var v in Music)
                _data.MusicTable.BackgroundMusicStrings.Add(v);

            _data.MusicTable.MenuPlaylist.Array = new MEX_MenuPlaylistItem[0];
            _data.MusicTable.MenuPlayListCount = MenuPlaylist.Length;
            foreach (var v in MenuPlaylist)
            {
                _data.MusicTable.MenuPlaylist.Add(new MEX_MenuPlaylistItem()
                {
                    HPSID = (ushort)v.MusicID,
                    ChanceToPlay = v.PlayChanceValue
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveIconData()
        {
            _data.MetaData.NumOfCSSIcons = Icons.Length;
            MEX_CSSIcon[] ico = new MEX_CSSIcon[Icons.Length];
            for (int i = 0; i < ico.Length; i++)
                ico[i] = Icons[i].ToIcon();
            _data.CSSIconData.Icons = ico;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveItemData()
        {
            _data.ItemTable.CommonItems.Array = ItemCommon;
            _data.ItemTable.FighterItems.Array = ItemFighter;
            _data.ItemTable.Pokemon.Array = ItemPokemon;
            _data.ItemTable.Stages.Array = ItemStage;
            _data.ItemTable.MEXItems.Array = ItemMEX;
            _data.ItemTable._s.GetCreateReference<HSDAccessor>(0x14)._s.Resize(Math.Max(4, ItemMEX.Length * 4));
        }

        #region Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fighterList_SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = fighterList.SelectedItem;
            propertyGrid2.SelectedObject = (fighterList.SelectedItem as MEXEntry).Functions;
        }

        #endregion

        #region Save Button GUI
        private void saveAllChangesButton_Click(object sender, EventArgs e)
        {
            SaveFighterData();
            SaveItemData();
            SaveEffectData();
            SaveIconData();
            SaveMusicData();
        }
        
        private void saveMusicButton_Click(object sender, EventArgs e)
        {
            SaveMusicData();
        }

        private void saveEffectButton_Click(object sender, EventArgs e)
        {
            SaveEffectData();
        }

        private void buttonSaveCSS_Click(object sender, EventArgs e)
        {
            SaveIconData();
        }

        private void saveItemButton_Click(object sender, EventArgs e)
        {
            SaveItemData();
        }

        #endregion


        #region IconRendering
        private static Color IconColor = Color.FromArgb(128, 255, 0, 0);

        private static Color SelectedIconColor = Color.FromArgb(128, 255, 255, 0);

        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            JOBJManager.Render(cam);

            foreach(var i in Icons)
            {
                if (cssIconEditor.SelectedObject == (object)i)
                    DrawShape.DrawRectangle(i.X1, i.Y1, i.X2, i.Y2, SelectedIconColor);
                else
                    DrawShape.DrawRectangle(i.X1, i.Y1, i.X2, i.Y2, IconColor);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadModel(HSD_JOBJ jobj)
        {
            JOBJManager.ClearRenderingCache();
            JOBJManager.SetJOBJ(jobj);
            if (!viewport.Visible)
                viewport.Visible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLoadPlSl_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);

            if (f != null)
            {
                var hsd = new HSDRawFile(f);
                if (hsd.Roots[0].Data is SBM_SelectChrDataTable tab)
                {
                    LoadModel(tab.MenuModel);
                    JOBJManager.SetAnimJoint(tab.MenuAnimation);
                    JOBJManager.Frame = 600;
                }
            }
        }
        #endregion

        #region Fighter Controls

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cloneButton_Click(object sender, EventArgs e)
        {
            if(fighterList.SelectedItem is MEXEntry me)
            {
                var clone = ObjectExtensions.Copy(me);
                // give unique name
                int clnIndex = 0;
                if (NameExists(clone.NameText))
                {
                    while (NameExists(clone.NameText + " " + clnIndex.ToString())) clnIndex++;
                    clone.NameText = clone.NameText + " " + clnIndex;
                }
                FighterEntries.Insert(FighterEntries.Count - 6, clone);
            }
        }

        /// <summary>
        /// Returns true if any fighter already uses given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool NameExists(string name)
        {
            foreach(var v in FighterEntries)
            {
                if (v.NameText.Equals(name))
                    return true;
            }
            return false;
        }

        private void saveFightersButton_Click(object sender, EventArgs e)
        {
            SaveFighterData();
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            FighterEntries.ResetBindings();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if(fighterList.SelectedItem is MEXEntry mex)
            {
                var f = Tools.FileIO.SaveFile("YAML (*.yaml)|*.yaml", mex.NameText + ".yaml");
                if (f != null)
                {
                    var builder = new SerializerBuilder();
                    builder.WithNamingConvention(CamelCaseNamingConvention.Instance);
                    builder.WithTypeInspector(inspector => new MEXTypeInspector(inspector));

                    using (StreamWriter writer = File.CreateText(f))
                    {
                        builder.Build().Serialize(writer, fighterList.SelectedItem);
                    }

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile("YAML (*.yaml)|*.yaml");
            if (f != null)
            {
                var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeInspector(inspector => new MEXTypeInspector(inspector))
                .Build();

                var me = deserializer.Deserialize<MEXEntry>(File.ReadAllText(f));

                FighterEntries.Insert(FighterEntries.Count - 6, ObjectExtensions.Copy(me));
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
            if (IsExtendedFighter(selected))
            {
                FighterEntries.RemoveAt(selected);
                return;
            }
            MessageBox.Show("Unable to delete base game fighters", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// Returns true if fighter at this index is an extended fighter
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool IsExtendedFighter(int index)
        {
            return (index >= 0x21 - 6 && index < FighterEntries.Count - 6);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool IsSpecialFighter(int index)
        {
            return (index >= FighterEntries.Count - 6);
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
                    brush = Brushes.DarkRed;

                e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(),
                e.Font, brush, e.Bounds, StringFormat.GenericDefault);

                e.DrawFocusRectangle();
            }
            catch
            {

            }
        }

        #endregion

        #region Music

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        private void PlayMusicFromName(string str)
        {
            var path = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), $"audio\\{str}");
            
            musicDSPPlayer.DSP = null;
            if (File.Exists(path))
            {
                musicDSPPlayer.SoundName = str;
                var dsp = new Sound.DSP();
                dsp.FromFile(path);
                musicDSPPlayer.DSP = dsp;
                musicDSPPlayer.PlaySound();
            }
            else
            {
                MessageBox.Show("Could not find sound \"" + str + "\"", "Sound Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void musicListEditor_DoubleClicked(object sender, EventArgs e)
        {
            if(musicListEditor.SelectedObject is HSD_String str)
            {
                PlayMusicFromName(str.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuPlaylistEditor_DoubleClick(object sender, EventArgs e)
        {
            if (menuPlaylistEditor.SelectedObject is MEXPlaylistEntry str)
            {
                PlayMusicFromName(Music[str.MusicID].Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void musicListEditor_ArrayUpdated(object sender, EventArgs e)
        {
            var oldValues = MEXConverter.musicIDValues.ToArray();
            MEXConverter.musicIDValues.Clear();
            MEXConverter.musicIDValues.AddRange(Music.Select(r => r.Value));

            if(oldValues != null && oldValues.Length == Music.Length)
            {
                for(int i = 0; i < oldValues.Length; i++)
                {
                    if(oldValues[i] != Music[i].Value)
                    {
                        var path = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), $"audio\\{oldValues[i]}");
                        var newpath = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), $"audio\\{Music[i].Value}");
                        
                        if (File.Exists(path) && !File.Exists(newpath))
                        {
                            if(MessageBox.Show($"Rename {oldValues[i]} to {Music[i].Value}?", "Rename File", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                File.Move(path, newpath);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createHPSButton_Click(object sender, EventArgs e)
        {
            var fs = Tools.FileIO.OpenFiles(DSP.SupportedImportFilter);

            if(fs != null)
            {
                var audioPath = Path.Combine(Path.GetDirectoryName(MainForm.Instance.FilePath), "audio\\");

                if (!Directory.Exists(audioPath))
                    Directory.CreateDirectory(audioPath);

                foreach(var f in fs)
                {
                    var dsp = new DSP();
                    dsp.FromFile(f);
                    HPS.SaveDSPAsHPS(dsp, Path.Combine(audioPath, Path.GetFileNameWithoutExtension(f) + ".hps"));
                    var newHPSName = Path.GetFileNameWithoutExtension(f) + ".hps";
                    foreach (var v in Music)
                        if (v.Value.Equals(newHPSName))
                            return;
                    musicListEditor.AddItem(new HSD_String() { Value = newHPSName });
                }
            }
        }

        #endregion

        #region Items

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mexItemCloneButton_Click(object sender, EventArgs e)
        {
            if(itemTabs.SelectedIndex == 0)
                mexItemEditor.AddItem(commonItemEditor.SelectedObject);
            if (itemTabs.SelectedIndex == 1)
                mexItemEditor.AddItem(fighterItemEditor.SelectedObject);
            if (itemTabs.SelectedIndex == 2)
                mexItemEditor.AddItem(pokemonItemEditor.SelectedObject);
            if (itemTabs.SelectedIndex == 3)
                mexItemEditor.AddItem(stageItemEditor.SelectedObject);
        }

        #endregion
    }
}
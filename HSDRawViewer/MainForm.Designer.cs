﻿namespace HSDRawViewer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsUnoptimizedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromDATFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.trimExcessDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iSOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadISOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.saveISOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveISOAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.viewFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.closeISOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertyViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aJToolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sSMEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sEMEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAudioPlaybackDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.LocationLabel = new System.Windows.Forms.Label();
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.showHideButton = new System.Windows.Forms.Button();
            this.nodeBox = new System.Windows.Forms.GroupBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.isoSaveProgressBar = new System.Windows.Forms.ProgressBar();
            this.isoSaveLabel = new System.Windows.Forms.Label();
            this.isoSaveProgessPanel = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.nodeBox.SuspendLayout();
            this.isoSaveProgessPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.HideSelection = false;
            this.treeView1.Indent = 16;
            this.treeView1.ItemHeight = 24;
            this.treeView1.LabelEdit = true;
            this.treeView1.Location = new System.Drawing.Point(4, 19);
            this.treeView1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(259, 429);
            this.treeView1.TabIndex = 0;
            this.treeView1.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView1_BeforeLabelEdit);
            this.treeView1.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView1_AfterLabelEdit);
            this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.treeView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.iSOToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(939, 30);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem1,
            this.saveToolStripMenuItem,
            this.saveAsUnoptimizedToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 26);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem1
            // 
            this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            this.saveToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem1.Size = new System.Drawing.Size(242, 26);
            this.saveToolStripMenuItem1.Text = "Save";
            this.saveToolStripMenuItem1.Click += new System.EventHandler(this.saveToolStripMenuItem1_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.saveToolStripMenuItem.Text = "Save As";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsUnoptimizedToolStripMenuItem
            // 
            this.saveAsUnoptimizedToolStripMenuItem.Name = "saveAsUnoptimizedToolStripMenuItem";
            this.saveAsUnoptimizedToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.saveAsUnoptimizedToolStripMenuItem.Text = "Save As (No Optimize)";
            this.saveAsUnoptimizedToolStripMenuItem.Click += new System.EventHandler(this.saveAsUnoptimizedToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNodeToolStripMenuItem,
            this.toolStripSeparator2,
            this.trimExcessDataToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(49, 26);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // addNodeToolStripMenuItem
            // 
            this.addNodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fromFileToolStripMenuItem,
            this.fromDATFileToolStripMenuItem,
            this.fromTypeToolStripMenuItem});
            this.addNodeToolStripMenuItem.Name = "addNodeToolStripMenuItem";
            this.addNodeToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.addNodeToolStripMenuItem.Text = "Add Node";
            // 
            // fromFileToolStripMenuItem
            // 
            this.fromFileToolStripMenuItem.Name = "fromFileToolStripMenuItem";
            this.fromFileToolStripMenuItem.Size = new System.Drawing.Size(185, 26);
            this.fromFileToolStripMenuItem.Text = "From File";
            this.fromFileToolStripMenuItem.Click += new System.EventHandler(this.addRootFromFileToolStripMenuItem1_Click);
            // 
            // fromDATFileToolStripMenuItem
            // 
            this.fromDATFileToolStripMenuItem.Name = "fromDATFileToolStripMenuItem";
            this.fromDATFileToolStripMenuItem.Size = new System.Drawing.Size(185, 26);
            this.fromDATFileToolStripMenuItem.Text = "From DAT File";
            this.fromDATFileToolStripMenuItem.Click += new System.EventHandler(this.addRootFromFileToolStripMenuItem_Click);
            // 
            // fromTypeToolStripMenuItem
            // 
            this.fromTypeToolStripMenuItem.Name = "fromTypeToolStripMenuItem";
            this.fromTypeToolStripMenuItem.Size = new System.Drawing.Size(185, 26);
            this.fromTypeToolStripMenuItem.Text = "From Type";
            this.fromTypeToolStripMenuItem.Click += new System.EventHandler(this.addRootFromTypeToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(178, 6);
            // 
            // trimExcessDataToolStripMenuItem
            // 
            this.trimExcessDataToolStripMenuItem.Name = "trimExcessDataToolStripMenuItem";
            this.trimExcessDataToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.trimExcessDataToolStripMenuItem.Text = "Trim/Fix Data";
            this.trimExcessDataToolStripMenuItem.ToolTipText = "Removes garbage and unused data as well as aligns certain structures";
            this.trimExcessDataToolStripMenuItem.Click += new System.EventHandler(this.trimExcessDataToolStripMenuItem_Click);
            // 
            // iSOToolStripMenuItem
            // 
            this.iSOToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadISOToolStripMenuItem,
            this.toolStripSeparator4,
            this.saveISOToolStripMenuItem,
            this.saveISOAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.viewFilesToolStripMenuItem,
            this.toolStripSeparator3,
            this.closeISOToolStripMenuItem});
            this.iSOToolStripMenuItem.Name = "iSOToolStripMenuItem";
            this.iSOToolStripMenuItem.Size = new System.Drawing.Size(46, 26);
            this.iSOToolStripMenuItem.Text = "ISO";
            // 
            // loadISOToolStripMenuItem
            // 
            this.loadISOToolStripMenuItem.Name = "loadISOToolStripMenuItem";
            this.loadISOToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.loadISOToolStripMenuItem.Text = "Load ISO";
            this.loadISOToolStripMenuItem.Click += new System.EventHandler(this.loadISOToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(221, 6);
            // 
            // saveISOToolStripMenuItem
            // 
            this.saveISOToolStripMenuItem.Enabled = false;
            this.saveISOToolStripMenuItem.Name = "saveISOToolStripMenuItem";
            this.saveISOToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.saveISOToolStripMenuItem.Text = "Save ISO";
            this.saveISOToolStripMenuItem.Click += new System.EventHandler(this.saveISOToolStripMenuItem_Click);
            // 
            // saveISOAsToolStripMenuItem
            // 
            this.saveISOAsToolStripMenuItem.Enabled = false;
            this.saveISOAsToolStripMenuItem.Name = "saveISOAsToolStripMenuItem";
            this.saveISOAsToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.saveISOAsToolStripMenuItem.Text = "Save ISO As";
            this.saveISOAsToolStripMenuItem.Click += new System.EventHandler(this.saveISOAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(221, 6);
            // 
            // viewFilesToolStripMenuItem
            // 
            this.viewFilesToolStripMenuItem.Enabled = false;
            this.viewFilesToolStripMenuItem.Name = "viewFilesToolStripMenuItem";
            this.viewFilesToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.viewFilesToolStripMenuItem.Text = "View File Tree";
            this.viewFilesToolStripMenuItem.Click += new System.EventHandler(this.viewFilesToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(221, 6);
            // 
            // closeISOToolStripMenuItem
            // 
            this.closeISOToolStripMenuItem.Enabled = false;
            this.closeISOToolStripMenuItem.Name = "closeISOToolStripMenuItem";
            this.closeISOToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.closeISOToolStripMenuItem.Text = "Close ISO";
            this.closeISOToolStripMenuItem.Click += new System.EventHandler(this.closeISOToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.propertyViewToolStripMenuItem,
            this.viewportToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(55, 26);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // propertyViewToolStripMenuItem
            // 
            this.propertyViewToolStripMenuItem.Checked = true;
            this.propertyViewToolStripMenuItem.CheckOnClick = true;
            this.propertyViewToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.propertyViewToolStripMenuItem.Name = "propertyViewToolStripMenuItem";
            this.propertyViewToolStripMenuItem.Size = new System.Drawing.Size(184, 26);
            this.propertyViewToolStripMenuItem.Text = "Property View";
            this.propertyViewToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.propertyViewToolStripMenuItem_CheckStateChanged);
            // 
            // viewportToolStripMenuItem
            // 
            this.viewportToolStripMenuItem.Checked = true;
            this.viewportToolStripMenuItem.CheckOnClick = true;
            this.viewportToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewportToolStripMenuItem.Name = "viewportToolStripMenuItem";
            this.viewportToolStripMenuItem.Size = new System.Drawing.Size(184, 26);
            this.viewportToolStripMenuItem.Text = "Viewport";
            this.viewportToolStripMenuItem.CheckedChanged += new System.EventHandler(this.viewportToolStripMenuItem_CheckedChanged);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aJToolToolStripMenuItem,
            this.sSMEditorToolStripMenuItem,
            this.sEMEditorToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(58, 26);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // aJToolToolStripMenuItem
            // 
            this.aJToolToolStripMenuItem.Name = "aJToolToolStripMenuItem";
            this.aJToolToolStripMenuItem.Size = new System.Drawing.Size(165, 26);
            this.aJToolToolStripMenuItem.Text = "AJ Tool";
            this.aJToolToolStripMenuItem.Click += new System.EventHandler(this.aJToolToolStripMenuItem_Click);
            // 
            // sSMEditorToolStripMenuItem
            // 
            this.sSMEditorToolStripMenuItem.Name = "sSMEditorToolStripMenuItem";
            this.sSMEditorToolStripMenuItem.Size = new System.Drawing.Size(165, 26);
            this.sSMEditorToolStripMenuItem.Text = "SSM Editor";
            this.sSMEditorToolStripMenuItem.Click += new System.EventHandler(this.sSMEditorToolStripMenuItem_Click);
            // 
            // sEMEditorToolStripMenuItem
            // 
            this.sEMEditorToolStripMenuItem.Name = "sEMEditorToolStripMenuItem";
            this.sEMEditorToolStripMenuItem.Size = new System.Drawing.Size(165, 26);
            this.sEMEditorToolStripMenuItem.Text = "SEM Editor";
            this.sEMEditorToolStripMenuItem.Click += new System.EventHandler(this.sEMEditorToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAudioPlaybackDeviceToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(75, 26);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // selectAudioPlaybackDeviceToolStripMenuItem
            // 
            this.selectAudioPlaybackDeviceToolStripMenuItem.Name = "selectAudioPlaybackDeviceToolStripMenuItem";
            this.selectAudioPlaybackDeviceToolStripMenuItem.Size = new System.Drawing.Size(287, 26);
            this.selectAudioPlaybackDeviceToolStripMenuItem.Text = "Select Audio Playback Device";
            this.selectAudioPlaybackDeviceToolStripMenuItem.Click += new System.EventHandler(this.selectAudioPlaybackDeviceToolStripMenuItem_Click);
            // 
            // LocationLabel
            // 
            this.LocationLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.LocationLabel.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LocationLabel.Location = new System.Drawing.Point(0, 71);
            this.LocationLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LocationLabel.Name = "LocationLabel";
            this.LocationLabel.Size = new System.Drawing.Size(939, 20);
            this.LocationLabel.TabIndex = 5;
            this.LocationLabel.Text = "Location:";
            // 
            // dockPanel
            // 
            this.dockPanel.ActiveAutoHideContent = null;
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dockPanel.Location = new System.Drawing.Point(288, 91);
            this.dockPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Size = new System.Drawing.Size(651, 452);
            this.dockPanel.TabIndex = 6;
            // 
            // showHideButton
            // 
            this.showHideButton.BackColor = System.Drawing.SystemColors.Control;
            this.showHideButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.showHideButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.showHideButton.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showHideButton.Location = new System.Drawing.Point(271, 91);
            this.showHideButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.showHideButton.Name = "showHideButton";
            this.showHideButton.Size = new System.Drawing.Size(17, 452);
            this.showHideButton.TabIndex = 10;
            this.showHideButton.Text = "<";
            this.showHideButton.UseVisualStyleBackColor = false;
            this.showHideButton.Click += new System.EventHandler(this.showHideButton_Click);
            // 
            // nodeBox
            // 
            this.nodeBox.Controls.Add(this.treeView1);
            this.nodeBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.nodeBox.Location = new System.Drawing.Point(0, 91);
            this.nodeBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nodeBox.Name = "nodeBox";
            this.nodeBox.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nodeBox.Size = new System.Drawing.Size(267, 452);
            this.nodeBox.TabIndex = 11;
            this.nodeBox.TabStop = false;
            this.nodeBox.Text = "Nodes";
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(267, 91);
            this.splitter1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(4, 452);
            this.splitter1.TabIndex = 12;
            this.splitter1.TabStop = false;
            // 
            // isoSaveProgressBar
            // 
            this.isoSaveProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.isoSaveProgressBar.Location = new System.Drawing.Point(97, 4);
            this.isoSaveProgressBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.isoSaveProgressBar.Name = "isoSaveProgressBar";
            this.isoSaveProgressBar.Size = new System.Drawing.Size(837, 28);
            this.isoSaveProgressBar.TabIndex = 13;
            // 
            // isoSaveLabel
            // 
            this.isoSaveLabel.AutoSize = true;
            this.isoSaveLabel.Location = new System.Drawing.Point(7, 10);
            this.isoSaveLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.isoSaveLabel.Name = "isoSaveLabel";
            this.isoSaveLabel.Size = new System.Drawing.Size(82, 17);
            this.isoSaveLabel.TabIndex = 14;
            this.isoSaveLabel.Text = "Saving ISO:";
            // 
            // isoSaveProgessPanel
            // 
            this.isoSaveProgessPanel.Controls.Add(this.isoSaveLabel);
            this.isoSaveProgessPanel.Controls.Add(this.isoSaveProgressBar);
            this.isoSaveProgessPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.isoSaveProgessPanel.Location = new System.Drawing.Point(0, 30);
            this.isoSaveProgessPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.isoSaveProgessPanel.Name = "isoSaveProgessPanel";
            this.isoSaveProgessPanel.Size = new System.Drawing.Size(939, 41);
            this.isoSaveProgessPanel.TabIndex = 17;
            this.isoSaveProgessPanel.Visible = false;
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(939, 543);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.showHideButton);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.nodeBox);
            this.Controls.Add(this.LocationLabel);
            this.Controls.Add(this.isoSaveProgessPanel);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MainForm";
            this.TabText = "Hal DAT Browser";
            this.Text = "HSDraw";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.nodeBox.ResumeLayout(false);
            this.isoSaveProgessPanel.ResumeLayout(false);
            this.isoSaveProgessPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.Label LocationLabel;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertyViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewportToolStripMenuItem;
        private System.Windows.Forms.Button showHideButton;
        private System.Windows.Forms.GroupBox nodeBox;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aJToolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsUnoptimizedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sSMEditorToolStripMenuItem;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem sEMEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAudioPlaybackDeviceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trimExcessDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iSOToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadISOToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem viewFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromDATFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromTypeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem saveISOToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveISOAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeISOToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ProgressBar isoSaveProgressBar;
        private System.Windows.Forms.Label isoSaveLabel;
        private System.Windows.Forms.Panel isoSaveProgessPanel;
    }
}


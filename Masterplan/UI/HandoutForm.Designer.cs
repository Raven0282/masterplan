﻿namespace Masterplan.UI
{
	partial class HandoutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HandoutForm));
            this.CloseBtn = new System.Windows.Forms.Button();
            this.Splitter = new System.Windows.Forms.SplitContainer();
            this.ItemSplitter = new System.Windows.Forms.SplitContainer();
            this.SourceList = new System.Windows.Forms.ListView();
            this.SourceHdr = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SourceToolbar = new System.Windows.Forms.ToolStrip();
            this.AddBtn = new System.Windows.Forms.ToolStripButton();
            this.AddAllBtn = new System.Windows.Forms.ToolStripButton();
            this.ItemList = new System.Windows.Forms.ListView();
            this.ItemHdr = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ItemToolbar = new System.Windows.Forms.ToolStrip();
            this.RemoveBtn = new System.Windows.Forms.ToolStripButton();
            this.ClearBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.UpBtn = new System.Windows.Forms.ToolStripButton();
            this.DownBtn = new System.Windows.Forms.ToolStripButton();
            this.BrowserPanel = new System.Windows.Forms.Panel();
            this.Browser = new System.Windows.Forms.WebBrowser();
            this.BrowserToolbar = new System.Windows.Forms.ToolStrip();
            this.ExportBtn = new System.Windows.Forms.ToolStripButton();
            this.PlayerViewBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.DMInfoBtn = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.Splitter)).BeginInit();
            this.Splitter.Panel1.SuspendLayout();
            this.Splitter.Panel2.SuspendLayout();
            this.Splitter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ItemSplitter)).BeginInit();
            this.ItemSplitter.Panel1.SuspendLayout();
            this.ItemSplitter.Panel2.SuspendLayout();
            this.ItemSplitter.SuspendLayout();
            this.SourceToolbar.SuspendLayout();
            this.ItemToolbar.SuspendLayout();
            this.BrowserPanel.SuspendLayout();
            this.BrowserToolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // CloseBtn
            // 
            this.CloseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CloseBtn.Location = new System.Drawing.Point(1061, 479);
            this.CloseBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CloseBtn.Name = "CloseBtn";
            this.CloseBtn.Size = new System.Drawing.Size(100, 28);
            this.CloseBtn.TabIndex = 0;
            this.CloseBtn.Text = "Close";
            this.CloseBtn.UseVisualStyleBackColor = true;
            // 
            // Splitter
            // 
            this.Splitter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Splitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.Splitter.Location = new System.Drawing.Point(16, 15);
            this.Splitter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Splitter.Name = "Splitter";
            // 
            // Splitter.Panel1
            // 
            this.Splitter.Panel1.Controls.Add(this.ItemSplitter);
            // 
            // Splitter.Panel2
            // 
            this.Splitter.Panel2.Controls.Add(this.BrowserPanel);
            this.Splitter.Panel2.Controls.Add(this.BrowserToolbar);
            this.Splitter.Size = new System.Drawing.Size(1145, 457);
            this.Splitter.SplitterDistance = 494;
            this.Splitter.SplitterWidth = 5;
            this.Splitter.TabIndex = 1;
            // 
            // ItemSplitter
            // 
            this.ItemSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ItemSplitter.Location = new System.Drawing.Point(0, 0);
            this.ItemSplitter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ItemSplitter.Name = "ItemSplitter";
            // 
            // ItemSplitter.Panel1
            // 
            this.ItemSplitter.Panel1.Controls.Add(this.SourceList);
            this.ItemSplitter.Panel1.Controls.Add(this.SourceToolbar);
            // 
            // ItemSplitter.Panel2
            // 
            this.ItemSplitter.Panel2.Controls.Add(this.ItemList);
            this.ItemSplitter.Panel2.Controls.Add(this.ItemToolbar);
            this.ItemSplitter.Size = new System.Drawing.Size(494, 457);
            this.ItemSplitter.SplitterDistance = 242;
            this.ItemSplitter.SplitterWidth = 5;
            this.ItemSplitter.TabIndex = 2;
            // 
            // SourceList
            // 
            this.SourceList.AllowDrop = true;
            this.SourceList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.SourceHdr});
            this.SourceList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SourceList.FullRowSelect = true;
            this.SourceList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.SourceList.HideSelection = false;
            this.SourceList.Location = new System.Drawing.Point(0, 27);
            this.SourceList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SourceList.MultiSelect = false;
            this.SourceList.Name = "SourceList";
            this.SourceList.Size = new System.Drawing.Size(242, 430);
            this.SourceList.TabIndex = 2;
            this.SourceList.UseCompatibleStateImageBehavior = false;
            this.SourceList.View = System.Windows.Forms.View.Details;
            this.SourceList.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.SourceList_ItemDrag);
            this.SourceList.DragDrop += new System.Windows.Forms.DragEventHandler(this.SourceList_DragDrop);
            this.SourceList.DragOver += new System.Windows.Forms.DragEventHandler(this.SourceList_DragOver);
            this.SourceList.DoubleClick += new System.EventHandler(this.AddBtn_Click);
            // 
            // SourceHdr
            // 
            this.SourceHdr.Text = "Item";
            this.SourceHdr.Width = 200;
            // 
            // SourceToolbar
            // 
            this.SourceToolbar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.SourceToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddBtn,
            this.AddAllBtn});
            this.SourceToolbar.Location = new System.Drawing.Point(0, 0);
            this.SourceToolbar.Name = "SourceToolbar";
            this.SourceToolbar.Size = new System.Drawing.Size(242, 27);
            this.SourceToolbar.TabIndex = 1;
            this.SourceToolbar.Text = "toolStrip2";
            // 
            // AddBtn
            // 
            this.AddBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.AddBtn.Image = ((System.Drawing.Image)(resources.GetObject("AddBtn.Image")));
            this.AddBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.AddBtn.Name = "AddBtn";
            this.AddBtn.Size = new System.Drawing.Size(41, 24);
            this.AddBtn.Text = "Add";
            this.AddBtn.Click += new System.EventHandler(this.AddBtn_Click);
            // 
            // AddAllBtn
            // 
            this.AddAllBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.AddAllBtn.Image = ((System.Drawing.Image)(resources.GetObject("AddAllBtn.Image")));
            this.AddAllBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.AddAllBtn.Name = "AddAllBtn";
            this.AddAllBtn.Size = new System.Drawing.Size(63, 24);
            this.AddAllBtn.Text = "Add All";
            this.AddAllBtn.Click += new System.EventHandler(this.AddAllBtn_Click);
            // 
            // ItemList
            // 
            this.ItemList.AllowDrop = true;
            this.ItemList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ItemHdr});
            this.ItemList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ItemList.FullRowSelect = true;
            this.ItemList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ItemList.HideSelection = false;
            this.ItemList.Location = new System.Drawing.Point(0, 27);
            this.ItemList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ItemList.MultiSelect = false;
            this.ItemList.Name = "ItemList";
            this.ItemList.Size = new System.Drawing.Size(247, 430);
            this.ItemList.TabIndex = 1;
            this.ItemList.UseCompatibleStateImageBehavior = false;
            this.ItemList.View = System.Windows.Forms.View.Details;
            this.ItemList.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.ItemList_ItemDrag);
            this.ItemList.DragDrop += new System.Windows.Forms.DragEventHandler(this.ItemList_DragDrop);
            this.ItemList.DragOver += new System.Windows.Forms.DragEventHandler(this.ItemList_DragOver);
            // 
            // ItemHdr
            // 
            this.ItemHdr.Text = "Item";
            this.ItemHdr.Width = 200;
            // 
            // ItemToolbar
            // 
            this.ItemToolbar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ItemToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RemoveBtn,
            this.ClearBtn,
            this.toolStripSeparator1,
            this.UpBtn,
            this.DownBtn});
            this.ItemToolbar.Location = new System.Drawing.Point(0, 0);
            this.ItemToolbar.Name = "ItemToolbar";
            this.ItemToolbar.Size = new System.Drawing.Size(247, 27);
            this.ItemToolbar.TabIndex = 0;
            this.ItemToolbar.Text = "toolStrip2";
            // 
            // RemoveBtn
            // 
            this.RemoveBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.RemoveBtn.Image = ((System.Drawing.Image)(resources.GetObject("RemoveBtn.Image")));
            this.RemoveBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RemoveBtn.Name = "RemoveBtn";
            this.RemoveBtn.Size = new System.Drawing.Size(67, 24);
            this.RemoveBtn.Text = "Remove";
            this.RemoveBtn.Click += new System.EventHandler(this.RemoveBtn_Click);
            // 
            // ClearBtn
            // 
            this.ClearBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ClearBtn.Image = ((System.Drawing.Image)(resources.GetObject("ClearBtn.Image")));
            this.ClearBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ClearBtn.Name = "ClearBtn";
            this.ClearBtn.Size = new System.Drawing.Size(47, 24);
            this.ClearBtn.Text = "Clear";
            this.ClearBtn.Click += new System.EventHandler(this.RemoveAll_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // UpBtn
            // 
            this.UpBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.UpBtn.Image = ((System.Drawing.Image)(resources.GetObject("UpBtn.Image")));
            this.UpBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.UpBtn.Name = "UpBtn";
            this.UpBtn.Size = new System.Drawing.Size(73, 24);
            this.UpBtn.Text = "Move Up";
            this.UpBtn.Click += new System.EventHandler(this.UpBtn_Click);
            // 
            // DownBtn
            // 
            this.DownBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.DownBtn.Image = ((System.Drawing.Image)(resources.GetObject("DownBtn.Image")));
            this.DownBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.DownBtn.Name = "DownBtn";
            this.DownBtn.Size = new System.Drawing.Size(93, 24);
            this.DownBtn.Text = "Move Down";
            this.DownBtn.Click += new System.EventHandler(this.DownBtn_Click);
            // 
            // BrowserPanel
            // 
            this.BrowserPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BrowserPanel.Controls.Add(this.Browser);
            this.BrowserPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BrowserPanel.Location = new System.Drawing.Point(0, 27);
            this.BrowserPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BrowserPanel.Name = "BrowserPanel";
            this.BrowserPanel.Size = new System.Drawing.Size(646, 430);
            this.BrowserPanel.TabIndex = 2;
            // 
            // Browser
            // 
            this.Browser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Browser.IsWebBrowserContextMenuEnabled = false;
            this.Browser.Location = new System.Drawing.Point(0, 0);
            this.Browser.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Browser.MinimumSize = new System.Drawing.Size(27, 25);
            this.Browser.Name = "Browser";
            this.Browser.ScriptErrorsSuppressed = true;
            this.Browser.Size = new System.Drawing.Size(644, 428);
            this.Browser.TabIndex = 1;
            this.Browser.WebBrowserShortcutsEnabled = false;
            // 
            // BrowserToolbar
            // 
            this.BrowserToolbar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.BrowserToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ExportBtn,
            this.PlayerViewBtn,
            this.toolStripSeparator2,
            this.DMInfoBtn});
            this.BrowserToolbar.Location = new System.Drawing.Point(0, 0);
            this.BrowserToolbar.Name = "BrowserToolbar";
            this.BrowserToolbar.Size = new System.Drawing.Size(646, 27);
            this.BrowserToolbar.TabIndex = 0;
            this.BrowserToolbar.Text = "toolStrip1";
            // 
            // ExportBtn
            // 
            this.ExportBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ExportBtn.Image = ((System.Drawing.Image)(resources.GetObject("ExportBtn.Image")));
            this.ExportBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ExportBtn.Name = "ExportBtn";
            this.ExportBtn.Size = new System.Drawing.Size(56, 24);
            this.ExportBtn.Text = "Export";
            this.ExportBtn.Click += new System.EventHandler(this.ExportBtn_Click);
            // 
            // PlayerViewBtn
            // 
            this.PlayerViewBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.PlayerViewBtn.Image = ((System.Drawing.Image)(resources.GetObject("PlayerViewBtn.Image")));
            this.PlayerViewBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.PlayerViewBtn.Name = "PlayerViewBtn";
            this.PlayerViewBtn.Size = new System.Drawing.Size(144, 24);
            this.PlayerViewBtn.Text = "Send to Player View";
            this.PlayerViewBtn.Click += new System.EventHandler(this.PlayerViewBtn_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // DMInfoBtn
            // 
            this.DMInfoBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.DMInfoBtn.Image = ((System.Drawing.Image)(resources.GetObject("DMInfoBtn.Image")));
            this.DMInfoBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.DMInfoBtn.Name = "DMInfoBtn";
            this.DMInfoBtn.Size = new System.Drawing.Size(107, 24);
            this.DMInfoBtn.Text = "Show DM Info";
            this.DMInfoBtn.Click += new System.EventHandler(this.DMInfoBtn_Click);
            // 
            // HandoutForm
            // 
            this.AcceptButton = this.CloseBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1177, 522);
            this.Controls.Add(this.Splitter);
            this.Controls.Add(this.CloseBtn);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimizeBox = false;
            this.Name = "HandoutForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export Handout";
            this.Splitter.Panel1.ResumeLayout(false);
            this.Splitter.Panel2.ResumeLayout(false);
            this.Splitter.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Splitter)).EndInit();
            this.Splitter.ResumeLayout(false);
            this.ItemSplitter.Panel1.ResumeLayout(false);
            this.ItemSplitter.Panel1.PerformLayout();
            this.ItemSplitter.Panel2.ResumeLayout(false);
            this.ItemSplitter.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ItemSplitter)).EndInit();
            this.ItemSplitter.ResumeLayout(false);
            this.SourceToolbar.ResumeLayout(false);
            this.SourceToolbar.PerformLayout();
            this.ItemToolbar.ResumeLayout(false);
            this.ItemToolbar.PerformLayout();
            this.BrowserPanel.ResumeLayout(false);
            this.BrowserToolbar.ResumeLayout(false);
            this.BrowserToolbar.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button CloseBtn;
		private System.Windows.Forms.SplitContainer Splitter;
		private System.Windows.Forms.ListView ItemList;
		private System.Windows.Forms.ColumnHeader ItemHdr;
		private System.Windows.Forms.ToolStrip ItemToolbar;
		private System.Windows.Forms.WebBrowser Browser;
		private System.Windows.Forms.ToolStrip BrowserToolbar;
		private System.Windows.Forms.ToolStripButton UpBtn;
		private System.Windows.Forms.ToolStripButton DownBtn;
		private System.Windows.Forms.ToolStripButton ExportBtn;
		private System.Windows.Forms.Panel BrowserPanel;
		private System.Windows.Forms.SplitContainer ItemSplitter;
		private System.Windows.Forms.ListView SourceList;
		private System.Windows.Forms.ColumnHeader SourceHdr;
		private System.Windows.Forms.ToolStrip SourceToolbar;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton PlayerViewBtn;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton DMInfoBtn;
		private System.Windows.Forms.ToolStripButton AddBtn;
		private System.Windows.Forms.ToolStripButton AddAllBtn;
		private System.Windows.Forms.ToolStripButton RemoveBtn;
		private System.Windows.Forms.ToolStripButton ClearBtn;
	}
}
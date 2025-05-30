﻿namespace Masterplan.UI
{
	partial class CreatureTemplateBuilderForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreatureTemplateBuilderForm));
            this.Toolbar = new System.Windows.Forms.ToolStrip();
            this.FileMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.FileExport = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionsMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.OptionsVariant = new System.Windows.Forms.ToolStripMenuItem();
            this.BtnPnl = new System.Windows.Forms.Panel();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.OKBtn = new System.Windows.Forms.Button();
            this.StatBlockBrowser = new System.Windows.Forms.WebBrowser();
            this.Toolbar.SuspendLayout();
            this.BtnPnl.SuspendLayout();
            this.SuspendLayout();
            // 
            // Toolbar
            // 
            this.Toolbar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.Toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenu,
            this.OptionsMenu});
            this.Toolbar.Location = new System.Drawing.Point(0, 0);
            this.Toolbar.Name = "Toolbar";
            this.Toolbar.Size = new System.Drawing.Size(512, 27);
            this.Toolbar.TabIndex = 0;
            this.Toolbar.Text = "toolStrip1";
            // 
            // FileMenu
            // 
            this.FileMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.FileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileExport});
            this.FileMenu.Image = ((System.Drawing.Image)(resources.GetObject("FileMenu.Image")));
            this.FileMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.FileMenu.Name = "FileMenu";
            this.FileMenu.Size = new System.Drawing.Size(46, 24);
            this.FileMenu.Text = "File";
            // 
            // FileExport
            // 
            this.FileExport.Name = "FileExport";
            this.FileExport.Size = new System.Drawing.Size(144, 26);
            this.FileExport.Text = "Export...";
            this.FileExport.Click += new System.EventHandler(this.FileExport_Click);
            // 
            // OptionsMenu
            // 
            this.OptionsMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.OptionsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OptionsVariant});
            this.OptionsMenu.Image = ((System.Drawing.Image)(resources.GetObject("OptionsMenu.Image")));
            this.OptionsMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.OptionsMenu.Name = "OptionsMenu";
            this.OptionsMenu.Size = new System.Drawing.Size(75, 24);
            this.OptionsMenu.Text = "Options";
            // 
            // OptionsVariant
            // 
            this.OptionsVariant.Name = "OptionsVariant";
            this.OptionsVariant.Size = new System.Drawing.Size(270, 26);
            this.OptionsVariant.Text = "Copy an Existing Creature...";
            this.OptionsVariant.Click += new System.EventHandler(this.OptionsVariant_Click);
            // 
            // BtnPnl
            // 
            this.BtnPnl.Controls.Add(this.CancelBtn);
            this.BtnPnl.Controls.Add(this.OKBtn);
            this.BtnPnl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BtnPnl.Location = new System.Drawing.Point(0, 545);
            this.BtnPnl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnPnl.Name = "BtnPnl";
            this.BtnPnl.Size = new System.Drawing.Size(512, 43);
            this.BtnPnl.TabIndex = 2;
            // 
            // CancelBtn
            // 
            this.CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(396, 7);
            this.CancelBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(100, 28);
            this.CancelBtn.TabIndex = 1;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // OKBtn
            // 
            this.OKBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKBtn.Location = new System.Drawing.Point(288, 7);
            this.OKBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(100, 28);
            this.OKBtn.TabIndex = 0;
            this.OKBtn.Text = "OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            // 
            // StatBlockBrowser
            // 
            this.StatBlockBrowser.AllowWebBrowserDrop = false;
            this.StatBlockBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StatBlockBrowser.IsWebBrowserContextMenuEnabled = false;
            this.StatBlockBrowser.Location = new System.Drawing.Point(0, 27);
            this.StatBlockBrowser.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.StatBlockBrowser.MinimumSize = new System.Drawing.Size(27, 25);
            this.StatBlockBrowser.Name = "StatBlockBrowser";
            this.StatBlockBrowser.ScriptErrorsSuppressed = true;
            this.StatBlockBrowser.Size = new System.Drawing.Size(512, 518);
            this.StatBlockBrowser.TabIndex = 2;
            this.StatBlockBrowser.WebBrowserShortcutsEnabled = false;
            this.StatBlockBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.Browser_Navigating);
            // 
            // CreatureTemplateBuilderForm
            // 
            this.AcceptButton = this.OKBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.ClientSize = new System.Drawing.Size(512, 588);
            this.Controls.Add(this.StatBlockBrowser);
            this.Controls.Add(this.BtnPnl);
            this.Controls.Add(this.Toolbar);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimizeBox = false;
            this.Name = "CreatureTemplateBuilderForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Creature Template Builder";
            this.Toolbar.ResumeLayout(false);
            this.Toolbar.PerformLayout();
            this.BtnPnl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip Toolbar;
		private System.Windows.Forms.Panel BtnPnl;
		private System.Windows.Forms.Button CancelBtn;
		private System.Windows.Forms.Button OKBtn;
		private System.Windows.Forms.WebBrowser StatBlockBrowser;
		private System.Windows.Forms.ToolStripDropDownButton OptionsMenu;
		private System.Windows.Forms.ToolStripMenuItem OptionsVariant;
		private System.Windows.Forms.ToolStripDropDownButton FileMenu;
		private System.Windows.Forms.ToolStripMenuItem FileExport;
	}
}
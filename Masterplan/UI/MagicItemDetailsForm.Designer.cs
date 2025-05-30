﻿namespace Masterplan.UI
{
	partial class MagicItemDetailsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MagicItemDetailsForm));
            this.Browser = new System.Windows.Forms.WebBrowser();
            this.Toolbar = new System.Windows.Forms.ToolStrip();
            this.ExportMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.ExportHTML = new System.Windows.Forms.ToolStripMenuItem();
            this.PlayerViewBtn = new System.Windows.Forms.ToolStripButton();
            this.Toolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // Browser
            // 
            this.Browser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Browser.Location = new System.Drawing.Point(0, 27);
            this.Browser.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Browser.MinimumSize = new System.Drawing.Size(27, 25);
            this.Browser.Name = "Browser";
            this.Browser.Size = new System.Drawing.Size(496, 419);
            this.Browser.TabIndex = 1;
            // 
            // Toolbar
            // 
            this.Toolbar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.Toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ExportMenu,
            this.PlayerViewBtn});
            this.Toolbar.Location = new System.Drawing.Point(0, 0);
            this.Toolbar.Name = "Toolbar";
            this.Toolbar.Size = new System.Drawing.Size(496, 27);
            this.Toolbar.TabIndex = 4;
            this.Toolbar.Text = "toolStrip1";
            // 
            // ExportMenu
            // 
            this.ExportMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ExportMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ExportHTML});
            this.ExportMenu.Image = ((System.Drawing.Image)(resources.GetObject("ExportMenu.Image")));
            this.ExportMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ExportMenu.Name = "ExportMenu";
            this.ExportMenu.Size = new System.Drawing.Size(66, 24);
            this.ExportMenu.Text = "Export";
            // 
            // ExportHTML
            // 
            this.ExportHTML.Name = "ExportHTML";
            this.ExportHTML.Size = new System.Drawing.Size(196, 26);
            this.ExportHTML.Text = "Export to HTML";
            this.ExportHTML.Click += new System.EventHandler(this.ExportHTML_Click);
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
            // MagicItemDetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 446);
            this.Controls.Add(this.Browser);
            this.Controls.Add(this.Toolbar);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimizeBox = false;
            this.Name = "MagicItemDetailsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Magic Item Details";
            this.Toolbar.ResumeLayout(false);
            this.Toolbar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.WebBrowser Browser;
		private System.Windows.Forms.ToolStrip Toolbar;
		private System.Windows.Forms.ToolStripButton PlayerViewBtn;
		private System.Windows.Forms.ToolStripDropDownButton ExportMenu;
		private System.Windows.Forms.ToolStripMenuItem ExportHTML;
	}
}
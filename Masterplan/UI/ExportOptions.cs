using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Masterplan.Tools;

namespace Masterplan.UI
{
    /// <summary>
    /// Export Options to select export formats
    /// </summary>
    public class ExportOptionsForm : Form
    {
        private RadioButton rbPng;
        private RadioButton rbJpg;
        private RadioButton rbRtf;
        private RadioButton rbHtm;
        private Button btnExport;
        private Label lblInstruction;

        // This would be passed in from the parent form/context
        private string _htmlContentToExport;

        /// <summary>
        /// Public class to capture the HTML content to export 
        /// </summary>
        /// <param name="htmlContent"></param>
        public ExportOptionsForm(string htmlContent)
        {
            _htmlContentToExport = htmlContent;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            lblInstruction = new Label();
            rbPng = new RadioButton();
            rbJpg = new RadioButton();
            rbRtf = new RadioButton();
            rbHtm = new RadioButton();
            btnExport = new Button();
            SuspendLayout();
            // 
            // lblInstruction
            // 
            lblInstruction.AutoSize = true;
            lblInstruction.Location = new Point(20, 20);
            lblInstruction.Name = "lblInstruction";
            lblInstruction.Size = new Size(132, 15);
            lblInstruction.TabIndex = 0;
            lblInstruction.Text = "Select an export format:";
            // 
            // rbPng
            // 
            rbPng.Checked = true;
            rbPng.Location = new Point(30, 50);
            rbPng.Name = "rbPng";
            rbPng.Size = new Size(104, 24);
            rbPng.TabIndex = 1;
            rbPng.TabStop = true;
            rbPng.Text = "PNG Image";
            // 
            // rbJpg
            // 
            rbJpg.Location = new Point(30, 80);
            rbJpg.Name = "rbJpg";
            rbJpg.Size = new Size(104, 24);
            rbJpg.TabIndex = 2;
            rbJpg.Text = "JPG Image";
            // 
            // rbRtf
            // 
            rbRtf.Location = new Point(30, 110);
            rbRtf.Name = "rbRtf";
            rbRtf.Size = new Size(104, 24);
            rbRtf.TabIndex = 3;
            rbRtf.Text = "RTF Document";
            // 
            // rbHtm
            // 
            rbHtm.Location = new Point(30, 140);
            rbHtm.Name = "rbHtm";
            rbHtm.Size = new Size(122, 24);
            rbHtm.TabIndex = 4;
            rbHtm.Text = "HTM Document";
            // 
            // btnExport
            // 
            btnExport.Location = new Point(99, 170);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(75, 23);
            btnExport.TabIndex = 4;
            btnExport.Text = "Export";
            btnExport.Click += BtnExport_Click;
            // 
            // ExportOptionsForm
            // 
            ClientSize = new Size(200, 203);
            Controls.Add(lblInstruction);
            Controls.Add(rbPng);
            Controls.Add(rbJpg);
            Controls.Add(rbRtf);
            Controls.Add(rbHtm);
            Controls.Add(btnExport);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ExportOptionsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Export Options";
            ResumeLayout(false);
            PerformLayout();
        }

        private async void BtnExport_Click(object sender, EventArgs e)
        {
            // Disable button to prevent double clicks during async process
            btnExport.Enabled = false;
            btnExport.Text = "Processing...";

            try
            {
                byte[] resultData = null;
                string extension = "";

                if (rbPng.Checked)
                {
                    var pngExporter = new PngExporter();
                    resultData = await pngExporter.StartPNGExport(_htmlContentToExport);
                    extension = "png";
                }
                else if (rbJpg.Checked)
                {
                    var jpgExporter = new JpgExporter();
                    resultData = await jpgExporter.StartJPGExport(_htmlContentToExport);
                    extension = "jpg";
                }
                else if (rbRtf.Checked)
                {
                    var RtfExporter = new RtfExporter();
                    resultData = await RtfExporter.StartRTFExport(_htmlContentToExport);
                    extension = "rtf";
                }

                if (resultData != null && resultData.Length > 0)
                {
                    SaveFileToDisk(resultData, extension);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnExport.Enabled = true;
                btnExport.Text = "Export";
            }
        }

        /// <summary>
        /// Save the file to disk
        /// Eventually move all save functions to a common save component
        /// </summary>
        /// <param name="data"></param>
        /// <param name="extension"></param>
        private void SaveFileToDisk(byte[] data, string extension)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = $"{extension.ToUpper()} Image|*.{extension}";
                sfd.Title = "Save Exported Image";
                sfd.FileName = $"Export_{DateTime.Now:yyyyMMdd_HHmm}.{extension}";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, data);
                    MessageBox.Show("File saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
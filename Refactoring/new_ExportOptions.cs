#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions; // Required for RtfExporter
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
            
            // Set PNG as default selection for convenience
            rbPng.Checked = true;
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
            lblInstruction.Location = new System.Drawing.Point(12, 9);
            lblInstruction.Name = "lblInstruction";
            lblInstruction.Size = new System.Drawing.Size(130, 13);
            lblInstruction.TabIndex = 0;
            lblInstruction.Text = "Select an export format:";
            // 
            // rbPng
            // 
            rbPng.AutoSize = true;
            rbPng.Location = new System.Drawing.Point(24, 30);
            rbPng.Name = "rbPng";
            rbPng.Size = new System.Drawing.Size(47, 17);
            rbPng.TabIndex = 1;
            rbPng.TabStop = true;
            rbPng.Text = "PNG";
            rbPng.UseVisualStyleBackColor = true;
            // 
            // rbJpg
            // 
            rbJpg.AutoSize = true;
            rbJpg.Location = new System.Drawing.Point(24, 53);
            rbJpg.Name = "rbJpg";
            rbJpg.Size = new System.Drawing.Size(45, 17);
            rbJpg.TabIndex = 2;
            rbJpg.TabStop = true;
            rbJpg.Text = "JPG";
            rbJpg.UseVisualStyleBackColor = true;
            // 
            // rbRtf
            // 
            rbRtf.AutoSize = true;
            rbRtf.Location = new System.Drawing.Point(24, 76);
            rbRtf.Name = "rbRtf";
            rbRtf.Size = new System.Drawing.Size(46, 17);
            rbRtf.TabIndex = 3;
            rbRtf.TabStop = true;
            rbRtf.Text = "RTF";
            rbRtf.UseVisualStyleBackColor = true;
            // 
            // rbHtm
            // 
            rbHtm.AutoSize = true;
            rbHtm.Location = new System.Drawing.Point(24, 99);
            rbHtm.Name = "rbHtm";
            rbHtm.Size = new System.Drawing.Size(51, 17);
            rbHtm.TabIndex = 4;
            rbHtm.TabStop = true;
            rbHtm.Text = "HTML";
            rbHtm.UseVisualStyleBackColor = true;
            // 
            // btnExport
            // 
            btnExport.Location = new System.Drawing.Point(12, 130);
            btnExport.Name = "btnExport";
            btnExport.Size = new System.Drawing.Size(160, 23);
            btnExport.TabIndex = 5;
            btnExport.Text = "Export";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // ExportOptionsForm
            // 
            this.ClientSize = new System.Drawing.Size(184, 165);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.rbHtm);
            this.Controls.Add(this.rbRtf);
            this.Controls.Add(this.rbJpg);
            this.Controls.Add(this.rbPng);
            this.Controls.Add(this.lblInstruction);
            this.Name = "ExportOptionsForm";
            this.Text = "Export Options";
            ResumeLayout(false);
            PerformLayout();
        }

        /// <summary>
        /// Handles the export button click event.
        /// </summary>
        private async void btnExport_Click(object? sender, EventArgs e)
        {
            byte[]? resultData = null;
            string extension = "";

            btnExport.Enabled = false;
            btnExport.Text = "Exporting...";

            try
            {
                if (rbPng.Checked || rbJpg.Checked)
                {
                    extension = rbPng.Checked ? "png" : "jpg";
                    var exporter = new PngExporter();
                    resultData = await exporter.StartPNGExport(_htmlContentToExport);
                }
                else if (rbRtf.Checked)
                {
                    extension = "rtf";
                    // ----------------------------------------------------
                    // !!! UPDATED RTF EXPORT LOGIC TO USE NEW EXPORTER !!!
                    // ----------------------------------------------------
                    var rtfExporter = new RtfExporter();
                    string rtfContent = rtfExporter.ExportToRtf(_htmlContentToExport);
                    resultData = Encoding.UTF8.GetBytes(rtfContent);
                    // ----------------------------------------------------
                }
                else if (rbHtm.Checked)
                {
                    extension = "htm";
                    // HTML is passed directly
                    resultData = Encoding.UTF8.GetBytes(_htmlContentToExport);
                }
                else
                {
                    MessageBox.Show("Please select an export format.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (resultData != null && resultData.Length > 0)
                {
                    SaveFileToDisk(resultData, extension);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}\\n{ex.InnerException?.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                // Note: RTF filter uses .rtf
                string filter = extension.ToUpper() == "RTF" || extension.ToUpper() == "HTM"
                    ? $"{extension.ToUpper()} File|*.{extension}"
                    : $"{extension.ToUpper()} Image|*.{extension}";

                sfd.Filter = filter;
                sfd.Title = $"Save Exported {extension.ToUpper()}";
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
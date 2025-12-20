#nullable enable

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlToOpenXml;
using Masterplan.Tools;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing; // for PaperKind
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions; // Required for RtfExporter
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Masterplan.UI
{
    /// <summary>
    /// Export Options to select export formats
    /// </summary>
    public class ExportOptionsForm : Form
    {
        private RadioButton rbPng;
        private RadioButton rbJpg;
        private RadioButton rbDoc;
        private RadioButton rbHtm;
        private RadioButton rbPdf;
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
            rbDoc = new RadioButton();
            rbHtm = new RadioButton();
            rbPdf = new RadioButton();
            btnExport = new Button();
            SuspendLayout();
            // 
            // lblInstruction
            // 
            lblInstruction.AutoSize = true;
            lblInstruction.Location = new System.Drawing.Point(12, 9);
            lblInstruction.Name = "lblInstruction";
            lblInstruction.Size = new System.Drawing.Size(132, 15);
            lblInstruction.TabIndex = 0;
            lblInstruction.Text = "Select an export format:";
            // 
            // rbPng
            // 
            rbPng.AutoSize = true;
            rbPng.Location = new System.Drawing.Point(78, 33);
            rbPng.Name = "rbPng";
            rbPng.Size = new System.Drawing.Size(49, 19);
            rbPng.TabIndex = 1;
            rbPng.TabStop = true;
            rbPng.Text = "PNG";
            rbPng.UseVisualStyleBackColor = true;
            // 
            // rbJpg
            // 
            rbJpg.AutoSize = true;
            rbJpg.Location = new System.Drawing.Point(78, 58);
            rbJpg.Name = "rbJpg";
            rbJpg.Size = new System.Drawing.Size(44, 19);
            rbJpg.TabIndex = 2;
            rbJpg.TabStop = true;
            rbJpg.Text = "JPG";
            rbJpg.UseVisualStyleBackColor = true;
            // 
            // rbDoc
            // 
            rbDoc.AutoSize = true;
            rbDoc.Location = new System.Drawing.Point(78, 108);
            rbDoc.Name = "rbDoc";
            rbDoc.Size = new System.Drawing.Size(57, 19);
            rbDoc.TabIndex = 4;
            rbDoc.TabStop = true;
            rbDoc.Text = "DOCX";
            rbDoc.UseVisualStyleBackColor = true;
            // 
            // rbHtm
            // 
            rbHtm.AutoSize = true;
            rbHtm.Location = new System.Drawing.Point(78, 83);
            rbHtm.Name = "rbHtm";
            rbHtm.Size = new System.Drawing.Size(52, 19);
            rbHtm.TabIndex = 3;
            rbHtm.TabStop = true;
            rbHtm.Text = "HTM";
            rbHtm.UseVisualStyleBackColor = true;
            // 
            // rbPdf
            // 
            rbPdf.AutoSize = true;
            rbPdf.Location = new System.Drawing.Point(78, 133);
            rbPdf.Name = "rbPdf";
            rbPdf.Size = new System.Drawing.Size(46, 19);
            rbPdf.TabIndex = 5;
            rbPdf.TabStop = true;
            rbPdf.Text = "PDF";
            rbPdf.UseVisualStyleBackColor = true;
            // 
            // btnExport
            // 
            btnExport.Location = new System.Drawing.Point(154, 154);
            btnExport.Name = "btnExport";
            btnExport.Size = new System.Drawing.Size(120, 23);
            btnExport.TabIndex = 6;
            btnExport.Text = "Export";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += btnExport_Click;
            // 
            // ExportOptionsForm
            // 
            ClientSize = new System.Drawing.Size(286, 189);
            Controls.Add(btnExport);
            Controls.Add(rbPdf);
            Controls.Add(rbHtm);
            Controls.Add(rbDoc);
            Controls.Add(rbJpg);
            Controls.Add(rbPng);
            Controls.Add(lblInstruction);
            Name = "ExportOptionsForm";
            Text = "Export Options";
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
                else if (rbDoc.Checked)
                {
                    // --- NEW PREPROCESSING STEP ---
                    string styledHtml = InlineTableStyles(_htmlContentToExport);

                    byte[] docxData = GenerateDocxFromHtml(styledHtml);

                    SaveFileToDisk(docxData, "docx");
                    return;
                }
                else if (rbPdf.Checked)
                {
                    // Use PuppeteerSharp to render the HTML into a PDF byte array
                    byte[] pdfData = await GeneratePdfFromHtmlAsync(_htmlContentToExport); // Must use 'await'
                    SaveFileToDisk(pdfData, "pdf"); // Export to PDF instead of RTF
                    return;
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

        private string InlineTableStyles(string htmlContent)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlContent);

            // Map classes to styles based on the original CSS/RtfExporter logic
            var styleMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // Dark backgrounds with White Text (color: #FFFFFF)
        {"heading", "background-color: #143D5F; color: #FFFFFF;"},
        {"hero", "background-color: #143D5F; color: #FFFFFF;"},
        {"trap", "background-color: #5B1F34; color: #FFFFFF;"},
        {"artifact", "background-color: #5B1F34; color: #FFFFFF;"},
        {"creature", "background-color: #364F27; color: #FFFFFF;"},
        {"item", "background-color: #D06015; color: #FFFFFF;"},
        {"daily", "background-color: #000000; color: #FFFFFF;"},
        {"encounterlog", "background-color: #000000; color: #FFFFFF;"},
        
        // Light backgrounds with Black Text (default)
        {"shaded", "background-color: #9FA48D;"},
        {"warning", "background-color: #E5A0A0;"},
        {"clear", "background-color: #FFFFFF;"},
        
        // Default table row (light beige)
        // If a TR has no class, it will use the default color. 
        // We handle this by setting the default style on the table itself.
    };

            var tableNode = doc.DocumentNode.SelectSingleNode("//table");
            if (tableNode != null)
            {
                // Set the default table style (light beige)
                tableNode.SetAttributeValue("style", "background-color: #E1E7C5; border: 1px solid #BBBBBB; border-collapse: collapse;");

                // Iterate through all table rows (tr)
                var rows = tableNode.SelectNodes(".//tr");
                if (rows != null)
                {
                    foreach (var rowNode in rows)
                    {
                        string rowClass = rowNode.GetAttributeValue("class", string.Empty).ToLower();
                        string inlineStyle = string.Empty;

                        foreach (var kvp in styleMap)
                        {
                            if (rowClass.Contains(kvp.Key))
                            {
                                inlineStyle = kvp.Value;
                                break;
                            }
                        }

                        if (!string.IsNullOrEmpty(inlineStyle))
                        {
                            // Overwrite any existing style with the correct inline style
                            rowNode.SetAttributeValue("style", inlineStyle);
                        }
                    }
                }
            }

            return doc.DocumentNode.OuterHtml;
        }


        /// <summary>
        /// Converts HTML content to a DOCX byte array using HtmlToOpenXml.
        /// </summary>
        private byte[] GenerateDocxFromHtml(string htmlContent)
        {
            using (MemoryStream generatedDocument = new MemoryStream())
            {
                // Create a WordprocessingDocument
                using (WordprocessingDocument package = WordprocessingDocument.Create(generatedDocument, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
                {
                    // Add a main document part
                    MainDocumentPart mainPart = package.AddMainDocumentPart();
                    mainPart.Document = new Document(new Body());

                    // Initialize the HtmlConverter
                    HtmlConverter converter = new HtmlConverter(mainPart);

                    // Convert the HTML and append it to the document body
                    var paragraphs = converter.Parse(htmlContent);
                    mainPart.Document.Body.Append(paragraphs);
                }

                return generatedDocument.ToArray();
            }
        }


        /// <summary>
        /// Renders HTML content to a PDF byte array using PuppeteerSharp.
        /// </summary>
        private async Task<byte[]> GeneratePdfFromHtmlAsync(string htmlContent)
        {
            // 1. Ensure the correct revision of Chromium is downloaded.
            await new BrowserFetcher().DownloadAsync();

            // 2. Launch the headless browser instance
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                // Optional: Setting a reasonable viewport size helps ensure table rendering is correct
                DefaultViewport = new ViewPortOptions { Width = 1000, Height = 800 }
            });

            using (var page = await browser.NewPageAsync())
            {
                // Set HTML content
                await page.SetContentAsync(htmlContent);

                // Wait briefly for CSS/layout to settle before printing
                await Task.Delay(100);

                // Generate the PDF
                var pdfOptions = new PdfOptions
                {
                    Format = PaperFormat.A4,
                    PrintBackground = true, // Ensures colors/backgrounds are printed
                    MarginOptions = new MarginOptions { Top = "0.5in", Bottom = "0.5in", Left = "0.5in", Right = "0.5in" }
                };

                return await page.PdfDataAsync(pdfOptions);
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
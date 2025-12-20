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
using Masterplan.Tools; // Required for PngExporter

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
            
            // Set PNG as default selection
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
            lblInstruction.Location = new Point(12, 9);
            lblInstruction.Name = "lblInstruction";
            lblInstruction.Size = new Size(300, 23);
            lblInstruction.Text = "Select the format to export the creature card in:";
            // 
            // rbPng
            // 
            rbPng.AutoSize = true;
            rbPng.Location = new Point(12, 40);
            rbPng.Name = "rbPng";
            rbPng.Size = new Size(51, 19);
            rbPng.TabStop = true;
            rbPng.Text = "PNG";
            rbPng.UseVisualStyleBackColor = true;
            // 
            // rbJpg
            // 
            rbJpg.AutoSize = true;
            rbJpg.Location = new Point(12, 65);
            rbJpg.Name = "rbJpg";
            rbJpg.Size = new Size(47, 19);
            rbJpg.Text = "JPG";
            rbJpg.UseVisualStyleBackColor = true;
            // 
            // rbRtf
            // 
            rbRtf.AutoSize = true;
            rbRtf.Location = new Point(12, 90);
            rbRtf.Name = "rbRtf";
            rbRtf.Size = new Size(47, 19);
            rbRtf.Text = "RTF";
            rbRtf.UseVisualStyleBackColor = true;
            // 
            // rbHtm
            // 
            rbHtm.AutoSize = true;
            rbHtm.Location = new Point(12, 115);
            rbHtm.Name = "rbHtm";
            rbHtm.Size = new Size(51, 19);
            rbHtm.Text = "HTM";
            rbHtm.UseVisualStyleBackColor = true;
            // 
            // btnExport
            // 
            btnExport.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnExport.Location = new Point(237, 149);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(75, 23);
            btnExport.Text = "Export";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += new EventHandler(btnExport_Click);
            // 
            // ExportOptionsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(324, 184);
            Controls.Add(btnExport);
            Controls.Add(rbHtm);
            Controls.Add(rbRtf);
            Controls.Add(rbJpg);
            Controls.Add(rbPng);
            Controls.Add(lblInstruction);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ExportOptionsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Export Options";
            ResumeLayout(false);
            PerformLayout();
        }

        private async void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            btnExport.Text = "Exporting...";

            try
            {
                byte[]? resultData = null;
                string extension = "";

                if (rbPng.Checked || rbJpg.Checked)
                {
                    // Uses the PngExporter logic (assumed to handle HTML rendering to image bytes)
                    extension = rbPng.Checked ? "png" : "jpg";
                    var imageExporter = new PngExporter();
                    // Assumes StartPNGExport handles the HTML rendering to image bytes
                    resultData = await imageExporter.StartPNGExport(_htmlContentToExport); 
                }
                else if (rbRtf.Checked)
                {
                    extension = "rtf";
                    // Use the embedded RtfExporter
                    var rtfExporter = new RtfExporter();
                    // This is a synchronous call returning RTF content as a byte array
                    resultData = rtfExporter.ExportTableToRtf(_htmlContentToExport); 
                }
                else if (rbHtm.Checked)
                {
                    extension = "htm";
                    // Export the raw HTML content as a byte array (UTF-8)
                    resultData = Encoding.UTF8.GetBytes(_htmlContentToExport); 
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
        /// Save the file to disk.
        /// </summary>
        /// <param name="data">The file content as a byte array.</param>
        /// <param name="extension">The file extension (png, jpg, rtf, htm).</param>
        private void SaveFileToDisk(byte[] data, string extension)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                string filter = "";
                string title = "Save Exported File";

                // Set the filter based on the extension
                if (extension.Equals("rtf", StringComparison.OrdinalIgnoreCase))
                {
                    filter = "Rich Text Format (*.rtf)|*.rtf";
                    title = "Save Exported RTF Document";
                }
                else if (extension.Equals("htm", StringComparison.OrdinalIgnoreCase))
                {
                    filter = "HTML Document (*.htm)|*.htm";
                    title = "Save Exported HTML Document";
                }
                else // PNG, JPG
                {
                    filter = $"{extension.ToUpper()} Image (*.{extension})|*.{extension}";
                    title = "Save Exported Image";
                }

                sfd.Filter = filter;
                sfd.Title = title;
                sfd.FileName = $"Export_{DateTime.Now:yyyyMMdd_HHmm}.{extension}";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, data);
                    MessageBox.Show("File saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }

    #region RTF EXPORTER LOGIC

    /// <summary>
    /// Converts HTML content, specifically designed for a single HTML table, into RTF format.
    /// This is embedded in the ExportOptions.cs file as requested by the user.
    /// </summary>
    public class RtfExporter
    {
        private const string RtfHeader = "{\\rtf1\\ansi\\deff0\n";
        private const string RtfFooter = "\n}";
        
        /// <summary>
        /// Converts the given HTML string (expected to contain a single table) into an RTF byte array.
        /// </summary>
        /// <param name="htmlContent">The HTML string containing the table to export.</param>
        /// <returns>A byte array representing the RTF document content.</returns>
        public byte[] ExportTableToRtf(string htmlContent)
        {
            Console.WriteLine("\n--- RTF Export Initiated (Parsing HTML Content) ---");

            // 1. Setup RTF color table (for basic white background and black text)
            // Color 0: Auto (default text color). Color 1: White (table background). Color 2: Black (fallback text color).
            string rtfColorTable = "{\\colortbl ;\\red255\\green255\\blue255;\\red0\\green0\\blue0;}\n";
            
            // 2. Extract the table body (the content between <tbody>...</tbody>)
            // Use Singleline option for easier parsing of multiline HTML
            Match tableBodyMatch = Regex.Match(htmlContent, @"<tbody.*?>(.*?)<\/tbody>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (!tableBodyMatch.Success)
            {
                // Fallback: try to match the whole table if no <tbody> is found
                tableBodyMatch = Regex.Match(htmlContent, @"<table.*?>(.*?)<\/table>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }

            if (!tableBodyMatch.Success)
            {
                Console.WriteLine("FAILURE: Could not find table content in HTML.");
                return Encoding.Default.GetBytes(RtfHeader + rtfColorTable + "\\pard\\sa200\\sl276\\slmult1\\f0\\fs24 Error: Could not parse table content.\\par" + RtfFooter);
            }

            string tableHtml = tableBodyMatch.Groups[1].Value;
            
            // 3. Build the RTF body
            StringBuilder rtfBody = new StringBuilder();

            // Match all table rows <tr>
            MatchCollection rowMatches = Regex.Matches(tableHtml, @"<tr.*?>(.*?)<\/tr>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // RTF table constants (A4 paper width in twips, 1 twip = 1/20 of a point)
            // A typical full page width is around 11900 twips. We use half of that for a narrow card style.
            const int totalTableWidthTwips = 7000;
            
            foreach (Match rowMatch in rowMatches)
            {
                string rowHtml = rowMatch.Groups[1].Value;
                MatchCollection cellMatches = Regex.Matches(rowHtml, @"<(td|th).*?>(.*?)<\/(td|th)>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

                if (cellMatches.Count == 0) continue;

                // Start of a new table row definition (table row properties)
                rtfBody.Append("{\\trowd\\trgaph108\\trleft-108\n"); // \\trgaph108 = 108 twips (standard) between cells

                int currentRowWidthTwips = 0;
                
                // Determine the width for each cell (simple equal width distribution)
                int cellWidthTwips = totalTableWidthTwips / cellMatches.Count;

                // Parse cells <td> or <th>
                for (int i = 0; i < cellMatches.Count; i++)
                {
                    Match cellMatch = cellMatches[i];
                    string cellHtml = cellMatch.Groups[0].Value;
                    string cellTag = cellMatch.Groups[1].Value.ToLower(); // td or th
                    string cellText = cellMatch.Groups[2].Value.Trim();
                    
                    // a) Check for basic formatting (e.g., is it a <th> or does it contain a <b> tag?)
                    bool isBold = cellTag.Equals("th") || Regex.IsMatch(cellHtml, @"<b\b[^>]*>|<\/b>", RegexOptions.IgnoreCase);
                    
                    // b) Define cell boundaries and color (bg color 1 = white)
                    int bgColorIndex = 1; // Default to white background
                    
                    // RTF Table cell definition: \clbrdr... are border definitions
                    // \cellx sets the right boundary of the cell relative to the table's left edge
                    currentRowWidthTwips += cellWidthTwips;

                    // \clbrdrt: border top, \clbrdrb: border bottom, \clbrdrl: border left, \clbrdrr: border right
                    // \brdrs: single border style, \brdrw10: width 10 twips, \clcbpat1: background color 1 (white)
                    rtfBody.Append($"\\clbrdrt\\brdrs\\brdrw10\\clbrdrb\\brdrs\\brdrw10\\clbrdrl\\brdrs\\brdrw10\\clbrdrr\\brdrs\\brdrw10\\clcbpat{bgColorIndex}\\cellx{currentRowWidthTwips}\n");

                    // c) Apply text formatting
                    string textFormatStart = isBold ? "{\\b " : "";
                    string textFormatEnd = isBold ? "\\b0}" : "";
                    
                    // d) Clean and append the text
                    // Use simple regex to strip remaining HTML tags like <i>, <p>, <span>, &diams;, etc., but preserve content
                    string cleanText = Regex.Replace(cellText, @"<[^>]*?>", ""); 
                    // Replace HTML entity for diamond (&diams;) with RTF bullet
                    cleanText = cleanText.Replace("&diams;", "{\\pard\\sa200\\sl276\\slmult1\\f0\\fs24 \\bullet\\tab }"); 
                    cleanText = cleanText.Replace(Environment.NewLine, "\\par "); // Replace newlines with RTF paragraph break
                    
                    // Escape RTF special characters (\, {, })
                    // Need to escape backslash first
                    cleanText = cleanText.Replace("\\", "\\\\").Replace("{", "\\{").Replace("}", "\\}");

                    // Apply text formatting and content
                    rtfBody.Append(textFormatStart);
                    rtfBody.Append(cleanText);
                    rtfBody.Append(textFormatEnd);
                    
                    rtfBody.Append("\\cell\n");
                }
                
                // End of row
                rtfBody.Append("}\n"); // Close the \trowd block
                rtfBody.Append("\\row\n");
            }
            
            // 4. Construct the full RTF string
            StringBuilder fullRtf = new StringBuilder();
            fullRtf.Append(RtfHeader);
            fullRtf.Append(rtfColorTable);
            
            // Standard formatting for default paragraph/text: \pard = paragraph default, \sa200 = space after, \sl276 = single line spacing
            fullRtf.Append("{\\fonttbl{\\f0 Arial;}}\n"); // Define a font
            fullRtf.Append("\\pard\\sa200\\sl276\\slmult1\\f0\\fs24\n"); // Apply default paragraph formatting (24 is 12pt)
            fullRtf.Append(rtfBody.ToString());
            fullRtf.Append(RtfFooter);

            // 5. Convert to byte array (RTF is typically ASCII/ANSI)
            // Using Encoding.Default which maps to the system's current ANSI code page.
            return Encoding.Default.GetBytes(fullRtf.ToString());
        }
    }

    #endregion
}
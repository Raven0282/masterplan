#nullable enable

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack; // Assumed dependency for parsing HTML

namespace Masterplan.Tools
{
    /// <summary>
    /// Service class responsible for converting HTML table content into a valid RTF string.
    /// </summary>
    public class RtfExporter
    {
        // Define the color table mapping for RTF based on the CSS provided in the HTML input.
        // The index in the RTF color table (\clccfN, \clcbpatN) will be N-1.
        // RTF Color Table: \redR\greenG\blueB;
        private const string RtfColorTable =
            "{\\colortbl;\\red0\\green0\\blue0;\\red255\\green255\\blue255;" + // 0: Black, 1: White (Default)
            "\\red225\\green231\\blue197;\\red20\\green61\\blue95;\\red91\\green159\\blue50;" + // 2: #E1E7C5, 3: #143D5F (creature heading), 4: #5B1F34 (trap/artifact)
            "\\red144\\green164\\blue141;\\red208\\green96\\blue21;\\red54\\green79\\blue39;" + // 5: #9FA48D (shaded), 6: #D06015 (item), 7: #364F27 (creature)
            "\\red229\\green160\\blue160;\\red35\\green142\\blue35;\\red139\\green0\\blue0;}";  // 8: #E5A0A0 (warning), 9: #238E23 (atwill), 10: #8B0000 (encounter)

        private Dictionary<string, int> CssToRtfColorIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            // Background Colors (using 1-based index for RTF reference)
            {"#E1E7C5", 2}, // tr
            {"#FFFFFF", 1}, // tr.clear
            {"#143D5F", 3}, // tr.heading, tr.hero (White text on this background)
            {"#5B1F34", 4}, // tr.trap, tr.artifact (White text on this background)
            {"#364F27", 7}, // tr.creature (White text on this background)
            {"#D06015", 6}, // tr.item (White text on this background)
            {"#9FA48D", 5}, // tr.shaded
            {"#238E23", 9}, // tr.atwill
            {"#8B0000", 10},// tr.encounter
            {"#E5A0A0", 8}, // tr.warning
            {"#000000", 0}, // tr.daily, tr.encounterlog, td.pvlogentry (White text on this background)
            
            // Text Color Overrides (not used for cell background, but helps mapping text color)
            {"#000000", 0},
            {"#FFFFFF", 1},
        };

        /// <summary>
        /// Converts the given HTML table content into a valid RTF string.
        /// </summary>
        /// <param name="htmlContent">The HTML string containing the table.</param>
        /// <returns>A string containing the RTF document content.</returns>
        public string ExportToRtf(string htmlContent)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var tableNode = doc.DocumentNode.SelectSingleNode("//table");
            if (tableNode == null)
            {
                return "{\\rtf1\\ansi\\deff0\\deflang1033{\\fonttbl{\\f0 Segoe UI;}}\\pard\\sa200\\sl276\\slmult1\\f0\\fs18 Could not find a table in the HTML content.\\par}";
            }

            var rtfBody = new StringBuilder();

            // 1. RTF Preamble (Defines document settings, font, and color table)
            rtfBody.Append("{\\rtf1\\ansi\\deff0\\deflang1033");
            rtfBody.Append("{\\fonttbl{\\f0 Segoe UI;}{\\f1 Times New Roman;}}"); // Define fonts
            rtfBody.Append(RtfColorTable); // Define color table
            rtfBody.Append("\\pard\\sa200\\sl276\\slmult1\\f0\\fs18\\qc\\par\\n"); // Default paragraph settings

            // 2. Determine Total Table Width (A standard page width is ~9000-11000 twips)
            // The HTML table has max-width: 400px. A sensible RTF width for a card is ~3 inches, or ~4500 twips.
            const int totalTableWidthTwips = 4500;
            // Assuming most rows have 3 columns max, calculate a base single cell width
            const int singleCellWidthTwips = totalTableWidthTwips / 3;

            var rows = tableNode.SelectNodes(".//tr");
            if (rows != null)
            {
                foreach (var rowNode in rows)
                {
                    var cells = rowNode.SelectNodes("./td");
                    if (cells == null) continue;

                    // Start of row definition: \trowd indicates table row definition
                    // \trgaph108: gutter spacing. \trleft-108: left indent.
                    // \trwWidth: total row width. Use a static width for consistency.
                    rtfBody.Append($"\\trowd\\trgaph108\\trleft-108\\trwWidth{totalTableWidthTwips}\\n");
                    
                    // Check if the row has a custom background class
                    string rowClass = rowNode.GetAttributeValue("class", string.Empty).ToLower();
                    int rowBgColorIndex = GetRowBackgroundColorIndex(rowClass);
                    int rowTextColorIndex = GetRowTextColorIndex(rowClass);

                    foreach (var cellNode in cells)
                    {
                        // a) Calculate cell width (using colspan)
                        int colspan = cellNode.GetAttributeValue("colspan", 1);
                        // Cell border spacing is usually 20 twips (a total of 40) + cell width.
                        int cellWidth = (int)(singleCellWidthTwips * colspan);
                        
                        // b) Process cell content
                        string cellText = cellNode.InnerHtml;
                        cellText = StripAndProcessHtml(cellText); // Clean up HTML tags and entities
                        
                        // Check if the cell itself has an explicit background color (e.g., from an inline style if present)
                        // Using the row color as the default.
                        int cellBgColorIndex = rowBgColorIndex;

                        // c) Cell Definition Commands:
                        // \clbrdrt, \clbrdrb, \clbrdrl, \clbrdrr: top, bottom, left, right border
                        // \brdrs: border style (single), \brdrw10: border width (10 twips)
                        // \clcbpatN: cell background color (N is index from \colortbl)
                        // \cellxN: right boundary of the cell (accumulative from the start of the table)
                        
                        // The boundary must be cumulative. We can use \clvmgf and \clvmrg for rowspan if needed, but for now, rely on colspan only.
                        
                        // For simplicity and to ensure borders render:
                        // Define all borders for every cell
                        string borderDef = "\\clbrdrt\\brdrs\\brdrw10\\clbrdrb\\brdrs\\brdrw10\\clbrdrl\\brdrs\\brdrw10\\clbrdrr\\brdrs\\brdrw10";
                        
                        // \clcbpatN: Cell background color index
                        string bgColorDef = $"\\clcbpat{cellBgColorIndex + 1}"; // +1 because RTF uses 1-based indexing for \colortbl

                        // \cellxN: Defines the *right edge* of the current cell. We approximate this.
                        // Instead of calculating cumulative width, use \cellx at end of each cell to simply end the cell.
                        // We will use \clwWidth to define the width for the entire table.

                        // Start cell (use \clvertalc for vertical alignment top, which is default for this HTML)
                        // We need a proper column definition before the \trowd or just rely on \cellx
                        // The simplest approach is to use \cellx to define the cell boundary based on the singleCellWidthTwips
                        
                        // For robust parsing, we need to calculate the cumulative right boundary for the \cellx command.
                        int cumulativeWidth = cellWidth; // Simplified: treat it as relative, Word often handles this.

                        // Final cell properties including borders and background
                        rtfBody.Append($"\\clvertalc{borderDef}{bgColorDef}\\cellx{cumulativeWidth}\\n");

                        // d) Apply text formatting
                        string textFormatStart = "";
                        string textFormatEnd = "";

                        // If row background suggests white text, set text color
                        if (rowTextColorIndex == 1) // White text
                        {
                            textFormatStart += "\\cf1 "; // White text color index is 1
                            textFormatEnd += "\\cf0";
                        }
                        
                        // Basic bolding check (The HTML content uses <B> which we've stripped, so we need to rely on the stripped content or an external heuristic)
                        // Since we stripped tags, the best we can do is rely on the row class (e.g., creature, heading)
                        bool isBold = rowClass.Contains("heading") || rowClass.Contains("creature") || rowClass.Contains("hero") || rowClass.Contains("item") || rowClass.Contains("artifact") || cellNode.InnerHtml.Contains("<B>");

                        if (isBold)
                        {
                            textFormatStart += "\\b ";
                            textFormatEnd += "\\b0";
                        }

                        // e) Append the text
                        rtfBody.Append(textFormatStart);
                        rtfBody.Append(cellText);
                        rtfBody.Append(textFormatEnd);
                        
                        // End cell
                        rtfBody.Append("\\cell\\n"); 
                    }
                    
                    // End of row
                    rtfBody.Append("\\row\\n"); // Close the row definition
                }
            }
            
            // 3. RTF Postamble
            rtfBody.Append("\\par\\n");
            rtfBody.Append("}"); // Close the RTF document
            
            return rtfBody.ToString();
        }

        /// <summary>
        /// Gets the RTF color index (0-based) for the given CSS class.
        /// </summary>
        private int GetRowBackgroundColorIndex(string rowClass)
        {
            if (rowClass.Contains("clear")) return CssToRtfColorIndex["#FFFFFF"] - 1; // White
            if (rowClass.Contains("creature")) return CssToRtfColorIndex["#364F27"] - 1; // Creature Green/Gray
            if (rowClass.Contains("heading") || rowClass.Contains("hero")) return CssToRtfColorIndex["#143D5F"] - 1; // Heading Blue
            if (rowClass.Contains("trap") || rowClass.Contains("artifact")) return CssToRtfColorIndex["#5B1F34"] - 1; // Trap/Artifact Dark Red
            if (rowClass.Contains("item")) return CssToRtfColorIndex["#D06015"] - 1; // Item Orange
            if (rowClass.Contains("shaded")) return CssToRtfColorIndex["#9FA48D"] - 1; // Shaded Light Green/Gray
            if (rowClass.Contains("atwill")) return CssToRtfColorIndex["#238E23"] - 1; // At-Will Green
            if (rowClass.Contains("encounter")) return CssToRtfColorIndex["#8B0000"] - 1; // Encounter Red
            if (rowClass.Contains("daily") || rowClass.Contains("encounterlog")) return CssToRtfColorIndex["#000000"] - 1; // Daily/Log Black
            if (rowClass.Contains("warning")) return CssToRtfColorIndex["#E5A0A0"] - 1; // Warning Pink
            
            return CssToRtfColorIndex["#E1E7C5"] - 1; // Default: Light Beige
        }

        /// <summary>
        /// Determines if the text should be white based on the row background.
        /// Returns 1 for white, 0 for black (default).
        /// </summary>
        private int GetRowTextColorIndex(string rowClass)
        {
            // Rows that use white text on a dark background
            if (rowClass.Contains("creature") || rowClass.Contains("heading") || rowClass.Contains("hero") ||
                rowClass.Contains("trap") || rowClass.Contains("artifact") || rowClass.Contains("item") ||
                rowClass.Contains("atwill") || rowClass.Contains("encounter") || rowClass.Contains("daily") ||
                rowClass.Contains("encounterlog"))
            {
                return 1; // White text (index 1 in color table)
            }
            return 0; // Black text (index 0 in color table is Black, but we rely on RTF default for 0)
        }

        /// <summary>
        /// Strips all HTML tags and converts common HTML entities to RTF-safe text.
        /// </summary>
        private string StripAndProcessHtml(string html)
        {
            // 1. Remove all <img> tags (cannot be embedded easily in this format)
            string cleanText = Regex.Replace(html, "<img[^>]*?>", "", RegexOptions.IgnoreCase);
            
            // 2. Remove all remaining HTML tags (like <B>, <BR>, <P>, etc.)
            cleanText = Regex.Replace(cleanText, "<[^>]*?>", " ", RegexOptions.IgnoreCase | RegexOptions.Multiline); 

            // 3. Decode common HTML entities (e.g., &diams; to a Unicode diamond or a simple *)
            cleanText = cleanText.Replace("&diams;", " * ")
                                 .Replace("&rsquo;", "'")
                                 .Replace("&amp;", "\\&")
                                 .Replace("&nbsp;", " ");

            // 4. Escape RTF special characters (must be done AFTER HTML cleanup)
            cleanText = cleanText.Replace("\\", "\\\\") // Escape backslashes first
                                 .Replace("{", "\\{")
                                 .Replace("}", "\\}")
                                 .Replace(Environment.NewLine, "\\par "); // Replace newlines with RTF paragraph break

            return cleanText.Trim();
        }
    }
}
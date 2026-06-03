#nullable disable

using Masterplan.Data;
using Masterplan.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace Masterplan.Tools
{
    /// <summary>
    /// Text size for HTML display.
    /// </summary>
    public enum DisplaySize
    {
        /// <summary>
        /// Small text size (e.g., 8pt).
        /// </summary>
        Small,

        /// <summary>
        /// Medium text size (e.g., 10pt).
        /// </summary>
        Medium,

        /// <summary>
        /// Large text size (e.g., 12pt).
        /// </summary>
        Large,

        /// <summary>
        /// Extra large text size (e.g., 14pt).
        /// </summary>
        ExtraLarge
    }

    /// <summary>
    /// Enumeration for the layout format width.
    /// </summary>
    public enum EditFormat
    {
        /// <summary>
        /// Narrow Format Editing (e.g., sidebars).
        /// </summary>
        Narrow,

        /// <summary>
        /// Full Format Editing (e.g., main content).
        /// </summary>
        Wide
    }

    /// <summary>
    /// Generates HTML content for various data types within the application.
    /// This is the core file containing common helper methods, enums, and CSS generation.
    /// </summary>
    public static partial class HTML // NOTE: Made partial to allow splitting across files
    {
        // --- PUBLIC HELPERS ---

        /// <summary>
        /// Combines a list of strings into a single string.
        /// </summary>
        /// <param name="lines">The list of strings.</param>
        /// <returns>The concatenated string.</returns>
        public static string Concatenate(List<string> lines)
        {
            return String.Join("\n", lines.ToArray());
        }

        /// <summary>
        /// Generates the HTML header and CSS styles.
        /// </summary>
        /// <param name="size">The desired display text size.</param>
        /// <returns>A list of strings containing the HTML head and style definitions.</returns>
        public static List<string> GetStyle(DisplaySize size)
        {
            List<string> lines = new List<string>();

            lines.Add("<HEAD>");
            lines.Add("<STYLE>");

            // Define the base font size based on DisplaySize
            int fontsize = 0;
            switch (size)
            {
                case DisplaySize.Small: fontsize = 8; break;
                case DisplaySize.Medium: fontsize = 10; break;
                case DisplaySize.Large: fontsize = 12; break;
                case DisplaySize.ExtraLarge: fontsize = 14; break;
            }

            // --- CSS Definitions ---

            lines.Add("BODY { font-family: Tahoma; font-size: " + fontsize + "pt; }");
            lines.Add("P { margin: 0; padding: 0; }");
            lines.Add("TABLE { border-collapse: collapse; width: 100%; }");

            // Stat Block and Card Specific Styles
            lines.Add(".table { border-style: solid; border-width: 1px; border-color: #666666; padding: 0.5em; }");
            lines.Add(".heading { font-size: 1.2em; font-weight: bold; }");
            lines.Add(".subheading { font-weight: bold; margin-top: 0.5em; }");
            lines.Add(".line { border-bottom-style: solid; border-bottom-width: 1px; border-bottom-color: #000000; margin-top: 0.5em; margin-bottom: 0.5em; }");
            lines.Add(".caption { font-size: 0.8em; margin-top: 0.5em; text-align: right; font-style: italic; }");
            lines.Add(".flavour { font-style: italic; }");
            lines.Add(".instruction { color: #808080; }");

            // Combat / Initiative Specific
            lines.Add(".combat-data { border-style: solid; border-width: 1px; border-color: #666666; background-color: #FF0000; color: #FFFFFF; font-weight: bold; padding: 0.2em; margin-bottom: 0.5em; }");
            lines.Add(".combat-data-white { border-style: solid; border-width: 1px; border-color: #666666; background-color: #FFFFFF; color: #000000; font-weight: bold; padding: 0.2em; margin-bottom: 0.5em; }");

            lines.Add("</STYLE>");
            lines.Add("</HEAD>");

            return lines;
        }

        // --- PRIVATE HELPERS ---
        public static string Process(string raw_text, bool strip_html)
        {
            if (raw_text == null)
                return "";

            List<Pair<string, string>> pairs = new List<Pair<string, string>>();

            pairs.Add(new Pair<string, string>("&", "&amp;"));
            pairs.Add(new Pair<string, string>("Á", "&Aacute;"));
            pairs.Add(new Pair<string, string>("á", "&aacute;"));
            pairs.Add(new Pair<string, string>("À", "&Agrave;"));
            pairs.Add(new Pair<string, string>("Â", "&Acirc;"));
            pairs.Add(new Pair<string, string>("à", "&agrave;"));
            pairs.Add(new Pair<string, string>("Â", "&Acirc;"));
            pairs.Add(new Pair<string, string>("â", "&acirc;"));
            pairs.Add(new Pair<string, string>("Ä", "&Auml;"));
            pairs.Add(new Pair<string, string>("ä", "&auml;"));
            pairs.Add(new Pair<string, string>("Ã", "&Atilde;"));
            pairs.Add(new Pair<string, string>("ã", "&atilde;"));
            pairs.Add(new Pair<string, string>("Å", "&Aring;"));
            pairs.Add(new Pair<string, string>("å", "&aring;"));
            pairs.Add(new Pair<string, string>("Æ", "&Aelig;"));
            pairs.Add(new Pair<string, string>("æ", "&aelig;"));
            pairs.Add(new Pair<string, string>("Ç", "&Ccedil;"));
            pairs.Add(new Pair<string, string>("ç", "&ccedil;"));
            pairs.Add(new Pair<string, string>("Ð", "&Eth;"));
            pairs.Add(new Pair<string, string>("ð", "&eth;"));
            pairs.Add(new Pair<string, string>("É", "&Eacute;"));
            pairs.Add(new Pair<string, string>("é", "&eacute;"));
            pairs.Add(new Pair<string, string>("È", "&Egrave;"));
            pairs.Add(new Pair<string, string>("è", "&egrave;"));
            pairs.Add(new Pair<string, string>("Ê", "&Ecirc;"));
            pairs.Add(new Pair<string, string>("ê", "&ecirc;"));
            pairs.Add(new Pair<string, string>("Ë", "&Euml;"));
            pairs.Add(new Pair<string, string>("ë", "&euml;"));
            pairs.Add(new Pair<string, string>("Í", "&Iacute;"));
            pairs.Add(new Pair<string, string>("í", "&iacute;"));
            pairs.Add(new Pair<string, string>("Ì", "&Igrave;"));
            pairs.Add(new Pair<string, string>("ì", "&igrave;"));
            pairs.Add(new Pair<string, string>("Î", "&Icirc;"));
            pairs.Add(new Pair<string, string>("î", "&icirc;"));
            pairs.Add(new Pair<string, string>("Ï", "&Iuml;"));
            pairs.Add(new Pair<string, string>("ï", "&iuml;"));
            pairs.Add(new Pair<string, string>("Ñ", "&Ntilde;"));
            pairs.Add(new Pair<string, string>("ñ", "&ntilde;"));
            pairs.Add(new Pair<string, string>("Ó", "&Oacute;"));
            pairs.Add(new Pair<string, string>("ó", "&oacute;"));
            pairs.Add(new Pair<string, string>("Ò", "&Ograve;"));
            pairs.Add(new Pair<string, string>("ò", "&ograve;"));
            pairs.Add(new Pair<string, string>("Ô", "&Ocirc;"));
            pairs.Add(new Pair<string, string>("ô", "&ocirc;"));
            pairs.Add(new Pair<string, string>("Ö", "&Ouml;"));
            pairs.Add(new Pair<string, string>("ö", "&ouml;"));
            pairs.Add(new Pair<string, string>("Õ", "&Otilde;"));
            pairs.Add(new Pair<string, string>("õ", "&otilde;"));
            pairs.Add(new Pair<string, string>("Ø", "&Oslash;"));
            pairs.Add(new Pair<string, string>("ø", "&oslash;"));
            pairs.Add(new Pair<string, string>("ß", "&szlig;"));
            pairs.Add(new Pair<string, string>("Þ", "&Thorn;"));
            pairs.Add(new Pair<string, string>("þ", "&thorn;"));
            pairs.Add(new Pair<string, string>("Ú", "&Uacute;"));
            pairs.Add(new Pair<string, string>("ú", "&uacute;"));
            pairs.Add(new Pair<string, string>("Ù", "&Ugrave;"));
            pairs.Add(new Pair<string, string>("ù", "&ugrave;"));
            pairs.Add(new Pair<string, string>("Û", "&Ucirc;"));
            pairs.Add(new Pair<string, string>("û", "&ucirc;"));
            pairs.Add(new Pair<string, string>("Ü", "&Uuml;"));
            pairs.Add(new Pair<string, string>("ü", "&uuml;"));
            pairs.Add(new Pair<string, string>("Ý", "&Yacute;"));
            pairs.Add(new Pair<string, string>("ý", "&yacute;"));
            pairs.Add(new Pair<string, string>("ÿ", "&yuml;"));
            pairs.Add(new Pair<string, string>("©", "&copy;"));
            pairs.Add(new Pair<string, string>("®", "&reg;"));
            pairs.Add(new Pair<string, string>("™", "&trade;"));
            pairs.Add(new Pair<string, string>("€", "&euro;"));
            pairs.Add(new Pair<string, string>("¢", "&cent;"));
            pairs.Add(new Pair<string, string>("£", "&pound;"));
            pairs.Add(new Pair<string, string>("‘", "&lsquo;"));
            pairs.Add(new Pair<string, string>("’", "&rsquo;"));
            pairs.Add(new Pair<string, string>("“", "&ldquo;"));
            pairs.Add(new Pair<string, string>("”", "&rdquo;"));
            pairs.Add(new Pair<string, string>("«", "&laquo;"));
            pairs.Add(new Pair<string, string>("»", "&raquo;"));
            pairs.Add(new Pair<string, string>("—", "&mdash;"));
            pairs.Add(new Pair<string, string>("–", "&ndash;"));
            pairs.Add(new Pair<string, string>("°", "&deg;"));
            pairs.Add(new Pair<string, string>("±", "&plusmn;"));
            pairs.Add(new Pair<string, string>("¼", "&frac14;"));
            pairs.Add(new Pair<string, string>("½", "&frac12;"));
            pairs.Add(new Pair<string, string>("¾", "&frac34;"));
            pairs.Add(new Pair<string, string>("×", "&times;"));
            pairs.Add(new Pair<string, string>("÷", "&divide;"));
            pairs.Add(new Pair<string, string>("α", "&alpha;"));
            pairs.Add(new Pair<string, string>("β", "&beta;"));
            pairs.Add(new Pair<string, string>("∞", "&infin;"));

            if (strip_html)
            {
                pairs.Add(new Pair<string, string>("\"", "&quot;"));

                pairs.Add(new Pair<string, string>("<", "&lt;"));
                pairs.Add(new Pair<string, string>(">", "&gt;"));
            }

            string html = raw_text;
            foreach (Pair<string, string> pair in pairs)
                html = html.Replace(pair.First, pair.Second);

            return html;
        }



        /// <summary>
        /// Cleans a string by converting line breaks to HTML breaks and replacing special characters.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>The cleaned string.</returns>
        public static string ConvertLineBreakToBR(string str)
        {
            if (str == null)
                return "";

            string result = str.Replace("\n", "<BR/>");
            result = result.Replace(" ", "&nbsp;"); // Maintain spaces
            return result;
        }
        
        /// <summary>
        /// Converts HTML breaks back to standard line breaks (\n).
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>The converted string.</returns>
        private static string ConvertBRToLineBreaks(string str)
        {
            if (str == null)
                return "";

            string result = str.Replace("<BR/>", "\n");
            result = result.Replace("<br/>", "\n");
            result = result.Replace("<BR>", "\n");
            result = result.Replace("<br>", "\n");
            return result;
        }

        /// <summary>
        /// Helper to wrap content in a paragraph tag.
        /// </summary>
        /// <param name="text">The text to wrap.</param>
        /// <returns>The wrapped HTML string.</returns>
        private static string Wrap(string text)
        {
            return "<P>" + text + "</P>";
        }

        // **********************************************
        // NOTE: The rest of the original helper methods (get_filename, 
        // get_map_name, get_time, get_map_link, get_map_tile) that rely
        // on internal fields like 'fFullPath' will be kept in the main 
        // orchestrator class (ExportProject.cs) or moved into a dedicated 
        // FileNaming helper class if 'HTML.cs' is strictly for content generation.
        // For now, to minimize breaking changes, we will assume they will be
        // distributed into the relevant partial files that need them.
        // **********************************************
    }
}
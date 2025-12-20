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

        /// <summary>
        /// Cleans a string by converting line breaks to HTML breaks and replacing special characters.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>The cleaned string.</returns>
        private static string Process(string str)
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
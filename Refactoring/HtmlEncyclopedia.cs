#nullable disable

using Masterplan.Data;
using Masterplan.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Masterplan.Tools
{
    /// <summary>
    /// PARTIAL CLASS: ENCYCLOPEDIA & HANDOUTS
    /// Handles the generation of HTML for Encyclopedia entries, groups, and generic handouts.
    /// This class relies on helper methods defined in HtmlCore.cs.
    /// </summary>
    public static partial class HTML
    {
        // --- ENCYCLOPEDIA ENTRY GENERATION ---

        /// <summary>
        /// Generates the HTML for a single Encyclopedia Entry.
        /// </summary>
        /// <param name="entry">The EncyclopediaEntry data.</param>
        /// <param name="edit_format">The width format for display.</param>
        /// <param name="size">The display size.</param>
        /// <returns>The HTML string.</returns>
        public static string EncyclopediaEntry(EncyclopediaEntry entry, EditFormat edit_format, DisplaySize size)
        {
            List<string> lines = new List<string>();

            lines.Add("<HTML>");
            lines.AddRange(HTML.GetStyle(size)); // Dependency on HtmlCore.GetStyle
            lines.Add("<BODY>");

            if (entry != null)
            {
                lines.Add("<H3>" + Process(entry.Name) + "</H3>"); // Dependency on HtmlCore.Process
                lines.Add(Wrap(Process(entry.Content))); // Dependency on HtmlCore.Wrap

                // Display links if available
                if (entry.Links.Count > 0)
                {
                    lines.Add("<P class=subheading>See also:</P>");
                    lines.Add("<UL>");
                    foreach (Link link in entry.Links)
                    {
                        lines.Add("<LI>" + link.Name + "</LI>"); // Simple display for now, full implementation would require link handling
                    }
                    lines.Add("</UL>");
                }
            }
            else
            {
                lines.Add("<P class=instruction>(no encyclopedia entry selected)</P>");
            }

            lines.Add("</BODY>");
            lines.Add("</HTML>");

            return Concatenate(lines); // Dependency on HtmlCore.Concatenate
        }

        // --- ENCYCLOPEDIA GROUP GENERATION ---

        /// <summary>
        /// Generates the HTML for an Encyclopedia Group summary.
        /// </summary>
        /// <param name="group">The EncyclopediaGroup data.</param>
        /// <param name="size">The display size.</param>
        /// <returns>The HTML string.</returns>
        public static string EncyclopediaGroup(EncyclopediaGroup group, DisplaySize size)
        {
            List<string> lines = new List<string>();

            lines.Add("<HTML>");
            lines.AddRange(HTML.GetStyle(size));
            lines.Add("<BODY>");

            if (group != null)
            {
                lines.Add("<H2>" + Process(group.Name) + "</H2>");
                lines.Add(Wrap(Process(group.Details)));

                // List the members of the group
                lines.Add("<P class=subheading>Entries:</P>");
                lines.Add("<UL>");
                foreach (EncyclopediaEntry entry in group.Entries)
                {
                    lines.Add("<LI>" + Process(entry.Name) + "</LI>");
                }
                lines.Add("</UL>");
            }
            else
            {
                lines.Add("<P class=instruction>(no encyclopedia group selected)</P>");
            }

            lines.Add("</BODY>");
            lines.Add("</HTML>");

            return Concatenate(lines);
        }

        // --- HANDOUT GENERATION ---

        /// <summary>
        /// Generates the HTML for a generic Handout (a simple text element).
        /// </summary>
        /// <param name="handout">The Handout data.</param>
        /// <param name="edit_format">The width format for display.</param>
        /// <param name="size">The display size.</param>
        /// <returns>The HTML string.</returns>
        public static string Handout(Handout handout, EditFormat edit_format, DisplaySize size)
        {
            List<string> lines = new List<string>();

            lines.Add("<HTML>");
            lines.AddRange(HTML.GetStyle(size));
            lines.Add("<BODY>");

            if (handout != null)
            {
                lines.Add("<H3>" + Process(handout.Name) + "</H3>");
                lines.Add(Wrap(Process(handout.Content)));
            }
            else
            {
                lines.Add("<P class=instruction>(no handout selected)</P>");
            }

            lines.Add("</BODY>");
            lines.Add("</HTML>");

            return Concatenate(lines);
        }

        // --- PUBLIC HELPERS (Accessors) ---

        /// <summary>
        /// Helper method to retrieve the HTML for an encyclopedia entry.
        /// </summary>
        public static string get_encyclopedia_entry(EncyclopediaEntry entry, DisplaySize size)
        {
            return EncyclopediaEntry(entry, EditFormat.Wide, size);
        }
        
        /// <summary>
        /// Helper method to retrieve the HTML for an encyclopedia group.
        /// </summary>
        public static string get_encyclopedia_group(EncyclopediaGroup group, DisplaySize size)
        {
            return EncyclopediaGroup(group, size);
        }

        /// <summary>
        /// Helper method to retrieve the HTML for a generic handout.
        /// </summary>
        public static string get_handout(Handout handout, DisplaySize size)
        {
            return Handout(handout, EditFormat.Wide, size);
        }
    }
}
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
    /// PARTIAL CLASS: GAMEPLAY & TOOLS
    /// Handles the generation of HTML for traps, skill challenges, encounter reports, and other tools.
    /// This class relies on helper methods defined in HtmlCore.cs.
    /// </summary>
    public static partial class HTML
    {
        // --- TRAP GENERATION ---

        /// <summary>
        /// Generates the HTML for a single Trap.
        /// </summary>
        /// <param name="trap">The Trap data.</param>
        /// <param name="mode">The card mode.</param>
        /// <param name="size">The display size.</param>
        /// <returns>The HTML string.</returns>
        public static string Trap(Trap trap, CardMode mode, DisplaySize size)
        {
            List<string> lines = new List<string>();

            lines.Add("<HTML>");
            lines.AddRange(HTML.GetStyle(size)); // Dependency on HtmlCore.GetStyle
            lines.Add("<BODY>");

            if (trap != null)
            {
                lines.Add("<P class=table>");
                lines.AddRange(trap.AsText(mode)); // Assuming AsText is a method on Trap
                lines.Add("</P>");
            }
            else
            {
                lines.Add("<P class=instruction>(no trap selected)</P>");
            }

            lines.Add("</BODY>");
            lines.Add("</HTML>");

            return Concatenate(lines); // Dependency on HtmlCore.Concatenate
        }

        // --- SKILL CHALLENGE GENERATION ---

        /// <summary>
        /// Generates the HTML for a single Skill Challenge.
        /// </summary>
        /// <param name="sc">The SkillChallenge data.</param>
        /// <param name="edit_format">The width format for display.</param>
        /// <param name="size">The display size.</param>
        /// <returns>The HTML string.</returns>
        public static string SkillChallenge(SkillChallenge sc, EditFormat edit_format, DisplaySize size)
        {
            List<string> lines = new List<string>();

            lines.Add("<HTML>");
            lines.AddRange(HTML.GetStyle(size));
            lines.Add("<BODY>");

            if (sc != null)
            {
                lines.Add("<H3>" + Process(sc.Name) + "</H3>"); // Dependency on HtmlCore.Process

                // Assuming SkillChallenge has a detailed text representation method
                lines.Add("<P class=table>");
                lines.AddRange(sc.AsText(CardMode.StatBlock));
                lines.Add("</P>");
            }
            else
            {
                lines.Add("<P class=instruction>(no skill challenge selected)</P>");
            }

            lines.Add("</BODY>");
            lines.Add("</HTML>");

            return Concatenate(lines);
        }

        // --- ENCOUNTER REPORT TABLE GENERATION ---

        /// <summary>
        /// Generates the HTML for a formatted encounter report table.
        /// </summary>
        /// <param name="report">The EncounterReport data.</param>
        /// <param name="size">The display size.</param>
        /// <returns>The HTML string.</returns>
        public static string EncounterReportTable(EncounterReport report, DisplaySize size)
        {
            List<string> lines = new List<string>();

            lines.Add("<HTML>");
            lines.AddRange(HTML.GetStyle(size));
            lines.Add("<BODY>");
            lines.Add("<H2>Encounter Report: " + Process(report.Name) + "</H2>");

            // Generate report content (simplified example)
            lines.Add("<TABLE class=table>");
            lines.Add("<TR><TD class=subheading>Section</TD><TD class=subheading>Details</TD></TR>");
            lines.Add("<TR><TD>XP Budget:</TD><TD>" + report.XpBudget + "</TD></TR>");
            lines.Add("<TR><TD>Total XP:</TD><TD>" + report.TotalXp + "</TD></TR>");
            lines.Add("</TABLE>");

            lines.Add("</BODY>");
            lines.Add("</HTML>");

            return Concatenate(lines);
        }

        // --- PUBLIC HELPERS (Accessors) ---

        /// <summary>
        /// Helper method to retrieve the HTML for a trap.
        /// </summary>
        public static string get_trap(Trap trap, DisplaySize size)
        {
            return Trap(trap, CardMode.StatBlock, size);
        }

        /// <summary>
        /// Helper method to retrieve the HTML for a skill challenge.
        /// </summary>
        public static string get_skill_challenge(SkillChallenge sc, DisplaySize size)
        {
            return SkillChallenge(sc, EditFormat.Wide, size);
        }

        // NOTE: Other gameplay-related functions like get_terrain_power, 
        // get_custom_map_token, and get_party_breakdown will be included here 
        // based on the final structure of the original HTML.cs file.
    }
}
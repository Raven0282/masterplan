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
    /// PARTIAL CLASS: ITEMS & ARTIFACTS
    /// Handles the generation of HTML for magic items, artifacts, and loot parcels.
    /// This class relies on helper methods defined in HtmlCore.cs.
    /// </summary>
    public static partial class HTML
    {
        // --- MAGIC ITEM GENERATION ---

        /// <summary>
        /// Generates the HTML for a single MagicItem.
        /// </summary>
        /// <param name="item">The MagicItem data.</param>
        /// <param name="mode">The card mode.</param>
        /// <param name="size">The display size.</param>
        /// <returns>The HTML string.</returns>
        public static string MagicItem(MagicItem item, CardMode mode, DisplaySize size)
        {
            List<string> lines = new List<string>();

            lines.Add("<HTML>");
            lines.AddRange(HTML.GetStyle(size)); // Dependency on HtmlCore.GetStyle
            lines.Add("<BODY>");

            if (item != null)
            {
                lines.Add("<P class=table>");
                lines.AddRange(item.AsText(mode)); // Assuming AsText is a method on MagicItem
                lines.Add("</P>");
            }
            else
            {
                lines.Add("<P class=instruction>(no magic item selected)</P>");
            }

            lines.Add("</BODY>");
            lines.Add("</HTML>");

            return Concatenate(lines); // Dependency on HtmlCore.Concatenate
        }

        // --- ARTIFACT GENERATION ---

        /// <summary>
        /// Generates the HTML for a single Artifact.
        /// </summary>
        /// <param name="artifact">The Artifact data.</param>
        /// <param name="mode">The card mode.</param>
        /// <param name="size">The display size.</param>
        /// <returns>The HTML string.</returns>
        public static string Artifact(Artifact artifact, CardMode mode, DisplaySize size)
        {
            List<string> lines = new List<string>();

            lines.Add("<HTML>");
            lines.AddRange(HTML.GetStyle(size)); // Dependency on HtmlCore.GetStyle
            lines.Add("<BODY>");

            if (artifact != null)
            {
                lines.Add("<P class=table>");
                lines.AddRange(artifact.AsText(mode)); // Assuming AsText is a method on Artifact
                lines.Add("</P>");
            }
            else
            {
                lines.Add("<P class=instruction>(no artifact selected)</P>");
            }

            lines.Add("</BODY>");
            lines.Add("</HTML>");

            return Concatenate(lines); // Dependency on HtmlCore.Concatenate
        }

        // --- PUBLIC HELPERS (Accessors) ---

        /// <summary>
        /// Helper method to retrieve the HTML for a magic item.
        /// </summary>
        public static string get_magic_item(MagicItem item, DisplaySize size)
        {
            return MagicItem(item, CardMode.StatBlock, size);
        }

        /// <summary>
        /// Helper method to retrieve the HTML for an artifact.
        /// </summary>
        public static string get_artifact(Artifact artifact, DisplaySize size)
        {
            return Artifact(artifact, CardMode.StatBlock, size);
        }
        
        /// <summary>
        /// Generates a list of HTML lines describing a list of loot parcels.
        /// </summary>
        /// <param name="parcels">The list of parcels.</param>
        /// <param name="size">The display size.</param>
        /// <returns>The HTML string.</returns>
        public static string get_parcels(List<Parcel> parcels, DisplaySize size)
        {
            List<string> lines = new List<string>();

            lines.Add("<HTML>");
            lines.AddRange(HTML.GetStyle(size)); // Dependency on HtmlCore.GetStyle
            lines.Add("<BODY>");
            lines.Add("<H3>Loot Parcels</H3>");

            if (parcels.Count == 0)
            {
                lines.Add("<P class=instruction>(no loot parcels)</P>");
            }
            else
            {
                foreach (Parcel parcel in parcels)
                {
                    lines.Add("<P class=table>");
                    // Assuming Parcel has an AsText method or similar utility
                    lines.Add(Wrap(Process(parcel.ToString()))); // Dependency on HtmlCore.Wrap and HtmlCore.Process
                    lines.Add("</P>");
                }
            }

            lines.Add("</BODY>");
            lines.Add("</HTML>");

            return Concatenate(lines);
        }
    }
}
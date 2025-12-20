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
    /// PARTIAL CLASS: PLAYER OPTIONS
    /// Handles the generation of HTML for races, classes, feats, backgrounds, and powers.
    /// This class relies on helper methods defined in HtmlCore.cs.
    /// </summary>
    public static partial class HTML
    {
        // --- PLAYER OPTION GENERATION (Generic) ---

        /// <summary>
        /// Generates the HTML for a generic Player Option (Race, Class, Feat, etc.).
        /// </summary>
        /// <param name="option">The generic IPlayerOption data.</param>
        /// <param name="mode">The card mode.</param>
        /// <param name="size">The display size.</param>
        /// <returns>The HTML string.</returns>
        public static string PlayerOption(IPlayerOption option, CardMode mode, DisplaySize size)
        {
            List<string> lines = new List<string>();

            lines.Add("<HTML>");
            lines.AddRange(HTML.GetStyle(size)); // Dependency on HtmlCore.GetStyle
            lines.Add("<BODY>");

            if (option != null)
            {
                lines.Add("<P class=table>");
                lines.AddRange(option.AsText(mode)); // Assuming AsText is a method on IPlayerOption
                lines.Add("</P>");
            }
            else
            {
                lines.Add("<P class=instruction>(no player option selected)</P>");
            }

            lines.Add("</BODY>");
            lines.Add("</HTML>");

            return Concatenate(lines); // Dependency on HtmlCore.Concatenate
        }

        // --- POWER GENERATION ---

        /// <summary>
        /// Generates the HTML for a Player Power.
        /// </summary>
        /// <param name="power">The PlayerPower data.</param>
        /// <param name="mode">The card mode.</param>
        /// <param name="size">The display size.</param>
        /// <returns>The HTML string.</returns>
        public static string PlayerPower(PlayerPower power, CardMode mode, DisplaySize size)
        {
            List<string> lines = new List<string>();

            lines.Add("<HTML>");
            lines.AddRange(HTML.GetStyle(size));
            lines.Add("<BODY>");

            if (power != null)
            {
                lines.Add("<P class=table>");
                lines.AddRange(power.AsText(mode)); // Assuming AsText is a method on PlayerPower
                lines.Add("</P>");
            }
            else
            {
                lines.Add("<P class=instruction>(no power selected)</P>");
            }

            lines.Add("</BODY>");
            lines.Add("</HTML>");

            return Concatenate(lines);
        }

        // --- PUBLIC HELPERS (Accessors) ---

        /// <summary>
        /// Helper method to retrieve the HTML for a generic player option.
        /// </summary>
        public static string get_player_option(IPlayerOption option, DisplaySize size)
        {
            return PlayerOption(option, CardMode.StatBlock, size);
        }

        /// <summary>
        /// Helper method to retrieve the HTML for a player power.
        /// </summary>
        public static string get_player_power(PlayerPower power, DisplaySize size)
        {
            return PlayerPower(power, CardMode.StatBlock, size);
        }
    }
}
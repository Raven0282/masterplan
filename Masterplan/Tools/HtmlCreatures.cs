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
    /// PARTIAL CLASS: CREATURES & HEROES
    /// Handles stat blocks, creature templates, and hero details.
    /// This class relies on helper methods defined in HtmlCore.cs.
    /// </summary>
    public static partial class HTML
    {
        // --- STAT BLOCK GENERATION ---

        /// <summary>
        /// Generates the full HTML for a creature's stat block.
        /// </summary>
        /// <param name="card">The creature's encounter card data.</param>
        /// <param name="data">The combat data (HP, temporary HP, conditions).</param>
        /// <param name="enc">The encounter the creature is part of.</param>
        /// <param name="include_wrapper">If true, includes the HTML/BODY tags.</param>
        /// <param name="initiative_holder">If true, includes initiative controls.</param>
        /// <param name="full">If true, includes combat data display.</param>
        /// <param name="mode">The display mode for the card.</param>
        /// <param name="size">The display size.</param>
        /// <returns>The HTML string.</returns>
        public static string StatBlock(EncounterCard card, CombatData data, Encounter enc, bool include_wrapper,
            bool initiative_holder, bool full, CardMode mode, DisplaySize size)
        {
            List<string> lines = new List<string>();

            if (include_wrapper)
            {
                lines.Add("<HTML>");
                lines.AddRange(HTML.GetStyle(size)); // Dependency on HtmlCore.GetStyle
                lines.Add("<BODY>");
            }

            if (full)
            {
                // Check if the creature needs to be dragged onto a map
                if ((data != null) && (data.Location == CombatData.NoPoint) && (enc != null) && (enc.MapID != Guid.Empty))
                {
                    lines.Add("<P class=instruction>Drag this creature from the list onto the map.</P>");
                }

                if (data != null)
                    lines.AddRange(get_combat_data(data, card.HP, enc, initiative_holder)); // Dependency on get_combat_data
            }

            if (card != null)
            {
                lines.Add("<P class=table>");
                lines.AddRange(card.AsText(data, mode, full)); // Assuming AsText is a method on EncounterCard
                lines.Add("</P>");
            }
            else
            {
                lines.Add("<P class=instruction>(no creature selected)</P>");
            }

            if (include_wrapper)
            {
                lines.Add("</BODY>");
                lines.Add("</HTML>");
            }

            return Concatenate(lines); // Dependency on HtmlCore.Concatenate
        }

        // --- HERO STAT BLOCK OVERLOAD ---

        /// <summary>
        /// Generates the HTML for a Hero's stat block (overload).
        /// </summary>
        public static string StatBlock(Hero hero, Encounter enc, bool include_wrapper,
            bool initiative_holder, bool full, CardMode mode, DisplaySize size)
        {
            List<string> lines = new List<string>();

            if (include_wrapper)
            {
                lines.Add("<HTML>");
                lines.AddRange(HTML.GetStyle(size));
                lines.Add("<BODY>");
            }

            if (full)
            {
                // Logic for hero combat data, identical to the creature StatBlock
                if (hero.CombatData != null)
                    lines.AddRange(get_combat_data(hero.CombatData, hero.HP, enc, initiative_holder));
            }

            // Hero specific card text generation
            lines.Add("<P class=table>");
            lines.AddRange(hero.AsText(mode, full));
            lines.Add("</P>");

            if (include_wrapper)
            {
                lines.Add("</BODY>");
                lines.Add("</HTML>");
            }

            return Concatenate(lines);
        }

        // --- PRIVATE HELPER METHODS ---

        /// <summary>
        /// Generates HTML lines for combat tracking data (HP, temp HP, conditions).
        /// </summary>
        private static List<string> get_combat_data(CombatData data, int max_hp, Encounter enc, bool initiative_holder)
        {
            List<string> lines = new List<string>();

            // Determine background color based on creature state
            string css_class = (data.Removed) ? "combat-data-white" : "combat-data";

            lines.Add("<TABLE class=" + css_class + ">");

            // Row 1: Name and State
            lines.Add("<TR>");
            lines.Add($"<TD colspan=\"2\">{Process(data.Name)}</TD>");
            lines.Add("</TR>");

            // Row 2: HP and Conditions
            lines.Add("<TR>");
            // HP Data
            lines.Add($"<TD>HP: {data.HP} / {max_hp} ({data.TempHP} temp)</TD>");

            // Conditions
            lines.Add("<TD>");
            if (data.Conditions.Count != 0)
            {
                lines.Add("Conditions: ");
                for (int i = 0; i < data.Conditions.Count; ++i)
                {
                    lines.Add(Process(data.Conditions[i].Type.ToString()));
                    if (i != data.Conditions.Count - 1)
                        lines.Add(", ");
                }
            }
            lines.Add("</TD>");
            lines.Add("</TR>");

            // Row 3: Initiative and Markings
            lines.Add("<TR>");

            // Initiative
            if (initiative_holder)
            {
                lines.Add($"<TD>Init: {data.Initiative} ({data.InitiativeModifier})</TD>");
            }
            else
            {
                lines.Add("<TD>&nbsp;</TD>");
            }

            // Markings
            if (data.TacticalMapData != null)
            {
                lines.Add($"<TD>Mark: {data.TacticalMapData.Mark}</TD>");
            }
            else
            {
                lines.Add("<TD>&nbsp;</TD>");
            }
            lines.Add("</TR>");
            lines.Add("</TABLE>");

            return lines;
        }

        /// <summary>
        /// Helper method to retrieve the HTML for a hero stat block.
        /// </summary>
        public static string get_hero(Hero hero, Encounter enc, bool full, DisplaySize size)
        {
            return StatBlock(hero, enc, true, true, full, CardMode.StatBlock, size);
        }

        /// <summary>
        /// Helper method to retrieve the HTML for a creature stat block.
        /// </summary>
        public static string get_creature_template(EncounterCard card, CombatData data, Encounter enc, bool full, DisplaySize size)
        {
            return StatBlock(card, data, enc, true, true, full, CardMode.StatBlock, size);
        }
    }
}
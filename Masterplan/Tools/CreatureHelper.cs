#nullable disable

using Masterplan.Data;
using System;
using System.Collections.Generic;

namespace Masterplan.Tools
{
    /*
     * JUNIOR DEVELOPER GUIDE:
     * CreatureHelper is a utility class containing static methods for manipulating Creature data.
     * It handles three main types of tasks:
     * 1. Data Management: Copying fields between creature instances.
     * 2. Statblock Parsing: Extracting structured data (like Regeneration or Range) from raw text fields.
     * 3. Math & Scaling: Adjusting a creature's level and automatically scaling its stats (HP, Defences, Attack Bonuses) 
     *    according to D&D 4th Edition rules.
     * 
     * THINNING PROCESS NOTES (MasterplanXP Migration):
     * 1. Scaling Logic (AdjustCreatureLevel): This contains pure D&D 4e business rules. It should be moved 
     *    to 'MasterplanXP.Core' so it can be reused by both the UI and any CLI tools without legacy dependencies.
     * 2. Parsing Logic (UpdatePowerRange, ConvertAura, ParseSkills): Legacy Masterplan relies heavily on 
     *    string parsing of "Details" fields. In MasterplanXP, we should prefer structured data at the DTO level, 
     *    but these methods should be moved to a 'StatblockParsingService' in 'MasterplanXP.Infrastructure'.
     * 3. CopyFields: This manual mapping is fragile. In the new architecture, we use DTOs and the Bridge project 
     *    to handle object state transitions.
     * 4. Logging: Replaces 'LogSystem.Trace' with 'Microsoft.Extensions.Logging' in the new projects.
     */

    /// <summary>
    /// Utility class for creature-related operations, including deep copying, 
    /// level scaling, and text parsing.
    /// </summary>
    class CreatureHelper
    {
        /// <summary>
        /// Performs a deep copy of all fields from one ICreature to another.
        /// THINNING NOTE: Replace with AutoMapper or DTO mapping in MasterplanXP.
        /// </summary>
        /// <param name="copy_from">The source creature.</param>
        /// <param name="copy_to">The destination creature.</param>
        public static void CopyFields(ICreature copy_from, ICreature copy_to)
        {
            try
            {
                if (copy_from != null)
                {
                    copy_to.ID = copy_from.ID;
                    copy_to.Name = copy_from.Name;
                    copy_to.Details = copy_from.Details;
                    copy_to.Size = copy_from.Size;
                    copy_to.Origin = copy_from.Origin;
                    copy_to.Type = copy_from.Type;
                    copy_to.Keywords = copy_from.Keywords;
                    copy_to.Level = copy_from.Level;
                    copy_to.Role = (copy_from.Role != null) ? copy_from.Role.Copy() : null;
                    copy_to.Senses = copy_from.Senses;
                    copy_to.Movement = copy_from.Movement;
                    copy_to.Alignment = copy_from.Alignment;
                    copy_to.Languages = copy_from.Languages;
                    copy_to.Skills = copy_from.Skills;
                    copy_to.Equipment = copy_from.Equipment;
                    copy_to.Category = copy_from.Category;

                    copy_to.Strength = (copy_from.Strength != null) ? copy_from.Strength.Copy() : new Ability();
                    copy_to.Constitution = (copy_from.Constitution != null) ? copy_from.Constitution.Copy() : new Ability();
                    copy_to.Dexterity = (copy_from.Dexterity != null) ? copy_from.Dexterity.Copy() : new Ability();
                    copy_to.Intelligence = (copy_from.Intelligence != null) ? copy_from.Intelligence.Copy() : new Ability();
                    copy_to.Wisdom = (copy_from.Wisdom != null) ? copy_from.Wisdom.Copy() : new Ability();
                    copy_to.Charisma = (copy_from.Charisma != null) ? copy_from.Charisma.Copy() : new Ability();

                    copy_to.HP = copy_from.HP;
                    copy_to.Initiative = copy_from.Initiative;
                    copy_to.AC = copy_from.AC;
                    copy_to.Fortitude = copy_from.Fortitude;
                    copy_to.Reflex = copy_from.Reflex;
                    copy_to.Will = copy_from.Will;

                    copy_to.Regeneration = (copy_from.Regeneration != null) ? copy_from.Regeneration.Copy() : null;

                    copy_to.Auras.Clear();
                    foreach (Aura aura in copy_from.Auras)
                        copy_to.Auras.Add(aura.Copy());

                    copy_to.CreaturePowers.Clear();
                    foreach (CreaturePower cp in copy_from.CreaturePowers)
                        copy_to.CreaturePowers.Add(cp.Copy());

                    copy_to.DamageModifiers.Clear();
                    foreach (DamageModifier dm in copy_from.DamageModifiers)
                        copy_to.DamageModifiers.Add(dm.Copy());

                    copy_to.Resist = copy_from.Resist;
                    copy_to.Vulnerable = copy_from.Vulnerable;
                    copy_to.Immune = copy_from.Immune;
                    copy_to.Tactics = copy_from.Tactics;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Trace(ex);
            }
        }

        /// <summary>
        /// Searches for "Regeneration" in a creature's auras and promotes it to the structured Regeneration property.
        /// THINNING NOTE: This is parsing logic. Move to a parsing service.
        /// </summary>
        /// <param name="c">The creature to update.</param>
        public static void UpdateRegen(ICreature c)
        {
            Aura regen_aura = FindAura(c, "Regeneration");
            if (regen_aura == null)
                regen_aura = FindAura(c, "Regen");

            if (regen_aura != null)
            {
                Regeneration regen = ConvertAura(regen_aura.Details);
                if (regen != null)
                {
                    c.Regeneration = regen;
                    c.Auras.Remove(regen_aura);
                }
            }
        }

        /// <summary>
        /// Attempts to parse and update a power's Range field based on keywords in its Details text.
        /// THINNING NOTE: Move to a 'StatblockParsingService'.
        /// </summary>
        /// <param name="c">The creature owning the power.</param>
        /// <param name="power">The power to analyze.</param>
        public static void UpdatePowerRange(ICreature c, CreaturePower power)
        {
            // DB Cleanup - change any existing entry in the range field from Self to Personal
            if (power.Range.Contains("Self") || power.Range.Contains("self"))
            {
                if (power.Range.Contains("self"))
                {
                    if (power.Range == "self")
                    { power.Range = "Personal"; }
                    return;
                }
                power.Range = "Personal";
                return;
            }

            // If the range field is not empty - do nothing
            if ((power.Range != null) && (power.Range != ""))
                return;

            // Malformed details will impact parsing
            // in that case send back - do nothing
            if (!power.Details.Contains(";"))
                return;

            // Start the parsing process to derive the
            // range of an attack from text on details field

            // Define the 5 ranges to be parsed.
            List<string> ranges = new List<string>();
            ranges.Add("close blast");
            ranges.Add("close burst");
            ranges.Add("area burst");
            ranges.Add("melee");
            ranges.Add("ranged");

            // setup a hold variable to keep the original power.Details field
            string originalDetails = power.Details;

            // split the details on the ";" delimiter for parsing
            string[] clauses = power.Details.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string clause in clauses)
            {
                foreach (string range in ranges)
                {
                    if (clause.ToLower().Contains(range))
                    {
                        try
                        {
                            int startIndex = clause.ToLower().IndexOf(range);
                            int endIndex = clause.Length;
                            power.Range = clause.Substring(startIndex, (endIndex - startIndex));

                            // if the details call out a basic attack, update the power to make it a basic attack
                            // exclude anything that is not an At Will attack							
                            bool is_basic = power.Range.IndexOf("basic", StringComparison.OrdinalIgnoreCase) >= 0;
                            if (is_basic && (power.Action.Use == PowerUseType.AtWill || power.Action.Use == PowerUseType.Basic))
                            {
                                power.Action.Use = PowerUseType.Basic;
                            }

                            break;
                        }
                        catch { } // Error Handling

                    }
                }
            }
            // Retain the details for verification
            // string parsing can be inaccurate mostly due
            // to non-standard entries in details field
            power.Details = originalDetails;
        }

        /// <summary>
        /// Finds an aura by its case-sensitive name.
        /// </summary>
        /// <param name="c">The creature.</param>
        /// <param name="name">Aura name.</param>
        /// <returns>The Aura if found, otherwise null.</returns>
        public static Aura FindAura(ICreature c, string name)
        {
            foreach (Aura a in c.Auras)
            {
                if (a.Name == name)
                    return a;
            }

            return null;
        }

        /// <summary>
        /// Parses a string like "5 (when bloodied)" into a Regeneration object.
        /// THINNING NOTE: Move to parsing service.
        /// </summary>
        /// <param name="aura_details">The raw detail string.</param>
        /// <returns>A structured Regeneration object.</returns>
        public static Regeneration ConvertAura(string aura_details)
        {
            aura_details = aura_details.Trim();

            bool parsing_value = true;
            string val_str = "";
            string details = "";

            foreach (char ch in aura_details)
            {
                if (!char.IsDigit(ch))
                    parsing_value = false;

                if (parsing_value)
                    val_str += ch;
                else
                    details += ch;
            }

            details = details.Trim();
            if (details.StartsWith("(") && details.EndsWith(")"))
            {
                details = details.Substring(1);
                details = details.Substring(0, details.Length - 1);

                details.Trim();
            }

            try
            {
                int value = (val_str != "") ? int.Parse(val_str) : 0;

                return new Regeneration(value, details);
            }
            catch (Exception ex)
            {
                LogSystem.Trace(ex);

                return null;
            }
        }

        /// <summary>
        /// Filters creature powers by their category (Standard, Minor, etc.).
        /// </summary>
        /// <param name="c">The creature.</param>
        /// <param name="category">The category to filter by.</param>
        /// <returns>A list of matching powers.</returns>
        public static List<CreaturePower> CreaturePowersByCategory(ICreature c, CreaturePowerCategory category)
        {
            List<CreaturePower> powers = new List<CreaturePower>();

            foreach (CreaturePower cp in c.CreaturePowers)
            {
                if (cp.Category == category)
                    powers.Add(cp);
            }

            return powers;
        }

        /// <summary>
        /// Mathematically adjusts a creature's level, scaling HP, Defences, and Powers accordingly.
        /// THINNING NOTE: CORE BUSINESS LOGIC. Move to MasterplanXP.Core.
        /// </summary>
        /// <param name="creature">The creature to scale.</param>
        /// <param name="delta">The number of levels to add or subtract.</param>
        public static void AdjustCreatureLevel(ICreature creature, int delta)
        {
            // HP Scaling Logic
            if (creature.Role is ComplexRole)
            {
                ComplexRole role = creature.Role as ComplexRole;

                int hp = 8;
                switch (role.Type)
                {
                    case RoleType.Artillery:
                    case RoleType.Lurker:
                        hp = 6;
                        break;
                    case RoleType.Brute:
                        hp = 10;
                        break;
                }

                switch (role.Flag)
                {
                    case RoleFlag.Elite:
                        hp *= 2;
                        break;
                    case RoleFlag.Solo:
                        hp *= 5;
                        break;
                }

                creature.HP += hp * delta;
                creature.HP = Math.Max(creature.HP, 1);
            }

            // Initiative Scaling
            int init_bonus = creature.Initiative - (creature.Level / 2);
            creature.Initiative = init_bonus + ((creature.Level + delta) / 2);

            // Defences Scaling (Flat +1 per level)
            creature.AC += delta;
            creature.Fortitude += delta;
            creature.Reflex += delta;
            creature.Will += delta;

            // Powers Scaling
            foreach (CreaturePower cp in creature.CreaturePowers)
                AdjustPowerLevel(cp, delta);

            // Skills Scaling
            if (creature.Skills != "")
            {
                // Parse string
                Dictionary<string, int> skill_list = CreatureHelper.ParseSkills(creature.Skills);

                // Sort
                BinarySearchTree<string> bst = new BinarySearchTree<string>();
                foreach (string skill_name in skill_list.Keys)
                    bst.Add(skill_name);

                string skill_str = "";
                foreach (string skill_name in bst.SortedList)
                {
                    if (skill_str != "")
                        skill_str += ", ";

                    int mod = skill_list[skill_name];

                    // Apply level adjustment
                    int bonus = mod - (creature.Level / 2);
                    mod = bonus + ((creature.Level + delta) / 2);

                    if (mod >= 0)
                        skill_str += skill_name + " +" + mod;
                    else
                        skill_str += skill_name + " " + mod;
                }

                creature.Skills = skill_str;
            }

            // Level Update
            creature.Level += delta;
        }

        /// <summary>
        /// Adjusts a power's attack bonus and damage based on level delta.
        /// </summary>
        /// <param name="cp">The power to adjust.</param>
        /// <param name="delta">The level delta.</param>
        public static void AdjustPowerLevel(CreaturePower cp, int delta)
        {
            if (cp.Attack != null)
                cp.Attack.Bonus += delta;

            // Adjust power damage strings
            string dmg_str = AI.ExtractDamage(cp.Details);
            if (dmg_str != "")
            {
                DiceExpression exp = DiceExpression.Parse(dmg_str);
                if (exp != null)
                {
                    DiceExpression exp_adj = exp.Adjust(delta);
                    if ((exp_adj != null) && (exp.ToString() != exp_adj.ToString()))
                    {
                        cp.Details = cp.Details.Replace(dmg_str, exp_adj + " damage");
                    }
                }
            }
        }

        /// <summary>
        /// Parses a skills string (e.g. "Acrobatics +10, Stealth +12") into a dictionary.
        /// THINNING NOTE: Move to parsing service.
        /// </summary>
        /// <param name="source">The skills string.</param>
        /// <returns>A dictionary of skill names and their bonuses.</returns>
        public static Dictionary<string, int> ParseSkills(string source)
        {
            Dictionary<string, int> skill_list = new Dictionary<string, int>();

            if ((source != null) && (source != ""))
            {
                string[] skills = source.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string skill in skills)
                {
                    string str = skill.Trim();

                    int index = str.IndexOf(" ");
                    if (index != -1)
                    {
                        string skill_name = str.Substring(0, index);
                        string skill_bonus = str.Substring(index + 1);

                        int bonus = 0;
                        try
                        {
                            bonus = int.Parse(skill_bonus);
                        }
                        catch
                        {
                            bonus = 0;
                        }

                        skill_list[skill_name] = bonus;
                    }
                }
            }

            return skill_list;
        }
    }
}

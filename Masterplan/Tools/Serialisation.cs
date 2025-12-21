#nullable disable

using Masterplan.Tools;
using Masterplan.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Masterplan.Tools
{
    public enum SerialisationMode { Binary, XML, MessagePack, XMLDTO }

    public class Serialisation<T>
    {
        private static string SafeStr(string s) => string.IsNullOrWhiteSpace(s) ? "nodata" : s;

        public static T Load(string filename, SerialisationMode mode)
        {
            T result = default(T);
            try
            {
                switch (mode)
                {
                    case SerialisationMode.Binary:
                        using (FileStream stream = new(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            result = (T)new BinaryFormatter().Deserialize(stream);
                        }
                        break;
                    case SerialisationMode.XML:
                    case SerialisationMode.XMLDTO:
                        using (XmlTextReader reader = new(filename))
                        {
                            // XMLDTO uses the same deserialization logic as standard XML but expects the DTO structure
                            result = (T)new XmlSerializer(typeof(T)).Deserialize(reader);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Trace(ex);
            }
            return result;
        }

        public static bool Save(string filename, T obj, SerialisationMode mode)
        {
            // RUN DISCOVERY ON EVERY SAVE TO GENERATE THE REPORT
            if (obj != null)
            {
                DiscoveryService.Analyze(obj);
            }

            string temp_filename = filename + ".tmp";
            bool ok = false;
            try
            {
                if (mode == SerialisationMode.XMLDTO && obj is Masterplan.Data.Library lib)
                {
                    // Map the complex Library object to the clean DTO structure
                    var dto = MapToLibraryDto(lib);
                    using (XmlTextWriter writer = new(temp_filename, Encoding.UTF8) { Formatting = Formatting.Indented })
                    {
                        new XmlSerializer(typeof(LibraryDto)).Serialize(writer, dto);
                    }
                    ok = true;
                }
                else if (mode == SerialisationMode.Binary)
                {
                    using (FileStream stream = new(temp_filename, FileMode.Create))
                    {
                        new BinaryFormatter().Serialize(stream, obj);
                    }
                    ok = true;
                }
                else if (mode == SerialisationMode.XML)
                {
                    using (XmlTextWriter writer = new(temp_filename, Encoding.UTF8) { Formatting = Formatting.Indented })
                    {
                        new XmlSerializer(typeof(T)).Serialize(writer, obj);
                    }
                    ok = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Trace(ex);
                ok = false;
            }

            if (ok)
            {
                if (File.Exists(filename)) File.Delete(filename);
                File.Move(temp_filename, filename);
            }
            return ok;
        }

        private static LibraryDto MapToLibraryDto(Masterplan.Data.Library lib)
        {
            if (lib == null) return null;
            var dto = new LibraryDto { ID = lib.ID, Name = SafeStr(lib.Name), ShowInAutoBuild = lib.ShowInAutoBuild };

            if (lib.Creatures != null)
            {
                foreach (var c in lib.Creatures)
                {
                    if (c == null) continue;
                    dto.Creatures.Add(new CreatureDto
                    {
                        ID = c.ID,
                        Name = SafeStr(c.Name),
                        Details = SafeStr(c.Details),
                        Level = c.Level,
                        HP = c.HP,
                        Size = c.Size.ToString(),
                        Origin = c.Origin.ToString(),
                        Type = c.Type.ToString(),
                        Keywords = SafeStr(c.Keywords),
                        Role = c.Role?.ToString() ?? "nodata", // Handle interface to string conversion
                        Senses = SafeStr(c.Senses),
                        Movement = SafeStr(c.Movement),
                        Alignment = SafeStr(c.Alignment),
                        Languages = SafeStr(c.Languages),
                        Skills = SafeStr(c.Skills),
                        Equipment = SafeStr(c.Equipment),
                        Category = SafeStr(c.Category),
                        Initiative = c.Initiative,
                        AC = c.AC,
                        Fortitude = c.Fortitude,
                        Reflex = c.Reflex,
                        Will = c.Will
                    });
                }
            }

            if (lib.Traps != null)
            {
                foreach (var t in lib.Traps)
                {
                    if (t == null) continue;
                    dto.Traps.Add(new TrapDto
                    {
                        ID = t.ID,
                        Name = SafeStr(t.Name),
                        Level = t.Level,
                        Role = t.Role.ToString()
                    });
                }
            }

            if (lib.SkillChallenges != null)
            {
                foreach (var sc in lib.SkillChallenges)
                {
                    if (sc == null) continue;
                    var scDto = new SkillChallengeDto
                    {
                        ID = sc.ID,
                        Name = SafeStr(sc.Name),
                        Level = sc.Level,
                        SuccessCondition = SafeStr(sc.Success),
                        FailureCondition = SafeStr(sc.Failure)
                    };
                    if (sc.Skills != null)
                    {
                        foreach (var s in sc.Skills)
                            scDto.Skills.Add(new SkillChallengeDataDto { SkillName = SafeStr(s.SkillName), Difficulty = s.Difficulty.ToString(), DCModifier = s.DCModifier, Details = SafeStr(s.Details) });
                    }
                    dto.SkillChallenges.Add(scDto);
                }
            }

            if (lib.Artifacts != null)
            {
                foreach (var art in lib.Artifacts)
                {
                    if (art == null) continue;
                    var aDto = new ArtifactDto
                    {
                        ID = art.ID,
                        Name = SafeStr(art.Name),
                        Tier = art.Tier.ToString(),
                        Description = SafeStr(art.Description)
                    };
                    if (art.ConcordanceLevels != null)
                    {
                        foreach (var lvl in art.ConcordanceLevels)
                            aDto.ConcordanceLevels.Add(new ArtifactConcordanceDto { Name = SafeStr(lvl.Name), ValueRange = SafeStr(lvl.ValueRange), Quote = SafeStr(lvl.Quote), Description = SafeStr(lvl.Description) });
                    }
                    dto.Artifacts.Add(aDto);
                }
            }

            if (lib.MagicItems != null)
            {
                foreach (var m in lib.MagicItems)
                {
                    if (m == null) continue;
                    var mDto = new MagicItemDto { ID = m.ID, Name = SafeStr(m.Name), Level = m.Level, Type = SafeStr(m.Type), Rarity = m.Rarity.ToString(), Description = SafeStr(m.Description) };
                    if (m.Sections != null)
                    {
                        foreach (var sec in m.Sections) mDto.Sections.Add(new SectionDto { Header = SafeStr(sec.Header), Details = SafeStr(sec.Details) });
                    }
                    dto.MagicItems.Add(mDto);
                }
            }

            if (lib.Tiles != null)
                foreach (var t in lib.Tiles) if (t != null) dto.Tiles.Add(new TileDto { ID = t.ID, Category = t.Category.ToString(), Size = t.Size.ToString(), Keywords = SafeStr(t.Keywords) });

            if (lib.TerrainPowers != null)
                foreach (var tp in lib.TerrainPowers) if (tp != null) dto.TerrainPowers.Add(new TerrainPowerDto { Name = SafeStr(tp.Name), Type = tp.Type.ToString(), FlavourText = SafeStr(tp.FlavourText), Requirement = SafeStr(tp.Requirement), Check = SafeStr(tp.Check), Success = SafeStr(tp.Success), Failure = SafeStr(tp.Failure), Target = SafeStr(tp.Target) });

            if (lib.Themes != null)
                foreach (var th in lib.Themes) if (th != null) dto.Themes.Add(new ThemeDto { Name = SafeStr(th.Name) });

            if (lib.Templates != null)
                foreach (var temp in lib.Templates) if (temp != null) dto.Templates.Add(new TemplateDto { Name = SafeStr(temp.Name) });

            return dto;
        }
    }
}
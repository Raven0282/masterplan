using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using MessagePack;
using Masterplan.Data;
using Masterplan.Dto;

namespace Masterplan.Tools
{
    /// <summary>
    /// Service responsible for mapping between legacy domain objects and MessagePack-ready DTOs.
    /// Supports the "Parallel Track" workflow for OMP to MPXP migration.
    /// </summary>
    public class LibraryConversionService
    {
        private static LibraryConversionService _instance;
        public static LibraryConversionService Instance => _instance ??= new LibraryConversionService();

        #region X-Format Cycle (MessagePack)
        public void SaveXLibrary(Library lib, string targetPath)
        {
            try
            {
                var dto = MapToLibraryDto(lib);
                var data = MessagePackSerializer.Serialize(dto);
                File.WriteAllBytes(targetPath, data);
            }
            catch (Exception ex) { LogSystem.Trace(ex); }
        }

        public void SaveXProject(Project p, string targetPath)
        {
            try
            {
                var dto = MapToProjectDto(p);
                var data = MessagePackSerializer.Serialize(dto);
                File.WriteAllBytes(targetPath, data);
            }
            catch (Exception ex) { LogSystem.Trace(ex); }
        }

        public Library LoadXLibrary(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            try
            {
                byte[] data = File.ReadAllBytes(filePath);
                var dto = MessagePackSerializer.Deserialize<LibraryDto>(data);
                return MapToLibrary(dto);
            }
            catch (Exception ex) { LogSystem.Trace(ex); return null; }
        }

        public Project LoadXProject(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            try
            {
                byte[] data = File.ReadAllBytes(filePath);
                var dto = MessagePackSerializer.Deserialize<ProjectDto>(data);
                return MapToProject(dto);
            }
            catch (Exception ex) { LogSystem.Trace(ex); return null; }
        }
        #endregion

        #region Library Mapping
        public Library MapToLibrary(LibraryDto dto)
        {
            if (dto == null) return null;
            var lib = new Library { ID = dto.ID, Name = dto.Name };
            
            if (dto.Creatures != null) foreach (var c in dto.Creatures) lib.Creatures.Add(MapToCreature(c));
            if (dto.Traps != null) foreach (var t in dto.Traps) lib.Traps.Add(MapToTrap(t));
            if (dto.SkillChallenges != null) foreach (var sc in dto.SkillChallenges) lib.SkillChallenges.Add(MapToSkillChallenge(sc));
            if (dto.MagicItems != null) foreach (var mi in dto.MagicItems) lib.MagicItems.Add(MapToMagicItem(mi));
            // Additional collection mappings for Artifacts, Tiles, etc. follow the same pattern
            return lib;
        }

        public LibraryDto MapToLibraryDto(Library lib)
        {
            if (lib == null) return null;
            var dto = new LibraryDto { ID = lib.ID, Name = lib.Name, ShowInAutoBuild = lib.ShowInAutoBuild };
            
            foreach (var c in lib.Creatures) dto.Creatures.Add(MapToCreatureDto(c));
            foreach (var t in lib.Traps) dto.Traps.Add(MapToTrapDto(t));
            foreach (var sc in lib.SkillChallenges) dto.SkillChallenges.Add(MapToSkillChallengeDto(sc));
            foreach (var mi in lib.MagicItems) dto.MagicItems.Add(MapToMagicItemDto(mi));
            return dto;
        }
        #endregion

        #region Project Mapping
        public Project MapToProject(ProjectDto dto)
        {
            if (dto == null) return null;
            var p = new Project
            {
                Name = dto.Name,
                Author = dto.Author,
                Password = dto.Password,
                PasswordHint = dto.PasswordHint
            };

            if (dto.Party != null)
            {
                p.Party.Size = dto.Party.Size;
                p.Party.Level = dto.Party.Level;
                p.Party.XP = dto.Party.XP;
            }

            if (dto.Heroes != null) foreach (var h in dto.Heroes) p.Heroes.Add(MapToHero(h));
            if (dto.Plot != null) p.Plot = MapToPlot(dto.Plot);
            if (dto.Notes != null) foreach (var n in dto.Notes) p.Notes.Add(new Note { ID = n.ID, Content = n.Content, Category = n.Category });
            
            return p;
        }

        public ProjectDto MapToProjectDto(Project p)
        {
            if (p == null) return null;
            var dto = new ProjectDto
            {
                Name = p.Name,
                Author = p.Author,
                Password = p.Password,
                PasswordHint = p.PasswordHint,
                Party = new PartyDto { Size = p.Party.Size, Level = p.Party.Level, XP = p.Party.XP }
            };

            foreach (var h in p.Heroes) dto.Heroes.Add(MapToHeroDto(h));
            dto.Plot = MapToPlotDto(p.Plot);
            foreach (var n in p.Notes) dto.Notes.Add(new NoteDto { ID = n.ID, Name = n.Name, Content = n.Content, Category = n.Category });
            
            return dto;
        }
        #endregion

        #region Creature Mapping
        private Creature MapToCreature(CreatureDto dto)
        {
            if (dto == null) return null;
            var c = new Creature {
                ID = dto.ID, Name = dto.Name, Details = dto.Details,
                Size = Enum.Parse<CreatureSize>(dto.Size), Origin = Enum.Parse<CreatureOrigin>(dto.Origin),
                Type = Enum.Parse<CreatureType>(dto.Type), Keywords = dto.Keywords,
                Level = dto.Level, Role = MapToRole(dto.Role), HP = dto.HP,
                Initiative = dto.Initiative, AC = dto.AC, Fortitude = dto.Fortitude,
                Reflex = dto.Reflex, Will = dto.Will, Senses = dto.Senses,
                Movement = dto.Movement, Alignment = dto.Alignment, Languages = dto.Languages,
                Skills = dto.Skills, Equipment = dto.Equipment, Category = dto.Category,
                Tactics = dto.Tactics, Resist = dto.Resist, Vulnerable = dto.Vulnerable, Immune = dto.Immune,
                Image = ByteArrayToImage(dto.ImageData),
                Regeneration = MapToRegeneration(dto.Regeneration)
            };
            
            if (dto.Strength != null) c.Strength.Score = dto.Strength.Score;
            if (dto.Constitution != null) c.Constitution.Score = dto.Constitution.Score;
            if (dto.Dexterity != null) c.Dexterity.Score = dto.Dexterity.Score;
            if (dto.Intelligence != null) c.Intelligence.Score = dto.Intelligence.Score;
            if (dto.Wisdom != null) c.Wisdom.Score = dto.Wisdom.Score;
            if (dto.Charisma != null) c.Charisma.Score = dto.Charisma.Score;

            foreach (var p in dto.Powers) c.CreaturePowers.Add(MapToPower(p));
            foreach (var a in dto.Auras) c.Auras.Add(new Aura { ID = a.ID, Name = a.Name, Keywords = a.Keywords, Details = a.Details });
            foreach (var dm in dto.DamageModifiers) c.DamageModifiers.Add(new DamageModifier { Type = Enum.Parse<DamageType>(dm.Type), Value = dm.Value });

            return c;
        }

        private CreatureDto MapToCreatureDto(Creature c)
        {
            if (c == null) return null;
            var dto = new CreatureDto {
                ID = c.ID, Name = c.Name, Details = c.Details, Size = c.Size.ToString(),
                Origin = c.Origin.ToString(), Type = c.Type.ToString(), Keywords = c.Keywords,
                Level = c.Level, Role = MapToRoleDto(c.Role), HP = c.HP,
                Initiative = c.Initiative, AC = c.AC, Fortitude = c.Fortitude,
                Reflex = c.Reflex, Will = c.Will, Senses = c.Senses,
                Movement = c.Movement, Alignment = c.Alignment, Languages = c.Languages,
                Skills = c.Skills, Equipment = c.Equipment, Category = c.Category,
                Tactics = c.Tactics, Resist = c.Resist, Vulnerable = c.Vulnerable, Immune = c.Immune,
                ImageData = ImageToByteArray(c.Image), Info = c.Info, Phenotype = c.Phenotype,
                Regeneration = MapToRegenerationDto(c.Regeneration),
                Strength = new AbilityScoreDto { Score = c.Strength.Score },
                Constitution = new AbilityScoreDto { Score = c.Constitution.Score },
                Dexterity = new AbilityScoreDto { Score = c.Dexterity.Score },
                Intelligence = new AbilityScoreDto { Score = c.Intelligence.Score },
                Wisdom = new AbilityScoreDto { Score = c.Wisdom.Score },
                Charisma = new AbilityScoreDto { Score = c.Charisma.Score }
            };

            foreach (var p in c.CreaturePowers) dto.Powers.Add(MapToPowerDto(p));
            foreach (var a in c.Auras) dto.Auras.Add(new AuraDto { ID = a.ID, Name = a.Name, Keywords = a.Keywords, Details = a.Details });
            foreach (var dm in c.DamageModifiers) dto.DamageModifiers.Add(new DamageModifierDto { Type = dm.Type.ToString(), Value = dm.Value });

            return dto;
        }
        #endregion

        #region Component Mapping
        private IRole MapToRole(RoleDto dto)
        {
            if (dto == null) return new ComplexRole();
            if (dto.IsMinion) return new Minion { HasRole = dto.MinionHasRole, Type = Enum.Parse<RoleType>(dto.Type) };
            return new ComplexRole { Type = Enum.Parse<RoleType>(dto.Type), Flag = Enum.Parse<RoleFlag>(dto.Flag), Leader = dto.Leader };
        }

        private RoleDto MapToRoleDto(IRole role)
        {
            if (role == null) return null;
            var dto = new RoleDto();
            if (role is Minion m) { dto.IsMinion = true; dto.MinionHasRole = m.HasRole; dto.Type = m.Type.ToString(); }
            else if (role is ComplexRole cr) { dto.IsMinion = false; dto.Type = cr.Type.ToString(); dto.Flag = cr.Flag.ToString(); dto.Leader = cr.Leader; }
            return dto;
        }

        private CreaturePower MapToPower(CreaturePowerDto dto)
        {
            var p = new CreaturePower { ID = dto.ID, Name = dto.Name, Keywords = dto.Keywords, Condition = dto.Condition, Range = dto.Range, Description = dto.Description, Details = dto.Details };
            if (dto.Action != null) p.Action = new PowerAction { Action = Enum.Parse<ActionType>(dto.Action.Action), Trigger = dto.Action.Trigger, SustainAction = Enum.Parse<ActionType>(dto.Action.SustainAction), Use = Enum.Parse<PowerUseType>(dto.Action.Use), Recharge = dto.Action.Recharge };
            if (dto.Attack != null) p.Attack = new PowerAttack { Bonus = dto.Attack.Bonus, Defence = Enum.Parse<DefenceType>(dto.Attack.Defence) };
            return p;
        }

        private CreaturePowerDto MapToPowerDto(CreaturePower p)
        {
            var dto = new CreaturePowerDto { ID = p.ID, Name = p.Name, Keywords = p.Keywords, Condition = p.Condition, Range = p.Range, Description = p.Description, Details = p.Details, Damage = p.Damage, Category = p.Category.ToString() };
            if (p.Action != null) dto.Action = new PowerActionDto { Action = p.Action.Action.ToString(), Trigger = p.Action.Trigger, SustainAction = p.Action.SustainAction.ToString(), Use = p.Action.Use.ToString(), Recharge = p.Action.Recharge };
            if (p.Attack != null) dto.Attack = new PowerAttackDto { Bonus = p.Attack.Bonus, Defence = p.Attack.Defence.ToString() };
            return dto;
        }

        private Hero MapToHero(HeroDto dto)
        {
            return new Hero {
                ID = dto.ID, Name = dto.Name, Player = dto.Player, Race = dto.Race,
                Class = dto.Class, Level = dto.Level, HP = dto.HP, AC = dto.AC,
                Fortitude = dto.Fortitude, Reflex = dto.Reflex, Will = dto.Will,
                Portrait = ByteArrayToImage(dto.PortraitData)
            };
        }

        private HeroDto MapToHeroDto(Hero h)
        {
            return new HeroDto {
                ID = h.ID, Name = h.Name, Player = h.Player, Race = h.Race,
                Class = h.Class, Level = h.Level, HP = h.HP, AC = h.AC,
                Fortitude = h.Fortitude, Reflex = h.Reflex, Will = h.Will,
                PortraitData = ImageToByteArray(h.Portrait)
            };
        }

        private Plot MapToPlot(PlotDto dto)
        {
            var plot = new Plot();
            if (dto.Points != null) foreach (var p in dto.Points) plot.Points.Add(MapToPlotPoint(p));
            return plot;
        }

        private PlotDto MapToPlotDto(Plot plot)
        {
            var dto = new PlotDto();
            foreach (var p in plot.Points) dto.Points.Add(MapToPlotPointDto(p));
            return dto;
        }

        private PlotPoint MapToPlotPoint(PlotPointDto dto)
        {
            var pp = new PlotPoint(dto.Name) { ID = dto.ID, Details = dto.Details, ReadAloud = dto.ReadAloud };
            pp.State = Enum.Parse<PlotPointState>(dto.State);
            pp.Colour = Enum.Parse<PlotPointColour>(dto.Colour);
            if (dto.Subplot != null) pp.Subplot = MapToPlot(dto.Subplot);
            return pp;
        }

        private PlotPointDto MapToPlotPointDto(PlotPoint pp)
        {
            return new PlotPointDto {
                ID = pp.ID, Name = pp.Name, Details = pp.Details, ReadAloud = pp.ReadAloud,
                State = pp.State.ToString(), Colour = pp.Colour.ToString(),
                Subplot = MapToPlotDto(pp.Subplot)
            };
        }
        #endregion

        #region Trap & Skill Mapping
        private Trap MapToTrap(TrapDto dto)
        {
            var t = new Trap { ID = dto.ID, Name = dto.Name, Type = Enum.Parse<TrapType>(dto.Type), Level = dto.Level, Role = MapToRole(dto.Role), ReadAloud = dto.ReadAloud, Description = dto.Description, Details = dto.Details, Initiative = dto.Initiative, Trigger = dto.Trigger };
            foreach (var s in dto.Skills) t.Skills.Add(new TrapSkillData { ID = s.ID, SkillName = s.SkillName, DC = s.DC, Details = s.Details });
            foreach (var a in dto.Attacks) t.Attacks.Add(MapToTrapAttack(a));
            if (dto.Countermeasures != null) t.Countermeasures.AddRange(dto.Countermeasures.Split('|', StringSplitOptions.RemoveEmptyEntries));
            return t;
        }

        private TrapDto MapToTrapDto(Trap t)
        {
            var dto = new TrapDto { ID = t.ID, Name = t.Name, Type = t.Type.ToString(), Level = t.Level, Role = MapToRoleDto(t.Role), ReadAloud = t.ReadAloud, Description = t.Description, Details = t.Details, Initiative = t.Initiative, Trigger = t.Trigger, Info = t.Info, XP = t.XP };
            foreach (var s in t.Skills) dto.Skills.Add(new TrapSkillDto { ID = s.ID, SkillName = s.SkillName, DC = s.DC, Details = s.Details });
            foreach (var a in t.Attacks) dto.Attacks.Add(MapToTrapAttackDto(a));
            dto.Countermeasures = string.Join("|", t.Countermeasures);
            return dto;
        }

        private TrapAttack MapToTrapAttack(TrapAttackDto dto)
        {
            var a = new TrapAttack { ID = dto.ID, Name = dto.Name, Trigger = dto.Trigger, Action = Enum.Parse<ActionType>(dto.Action), Range = dto.Range, Keywords = dto.Keywords, Target = dto.Target, HasInitiative = dto.HasInitiative, Initiative = dto.Initiative, OnHit = dto.OnHit, OnMiss = dto.OnMiss, Effect = dto.Effect, Notes = dto.Notes };
            if (dto.Attack != null) a.Attack = new PowerAttack { Bonus = dto.Attack.Bonus, Defence = Enum.Parse<DefenceType>(dto.Attack.Defence) };
            return a;
        }

        private TrapAttackDto MapToTrapAttackDto(TrapAttack a)
        {
            var dto = new TrapAttackDto { ID = a.ID, Name = a.Name, Trigger = a.Trigger, Action = a.Action.ToString(), Range = a.Range, Keywords = a.Keywords, Target = a.Target, HasInitiative = a.HasInitiative, Initiative = a.Initiative, OnHit = a.OnHit, OnMiss = a.OnMiss, Effect = a.Effect, Notes = a.Notes };
            if (a.Attack != null) dto.Attack = new PowerAttackDto { Bonus = a.Attack.Bonus, Defence = a.Attack.Defence.ToString() };
            return dto;
        }

        private SkillChallenge MapToSkillChallenge(SkillChallengeDto dto)
        {
            var sc = new SkillChallenge { ID = dto.ID, Name = dto.Name, Level = dto.Level, Complexity = dto.Complexity, Success = dto.Success, Failure = dto.Failure, Notes = dto.Notes, MapID = dto.MapID, MapAreaID = dto.MapAreaID };
            foreach (var s in dto.Skills) sc.Skills.Add(new SkillChallengeData { SkillName = s.SkillName, Difficulty = Enum.Parse<Difficulty>(s.Difficulty), DCModifier = s.DCModifier, Details = s.Details, Success = s.Success, Failure = s.Failure });
            return sc;
        }

        private SkillChallengeDto MapToSkillChallengeDto(SkillChallenge sc)
        {
            var dto = new SkillChallengeDto { ID = sc.ID, Name = sc.Name, Level = sc.Level, Complexity = sc.Complexity, Success = sc.Success, Failure = sc.Failure, Notes = sc.Notes, MapID = sc.MapID, MapAreaID = sc.MapAreaID, Successes = sc.Successes, Info = sc.Info };
            foreach (var s in sc.Skills) dto.Skills.Add(new SkillChallengeDataDto { SkillName = s.SkillName, Difficulty = s.Difficulty.ToString(), DCModifier = s.DCModifier, Details = s.Details, Success = s.Success, Failure = s.Failure });
            return dto;
        }
        #endregion

        #region Helpers
        private byte[] ImageToByteArray(Image img)
        {
            if (img == null) return null;
            try { using (var ms = new MemoryStream()) { img.Save(ms, ImageFormat.Png); return ms.ToArray(); } }
            catch { return null; }
        }

        private Image ByteArrayToImage(byte[] data)
        {
            if (data == null || data.Length == 0) return null;
            try { using (var ms = new MemoryStream(data)) return new Bitmap(ms); }
            catch { return null; }
        }

        private Regeneration MapToRegeneration(RegenerationDto dto) => dto != null ? new Regeneration(dto.Value, dto.Details) : null;
        private RegenerationDto MapToRegenerationDto(Regeneration r) => r != null ? new RegenerationDto { Value = r.Value, Details = r.Details } : null;

        private MagicItem MapToMagicItem(MagicItemDto dto)
        {
            var mi = new MagicItem { ID = dto.ID, Name = dto.Name, Type = dto.Type, Rarity = Enum.Parse<MagicItemRarity>(dto.Rarity), Level = dto.Level, Description = dto.Description };
            foreach (var s in dto.Sections) mi.Sections.Add(new MagicItemSection { Header = s.Header, Details = s.Details });
            return mi;
        }

        private MagicItemDto MapToMagicItemDto(MagicItem mi)
        {
            var dto = new MagicItemDto { ID = mi.ID, Name = mi.Name, Type = mi.Type, Rarity = mi.Rarity.ToString(), Level = mi.Level, Description = mi.Description, Info = mi.Info };
            foreach (var s in mi.Sections) dto.Sections.Add(new SectionDto { Header = s.Header, Details = s.Details });
            return dto;
        }
        #endregion
    }
}

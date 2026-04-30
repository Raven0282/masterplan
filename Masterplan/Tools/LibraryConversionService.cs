using Masterplan.Data;
using Masterplan.Dto;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

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
        public bool SaveXLibrary(Library lib, string targetPath)
        {
            try
            {
                var dto = MapToLibraryDto(lib);
                var data = MessagePackSerializer.Serialize(dto);
                File.WriteAllBytes(targetPath, data);
                return true;
            }
            catch (Exception ex) { LogSystem.Trace(ex); return false; }
        }

        public bool SaveXProject(Project p, string targetPath)
        {
            try
            {
                var dto = MapToProjectDto(p);
                var data = MessagePackSerializer.Serialize(dto);
                File.WriteAllBytes(targetPath, data);
                return true;
            }
            catch (Exception ex) { LogSystem.Trace(ex); return false; }
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
            if (dto.InactiveHeroes != null) foreach (var h in dto.InactiveHeroes) p.InactiveHeroes.Add(MapToHero(h));
            if (dto.Plot != null) p.Plot = MapToPlot(dto.Plot);
            if (dto.Encyclopedia != null) p.Encyclopedia = MapToEncyclopedia(dto.Encyclopedia);
            if (dto.Notes != null) foreach (var n in dto.Notes) p.Notes.Add(new Note { ID = n.ID, Content = n.Content, Category = n.Category });
            if (dto.Maps != null) foreach (var m in dto.Maps) p.Maps.Add(MapToMap(m));
            if (dto.RegionalMaps != null) foreach (var rm in dto.RegionalMaps) p.RegionalMaps.Add(MapToRegionalMap(rm));
            if (dto.Decks != null) foreach (var d in dto.Decks) p.Decks.Add(MapToDeck(d));
            if (dto.NPCs != null) foreach (var npc in dto.NPCs) p.NPCs.Add(MapToNPC(npc));
            if (dto.CustomCreatures != null) foreach (var cc in dto.CustomCreatures) p.CustomCreatures.Add(MapToCustomCreature(cc));
            if (dto.Calendars != null) foreach (var c in dto.Calendars) p.Calendars.Add(MapToCalendar(c));
            if (dto.Attachments != null) foreach (var a in dto.Attachments) p.Attachments.Add(MapToAttachment(a));
            if (dto.Backgrounds != null) foreach (var b in dto.Backgrounds) p.Backgrounds.Add(MapToBackground(b));
            if (dto.TreasureParcels != null) foreach (var tp in dto.TreasureParcels) p.TreasureParcels.Add(MapToParcel(tp));
            if (dto.CampaignSettings != null) p.CampaignSettings = MapToCampaignSettings(dto.CampaignSettings);
            if (dto.Library != null) p.Library = MapToLibrary(dto.Library);
            if (dto.SavedCombats != null) foreach (var sc in dto.SavedCombats) p.SavedCombats.Add(MapToCombatState(sc, p));
            if (dto.AddInData != null) foreach (var kv in dto.AddInData) p.AddInData[kv.Key] = kv.Value;

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
                Party = new PartyDto { Size = p.Party.Size, Level = p.Party.Level, XP = p.Party.XP },
                Library = MapToLibraryDto(p.Library)
            };

            foreach (var h in p.Heroes) dto.Heroes.Add(MapToHeroDto(h));
            foreach (var h in p.InactiveHeroes) dto.InactiveHeroes.Add(MapToHeroDto(h));
            dto.Plot = MapToPlotDto(p.Plot);
            dto.Encyclopedia = MapToEncyclopediaDto(p.Encyclopedia);
            foreach (var n in p.Notes) dto.Notes.Add(new NoteDto { ID = n.ID, Name = n.Name, Content = n.Content, Category = n.Category });
            foreach (var m in p.Maps) dto.Maps.Add(MapToMapDto(m));
            foreach (var rm in p.RegionalMaps) dto.RegionalMaps.Add(MapToRegionalMapDto(rm));
            foreach (var d in p.Decks) dto.Decks.Add(MapToDeckDto(d));
            foreach (var npc in p.NPCs) dto.NPCs.Add(MapToNPCDto(npc));
            foreach (var cc in p.CustomCreatures) dto.CustomCreatures.Add(MapToCreatureDto(cc));
            foreach (var c in p.Calendars) dto.Calendars.Add(MapToCalendarDto(c));
            foreach (var a in p.Attachments) dto.Attachments.Add(MapToAttachmentDto(a));
            foreach (var b in p.Backgrounds) dto.Backgrounds.Add(MapToBackgroundDto(b));
            foreach (var tp in p.TreasureParcels) dto.TreasureParcels.Add(MapToParcelDto(tp));
            dto.CampaignSettings = MapToCampaignSettingsDto(p.CampaignSettings);
            foreach (var sc in p.SavedCombats) dto.SavedCombats.Add(MapToCombatStateDto(sc));
            if (p.AddInData != null) foreach (var kv in p.AddInData) dto.AddInData[kv.Key] = kv.Value;

            return dto;
        }
        #endregion

        #region Combat State Mapping
        private CombatState MapToCombatState(CombatStateDto dto, Project context)
        {
            if (dto == null) return null;
            var cs = new CombatState
            {
                Timestamp = dto.Timestamp,
                PartyLevel = dto.PartyLevel,
                Encounter = MapToEncounter(dto.Encounter),
                CurrentRound = dto.CurrentRound,
                RemovedCreatureXP = dto.RemovedCreatureXP,
                CurrentActor = dto.CurrentActor,
                Viewpoint = new Rectangle(dto.ViewpointX, dto.ViewpointY, dto.ViewpointWidth, dto.ViewpointHeight),
                Log = MapToEncounterLog(dto.Log)
            };

            if (dto.HeroData != null)
                foreach (var kv in dto.HeroData) cs.HeroData[kv.Key] = MapToCombatData(kv.Value);

            if (dto.TrapData != null)
                foreach (var kv in dto.TrapData) cs.TrapData[kv.Key] = MapToCombatData(kv.Value);

            if (dto.TokenLinks != null)
                foreach (var tl in dto.TokenLinks) cs.TokenLinks.Add(MapToTokenLink(tl, context));

            if (dto.Sketches != null)
                foreach (var s in dto.Sketches) cs.Sketches.Add(MapToMapSketch(s));

            if (dto.QuickEffects != null)
                foreach (var qe in dto.QuickEffects) cs.QuickEffects.Add(MapToOngoingCondition(qe));

            return cs;
        }

        private CombatStateDto MapToCombatStateDto(CombatState cs)
        {
            if (cs == null) return null;
            var dto = new CombatStateDto
            {
                Timestamp = cs.Timestamp,
                PartyLevel = cs.PartyLevel,
                Encounter = MapToEncounterDto(cs.Encounter),
                CurrentRound = cs.CurrentRound,
                RemovedCreatureXP = cs.RemovedCreatureXP,
                CurrentActor = cs.CurrentActor,
                ViewpointX = cs.Viewpoint.X,
                ViewpointY = cs.Viewpoint.Y,
                ViewpointWidth = cs.Viewpoint.Width,
                ViewpointHeight = cs.Viewpoint.Height,
                Log = MapToEncounterLogDto(cs.Log)
            };

            if (cs.HeroData != null)
                foreach (var kv in cs.HeroData) dto.HeroData[kv.Key] = MapToCombatDataDto(kv.Value);

            if (cs.TrapData != null)
                foreach (var kv in cs.TrapData) dto.TrapData[kv.Key] = MapToCombatDataDto(kv.Value);

            if (cs.TokenLinks != null)
                foreach (var tl in cs.TokenLinks) dto.TokenLinks.Add(MapToTokenLinkDto(tl));

            if (cs.Sketches != null)
                foreach (var s in cs.Sketches) dto.Sketches.Add(MapToMapSketchDto(s));

            if (cs.QuickEffects != null)
                foreach (var qe in cs.QuickEffects) dto.QuickEffects.Add(MapToOngoingConditionDto(qe));

            return dto;
        }

        private TokenLink MapToTokenLink(TokenLinkDto dto, Project context)
        {
            var tl = new TokenLink { Text = dto.Text };
            if (dto.Tokens != null)
                foreach (var t in dto.Tokens) tl.Tokens.Add(MapToToken(t, context));
            return tl;
        }

        private TokenLinkDto MapToTokenLinkDto(TokenLink tl)
        {
            var dto = new TokenLinkDto { Text = tl.Text };
            foreach (var t in tl.Tokens) dto.Tokens.Add(MapToTokenDto(t));
            return dto;
        }

        private IToken MapToToken(TokenDto dto, Project context)
        {
            if (dto.Type == "Creature")
                return new CreatureToken(dto.SlotID, MapToCombatData(dto.Data));
            if (dto.Type == "Custom")
                return MapToCustomToken(dto.CustomToken);
            if (dto.Type == "Hero")
                return context?.FindHero(dto.HeroID);
            return null;
        }

        private TokenDto MapToTokenDto(IToken token)
        {
            if (token is CreatureToken ct)
                return new TokenDto { Type = "Creature", SlotID = ct.SlotID, Data = MapToCombatDataDto(ct.Data) };
            if (token is CustomToken cust)
                return new TokenDto { Type = "Custom", CustomToken = MapToCustomTokenDto(cust) };
            if (token is Hero h)
                return new TokenDto { Type = "Hero", HeroID = h.ID };
            return null;
        }

        private CustomToken MapToCustomToken(CustomTokenDto dto)
        {
            if (dto == null) return null;
            var ct = new CustomToken
            {
                ID = dto.ID,
                Type = Enum.Parse<CustomTokenType>(dto.Type),
                Name = dto.Name,
                Details = dto.Details,
                TokenSize = Enum.Parse<CreatureSize>(dto.TokenSize),
                OverlaySize = new Size(dto.OverlaySizeWidth, dto.OverlaySizeHeight),
                OverlayStyle = Enum.Parse<OverlayStyle>(dto.OverlayStyle),
                Colour = Color.FromArgb(dto.ARGB),
                Image = ByteArrayToImage(dto.ImageData),
                DifficultTerrain = dto.DifficultTerrain,
                Opaque = dto.Opaque,
                Data = MapToCombatData(dto.Data),
                TerrainPower = MapToTerrainPower(dto.TerrainPower),
                CreatureID = dto.CreatureID
            };
            return ct;
        }

        private CustomTokenDto MapToCustomTokenDto(CustomToken ct)
        {
            if (ct == null) return null;
            var dto = new CustomTokenDto
            {
                ID = ct.ID,
                Type = ct.Type.ToString(),
                Name = ct.Name,
                Details = ct.Details,
                TokenSize = ct.TokenSize.ToString(),
                OverlaySizeWidth = ct.OverlaySize.Width,
                OverlaySizeHeight = ct.OverlaySize.Height,
                OverlayStyle = ct.OverlayStyle.ToString(),
                ARGB = ct.Colour.ToArgb(),
                ImageData = ImageToByteArray(ct.Image),
                DifficultTerrain = ct.DifficultTerrain,
                Opaque = ct.Opaque,
                Data = MapToCombatDataDto(ct.Data),
                TerrainPower = MapToTerrainPowerDto(ct.TerrainPower),
                CreatureID = ct.CreatureID
            };
            return dto;
        }

        private MapSketch MapToMapSketch(MapSketchDto dto)
        {
            if (dto == null) return null;
            var s = new MapSketch { Colour = Color.FromArgb(dto.ARGB), Width = dto.Width };
            if (dto.Points != null) foreach (var p in dto.Points) s.Points.Add(new MapSketchPoint { Square = new Point(p.SquareX, p.SquareY), Location = new PointF(p.LocationX, p.LocationY) });
            return s;
        }

        private MapSketchDto MapToMapSketchDto(MapSketch s)
        {
            if (s == null) return null;
            var dto = new MapSketchDto { ARGB = s.Colour.ToArgb(), Width = s.Width };
            foreach (var p in s.Points) dto.Points.Add(new MapSketchPointDto { SquareX = p.Square.X, SquareY = p.Square.Y, LocationX = p.Location.X, LocationY = p.Location.Y });
            return dto;
        }
        #endregion

        #region Hero Mapping
        private Hero MapToHero(HeroDto dto)
        {
            if (dto == null) return null;
            var h = new Hero
            {
                ID = dto.ID,
                Name = dto.Name,
                Player = dto.Player,
                Size = Enum.Parse<CreatureSize>(dto.Size),
                Race = dto.Race,
                Class = dto.Class,
                Level = dto.Level,
                ParagonPath = dto.ParagonPath,
                EpicDestiny = dto.EpicDestiny,
                PowerSource = dto.PowerSource,
                Role = Enum.Parse<HeroRoleType>(dto.Role),
                HP = dto.HP,
                AC = dto.AC,
                Fortitude = dto.Fortitude,
                Reflex = dto.Reflex,
                Will = dto.Will,
                InitBonus = dto.InitBonus,
                PassivePerception = dto.PassivePerception,
                PassiveInsight = dto.PassiveInsight,
                Languages = dto.Languages,
                Portrait = ByteArrayToImage(dto.PortraitData)
            };
            if (dto.Tokens != null) foreach (var t in dto.Tokens) h.Tokens.Add(MapToCustomToken(t));
            return h;
        }

        private HeroDto MapToHeroDto(Hero h)
        {
            if (h == null) return null;
            var dto = new HeroDto
            {
                ID = h.ID,
                Name = h.Name,
                Player = h.Player,
                Size = h.Size.ToString(),
                Race = h.Race,
                Class = h.Class,
                Level = h.Level,
                ParagonPath = h.ParagonPath,
                EpicDestiny = h.EpicDestiny,
                PowerSource = h.PowerSource,
                Role = h.Role.ToString(),
                HP = h.HP,
                AC = h.AC,
                Fortitude = h.Fortitude,
                Reflex = h.Reflex,
                Will = h.Will,
                InitBonus = h.InitBonus,
                PassivePerception = h.PassivePerception,
                PassiveInsight = h.PassiveInsight,
                Languages = h.Languages,
                PortraitData = ImageToByteArray(h.Portrait),
                Info = h.Info
            };
            foreach (var t in h.Tokens) dto.Tokens.Add(MapToCustomTokenDto(t));
            return dto;
        }
        #endregion

        #region Library Mapping
        public Library MapToLibrary(LibraryDto dto)
        {
            if (dto == null) return null;
            var lib = new Library { ID = dto.ID, Name = dto.Name };

            if (dto.Creatures != null)
            {
                foreach (var c in dto.Creatures)
                {
                    var creature = MapToCreature(c) as Creature;
                    if (creature != null) lib.Creatures.Add(creature);
                }
            }
            if (dto.Traps != null) foreach (var t in dto.Traps) lib.Traps.Add(MapToTrap(t));
            if (dto.SkillChallenges != null) foreach (var sc in dto.SkillChallenges) lib.SkillChallenges.Add(MapToSkillChallenge(sc));
            if (dto.MagicItems != null) foreach (var mi in dto.MagicItems) lib.MagicItems.Add(MapToMagicItem(mi));
            if (dto.Artifacts != null) foreach (var a in dto.Artifacts) lib.Artifacts.Add(MapToArtifact(a));
            if (dto.Templates != null) foreach (var t in dto.Templates) lib.Templates.Add(MapToTemplate(t));
            if (dto.Themes != null) foreach (var th in dto.Themes) lib.Themes.Add(MapToTheme(th));
            if (dto.Tiles != null) foreach (var t in dto.Tiles) lib.Tiles.Add(MapToTile(t));
            if (dto.TerrainPowers != null) foreach (var tp in dto.TerrainPowers) lib.TerrainPowers.Add(MapToTerrainPower(tp));

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
            foreach (var a in lib.Artifacts) dto.Artifacts.Add(MapToArtifactDto(a));
            foreach (var t in lib.Templates) dto.Templates.Add(MapToTemplateDto(t));
            foreach (var th in lib.Themes) dto.Themes.Add(MapToThemeDto(th));
            foreach (var t in lib.Tiles) dto.Tiles.Add(MapToTileDto(t));
            foreach (var tp in lib.TerrainPowers) dto.TerrainPowers.Add(MapToTerrainPowerDto(tp));

            return dto;
        }
        #endregion

        #region Creature Mapping
        private ICreature MapToCreature(CreatureDto dto)
        {
            if (dto == null) return null;
            var c = new Creature
            {
                ID = dto.ID,
                Name = dto.Name,
                Details = dto.Details,
                Size = Enum.Parse<CreatureSize>(dto.Size),
                Origin = Enum.Parse<CreatureOrigin>(dto.Origin),
                Type = Enum.Parse<CreatureType>(dto.Type),
                Keywords = dto.Keywords,
                Level = dto.Level,
                Role = MapToRole(dto.Role),
                Senses = dto.Senses,
                Movement = dto.Movement,
                Alignment = dto.Alignment,
                Languages = dto.Languages,
                Skills = dto.Skills,
                Equipment = dto.Equipment,
                Category = dto.Category,
                Strength = new Ability { Score = dto.Strength != null ? dto.Strength.Score : 10 },
                Constitution = new Ability { Score = dto.Constitution != null ? dto.Constitution.Score : 10 },
                Dexterity = new Ability { Score = dto.Dexterity != null ? dto.Dexterity.Score : 10 },
                Intelligence = new Ability { Score = dto.Intelligence != null ? dto.Intelligence.Score : 10 },
                Wisdom = new Ability { Score = dto.Wisdom != null ? dto.Wisdom.Score : 10 },
                Charisma = new Ability { Score = dto.Charisma != null ? dto.Charisma.Score : 10 },
                HP = dto.HP,
                Initiative = dto.Initiative,
                AC = dto.AC,
                Fortitude = dto.Fortitude,
                Reflex = dto.Reflex,
                Will = dto.Will,
                Regeneration = MapToRegeneration(dto.Regeneration),
                Resist = dto.Resist,
                Vulnerable = dto.Vulnerable,
                Immune = dto.Immune,
                Tactics = dto.Tactics,
                Image = ByteArrayToImage(dto.ImageData)
            };
            if (dto.Auras != null) foreach (var a in dto.Auras) c.Auras.Add(MapToAura(a));
            if (dto.Powers != null) foreach (var p in dto.Powers) c.CreaturePowers.Add(MapToPower(p));
            if (dto.DamageModifiers != null) foreach (var dm in dto.DamageModifiers) c.DamageModifiers.Add(MapToDamageModifier(dm));
            return c;
        }

        private CustomCreature MapToCustomCreature(CreatureDto dto)
        {
            if (dto == null) return null;
            var c = new CustomCreature
            {
                ID = dto.ID,
                Name = dto.Name,
                Details = dto.Details,
                Size = Enum.Parse<CreatureSize>(dto.Size),
                Origin = Enum.Parse<CreatureOrigin>(dto.Origin),
                Type = Enum.Parse<CreatureType>(dto.Type),
                Keywords = dto.Keywords,
                Level = dto.Level,
                Role = MapToRole(dto.Role),
                Senses = dto.Senses,
                Movement = dto.Movement,
                Alignment = dto.Alignment,
                Languages = dto.Languages,
                Skills = dto.Skills,
                Equipment = dto.Equipment,
                Strength = new Ability { Score = dto.Strength != null ? dto.Strength.Score : 10 },
                Constitution = new Ability { Score = dto.Constitution != null ? dto.Constitution.Score : 10 },
                Dexterity = new Ability { Score = dto.Dexterity != null ? dto.Dexterity.Score : 10 },
                Intelligence = new Ability { Score = dto.Intelligence != null ? dto.Intelligence.Score : 10 },
                Wisdom = new Ability { Score = dto.Wisdom != null ? dto.Wisdom.Score : 10 },
                Charisma = new Ability { Score = dto.Charisma != null ? dto.Charisma.Score : 10 },
                Regeneration = MapToRegeneration(dto.Regeneration),
                Resist = dto.Resist,
                Vulnerable = dto.Vulnerable,
                Immune = dto.Immune,
                Tactics = dto.Tactics,
                Image = ByteArrayToImage(dto.ImageData)
            };
            // Set calculated properties through modifiers where applicable
            c.HP = dto.HP;
            c.Initiative = dto.Initiative;
            c.AC = dto.AC;
            c.Fortitude = dto.Fortitude;
            c.Reflex = dto.Reflex;
            c.Will = dto.Will;

            if (dto.Auras != null) foreach (var a in dto.Auras) c.Auras.Add(MapToAura(a));
            if (dto.Powers != null) foreach (var p in dto.Powers) c.CreaturePowers.Add(MapToPower(p));
            if (dto.DamageModifiers != null) foreach (var dm in dto.DamageModifiers) c.DamageModifiers.Add(MapToDamageModifier(dm));
            return c;
        }

        private CreatureDto MapToCreatureDto(ICreature c)
        {
            if (c == null) return null;
            var dto = new CreatureDto
            {
                ID = c.ID,
                Name = c.Name,
                Details = c.Details,
                Size = c.Size.ToString(),
                Origin = c.Origin.ToString(),
                Type = c.Type.ToString(),
                Keywords = c.Keywords,
                Level = c.Level,
                Role = MapToRoleDto(c.Role),
                Senses = c.Senses,
                Movement = c.Movement,
                Alignment = c.Alignment,
                Languages = c.Languages,
                Skills = c.Skills,
                Equipment = c.Equipment,
                Category = c.Category,
                Strength = new AbilityScoreDto { Score = c.Strength.Score },
                Constitution = new AbilityScoreDto { Score = c.Constitution.Score },
                Dexterity = new AbilityScoreDto { Score = c.Dexterity.Score },
                Intelligence = new AbilityScoreDto { Score = c.Intelligence.Score },
                Wisdom = new AbilityScoreDto { Score = c.Wisdom.Score },
                Charisma = new AbilityScoreDto { Score = c.Charisma.Score },
                HP = c.HP,
                Initiative = c.Initiative,
                AC = c.AC,
                Fortitude = c.Fortitude,
                Reflex = c.Reflex,
                Will = c.Will,
                Regeneration = MapToRegenerationDto(c.Regeneration),
                Resist = c.Resist,
                Vulnerable = c.Vulnerable,
                Immune = c.Immune,
                Tactics = c.Tactics,
                ImageData = ImageToByteArray(c.Image),
                Info = c.Info,
                Phenotype = c.Phenotype
            };
            foreach (var a in c.Auras) dto.Auras.Add(MapToAuraDto(a));
            foreach (var p in c.CreaturePowers) dto.Powers.Add(MapToPowerDto(p));
            foreach (var dm in c.DamageModifiers) dto.DamageModifiers.Add(MapToDamageModifierDto(dm));
            return dto;
        }
        #endregion

        #region Artifact Mapping
        private Artifact MapToArtifact(ArtifactDto dto)
        {
            if (dto == null) return null;
            var a = new Artifact { ID = dto.ID, Name = dto.Name, Tier = Enum.Parse<Tier>(dto.Tier), Description = dto.Description, Details = dto.Details, Goals = dto.Goals, RoleplayingTips = dto.RoleplayingTips };
            if (dto.Sections != null) foreach (var s in dto.Sections) a.Sections.Add(new MagicItemSection { Header = s.Header, Details = s.Details });
            if (dto.ConcordanceLevels != null) foreach (var cl in dto.ConcordanceLevels) a.ConcordanceLevels.Add(MapToArtifactConcordance(cl));
            if (dto.ConcordanceRules != null) foreach (var rule in dto.ConcordanceRules) a.ConcordanceRules.Add(new Pair<string, string>(rule.First, rule.Second));
            return a;
        }

        private ArtifactDto MapToArtifactDto(Artifact a)
        {
            if (a == null) return null;
            var dto = new ArtifactDto { ID = a.ID, Name = a.Name, Tier = a.Tier.ToString(), Description = a.Description, Details = a.Details, Goals = a.Goals, RoleplayingTips = a.RoleplayingTips };
            foreach (var s in a.Sections) dto.Sections.Add(new SectionDto { Header = s.Header, Details = s.Details });
            foreach (var cl in a.ConcordanceLevels) dto.ConcordanceLevels.Add(MapToArtifactConcordanceDto(cl));
            foreach (var rule in a.ConcordanceRules) dto.ConcordanceRules.Add(new PairDto<string, string> { First = rule.First, Second = rule.Second });
            return dto;
        }

        private ArtifactConcordance MapToArtifactConcordance(ArtifactConcordanceDto dto)
        {
            if (dto == null) return null;
            var ac = new ArtifactConcordance { Name = dto.Name, ValueRange = dto.ValueRange, Quote = dto.Quote, Description = dto.Description };
            if (dto.Sections != null) foreach (var s in dto.Sections) ac.Sections.Add(new MagicItemSection { Header = s.Header, Details = s.Details });
            return ac;
        }

        private ArtifactConcordanceDto MapToArtifactConcordanceDto(ArtifactConcordance ac)
        {
            if (ac == null) return null;
            var dto = new ArtifactConcordanceDto { Name = ac.Name, ValueRange = ac.ValueRange, Quote = ac.Quote, Description = ac.Description };
            foreach (var s in ac.Sections) dto.Sections.Add(new SectionDto { Header = s.Header, Details = s.Details });
            return dto;
        }
        #endregion

        #region Theme Mapping
        private MonsterTheme MapToTheme(ThemeDto dto)
        {
            if (dto == null) return null;
            var th = new MonsterTheme { ID = dto.ID, Name = dto.Name };
            if (dto.Powers != null) foreach (var p in dto.Powers) th.Powers.Add(new ThemePowerData { Power = MapToPower(p.Power), Type = Enum.Parse<PowerType>(p.Type), Roles = p.Roles.Select(r => Enum.Parse<RoleType>(r)).ToList() });
            if (dto.SkillBonuses != null) foreach (var sb in dto.SkillBonuses) th.SkillBonuses.Add(new Pair<string, int>(sb.First, sb.Second));
            return th;
        }

        private ThemeDto MapToThemeDto(MonsterTheme th)
        {
            if (th == null) return null;
            var dto = new ThemeDto { ID = th.ID, Name = th.Name };
            foreach (var p in th.Powers) dto.Powers.Add(new ThemePowerDataDto { Power = MapToPowerDto(p.Power), Type = p.Type.ToString(), Roles = p.Roles.Select(r => r.ToString()).ToList() });
            foreach (var sb in th.SkillBonuses) dto.SkillBonuses.Add(new PairDto<string, int> { First = sb.First, Second = sb.Second });
            return dto;
        }
        #endregion

        #region Template Mapping
        private CreatureTemplate MapToTemplate(TemplateDto dto)
        {
            if (dto == null) return null;
            var t = new CreatureTemplate
            {
                ID = dto.ID,
                Name = dto.Name,
                Type = Enum.Parse<CreatureTemplateType>(dto.Type),
                Role = Enum.Parse<RoleType>(dto.Role),
                Leader = dto.Leader,
                Senses = dto.Senses,
                Movement = dto.Movement,
                HP = dto.HP,
                Initiative = dto.Initiative,
                AC = dto.AC,
                Fortitude = dto.Fortitude,
                Reflex = dto.Reflex,
                Will = dto.Will,
                Regeneration = MapToRegeneration(dto.Regeneration),
                Resist = dto.Resist,
                Vulnerable = dto.Vulnerable,
                Immune = dto.Immune,
                Tactics = dto.Tactics
            };
            if (dto.Auras != null) foreach (var a in dto.Auras) t.Auras.Add(new Aura { ID = a.ID, Name = a.Name, Keywords = a.Keywords, Details = a.Details });
            if (dto.Powers != null) foreach (var p in dto.Powers) t.CreaturePowers.Add(MapToPower(p));
            if (dto.DamageModifierTemplates != null) foreach (var dm in dto.DamageModifierTemplates) t.DamageModifierTemplates.Add(new DamageModifierTemplate { Type = Enum.Parse<DamageType>(dm.Type), HeroicValue = dm.HeroicValue, ParagonValue = dm.ParagonValue, EpicValue = dm.EpicValue });
            return t;
        }

        private TemplateDto MapToTemplateDto(CreatureTemplate t)
        {
            if (t == null) return null;
            var dto = new TemplateDto
            {
                ID = t.ID,
                Name = t.Name,
                Type = t.Type.ToString(),
                Role = t.Role.ToString(),
                Leader = t.Leader,
                Senses = t.Senses,
                Movement = t.Movement,
                HP = t.HP,
                Initiative = t.Initiative,
                AC = t.AC,
                Fortitude = t.Fortitude,
                Reflex = t.Reflex,
                Will = t.Will,
                Regeneration = MapToRegenerationDto(t.Regeneration),
                Resist = t.Resist,
                Vulnerable = t.Vulnerable,
                Immune = t.Immune,
                Tactics = t.Tactics
            };
            foreach (var a in t.Auras) dto.Auras.Add(new AuraDto { ID = a.ID, Name = a.Name, Keywords = a.Keywords, Details = a.Details });
            foreach (var p in t.CreaturePowers) dto.Powers.Add(MapToPowerDto(p));
            foreach (var dm in t.DamageModifierTemplates) dto.DamageModifierTemplates.Add(new DamageModifierTemplateDto { Type = dm.Type.ToString(), HeroicValue = dm.HeroicValue, ParagonValue = dm.ParagonValue, EpicValue = dm.EpicValue });
            return dto;
        }
        #endregion

        #region Tile Mapping
        private Tile MapToTile(TileDto dto)
        {
            if (dto == null) return null;
            return new Tile
            {
                ID = dto.ID,
                Category = Enum.Parse<TileCategory>(dto.Category),
                Size = new Size(dto.Width, dto.Height),
                Image = ByteArrayToImage(dto.ImageData),
                Keywords = dto.Keywords,
                BlankColour = Color.FromArgb(dto.ARGB)
            };
        }

        private TileDto MapToTileDto(Tile t)
        {
            if (t == null) return null;
            return new TileDto
            {
                ID = t.ID,
                Category = t.Category.ToString(),
                Width = t.Size.Width,
                Height = t.Size.Height,
                ImageData = ImageToByteArray(t.Image),
                Keywords = t.Keywords,
                ARGB = t.BlankColour.ToArgb()
            };
        }
        #endregion

        #region Plot Mapping
        private Plot MapToPlot(PlotDto dto)
        {
            var plot = new Plot();
            if (dto != null && dto.Points != null) foreach (var p in dto.Points) plot.Points.Add(MapToPlotPoint(p));
            return plot;
        }

        private PlotDto MapToPlotDto(Plot plot)
        {
            var dto = new PlotDto();
            if (plot != null) foreach (var p in plot.Points) dto.Points.Add(MapToPlotPointDto(p));
            return dto;
        }

        private PlotPoint MapToPlotPoint(PlotPointDto dto)
        {
            if (dto == null) return null;
            var pp = new PlotPoint(dto.Name) { ID = dto.ID, Details = dto.Details, ReadAloud = dto.ReadAloud };
            pp.State = Enum.Parse<PlotPointState>(dto.State);
            pp.Colour = Enum.Parse<PlotPointColour>(dto.Colour);
            if (dto.Subplot != null) pp.Subplot = MapToPlot(dto.Subplot);
            if (dto.Element != null) pp.Element = MapToElement(dto.Element);
            if (dto.Parcels != null) foreach (var p in dto.Parcels) pp.Parcels.Add(MapToParcel(p));
            if (dto.Links != null) pp.Links.AddRange(dto.Links);
            if (dto.EncyclopediaEntryIDs != null) pp.EncyclopediaEntryIDs.AddRange(dto.EncyclopediaEntryIDs);
            return pp;
        }

        private PlotPointDto MapToPlotPointDto(PlotPoint pp)
        {
            if (pp == null) return null;
            var dto = new PlotPointDto
            {
                ID = pp.ID,
                Name = pp.Name,
                Details = pp.Details,
                ReadAloud = pp.ReadAloud,
                State = pp.State.ToString(),
                Colour = pp.Colour.ToString(),
                Subplot = MapToPlotDto(pp.Subplot),
                Element = MapToElementDto(pp.Element)
            };
            foreach (var p in pp.Parcels) dto.Parcels.Add(MapToParcelDto(p));
            dto.Links.AddRange(pp.Links);
            dto.EncyclopediaEntryIDs.AddRange(pp.EncyclopediaEntryIDs);
            return dto;
        }

        private IElement MapToElement(ElementDto dto)
        {
            if (dto == null) return null;
            switch (dto.Type)
            {
                case "Encounter":
                    return MapToEncounter(MessagePackSerializer.Deserialize<EncounterDto>(dto.Data));
                case "SkillChallenge":
                    return MapToSkillChallenge(MessagePackSerializer.Deserialize<SkillChallengeDto>(dto.Data));
                case "Trap":
                    return new TrapElement { Trap = MapToTrap(MessagePackSerializer.Deserialize<TrapDto>(dto.Data)) };
                case "Quest":
                    return MapToQuest(MessagePackSerializer.Deserialize<QuestDto>(dto.Data));
                case "Map":
                    return MapToMapElement(MessagePackSerializer.Deserialize<MapElementDto>(dto.Data));
                default:
                    return null;
            }
        }

        private ElementDto MapToElementDto(IElement element)
        {
            if (element == null) return null;
            if (element is Encounter enc)
                return new ElementDto { Type = "Encounter", Data = MessagePackSerializer.Serialize(MapToEncounterDto(enc)) };
            if (element is SkillChallenge sc)
                return new ElementDto { Type = "SkillChallenge", Data = MessagePackSerializer.Serialize(MapToSkillChallengeDto(sc)) };
            if (element is TrapElement te)
                return new ElementDto { Type = "Trap", Data = MessagePackSerializer.Serialize(MapToTrapDto(te.Trap)) };
            if (element is Quest q)
                return new ElementDto { Type = "Quest", Data = MessagePackSerializer.Serialize(MapToQuestDto(q)) };
            if (element is MapElement me)
                return new ElementDto { Type = "Map", Data = MessagePackSerializer.Serialize(MapToMapElementDto(me)) };
            return null;
        }

        private Quest MapToQuest(QuestDto dto)
        {
            if (dto == null) return null;
            return new Quest { Level = dto.Level, Type = Enum.Parse<QuestType>(dto.Type), XP = dto.XP };
        }

        private QuestDto MapToQuestDto(Quest q)
        {
            if (q == null) return null;
            return new QuestDto { Level = q.Level, Type = q.Type.ToString(), XP = q.XP };
        }

        private MapElement MapToMapElement(MapElementDto dto)
        {
            if (dto == null) return null;
            return new MapElement(dto.MapID, dto.MapAreaID);
        }

        private MapElementDto MapToMapElementDto(MapElement me)
        {
            if (me == null) return null;
            return new MapElementDto { MapID = me.MapID, MapAreaID = me.MapAreaID };
        }

        private Encounter MapToEncounter(EncounterDto dto)
        {
            if (dto == null) return new Encounter();
            var enc = new Encounter { MapID = dto.MapID, MapAreaID = dto.MapAreaID };
            if (dto.Slots != null) foreach (var s in dto.Slots) enc.Slots.Add(MapToEncounterSlot(s));
            if (dto.Traps != null) foreach (var t in dto.Traps) enc.Traps.Add(MapToTrap(t));
            if (dto.SkillChallenges != null) foreach (var sc in dto.SkillChallenges) enc.SkillChallenges.Add(MapToSkillChallenge(sc));
            if (dto.CustomTokens != null) foreach (var ct in dto.CustomTokens) enc.CustomTokens.Add(MapToCustomToken(ct));
            if (dto.Notes != null) foreach (var n in dto.Notes) enc.Notes.Add(new EncounterNote { ID = n.ID, Title = n.Title, Contents = n.Contents });
            if (dto.Waves != null) foreach (var w in dto.Waves) enc.Waves.Add(MapToEncounterWave(w));
            return enc;
        }

        private EncounterDto MapToEncounterDto(Encounter enc)
        {
            if (enc == null) return null;
            var dto = new EncounterDto { MapID = enc.MapID, MapAreaID = enc.MapAreaID };
            foreach (var s in enc.Slots) dto.Slots.Add(MapToEncounterSlotDto(s));
            foreach (var t in enc.Traps) dto.Traps.Add(MapToTrapDto(t));
            foreach (var sc in enc.SkillChallenges) dto.SkillChallenges.Add(MapToSkillChallengeDto(sc));
            foreach (var ct in enc.CustomTokens) dto.CustomTokens.Add(MapToCustomTokenDto(ct));
            foreach (var n in enc.Notes) dto.Notes.Add(new EncounterNoteDto { ID = n.ID, Title = n.Title, Contents = n.Contents });
            foreach (var w in enc.Waves) dto.Waves.Add(MapToEncounterWaveDto(w));
            return dto;
        }

        private EncounterSlot MapToEncounterSlot(EncounterSlotDto dto)
        {
            if (dto == null) return null;
            var slot = new EncounterSlot { ID = dto.ID, Type = Enum.Parse<EncounterSlotType>(dto.Type) };
            if (dto.Card != null)
            {
                slot.Card = new EncounterCard
                {
                    CreatureID = dto.Card.CreatureID,
                    TemplateIDs = dto.Card.TemplateIDs ?? new List<Guid>(),
                    LevelAdjustment = dto.Card.LevelAdjustment,
                    ThemeID = dto.Card.ThemeID,
                    ThemeAttackPowerID = dto.Card.ThemeAttackPowerID,
                    ThemeUtilityPowerID = dto.Card.ThemeUtilityPowerID,
                    Drawn = dto.Card.Drawn
                };
            }
            if (dto.CombatData != null) foreach (var cd in dto.CombatData) slot.CombatData.Add(MapToCombatData(cd));
            return slot;
        }

        private EncounterSlotDto MapToEncounterSlotDto(EncounterSlot slot)
        {
            if (slot == null) return null;
            var dto = new EncounterSlotDto { ID = slot.ID, Type = slot.Type.ToString() };
            if (slot.Card != null)
            {
                dto.Card = new EncounterCardDto
                {
                    CreatureID = slot.Card.CreatureID,
                    TemplateIDs = slot.Card.TemplateIDs,
                    LevelAdjustment = slot.Card.LevelAdjustment,
                    ThemeID = slot.Card.ThemeID,
                    ThemeAttackPowerID = slot.Card.ThemeAttackPowerID,
                    ThemeUtilityPowerID = slot.Card.ThemeUtilityPowerID,
                    Drawn = slot.Card.Drawn,
                    Title = slot.Card.Title,
                    XP = slot.Card.XP
                };
            }
            foreach (var cd in slot.CombatData) dto.CombatData.Add(MapToCombatDataDto(cd));
            return dto;
        }

        private EncounterWave MapToEncounterWave(EncounterWaveDto dto)
        {
            if (dto == null) return null;
            var w = new EncounterWave { ID = dto.ID, Name = dto.Name, Active = dto.Active };
            if (dto.Slots != null) foreach (var s in dto.Slots) w.Slots.Add(MapToEncounterSlot(s));
            return w;
        }

        private EncounterWaveDto MapToEncounterWaveDto(EncounterWave w)
        {
            if (w == null) return null;
            var dto = new EncounterWaveDto { ID = w.ID, Name = w.Name, Active = w.Active };
            foreach (var s in w.Slots) dto.Slots.Add(MapToEncounterSlotDto(s));
            return dto;
        }

        private CombatData MapToCombatData(CombatDataDto dto)
        {
            if (dto == null) return null;
            var cd = new CombatData { ID = dto.ID, DisplayName = dto.DisplayName, Location = new Point(dto.X, dto.Y), Visible = dto.Visible, Initiative = dto.Initiative, Delaying = dto.Delaying, Damage = dto.Damage, TempHP = dto.TempHP, Altitude = dto.Altitude };
            if (dto.UsedPowers != null) cd.UsedPowers.AddRange(dto.UsedPowers);
            if (dto.Conditions != null) foreach (var c in dto.Conditions) cd.Conditions.Add(MapToOngoingCondition(c));
            return cd;
        }

        private CombatDataDto MapToCombatDataDto(CombatData cd)
        {
            if (cd == null) return null;
            var dto = new CombatDataDto { ID = cd.ID, DisplayName = cd.DisplayName, X = cd.Location.X, Y = cd.Location.Y, Visible = cd.Visible, Initiative = cd.Initiative, Delaying = cd.Delaying, Damage = cd.Damage, TempHP = cd.TempHP, Altitude = cd.Altitude };
            if (cd.UsedPowers != null) dto.UsedPowers.AddRange(cd.UsedPowers);
            foreach (var c in cd.Conditions) dto.Conditions.Add(MapToOngoingConditionDto(c));
            return dto;
        }
        #endregion

        #region Component Mapping
        private Encyclopedia MapToEncyclopedia(EncyclopediaDto dto)
        {
            var e = new Encyclopedia();
            if (dto != null && dto.Entries != null) foreach (var entry in dto.Entries) e.Entries.Add(MapToEncyclopediaEntry(entry));
            return e;
        }

        private EncyclopediaDto MapToEncyclopediaDto(Encyclopedia e)
        {
            var dto = new EncyclopediaDto();
            if (e != null) foreach (var entry in e.Entries) dto.Entries.Add(MapToEncyclopediaEntryDto(entry));
            return dto;
        }

        private EncyclopediaEntry MapToEncyclopediaEntry(EncyclopediaEntryDto dto)
        {
            if (dto == null) return null;
            var entry = new EncyclopediaEntry { ID = dto.ID, Name = dto.Name, Category = dto.Category, Details = dto.Details, DMInfo = dto.DMInfo };
            if (dto.Images != null) foreach (var img in dto.Images) entry.Images.Add(new EncyclopediaImage { ID = img.ID, Name = img.Name, Image = ByteArrayToImage(img.ImageData) });
            return entry;
        }

        private EncyclopediaEntryDto MapToEncyclopediaEntryDto(EncyclopediaEntry entry)
        {
            if (entry == null) return null;
            var dto = new EncyclopediaEntryDto { ID = entry.ID, Name = entry.Name, Category = entry.Category, Details = entry.Details, DMInfo = entry.DMInfo };
            foreach (var img in entry.Images) dto.Images.Add(new EncyclopediaImageDto { ID = img.ID, Name = img.Name, ImageData = ImageToByteArray(img.Image) });
            return dto;
        }

        private Map MapToMap(MapDto dto)
        {
            if (dto == null) return null;
            var m = new Map { ID = dto.ID, Name = dto.Name, Category = dto.Category };
            if (dto.Tiles != null) foreach (var t in dto.Tiles) m.Tiles.Add(new TileData { ID = t.ID, TileID = t.TileID, Location = new Point(t.X, t.Y), Rotations = t.Rotations });
            if (dto.Areas != null) foreach (var a in dto.Areas) m.Areas.Add(new MapArea { ID = a.ID, Name = a.Name, Details = a.Details, Region = new Rectangle(a.X, a.Y, a.Width, a.Height) });
            return m;
        }

        private MapDto MapToMapDto(Map m)
        {
            if (m == null) return null;
            var dto = new MapDto { ID = m.ID, Name = m.Name, Category = m.Category };
            foreach (var t in m.Tiles) dto.Tiles.Add(new MapTileDto { ID = t.ID, TileID = t.TileID, X = t.Location.X, Y = t.Location.Y, Rotations = t.Rotations });
            foreach (var a in m.Areas) dto.Areas.Add(new MapAreaDto { ID = a.ID, Name = a.Name, Details = a.Details, X = a.Region.X, Y = a.Region.Y, Width = a.Region.Width, Height = a.Region.Height });
            return dto;
        }

        private RegionalMap MapToRegionalMap(RegionalMapDto dto)
        {
            if (dto == null) return null;
            var rm = new RegionalMap { ID = dto.ID, Name = dto.Name, Image = ByteArrayToImage(dto.ImageData) };
            if (dto.Locations != null) foreach (var l in dto.Locations) rm.Locations.Add(new MapLocation { ID = l.ID, Name = l.Name, Category = l.Category, Point = new PointF(l.X, l.Y) });
            return rm;
        }

        private RegionalMapDto MapToRegionalMapDto(RegionalMap rm)
        {
            if (rm == null) return null;
            var dto = new RegionalMapDto { ID = rm.ID, Name = rm.Name, ImageData = ImageToByteArray(rm.Image) };
            foreach (var l in rm.Locations) dto.Locations.Add(new MapLocationDto { ID = l.ID, Name = l.Name, Category = l.Category, X = l.Point.X, Y = l.Point.Y });
            return dto;
        }

        private EncounterDeck MapToDeck(DeckDto dto)
        {
            if (dto == null) return null;
            var d = new EncounterDeck { ID = dto.ID, Name = dto.Name, Level = dto.Level };
            if (dto.Cards != null) foreach (var c in dto.Cards) d.Cards.Add(new EncounterCard { CreatureID = c.CreatureID, TemplateIDs = c.TemplateIDs ?? new List<Guid>(), LevelAdjustment = c.LevelAdjustment, ThemeID = c.ThemeID });
            return d;
        }

        private DeckDto MapToDeckDto(EncounterDeck d)
        {
            if (d == null) return null;
            var dto = new DeckDto { ID = d.ID, Name = d.Name, Level = d.Level };
            foreach (var c in d.Cards) dto.Cards.Add(new EncounterCardDto { CreatureID = c.CreatureID, TemplateIDs = c.TemplateIDs, LevelAdjustment = c.LevelAdjustment, ThemeID = c.ThemeID, Title = c.Title, XP = c.XP });
            return dto;
        }

        private NPC MapToNPC(NPCDto dto)
        {
            if (dto == null) return null;
            return new NPC { ID = dto.ID, TemplateID = dto.TemplateID };
        }

        private NPCDto MapToNPCDto(NPC npc)
        {
            if (npc == null) return null;
            return new NPCDto { ID = npc.ID, TemplateID = npc.TemplateID };
        }

        private Calendar MapToCalendar(CalendarDto dto)
        {
            if (dto == null) return null;
            return new Calendar { ID = dto.ID, Name = dto.Name, Details = dto.Details, CampaignYear = dto.CampaignYear };
        }

        private CalendarDto MapToCalendarDto(Calendar c)
        {
            if (c == null) return null;
            return new CalendarDto { ID = c.ID, Name = c.Name, Details = c.Details, CampaignYear = c.CampaignYear };
        }

        private Attachment MapToAttachment(AttachmentDto dto)
        {
            if (dto == null) return null;
            return new Attachment { ID = dto.ID, Name = dto.Name, Contents = dto.Contents };
        }

        private AttachmentDto MapToAttachmentDto(Attachment a)
        {
            if (a == null) return null;
            return new AttachmentDto { ID = a.ID, Name = a.Name, Contents = a.Contents };
        }

        private Background MapToBackground(BackgroundDto dto)
        {
            if (dto == null) return null;
            return new Background { ID = dto.ID, Title = dto.Title, Details = dto.Details };
        }

        private BackgroundDto MapToBackgroundDto(Background b)
        {
            if (b == null) return null;
            return new BackgroundDto { ID = b.ID, Title = b.Title, Details = b.Details };
        }

        private Parcel MapToParcel(ParcelDto dto)
        {
            if (dto == null) return null;
            return new Parcel { Name = dto.Name, Details = dto.Details, Value = dto.Value, MagicItemID = dto.MagicItemID, ArtifactID = dto.ArtifactID, HeroID = dto.HeroID };
        }

        private ParcelDto MapToParcelDto(Parcel p)
        {
            if (p == null) return null;
            return new ParcelDto { Name = p.Name, Details = p.Details, Value = p.Value, MagicItemID = p.MagicItemID, ArtifactID = p.ArtifactID, HeroID = p.HeroID };
        }

        private CampaignSettings MapToCampaignSettings(CampaignSettingsDto dto)
        {
            if (dto == null) return null;
            return new CampaignSettings { HP = dto.HP, XP = dto.XP, AttackBonus = dto.AttackBonus, Damage = dto.Damage, ACBonus = dto.ACBonus, NADBonus = dto.NADBonus };
        }

        private CampaignSettingsDto MapToCampaignSettingsDto(CampaignSettings cs)
        {
            if (cs == null) return null;
            return new CampaignSettingsDto { HP = cs.HP, XP = cs.XP, AttackBonus = cs.AttackBonus, Damage = cs.Damage, ACBonus = cs.ACBonus, NADBonus = cs.NADBonus };
        }

        private MagicItem MapToMagicItem(MagicItemDto dto)
        {
            if (dto == null) return null;
            var mi = new MagicItem { ID = dto.ID, Name = dto.Name, Type = dto.Type, Rarity = Enum.Parse<MagicItemRarity>(dto.Rarity), Level = dto.Level, Description = dto.Description };
            if (dto.Sections != null) foreach (var s in dto.Sections) mi.Sections.Add(new MagicItemSection { Header = s.Header, Details = s.Details });
            return mi;
        }

        private MagicItemDto MapToMagicItemDto(MagicItem mi)
        {
            if (mi == null) return null;
            var dto = new MagicItemDto { ID = mi.ID, Name = mi.Name, Type = mi.Type, Rarity = mi.Rarity.ToString(), Level = mi.Level, Description = mi.Description, Info = mi.Info };
            foreach (var s in mi.Sections) dto.Sections.Add(new SectionDto { Header = s.Header, Details = s.Details });
            return dto;
        }

        private TerrainPower MapToTerrainPower(TerrainPowerDto dto)
        {
            if (dto == null) return null;
            return new TerrainPower
            {
                ID = dto.ID,
                Name = dto.Name,
                Type = Enum.Parse<TerrainPowerType>(dto.Type),
                FlavourText = dto.FlavourText,
                Action = Enum.Parse<ActionType>(dto.Action),
                Requirement = dto.Requirement,
                Check = dto.Check,
                Success = dto.Success,
                Failure = dto.Failure,
                Target = dto.Target,
                Attack = dto.Attack,
                Hit = dto.Hit,
                Miss = dto.Miss,
                Effect = dto.Effect
            };
        }

        private TerrainPowerDto MapToTerrainPowerDto(TerrainPower tp)
        {
            if (tp == null) return null;
            return new TerrainPowerDto
            {
                ID = tp.ID,
                Name = tp.Name,
                Type = tp.Type.ToString(),
                FlavourText = tp.FlavourText,
                Action = tp.Action.ToString(),
                Requirement = tp.Requirement,
                Check = tp.Check,
                Success = tp.Success,
                Failure = tp.Failure,
                Target = tp.Target,
                Attack = tp.Attack,
                Hit = tp.Hit,
                Miss = tp.Miss,
                Effect = tp.Effect
            };
        }

        private Trap MapToTrap(TrapDto dto)
        {
            if (dto == null) return null;
            var t = new Trap { ID = dto.ID, Name = dto.Name, Type = Enum.Parse<TrapType>(dto.Type), Level = dto.Level, Role = MapToRole(dto.Role), ReadAloud = dto.ReadAloud, Description = dto.Description, Details = dto.Details, Initiative = dto.Initiative, Trigger = dto.Trigger };
            if (dto.Skills != null) foreach (var s in dto.Skills) t.Skills.Add(new TrapSkillData { ID = s.ID, SkillName = s.SkillName, DC = s.DC, Details = s.Details });
            if (dto.Attacks != null) foreach (var a in dto.Attacks) t.Attacks.Add(MapToTrapAttack(a));
            if (dto.Countermeasures != null) t.Countermeasures.AddRange(dto.Countermeasures.Split('|', StringSplitOptions.RemoveEmptyEntries));
            return t;
        }

        private TrapDto MapToTrapDto(Trap t)
        {
            if (t == null) return null;
            var dto = new TrapDto { ID = t.ID, Name = t.Name, Type = t.Type.ToString(), Level = t.Level, Role = MapToRoleDto(t.Role), ReadAloud = t.ReadAloud, Description = t.Description, Details = t.Details, Initiative = t.Initiative, Trigger = t.Trigger, Info = t.Info, XP = t.XP };
            foreach (var s in t.Skills) dto.Skills.Add(new TrapSkillDto { ID = s.ID, SkillName = s.SkillName, DC = s.DC, Details = s.Details });
            foreach (var a in t.Attacks) dto.Attacks.Add(MapToTrapAttackDto(a));
            dto.Countermeasures = string.Join("|", t.Countermeasures);
            return dto;
        }

        private TrapAttack MapToTrapAttack(TrapAttackDto dto)
        {
            if (dto == null) return null;
            var a = new TrapAttack { ID = dto.ID, Name = dto.Name, Trigger = dto.Trigger, Action = Enum.Parse<ActionType>(dto.Action), Range = dto.Range, Keywords = dto.Keywords, Target = dto.Target, HasInitiative = dto.HasInitiative, Initiative = dto.Initiative, OnHit = dto.OnHit, OnMiss = dto.OnMiss, Effect = dto.Effect, Notes = dto.Notes };
            if (dto.Attack != null) a.Attack = new PowerAttack { Bonus = dto.Attack.Bonus, Defence = Enum.Parse<DefenceType>(dto.Attack.Defence) };
            return a;
        }

        private TrapAttackDto MapToTrapAttackDto(TrapAttack a)
        {
            if (a == null) return null;
            var dto = new TrapAttackDto { ID = a.ID, Name = a.Name, Trigger = a.Trigger, Action = a.Action.ToString(), Range = a.Range, Keywords = a.Keywords, Target = a.Target, HasInitiative = a.HasInitiative, Initiative = a.Initiative, OnHit = a.OnHit, OnMiss = a.OnMiss, Effect = a.Effect, Notes = a.Notes };
            if (a.Attack != null) dto.Attack = new PowerAttackDto { Bonus = a.Attack.Bonus, Defence = a.Attack.Defence.ToString() };
            return dto;
        }

        private SkillChallenge MapToSkillChallenge(SkillChallengeDto dto)
        {
            if (dto == null) return null;
            var sc = new SkillChallenge { ID = dto.ID, Name = dto.Name, Level = dto.Level, Complexity = dto.Complexity, Success = dto.Success, Failure = dto.Failure, Notes = dto.Notes, MapID = dto.MapID, MapAreaID = dto.MapAreaID };
            if (dto.Skills != null) foreach (var s in dto.Skills) sc.Skills.Add(new SkillChallengeData { SkillName = s.SkillName, Difficulty = Enum.Parse<Difficulty>(s.Difficulty), DCModifier = s.DCModifier, Details = s.Details, Success = s.Success, Failure = s.Failure });
            return sc;
        }

        private SkillChallengeDto MapToSkillChallengeDto(SkillChallenge sc)
        {
            if (sc == null) return null;
            var dto = new SkillChallengeDto { ID = sc.ID, Name = sc.Name, Level = sc.Level, Complexity = sc.Complexity, Success = sc.Success, Failure = sc.Failure, Notes = sc.Notes, MapID = sc.MapID, MapAreaID = sc.MapAreaID, Successes = sc.Successes, Info = sc.Info };
            foreach (var s in sc.Skills) dto.Skills.Add(new SkillChallengeDataDto { SkillName = s.SkillName, Difficulty = s.Difficulty.ToString(), DCModifier = s.DCModifier, Details = s.Details, Success = s.Success, Failure = s.Failure });
            return dto;
        }

        private OngoingCondition MapToOngoingCondition(OngoingConditionDto dto)
        {
            if (dto == null) return null;
            var oc = new OngoingCondition
            {
                Type = Enum.Parse<OngoingType>(dto.Type),
                Data = dto.Data,
                DamageType = Enum.Parse<DamageType>(dto.DamageType),
                Value = dto.Value,
                DefenceMod = dto.DefenceMod,
                Regeneration = MapToRegeneration(dto.Regeneration) ?? new Regeneration(),
                DamageModifier = MapToDamageModifier(dto.DamageModifier) ?? new DamageModifier(),
                Aura = MapToAura(dto.Aura) ?? new Aura(),
                Duration = Enum.Parse<DurationType>(dto.Duration),
                DurationCreatureID = dto.DurationCreatureID,
                DurationRound = dto.DurationRound,
                SavingThrowModifier = dto.SavingThrowModifier
            };
            if (dto.Defences != null)
            {
                foreach (var d in dto.Defences) oc.Defences.Add(Enum.Parse<DefenceType>(d));
            }
            return oc;
        }

        private OngoingConditionDto MapToOngoingConditionDto(OngoingCondition oc)
        {
            if (oc == null) return null;
            var dto = new OngoingConditionDto
            {
                Type = oc.Type.ToString(),
                Data = oc.Data,
                DamageType = oc.DamageType.ToString(),
                Value = oc.Value,
                DefenceMod = oc.DefenceMod,
                Regeneration = MapToRegenerationDto(oc.Regeneration),
                DamageModifier = MapToDamageModifierDto(oc.DamageModifier),
                Aura = MapToAuraDto(oc.Aura),
                Duration = oc.Duration.ToString(),
                DurationCreatureID = oc.DurationCreatureID,
                DurationRound = oc.DurationRound,
                SavingThrowModifier = oc.SavingThrowModifier
            };
            foreach (var d in oc.Defences) dto.Defences.Add(d.ToString());
            return dto;
        }

        private EncounterLog MapToEncounterLog(EncounterLogDto dto)
        {
            if (dto == null) return new EncounterLog();
            var log = new EncounterLog { Active = dto.Active };
            if (dto.Entries != null)
                foreach (var e in dto.Entries) log.Entries.Add(MapToLogEntry(e));
            return log;
        }

        private EncounterLogDto MapToEncounterLogDto(EncounterLog log)
        {
            if (log == null) return null;
            var dto = new EncounterLogDto { Active = log.Active };
            foreach (var e in log.Entries) dto.Entries.Add(MapToLogEntryDto(e));
            return dto;
        }

        private IEncounterLogEntry MapToLogEntry(LogEntryDto dto)
        {
            if (dto == null) return null;
            switch (dto.Type)
            {
                case "StartRound":
                    var sr = MessagePackSerializer.Deserialize<StartRoundEntryDto>(dto.Data);
                    return new StartRoundLogEntry { Timestamp = dto.Timestamp, Round = sr.Round };
                case "StartTurn":
                    return new StartTurnLogEntry { CombatantID = dto.CombatantID, Timestamp = dto.Timestamp };
                case "Damage":
                    var d = MessagePackSerializer.Deserialize<DamageEntryDto>(dto.Data);
                    return new DamageLogEntry { CombatantID = dto.CombatantID, Timestamp = dto.Timestamp, Amount = d.Amount, Types = d.Types != null ? d.Types.Select(t => Enum.Parse<DamageType>(t)).ToList() : new List<DamageType>() };
                case "State":
                    var s = MessagePackSerializer.Deserialize<StateEntryDto>(dto.Data);
                    return new StateLogEntry { CombatantID = dto.CombatantID, Timestamp = dto.Timestamp, State = Enum.Parse<CreatureState>(s.State) };
                case "Effect":
                    var ef = MessagePackSerializer.Deserialize<EffectEntryDto>(dto.Data);
                    return new EffectLogEntry { CombatantID = dto.CombatantID, Timestamp = dto.Timestamp, EffectText = ef.EffectText, Added = ef.Added };
                case "Power":
                    var p = MessagePackSerializer.Deserialize<PowerEntryDto>(dto.Data);
                    return new PowerLogEntry { CombatantID = dto.CombatantID, Timestamp = dto.Timestamp, PowerName = p.PowerName, Added = p.Added };
                case "Skill":
                    var sk = MessagePackSerializer.Deserialize<SkillEntryDto>(dto.Data);
                    return new SkillLogEntry { CombatantID = dto.CombatantID, Timestamp = dto.Timestamp, SkillName = sk.SkillName };
                case "SkillChallenge":
                    var sc = MessagePackSerializer.Deserialize<SkillChallengeLogEntryDto>(dto.Data);
                    return new SkillChallengeLogEntry { CombatantID = dto.CombatantID, Timestamp = dto.Timestamp, Success = sc.Success };
                case "Move":
                    var m = MessagePackSerializer.Deserialize<MoveEntryDto>(dto.Data);
                    return new MoveLogEntry { CombatantID = dto.CombatantID, Timestamp = dto.Timestamp, Distance = m.Distance, Details = m.Details };
                case "Pause":
                    return new PauseLogEntry { Timestamp = dto.Timestamp };
                case "Resume":
                    return new ResumeLogEntry { Timestamp = dto.Timestamp };
                default:
                    return null;
            }
        }

        private LogEntryDto MapToLogEntryDto(IEncounterLogEntry entry)
        {
            if (entry == null) return null;
            var dto = new LogEntryDto { CombatantID = entry.CombatantID, Timestamp = entry.Timestamp };
            if (entry is StartRoundLogEntry sr) { dto.Type = "StartRound"; dto.Data = MessagePackSerializer.Serialize(new StartRoundEntryDto { Round = sr.Round }); }
            else if (entry is StartTurnLogEntry) { dto.Type = "StartTurn"; }
            else if (entry is DamageLogEntry d) { dto.Type = "Damage"; dto.Data = MessagePackSerializer.Serialize(new DamageEntryDto { Amount = d.Amount, Types = d.Types != null ? d.Types.Select(t => t.ToString()).ToList() : new List<string>() }); }
            else if (entry is StateLogEntry s) { dto.Type = "State"; dto.Data = MessagePackSerializer.Serialize(new StateEntryDto { State = s.State.ToString() }); }
            else if (entry is EffectLogEntry ef) { dto.Type = "Effect"; dto.Data = MessagePackSerializer.Serialize(new EffectEntryDto { EffectText = ef.EffectText, Added = ef.Added }); }
            else if (entry is PowerLogEntry p) { dto.Type = "Power"; dto.Data = MessagePackSerializer.Serialize(new PowerEntryDto { PowerName = p.PowerName, Added = p.Added }); }
            else if (entry is SkillLogEntry sk) { dto.Type = "Skill"; dto.Data = MessagePackSerializer.Serialize(new SkillEntryDto { SkillName = sk.SkillName }); }
            else if (entry is SkillChallengeLogEntry sc) { dto.Type = "SkillChallenge"; dto.Data = MessagePackSerializer.Serialize(new SkillChallengeLogEntryDto { Success = sc.Success }); }
            else if (entry is MoveLogEntry m) { dto.Type = "Move"; dto.Data = MessagePackSerializer.Serialize(new MoveEntryDto { Distance = m.Distance, Details = m.Details }); }
            else if (entry is PauseLogEntry) { dto.Type = "Pause"; }
            else if (entry is ResumeLogEntry) { dto.Type = "Resume"; }
            return dto;
        }

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
            if (dto == null) return null;
            var p = new CreaturePower { ID = dto.ID, Name = dto.Name, Keywords = dto.Keywords, Condition = dto.Condition, Range = dto.Range, Description = dto.Description, Details = dto.Details };
            if (dto.Action != null) p.Action = new PowerAction { Action = Enum.Parse<ActionType>(dto.Action.Action), Trigger = dto.Action.Trigger, SustainAction = Enum.Parse<ActionType>(dto.Action.SustainAction), Use = Enum.Parse<PowerUseType>(dto.Action.Use), Recharge = dto.Action.Recharge };
            if (dto.Attack != null) p.Attack = new PowerAttack { Bonus = dto.Attack.Bonus, Defence = Enum.Parse<DefenceType>(dto.Attack.Defence) };
            return p;
        }

        private CreaturePowerDto MapToPowerDto(CreaturePower p)
        {
            if (p == null) return null;
            var dto = new CreaturePowerDto { ID = p.ID, Name = p.Name, Keywords = p.Keywords, Condition = p.Condition, Range = p.Range, Description = p.Description, Details = p.Details, Damage = p.Damage, Category = p.Category.ToString() };
            if (p.Action != null) dto.Action = new PowerActionDto { Action = p.Action.Action.ToString(), Trigger = p.Action.Trigger, SustainAction = p.Action.SustainAction.ToString(), Use = p.Action.Use.ToString(), Recharge = p.Action.Recharge };
            if (p.Attack != null) dto.Attack = new PowerAttackDto { Bonus = p.Attack.Bonus, Defence = p.Attack.Defence.ToString() };
            return dto;
        }

        private Regeneration MapToRegeneration(RegenerationDto dto) => dto != null ? new Regeneration(dto.Value, dto.Details) : null;
        private RegenerationDto MapToRegenerationDto(Regeneration r) => r != null ? new RegenerationDto { Value = r.Value, Details = r.Details } : null;

        private DamageModifier MapToDamageModifier(DamageModifierDto dto)
        {
            if (dto == null) return null;
            return new DamageModifier { Type = Enum.Parse<DamageType>(dto.Type), Value = dto.Value };
        }

        private DamageModifierDto MapToDamageModifierDto(DamageModifier dm)
        {
            if (dm == null) return null;
            return new DamageModifierDto { Type = dm.Type.ToString(), Value = dm.Value };
        }

        private Aura MapToAura(AuraDto dto)
        {
            if (dto == null) return null;
            return new Aura { ID = dto.ID, Name = dto.Name, Keywords = dto.Keywords, Details = dto.Details };
        }

        private AuraDto MapToAuraDto(Aura a)
        {
            if (a == null) return null;
            return new AuraDto { ID = a.ID, Name = a.Name, Keywords = a.Keywords, Details = a.Details };
        }

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
        #endregion
    }
}

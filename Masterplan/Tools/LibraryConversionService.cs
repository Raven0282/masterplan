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

        #region Library Mapping
        public Library MapToLibrary(LibraryDto dto)
        {
            if (dto == null) return null;
            var lib = new Library { ID = dto.ID, Name = dto.Name };
            
            if (dto.Creatures != null) foreach (var c in dto.Creatures) lib.Creatures.Add(MapToCreature(c) as Creature);
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
            
            return dto;
        }
        #endregion

        #region Creature Mapping
        private ICreature MapToCreature(CreatureDto dto)
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

        private CustomCreature MapToCustomCreature(CreatureDto dto)
        {
            if (dto == null) return null;
            var cc = new CustomCreature
            {
                ID = dto.ID, Name = dto.Name, Details = dto.Details,
                Size = Enum.Parse<CreatureSize>(dto.Size), Origin = Enum.Parse<CreatureOrigin>(dto.Origin),
                Type = Enum.Parse<CreatureType>(dto.Type), Keywords = dto.Keywords,
                Level = dto.Level, Role = MapToRole(dto.Role), HP = dto.HP,
                Initiative = dto.Initiative, AC = dto.AC, Fortitude = dto.Fortitude,
                Reflex = dto.Reflex, Will = dto.Will, Senses = dto.Senses,
                Movement = dto.Movement, Alignment = dto.Alignment, Languages = dto.Languages,
                Skills = dto.Skills, Equipment = dto.Equipment,
                Tactics = dto.Tactics, Resist = dto.Resist, Vulnerable = dto.Vulnerable, Immune = dto.Immune,
                Image = ByteArrayToImage(dto.ImageData),
                Regeneration = MapToRegeneration(dto.Regeneration)
            };

            if (dto.Strength != null) cc.Strength.Score = dto.Strength.Score;
            if (dto.Constitution != null) cc.Constitution.Score = dto.Constitution.Score;
            if (dto.Dexterity != null) cc.Dexterity.Score = dto.Dexterity.Score;
            if (dto.Intelligence != null) cc.Intelligence.Score = dto.Intelligence.Score;
            if (dto.Wisdom != null) cc.Wisdom.Score = dto.Wisdom.Score;
            if (dto.Charisma != null) cc.Charisma.Score = dto.Charisma.Score;

            foreach (var p in dto.Powers) cc.CreaturePowers.Add(MapToPower(p));
            foreach (var a in dto.Auras) cc.Auras.Add(new Aura { ID = a.ID, Name = a.Name, Keywords = a.Keywords, Details = a.Details });
            foreach (var dm in dto.DamageModifiers) cc.DamageModifiers.Add(new DamageModifier { Type = Enum.Parse<DamageType>(dm.Type), Value = dm.Value });

            return cc;
        }

        private CreatureDto MapToCreatureDto(ICreature c)
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
            if (dto.Element != null) pp.Element = MapToElement(dto.Element);
            if (dto.Parcels != null) foreach (var p in dto.Parcels) pp.Parcels.Add(MapToParcel(p));
            if (dto.Links != null) pp.Links.AddRange(dto.Links);
            if (dto.EncyclopediaEntryIDs != null) pp.EncyclopediaEntryIDs.AddRange(dto.EncyclopediaEntryIDs);
            return pp;
        }

        private PlotPointDto MapToPlotPointDto(PlotPoint pp)
        {
            var dto = new PlotPointDto {
                ID = pp.ID, Name = pp.Name, Details = pp.Details, ReadAloud = pp.ReadAloud,
                State = pp.State.ToString(), Colour = pp.Colour.ToString(),
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
            return new Quest { Level = dto.Level, Type = Enum.Parse<QuestType>(dto.Type), XP = dto.XP };
        }

        private QuestDto MapToQuestDto(Quest q)
        {
            return new QuestDto { Level = q.Level, Type = q.Type.ToString(), XP = q.XP };
        }

        private MapElement MapToMapElement(MapElementDto dto)
        {
            return new MapElement(dto.MapID, dto.MapAreaID);
        }

        private MapElementDto MapToMapElementDto(MapElement me)
        {
            return new MapElementDto { MapID = me.MapID, MapAreaID = me.MapAreaID };
        }


        private Encounter MapToEncounter(EncounterDto dto)
        {
            var enc = new Encounter { MapID = dto.MapID, MapAreaID = dto.MapAreaID };
            if (dto.Slots != null) foreach (var s in dto.Slots) enc.Slots.Add(MapToEncounterSlot(s));
            if (dto.Traps != null) foreach (var t in dto.Traps) enc.Traps.Add(MapToTrap(t));
            if (dto.SkillChallenges != null) foreach (var sc in dto.SkillChallenges) enc.SkillChallenges.Add(MapToSkillChallenge(sc));
            if (dto.CustomTokens != null) foreach (var ct in dto.CustomTokens) enc.CustomTokens.Add(new CustomToken { ID = ct.ID, Name = ct.Name });
            if (dto.Notes != null) foreach (var n in dto.Notes) enc.Notes.Add(new EncounterNote { ID = n.ID, Title = n.Title, Contents = n.Contents });
            if (dto.Waves != null) foreach (var w in dto.Waves) enc.Waves.Add(MapToEncounterWave(w));
            return enc;
        }

        private EncounterDto MapToEncounterDto(Encounter enc)
        {
            var dto = new EncounterDto { MapID = enc.MapID, MapAreaID = enc.MapAreaID };
            foreach (var s in enc.Slots) dto.Slots.Add(MapToEncounterSlotDto(s));
            foreach (var t in enc.Traps) dto.Traps.Add(MapToTrapDto(t));
            foreach (var sc in enc.SkillChallenges) dto.SkillChallenges.Add(MapToSkillChallengeDto(sc));
            foreach (var ct in enc.CustomTokens) dto.CustomTokens.Add(new CustomTokenDto { ID = ct.ID, Name = ct.Name });
            foreach (var n in enc.Notes) dto.Notes.Add(new EncounterNoteDto { ID = n.ID, Title = n.Title, Contents = n.Contents });
            foreach (var w in enc.Waves) dto.Waves.Add(MapToEncounterWaveDto(w));
            return dto;
        }

        private EncounterSlot MapToEncounterSlot(EncounterSlotDto dto)
        {
            var slot = new EncounterSlot { ID = dto.ID, Type = Enum.Parse<EncounterSlotType>(dto.Type) };
            if (dto.Card != null) slot.Card = new EncounterCard { CreatureID = dto.Card.CreatureID, TemplateIDs = dto.Card.TemplateIDs, LevelAdjustment = dto.Card.LevelAdjustment, ThemeID = dto.Card.ThemeID };
            if (dto.CombatData != null) foreach (var cd in dto.CombatData) slot.CombatData.Add(MapToCombatData(cd));
            return slot;
        }

        private EncounterSlotDto MapToEncounterSlotDto(EncounterSlot slot)
        {
            var dto = new EncounterSlotDto { ID = slot.ID, Type = slot.Type.ToString(), Card = new EncounterCardDto { CreatureID = slot.Card.CreatureID, TemplateIDs = slot.Card.TemplateIDs, LevelAdjustment = slot.Card.LevelAdjustment, ThemeID = slot.Card.ThemeID, Title = slot.Card.Title, XP = slot.Card.XP } };
            foreach (var cd in slot.CombatData) dto.CombatData.Add(MapToCombatDataDto(cd));
            return dto;
        }

        private EncounterWave MapToEncounterWave(EncounterWaveDto dto)
        {
            var w = new EncounterWave { ID = dto.ID, Name = dto.Name, Active = dto.Active };
            if (dto.Slots != null) foreach (var s in dto.Slots) w.Slots.Add(MapToEncounterSlot(s));
            return w;
        }

        private EncounterWaveDto MapToEncounterWaveDto(EncounterWave w)
        {
            var dto = new EncounterWaveDto { ID = w.ID, Name = w.Name, Active = w.Active };
            foreach (var s in w.Slots) dto.Slots.Add(MapToEncounterSlotDto(s));
            return dto;
        }

        private CombatData MapToCombatData(CombatDataDto dto)
        {
            var cd = new CombatData { ID = dto.ID, DisplayName = dto.DisplayName, Location = new Point(dto.X, dto.Y), Visible = dto.Visible, Initiative = dto.Initiative, Delaying = dto.Delaying, Damage = dto.Damage, TempHP = dto.TempHP, Altitude = dto.Altitude };
            if (dto.UsedPowers != null) cd.UsedPowers.AddRange(dto.UsedPowers);
            if (dto.Conditions != null) foreach (var c in dto.Conditions) cd.Conditions.Add(MapToOngoingCondition(c));
            return cd;
        }

        private CombatDataDto MapToCombatDataDto(CombatData cd)
        {
            var dto = new CombatDataDto { ID = cd.ID, DisplayName = cd.DisplayName, X = cd.Location.X, Y = cd.Location.Y, Visible = cd.Visible, Initiative = cd.Initiative, Delaying = cd.Delaying, Damage = cd.Damage, TempHP = cd.TempHP, Altitude = cd.Altitude };
            dto.UsedPowers.AddRange(cd.UsedPowers);
            foreach (var c in cd.Conditions) dto.Conditions.Add(MapToOngoingConditionDto(c));
            return dto;
        }

        private OngoingCondition MapToOngoingCondition(OngoingConditionDto dto)
        {
            var oc = new OngoingCondition { Type = Enum.Parse<OngoingType>(dto.Type), Data = dto.Data, DamageType = Enum.Parse<DamageType>(dto.DamageType), Value = dto.Value, DefenceMod = dto.DefenceMod, Duration = Enum.Parse<DurationType>(dto.Duration), DurationCreatureID = dto.DurationCreatureID, DurationRound = dto.DurationRound, SavingThrowModifier = dto.SavingThrowModifier };
            if (dto.Defences != null) foreach (var d in dto.Defences) oc.Defences.Add(Enum.Parse<DefenceType>(d));
            if (dto.Regeneration != null) oc.Regeneration = MapToRegeneration(dto.Regeneration);
            if (dto.DamageModifier != null) oc.DamageModifier = new DamageModifier { Type = Enum.Parse<DamageType>(dto.DamageModifier.Type), Value = dto.DamageModifier.Value };
            if (dto.Aura != null) oc.Aura = new Aura { ID = dto.Aura.ID, Name = dto.Aura.Name, Keywords = dto.Aura.Keywords, Details = dto.Aura.Details };
            return oc;
        }

        private OngoingConditionDto MapToOngoingConditionDto(OngoingCondition oc)
        {
            var dto = new OngoingConditionDto { Type = oc.Type.ToString(), Data = oc.Data, DamageType = oc.DamageType.ToString(), Value = oc.Value, DefenceMod = oc.DefenceMod, Duration = oc.Duration.ToString(), DurationCreatureID = oc.DurationCreatureID, DurationRound = oc.DurationRound, SavingThrowModifier = oc.SavingThrowModifier };
            foreach (var d in oc.Defences) dto.Defences.Add(d.ToString());
            if (oc.Regeneration != null) dto.Regeneration = MapToRegenerationDto(oc.Regeneration);
            if (oc.DamageModifier != null) dto.DamageModifier = new DamageModifierDto { Type = oc.DamageModifier.Type.ToString(), Value = oc.DamageModifier.Value };
            if (oc.Aura != null) dto.Aura = new AuraDto { ID = oc.Aura.ID, Name = oc.Aura.Name, Keywords = oc.Aura.Keywords, Details = oc.Aura.Details };
            return dto;
        }

        #endregion

        #region Project Component Mapping
        private Encyclopedia MapToEncyclopedia(EncyclopediaDto dto)
        {
            var e = new Encyclopedia();
            if (dto.Entries != null) foreach (var entry in dto.Entries) e.Entries.Add(MapToEncyclopediaEntry(entry));
            return e;
        }

        private EncyclopediaDto MapToEncyclopediaDto(Encyclopedia e)
        {
            var dto = new EncyclopediaDto();
            foreach (var entry in e.Entries) dto.Entries.Add(MapToEncyclopediaEntryDto(entry));
            return dto;
        }

        private EncyclopediaEntry MapToEncyclopediaEntry(EncyclopediaEntryDto dto)
        {
            var entry = new EncyclopediaEntry { ID = dto.ID, Name = dto.Name, Category = dto.Category, Details = dto.Details, DMInfo = dto.DMInfo };
            if (dto.Images != null) foreach (var img in dto.Images) entry.Images.Add(new EncyclopediaImage { ID = img.ID, Name = img.Name, Image = ByteArrayToImage(img.ImageData) });
            return entry;
        }

        private EncyclopediaEntryDto MapToEncyclopediaEntryDto(EncyclopediaEntry entry)
        {
            var dto = new EncyclopediaEntryDto { ID = entry.ID, Name = entry.Name, Category = entry.Category, Details = entry.Details, DMInfo = entry.DMInfo };
            foreach (var img in entry.Images) dto.Images.Add(new EncyclopediaImageDto { ID = img.ID, Name = img.Name, ImageData = ImageToByteArray(img.Image) });
            return dto;
        }

        private Map MapToMap(MapDto dto)
        {
            var m = new Map { ID = dto.ID, Name = dto.Name, Category = dto.Category };
            if (dto.Tiles != null) foreach (var t in dto.Tiles) m.Tiles.Add(new TileData { ID = t.ID, TileID = t.TileID, Location = new Point(t.X, t.Y), Rotations = t.Rotations });
            if (dto.Areas != null) foreach (var a in dto.Areas) m.Areas.Add(new MapArea { ID = a.ID, Name = a.Name, Details = a.Details, Region = new Rectangle(a.X, a.Y, a.Width, a.Height) });
            return m;
        }

        private MapDto MapToMapDto(Map m)
        {
            var dto = new MapDto { ID = m.ID, Name = m.Name, Category = m.Category };
            foreach (var t in m.Tiles) dto.Tiles.Add(new MapTileDto { ID = t.ID, TileID = t.TileID, X = t.Location.X, Y = t.Location.Y, Rotations = t.Rotations });
            foreach (var a in m.Areas) dto.Areas.Add(new MapAreaDto { ID = a.ID, Name = a.Name, Details = a.Details, X = a.Region.X, Y = a.Region.Y, Width = a.Region.Width, Height = a.Region.Height });
            return dto;
        }

        private RegionalMap MapToRegionalMap(RegionalMapDto dto)
        {
            var rm = new RegionalMap { ID = dto.ID, Name = dto.Name, Image = ByteArrayToImage(dto.ImageData) };
            if (dto.Locations != null) foreach (var l in dto.Locations) rm.Locations.Add(new MapLocation { ID = l.ID, Name = l.Name, Category = l.Category, Point = new PointF(l.X, l.Y) });
            return rm;
        }

        private RegionalMapDto MapToRegionalMapDto(RegionalMap rm)
        {
            var dto = new RegionalMapDto { ID = rm.ID, Name = rm.Name, ImageData = ImageToByteArray(rm.Image) };
            foreach (var l in rm.Locations) dto.Locations.Add(new MapLocationDto { ID = l.ID, Name = l.Name, Category = l.Category, X = l.Point.X, Y = l.Point.Y });
            return dto;
        }

        private EncounterDeck MapToDeck(DeckDto dto)
        {
            var d = new EncounterDeck { ID = dto.ID, Name = dto.Name, Level = dto.Level };
            if (dto.Cards != null) foreach (var c in dto.Cards) d.Cards.Add(new EncounterCard { CreatureID = c.CreatureID, TemplateIDs = c.TemplateIDs, LevelAdjustment = c.LevelAdjustment, ThemeID = c.ThemeID });
            return d;
        }

        private DeckDto MapToDeckDto(EncounterDeck d)
        {
            var dto = new DeckDto { ID = d.ID, Name = d.Name, Level = d.Level };
            foreach (var c in d.Cards) dto.Cards.Add(new EncounterCardDto { CreatureID = c.CreatureID, TemplateIDs = c.TemplateIDs, LevelAdjustment = c.LevelAdjustment, ThemeID = c.ThemeID, Title = c.Title, XP = c.XP });
            return dto;
        }

        private NPC MapToNPC(NPCDto dto)
        {
            return new NPC { ID = dto.ID, TemplateID = dto.TemplateID };
        }

        private NPCDto MapToNPCDto(NPC npc)
        {
            return new NPCDto { ID = npc.ID, TemplateID = npc.TemplateID };
        }

        private Calendar MapToCalendar(CalendarDto dto)
        {
            return new Calendar { ID = dto.ID, Name = dto.Name, Details = dto.Details, CampaignYear = dto.CampaignYear };
        }

        private CalendarDto MapToCalendarDto(Calendar c)
        {
            return new CalendarDto { ID = c.ID, Name = c.Name, Details = c.Details, CampaignYear = c.CampaignYear };
        }

        private Attachment MapToAttachment(AttachmentDto dto)
        {
            return new Attachment { ID = dto.ID, Name = dto.Name, Contents = dto.Contents };
        }

        private AttachmentDto MapToAttachmentDto(Attachment a)
        {
            return new AttachmentDto { ID = a.ID, Name = a.Name, Contents = a.Contents };
        }

        private Background MapToBackground(BackgroundDto dto)
        {
            return new Background { ID = dto.ID, Title = dto.Title, Details = dto.Details };
        }

        private BackgroundDto MapToBackgroundDto(Background b)
        {
            return new BackgroundDto { ID = b.ID, Title = b.Title, Details = b.Details };
        }

        private Parcel MapToParcel(ParcelDto dto)
        {
            return new Parcel { Name = dto.Name, Details = dto.Details, Value = dto.Value, MagicItemID = dto.MagicItemID, ArtifactID = dto.ArtifactID, HeroID = dto.HeroID };
        }

        private ParcelDto MapToParcelDto(Parcel p)
        {
            return new ParcelDto { Name = p.Name, Details = p.Details, Value = p.Value, MagicItemID = p.MagicItemID, ArtifactID = p.ArtifactID, HeroID = p.HeroID };
        }

        private CampaignSettings MapToCampaignSettings(CampaignSettingsDto dto)
        {
            return new CampaignSettings { HP = dto.HP, XP = dto.XP, AttackBonus = dto.AttackBonus, Damage = dto.Damage, ACBonus = dto.ACBonus, NADBonus = dto.NADBonus };
        }

        private CampaignSettingsDto MapToCampaignSettingsDto(CampaignSettings cs)
        {
            return new CampaignSettingsDto { HP = cs.HP, XP = cs.XP, AttackBonus = cs.AttackBonus, Damage = cs.Damage, ACBonus = cs.ACBonus, NADBonus = cs.NADBonus };
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

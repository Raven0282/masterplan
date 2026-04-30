using System;
using System.Collections.Generic;
using MessagePack;

namespace Masterplan.Dto
{
    #region Root Containers
    [MessagePackObject]
    public partial class LibraryDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public bool ShowInAutoBuild { get; set; }
        [Key(3)] public List<CreatureDto> Creatures { get; set; } = new();
        [Key(4)] public List<TemplateDto> Templates { get; set; } = new();
        [Key(5)] public List<ThemeDto> Themes { get; set; } = new();
        [Key(6)] public List<TrapDto> Traps { get; set; } = new();
        [Key(7)] public List<SkillChallengeDto> SkillChallenges { get; set; } = new();
        [Key(8)] public List<MagicItemDto> MagicItems { get; set; } = new();
        [Key(9)] public List<ArtifactDto> Artifacts { get; set; } = new();
        [Key(10)] public List<TileDto> Tiles { get; set; } = new();
        [Key(11)] public List<TerrainPowerDto> TerrainPowers { get; set; } = new();
    }

    [MessagePackObject]
    public partial class ProjectDto
    {
        [Key(0)] public string Name { get; set; }
        [Key(1)] public string Author { get; set; }
        [Key(2)] public PartyDto Party { get; set; }
        [Key(3)] public List<HeroDto> Heroes { get; set; } = new();
        [Key(4)] public List<HeroDto> InactiveHeroes { get; set; } = new();
        [Key(5)] public PlotDto Plot { get; set; }
        [Key(6)] public EncyclopediaDto Encyclopedia { get; set; }
        [Key(7)] public List<NoteDto> Notes { get; set; } = new();
        [Key(8)] public List<MapDto> Maps { get; set; } = new();
        [Key(9)] public List<RegionalMapDto> RegionalMaps { get; set; } = new();
        [Key(10)] public List<DeckDto> Decks { get; set; } = new();
        [Key(11)] public List<NPCDto> NPCs { get; set; } = new();
        [Key(12)] public List<CreatureDto> CustomCreatures { get; set; } = new();
        [Key(13)] public List<CalendarDto> Calendars { get; set; } = new();
        [Key(14)] public List<AttachmentDto> Attachments { get; set; } = new();
        [Key(15)] public List<BackgroundDto> Backgrounds { get; set; } = new();
        [Key(16)] public List<ParcelDto> TreasureParcels { get; set; } = new();
        [Key(17)] public List<PlayerOptionDto> PlayerOptions { get; set; } = new();
        [Key(18)] public CampaignSettingsDto CampaignSettings { get; set; }
        [Key(19)] public string Password { get; set; }
        [Key(20)] public string PasswordHint { get; set; }
        [Key(21)] public LibraryDto Library { get; set; }
        [Key(22)] public List<CombatStateDto> SavedCombats { get; set; } = new();
        [Key(23)] public Dictionary<string, string> AddInData { get; set; } = new();
    }
    #endregion

    #region Combat State
    [MessagePackObject]
    public partial class CombatStateDto
    {
        [Key(0)] public DateTime Timestamp { get; set; }
        [Key(1)] public int PartyLevel { get; set; }
        [Key(2)] public EncounterDto Encounter { get; set; }
        [Key(3)] public int CurrentRound { get; set; }
        [Key(4)] public Dictionary<Guid, CombatDataDto> HeroData { get; set; } = new();
        [Key(5)] public Dictionary<Guid, CombatDataDto> TrapData { get; set; } = new();
        [Key(6)] public List<TokenLinkDto> TokenLinks { get; set; } = new();
        [Key(7)] public int RemovedCreatureXP { get; set; }
        [Key(8)] public Guid CurrentActor { get; set; }
        [Key(9)] public int ViewpointX { get; set; }
        [Key(10)] public int ViewpointY { get; set; }
        [Key(11)] public int ViewpointWidth { get; set; }
        [Key(12)] public int ViewpointHeight { get; set; }
        [Key(13)] public List<MapSketchDto> Sketches { get; set; } = new();
        [Key(14)] public List<OngoingConditionDto> QuickEffects { get; set; } = new();
        [Key(15)] public EncounterLogDto Log { get; set; }
    }

    [MessagePackObject]
    public partial class TokenLinkDto
    {
        [Key(0)] public string Text { get; set; }
        [Key(1)] public List<TokenDto> Tokens { get; set; } = new();
    }

    [MessagePackObject]
    public partial class TokenDto
    {
        [Key(0)] public string Type { get; set; } // "Creature", "Custom", or "Hero"
        [Key(1)] public Guid SlotID { get; set; } // For CreatureToken
        [Key(2)] public CombatDataDto Data { get; set; } // For CreatureToken or CustomToken
        [Key(3)] public CustomTokenDto CustomToken { get; set; } // For CustomToken
        [Key(4)] public Guid HeroID { get; set; } // For HeroToken
    }

    [MessagePackObject]
    public partial class MapSketchDto
    {
        [Key(0)] public int ARGB { get; set; }
        [Key(1)] public int Width { get; set; }
        [Key(2)] public List<MapSketchPointDto> Points { get; set; } = new();
    }

    [MessagePackObject]
    public partial class MapSketchPointDto
    {
        [Key(0)] public int SquareX { get; set; }
        [Key(1)] public int SquareY { get; set; }
        [Key(2)] public float LocationX { get; set; }
        [Key(3)] public float LocationY { get; set; }
    }

    [MessagePackObject]
    public partial class EncounterLogDto
    {
        [Key(0)] public List<LogEntryDto> Entries { get; set; } = new();
        [Key(1)] public bool Active { get; set; }
    }

    [MessagePackObject]
    public partial class LogEntryDto
    {
        [Key(0)] public string Type { get; set; }
        [Key(1)] public Guid CombatantID { get; set; }
        [Key(2)] public DateTime Timestamp { get; set; }
        [Key(3)] public byte[] Data { get; set; } // Serialized specific entry data
    }

    // Specific Log Entry Data DTOs
    [MessagePackObject]
    public partial class StartRoundEntryDto
    {
        [Key(0)] public int Round { get; set; }
    }

    [MessagePackObject]
    public partial class DamageEntryDto
    {
        [Key(0)] public int Amount { get; set; }
        [Key(1)] public List<string> Types { get; set; } = new();
    }

    [MessagePackObject]
    public partial class StateEntryDto
    {
        [Key(0)] public string State { get; set; }
    }

    [MessagePackObject]
    public partial class EffectEntryDto
    {
        [Key(0)] public string EffectText { get; set; }
        [Key(1)] public bool Added { get; set; }
    }

    [MessagePackObject]
    public partial class PowerEntryDto
    {
        [Key(0)] public string PowerName { get; set; }
        [Key(1)] public bool Added { get; set; }
    }

    [MessagePackObject]
    public partial class SkillEntryDto
    {
        [Key(0)] public string SkillName { get; set; }
    }

    [MessagePackObject]
    public partial class SkillChallengeLogEntryDto
    {
        [Key(0)] public bool Success { get; set; }
    }

    [MessagePackObject]
    public partial class MoveEntryDto
    {
        [Key(0)] public int Distance { get; set; }
        [Key(1)] public string Details { get; set; }
    }
    #endregion

    #region Creature & Common Components
    [MessagePackObject]
    public partial class CreatureDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string Details { get; set; }
        [Key(3)] public string Size { get; set; }
        [Key(4)] public string Origin { get; set; }
        [Key(5)] public string Type { get; set; }
        [Key(6)] public string Keywords { get; set; }
        [Key(7)] public int Level { get; set; }
        [Key(8)] public RoleDto Role { get; set; }
        [Key(9)] public string Senses { get; set; }
        [Key(10)] public string Movement { get; set; }
        [Key(11)] public string Alignment { get; set; }
        [Key(12)] public string Languages { get; set; }
        [Key(13)] public string Skills { get; set; }
        [Key(14)] public string Equipment { get; set; }
        [Key(15)] public string Category { get; set; }
        [Key(16)] public AbilityScoreDto Strength { get; set; }
        [Key(17)] public AbilityScoreDto Constitution { get; set; }
        [Key(18)] public AbilityScoreDto Dexterity { get; set; }
        [Key(19)] public AbilityScoreDto Intelligence { get; set; }
        [Key(20)] public AbilityScoreDto Wisdom { get; set; }
        [Key(21)] public AbilityScoreDto Charisma { get; set; }
        [Key(22)] public int HP { get; set; }
        [Key(23)] public int Initiative { get; set; }
        [Key(24)] public int AC { get; set; }
        [Key(25)] public int Fortitude { get; set; }
        [Key(26)] public int Reflex { get; set; }
        [Key(27)] public int Will { get; set; }
        [Key(28)] public RegenerationDto Regeneration { get; set; }
        [Key(29)] public List<AuraDto> Auras { get; set; } = new();
        [Key(30)] public List<CreaturePowerDto> Powers { get; set; } = new();
        [Key(31)] public List<DamageModifierDto> DamageModifiers { get; set; } = new();
        [Key(32)] public string Resist { get; set; }
        [Key(33)] public string Vulnerable { get; set; }
        [Key(34)] public string Immune { get; set; }
        [Key(35)] public string Tactics { get; set; }
        [Key(36)] public byte[] ImageData { get; set; }
        [Key(37)] public string Info { get; set; }
        [Key(38)] public string Phenotype { get; set; }
    }

    [MessagePackObject]
    public partial class AbilityScoreDto
    {
        [Key(0)] public int Score { get; set; }
    }

    [MessagePackObject]
    public partial class RoleDto
    {
        [Key(0)] public string Type { get; set; }
        [Key(1)] public string Flag { get; set; }
        [Key(2)] public bool Leader { get; set; }
        [Key(3)] public bool IsMinion { get; set; }
        [Key(4)] public bool MinionHasRole { get; set; }
    }

    [MessagePackObject]
    public partial class CreaturePowerDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public PowerActionDto Action { get; set; }
        [Key(3)] public string Keywords { get; set; }
        [Key(4)] public string Condition { get; set; }
        [Key(5)] public string Range { get; set; }
        [Key(6)] public PowerAttackDto Attack { get; set; }
        [Key(7)] public string Description { get; set; }
        [Key(8)] public string Details { get; set; }
        [Key(9)] public string Damage { get; set; }
        [Key(10)] public string Category { get; set; }
    }

    [MessagePackObject]
    public partial class PowerActionDto
    {
        [Key(0)] public string Action { get; set; }
        [Key(1)] public string Trigger { get; set; }
        [Key(2)] public string SustainAction { get; set; }
        [Key(3)] public string Use { get; set; }
        [Key(4)] public string Recharge { get; set; }
    }

    [MessagePackObject]
    public partial class PowerAttackDto
    {
        [Key(0)] public int Bonus { get; set; }
        [Key(1)] public string Defence { get; set; }
    }

    [MessagePackObject]
    public partial class AuraDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string Keywords { get; set; }
        [Key(3)] public string Details { get; set; }
    }

    [MessagePackObject]
    public partial class RegenerationDto
    {
        [Key(0)] public int Value { get; set; }
        [Key(1)] public string Details { get; set; }
    }

    [MessagePackObject]
    public partial class DamageModifierDto
    {
        [Key(0)] public string Type { get; set; }
        [Key(1)] public int Value { get; set; }
    }
    #endregion

    #region Traps & Hazards
    [MessagePackObject]
    public partial class TrapDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string Type { get; set; }
        [Key(3)] public int Level { get; set; }
        [Key(4)] public RoleDto Role { get; set; }
        [Key(5)] public string ReadAloud { get; set; }
        [Key(6)] public string Description { get; set; }
        [Key(7)] public string Details { get; set; }
        [Key(8)] public List<TrapSkillDto> Skills { get; set; } = new();
        [Key(9)] public int Initiative { get; set; }
        [Key(10)] public string Trigger { get; set; }
        [Key(11)] public List<TrapAttackDto> Attacks { get; set; } = new();
        [Key(12)] public string Countermeasures { get; set; }
        [Key(13)] public int XP { get; set; }
        [Key(14)] public string Info { get; set; }
    }

    [MessagePackObject]
    public partial class TrapAttackDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string Trigger { get; set; }
        [Key(3)] public string Action { get; set; }
        [Key(4)] public string Range { get; set; }
        [Key(5)] public string Keywords { get; set; }
        [Key(6)] public string Target { get; set; }
        [Key(7)] public bool HasInitiative { get; set; }
        [Key(8)] public int Initiative { get; set; }
        [Key(9)] public PowerAttackDto Attack { get; set; }
        [Key(10)] public string OnHit { get; set; }
        [Key(11)] public string OnMiss { get; set; }
        [Key(12)] public string Effect { get; set; }
        [Key(13)] public string Notes { get; set; }
    }

    [MessagePackObject]
    public partial class TrapSkillDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string SkillName { get; set; }
        [Key(2)] public int DC { get; set; }
        [Key(3)] public string Details { get; set; }
    }
    #endregion

    #region Plot & Encounter
    [MessagePackObject]
    public partial class PlotDto
    {
        [Key(0)] public List<PlotPointDto> Points { get; set; } = new();
    }

    [MessagePackObject]
    public partial class PlotPointDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string State { get; set; }
        [Key(3)] public string Colour { get; set; }
        [Key(4)] public string Details { get; set; }
        [Key(5)] public string ReadAloud { get; set; }
        [Key(6)] public List<Guid> Links { get; set; } = new();
        [Key(7)] public PlotDto Subplot { get; set; }
        [Key(8)] public ElementDto Element { get; set; }
        [Key(9)] public List<ParcelDto> Parcels { get; set; } = new();
        [Key(10)] public List<Guid> EncyclopediaEntryIDs { get; set; } = new();
    }

    [MessagePackObject]
    public partial class ElementDto
    {
        [Key(0)] public string Type { get; set; }
        [Key(1)] public byte[] Data { get; set; }
    }

    [MessagePackObject]
    public partial class EncounterDto
    {
        [Key(0)] public List<EncounterSlotDto> Slots { get; set; } = new();
        [Key(1)] public List<TrapDto> Traps { get; set; } = new();
        [Key(2)] public List<SkillChallengeDto> SkillChallenges { get; set; } = new();
        [Key(3)] public List<CustomTokenDto> CustomTokens { get; set; } = new();
        [Key(4)] public Guid MapID { get; set; }
        [Key(5)] public Guid MapAreaID { get; set; }
        [Key(6)] public List<EncounterNoteDto> Notes { get; set; } = new();
        [Key(7)] public List<EncounterWaveDto> Waves { get; set; } = new();
    }

    [MessagePackObject]
    public partial class EncounterSlotDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public EncounterCardDto Card { get; set; }
        [Key(2)] public string Type { get; set; }
        [Key(3)] public List<CombatDataDto> CombatData { get; set; } = new();
    }

    [MessagePackObject]
    public partial class CombatDataDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string DisplayName { get; set; }
        [Key(2)] public int X { get; set; }
        [Key(3)] public int Y { get; set; }
        [Key(4)] public bool Visible { get; set; }
        [Key(5)] public int Initiative { get; set; }
        [Key(6)] public bool Delaying { get; set; }
        [Key(7)] public int Damage { get; set; }
        [Key(8)] public int TempHP { get; set; }
        [Key(9)] public int Altitude { get; set; }
        [Key(10)] public List<Guid> UsedPowers { get; set; } = new();
        [Key(11)] public List<OngoingConditionDto> Conditions { get; set; } = new();
    }

    [MessagePackObject]
    public partial class OngoingConditionDto
    {
        [Key(0)] public string Type { get; set; }
        [Key(1)] public string Data { get; set; }
        [Key(2)] public string DamageType { get; set; }
        [Key(3)] public int Value { get; set; }
        [Key(4)] public int DefenceMod { get; set; }
        [Key(5)] public List<string> Defences { get; set; } = new();
        [Key(6)] public RegenerationDto Regeneration { get; set; }
        [Key(7)] public DamageModifierDto DamageModifier { get; set; }
        [Key(8)] public AuraDto Aura { get; set; }
        [Key(9)] public string Duration { get; set; }
        [Key(10)] public Guid DurationCreatureID { get; set; }
        [Key(11)] public int DurationRound { get; set; }
        [Key(12)] public int SavingThrowModifier { get; set; }
    }

    [MessagePackObject]
    public partial class EncounterNoteDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Title { get; set; }
        [Key(2)] public string Contents { get; set; }
    }

    [MessagePackObject]
    public partial class EncounterWaveDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public bool Active { get; set; }
        [Key(3)] public List<EncounterSlotDto> Slots { get; set; } = new();
    }

    [MessagePackObject]
    public partial class CustomTokenDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Type { get; set; } // CustomTokenType
        [Key(2)] public string Name { get; set; }
        [Key(3)] public string Details { get; set; }
        [Key(4)] public string TokenSize { get; set; } // CreatureSize
        [Key(5)] public int OverlaySizeWidth { get; set; }
        [Key(6)] public int OverlaySizeHeight { get; set; }
        [Key(7)] public string OverlayStyle { get; set; } // OverlayStyle
        [Key(8)] public int ARGB { get; set; }
        [Key(9)] public byte[] ImageData { get; set; }
        [Key(10)] public bool DifficultTerrain { get; set; }
        [Key(11)] public bool Opaque { get; set; }
        [Key(12)] public CombatDataDto Data { get; set; }
        [Key(13)] public TerrainPowerDto TerrainPower { get; set; }
        [Key(14)] public Guid CreatureID { get; set; }
    }

    [MessagePackObject]
    public partial class QuestDto
    {
        [Key(0)] public int Level { get; set; }
        [Key(1)] public string Type { get; set; }
        [Key(2)] public int XP { get; set; }
    }

    [MessagePackObject]
    public partial class MapElementDto
    {
        [Key(0)] public Guid MapID { get; set; }
        [Key(1)] public Guid MapAreaID { get; set; }
    }
    #endregion

    #region Skill Challenges
    [MessagePackObject]
    public partial class SkillChallengeDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public int Level { get; set; }
        [Key(3)] public int Complexity { get; set; }
        [Key(4)] public List<SkillChallengeDataDto> Skills { get; set; } = new();
        [Key(5)] public string Success { get; set; }
        [Key(6)] public string Failure { get; set; }
        [Key(7)] public string Notes { get; set; }
        [Key(8)] public Guid MapID { get; set; }
        [Key(9)] public Guid MapAreaID { get; set; }
        [Key(10)] public int Successes { get; set; }
        [Key(11)] public string Info { get; set; }
    }

    [MessagePackObject]
    public partial class SkillChallengeDataDto
    {
        [Key(0)] public string SkillName { get; set; }
        [Key(1)] public string Difficulty { get; set; }
        [Key(2)] public int DCModifier { get; set; }
        [Key(3)] public string Details { get; set; }
        [Key(4)] public string Success { get; set; }
        [Key(5)] public string Failure { get; set; }
    }
    #endregion

    #region Items, Tiles & Themes
    [MessagePackObject]
    public partial class MagicItemDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string Type { get; set; }
        [Key(3)] public string Rarity { get; set; }
        [Key(4)] public int Level { get; set; }
        [Key(5)] public string Description { get; set; }
        [Key(6)] public List<SectionDto> Sections { get; set; } = new();
        [Key(7)] public string Info { get; set; }
    }

    [MessagePackObject]
    public partial class ArtifactDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string Tier { get; set; }
        [Key(3)] public string Description { get; set; }
        [Key(4)] public string Details { get; set; }
        [Key(5)] public string Goals { get; set; }
        [Key(6)] public string RoleplayingTips { get; set; }
        [Key(7)] public List<SectionDto> Sections { get; set; } = new();
        [Key(8)] public List<ArtifactConcordanceDto> ConcordanceLevels { get; set; } = new();
        [Key(9)] public List<PairDto<string, string>> ConcordanceRules { get; set; } = new();
    }

    [MessagePackObject]
    public partial class PairDto<T1, T2>
    {
        [Key(0)] public T1 First { get; set; }
        [Key(1)] public T2 Second { get; set; }
    }

    [MessagePackObject]
    public partial class ArtifactConcordanceDto
    {
        [Key(0)] public string Name { get; set; }
        [Key(1)] public string ValueRange { get; set; }
        [Key(2)] public string Quote { get; set; }
        [Key(3)] public string Description { get; set; }
        [Key(4)] public List<SectionDto> Sections { get; set; } = new();
    }

    [MessagePackObject]
    public partial class TileDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Category { get; set; }
        [Key(2)] public int Width { get; set; }
        [Key(3)] public int Height { get; set; }
        [Key(4)] public byte[] ImageData { get; set; }
        [Key(5)] public string Keywords { get; set; }
        [Key(6)] public int ARGB { get; set; }
    }

    [MessagePackObject]
    public partial class TerrainPowerDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string Type { get; set; }
        [Key(3)] public string FlavourText { get; set; }
        [Key(4)] public string Action { get; set; }
        [Key(5)] public string Requirement { get; set; }
        [Key(6)] public string Check { get; set; }
        [Key(7)] public string Success { get; set; }
        [Key(8)] public string Failure { get; set; }
        [Key(9)] public string Target { get; set; }
        [Key(10)] public string Attack { get; set; }
        [Key(11)] public string Hit { get; set; }
        [Key(12)] public string Miss { get; set; }
        [Key(13)] public string Effect { get; set; }
    }

    [MessagePackObject]
    public partial class ThemeDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public List<ThemePowerDataDto> Powers { get; set; } = new();
        [Key(3)] public List<PairDto<string, int>> SkillBonuses { get; set; } = new();
    }

    [MessagePackObject]
    public partial class ThemePowerDataDto
    {
        [Key(0)] public CreaturePowerDto Power { get; set; }
        [Key(1)] public string Type { get; set; }
        [Key(2)] public List<string> Roles { get; set; } = new();
    }

    [MessagePackObject]
    public partial class TemplateDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string Type { get; set; }
        [Key(3)] public string Role { get; set; }
        [Key(4)] public bool Leader { get; set; }
        [Key(5)] public string Senses { get; set; }
        [Key(6)] public string Movement { get; set; }
        [Key(7)] public int HP { get; set; }
        [Key(8)] public int Initiative { get; set; }
        [Key(9)] public int AC { get; set; }
        [Key(10)] public int Fortitude { get; set; }
        [Key(11)] public int Reflex { get; set; }
        [Key(12)] public int Will { get; set; }
        [Key(13)] public RegenerationDto Regeneration { get; set; }
        [Key(14)] public List<AuraDto> Auras { get; set; } = new();
        [Key(15)] public List<CreaturePowerDto> Powers { get; set; } = new();
        [Key(16)] public List<DamageModifierTemplateDto> DamageModifierTemplates { get; set; } = new();
        [Key(17)] public string Resist { get; set; }
        [Key(18)] public string Vulnerable { get; set; }
        [Key(19)] public string Immune { get; set; }
        [Key(20)] public string Tactics { get; set; }
    }

    [MessagePackObject]
    public partial class DamageModifierTemplateDto
    {
        [Key(0)] public string Type { get; set; }
        [Key(1)] public int HeroicValue { get; set; }
        [Key(2)] public int ParagonValue { get; set; }
        [Key(3)] public int EpicValue { get; set; }
    }

    [MessagePackObject]
    public partial class SectionDto
    {
        [Key(0)] public string Header { get; set; }
        [Key(1)] public string Details { get; set; }
    }
    #endregion

    #region Project Sub-Models
    [MessagePackObject]
    public partial class PartyDto
    {
        [Key(0)] public int Size { get; set; }
        [Key(1)] public int XP { get; set; }
        [Key(2)] public int Level { get; set; }
    }

    [MessagePackObject]
    public partial class HeroDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string Player { get; set; }
        [Key(3)] public string Size { get; set; }
        [Key(4)] public string Race { get; set; }
        [Key(5)] public int Level { get; set; }
        [Key(6)] public string Class { get; set; }
        [Key(7)] public string ParagonPath { get; set; }
        [Key(8)] public string EpicDestiny { get; set; }
        [Key(9)] public string PowerSource { get; set; }
        [Key(10)] public string Role { get; set; }
        [Key(11)] public int HP { get; set; }
        [Key(12)] public int AC { get; set; }
        [Key(13)] public int Fortitude { get; set; }
        [Key(14)] public int Reflex { get; set; }
        [Key(15)] public int Will { get; set; }
        [Key(16)] public int InitBonus { get; set; }
        [Key(17)] public int PassivePerception { get; set; }
        [Key(18)] public int PassiveInsight { get; set; }
        [Key(19)] public string Languages { get; set; }
        [Key(20)] public byte[] PortraitData { get; set; }
        [Key(21)] public string Info { get; set; }
        [Key(22)] public List<CustomTokenDto> Tokens { get; set; } = new();
    }

    [MessagePackObject]
    public partial class EncyclopediaDto
    {
        [Key(0)] public List<EncyclopediaEntryDto> Entries { get; set; } = new();
    }

    [MessagePackObject]
    public partial class EncyclopediaEntryDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string Category { get; set; }
        [Key(3)] public string Details { get; set; }
        [Key(4)] public string DMInfo { get; set; }
        [Key(5)] public List<EncyclopediaImageDto> Images { get; set; } = new();
    }

    [MessagePackObject]
    public partial class EncyclopediaImageDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public byte[] ImageData { get; set; }
    }

    [MessagePackObject]
    public partial class MapDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string Category { get; set; }
        [Key(3)] public List<MapTileDto> Tiles { get; set; } = new();
        [Key(4)] public List<MapAreaDto> Areas { get; set; } = new();
    }

    [MessagePackObject]
    public partial class MapTileDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public Guid TileID { get; set; }
        [Key(2)] public int X { get; set; }
        [Key(3)] public int Y { get; set; }
        [Key(4)] public int Rotations { get; set; }
    }

    [MessagePackObject]
    public partial class MapAreaDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string Details { get; set; }
        [Key(3)] public int X { get; set; }
        [Key(4)] public int Y { get; set; }
        [Key(5)] public int Width { get; set; }
        [Key(6)] public int Height { get; set; }
    }

    [MessagePackObject]
    public partial class RegionalMapDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public byte[] ImageData { get; set; }
        [Key(3)] public List<MapLocationDto> Locations { get; set; } = new();
    }

    [MessagePackObject]
    public partial class MapLocationDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string Category { get; set; }
        [Key(3)] public float X { get; set; }
        [Key(4)] public float Y { get; set; }
    }

    [MessagePackObject]
    public partial class DeckDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public int Level { get; set; }
        [Key(3)] public List<EncounterCardDto> Cards { get; set; } = new();
    }

    [MessagePackObject]
    public partial class EncounterCardDto
    {
        [Key(0)] public Guid CreatureID { get; set; }
        [Key(1)] public List<Guid> TemplateIDs { get; set; } = new();
        [Key(2)] public int LevelAdjustment { get; set; }
        [Key(3)] public Guid ThemeID { get; set; }
        [Key(4)] public string Title { get; set; }
        [Key(5)] public int XP { get; set; }
        [Key(6)] public Guid ThemeAttackPowerID { get; set; }
        [Key(7)] public Guid ThemeUtilityPowerID { get; set; }
        [Key(8)] public bool Drawn { get; set; }
    }

    [MessagePackObject]
    public partial class NPCDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public Guid TemplateID { get; set; }
    }

    [MessagePackObject]
    public partial class CalendarDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string Details { get; set; }
        [Key(3)] public int CampaignYear { get; set; }
    }

    [MessagePackObject]
    public partial class AttachmentDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public byte[] Contents { get; set; }
    }

    [MessagePackObject]
    public partial class BackgroundDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Title { get; set; }
        [Key(2)] public string Details { get; set; }
    }

    [MessagePackObject]
    public partial class ParcelDto
    {
        [Key(0)] public string Name { get; set; }
        [Key(1)] public string Details { get; set; }
        [Key(2)] public int Value { get; set; }
        [Key(3)] public Guid MagicItemID { get; set; }
        [Key(4)] public Guid ArtifactID { get; set; }
        [Key(5)] public Guid HeroID { get; set; }
    }

    [MessagePackObject]
    public partial class PlayerOptionDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
    }

    [MessagePackObject]
    public partial class NoteDto
    {
        [Key(0)] public Guid ID { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public string Content { get; set; }
        [Key(3)] public string Category { get; set; }
    }

    [MessagePackObject]
    public partial class CampaignSettingsDto
    {
        [Key(0)] public double HP { get; set; }
        [Key(1)] public double XP { get; set; }
        [Key(2)] public int AttackBonus { get; set; }
        [Key(3)] public double Damage { get; set; }
        [Key(4)] public int ACBonus { get; set; }
        [Key(5)] public int NADBonus { get; set; }
    }
    #endregion
}

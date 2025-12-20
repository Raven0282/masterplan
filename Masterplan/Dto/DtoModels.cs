using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Masterplan.Dto
{
    [XmlRoot("Library")]
    [XmlType("Library")]
    public class LibraryDto
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public bool ShowInAutoBuild { get; set; }

        [XmlArray("Creatures")]
        [XmlArrayItem("Creature")]
        public List<CreatureDto> Creatures { get; set; } = new List<CreatureDto>();

        [XmlArray("Traps")]
        [XmlArrayItem("Trap")]
        public List<TrapDto> Traps { get; set; } = new List<TrapDto>();

        [XmlArray("SkillChallenges")]
        [XmlArrayItem("SkillChallenge")]
        public List<SkillChallengeDto> SkillChallenges { get; set; } = new List<SkillChallengeDto>();

        [XmlArray("MagicItems")]
        [XmlArrayItem("MagicItem")]
        public List<MagicItemDto> MagicItems { get; set; } = new List<MagicItemDto>();

        [XmlArray("Artifacts")]
        [XmlArrayItem("Artifact")]
        public List<ArtifactDto> Artifacts { get; set; } = new List<ArtifactDto>();

        [XmlArray("Tiles")]
        [XmlArrayItem("Tile")]
        public List<TileDto> Tiles { get; set; } = new List<TileDto>();

        [XmlArray("Templates")]
        [XmlArrayItem("Template")]
        public List<TemplateDto> Templates { get; set; } = new List<TemplateDto>();

        [XmlArray("Themes")]
        [XmlArrayItem("Theme")]
        public List<ThemeDto> Themes { get; set; } = new List<ThemeDto>();

        [XmlArray("TerrainPowers")]
        [XmlArrayItem("TerrainPower")]
        public List<TerrainPowerDto> TerrainPowers { get; set; } = new List<TerrainPowerDto>();
    }

    #region Creature Components
    [XmlType("Creature")]
    public class CreatureDto
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public string Size { get; set; }
        public string Origin { get; set; }
        public string Type { get; set; }
        public string Keywords { get; set; }
        public int Level { get; set; }
        public string Role { get; set; }
        public string Senses { get; set; }
        public string Movement { get; set; }
        public string Alignment { get; set; }
        public string Languages { get; set; }
        public string Skills { get; set; }
        public string Equipment { get; set; }
        public string Category { get; set; }
        public int HP { get; set; }
        public int Initiative { get; set; }
        public int AC { get; set; }
        public int Fortitude { get; set; }
        public int Reflex { get; set; }
        public int Will { get; set; }
    }
    #endregion

    #region Trap Components
    [XmlType("Trap")]
    public class TrapDto
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Role { get; set; }
        public int Level { get; set; }
        public string Description { get; set; }
        public string Trigger { get; set; }
        public string Info { get; set; }
        public int XP { get; set; }
    }
    #endregion

    #region Item & Artifact Components
    [XmlType("MagicItem")]
    public class MagicItemDto
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Rarity { get; set; }
        public int Level { get; set; }
        public string Description { get; set; }
        [XmlArray("Sections")]
        [XmlArrayItem("Section")]
        public List<SectionDto> Sections { get; set; } = new List<SectionDto>();
    }

    [XmlType("Artifact")]
    public class ArtifactDto
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Tier { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public string Goals { get; set; }
        public string RoleplayingTips { get; set; }
        [XmlArray("ConcordanceLevels")]
        [XmlArrayItem("ConcordanceLevel")]
        public List<ArtifactConcordanceDto> ConcordanceLevels { get; set; } = new List<ArtifactConcordanceDto>();
    }

    [XmlType("ArtifactConcordance")]
    public class ArtifactConcordanceDto
    {
        public string Name { get; set; }
        public string ValueRange { get; set; }
        public string Quote { get; set; }
        public string Description { get; set; }
        [XmlArray("Sections")]
        [XmlArrayItem("Section")]
        public List<SectionDto> Sections { get; set; } = new List<SectionDto>();
    }
    #endregion

    #region Skill Challenge Components
    [XmlType("SkillChallenge")]
    public class SkillChallengeDto
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Complexity { get; set; }
        public string SuccessCondition { get; set; }
        public string FailureCondition { get; set; }
        [XmlArray("Skills")]
        [XmlArrayItem("Skill")]
        public List<SkillChallengeDataDto> Skills { get; set; } = new List<SkillChallengeDataDto>();
    }

    [XmlType("SkillChallengeData")]
    public class SkillChallengeDataDto { public string SkillName { get; set; } public string Difficulty { get; set; } public int DCModifier { get; set; } public string Details { get; set; } }
    #endregion

    #region Terrain & Tiles
    [XmlType("TerrainPower")]
    public class TerrainPowerDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string FlavourText { get; set; }
        public string Requirement { get; set; }
        public string Check { get; set; }
        public string Success { get; set; }
        public string Failure { get; set; }
        public string Target { get; set; }
    }

    [XmlType("Tile")]
    public class TileDto { public Guid ID { get; set; } public string Category { get; set; } public string Size { get; set; } public string Keywords { get; set; } }
    #endregion

    [XmlType("Section")] public class SectionDto { public string Header { get; set; } public string Details { get; set; } }
    [XmlType("Template")] public class TemplateDto { public string Name { get; set; } }
    [XmlType("Theme")] public class ThemeDto { public string Name { get; set; } }
}
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

        public List<CreatureDto> Creatures { get; set; } = new List<CreatureDto>();
        public List<TrapDto> Traps { get; set; } = new List<TrapDto>();
        public List<SkillChallengeDto> SkillChallenges { get; set; } = new List<SkillChallengeDto>();
        public List<MagicItemDto> MagicItems { get; set; } = new List<MagicItemDto>();
        public List<ArtifactDto> Artifacts { get; set; } = new List<ArtifactDto>();
        public List<TileDto> Tiles { get; set; } = new List<TileDto>();
        public List<TerrainPowerDto> TerrainPowers { get; set; } = new List<TerrainPowerDto>();
        public List<ThemeDto> Themes { get; set; } = new List<ThemeDto>();
        public List<TemplateDto> Templates { get; set; } = new List<TemplateDto>(); // Added for CS1061
        public List<EncyclopediaEntryDto> Encyclopedia { get; set; } = new List<EncyclopediaEntryDto>();
    }

    #region Skill Challenges
    [XmlType("SkillChallenge")]
    public class SkillChallengeDto
    {
        public Guid ID { get; set; } // Added for CS0117
        public string Name { get; set; }
        public int Level { get; set; }
        public int Complexity { get; set; }
        public string SuccessCondition { get; set; } // Added for CS0117
        public string FailureCondition { get; set; } // Added for CS0117

        [XmlArray("Skills")]
        [XmlArrayItem("Skill")]
        public List<SkillChallengeDataDto> Skills { get; set; } = new List<SkillChallengeDataDto>(); // Added for CS1061
    }

    [XmlType("SkillChallengeData")]
    public class SkillChallengeDataDto // Added for CS0246
    {
        public string SkillName { get; set; }
        public string Difficulty { get; set; }
        public int DCModifier { get; set; }
        public string Details { get; set; }
    }
    #endregion

    #region Artifacts
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
        [XmlArrayItem("Level")]
        public List<ArtifactConcordanceDto> ConcordanceLevels { get; set; } = new List<ArtifactConcordanceDto>(); // Added for CS1061
    }

    [XmlType("ArtifactConcordance")]
    public class ArtifactConcordanceDto // Added for CS0246
    {
        public string Name { get; set; }
        public string ValueRange { get; set; }
        public string Quote { get; set; }
        public string Description { get; set; }
        public List<SectionDto> Sections { get; set; } = new List<SectionDto>();
    }
    #endregion

    #region Creatures & Basic Elements
    [XmlType("Creature")]
    public class CreatureDto
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public int Level { get; set; }
        public int HP { get; set; }
        public string Size { get; set; }
        public string Origin { get; set; }
        public string Type { get; set; }
        public string Keywords { get; set; }
        public string Role { get; set; }
        public string Senses { get; set; }
        public string Movement { get; set; }
        public string Alignment { get; set; }
        public string Languages { get; set; }
        public string Skills { get; set; }
        public string Equipment { get; set; }
        public string Category { get; set; }
        public int Initiative { get; set; }
        public int AC { get; set; }
        public int Fortitude { get; set; }
        public int Reflex { get; set; }
        public int Will { get; set; }
    }

    [XmlType("Trap")]
    public class TrapDto
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public string Type { get; set; }
        public string Role { get; set; }
        public string Description { get; set; }
        public string Trigger { get; set; }
        public string Info { get; set; }
        public int XP { get; set; }
    }

    [XmlType("MagicItem")]
    public class MagicItemDto
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public string Type { get; set; }
        public string Rarity { get; set; }
        public string Description { get; set; }
        public List<SectionDto> Sections { get; set; } = new List<SectionDto>();
    }

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

    [XmlType("Tile")] public class TileDto { public Guid ID { get; set; } public string Category { get; set; } public string Size { get; set; } public string Keywords { get; set; } }
    [XmlType("Theme")] public class ThemeDto { public string Name { get; set; } }
    [XmlType("Template")] public class TemplateDto { public string Name { get; set; } }
    [XmlType("Section")] public class SectionDto { public string Header { get; set; } public string Details { get; set; } }
    [XmlType("EncyclopediaEntry")] public class EncyclopediaEntryDto { public Guid ID { get; set; } public string Name { get; set; } public string Category { get; set; } public string Details { get; set; } }
    #endregion
}
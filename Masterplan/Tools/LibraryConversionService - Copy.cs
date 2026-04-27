using Masterplan.Data;
using Masterplan.Dto;
using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace Masterplan.Tools
{
    /// <summary>
    /// A standalone service within the Legacy Masterplan project to convert 
    /// .library (BinaryFormatter) files into .mpxl (MessagePack) files.
    /// This bridges the gap between .NET 8 and the future .NET 10 environment.
    /// </summary>
    public static class LibraryConversionService
    {
        private static readonly MessagePackSerializerOptions _options = MessagePackSerializerOptions.Standard
            .WithResolver(MessagePack.Resolvers.StandardResolver.Instance);

        /// <summary>
        /// Orchestrates the conversion of a legacy library file to the modern MPX format.
        /// </summary>
        /// <param name="legacyPath">Source .library file path.</param>
        /// <param name="outputPath">Destination .mpxl file path.</param>
        public static void ConvertToMpxl(string legacyPath, string outputPath)
        {
            if (!File.Exists(legacyPath))
                throw new FileNotFoundException("Legacy library file not found.", legacyPath);

            // 1. Ingest legacy data using BinaryFormatter
            Library legacyLibrary = LoadLegacyBinary(legacyPath);

            // 2. Map legacy object graph to modern DTOs
            LibraryDto modernLibrary = MapLibrary(legacyLibrary);

            // 3. Serialize to MessagePack
            byte[] bytes = MessagePackSerializer.Serialize(modernLibrary, _options);
            File.WriteAllBytes(outputPath, bytes);
        }

        private static LibraryDto MapLibrary(Library legacy)
        {
            var dto = new LibraryDto
            {
                ID = legacy.ID,
                Name = legacy.Name ?? "Imported Library",
                ShowInAutoBuild = true // Default for modern UI
            };

            // Map Creatures (Monsters)
            if (legacy.Creatures != null)
                dto.Creatures = legacy.Creatures.Select(MapCreature).ToList();

            // Map Map Tiles (This is critical for the Mapping Module straddling)
            if (legacy.Tiles != null)
                dto.Tiles = legacy.Tiles.Select(MapTile).ToList();

            // Map Templates and Themes
            if (legacy.Templates != null)
                dto.Templates = legacy.Templates.Select(t => new TemplateDto { Name = t.Name }).ToList();

            if (legacy.Themes != null)
                dto.Themes = legacy.Themes.Select(t => new ThemeDto { Name = t.Name }).ToList();

            return dto;
        }

        private static CreatureDto MapCreature(Creature legacy)
        {
            return new CreatureDto
            {
                ID = legacy.ID,
                Name = legacy.Name,
                Level = legacy.Level,
                Role = legacy.Role.ToString(),
                HP = legacy.HP,
                Initiative = legacy.Initiative,
                AC = legacy.AC,
                Fortitude = legacy.Fortitude,
                Reflex = legacy.Reflex,
                Will = legacy.Will,
                Strength = legacy.Strength.Value,
                Constitution = legacy.Constitution.Value,
                Dexterity = legacy.Dexterity.Value,
                Intelligence = legacy.Intelligence.Value,
                Wisdom = legacy.Wisdom.Value,
                Charisma = legacy.Charisma.Value,

                // Convert Portrait to cross-platform byte array
                ImageData = ImageToByteArray(legacy.Image),

                // Map Powers
                Powers = legacy.Powers?.Select(p => new PowerDto
                {
                    ID = p.ID,
                    Name = p.Name,
                    Action = p.Action?.ToString() ?? "None",
                    Usage = p.Usage.ToString(),
                    Description = p.Description,
                    Details = p.Details
                }).ToList() ?? new List<PowerDto>()
            };
        }

        private static TileDto MapTile(Tile legacy)
        {
            return new TileDto
            {
                ID = legacy.ID,
                Category = legacy.Category.ToString(),
                Size = $"{legacy.Size.Width}x{legacy.Size.Height}",
                // Convert Map Tile graphics to byte array
                ImageData = ImageToByteArray(legacy.Image)
            };
        }

        /// <summary>
        /// Converts GDI+ System.Drawing.Image to a portable byte array (PNG).
        /// This allows the legacy app to load it via MemoryStream and the 
        /// modern app to load it via Avalonia.Media.Imaging.
        /// </summary>
        private static byte[] ImageToByteArray(Image image)
        {
            if (image == null) return null;

            using (var ms = new MemoryStream())
            {
                // We use PNG to ensure transparency is preserved for tokens and map tiles
                image.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private static Library LoadLegacyBinary(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                // Note: The project must have <EnableUnsafeBinaryFormatterSerialization>true</EnableUnsafeBinaryFormatterSerialization>
                // set in the .csproj for .NET 8 to allow this.
#pragma warning disable SYSLIB0011
                BinaryFormatter formatter = new BinaryFormatter();
                return (Library)formatter.Deserialize(fs);
#pragma warning restore SYSLIB0011
            }
        }
    }
}
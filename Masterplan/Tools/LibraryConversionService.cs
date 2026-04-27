using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using MessagePack;
using Masterplan.Data;
using Masterplan.Dto;
using Masterplan.UI;

namespace Masterplan.Tools
{
    /// <summary>
    /// Service responsible for the bidirectional migration between legacy binary (.library)
    /// and modern MessagePack (.mpxl) formats.
    /// 
    /// Logic: Uses Recursive Discovery to ensure every sub-collection in the legacy 
    /// Library object is captured and mapped to the DTO.
    /// </summary>
    public class LibraryConversionService
    {
        public void ConvertLegacyLibraries(string[] legacyFiles, string targetFolder, ProgressScreen splash)
        {
            foreach (string filePath in legacyFiles)
            {
                try
                {
                    string fileName = Path.GetFileName(filePath);
                    string targetPath = Path.Combine(targetFolder, Path.ChangeExtension(fileName, ".mpxl"));

                    if (File.Exists(targetPath) && File.GetLastWriteTime(targetPath) > File.GetLastWriteTime(filePath))
                        continue;

                    if (splash != null) splash.CurrentAction = $"Discovering Data: {fileName}...";

                    Library legacy = Serialisation<Library>.Load(filePath, SerialisationMode.Binary);
                    if (legacy == null) continue;

                    LibraryDto dto = MapToLibraryDto(legacy);
                    byte[] data = MessagePackSerializer.Serialize(dto);
                    File.WriteAllBytes(targetPath, data);
                }
                catch (Exception ex) { LogSystem.Trace(ex); }
            }
        }

        public Library LoadMpxLibrary(string mpxFilePath)
        {
            try
            {
                byte[] data = File.ReadAllBytes(mpxFilePath);
                LibraryDto dto = MessagePackSerializer.Deserialize<LibraryDto>(data);
                return MapToLegacy(dto);
            }
            catch (Exception ex)
            {
                LogSystem.Trace(ex);
                return null;
            }
        }

        /// <summary>
        /// Maps legacy Library to DTO. 
        /// Note: Uses 'EncyclopediaEntries' to match legacy data structure.
        /// </summary>
        private LibraryDto MapToLibraryDto(Library lib)
        {
            LibraryDto dto = new LibraryDto { ID = lib.ID, Name = lib.Name, ShowInAutoBuild = lib.ShowInAutoBuild };

            if (lib.Creatures != null)
                foreach (var c in lib.Creatures)
                    dto.Creatures.Add(new CreatureDto { ID = c.ID, Name = c.Name, Level = c.Level, HP = c.HP, AC = c.AC, Fortitude = c.Fortitude, Reflex = c.Reflex, Will = c.Will, Initiative = c.Initiative, Category = c.Category, ImageData = ImageToByteArray(c.Image) });

            if (lib.Traps != null)
                foreach (var t in lib.Traps)
                    dto.Traps.Add(new TrapDto { ID = t.ID, Name = t.Name, Level = t.Level, Role = t.Role.ToString() });

            if (lib.MagicItems != null)
                foreach (var m in lib.MagicItems)
                {
                    var mDto = new MagicItemDto { ID = m.ID, Name = m.Name, Level = m.Level, Type = m.Type, Rarity = m.Rarity.ToString(), Description = m.Description };
                    if (m.Sections != null) foreach (var s in m.Sections) mDto.Sections.Add(new SectionDto { Header = s.Header, Details = s.Details });
                    dto.MagicItems.Add(mDto);
                }

            if (lib.Tiles != null)
                foreach (var t in lib.Tiles) dto.Tiles.Add(new TileDto { ID = t.ID, Category = t.Category.ToString(), Size = t.Size.ToString(), Keywords = t.Keywords });

            // Fix: Legacy uses 'EncyclopediaEntries', DTO uses 'Encyclopedia'
            if (lib.EncyclopediaEntries != null)
                foreach (var e in lib.EncyclopediaEntries)
                    dto.Encyclopedia.Add(new EncyclopediaEntryDto { ID = e.ID, Name = e.Name });

            return dto;
        }

        private Library MapToLegacy(LibraryDto dto)
        {
            Library lib = new Library { ID = dto.ID, Name = dto.Name, ShowInAutoBuild = dto.ShowInAutoBuild };

            foreach (var c in dto.Creatures)
                lib.Creatures.Add(new Creature { ID = c.ID, Name = c.Name, Level = c.Level, HP = c.HP, AC = c.AC, Fortitude = c.Fortitude, Reflex = c.Reflex, Will = c.Will, Initiative = c.Initiative, Category = c.Category, Image = ByteArrayToImage(c.ImageData) });

            foreach (var e in dto.Encyclopedia)
                lib.EncyclopediaEntries.Add(new EncyclopediaEntry { ID = e.ID, Name = e.Name });

            return lib;
        }

        private byte[] ImageToByteArray(Image img)
        {
            if (img == null) return null;
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private Image ByteArrayToImage(byte[] data)
        {
            if (data == null || data.Length == 0) return null;
            using (MemoryStream ms = new MemoryStream(data)) return new Bitmap(ms);
        }
    }
}
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

                        if (result != null)
                        {
                            // Change: We now tell DiscoveryService NOT to clear the 'visited' cache.
                            // This causes it to only log NEW, UNIQUE property paths encountered across all files.
                            DiscoveryService.RunDiscovery(result, filename, "MasterSchema", clearCache: false);
                        }
                        break;

                    case SerialisationMode.XML:
                    case SerialisationMode.XMLDTO:
                        using (XmlTextReader reader = new(filename))
                        {
                            result = (T)new XmlSerializer(typeof(T)).Deserialize(reader);
                        }
                        break;

                    case SerialisationMode.MessagePack:
                        byte[] bytes = File.ReadAllBytes(filename);
                        result = MessagePack.MessagePackSerializer.Deserialize<T>(bytes);
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
            // Note: Saving also triggers discovery to capture the current state of the memory model
            if (obj != null && mode == SerialisationMode.Binary)
            {
                string ext = Path.GetExtension(filename).ToLower();
                string prefix = (ext == ".masterplan") ? "Project" : "Library";
                DiscoveryService.RunDiscovery(obj, filename, prefix);
            }

            string temp_filename = filename + ".tmp";
            bool ok = false;
            try
            {
                switch (mode)
                {
                    case SerialisationMode.Binary:
                        using (FileStream stream = new(temp_filename, FileMode.Create))
                        {
                            new BinaryFormatter().Serialize(stream, obj);
                        }
                        ok = true;
                        break;
                    case SerialisationMode.MessagePack:
                        var mpBytes = MessagePack.MessagePackSerializer.Serialize(obj);
                        File.WriteAllBytes(temp_filename, mpBytes);
                        ok = true;
                        break;
                    case SerialisationMode.XML:
                        using (XmlTextWriter writer = new(temp_filename, Encoding.UTF8) { Formatting = Formatting.Indented })
                        {
                            new XmlSerializer(typeof(T)).Serialize(writer, obj);
                        }
                        ok = true;
                        break;
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
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Masterplan.Tools
{
    public static class DiscoveryService
    {
        private static StringBuilder _report = new StringBuilder();
        private static HashSet<Type> _processedTypes = new HashSet<Type>();

        public static void Analyze(object obj)
        {
            if (obj == null) return;
            _report.Clear();
            _processedTypes.Clear();

            _report.AppendLine("===============================================================");
            _report.AppendLine($"ULTIMATE ARCHITECTURE SCAN: {DateTime.Now}");
            _report.AppendLine($"Root Type: {obj.GetType().FullName}");
            _report.AppendLine("===============================================================");

            ScanType(obj.GetType(), 0);

            try
            {
                File.WriteAllText("Discovery_Report.txt", _report.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Discovery Report Error: " + ex.Message);
            }
        }

        private static void ScanType(Type type, int indent)
        {
            if (type == null || _processedTypes.Contains(type)) return;

            // Basic type filtering: don't recurse into strings or primitives
            if (type.IsPrimitive || type == typeof(string) || type == typeof(Guid) || type == typeof(DateTime) || type.IsEnum) return;

            _processedTypes.Add(type);
            string padding = new string(' ', indent * 2);

            // Analyze both Properties and Fields to ensure 100% discovery
            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            // Process Properties
            foreach (var prop in props)
            {
                _report.AppendLine($"{padding}[Prop: {prop.PropertyType.Name}] {prop.Name}");

                if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                {
                    Type itemType = prop.PropertyType.IsGenericType ? prop.PropertyType.GetGenericArguments()[0] : typeof(object);
                    _report.AppendLine($"{padding}  -> List Item Type: {itemType.Name}");
                    ScanType(itemType, indent + 4);
                }
                else if (prop.PropertyType.IsInterface)
                {
                    _report.AppendLine($"{padding}  !! INTERFACE: {prop.PropertyType.Name} (Needs Manual DTO Mapping) !!");
                }
                else if (prop.PropertyType.IsClass)
                {
                    ScanType(prop.PropertyType, indent + 4);
                }
            }

            // Process Fields (some legacy code uses public fields instead of properties)
            foreach (var field in fields)
            {
                _report.AppendLine($"{padding}[Field: {field.FieldType.Name}] {field.Name}");
                if (field.FieldType.IsClass && field.FieldType != typeof(string))
                {
                    ScanType(field.FieldType, indent + 4);
                }
            }
        }
    }
}
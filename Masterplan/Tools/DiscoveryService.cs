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
            _report.AppendLine($"DEEP SCAN: {DateTime.Now} | Type: {obj.GetType().FullName}");
            _report.AppendLine("===============================================================");

            ScanProperties(obj.GetType(), 0);

            File.WriteAllText("Discovery_Report.txt", _report.ToString());
        }

        private static void ScanProperties(Type type, int indent)
        {
            if (type == null || _processedTypes.Contains(type)) return;
            if (type.IsPrimitive || type == typeof(string) || type == typeof(Guid)) return;

            bool isCoreData = type.FullName.StartsWith("Masterplan.Data");
            if (isCoreData) _processedTypes.Add(type);

            string padding = new string(' ', indent * 2);
            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                _report.AppendLine($"{padding}[{prop.PropertyType.Name}] {prop.Name}");

                if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                {
                    Type itemType = prop.PropertyType.IsGenericType ? prop.PropertyType.GetGenericArguments()[0] : typeof(object);
                    _report.AppendLine($"{padding}  -> Collection Item: {itemType.Name}");
                    if (itemType.FullName.StartsWith("Masterplan.Data")) ScanProperties(itemType, indent + 4);
                }
                else if (prop.PropertyType.FullName.StartsWith("Masterplan.Data"))
                {
                    ScanProperties(prop.PropertyType, indent + 4);
                }
            }
        }
    }
}
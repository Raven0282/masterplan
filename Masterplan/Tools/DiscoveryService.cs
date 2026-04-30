using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Masterplan.Tools
{
    /// <summary>
    /// A diagnostic utility that maps a unified property schema across multiple files.
    /// Deduplicates property paths to create a single Master Schema for DTO migration.
    /// </summary>
    public static class DiscoveryService
    {
        // Persistent cache to ensure we only log a property path once per session
        private static readonly HashSet<string> _discoveredPaths = new HashSet<string>();
        private static readonly HashSet<object> _visitedObjects = new HashSet<object>();
        private static readonly List<string> _reportLines = new List<string>();

        /// <summary>
        /// Analyzes an object graph and appends unique property paths to the Master Schema.
        /// </summary>
        /// <param name="root">The object to analyze.</param>
        /// <param name="filePath">The source file path for context.</param>
        /// <param name="prefix">Report filename prefix (e.g., "MasterSchema").</param>
        /// <param name="clearCache">If true, clears previously discovered paths before running.</param>
        public static void RunDiscovery(object root, string filePath, string prefix = "MasterSchema", bool clearCache = false)
        {
            if (root == null) return;

            if (clearCache)
            {
                _discoveredPaths.Clear();
                _reportLines.Clear();
            }

            // Always clear visited objects per-file to ensure we scan the new file's graph fully
            _visitedObjects.Clear();

            string directory = Path.GetDirectoryName(filePath);
            string discoveryFolder = Path.Combine(directory, "Discovery");

            if (!Directory.Exists(discoveryFolder))
            {
                Directory.CreateDirectory(discoveryFolder);
            }

            // We log the source file for traceability, but the property paths themselves are deduplicated
            _reportLines.Add($"// Processing Source: {Path.GetFileName(filePath)}");

            MapProperties(root, root.GetType().Name);

            // Save the current state of the Master Schema after each file is processed
            string reportPath = Path.Combine(discoveryFolder, $"{prefix}.txt");
            File.WriteAllLines(reportPath, _reportLines);
        }

        private static void MapProperties(object obj, string path)
        {
            if (obj == null) return;

            Type type = obj.GetType();

            // Skip common primitives to focus on complex data structures
            if (type.IsPrimitive || type == typeof(string) || type == typeof(Guid) || type == typeof(DateTime))
            {
                return;
            }

            if (_visitedObjects.Contains(obj)) return;
            _visitedObjects.Add(obj);

            // Handle Collections: Deduplicate the collection path itself
            if (obj is IEnumerable enumerable && !(obj is string))
            {
                if (!_discoveredPaths.Contains(path + "[]"))
                {
                    _reportLines.Add($"[COLLECTION] {path} (Type: {type.Name})");
                    _discoveredPaths.Add(path + "[]");
                }

                int count = 0;
                foreach (var item in enumerable)
                {
                    if (item == null) continue;
                    // Scan the first item to determine the internal schema of the collection
                    if (count < 1)
                    {
                        MapProperties(item, $"{path}[]");
                    }
                    count++;
                }
                return;
            }

            // Reflect on Properties
            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                try
                {
                    if (prop.GetIndexParameters().Length > 0) continue;

                    string currentPath = $"{path}.{prop.Name}";
                    string info = $"{currentPath} (Type: {prop.PropertyType.Name})";

                    // Only add to report if this specific path has never been seen before
                    if (!_discoveredPaths.Contains(currentPath))
                    {
                        _reportLines.Add(info);
                        _discoveredPaths.Add(currentPath);
                    }

                    object val = prop.GetValue(obj);
                    if (val != null)
                    {
                        MapProperties(val, currentPath);
                    }
                }
                catch
                {
                    // Ignore inaccessible properties
                }
            }
        }
    }
}
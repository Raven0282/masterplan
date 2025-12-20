#nullable disable

using Masterplan.Data;
using Masterplan.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Masterplan.Tools
{
    /// <summary>
    /// PARTIAL CLASS: WORLD & PLOT
    /// Handles the generation of HTML for plot points, map areas, and locations.
    /// This class relies on helper methods defined in HtmlCore.cs.
    /// </summary>
    public static partial class HTML
    {
        // --- PLOT POINT GENERATION ---

        /// <summary>
        /// Generates the HTML for a single PlotPoint.
        /// </summary>
        /// <param name="pp">The PlotPoint data.</param>
        /// <param name="edit_format">The width format for display.</param>
        /// <param name="size">The display size.</param>
        /// <returns>The HTML string.</returns>
        public static string PlotPoint(PlotPoint pp, EditFormat edit_format, DisplaySize size)
        {
            List<string> lines = new List<string>();

            lines.Add("<HTML>");
            lines.AddRange(HTML.GetStyle(size)); // Dependency on HtmlCore.GetStyle
            lines.Add("<BODY>");

            if (pp != null)
            {
                lines.Add("<H3>" + Process(pp.Name) + "</H3>"); // Dependency on HtmlCore.Process

                lines.Add(Wrap(Process(pp.Details))); // Dependency on HtmlCore.Wrap
            }
            else
            {
                lines.Add("<P class=instruction>(no plot point selected)</P>");
            }

            lines.Add("</BODY>");
            lines.Add("</HTML>");

            return Concatenate(lines); // Dependency on HtmlCore.Concatenate
        }

        // --- MAP AREA GENERATION ---

        /// <summary>
        /// Generates the HTML for a single MapArea.
        /// </summary>
        /// <param name="area">The MapArea data.</param>
        /// <param name="edit_format">The width format for display.</param>
        /// <param name="size">The display size.</param>
        /// <returns>The HTML string.</returns>
        public static string MapArea(MapArea area, EditFormat edit_format, DisplaySize size)
        {
            List<string> lines = new List<string>();

            lines.Add("<HTML>");
            lines.AddRange(HTML.GetStyle(size));
            lines.Add("<BODY>");

            if (area != null)
            {
                lines.Add("<H3>" + Process(area.Name) + "</H3>");
                lines.Add(Wrap(Process(area.Details)));
            }
            else
            {
                lines.Add("<P class=instruction>(no map area selected)</P>");
            }

            lines.Add("</BODY>");
            lines.Add("</HTML>");

            return Concatenate(lines);
        }

        // --- MAP LOCATION GENERATION ---

        /// <summary>
        /// Generates the HTML for a single MapLocation.
        /// </summary>
        /// <param name="loc">The MapLocation data.</param>
        /// <param name="size">The display size.</param>
        /// <returns>The HTML string.</returns>
        public static string MapLocation(MapLocation loc, DisplaySize size)
        {
            List<string> lines = new List<string>();

            lines.Add("<HTML>");
            lines.AddRange(HTML.GetStyle(size));
            lines.Add("<BODY>");

            if (loc != null)
            {
                lines.Add("<H3>" + Process(loc.Name) + "</H3>");
                lines.Add(Wrap(Process(loc.Details)));
            }
            else
            {
                lines.Add("<P class=instruction>(no map location selected)</P>");
            }

            lines.Add("</BODY>");
            lines.Add("</HTML>");

            return Concatenate(lines);
        }

        // --- PUBLIC HELPERS (Accessors) ---

        /// <summary>
        /// Helper method to retrieve the HTML for a plot point.
        /// </summary>
        public static string get_plot_point(PlotPoint pp, EditFormat edit_format, DisplaySize size)
        {
            return PlotPoint(pp, edit_format, size);
        }

        /// <summary>
        /// Helper method to retrieve the HTML for a specific map area.
        /// </summary>
        public static string get_map_area_details(MapArea area, EditFormat edit_format, DisplaySize size)
        {
            return MapArea(area, edit_format, size);
        }

        /// <summary>
        /// Helper method to retrieve the HTML for a map location.
        /// </summary>
        public static string get_map_location(MapLocation loc, DisplaySize size)
        {
            return MapLocation(loc, size);
        }

        // NOTE: get_plot, get_encounter, get_map_link, get_map_tile 
        // will be handled in subsequent files (Encyclopedia/Gameplay) 
        // or the final Project Export file depending on their dependencies.
    }
}
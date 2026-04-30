#nullable enable

using PuppeteerSharp;
using System;
using System.Threading.Tasks;

namespace Masterplan.Tools
{
    /// <summary>
    /// Service class responsible for rendering HTML content into PNG or JPG files using a headless browser.
    /// </summary>
    public class HtmlTableExporter
    {
        private const string TableSelector = "table"; // CSS selector for the element we want to screenshot

        /// <summary>
        /// Renders a given HTML string using a headless Chrome instance and returns the PNG image data.
        /// </summary>
        /// <param name="htmlContent">The full HTML content to render (including CSS/Style tags).</param>
        /// <returns>A Task representing the asynchronous operation, returning a byte array containing the PNG image data, or null on failure.</returns>
        public async Task<byte[]?> ExportTableToPng(string htmlContent)
        {
            // Call the internal helper with PNG type
            return await ExportTableInternal(htmlContent, ScreenshotType.Png);
        }

        /// <summary>
        /// Renders a given HTML string using a headless Chrome instance and returns the JPG image data.
        /// Required by the JpgExporter class.
        /// </summary>
        /// <param name="htmlContent">The full HTML content to render.</param>
        /// <returns>A Task representing the asynchronous operation, returning a byte array containing the JPG image data, or null on failure.</returns>
        public async Task<byte[]?> ExportTableToJpg(string htmlContent)
        {
            // Call the internal helper with JPEG type
            return await ExportTableInternal(htmlContent, ScreenshotType.Jpeg);
        }

        /// <summary>
        /// Internal helper method containing the shared Puppeteer logic for both PNG and JPG exports.
        /// </summary>
        /// <param name="htmlContent">The HTML to render.</param>
        /// <param name="type">The type of screenshot (PNG or JPEG).</param>
        private async Task<byte[]?> ExportTableInternal(string htmlContent, ScreenshotType type)
        {
            Console.WriteLine($"1. Downloading Chromium browser executable (if not already present)...");
            try
            {
                // Removed 'BrowserVersion.Latest' to use the default DownloadAsync() overload.
                await new BrowserFetcher().DownloadAsync();

                Console.WriteLine("2. Launching headless browser...");
                await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    DefaultViewport = new ViewPortOptions { Width = 1000, Height = 800 } // Set a reasonable default viewport size
                });

                await using var page = await browser.NewPageAsync();

                Console.WriteLine("3. Setting HTML content into the browser page...");
                await page.SetContentAsync(htmlContent);

                Console.WriteLine($"4. Locating the table element with selector: {TableSelector}");
                var tableElement = await page.QuerySelectorAsync(TableSelector);

                if (tableElement == null)
                {
                    Console.WriteLine($"Error: Could not find the table element with selector: {TableSelector}");
                    return null; // Return null on failure
                }

                // 4b. NEW LOGIC: Check and set table width to 400px if not explicitly set in the inline style.
                Console.WriteLine("4b. Checking and setting default table width if necessary...");

                await page.EvaluateExpressionAsync($@"
                    const table = document.querySelector('{TableSelector}');
                    if (table) {{
                        // Force max-width to 400px and set width to 100% to ensure it scales correctly within that limit.
                        table.style.maxWidth = '400px';
                        table.style.width = '99%';
                        console.log('Forcing max-width: 400px applied successfully.');
                    }}
                ");
                // ---------------------------------------------

                Console.WriteLine($"5. Capturing {type} screenshot of the table element...");

                var screenshotOptions = new ElementScreenshotOptions
                {
                    Type = type,
                    OmitBackground = false
                };

                // JPG supports quality settings (0-100). 
                // We set it to 90 for a good balance of quality and size.
                if (type == ScreenshotType.Jpeg)
                {
                    screenshotOptions.Quality = 90;
                }

                // Take a screenshot and get the result as a Base64 encoded string.
                string base64Image = await tableElement.ScreenshotBase64Async(screenshotOptions);

                // Convert the Base64 string back into a byte array for the caller to save.
                Console.WriteLine("6. Image data successfully retrieved.");
                return Convert.FromBase64String(base64Image);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during export: {ex.Message}");
                Console.WriteLine("Ensure PuppeteerSharp is installed and dependencies are met.");
                return null; // Return null on exception
            }
        }
    }
}
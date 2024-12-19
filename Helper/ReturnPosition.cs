using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaywrightNFL.Helper
{
    public static class ReturnPosition
    {
        private static Dictionary<string, string> namePosition = new();
        public static async Task<string> returnFromDictionary(string playerName, IPage page, string selectWeekOption)
        {

            if (namePosition.ContainsKey(playerName))
            {
                return namePosition[playerName];
            }

            var newURL = await page.GetByRole(AriaRole.Link, new() { Name = playerName }).GetAttributeAsync("href");

            if (newURL == null)
            {
                Console.WriteLine("Null URL");
            }
            var newPage = await InitializeBrowser.SetURL(newURL);

            await newPage.Locator(".Row > div").IsVisibleAsync();
            var positionExtraInfo = await newPage.Locator(".Row > div").TextContentAsync();

            string? positionExtraInfoString = Convert.ToString(positionExtraInfo);

            if (positionExtraInfoString != null)
            {
                var justPositionSplit = positionExtraInfoString.Split(',');
                var justPosition = justPositionSplit[1].Trim();
                namePosition.Add(playerName, justPosition);
            }

            await newPage.CloseAsync();

            return namePosition[playerName];
        }
    }
}

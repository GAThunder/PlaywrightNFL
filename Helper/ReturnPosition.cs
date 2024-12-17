using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaywrightNFL.Helper
{
    public class ReturnPosition
    {
        private static Dictionary<string, string> namePosition = new();
        public static async Task<string> returnFromDictionary(string playerName, IPage page, string selectWeekOption)
        {
            if (namePosition.ContainsKey(playerName))
            {
                return namePosition[playerName];
            }

            await page.GetByRole(AriaRole.Link, new() { Name = playerName }).ClickAsync();
            await page.Locator(".Row > div").IsVisibleAsync();
            var positionExtraInfo = await page.Locator(".Row > div").TextContentAsync();

            string? positionExtraInfoString = Convert.ToString(positionExtraInfo);

            if (positionExtraInfoString != null)
            {
                var justPositionSplit = positionExtraInfoString.Split(',');
                var justPosition = justPositionSplit[1].Trim();
                namePosition.Add(playerName, justPosition);
            }

            await page.GoBackAsync();

            await InitializeBrowser.GoToPassingPage(page, selectWeekOption);

            return namePosition[playerName];
        }
    }
}

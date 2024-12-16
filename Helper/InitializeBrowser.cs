using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlaywrightNFL.Helper
{
    public class InitializeBrowser()
    {
        public async Task<IPage> SetURL(string browserURL)
        {
            // initialize a Playwright instance to
            // perform browser automation
            var playwright = await Playwright.CreateAsync();

            //set the selector for Yahoos attributes
            playwright.Selectors.SetTestIdAttribute("data-tst");

            // initialize a Chromium instance
            var browser = await playwright.Chromium.LaunchAsync(new()
            {
                Headless = false, // set to "false" while developing
            });
            // open a new page within the current browser context
            var page = await browser.NewPageAsync();

            // Internet here is a bit slow, needs time
            page.SetDefaultTimeout(0);

            // visit the target page
            await page.GotoAsync(browserURL);

            return page;
        }
    }

}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlaywrightNFL.Helper
{
    public static class InitializeBrowser
    {
        public static async Task<IPage> SetURL(string browserURL)
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

            
            await page.RouteAsync("**/*", async route =>
           {
               if (route.Request.Url.Contains("googlesyndication"))
               {
                   await route.AbortAsync();
               }
               else
               {
                   await route.ContinueAsync();
               }
           });

            // Internet here is a bit slow, needs time
            page.SetDefaultTimeout(0);

            // visit the target page
            await page.GotoAsync(browserURL);

            return page;
        }

        public static async Task GoToPassingPage(IPage page, string selectWeekOption)
        {
            // Select the week we're getting the stats for
            await page.GetByTestId("selection-dropdown").Nth(1).SelectOptionAsync(new[] { selectWeekOption });
            //Select the stat category
            await page.GetByRole(AriaRole.Button, new() { Name = "Passing" }).ClickAsync();
            //Get all rows of stats for that week
            await page.GetByRole(AriaRole.Table).IsVisibleAsync();
        }
    }

}

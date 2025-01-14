﻿using Microsoft.Playwright;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;
using System.Collections.Generic;
using CsvHelper;
using System.Dynamic;
using System.Threading;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using CsvHelper.Configuration;
using PlaywrightNFL.Classes;
using PlaywrightNFL.Helper;
using System.Runtime.InteropServices;

namespace PlaywrightNFL;
class Program
{
    static async Task Main()
    {
        //put the URL into a variable to make it easier to manage in case it has to be called again
        var browserURL = "https://sports.yahoo.com/nfl/stats/weekly/?selectedTable=0&week={%22week%22:1,%22seasonPhase%22:%22REGULAR_SEASON%22}";
        //returns type IPage
        var page = await InitializeBrowser.SetURL(browserURL);

        //Select the week dropdown and get all current weeks
        var pageWeekCount = await page.GetByTestId("selection-dropdown").Nth(1).AllInnerTextsAsync();
       //split it into an array so we can set weekMax from the count
        var weekArr = pageWeekCount[0].Split('\n');
        var weekMax = weekArr.Length;

        var pageCurrentYear = await page.GetByTestId("season-dropdown").AllInnerTextsAsync();
        var pageCurrentYearArr = pageCurrentYear[0].Split('\n');
        var currentYear = pageCurrentYearArr[0];

        for (int weekCount = 1; weekCount <= weekMax; weekCount++)
        {

            var selectWeekOption = "Map { \"week\": " + weekCount + ", \"seasonPhase\": \"REGULAR_SEASON\" }";

            await InitializeBrowser.GoToPassingPage(page, selectWeekOption);

            var tableRows = await page.GetByRole(AriaRole.Row).AllInnerTextsAsync();
            //Turn it into a list as tableRows isn't enumerable
            List<string> ListRows = tableRows.ToList();

            //Skip the header row
            IEnumerable<string[]> TableParsed = ListRows.Skip(1).Select(x =>
            {
                string replacement = Regex.Replace(x, @"\n", "");
                var parsed = replacement.Split('\t');
                return parsed;
            });

            var PlayerRows = new List<QB>();

            await CreatePlayers.AddPlayersToList(TableParsed, PlayerRows, page, selectWeekOption);
            
            using (var writer = new StreamWriter($"Week{weekCount}_{currentYear}.csv"))

            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<QBMap>();
                // populate the CSV file
                csv.WriteRecords(PlayerRows);
            }

            Console.WriteLine($"Week {weekCount} Completed");
        }
    }
}

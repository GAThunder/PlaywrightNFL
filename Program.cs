using Microsoft.Playwright;
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

namespace PlaywrightNFL;
class Program
{
   
    static async Task Main()
    {

        

        //put the URL into a variable to make it easier to manage in case it has to be called again
        var browserURL = "https://sports.yahoo.com/nfl/stats/weekly/?selectedTable=0&week={%22week%22:1,%22seasonPhase%22:%22REGULAR_SEASON%22}";
        var browserSetUp = new InitializeBrowser();
        var page = await browserSetUp.SetURL(browserURL);

        //set the initial week, and the maximum amount of weeks currently played.
        int weekCount = 1;
        int weekMax = 15;
        int currentYear = 2024;

        string currentWeekLabel = "WEEK 1WEEK 2WEEK 3WEEK 4WEEK 5WEEK 6WEEK 7WEEK 8WEEK 9WEEK 10WEEK 11WEEK 12WEEK 13WEEK 14WEEK 15";
        string selectWeekOption = "Map { \"week\": " + weekCount + ", \"seasonPhase\": \"REGULAR_SEASON\" }";

        Dictionary<string, string> name_position = new Dictionary<string, string>();
        var PlayerRows = new List<QB>();

        

        for (weekCount = 1; weekCount <= weekMax; weekCount++)
        {

            // Select the week we're getting the stats for
            await page.GetByLabel(currentWeekLabel, new() { Exact = true }).SelectOptionAsync(new[] { selectWeekOption });
            //Select the stat category
            await page.GetByRole(AriaRole.Button, new() { Name = "Passing" }).ClickAsync();
            //Get all rows of stats for that week
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

            /* The table I'm scraping doesn't have player positions. This will click on the player page, and fetch the position
             * since the player's name appears multiple times, it'll put it into an internal dictionary, to fetch it inside the program
             * and reduce page calls
             */

            Console.WriteLine(weekCount);

            foreach (var playerInfoArr in TableParsed) {

                //var playerInfoArr = TableParsed[0];
                string? playerName = playerInfoArr[0];

                //Check if the player is in the dictionary, and if not get his position and add him
                if (!name_position.ContainsKey(playerName))
                {

                    await page.GetByRole(AriaRole.Link, new() { Name = playerName }).ClickAsync();
                    await page.Locator(".Row > div").IsVisibleAsync();
                    var positionExtraInfo = await page.Locator(".Row > div").TextContentAsync();

                    string? positionExtraInfoString = Convert.ToString(positionExtraInfo);

                    if (positionExtraInfoString != null)
                    {
                        var justPositionSplit = positionExtraInfoString.Split(',');
                        var justPosition = justPositionSplit[1].Trim();
                        name_position.Add(playerName, justPosition);
                    }
                    await page.GoBackAsync();
                    // Select the week we're getting the stats for
                    await page.GetByLabel(currentWeekLabel, new() { Exact = true }).SelectOptionAsync(new[] { selectWeekOption });
                    //Select the stat category
                    await page.GetByRole(AriaRole.Button, new() { Name = "Passing" }).ClickAsync();
                }

                // CSV won't print it if its not a string, gotta manually output
                var newPlayer = new QB {
                    Name = playerInfoArr[0],
                    Team = playerInfoArr[1],
                    Position = name_position[playerInfoArr[0]],
                    QBRating = playerInfoArr[2],
                    Completions = playerInfoArr[3],
                    Attempts = playerInfoArr[4],
                    Yards = playerInfoArr[6],
                    Touchdowns = playerInfoArr[8],
                    Interceptions = playerInfoArr[9],
                    FirstDowns = playerInfoArr[10],
                    Sacks = playerInfoArr[11],
                    Fumbles = playerInfoArr[13]
                };

                PlayerRows.Add(newPlayer);
            }
            using (var writer = new StreamWriter($"Week{weekCount}_{currentYear}.csv"))

            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<QBMap>();
                // populate the CSV file
                csv.WriteRecords(PlayerRows);
            }


            //Clear Table
            PlayerRows.Clear();
        }
    }
}

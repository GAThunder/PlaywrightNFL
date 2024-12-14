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
class Program
{
  public class Player
    {
        public string? Name { get; set; }
        public string? Team { get; set; }
        public string? Position { get; set; }
    }

   public class QB : Player
    {
        public string? Completions { get; set; }
        public string? Attempts { get; set; }
        public string? Yards { get; set; }
        public string? Touchdowns { get; set; }
        public string? Interceptions { get; set; }
        public string? Sacks { get; set; }
        public string? Fumbles { get; set; }
        public string? FirstDowns { get; set; }
        public string? QBRating { get; set; }
    }

    public sealed class QBMap : ClassMap<QB>
    {
        public QBMap()
        {
            Map(m => m.Name).Index(0);
            Map(m => m.Position).Index(1);
            Map(m => m.Team).Index(2);
            Map(m => m.Completions).Index(3);
            Map(m => m.Attempts).Index(4);
            Map(m => m.Yards).Index(5);
            Map(m => m.Touchdowns).Index(6);
            Map(m => m.Interceptions).Index(7);
            Map(m => m.Sacks).Index(8);
            Map(m => m.Fumbles).Index(9);
            Map(m => m.FirstDowns).Index(10);
            Map(m => m.QBRating).Index(11);
        }
    }
    static async Task Main()
    {

        // initialize a Playwright instance to
        // perform browser automation
        using var playwright = await Playwright.CreateAsync();

        // initialize a Chromium instance
        await using var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = false, // set to "false" while developing
        });
        // open a new page within the current browser context
        var page = await browser.NewPageAsync();
        //put the URL into a variable to make it easier to manage in case it has to be called again
        var browserURl = "https://sports.yahoo.com/nfl/stats/weekly/?selectedTable=0&week={%22week%22:1,%22seasonPhase%22:%22REGULAR_SEASON%22}";
        // Internet is a bit slow, needs time
        page.SetDefaultTimeout(0);
        // visit the target page
        await page.GotoAsync(browserURl);

        //set the initial week, and the maximum amount of weeks currently played.
        int weekCount = 1;
        int weekMax = 15;
        int currentYear = 2024;

        string currentWeekLabel = "WEEK 1WEEK 2WEEK 3WEEK 4WEEK 5WEEK 6WEEK 7WEEK 8WEEK 9WEEK 10WEEK 11WEEK 12WEEK 13WEEK 14WEEK 15";
        string selectWeekOption = "Map { \"week\": " + weekCount + ", \"seasonPhase\": \"REGULAR_SEASON\" }";

        Dictionary<string, string> name_position = new Dictionary<string, string>();
        var PlayerRows = new List<QB>();

        var TableParsed = new List<string[]>();

        for (weekCount = 1; weekCount <= weekMax; weekCount++)
        {

            // Select the week we're getting the stats for
            await page.GetByLabel(currentWeekLabel, new() { Exact = true }).SelectOptionAsync(new[] { selectWeekOption });
            //Select the stat category
            await page.GetByRole(AriaRole.Button, new() { Name = "Passing" }).ClickAsync();
            //Get all rows of stats for that week
            var tableRows = await page.GetByRole(AriaRole.Row).AllInnerTextsAsync();
            //Turn it into a list to manage it easier
            List<string> ListRows = tableRows.ToList();


            //Console log it to help make sure the information is displaying correctly
            ListRows.ForEach(row =>
            {
                //These two remove any new line breaks so its on a singal line
                string replacement = Regex.Replace(row, @"\n", "");
                //Takes the line and makes it into an array, will use this when assigning it to a class and exporting
                var parsed = replacement.Split('\t');

                if (parsed[0] != "Player")
                {
                    TableParsed.Add(parsed);
                }

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
            TableParsed.Clear();
            




        }
    }
}

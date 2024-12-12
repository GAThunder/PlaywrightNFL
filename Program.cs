using Microsoft.Playwright;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;
using System.Collections.Generic;
using CsvHelper;
using System.Dynamic;
using System.Threading;
class Program
{
    class Player
    {
        public string? Name { get; set; }
        public string? Team { get; set; }
        public string? Position { get; set; }
    }

    class QB : Player
    {
        public int? Attempt { get; set; }
        public int? Completion { get; set; }
        public int? Yards { get; set; }
        public int? Touchdown { get; set; }
        public int? Interception { get; set; }
        public double? QBRating { get; set; }
        public int? Sack { get; set; }
        public int? Fumbles { get; set; }
        public int? FirstDowns { get; set; }
    }
    static async Task Main()
    {

        // initialize a Playwright instance to
        // perform browser automation
        using var playwright = await Playwright.CreateAsync();

        // initialize a Chromium instance
        await using var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = true, // set to "false" while developing
        });
        // open a new page within the current browser context
        var page = await browser.NewPageAsync();
        //put the URL into a variable to make it easier to manage in case it has to be called again
        var browserURl = "https://sports.yahoo.com/nfl/stats/weekly/?selectedTable=0&week={%22week%22:1,%22seasonPhase%22:%22REGULAR_SEASON%22}";

        page.SetDefaultTimeout(0);
        // visit the target page
        await page.GotoAsync(browserURl);

        //set the initial week, and the maximum amount of weeks currently played.
        int weekCount = 1;
        int weekMax = 14;

        Dictionary<string, string> name_position = new Dictionary<string, string>();
        var PlayerRows = new List<Player>();

        var TableParsed = new List<string[]>();

        for (weekCount = 1; weekCount <= weekMax; weekCount++)
        {

            // Select the week we're getting the stats for
            await page.GetByLabel("WEEK 1WEEK 2WEEK 3WEEK 4WEEK 5WEEK 6WEEK 7WEEK 8WEEK 9WEEK 10WEEK 11WEEK 12WEEK 13WEEK 14", new() { Exact = true }).SelectOptionAsync(new[] { "Map { \"week\": " + weekCount + ", \"seasonPhase\": \"REGULAR_SEASON\" }" });
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

            var testArr = TableParsed[1];
            string? testVar = testArr[0];

            Console.WriteLine(testVar);

            await page.GetByRole(AriaRole.Link, new() { Name = testVar }).ClickAsync();
            await page.Locator(".Row > div").IsVisibleAsync();
            var positionExtraInfo = await page.Locator(".Row > div").TextContentAsync();

            string? positionExtraInfoString = Convert.ToString(positionExtraInfo);

            if (positionExtraInfoString != null)
            {
                var justPositionSplit = positionExtraInfoString.Split(',');
                var justPosition = justPositionSplit[1].Trim();
                Console.WriteLine(justPosition);
            }

            TableParsed.Clear();
            await page.GotoAsync(browserURl);




        }
    }
}

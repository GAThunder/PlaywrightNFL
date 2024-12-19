using PlaywrightNFL.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlaywrightNFL.Helper;
using Microsoft.Playwright;

namespace PlaywrightNFL.Helper
{
    public static class CreatePlayers
    {
        public static async Task AddPlayersToList(
            IEnumerable<string[]> TableParsed, 
            List<QB> PlayerRows,
            IPage page,
            string selectWeekOption)
        {
            List<Task> CreatePlayersInProgress = new List<Task>();

            foreach (var playerInfoArr in TableParsed)
            {
                if (CreatePlayersInProgress.Count < 4)
                {
                    CreatePlayersInProgress.Add(Task.Run(() => (CreatePlayer(playerInfoArr, page, selectWeekOption, PlayerRows))));
                }
                else
                {
                    await Task.WhenAll(CreatePlayersInProgress);
                    CreatePlayersInProgress.Clear();
                    CreatePlayersInProgress.Add(Task.Run(() => (CreatePlayer(playerInfoArr, page, selectWeekOption, PlayerRows))));
                }
            }

            await Task.WhenAll(CreatePlayersInProgress);
        }
        public static async Task CreatePlayer(string[] playerInfoArr, 
            IPage page, 
            string selectWeekOption,
            List<QB> PlayerRows)
        {
            string? playerName = playerInfoArr[0];

            var newPlayer = new QB
            {
                Name = playerInfoArr[0],
                Team = playerInfoArr[1],
                Position = await ReturnPosition.returnFromDictionary(playerName, page, selectWeekOption),
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
    }
}

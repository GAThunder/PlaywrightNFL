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
    public class CreatePlayers
    {
        public async Task AddPlayers(
            IEnumerable<string[]> TableParsed, 
            List<QB> PlayerRows,
            ReturnPosition namePosition,
            IPage page, 
            string selectWeekOption)
        {
            foreach (var playerInfoArr in TableParsed)
            {
                string? playerName = playerInfoArr[0];

                //Check if the player is in the dictionary, and if not get his position and add him

                var newPlayer = new QB
                {
                    Name = playerInfoArr[0],
                    Team = playerInfoArr[1],
                    Position = await namePosition.returnFromDictionary(playerName, page, selectWeekOption),
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
}

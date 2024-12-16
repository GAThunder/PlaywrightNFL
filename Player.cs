using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaywrightNFL
{
    public class Player
    {
        public string? Name { get; set; }
        public string? Team { get; set; }
        public string? Position { get; set; }
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
}

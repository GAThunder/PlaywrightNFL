using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaywrightNFL
{
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
}

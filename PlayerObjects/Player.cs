using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaywrightNFL.Classes
{
    public class Player
    {
        public string? Name { get; set; }
        public string? Team { get; set; }
        public string? Position { get; set; }
    }
}

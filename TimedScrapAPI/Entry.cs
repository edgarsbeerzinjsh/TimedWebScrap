using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimedScrapAPI
{
    public class Entry
    {
        public string Api { get; set; }
        public string Description { get; set; }
        public string Auth { get; set; }
        public bool Https { get; set; }
        public string Cors { get; set; }
        public string Link { get; set; }
        public string Category { get; set; }
    }
}

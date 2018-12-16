using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.UserData
{
    public class Configuration
    {   
        public bool IsGreedyOnRessources { get; set; }
        public string[] FarmingVillages { get; set; }
        public List<Dictionary<string, double>> Templates { get; set; }
        public User User { get; set; }
        public String TorBrowserPath { get; set; }

        public Configuration()
        {

        }

        public override string ToString()
        {
            return $"User: {User}, Using Tor:{TorBrowserPath != null}";
        }
    }
}

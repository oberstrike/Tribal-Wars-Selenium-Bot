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
      
        public string[] FarmingVillages { get; set; }
        public List<Dictionary<string, double>> Templates { get; set; }
        public User User { get; set; }
        public String TorBrowserPath { get; set; }

        public static List<string> BuildingList = new List<string>() { "main","smith","wood","stone", "iron", "wall", "market", "stable", "storage", "statue", "garage", "barracks" };
        public Configuration()
        {

        }

        public override string ToString()
        {
            return $"User: {User}, Using Tor:{TorBrowserPath != null}";
        }
    }
}

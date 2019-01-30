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
        public string[] FarmingVillages { get; set; } = new string[0];
        public string[] BuildOrder { get; set; } = new string[0];
        public User User { get; set; } = new User();
        public bool Build { get; set; }
        public bool Farm { get; set; }
        public bool Trade {get; set;}
        public int MinimumTimeToWait { get; set; } = 6;
        public int MaximumTimeToWait { get; set; } = 11;

        public Configuration()
        {

        }

        

        public override string ToString()
        {
            return $"User: {User}, Using Tor:{User.TorBrowserPath != null}";
        }
    }
}

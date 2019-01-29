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
        public User User { get; set; }
        public bool Build { get; set; }
        public bool Farm { get; set; }
        public Configuration()
        {

        }

        

        public override string ToString()
        {
            return $"User: {User}, Using Tor:{User.TorBrowserPath != null}";
        }
    }
}

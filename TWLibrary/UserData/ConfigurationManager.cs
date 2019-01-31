using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLiteApplication.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.UserData
{

    public class Configuration
    {
        public string[] FarmingVillages { get; set; }
        public string[] BuildOrder { get; set; }
        public User User { get; set; }
        public bool Build { get; set; }
        public bool Farm { get; set; }
        public bool Trade { get; set; }
        public int TimeToWait { get; set; }
        public int MinimumTimeToWait { get; set; } = 5;
        public int MaximumTimeToWait { get; set; } = 10;

        public Configuration()
        {

        }



        public override string ToString()
        {
            return $"User: {User}, Using Tor:{User.TorBrowserPath != null}";
        }
    }


    public class ConfigurationManager
    {
        public Configuration Configuration { get; set; } = new Configuration();

        public string Path { get; set; }

        public ConfigurationManager(string path)
        {
            Path = path;
            Init();

        }

        private void Init()
        {
            try
            {
                using (var file = File.OpenText(Path))
                {
                    using (var reader = new JsonTextReader(file))
                    {
                        JObject o2 = (JObject)JToken.ReadFrom(reader);
                        Configuration = JsonConvert.DeserializeObject<Configuration>(o2.ToString());
                    }

                }
            }
            catch(Exception e)
            {
                Client.Print("Die Config wurde nicht gefunden.");
                Client.Print(e.Message);
                SaveConfigFile(Configuration);
                return;
            }

        }
        
        public void SaveConfigFile(Configuration config)
        {
            var json = JsonConvert.SerializeObject(config);

            using (StreamWriter file = File.CreateText(Path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, config);
                Client.Print("Config wurde erstellt.");
            }




        }
        

    }
}

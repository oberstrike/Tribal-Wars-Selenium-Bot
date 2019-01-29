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

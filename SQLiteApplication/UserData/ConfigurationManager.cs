using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public Configuration Configuration { get; set; }

        public string Path { get; set; }

        public ConfigurationManager(string path)
        {
            Path = path;
            Init();

        }

        private void Init()
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

    }
}

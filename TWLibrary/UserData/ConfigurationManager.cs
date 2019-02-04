using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TWLibrary.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWLibrary.UserData;
using TWLibrary.Tools;
using System.Threading;

namespace TWLibrary.UserData
{

    public class Configuration
    {
        public string[] FarmingVillages { get; set; } = new string[0];
        public string[] BuildOrder { get; set; } = new string[0];
        public User User { get; set; } = new User();
        public bool Build { get; set; } = false;
        public bool Farm { get; set; } = true;
        public bool Trade { get; set; } = true;
        public int MinimumTimeToWait { get; set; } = 5;
        public int MaximumTimeToWait { get; set; } = 10;
        public EmailAccount EmailAccount { get; set; } = new EmailAccount();

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
                        CheckIfIsEncrypted(Configuration.EmailAccount);
                        CheckIfIsEncrypted(Configuration.User);

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

        private void CheckIfIsEncrypted(IEncrypted encrypted)
        {
            if (!encrypted.IsEncrypted)
            {
                encrypted.EncryptPassword();
                encrypted.IsEncrypted = true;
                SaveConfigFile(Configuration);
            }
        }

        public void SaveConfigFile(Configuration config)
        {
            var json = JsonConvert.SerializeObject(config);


            using (StreamWriter file = File.CreateText(Path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, config);
                Client.Print("Config wurde gespeichert.");
            }
            Thread.Sleep(5000);




        }
        

    }
}

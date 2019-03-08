using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using TWLibrary.Tools;
using TWLibrary.VillageData;
using TWLibrary.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWLibrary.Updaters
{
    public class MainUpdater : IUpdater
    {
        private Village village;
        private IWebDriver driver;

        private readonly string _getVillageScript = "return TribalWars.getGameData().village";
        private readonly string _getBuildingsScript = "return BuildingMain.buildings";
        private readonly By _buildingsInQueueTimePath = By.XPath("//tr[contains(@class, 'buildorder')]//span");
        private readonly By _buildingsInQueueNamePath = By.XPath("//tr[contains(@class, 'buildorder')]//img");

        public void Update(Village village)
        {
            this.village = village;
            driver = village.Driver;
            UpdateBuildingQueue();
            UpdateRessources();
            UpdateBuildings();

        }

        private void UpdateBuildings()
        {
            var building = default(Building);
            do
            {
                building = village.GetNextBuilding();
                if(building == null)
                    return;
                if (!building.IsBuildeable)
                    return;
                
                village.Build(building);
                UpdateRessources();
                Client.Sleep();

            } while (building != null);

        }

        private void UpdateBuildingQueue()
        {
  
            var timeElements = driver.FindElements(_buildingsInQueueTimePath).Select(x => TimeSpan.Parse(x.Text));
            var nameElements = driver.FindElements(_buildingsInQueueNamePath).Select(x => x.GetAttribute("title"));

            village.BuildingsInQueue = new List<KeyValuePair<string, TimeSpan>>();

            for (int i = 0; i < timeElements.Count(); i++)
            {
                village.BuildingsInQueue.Add(new KeyValuePair<string, TimeSpan>(nameElements.ElementAt(i), timeElements.ElementAt(i)));

            }
        }

        private void UpdateRessources()
        {
            var villageData = (Dictionary<string, object>)driver.ExecuteScript(_getVillageScript);
 


            ResourcesManager manager = new ResourcesManager(village);

            manager.Wood = (long)villageData["wood"];
            manager.Iron = (long)villageData["iron"];
            manager.Stone = (long)villageData["stone"];
            manager.WoodProduction = (double)villageData["wood_prod"] * 60 * 60;
            manager.IronProduction = (double)villageData["iron_prod"] * 60 * 60;
            manager.StoneProduction = (double)villageData["stone_prod"] * 60 * 60;
            manager.StorageMax = (long)villageData["storage_max"];
            manager.Population = (long)villageData["pop"];
            manager.MaxPopulation = (long)villageData["pop_max"];
            village.Buildings = GetBuildings((Dictionary<string, object>)driver.ExecuteScript(_getBuildingsScript));
            village.Csrf = (string)driver.ExecuteScript("return csrf_token");
            village.RManager = manager;
            village.Coordinates = (string) villageData["coord"];
           
        }

        /// <summary>
        /// Extrahiert die Gebäude mittels der GameData
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        private ICollection<Building> GetBuildings(Dictionary<string, object> keyValuePairs)
        {
            List<Building> newBuildings = new List<Building>();
            foreach (var key in keyValuePairs.Keys)
            {
                var dictionary = (Dictionary<string, object>)keyValuePairs[key];

                string text = null;
                TimeSpan? timeSpan = null;
                if (dictionary.ContainsKey("error"))
                {
                    text = (string)dictionary["error"];
                    if (text != null)
                    {
                        timeSpan = ConvertTextToTime(text, timeSpan);
                    }

                }
                try
                {
                    newBuildings.Add(new BuildingBuilder()
                                .WithName(key)
                                .WithWood((Int64)dictionary["wood"])
                                .WithStone((Int64)dictionary["stone"])
                                .WithIron((Int64)dictionary["iron"])
                                .WithLevel(double.Parse((string)dictionary["level"]))
                                .WithPopulation((Int64)dictionary["pop"])
                                .WithMaxLevel((Int64)dictionary["max_level"])
                                .WithBuildeable(text == null)
                                .WithBuildingTime(timeSpan)
                                .WithVillage(village)
                                .Build());

                }
                catch(Exception e)
                {
                    Client.Print(e.Message);
                    Client.Print(e.Source);
                    Client.Print(key + " wurde nicht gefunden");
                }



            }
            return newBuildings;
        }
        /// <summary>
        /// Extrahiert und Konvertiert die Zeit aus dem Gebäudetext im Hauptgebäude
        /// </summary>
        /// <param name="text"></param>
        /// <param name="timeSpan"></param>
        /// <returns>Zeit bis zum Bau</returns>
        private static TimeSpan? ConvertTextToTime(string text, TimeSpan? timeSpan)
        {
            if (text.Length > 1 && text.Contains("Genug") && text.Contains("um") && !text.Contains("am"))
            {
                var date = text.Split(' ')[4];

                var dateTime = DateTime.Parse(date);
                var nowTime = DateTime.Now;

                if (text.Contains("morgen"))
                {
                    dateTime = dateTime.AddDays(1);
                } else if (text.Contains("am"))
                {
                    var b = text.Split(' ')[7];
                }
                timeSpan = dateTime - nowTime;
            }else if (text.Contains("am"))
            {
                var strArray = text.Split(' ');
                var uhrzeit = TimeSpan.Parse(strArray[5]);

                var date = DateTime.Today.AddDays(2).Add(uhrzeit);
                timeSpan = date - DateTime.Now;



            }

            return timeSpan;
        }
    }
}

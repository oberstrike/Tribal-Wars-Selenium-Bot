using OpenQA.Selenium.Firefox;
using SQLiteApplication.VillageData;
using SQLiteApplication.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.Tools
{
    public class MainUpdater : IUpdater
    {
        private Village village;
        private FirefoxDriver driver;

        public void Update( FirefoxDriver driver, Village village)
        {
            this.village = village;
            this.driver = driver;
            UpdateBuildingQueue();
            UpdateRessources();

        }


        private void UpdateBuildingQueue()
        {
  
            var timeElements = driver.FindElementsByXPath("//tr[contains(@class, 'buildorder')]//span").Select(x => TimeSpan.Parse(x.Text));
            var nameElements = driver.FindElementsByXPath("//tr[contains(@class, 'buildorder')]//img").Select(x => x.GetAttribute("title"));

            village.BuildingsInQueue = new List<KeyValuePair<string, TimeSpan>>();

            for (int i = 0; i < timeElements.Count(); i++)
            {
                village.BuildingsInQueue.Add(new KeyValuePair<string, TimeSpan>(nameElements.ElementAt(i), timeElements.ElementAt(i)));

            }
        }

        private void UpdateRessources()
        {
            var villageData = (Dictionary<string, object>)driver.ExecuteScript("return TribalWars.getGameData().village");
            ResourcesManager manager = new ResourcesManager(village);

            manager.Wood = (Int64)villageData["wood"];
            manager.Iron = (Int64)villageData["iron"];
            manager.Stone = (Int64)villageData["stone"];
            manager.WoodProduction = (double)villageData["wood_prod"] * 60 * 60;
            manager.IronProduction = (double)villageData["iron_prod"] * 60 * 60;
            manager.StoneProduction = (double)villageData["stone_prod"] * 60 * 60;
            manager.StorageMax = (Int64)villageData["storage_max"];
            manager.Population = (Int64)villageData["pop"];
            manager.MaxPopulation = (Int64)villageData["pop_max"];
            village.Buildings = GetBuildings((Dictionary<string, object>)driver.ExecuteScript("return BuildingMain.buildings"));
            village.Csrf = (string)driver.ExecuteScript("return csrf_token");
            village.RManager = manager;
            village.Coordinates = (string) villageData["coord"];

            foreach(var building in village.Buildings)
            {
                village.RManager.GetMissingRessourcesForBuilding(building);
            }
        }

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
                        timeSpan = GetTimeToBuild(text, timeSpan);
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
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.Source);
                    Console.WriteLine(key + " wurde nicht gefunden");
                }



            }
            return newBuildings;
        }

        private static TimeSpan? GetTimeToBuild(string text, TimeSpan? timeSpan)
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

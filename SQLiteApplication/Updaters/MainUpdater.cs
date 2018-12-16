using OpenQA.Selenium.Firefox;
using SQLiteApplication.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.Tools
{
    public class MainUpdater : AbstractUpdater
    {
        private Village village;
        private FirefoxDriver driver;

        public override Action<FirefoxDriver, Village> UpdateAction { get => this.MainUpdate; }

        public void MainUpdate( FirefoxDriver driver, Village village)
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

            village.Wood = (Int64)villageData["wood"];
            village.Iron = (Int64)villageData["iron"];
            village.Stone = (Int64)villageData["stone"];
            village.WoodProduction = (double)villageData["wood_prod"] * 60 * 60;
            village.IronProduction = (double)villageData["iron_prod"] * 60 * 60;
            village.StoneProduction = (double)villageData["stone_prod"] * 60 * 60;
            village.StorageMax = (Int64)villageData["storage_max"];
            village.Population = (Int64)villageData["pop"];
            village.MaxPopulation = (Int64)villageData["pop_max"];
            village.Buildings = GetBuildings((Dictionary<string, object>)driver.ExecuteScript("return BuildingMain.buildings"));
            village.Csrf = (string)driver.ExecuteScript("return csrf_token");

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
            if (text.Length > 1 && text.Contains("Genug") && text.Contains("um"))
            {
                var date = text.Split(' ')[4];

                var dateTime = DateTime.Parse(date);
                var nowTime = DateTime.Now;

                if (text.Contains("morgen"))
                {
                    dateTime = dateTime.AddDays(1);
                }
                timeSpan = dateTime - nowTime;
            }

            return timeSpan;
        }
    }
}

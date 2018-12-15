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
  

        public override Action<FirefoxDriver, Village> UpdateAction => (driver, village) =>
        {
            UpdateRessources(driver, village);
            UpdateBuildingQueue(driver, village);
        };

        private void UpdateBuildingQueue(FirefoxDriver driver, Village village)
        {
  
            var timeElements = driver.FindElementsByXPath("//tr[contains(@class, 'buildorder')]//span").Select(x => TimeSpan.Parse(x.Text));
            var nameElements = driver.FindElementsByXPath("//tr[contains(@class, 'buildorder')]//img").Select(x => x.GetAttribute("title"));

            village.BuildingsInQueue = new List<KeyValuePair<string, TimeSpan>>();

            for (int i = 0; i < timeElements.Count(); i++)
            {
                village.BuildingsInQueue.Add(new KeyValuePair<string, TimeSpan>(nameElements.ElementAt(i), timeElements.ElementAt(i)));

            }
        }

        private void UpdateRessources(FirefoxDriver driver, Village village)
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
            village.Buildings = village.GetBuildings((Dictionary<string, object>)driver.ExecuteScript("return BuildingMain.buildings"));
            village.Csrf = (string)driver.ExecuteScript("return csrf_token");
            
        }

    }

}

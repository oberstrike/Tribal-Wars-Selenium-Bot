using SQLiteApplication.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.Tools
{
    public class MainUpdater : Updater
    {
        private Client client;

        public void Update(Client client)
        {
            client.GoTo(client.Creator.GetMain());
            this.client = client;
            UpdateRessources();
            UpdateBuildingQueue();
            Client.Sleep();

        }

        private void UpdateBuildingQueue()
        {
  
            var timeElements = client.Driver.FindElementsByXPath("//tr[contains(@class, 'buildorder')]//span").Select(x => TimeSpan.Parse(x.Text));
            var nameElements = client.Driver.FindElementsByXPath("//tr[contains(@class, 'buildorder')]//img").Select(x => x.GetAttribute("title"));

            client.Config.Village.BuildingsInQueue = new List<KeyValuePair<string, TimeSpan>>();

            for (int i = 0; i < timeElements.Count(); i++)
            {
                client.Config.Village.BuildingsInQueue.Add(new KeyValuePair<string, TimeSpan>(nameElements.ElementAt(i), timeElements.ElementAt(i)));

            }
        }

        private void UpdateRessources()
        {
            var villageData = (Dictionary<string, object>)client.Executor.ExecuteScript("return TribalWars.getGameData().village");
            UserData.Configuration config = client.Config;

            config.Village.Wood = (Int64)villageData["wood"];
            config.Village.Iron = (Int64)villageData["iron"];
            config.Village.Stone = (Int64)villageData["stone"];
            config.Village.WoodProduction = (double)villageData["wood_prod"] * 60 * 60;
            config.Village.IronProduction = (double)villageData["iron_prod"] * 60 * 60;
            config.Village.StoneProduction = (double)villageData["stone_prod"] * 60 * 60;
            config.Village.StorageMax = (Int64)villageData["storage_max"];
            config.Village.Population = (Int64)villageData["pop"];
            config.Village.MaxPopulation = (Int64)villageData["pop_max"];
            config.Village.Buildings = client.GetBuildings((Dictionary<string, object>)client.Executor.ExecuteScript("return BuildingMain.buildings"));
            client.Csrf = (string)client.Executor.ExecuteScript("return csrf_token");

        }
    }
}

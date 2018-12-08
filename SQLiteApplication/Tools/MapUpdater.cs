using System;
using System.Collections.Generic;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.VillageData;

namespace SQLiteApplication.Tools
{
    class MapUpdater : Updater
    {
        public override Action<FirefoxDriver, Village> UpdateAction => (driver, village) => {

            try
            {
                var players = (Dictionary<string, object>)driver.ExecuteScript("return TWMap.players");
                var villages = (Dictionary<string, object>)driver.ExecuteScript("return TWMap.villages");
                

                foreach (var lVillage in villages)
                {
                    TWVillage tvillage = new TWVillage((Dictionary<string, object>) lVillage.Value);
                    village.FManager.Villages.Add(tvillage);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
       
        };
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenQA.Selenium;
using SQLiteApplication.VillageData;
using SQLiteApplication.Web;

namespace SQLiteApplication.Tools
{
    class TroopUpdater : Updater
    {
        public void Update(Client client)
        {
            client.Driver.Navigate().GoToUrl(client.Creator.GetPlace());
            foreach (var unit in Farmmanager.Units)
            {
                var count = double.Parse(Regex.Match(client.Driver.FindElements(By.Id($"units_entry_all_{unit}")).First().Text, @"\d+").Value);
                client.Config.Village.GetUnits()[unit] = count;

            }
        }
    }
}

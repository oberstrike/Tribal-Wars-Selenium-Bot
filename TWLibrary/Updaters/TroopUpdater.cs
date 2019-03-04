using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using TWLibrary.Tools;
using TWLibrary.Web;

namespace TWLibrary.Updaters
{
    class TroopUpdater : IUpdater
    {
        public void Update(Village village)
        {
            foreach (Unit unit in Enum.GetValues(typeof(Unit)))
            {
                var count = double.Parse(Regex.Match(village.Driver.FindElements(By.Id($"units_entry_all_{unit}")).First().Text, @"\d+").Value);
                village.Units[unit] = count;

            }
        }
    }
}

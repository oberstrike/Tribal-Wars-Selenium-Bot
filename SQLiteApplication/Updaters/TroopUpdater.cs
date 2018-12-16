using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.Web;

namespace SQLiteApplication.Tools
{
    class TroopUpdater : AbstractUpdater
    {
        public override Action<FirefoxDriver, Village> UpdateAction => throw new NotImplementedException();

        public void TroopUpdate(FirefoxDriver driver, Village village)
        {
            foreach (Unit unit in Enum.GetValues(typeof(Unit)))
            {
                var count = double.Parse(Regex.Match(driver.FindElements(By.Id($"units_entry_all_{unit}")).First().Text, @"\d+").Value);
                village.Units[unit] = count;

            }
        }
    }
}

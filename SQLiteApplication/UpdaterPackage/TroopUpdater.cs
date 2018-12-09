using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.VillageData;
using SQLiteApplication.Web;

namespace SQLiteApplication.Tools
{
    class TroopUpdater : Updater
    {
        public override Action<FirefoxDriver, Village> UpdateAction => (driver, village) =>
        {
            var unitElements = driver.FindElements(By.XPath("//tr[contains(@class, 'row_')]//td[3]"));

            foreach(var element in unitElements)
            {
                string content = element.Text;
                string[] sArray = content.Split('/');
                int value = int.Parse(sArray[1]);
                Console.WriteLine(value);
            }
            

        };
    }
}



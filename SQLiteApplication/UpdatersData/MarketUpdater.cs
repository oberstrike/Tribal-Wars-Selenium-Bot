using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.Web;

namespace SQLiteApplication.Tools
{

    class MarketUpdater : AbstractUpdater
    {
        public override Action<FirefoxDriver, Village> UpdateAction => (driver, village) =>
        {
            
            village.HaendlerCount = int.Parse(driver.FindElement(By.Id("market_merchant_available_count")).Text);
            Client.Sleep();
        };
    }
}

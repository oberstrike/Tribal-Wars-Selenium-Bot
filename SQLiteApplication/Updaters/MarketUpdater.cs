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
    public class MarketUpdater : AbstractUpdater
    {
        public override Action<FirefoxDriver, Village> UpdateAction => throw new NotImplementedException();

        public void MarketUpdate(FirefoxDriver driver, Village village)
        {
            var available = driver.FindElement(By.Id("market_merchant_available_count")).Text;
            village.Traders = double.Parse(available);
        }
    }
}

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
    public class MarketUpdater : IUpdater { 
    
        public void Update(Village village)
        {
            var available = village.Driver.FindElement(By.Id("market_merchant_available_count")).Text;
            var total = village.Driver.FindElement(By.Id("market_merchant_total_count")).Text;
            village.TManager.AvailableTraders = int.Parse(available);
            village.TManager.TotalTraders = int.Parse(total);
            
            var elements = village.Driver.FindElements(By.XPath("//span[@class='nowrap']"));

            if (elements.Count == 0)
                return;
            var wood = GetRessource(village.Driver, "Holz");
            var iron = GetRessource(village.Driver, "Eisen");
            var stone = GetRessource(village.Driver, "Lehm");

            village.RManager.Wood += wood;
            village.RManager.Iron += iron;
            village.RManager.Stone += stone;



        }

        private static double GetRessource(IWebDriver driver, string resource)
        {
            try
            {
                return double.Parse(driver.FindElement(By.XPath("(//span[@class='icon header iron' and @title='" + resource + "'])[1]//..")).Text);
            }
            catch(Exception e)
            {
#if DEBUG
                Client.Print(e.Message);
#endif
                return 0;
            }
        }
    }
}

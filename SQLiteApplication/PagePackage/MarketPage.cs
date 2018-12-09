using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.PagesData
{
    public class MarketPage : Page
    {
        public MarketPage(Village village, FirefoxDriver driver) : base(village, driver)
        {

        }


        public override List<Updater> Updaters => new List<Updater>() { new MarketUpdater() };

        public override string Url()
        {
            return base.Url() + "&screen=market&mode=send";
        }

        public void SendRessource(int wood, int stone, int iron, string targetId)
        {
            GoTo();

            IWebElement woodInput = Driver.FindElement(By.XPath("//input[@name='wood']"));
            IWebElement stoneInput = Driver.FindElement(By.XPath("//input[@name='stone']"));
            IWebElement ironInput = Driver.FindElement(By.XPath("//input[@name='iron']"));
            IWebElement targetInput = Driver.FindElement(By.XPath("//input[@placeholder='123|456']"));

  
            woodInput.SendKeys(wood.ToString());
            stoneInput.SendKeys(stone.ToString());
            ironInput.SendKeys(iron.ToString());
            targetInput.SendKeys(targetId);
            
        }

    }
}

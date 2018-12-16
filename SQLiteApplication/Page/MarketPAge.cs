using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.Page
{
    class MarketPage : AbstractBuildingPage
    {
        public MarketPage(FirefoxDriver driver, Village village) : base(driver, village)
        {

        }


        public override string BUILDING => "market";

        public override List<AbstractUpdater> Updaters => new List<AbstractUpdater>() { new MarketUpdater() };

        public override string URL => Village.pathCreator.GetMarketModeSend();

        public void SendRessource(int wood, int stone, int iron, string targetId)
        {
            GoTo();

            var woodInput = Driver.FindElement(By.XPath("//input[@name='wood']"));
            var stoneInput = Driver.FindElement(By.XPath("//input[@name='stone']"));
            var ironInput = Driver.FindElement(By.XPath("//input[@name='iron']"));
            var targetInput = Driver.FindElement(By.XPath("//input[@placeholder='123|456']"));

            if (Village.CanConsume(wood, stone, iron, 0))
            {
                woodInput.SendKeys(wood.ToString());
                stoneInput.SendKeys(stone.ToString());
                ironInput.SendKeys(iron.ToString());
                targetInput.SendKeys(targetId);
            }
        }

    }
}

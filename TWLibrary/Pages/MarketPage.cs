using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using TWLibrary.Tools;
using TWLibrary.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TWLibrary.Updaters;

namespace TWLibrary.Page
{
    class MarketPage : AbstractBuildingPage
    {
        public IWebDriver Driver { get; set; }

        public MarketPage(Village village) : base(village)
        {
            Driver = village.Driver;
        }


        public override string BUILDING => "market";

        public override List<IUpdater> Updaters => new List<IUpdater>() { new MarketUpdater() };

        public override string URL => Village.pathCreator.GetMarketModeSend();

        public bool SendRessource(double wood, double stone, double iron, string targetCoordinates)
        {
            Driver.GoTo(URL);
            Client.Sleep();

            var woodInput = Driver.FindElement(By.XPath("//input[@name='wood']"));
            var stoneInput = Driver.FindElement(By.XPath("//input[@name='stone']"));
            var ironInput = Driver.FindElement(By.XPath("//input[@name='iron']"));
            var targetInput = Driver.FindElement(By.XPath("//input[@placeholder='123|456']"));
            var btn = Driver.FindElement(By.XPath("//input[@value='OK']"));

            if (Village.CanConsume(wood, stone, iron, 0))
            {
                woodInput.SendKeys(wood.ToString());
                Task.Delay(750).Wait();
                stoneInput.SendKeys(stone.ToString());
                Task.Delay(750).Wait();
                ironInput.SendKeys(iron.ToString());
                Task.Delay(750).Wait();
                targetInput.SendKeys(targetCoordinates);
                Task.Delay(1500).Wait();
            }
            btn.Click();
            Client.Sleep();
            btn = Driver.FindElement(By.XPath("//input[@value='OK']"));
            btn.Click();
            Client.Print($"{wood} wood, {stone} stone, {iron} iron an {targetCoordinates} gesendet.");
            return true;
        }

       
    }
}

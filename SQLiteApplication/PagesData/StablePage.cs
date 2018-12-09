
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.Tools;
using SQLiteApplication.VillageData;
using SQLiteApplication.Web;
using System;
using System.Collections.Generic;

namespace SQLiteApplication.PagesData
{
    public class StablePage : Page
    {
        public StablePage(Village village, FirefoxDriver driver) : base(village, driver)
        {

        }

        public override List<Updater> Updaters => new List<Updater>() { };

        public override string Url()
        {
            return base.Url() + "&screen=place";
        }
        
        public void TrainUnitsInStable(Dictionary<Unit, double> units)
        {
            GoTo();
            IWebElement spysInput = null;
            IWebElement lightInput = null;
            IWebElement heavyInput = null;
            try
            {
                spysInput = Driver.FindElementByXPath("//input[@id='spy_0']");
                lightInput = Driver.FindElementByXPath("//input[@id='light_0']");
                heavyInput = Driver.FindElementByXPath("//input[@id='heavy_0']");
            }
            catch(Exception e)
            {
                Console.WriteLine("Stable Error");
                Console.WriteLine(e.Message);
            
            }
            IWebElement trainBtn = Driver.FindElementByCssSelector(".btn.btn-recruit");
            FillForm(units, spysInput);
            FillForm(units, lightInput);
            FillForm(units, heavyInput);
           
            trainBtn.Click();
            Client.Sleep();
        }
        
        private void FillForm(Dictionary<Unit, double> units, IWebElement input)
        {
            foreach (var kvp in units)
            {
                string name = kvp.Key.GetName();
                double count = kvp.Value;
                if (PageVillage.IsTrainable(count, kvp.Key) && input != null)
                {
                    input.SendKeys(count.ToString());
                }
            }
        }
    }
}

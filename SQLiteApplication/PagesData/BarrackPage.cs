using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.Tools;
using SQLiteApplication.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.PagesData
{
    public class BarrackPage : Page
    {
        public BarrackPage(Village village, FirefoxDriver driver) : base(village, driver)
        {

        }


        public override List<Updater> Updaters => new List<Updater>() { new TroopUpdater() };

        public override string Url()
        {
            return base.Url() + "&screen=barracks";
        }

        public void TrainUnits(Dictionary<string, double> units, int villageId)
        {

            if (units.ContainsKey("spears") || units.ContainsKey("sword") || units.ContainsKey("axe"))
            {
                TrainUnitsInBarracks(units);
            }

            if (units.ContainsKey("spy") || units.ContainsKey("light") || units.ContainsKey("heavy"))
            {
                TrainUnitsInStable(units);
            }
        }

/*
Soll in StablePage verschoben werden.

        private void TrainUnitsInStable(Dictionary<string, double> units)
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
            catch
            {

            }
            IWebElement trainBtn = Driver.FindElementByCssSelector(".btn.btn-recruit");


            FillForm(units, spysInput, "spy");
            FillForm(units, lightInput, "light");
            FillForm(units, heavyInput, "heavy");
           
            trainBtn.Click();
            Client.Sleep();

        }
*/
        private void TrainUnitsInBarracks(Dictionary<Unit, double> units)
        {
            KeyValuePair<string, IWebElement> spearsInput = new KeyValuePair<string, IWebElement>();
            IWebElement swordInput = null;
            IWebElement axeInput = null;

            try
            {
                spearsInput = new KeyValuePair<string, IWebElement>("spears", Driver.FindElementByXPath("//input[@id='spear_0']")); ;
                swordInput = Driver.FindElementByXPath("//input[@id='sword_0']");
                axeInput = Driver.FindElementByXPath("//input[@id='axe_0']");
            }
            catch
            {

            }
            IWebElement trainBtn = Driver.FindElementByCssSelector(".btn.btn-recruit");
           


            FillForm(units, spearsInput.Value, "spears");
            FillForm(units, swordInput, "sword");
            FillForm(units, axeInput, "axe");

            trainBtn.Click();
            Client.Sleep();
        }

        private void FillForm(Dictionary<Unit, double> units, IWebElement input, string unit)
        {
            if (units.ContainsKey(unit))
            {
                double count = units[unit];
                if (PageVillage.IsTrainable(count, unit) && input != null)
                {
                    input.SendKeys(count.ToString());
                }

            }
        }
    }
}

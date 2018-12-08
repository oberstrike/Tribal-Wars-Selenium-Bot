using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.Tools;
using SQLiteApplication.VillageData;
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
           


            FillForm(units, spearsInput.Value);
            FillForm(units, swordInput);
            FillForm(units, axeInput);

            trainBtn.Click();
            Client.Sleep();
        }

        private void FillForm(Dictionary<Unit, double> units, IWebElement input)
        {
            foreach(var kvp in units)
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

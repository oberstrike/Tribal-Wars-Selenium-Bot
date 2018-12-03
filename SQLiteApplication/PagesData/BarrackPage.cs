using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.Tools;
using SQLiteApplication.Web;
using System;
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
                TrainUnitsInBarracks(units, PageVillage);
            }

            if (units.ContainsKey("spy") || units.ContainsKey("light") || units.ContainsKey("heavy"))
            {
                TrainUnitsInStable(units, PageVillage);
            }
        }

        private void TrainUnitsInStable(Dictionary<string, double> units, Village village)
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


            if (units.ContainsKey("spy"))
            {
                double count = units["spy"];
                if (village.IsTrainable(count, "spy") && spysInput != null)
                {
                    spysInput.SendKeys(count.ToString());
                }

            }
            if (units.ContainsKey("light"))
            {
                double count = units["light"];
                if (village.IsTrainable(count, "light") && lightInput != null)
                {

                    lightInput.SendKeys(units["light"].ToString());
                }

            }
            if (units.ContainsKey("heavy"))
            {
                {
                    double count = units["axe"];
                    if (village.IsTrainable(count, "axe") && heavyInput != null)
                    {

                        heavyInput.SendKeys(units["axe"].ToString());
                    }
                }
            }
            trainBtn.Click();
            Client.Sleep();

        }

        private void TrainUnitsInBarracks(Dictionary<string, double> units, Village village)
        {
            IWebElement spearsInput = null;
            IWebElement swordInput = null;
            IWebElement axeInput = null;

            try
            {
                spearsInput = Driver.FindElementByXPath("//input[@id='spear_0']");
                swordInput = Driver.FindElementByXPath("//input[@id='sword_0']");
                axeInput = Driver.FindElementByXPath("//input[@id='axe_0']");
            }
            catch
            {

            }
            IWebElement trainBtn = Driver.FindElementByCssSelector(".btn.btn-recruit");


            if (units.ContainsKey("spears"))
            {
                double count = units["spears"];
                if (village.IsTrainable(count, "spears") && spearsInput != null)
                {
                    spearsInput.SendKeys(count.ToString());
                }

            }
            if (units.ContainsKey("sword"))
            {
                double count = units["sword"];
                if (village.IsTrainable(count, "sword") && swordInput != null)
                {

                    swordInput.SendKeys(units["sword"].ToString());
                }

            }
            if (units.ContainsKey("axe"))
            {
                {
                    double count = units["axe"];
                    if (village.IsTrainable(count, "axe") && axeInput != null)
                    {

                        axeInput.SendKeys(units["axe"].ToString());
                    }
                }
            }

            trainBtn.Click();
            Client.Sleep();
        }


    }
}

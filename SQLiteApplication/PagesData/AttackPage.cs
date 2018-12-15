using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.Tools;
using SQLiteApplication.VillageData;
using SQLiteApplication.Web;

namespace SQLiteApplication.PagesData
{
    class AttackPage : AbstractPage
    {
        public AttackPage(Village village, FirefoxDriver driver) : base(village, driver)
        {
        }

        public override List<AbstractUpdater> Updaters => new List<AbstractUpdater>();

        public void Attack(Dictionary<Unit, double> units, string targetVIllage)
        {
            GoTo(targetVIllage);

            foreach (KeyValuePair<Unit, double> kvp in units)
            {
                Driver.FindElement(By.Id("unit_input_" + kvp.Key.GetName())).SendKeys(kvp.Value.ToString());
                Client.Sleep();
            }
            Driver.FindElement(By.Id("target_attack")).Click();
            Client.Sleep();
            Driver.FindElement(By.Id("troop_confirm_go")).Click();

        }

        public override string Url()
        {
            return base.Url() + "&screen=place&target=";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.Tools;

namespace SQLiteApplication.PagesData
{
    class AttackPage : Page
    {
        public AttackPage(Village village, FirefoxDriver driver) : base(village, driver)
        {
        }

        public override List<Updater> Updaters => throw new NotImplementedException();

        public void Attack(FirefoxDriver client, Dictionary<string, double> units, string target, int villageId)
        {
            GoTo(client, target);

            foreach (KeyValuePair<string, double> kvp in units)
            {
                client.FindElement(By.Id("unit_input_" + kvp.Key)).SendKeys(kvp.Value.ToString());
                Program.Sleep();
            }
            client.FindElement(By.Id("target_attack")).Click();
            Program.Sleep();
            client.FindElement(By.Id("troop_confirm_go")).Click();

        }

        public override string Url()
        {
            return base.Url() + "&screen=place&target=";
        }
    }
}

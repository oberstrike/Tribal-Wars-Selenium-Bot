using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.Tools;
using SQLiteApplication.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.Page
{
    public class PlacePage : AbstractPage
    {
        public PlacePage(FirefoxDriver driver, Village village) : base(driver, village)
        {

        }


        public override List<IUpdater> Updaters => new List<IUpdater>() { new MovementUpdater(), new TroopUpdater() };
        public override string URL => Village.pathCreator.GetPlace();

        public void Attack(Dictionary<string, double> units, string target)
        {
            Driver.GoTo(URL+"&target=" +  target);
            foreach (var kvp in units)
            {
                Driver.FindElement(By.Id("unit_input_" + kvp.Key)).SendKeys(kvp.Value.ToString());
                Client.Sleep();
            }
            Driver.FindElement(By.Id("target_attack")).Click();
            Client.Sleep();
            Driver.FindElement(By.Id("troop_confirm_go")).Click();

        }
    }
}

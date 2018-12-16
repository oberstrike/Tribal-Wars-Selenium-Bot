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
    class BarracksPage : AbstractBuildingPage
    {
        public BarracksPage(FirefoxDriver driver, Village village) : base(driver, village)
        {

        }

        public static readonly string[] UNITS = { "spear", "sword", "axe" };

        public override string BUILDING => "barracks";

        public override List<AbstractUpdater> Updaters => new List<AbstractUpdater>();

        public override string URL => Village.pathCreator.GetBarracks();

        public void Train(Unit unit, double count)
        {
            GoTo();
            var element = Driver.FindElementByXPath($"//input[@id='{unit.GetName()}_0']");
            element.SendKeys(count.ToString());
            var button = Driver.FindElementByXPath("//input[@class='btn btn-recruit']");
            button.Click();
            Client.Sleep();
        }
    }
}

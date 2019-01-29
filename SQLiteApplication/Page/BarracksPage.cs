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
    class BarracksPage : AbstractBuildingPage
    {
        public BarracksPage(Village village) : base( village)
        {

        }

        public static readonly string[] UNITS = { "spear", "sword", "axe" };

        public override string BUILDING => "barracks";

        public override List<IUpdater> Updaters => new List<IUpdater>();

        public override string URL => Village.pathCreator.GetBarracks();

        public void Train(Unit unit, double count)
        {
            Village.Driver.GoTo(URL);
            var element = Village.Driver.FindElement(By.XPath($"//input[@id='{unit.GetName()}_0']"));
            element.SendKeys(count.ToString());
            var button = Village.Driver.FindElement(By.XPath("//input[@class='btn btn-recruit']"));
            button.Click();
            Client.Sleep();
        }
    }
}

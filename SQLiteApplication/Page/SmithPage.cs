using OpenQA.Selenium.Firefox;
using SQLiteApplication.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.Page
{
    public class SmithPage : AbstractBuildingPage
    {
        public SmithPage(FirefoxDriver driver, Village village) : base(driver, village)
        {

        }

        public override string BUILDING => "smith";

        public override List<IUpdater> Updaters => new List<IUpdater>() { new SmithUpdater() };

        public override string URL => "smith";
    }
}

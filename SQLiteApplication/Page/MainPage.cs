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
    public class MainPage : AbstractPage
    {
        public MainPage(FirefoxDriver driver, Village village) : base(driver, village)
        {

        }

        public override string URL => Village.pathCreator.GetMain();


        public override List<AbstractUpdater> Updaters => new List<AbstractUpdater>() { new MainUpdater() };

        public void Build(Building building)
        {
            if (Village.CanConsume(building))
            {
                Driver.ExecuteScript($"BuildingMain.build(\"{building.Name}\")");
                Client.Sleep();
            }

        }

    }
}

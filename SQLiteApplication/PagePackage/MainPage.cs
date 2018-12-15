using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteApplication.Tools;
using SQLiteApplication.Web;

namespace SQLiteApplication.PagesData
{
    public class MainPage : AbstractPage
    {


        public MainPage(Village village, FirefoxDriver driver) : base(village, driver)
        {

        }

        public override List<AbstractUpdater> Updaters => new List<AbstractUpdater>() { new MainUpdater() };

        public override string Url()
        {
            return base.Url() + "&screen=main";
        }

        public void Build(Building building)
        {
            Console.WriteLine("URL:" + Url());
            GoTo();
            Client.Sleep();
            Driver.ExecuteScript($"BuildingMain.build(\"{building.Name}\")");
        }
    }
}

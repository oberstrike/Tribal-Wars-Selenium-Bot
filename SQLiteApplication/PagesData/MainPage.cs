using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteApplication.Tools;

namespace SQLiteApplication.PagesData
{
    public class MainPage : Page
    {


        public MainPage(Village village, FirefoxDriver driver) : base(village, driver)
        {

        }

        public override List<Updater> Updaters => new List<Updater>() { new MainUpdater() };

        public override string Url()
        {
            return base.Url() + "&screen=main";
        }

        public void Build(Building building)
        {
            Console.WriteLine("URL:" + Url());
            GoTo();
            Program.Sleep();
            Driver.ExecuteScript($"BuildingMain.build(\"{building.Name}\")");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.Tools;

namespace SQLiteApplication.PagesData
{
    public class MapPage : Page
    {
        public MapPage(Village village, FirefoxDriver driver) : base(village, driver)
        {

        }

        public override List<Updater> Updaters => new List<Updater>() { new MapUpdater() };

        public override string Url()
        {
            return base.Url() + "&screen=map";
        }
    }
}

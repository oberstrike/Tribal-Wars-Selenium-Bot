using OpenQA.Selenium.Firefox;
using SQLiteApplication.Tools;
using SQLiteApplication.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.PagesData
{
    public class PlacePage : Page
    {
        public PlacePage(Village village, FirefoxDriver driver) : base(village, driver)
        {

        }

        public override List<Updater> Updaters => new List<Updater>() { };

        public override string Url()
        {
            return base.Url() + "&screen=place";
        }

    

    }
}

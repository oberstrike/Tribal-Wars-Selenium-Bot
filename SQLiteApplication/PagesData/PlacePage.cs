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
    public class PlacePage : AbstractPage
    {
        public PlacePage(Village village, FirefoxDriver driver) : base(village, driver)
        {

        }

        public override List<AbstractUpdater> Updaters => new List<AbstractUpdater>() { };

        public override string Url()
        {
            return base.Url() + "&screen=place";
        }

    

    }
}

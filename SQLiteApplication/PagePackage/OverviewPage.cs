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
    public class OverviewPage : AbstractPage
    {
        public OverviewPage(Village village, FirefoxDriver driver ) : base(village, driver)
        {

        }

        public override string Url()
        {
            return base.Url() + "&screen=overview_villages";
        }
        
        public override List<AbstractUpdater> Updaters => new List<AbstractUpdater>() { };
    }
}

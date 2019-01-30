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
    public class PlacePage : AbstractPage
    {
        public IWebDriver Driver { get; set; }
        public PlacePage(Village village) : base( village)
        {
            Driver = village.Driver;
        }
        public override List<IUpdater> Updaters => new List<IUpdater>() { new MovementUpdater(), new TroopUpdater() };
        public override string URL => Village.pathCreator.GetPlace();


    }
}

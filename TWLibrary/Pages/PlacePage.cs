using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using TWLibrary.Tools;
using TWLibrary.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWLibrary.Updaters;

namespace TWLibrary.Page
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

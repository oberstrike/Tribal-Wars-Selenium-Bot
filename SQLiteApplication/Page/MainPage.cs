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
        public MainPage(Village village) : base(village)
        {

        }

        public override string URL => Village.pathCreator.GetMain();
        public override List<IUpdater> Updaters => new List<IUpdater>() { new MainUpdater() };

        public void Build(Building building)
        {
            if (Village.CanConsume(building))
            {
                Village.Driver.ExecuteScript($"BuildingMain.build(\"{building.Name}\")");
                Client.Sleep();
            }

        }

    }
}

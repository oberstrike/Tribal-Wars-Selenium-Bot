using TWLibrary.Tools;
using TWLibrary.Updaters;
using TWLibrary.VillageData;
using TWLibrary.Web;
using System.Collections.Generic;

namespace TWLibrary.Page
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

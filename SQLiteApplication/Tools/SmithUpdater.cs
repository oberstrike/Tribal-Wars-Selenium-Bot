using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteApplication.Web;

namespace SQLiteApplication.Tools
{
    class SmithUpdater : Updater
    {
        public void Update(Client client)
        {
            client.GoTo(client.Creator.GetSmith());
            client.Config.Village.Technologies = (Dictionary<string, object>)client.Executor.ExecuteScript("return BuildingSmith.techs.available");
            Client.Sleep();
        }
    }
}

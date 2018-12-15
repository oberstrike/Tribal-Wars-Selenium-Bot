using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using SQLiteApplication.Web;

namespace SQLiteApplication.Tools
{
    class MarketUpdater : Updater
    {
        public void Update(Client client)
        {
            client.GoTo(client.Creator.GetMarketModeSend());
            client.Config.Village.HaendlerCount = int.Parse(client.Driver.FindElement(By.Id("market_merchant_available_count")).Text);
            Client.Sleep();
        }
    }
}

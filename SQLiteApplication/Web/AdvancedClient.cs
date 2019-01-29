using SQLiteApplication.Page;
using SQLiteApplication.Tools;
using SQLiteApplication.UserData;
using System;
using System.Collections.Generic;

namespace SQLiteApplication.Web
{
    public class AdvancedClient : Client
    {
        public AdvancedClient(Configuration configuration) : base(configuration)
        {
   //         Plugins.Add(new TradingPlugin());
        }

        internal override List<Village> GetVillages()
        {
            List<Village> villages = base.GetVillages();
            foreach (Village village in villages)
            {
                village.Pages.Add(new FarmassistPage(village));
               
                village.Pages.Shuffle();
                //            village.Pages.Add(new MarketPage(village));
            }

            villages.Shuffle();
            return villages;
        }
    }
}

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
            if (Config.Trade)
                Plugins.Add(new TradingPlugin());
        }

        internal override List<Village> FindVillagesInOverviewPage()
        {

            {
                List<Village> villages = base.FindVillagesInOverviewPage();
                foreach (Village village in villages)
                {
                    if (Config.Farm)
                        village.Pages.Add(new FarmassistPage(village));
                    if (Config.Build)
                        village.Pages.Add(new MainPage(village));
                    if (Config.Trade)
                        village.Pages.Add(new MarketPage(village));
                    village.Pages.Shuffle();
                }

                villages.Shuffle();
                return villages;
            }
        }

        }
    }

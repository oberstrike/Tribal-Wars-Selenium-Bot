using TWLibrary.Page;
using TWLibrary.Tools;
using TWLibrary.UserData;
using System;
using System.Collections.Generic;

namespace TWLibrary.Web
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

using SQLiteApplication.Page;
using SQLiteApplication.VillageData;
using SQLiteApplication.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLiteApplication.Tools
{
    public interface IPlugin
    {
        void Compute(Client client);

    }

    public class TradingPlugin : IPlugin
    {
        public void Compute(Client client)
        {
            foreach(var village in client.Config.User.Villages)
            {
                if (village.Pages.Where(each => each is MarketPage).Count() == 0) village.Pages.Add(new MarketPage(village));
                village.RManager.GetMissingRessourcesForBuildings(village.GetBuildingsInBuildOrder());

            }

            IEnumerable<ResourcesManager> managers = client.Config.User.Villages.Select(each => each.RManager);

            string[] ressis = { "Wood", "Iron", "Stone" };
            foreach (string res in ressis)
            {
                List<ResourcesManager> managersWithMore = new List<ResourcesManager>();
                List<ResourcesManager> managersWithLess = new List<ResourcesManager>();

                foreach (ResourcesManager manager in managers)
                {
                    double value = (double)manager.GetType().GetProperty("Unused" + res).GetValue(manager);

                    if (value > 0)
                        managersWithMore.Add(manager);
                    else
                        managersWithLess.Add(manager);
                }

                if (managersWithMore.Count > 0)
                {

                    double ueberschuss = managersWithMore.Sum(each => (double)each.GetType().GetProperty("Unused" + res).GetValue(each));
                    double fehlende = managersWithLess.Sum(each => (double)each.GetType().GetProperty("Unused" + res).GetValue(each));

                    if (managersWithLess.Count == 0)
                        break;


                    managersWithLess.Shuffle();
                    foreach (ResourcesManager manager in managersWithLess)
                    {
                        double dorfBenoetigteMaterialien = Math.Abs(Math.Round((double)manager.GetType().GetProperty("Unused" + res).GetValue(manager)));
                        if (dorfBenoetigteMaterialien == 0)
                            break;
                        

                        if (managersWithMore.Count > 0)
                        {
                            foreach (ResourcesManager managerWithMore in managersWithMore)
                            {
                                double dorfUebrigeMaterialien = Math.Round((double)managerWithMore.GetType().GetProperty("Unused" + res).GetValue(managerWithMore));
                                Dictionary<string, double> resToSend = new Dictionary<string, double>();

                                if (dorfBenoetigteMaterialien < dorfUebrigeMaterialien)
                                {
                                    dorfUebrigeMaterialien = dorfBenoetigteMaterialien;
                                }
                                ueberschuss -= dorfUebrigeMaterialien;
                                fehlende += dorfUebrigeMaterialien;
                                resToSend.Add(res, dorfUebrigeMaterialien);

                                bool succesfull = managerWithMore.MyVillage.SendRessourceToVillage(resToSend, manager.MyVillage);
                                if(succesfull)
                                {
              
                                    if (ueberschuss == 0 | fehlende == 0)
                                        break;
                                }

    
                            }
                        }
                    }
                }
                
            }


        }
    }
}

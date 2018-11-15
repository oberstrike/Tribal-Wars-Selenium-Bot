using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.UserData;
using SQLiteApplication.VillageData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SQLiteApplication.Web
{

    public class BasisClient : Client
    {
   

        public BasisClient(string driverPath, Configuration configuration) : base(driverPath, configuration)
        {
            
        }


        public override void Farm()
        {
            string[] villages = new string[Config.FarmingVillages.Count() - 1];
            System.Array.Copy(Config.FarmingVillages, villages, villages.Length);

            var templates = Config.Templates;

            foreach (var template in templates)
            {
                bool isPossible = true;

                foreach (var key in template.Keys)
                {
                    if (template[key] > Config.Village.GetUnits()[key])
                    {
                        isPossible = false;
                    }

                }
  
                if (isPossible)
                {

                    foreach (var id in Config.FarmingVillages)
                    {
                        bool test = true;
                        if (Config.Village.TroupMovements != null)
                        {
                            foreach (var move in Config.Village.TroupMovements)
                            {
                                if (move.TargetId == id)
                                {
                                    test = false;
                                }

                            }
                        }

                        Console.WriteLine(id + "  " + test);

                        if (test)
                        {
                            Attack(template, id);
                            Thread.Sleep(500);
                            UpdateVillage();
                        }
                    }


                }

            }
        }

     

        }
}

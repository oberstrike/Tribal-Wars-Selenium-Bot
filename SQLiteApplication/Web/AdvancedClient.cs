using OpenQA.Selenium;
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
    public class AdvancedClient : Client
    {


        public AdvancedClient(Configuration configuration) : base(configuration)
        {
            
        }
        
        protected override void UpdateVillage()
        {
            var hasFarmmanager = (bool) Executor.ExecuteScript("return TribalWars.getGameData().features.FarmAssistent.active");
            if (hasFarmmanager)
            {
                base.UpdateVillage();
            }
            else
            {
                Console.WriteLine("Farmassistent wird benötigt");
                throw new Exception("Bot kann nicht gestartet.");
            }

        }

       
        public override void UpdateTroops()
        {
            Sleep();
            Driver.Navigate().GoToUrl(Creator.GetFarmAssist());
            var units = (Dictionary<string,object>) Executor.ExecuteScript("return Accountmanager.farm.current_units");
            Dictionary<string, double> nUnits = new Dictionary<string, double>();
            foreach (var values in units)
            {
       
                nUnits.Add(values.Key, double.Parse((string)values.Value));
            }
            Config.Village.SetUnits(nUnits);
            
        }
        
    }
}

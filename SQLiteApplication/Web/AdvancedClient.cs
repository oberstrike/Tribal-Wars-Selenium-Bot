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


        public AdvancedClient(string driverPath, Configuration configuration) : base(driverPath, configuration)
        {
            
        }

        public override void Farm()
        {
            if (!Driver.Url.Equals(Creator.GetFarmAssist()))
            {
                Driver.Navigate().GoToUrl(Creator.GetFarmAssist());
            }
            var forms = Driver.FindElements(By.CssSelector("form[action*='action=edit']"));
            List<Dictionary<string, object>> keyValuePairs = new List<Dictionary<string, object>>();
            List<string> templateIds = new List<string>();
            foreach(var value in forms)
            {  
                string id  = Regex.Match(value.GetAttribute("action"), @"id=(.*)&h").Groups[1].Value;
                var template = (Dictionary<string, object>) Executor.ExecuteScript($"return Accountmanager.farm.templates['t_{id}'];");
                if(template != null)
                {
                    keyValuePairs.Add(template);
                    templateIds.Add(id);
                }
            }

            var targets = GetNotAttackedVillages();

            if (targets.Count == 0)
            {
                return;
            }

            int count = 0;
            foreach (var template in keyValuePairs)
            {
                bool isPossible = true;
                foreach(var kvp in template)
                {
                    var unitCount = Config.Village.GetUnits()[kvp.Key];
                    if(unitCount < (Int64) kvp.Value)
                    {
                        isPossible = false;
                    }
                    
                }
                if (isPossible)
                {
                    Thread.Sleep(500);
                    Executor.ExecuteScript($"Accountmanager.farm.sendUnits(this, {targets[0]}, {templateIds[count++]})");
                    UpdateTroupMovement();
                    Thread.Sleep(500);
                    UpdateTroops();
                    Thread.Sleep(500);

                    targets = GetNotAttackedVillages();
                    if (targets.Count == 0)
                    {
                        return;
                    }

                    Driver.Navigate().GoToUrl(Creator.GetFarmAssist());

                    targets = GetNotAttackedVillages();
                    if (targets.Count == 0)
                    {
                        return;
                    }
                }
                else
                {
                    continue;
                }
        


            }


        }

 
        

        public override void UpdateTroops()
        {
            Thread.Sleep(500);
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

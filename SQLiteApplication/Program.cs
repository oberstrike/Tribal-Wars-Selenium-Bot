
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Threading;
using System.IO;
using SQLiteApplication.UserData;
using SQLiteApplication.Web;

namespace SQLiteApplication
{
    
    class Program
    {

        public static void Sleep()
        {
            Thread.Sleep((new Random().Next(1, 5) * 1000) + 245);

        }


        static void Main(string[] args)
        {

            string configPath = @"Config.json";

            ConfigurationManager manager = new ConfigurationManager(configPath);
            Configuration configuration = manager.Configuration;


            Village village = new Village(configuration.Village.MaxBuildings);
            configuration.Village = village;
            Client client = null;

            if (configuration.User.HasFarmmanager)
            {
                client = new AdvancedClient(@"", configuration);
            }
            else
            {
                client = new BasisClient(@"", configuration);
            }


            while (true)
            {
                client.Connect();

                DateTime? targetTime = new DateTime?();
                client.Login();
                string[] ressis = { "main" };
                foreach (var building in village.Buildings)
                {
                    if (ressis.Contains(building.Name))
                    {
                        if (building.IsBuildeable)
                        {
                            client.Build(building.Name);
                        }
                        else
                        {
                            if (!targetTime.HasValue)
                            {
                                targetTime = building.BuildingTime;
                            }
                            else
                            {
                                if(building.BuildingTime < targetTime)
                                {
                                    targetTime = building.BuildingTime;
                                }
                            }
                        }

                    }
                }


                client.Farm();
                Thread.Sleep(2500);
                client.Logout();
                client.Close();

                double diff = 0;
                if (targetTime.HasValue)
                {
                    diff = (targetTime.Value - DateTime.Now).TotalSeconds;
                    if (diff > 600 || diff < 0)
                    {
                        diff = 100 + new Random().Next(300, 700);
                    }

                }
                else
                {
                    diff = 100 + new Random().Next(300, 700);
                }




                Console.WriteLine(DateTime.Now + " Warte: " + diff + " Sekunden");



                Thread.Sleep(System.Convert.ToInt32(diff)* 1000);
            }


        

            
        }
    }
}

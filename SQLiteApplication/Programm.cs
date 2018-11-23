
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

    class Programm
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
                client = new AdvancedClient(configuration);
            }
            else
            {
                client = new Client(configuration);
            }


            while (true)
            {
                try
                {
                    client.Connect();
                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm: ")}: Starte Prozess");
                    Console.WriteLine(configuration);


                    DateTime? targetTime = new DateTime?();
                    client.Login();
                    Console.WriteLine(DateTime.Now.ToString("HH:mm: ") + configuration.Village);
                    Console.WriteLine(DateTime.Now.ToString("HH:mm: ") + village.BuildingsInQueue.Key + " " + village.BuildingsInQueue.Value.ToString("HH:mm:ss"));


                    string[] ressis = { "iron", "wood", "stone" };
                    foreach (var building in village.Buildings)
                    {
                        if (ressis.Contains(building.Name))
                        {

                            Console.WriteLine($"{DateTime.Now.ToString("HH:mm: ")}: {building}");
                            if (building.IsBuildeable)
                            {
                                client.Build(building.Name);
                                Console.WriteLine(DateTime.Now.ToString("HH:mm: ") + "Baue " + building.Name + " aus");
                            }
                            else
                            {
                                if (!targetTime.HasValue)
                                {
                                    targetTime = building.BuildingTime;
                                }
                                else
                                {
                                    if (building.BuildingTime < targetTime)
                                    {
                                        targetTime = building.BuildingTime;
                                    }
                                }
                            }

                        }
                    }
                    client.Logout();
                    client.Close();

                    double diff = 0;
                    if (targetTime.HasValue)
                    {
                        diff = (targetTime.Value - DateTime.Now).TotalSeconds;
                        if (diff > 1000 || diff < 0)
                        {
                            diff = 100 + new Random().Next(300, 900);
                        }

                    }
                    else
                    {
                        diff = 100 + new Random().Next(300, 700);
                    }


                    DateTime nextTime = DateTime.Now.AddSeconds(diff);


                    Console.WriteLine(DateTime.Now + " Warte bis " + nextTime.ToString("HH:mm:ss"));
                    Thread.Sleep(System.Convert.ToInt32(diff) * 1000);
                }
                catch(Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }
        }
    }
}

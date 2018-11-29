
using SQLiteApplication.UserData;
using SQLiteApplication.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SQLiteApplication
{
    internal class Programm
    {

        public static void Sleep()
        {
            //            Thread.Sleep((new Random().Next(1, 5) * 1000) + 245);
            Thread.Sleep((new Random().Next(1, 2) * 1000) + 245);

        }

        private static void Main(string[] args)
        {
            int errorCount = 0;

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


                    TimeSpan? targetTime = new TimeSpan?();
                    client.Login();

                    Console.WriteLine(DateTime.Now.ToString("HH:mm: ") + configuration.Village);
                    Console.WriteLine(village.Technologies["heavy"]);

                    foreach (KeyValuePair<string, TimeSpan> kvp in village.BuildingsInQueue)
                    {
                        Console.WriteLine(DateTime.Now.ToString("HH:mm: ") + kvp.Key + " " + kvp.Value);
                    }
                    string[] ressis = { "iron", "wood", "stone" };
                    foreach (Building building in village.Buildings)
                    {
                        if (ressis.Contains(building.Name))
                        {
                            Console.WriteLine($"{DateTime.Now.ToString("HH:mm: ")}: {building}");
                            if (building.IsBuildeable)
                            {
                                client.Build(building);
                                Console.WriteLine(DateTime.Now.ToString("HH:mm: ") + "Baue " + building.Name + " aus");
                            }
                            else
                            {
                                if (!targetTime.HasValue)
                                {
                                    targetTime = building.TimeToCanBuild;
                                }
                                else
                                {
                                    if (building.TimeToCanBuild < targetTime)
                                    {
                                        targetTime = building.TimeToCanBuild;
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
                        diff = (DateTime.Now.Add(targetTime.Value) - DateTime.Now).TotalSeconds;
                        if (diff > 1000 || diff < 0)
                        {
                            diff /= 4;
                        }

                    }
                    else
                    {
                        diff = 100 + new Random().Next(455, 1400);
                    }
                    DateTime nextTime = DateTime.Now.AddSeconds(diff);
                    Console.WriteLine(DateTime.Now + ": " + targetTime);
                    Console.WriteLine(DateTime.Now + " Warte bis " + nextTime.ToString("HH:mm:ss"));


                    Thread.Sleep(System.Convert.ToInt32(diff) * 1000);
                }
                catch
                {
                    if (++errorCount >= 3)
                    {
                        Console.WriteLine("Bot Schutz");
                        return;
                    }
                }
            }
        }
    }

}

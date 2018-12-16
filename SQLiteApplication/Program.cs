using SQLiteApplication.Tools;
using SQLiteApplication.UserData;
using SQLiteApplication.Web;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SQLiteApplication
{
    internal class Program
    {
        public static List<string> buildOrder = new List<string>() { "wood", "iron", "stone" };

        private static void Main(string[] args)
        {
            Console.WriteLine("Starte Test um " + DateTime.Now);
            Configuration config = new ConfigurationManager("Config.json").Configuration;
            int errorCount = 0;
           

            while (errorCount < 2)
            {
                try
                {
                    Client client = new Client(config);
                    client.Connect();
                    client.Login();

                    UpdateVillages(config);
                    UpgradeBuilding(config.User.Villages);
                    UpdateVillages(config);
                    TimeSpan? timeSpan = GetBestTime(config.User.Villages);

                    client.Logout();

                    if (!timeSpan.HasValue)
                    {
                        timeSpan = new TimeSpan(new Random().Next(2, 3), new Random().Next(1, 20), new Random().Next(1, 20));
                    }
                    timeSpan = timeSpan.Value.Add(new TimeSpan(0, 0, 1));

                    Console.WriteLine("Schlafe für " + timeSpan);
                    Console.WriteLine("Schlafe bis " + DateTime.Now.Add(timeSpan.Value));
                    Thread.Sleep(timeSpan.Value);
                }
                catch (Exception e)
                {
                    errorCount++;
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static void UpdateVillages(Configuration config)
        {
            foreach (Village village in config.User.Villages)
            {
                village.Update();

            }
        }

        private static void UpgradeBuilding(List<Village> villages)
        {
            foreach (Village village in villages)
            {
                foreach (Building building in village.Buildings)
                {
                    if (buildOrder.Contains(building.Name))
                    {
                        if (building.IsBuildeable && building.Level != building.MaxLevel)
                        {
                            village.Build(building);
                            break;
                        }
                    }
                }
            }
        }

        private static TimeSpan? GetBestTime(List<Village> villages)
        {
            TimeSpan? timeSpan = null;
            foreach (Village village in villages)
            {
                foreach (Building building in village.Buildings)
                {
                    if (buildOrder.Contains(building.Name))
                    {
                        if (building.TimeToCanBuild != TimeSpan.Zero)
                        {
                            if (!timeSpan.HasValue)
                            {
                                timeSpan = building.TimeToCanBuild;
                            }
                            else
                            {
                                if (building.TimeToCanBuild < timeSpan.Value)
                                {
                                    timeSpan = building.TimeToCanBuild;
                                }
                            }
                        }
                    }
                   
                }

            }
            return timeSpan;
        }
    }
}

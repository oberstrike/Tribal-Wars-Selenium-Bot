using SQLiteApplication.UserData;
using SQLiteApplication.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SQLiteApplication
{
    internal class Program
    {
        public static List<string> BuildOrder { get; set; }


        public static void Main(string[] args)
        {
            Console.WriteLine("Starte test 7e");
            string configPath = @"Config.json";
            int counter = 0;

            BuildOrder = new List<string>();
            BuildOrder.Add("wood");
            BuildOrder.Add("iron");
            BuildOrder.Add("stone");
            int botCounter = 0;


            ConfigurationManager manager = new ConfigurationManager(configPath);
            Configuration configuration = manager.Configuration;

            while (botCounter < 2)
            {
                Client client = null;
                try
                {
                    client = new Client(configuration);
                    client.Connect();
                    client.Login();
                    client.Update();
                    Console.WriteLine("Update abgeschlossen");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    botCounter++;
                    continue;
                }

                TimeSpan? timeSpan = null;
                foreach (var village in configuration.User.Villages)
                {
                    ComputeVillage(counter, ref timeSpan, village);
                }

                counter++;
                client.Logout();
                client.Close();


                Console.WriteLine("Schlafe für " + timeSpan);
                Console.WriteLine("Bis: " + DateTime.Now.Add(timeSpan.Value));
                Thread.Sleep(timeSpan.Value);

            }
            Console.WriteLine("Botschutz detected");


        }

        private static void ComputeVillage(int counter, ref TimeSpan? timeSpan, Village village)
        {
            TimeSpan? localTimeSpan = GetBestTime(village);
            if (!localTimeSpan.HasValue)
            {
                localTimeSpan = new TimeSpan(0, 0, 30);
            }
            else
            {
                double value = localTimeSpan.Value.Ticks * 0.75D;
                long v = (long)value;
                if (counter % 2 == 0)
                    timeSpan = new TimeSpan(v);
            }
            if (!timeSpan.HasValue)
            {
                if (localTimeSpan < timeSpan)
                    timeSpan = localTimeSpan;
            }
        }

        private static TimeSpan? GetBestTime(Village village)
        {
            if (BuildOrder.Count > 0)
            {
                IEnumerable<Building> buildings = village.Buildings.Where(each => each.IsBuildeable && BuildOrder.Contains(each.Name)).Select(each => each);
                while (buildings.Count() > 0)
                {
                    village.Build(buildings.First());
                    buildings = village.Buildings.Where(each => each.IsBuildeable && BuildOrder.Contains(each.Name));

                }
            }

            return village.Buildings.Where(each => each.TimeToCanBuild != TimeSpan.Zero && BuildOrder.Contains(each.Name)).Select(each => each.TimeToCanBuild).Min();
        }



        public static void PrintBuildOrder()
        {
            Console.WriteLine("BuildOrder: ");
            Console.Write("\n[ ");
            foreach (string build in BuildOrder)
            {
                Console.Write(build + " ");
            }
            Console.WriteLine("]\n");
        }

    }




}

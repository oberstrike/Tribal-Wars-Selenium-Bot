
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using SQLiteApplication.PagesData;
using SQLiteApplication.UserData;
using SQLiteApplication.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SQLiteApplication
{
    internal class Program
    {
        public static List<string> BuildOrder { get; set; }


        public static void Main(string[] args)
        {
            Console.WriteLine("Starte test 7d" );
            string configPath = @"Config.json";
          
            BuildOrder = new List<string>();
            BuildOrder.Add("wood");
    //        BuildOrder.Add("iron");
            BuildOrder.Add("stone");
            int botCounter = 0;


            ConfigurationManager manager = new ConfigurationManager(configPath);
            Configuration configuration = manager.Configuration;

            while (botCounter < 2)
            {
                Client client = null;

                client = new Client(configuration);
                client.Connect();
                client.Login();
                client.Update();
                Console.WriteLine("Update abgeschlossen");
 
                var village = configuration.User.Villages.First();

                TimeSpan? timeSpan = GetBestTime(village);
                if (!timeSpan.HasValue)
                {
                    timeSpan = new TimeSpan(new Random().Next(1, 4), 0, 0);
                }

                client.Logout();
                client.Close();
                

                Console.WriteLine("Schlafe für " + timeSpan);
                Console.WriteLine("Bis: " + DateTime.Now.Add(timeSpan.Value));
                Thread.Sleep(timeSpan.Value);
            
            }
            Console.WriteLine("Botschutz detected");
            
         
        }

        private static TimeSpan? GetBestTime(Village village)
        {
            if (BuildOrder.Count > 0)
            {
                var buildings = village.Buildings.Where(each => each.IsBuildeable && BuildOrder.Contains(each.Name)).Select(each => each);
                while(buildings.Count() > 0)
                {
                    village.Build(buildings.First());
                    buildings = village.Buildings.Where(each => each.IsBuildeable && BuildOrder.Contains(each.Name));

                }
            }

            return village.Buildings.Where(each => each.TimeToCanBuild != null && BuildOrder.Contains(each.Name)).Select(each => each.TimeToCanBuild).Min();
        }

       

        public static void PrintBuildOrder()
        {
            Console.WriteLine("BuildOrder: ");
            Console.Write("\n[ ");
            foreach (var build in BuildOrder)
            {
                Console.Write(build + " ");
            }
            Console.WriteLine("]\n");
        }

    }




}

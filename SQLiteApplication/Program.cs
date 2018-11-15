
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




        static void Main(string[] args)
        {

            string configPath = @"C:\Users\oberstrike\source\repos\Project_Uno\config.json";

            ConfigurationManager manager = new ConfigurationManager(configPath);
            Configuration configuration = manager.Configuration;


            Village village = new Village(configuration.Village.MaxBuildings);
            configuration.Village = village;
           

            Client client = new AdvancedClient(@"G:\GeckoDriver", configuration);
            client.Connect();
            client.Login();

            client.Farm();

            Console.Read();
            return;
    


            Thread.Sleep(1000);


            if (configuration.IsGreedyOnRessources)
            {
                var buildings = village.Buildings.Where(value => value.Level < configuration.Village.MaxBuildings[value.Name]).ToList();


                var mainLevel = buildings.Where(value => value.Name == "main").First().Level;


                var ressisBuilding = buildings.Where(value => value.Name == "wood" || value.Name == "iron" || value.Name == "stone").ToList();
                                

                foreach(var b in ressisBuilding)
                {
                    Console.WriteLine(b);
                }
            }



            foreach(var move in configuration.Village.TroupMovements)
            {
                Console.WriteLine(move);
            }


            Thread.Sleep(1000);
            client.Logout();

            Console.Read();


            return;


            
        }
    }
}

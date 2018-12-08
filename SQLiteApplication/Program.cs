
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
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
            Console.WriteLine("Starte test 6v" );
            string configPath = @"Config.json";
            Task task = null;
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
                catch
                {
                    botCounter++;
                    continue;
                }
                
                var village = configuration.User.Villages.First();

                TimeSpan? timeSpan = null;

                if (BuildOrder.Count > 0)
                {
                    foreach (var building in village.Buildings)
                    {
                        if (BuildOrder.Contains(building.Name))
                        {
                            Console.WriteLine(building);
                            if (building.IsBuildeable)
                            {
                                village.Build(building);
                                break;
                            }
                            else
                            {
                                if (!timeSpan.HasValue)
                                {
                                    timeSpan = building.TimeToCanBuild.Add(new TimeSpan(0, 1, 0));
                                }
                                else if(timeSpan < building.TimeToCanBuild) 
                                {
                                    timeSpan = building.TimeToCanBuild;
                                }
                         
                            }

                        }
                    }
                }
                if (!timeSpan.HasValue)
                {
                    timeSpan = new TimeSpan(new Random().Next(1,4), 0, 0);
                }
                
                client.Logout();
                client.Close();

                task = new Task(Program.Event);
                task.Start();

                Console.WriteLine("Schlafe für " + timeSpan);
                Console.WriteLine("Bis: " + DateTime.Now.Add(timeSpan.Value));
                Thread.Sleep(timeSpan.Value);
                task.Dispose();

                
            }
            Console.WriteLine("Botschutz detected");
            
         
        }
        public static void Event()
        {
            while (true)
            {
                var input = Console.ReadLine();
                if(Configuration.BuildingList.Contains(input))
                {
                    BuildOrder.Add(input);
                }else if (input.Contains("remove"))
                {
                    var inputs = input.Split(' ');
                    if(inputs.Length == 2)
                    {
                        string value = inputs[1];
                        if (BuildOrder.Contains(inputs[1]))
                        {
                            int index = BuildOrder.LastIndexOf(value);
                            BuildOrder.RemoveAt(index);
                        }
                    }
                }
                PrintBuildOrder();
            }

        
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

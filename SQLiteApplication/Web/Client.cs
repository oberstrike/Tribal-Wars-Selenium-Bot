using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
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

    public class Client
    {
        private readonly List<string> urls = new List<string>() {"https://www.die-staemme.de/" };

        private IWebDriver Driver { get; set; }
        private bool _isConnected;
        private bool _isLoggedIn;
        private IJavaScriptExecutor _executor;
        private string _urlBase;
        private Configuration _configuration;
        private Farmmanager _farmmanager;
        private string _csrf;

        public Client(string driverPath, Configuration configuration)
        {
            Driver = new FirefoxDriver(driverPath);
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
            _executor = (IJavaScriptExecutor)Driver;
            _configuration = configuration;
            _farmmanager = new Farmmanager() { Templates = configuration.Templates };
            _urlBase = $"https://de{configuration.User.Server}.die-staemme.de/game.php?";

        }

        public void Connect()
        {
            try
            {
                Driver.Navigate().GoToUrl(urls[0]);       
                _isConnected = true;
            }
            catch(Exception e)
            {
                Console.WriteLine("Verbindung fehlgeschlagen.");
                Console.WriteLine(e.Message);
            }
        }

        public void Login()
        {
            if (_isConnected)
            {
                if (Driver.Url.Equals(urls[0]))
                {
                    Driver.FindElement(By.Id("user")).SendKeys(_configuration.User.Name);
                    Driver.FindElement(By.Id("password")).SendKeys(_configuration.User.Password);
                    Driver.FindElement(By.ClassName("btn-login")).Click();
                    Thread.Sleep(750);
                    Driver.FindElements(By.ClassName("world_button_active")).Where(each => each.Text.Contains(_configuration.User.Server.ToString())).First().Click();
                    Thread.Sleep(750);
                    if(Driver.Url != urls[0])
                    {
                        RefreshVillage();
                    }
                    _isLoggedIn = true;
                }

            }
    
        }

        private void RefreshVillage()
        {
            Thread.Sleep(500);
     
            var villageData = (Dictionary<string, object>)_executor.ExecuteScript("return TribalWars.getGameData().village");
            double id = 0;

            if(_configuration.Village.Id == 0)
            {
                try
                {
                    if (!double.TryParse((string)villageData["id"], out id))
                    {
                        id = (Int64)villageData["id"];
                    }
                }
                catch
                {
                    id = (Int64)villageData["id"];
                }
            }else
            {
                id = _configuration.Village.Id;
            }


            Driver.Navigate().Refresh();
            Thread.Sleep(500);
            Driver.Navigate().GoToUrl($"{_urlBase}village={id}&screen=main");
            Thread.Sleep(500);
            _configuration.Village.Buildings = GetBuildings((Dictionary<string, object>)_executor.ExecuteScript("return BuildingMain.buildings"));
            _configuration.Village.Id = id;
            _configuration.Village.Wood = (Int64)villageData["wood"];
            _configuration.Village.Iron = (Int64)villageData["iron"];
            _configuration.Village.Stone = (Int64)villageData["stone"];
            _csrf = (string)_executor.ExecuteScript("return csrf_token");
            Thread.Sleep(500);
            Driver.Navigate().GoToUrl($"{_urlBase}village={_configuration.User.Server}&screen=place&mode=command");
            foreach (var unit in _farmmanager.Units)
            {
                var count = double.Parse(Regex.Match(Driver.FindElements(By.Id($"units_entry_all_{unit}")).First().Text, @"\d+").Value);
                _configuration.Village.GetUnits()[unit] = count;

            }
        }

        public void Logout()
        {
            Driver.Navigate().GoToUrl($"{_urlBase}village={_configuration.Village.Id}&screen=&action=logout&h={_csrf}");
            _isLoggedIn = false;
        }

        public void Build(string buildingName)
        {
            if(_isConnected && _isLoggedIn)
            {
                Thread.Sleep(250);
                Driver.Navigate().GoToUrl($"{_urlBase}village={_configuration.Village.Id}&screen=main");
                _executor.ExecuteScript($"BuildingMain.build(\"{buildingName}\")");
            }
            Console.WriteLine("Ausbauen nicht möglich");
        }

        public void Farm()
        {
            string[] villages = new string[_configuration.FarmingVillages.Count()-1];
            System.Array.Copy(_configuration.FarmingVillages, villages, villages.Length);


          

            var templates = _configuration.Templates;
            
            foreach(var template in templates)
            {
                bool isPossible = true;

                foreach(var key in template.Keys)
                {
                    Console.WriteLine(template[key] + " " + _configuration.Village.GetUnits()[key]);
                    if(template[key] > _configuration.Village.GetUnits()[key])
                    {
                        isPossible = false;               
                    }
                    
                }
                Console.WriteLine(isPossible);
                if (isPossible)
                {
                    string goalVillage = _farmmanager.LastAttackedVillage;
                    if(goalVillage != null)
                    {
                        int count = 0;
                        while(goalVillage != _farmmanager.LastAttackedVillage)
                        {
                            Console.WriteLine(count);
                            if(count < villages.Length)
                            {
                                goalVillage = villages[count];
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        goalVillage = _configuration.FarmingVillages[0];
                    }
                    Console.WriteLine("Goal: " + goalVillage);
                    if(goalVillage != null)
                    {
                        Attack(template, goalVillage);
                    }
                }
                RefreshVillage();

            }



        }

        private void Attack(Dictionary<string, double> dictionary, string target)
        {
            Driver.Navigate().GoToUrl($"{_urlBase}village={_configuration.Village.Id}&screen=place&target={target}");
            foreach(var kvp in dictionary)
            {
                Driver.FindElement(By.Id("unit_input_" + kvp.Key)).SendKeys(kvp.Value.ToString());
                Thread.Sleep(500);
            }
            Console.WriteLine("Ich greife " + target + " an");
            Driver.FindElement(By.Id("target_attack")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.Id("troop_confirm_go")).Click();

        }

        private List<Building> GetBuildings(Dictionary<string, object> keyValuePairs)
        {
            List<Building> newBuildings = new List<Building>();
            foreach (var key in keyValuePairs.Keys)
            {
                var dictionary = (Dictionary<string, object>)keyValuePairs[key];
                newBuildings.Add(new BuildingBuilder()
                    .WithName(key)
                    .WithWood((Int64)dictionary["wood"])
                    .WithStone((Int64)dictionary["stone"])
                    .WithIron((Int64)dictionary["iron"])
                    .WithLevel(double.Parse((string)dictionary["level"]))
                    .WithPopulation((Int64)dictionary["pop"])
                    .WithMaxLevel((Int64)dictionary["max_level"])
                    .WithTargetLevel(_configuration.Village.MaxBuildings[key])
                    .WithBuildeable(!dictionary.ContainsKey("error"))
                    .Build());
            }
            return newBuildings;
        }

        public bool IsConnected => _isConnected;

        public bool IsLoggedIn => _isLoggedIn;
    }
}

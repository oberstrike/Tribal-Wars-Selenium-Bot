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
    public abstract class Client
    {
        private IJavaScriptExecutor _executor;
        private Farmmanager _farmmanager;
        private bool _isConnected;
        private bool _isLoggedIn;
        private PathCreator _creator;
        private string _csrf;
        private readonly List<string> urls = new List<string>() { "https://www.die-staemme.de/" };

        public FirefoxDriver Driver { get; }

        public Client(string driverPath, Configuration configuration)
        {
            Driver = new FirefoxDriver(driverPath);
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
            Executor = (IJavaScriptExecutor)Driver;
            Config = configuration;
            Farmmanager = new Farmmanager() { Templates = configuration.Templates };

        }

        public void Connect()
        {
            try
            {
                Driver.Navigate().GoToUrl(urls[0]);
                IsConnected = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Verbindung fehlgeschlagen.");
                Console.WriteLine(e.Message);
            }
        }

        public void Login()
        {
            if (IsConnected)
            {
                if (Driver.Url.Equals(urls[0]))
                {
                    Driver.FindElement(By.Id("user")).SendKeys(Config.User.Name);
                    Driver.FindElement(By.Id("password")).SendKeys(Config.User.Password);
                    Driver.FindElement(By.ClassName("btn-login")).Click();
                    Thread.Sleep(750);
                    Driver.FindElements(By.ClassName("world_button_active")).Where(each => each.Text.Contains(Config.User.Server.ToString())).First().Click();
                    Thread.Sleep(750);
                    if (Driver.Url != urls[0])
                    {
                        UpdateVillage();
                    }
                    IsLoggedIn = true;
                }

            }

        }

        public List<Building> GetBuildings(Dictionary<string, object> keyValuePairs)
        {
            List<Building> newBuildings = new List<Building>();
            foreach (var key in keyValuePairs.Keys)
            {
                var dictionary = (Dictionary<string, object>)keyValuePairs[key];
                try
                {
                    newBuildings.Add(new BuildingBuilder()
                                .WithName(key)
                                .WithWood((Int64)dictionary["wood"])
                                .WithStone((Int64)dictionary["stone"])
                                .WithIron((Int64)dictionary["iron"])
                                .WithLevel(double.Parse((string)dictionary["level"]))
                                .WithPopulation((Int64)dictionary["pop"])
                                .WithMaxLevel((Int64)dictionary["max_level"])
                                .WithTargetLevel(Config.Village.MaxBuildings[key])
                                .WithBuildeable(!dictionary.ContainsKey("error"))
                                .Build());

                }catch(Exception e)
                {
                    Console.WriteLine(key + " wurde nicht gefunden");
                }



            }
            return newBuildings;
        }


        public void Logout()
        {
            Driver.Navigate().GoToUrl(Creator.GetLogout(Csrf));
            IsLoggedIn = false;
        }
        public abstract void Farm();

        protected void UpdateVillage()
        {
            Console.WriteLine("Aktualisiere Das Dorf");
            Thread.Sleep(500);
            RefreshRessis();
            Thread.Sleep(500);
            UpdateTroops();
            Thread.Sleep(500);
            UpdateTroupMovement();
            Thread.Sleep(500);

        }

        public virtual void UpdateTroops()
        {
            Driver.Navigate().GoToUrl(Creator.GetPlace());
            foreach (var unit in Farmmanager.Units)
            {
                var count = double.Parse(Regex.Match(Driver.FindElements(By.Id($"units_entry_all_{unit}")).First().Text, @"\d+").Value);
                Config.Village.GetUnits()[unit] = count;

            }
        }

        public void Build(string buildingName)
        {
            if (IsConnected && IsLoggedIn)
            {
                Thread.Sleep(250);
                if (Driver.Url != Creator.GetMain())
                {
                    Driver.Navigate().GoToUrl(Creator.GetMain());
                }

                Executor.ExecuteScript($"BuildingMain.build(\"{buildingName}\")");
            }
            Console.WriteLine("Ausbauen nicht möglich");
        }
        public Farmmanager Farmmanager { get => _farmmanager; set => _farmmanager = value; }
        public bool IsConnected { get => _isConnected; set => _isConnected = value; }
        public bool IsLoggedIn { get => _isLoggedIn; set => _isLoggedIn = value; }
        internal PathCreator Creator { get => _creator; set => _creator = value; }
        public string Csrf { get => _csrf; set => _csrf = value; }
        public Configuration Config { get; set; }
        public IJavaScriptExecutor Executor { get => _executor; set => _executor = value; }

        protected void Attack(Dictionary<string, double> units, string target)
        {
            Driver.Navigate().GoToUrl(Creator.GetAttackLink(target));
            foreach (var kvp in units)
            {
                Driver.FindElement(By.Id("unit_input_" + kvp.Key)).SendKeys(kvp.Value.ToString());
                Thread.Sleep(500);
            }
            Console.WriteLine("Ich greife " + target + " an");
            Driver.FindElement(By.Id("target_attack")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.Id("troop_confirm_go")).Click();

        }

        protected void RefreshRessis()
        {
            double id = 0;
            var villageData = (Dictionary<string, object>)Executor.ExecuteScript("return TribalWars.getGameData().village");
            id = GetVillageId();

            if (Config.Village.Id != 0)
            {
            }
            else
            {
                id = Config.Village.Id;
            }
      

            Thread.Sleep(500);
            Driver.Navigate().GoToUrl(Creator.GetMain());
            Thread.Sleep(500);
            Config.Village.Buildings = GetBuildings((Dictionary<string, object>)Executor.ExecuteScript("return BuildingMain.buildings"));
            Config.Village.Id = id;
            Config.Village.Wood = (Int64)villageData["wood"];
            Config.Village.Iron = (Int64)villageData["iron"];
            Config.Village.Stone = (Int64)villageData["stone"];
            Csrf = (string)Executor.ExecuteScript("return csrf_token");
        }

        public double GetVillageId()
        {
            double id = 0;
            if (IsLoggedIn && IsConnected)
            {
                Thread.Sleep(500);
                var output = (Executor.ExecuteScript("return TribalWars.getGameData().village.id"));

                if (typeof(string).Equals(output.GetType()))
                {
                    double.TryParse((string)output, out id);
                }
                else
                {
                    id = (Int64)output;
                }
            }
            Creator = new PathCreator(Config.User.Server.ToString(), id.ToString());
            return id;
            
        }

        public void UpdateTroupMovement()
        {
            Driver.Navigate().GoToUrl(Creator.GetPlace());
            string tr_Class = "command-row";
            Thread.Sleep(500);
            try
            {
                var rows = Driver.FindElement(By.Id("commands_outgoings")).FindElements(By.ClassName(tr_Class));
                ICollection<TroupMovement> movements = new List<TroupMovement>();

                foreach (var row in rows)
                {
                    var attackType = (string)row.FindElement(By.ClassName("command_hover_details")).GetAttribute("data-command-type");
                    var targetElement = row.FindElement(By.ClassName("quickedit-out"));
                    var movementId = targetElement.GetAttribute("data-id");

                    movements.Add(new TroupMovement() { Type = attackType, MovementId = movementId });

                    Thread.Sleep(500);
                }

                foreach (var movement in movements)
                {
                    var d = Driver.FindElement(By.CssSelector($".quickedit-out[data-id='{movement.MovementId}']"));


                    if (movements.Where(move => move.MovementId == d.GetAttribute("")).Count() == 0)
                    {
                        d.Click();
                        var id = Driver.FindElement(By.CssSelector($".village_anchor.contexted[data-player='0']")).GetAttribute("data-id");
                        movement.TargetId = id;
                        Thread.Sleep(500);
                        Driver.Navigate().GoToUrl(Creator.GetPlace());
                    }
                }
                Config.Village.TroupMovements = movements;
            }
            catch
            {
                Console.WriteLine("Keine Truppenbewegungen entdeckt");
            }
        }

        public IList<string> GetNotAttackedVillages()
        {
            var villages = Config.FarmingVillages;
            var movements = Config.Village.TroupMovements;
            List<string> targets = new List<string>();



            if (villages != null)
            {
                if(movements != null)
                {
                    var aggregate = movements.Select(x => x.TargetId).ToList();
                    if (movements.Count > 0 && villages.Length > 0)
                    {
                        foreach(var target in villages)
                        {
                            
                            if (!aggregate.Contains(target))
                            {
                                targets.Add(target);
                            }

                        }

                    }
                }
                else
                {
                    return villages;
                }

            }

            return targets;
        }
    }
}

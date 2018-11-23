using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using SQLiteApplication.UserData;
using SQLiteApplication.VillageData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SQLiteApplication.Web
{
    public class Client
    {
        public static void Sleep()
        {
            Thread.Sleep((new Random().Next(1, 5) * 1000) + 245);
        }

        private IJavaScriptExecutor _executor;
        private Farmmanager _farmmanager;
        private bool _isConnected;
        private bool _isLoggedIn;
        private PathCreator _creator;
        private string _csrf;
        private readonly List<string> urls = new List<string>() { "https://www.die-staemme.de/" };
        public Process TorProcess { get; set; }
        private FirefoxOptions options;

        public FirefoxDriver Driver { get; set; }

        public Client( Configuration configuration)
        {
            Config = configuration;
            options = new FirefoxOptions();
            options.AddArgument("--headless");
            if (configuration.TorBrowserPath != null)
            {
                ConfigureAdvancedBrowser();

            }
        }

        private void ConfigureAdvancedBrowser()
        {
            var localIds = Process.GetProcessesByName("tor");

            if (localIds.Length == 0)
            {

                TorProcess = new Process();
                TorProcess.StartInfo.FileName = Config.TorBrowserPath;
                TorProcess.StartInfo.Arguments = " - n";
                TorProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                TorProcess.Start();
                Sleep();
            }
            FirefoxProfile profile = new FirefoxProfile();
            profile.SetPreference("network.proxy.type", 1);
            profile.SetPreference("network.proxy.socks", "127.0.0.1");
            profile.SetPreference("network.proxy.socks_port", 9150);
            options.Profile = profile;
        }

        public void Connect()
        {
            try
            {
                Driver = new FirefoxDriver(options);
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
                WebDriverWait webDriver = new WebDriverWait(this.Driver, TimeSpan.FromSeconds(60));
                Executor = (IJavaScriptExecutor)Driver;
      
                Farmmanager = new Farmmanager() { Templates = Config.Templates };
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
                if (!Driver.Url.Equals(urls[0]))
                {
                    Driver.Navigate().GoToUrl(urls[0]);
                }
                var contains = Driver.PageSource.Contains(Config.User.Name);
                if (!contains)
                {
                    Driver.FindElement(By.Id("user")).SendKeys(Config.User.Name);
                    Driver.FindElement(By.Id("password")).SendKeys(Config.User.Password);
                    Driver.FindElement(By.ClassName("btn-login")).Click();
                    Sleep();
                }


                    Driver.FindElements(By.ClassName("world_button_active")).Where(each => each.Text.Contains(Config.User.Server.ToString())).First().Click();
                    Sleep();
                    if (Driver.Url != urls[0])
                    {
                        UpdateVillage();
                    }
                    IsLoggedIn = true;
                

            }

        }

        public List<Building> GetBuildings(Dictionary<string, object> keyValuePairs)
        {
      
            List<Building> newBuildings = new List<Building>();
            foreach (var key in keyValuePairs.Keys)
            {            
                var dictionary = (Dictionary<string, object>)keyValuePairs[key];

                string text = null;
                DateTime? dateTime = null;
                if (dictionary.ContainsKey("error"))
                {
                    text = (string)dictionary["error"];
                    if (text != null)
                    {
                        if (text.Length > 1 && text.Contains("Genug") && text.Contains("um"))
                        {
                            var date = text.Split(' ')[4];

                            dateTime = DateTime.Parse(date);
                            var nowTime = DateTime.Now;

                            if (dateTime.Value.Ticks < nowTime.Ticks)
                            {
                                dateTime = dateTime.Value.AddDays(1).AddMinutes(2);
                            }
                        }
                    }

                }
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
                                .WithBuildeable(text == null)
                                .WithBuildingTime(dateTime)
                                .Build());

                }catch
                {
                    Console.WriteLine(key + " wurde nicht gefunden");Thread.Sleep((new Random().Next(1, 5) * 1000) + 245);
                }



            }
            return newBuildings;
        }
        public void Logout()
        {
            Driver.Navigate().GoToUrl(Creator.GetLogout(Csrf));
            IsLoggedIn = false;
        }

        public void Close()
        {
            Driver.Close();
        }

        protected virtual void UpdateVillage()
        {
            Sleep();
            RefreshRessis();
            Sleep();
            UpdateTroupMovement();
            Sleep();
            UpdateBuildingQueue();
            Sleep();
            UpdateMarket();
            Sleep();
            UpdateTroops();
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
                Sleep();
                GoToMain();

                Executor.ExecuteScript($"BuildingMain.build(\"{buildingName}\")");
                Sleep();
                RefreshRessis();
                return;
            }
            
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
                Sleep();
            }
            Driver.FindElement(By.Id("target_attack")).Click();
            Sleep();
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
            GoToMain();
            Sleep();
            Config.Village.Buildings = GetBuildings((Dictionary<string, object>)Executor.ExecuteScript("return BuildingMain.buildings"));
            Config.Village.Id = id;
            Config.Village.Wood = (Int64)villageData["wood"];
            Config.Village.Iron = (Int64)villageData["iron"];
            Config.Village.Stone = (Int64)villageData["stone"];
            Csrf = (string)Executor.ExecuteScript("return csrf_token");
        }

        private void GoToMain()
        {
            if (Driver.Url != Creator.GetMain())
            {
                Sleep();
                Driver.Navigate().GoToUrl(Creator.GetMain());
            }
        }

        private void GoToBarracks()
        {
            if (Driver.Url != Creator.GetBarracks())
            {
                Sleep();
                Driver.Navigate().GoToUrl(Creator.GetBarracks());
            }
        }

        private void GoToMarket()
        {
            if (Driver.Url != Creator.GetMarketModeSend())
            {
                Sleep();
                Driver.Navigate().GoToUrl(Creator.GetMarketModeSend());
            }
        }

        public double GetVillageId()
        {
            double id = 0;
            if (IsLoggedIn && IsConnected)
            {
                Sleep();
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
            Sleep();
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

                    Sleep();
                }

                foreach (var movement in movements)
                {
                    var d = Driver.FindElement(By.CssSelector($".quickedit-out[data-id='{movement.MovementId}']"));


                    if (movements.Where(move => move.MovementId == d.GetAttribute("")).Count() == 0)
                    {
                        d.Click();
                        var id = Driver.FindElements(By.XPath("//*[@data-player]")).ToArray()[1].GetAttribute("data -id");
                        movement.TargetId = id;
                        Sleep();
                        Driver.Navigate().GoToUrl(Creator.GetPlace());
                    }
                }
                Config.Village.OutcomingTroops = movements;
            }
            catch
            {
                
            }
        }

        public IList<string> GetNotAttackedVillages()
        {
            var villages = Config.FarmingVillages;
            var movements = Config.Village.OutcomingTroops;
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

        public void UpdateBuildingQueue()
        {
            GoToMain();
            try
            {
                var firstElement = Driver.FindElement(By.XPath("//tr[contains(@class, 'lit nodrag buildorder_')]"));
                var nextElements = Driver.FindElements(By.XPath("//tr[contains(@id, 'buildorder_')]"));

                if (firstElement != null)
                {
                    var currentQueue = firstElement.FindElement(By.XPath("//img[@class='bmain_list_img']")).GetAttribute("title");
                    var currentTime = DateTime.Parse(Driver.FindElement(By.XPath("//tbody[@id='buildqueue']//td[contains(@class, 'nowrap')]")).Text);
                    Config.Village.BuildingsInQueue = new KeyValuePair<string, DateTime>(currentQueue, currentTime);


                    /*
                     * Experimantal
                     *
                     */

                    var web = Driver.FindElements(By.XPath("//tr[contains(@id, 'buildorder')]"));
                    foreach (var element in web)
                    {
                        var innerElement = element.FindElement(By.XPath("//img[contains(@class, 'bmain_list_img')]"));
                        string title = innerElement.GetAttribute("title");
    

                    }
                }
            }
            catch
            {

            }
        }

        public void UpdateMarket()
        {
            GoToMarket();
            Sleep();
            Config.Village.HaendlerCount = int.Parse(Driver.FindElement(By.Id("market_merchant_available_count")).Text);
   

        }

        public void SendRessource(int wood, int stone, int iron, string targetId)
        {
            GoToMarket();

            var woodInput = Driver.FindElement(By.XPath("//input[@name='wood']"));
            var stoneInput = Driver.FindElement(By.XPath("//input[@name='stone']"));
            var ironInput = Driver.FindElement(By.XPath("//input[@name='iron']"));
            var targetInput = Driver.FindElement(By.XPath("//input[@placeholder='123|456']"));

            woodInput.SendKeys(wood.ToString());
            stoneInput.SendKeys(stone.ToString());
            ironInput.SendKeys(iron.ToString());
            targetInput.SendKeys(targetId);
           
            UpdateMarket();

        }

        public void TrainUnits(Dictionary<string, double> units)
        {
            GoToBarracks();
            
            var spearsInput = Driver.FindElementByXPath("//input[@id='spear_0']");
            var swordInput = Driver.FindElementByXPath("//input[@id='sword_0']");
            var axeInput = Driver.FindElementByXPath("//input[@id='axe_0']");
            var trainBtn = Driver.FindElementByXPath(".btn.btn-recruit");

            if (units.ContainsKey("spears"))
            {
                spearsInput.SendKeys(units["spears"].ToString());
            }
            if (units.ContainsKey("sword"))
            {
                spearsInput.SendKeys(units["sword"].ToString());
            }
            if (units.ContainsKey("axe"))
            {
                spearsInput.SendKeys(units["axe"].ToString());
            }

            trainBtn.Click();
            Sleep();


        }
    }
}

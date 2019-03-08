using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using TWLibrary.Page;
using TWLibrary.Tools;
using TWLibrary.UserData;
using TWLibrary.VillageData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace TWLibrary.Web
{
    public class Client
    {

        #region STATIC
        public static void Sleep()
        {
            Randomizer random = new Randomizer();
            int delay = (random.Next(1, 3) * 1000) + random.Next(1, 13) * 19;
            Task.Delay(delay).Wait();
        }

        public static void Print(object input)
        {
            new TaskFactory().StartNew(() => Console.WriteLine(DateTime.Now + ": " + input.ToString()));
        }
        #endregion

        #region Properties
        private readonly string _mainURL = "https://www.die-staemme.de/";
        private FirefoxOptions firefoxOptions;
        private ChromeOptions chromeOptions;
        public bool IsConnected { get; set; }
        public bool IsLoggedIn { get; set; }
        public List<IPlugin> Plugins { get; set; } = new List<IPlugin>();
        public Process TorProcess { get; set; }
        public IWebDriver Driver { get; set; }
        public User User { get; set; }
        #endregion

        #region METHODS
        public void Update()
        {
            foreach (Village village in User.Villages)
            {
                Print("Starte Update von " + village.Name);
                village.Update();
                Print("Update wurde beendet von " + village.Name);
            }

            foreach (IVillagesPlugin plugin in Plugins)
            {
                plugin.Compute(User.Villages);
            }
        }
        public Building GetBuildingWithShortestTimeToBuild(List<Building> buildings)
        {
            Building returnValue = null;

            foreach (Building building in buildings)
            {
                if (returnValue == null)
                {
                    returnValue = building;
                }
                else if (returnValue.TimeToCanBuild > building.TimeToCanBuild)
                {
                    returnValue = building;
                }

            }

            return returnValue;

        }
        public IEnumerable<Building> GetBuildeableBuildings()
        {
            return User.Villages.SelectMany(each => each.GetBuildingsInBuildOrder());

        }
        public TimeSpan? GetBestTimeToCanBuild()
        {

            foreach (Village village in User.Villages)
            {
                if (village.BuildingsInQueue == null)
                    break;

                var sum = new TimeSpan(village.BuildingsInQueue.Sum(each => each.Value.Ticks));


            }
            if (GetBuildeableBuildings() == null)
                return null;


            return GetBuildeableBuildings().Select(each => each.TimeToCanBuild).Min();
        }
        public TimeSpan? GetBestTimeForQueue()
        {
            var a = User.Villages.Select(each => new TimeSpan(each.BuildingsInQueue.Sum(time => time.Value.Ticks))).Where(each => each != TimeSpan.Zero).Select(each => each);
            return a.Min();

        }
        public Client(User user)
        {
            User = user;

            firefoxOptions = new FirefoxOptions();
   //         firefoxOptions.AddArgument("--headless");

            //Experimental
            firefoxOptions.LogLevel = FirefoxDriverLogLevel.Error;
            


            if (User == null)
                return;

            if (User.TorBrowserPath != null)
            {
                ConfigureAdvancedBrowser();
            }
        }
        private void ConfigureAdvancedBrowser()
        {
            Process[] localIds = Process.GetProcessesByName("tor");

            if (localIds.Length == 0)
            {

                TorProcess = new Process();
                TorProcess.StartInfo.FileName = User.TorBrowserPath;
                TorProcess.StartInfo.Arguments = " - n";
                TorProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                TorProcess.Start();
                Sleep();
            }
            FirefoxProfile profile = new FirefoxProfile();
            profile.SetPreference("network.proxy.type", 1);
            profile.SetPreference("network.proxy.socks", "127.0.0.1");
            profile.SetPreference("network.proxy.socks_port", 9150);
           
            firefoxOptions.Profile = profile;
            Client.Print("Starte versteckten Client.");

        }
        public void Connect()
        {
            try
            {
                Driver = GetFirefoxDriver();
                Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(25);
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                Driver.Navigate().GoToUrl(_mainURL);
                IsConnected = true;
            }
            catch (Exception e)
            {
                Client.Print("Verbindung fehlgeschlagen.");
                Client.Print(e.Message);
            }
        }

        private IWebDriver GetChromeDriver()
        {
            chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            return new ChromeDriver(chromeOptions);
        }

        private IWebDriver GetFirefoxDriver()
        {
            return new FirefoxDriver(firefoxOptions);

        }


        public void Login()
        {
            if (IsConnected)
            {
                if (!Driver.Url.Equals(_mainURL))
                {
                    Driver.Navigate().GoToUrl(_mainURL);
                }
                bool contains = Driver.PageSource.Contains(User.Name);
                if (!contains)
                {
                    Driver.FindElement(By.Id("user")).SendKeys(User.Name);
                    Driver.FindElement(By.Id("password")).SendKeys(User.DecryptPassword());
                    Sleep();
                    Driver.FindElement(By.ClassName("btn-login")).Click();
                    Sleep();
                }

                try
                {
                    Driver.FindElements(By.ClassName("world_button_active")).Where(each => each.Text.Contains(User.Server.ToString())).First().Click();
                }
                catch (Exception e)
                {
                    Print(e.Message);
                    Print("Einloggbutton wurde nicht gefunden");
                }

                Sleep();
                if (Driver.Url != _mainURL)
                {
                    var userData = (bool)Driver.ExecuteScript("return TribalWars.getGameData().features[\"Premium\"].active");

                    User.IsPremium = userData;

                    User.Villages = FindVillagesInOverviewPage();
                }
                IsLoggedIn = true;


            }

        }
        internal virtual List<Village> FindVillagesInOverviewPage()
        {
            KeyValuePair<double[], string[]> keyValuePair = GetVillageIds();
            double[] ids = keyValuePair.Key;
            string[] names = keyValuePair.Value;

            List<Village> villages = new List<Village>();

            for (int i = 0; i < ids.Length; i++)
            {
                Village village = Factory.GetVillage(ids[i], User.Server, Driver, User);
                village.Name = names[i];
                village.FarmingVillages = User.FarmingVillages;
                villages.Add(village);

            }
            Sleep();
            return villages;
        }
        private KeyValuePair<double[], string[]> GetVillageIds()
        {
            Sleep();
            GoTo(PathCreator.GetOverview(User.Server.ToString()));
            Sleep();
            IWebElement trTag = Driver.FindElement(By.XPath("//tr[contains(@class,'nowrap')]"));

            IReadOnlyList<IWebElement> idElements = trTag.FindElements(By.XPath("//span[@class='quickedit-vn']"));
            IReadOnlyList<IWebElement> nameElements = trTag.FindElements(By.XPath("//span[@class='quickedit-label']"));
            double[] dArray = new double[idElements.Count];
            string[] sArray = new string[idElements.Count];
            for (int i = 0; i < dArray.Length; i++)
            {
                var name = nameElements[i].GetAttribute("data-text");
                dArray[i] = double.Parse(idElements[i].GetAttribute("data-id"));
                sArray[i] = name;
            }
            return new KeyValuePair<double[], string[]>(dArray, sArray);
        }
        public void Logout()
        {
            Driver.Navigate().GoToUrl(PathCreator.GetLogout(User.Server.ToString()));
            IsLoggedIn = false;
        }
        public void Close()
        {
            Driver.Close();
        }
        public void GoTo(string url)
        {
            Driver.GoTo(url);
        }
        #endregion
    }
}

using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.Page;
using SQLiteApplication.Tools;
using SQLiteApplication.UserData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SQLiteApplication.Web
{
    public class Client
    {

        #region STATIC

        public static void Sleep()
        {
            Thread.Sleep((new Random().Next(1, 2) * 1000) + new Random().Next(1, 10) * 10);
        }
        #endregion

        private readonly List<string> urls = new List<string>() { "https://www.die-staemme.de/" };

        private FirefoxOptions options;
        public List<string> BuildOrder { get; set; } = new List<string>() { "stone", "wood", "iron" };
        #region Properties
        public bool IsConnected { get; set; }
        public bool IsLoggedIn { get; set; }
        public Configuration Config { get; set; }
        public void Update()
        {
            foreach (Village village in Config.User.Villages)
            {
                Update(village);
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
            return Config.User.Villages.SelectMany(each => each.Buildings).Where(each => BuildOrder.Contains(each.Name) && each.TimeToCanBuild != TimeSpan.Zero).Select(each => each);

        }


        public TimeSpan? GetBestTime()
        {
            return GetBuildeableBuildings().Select(each => each.TimeToCanBuild).Min();
        }
        public Process TorProcess { get; set; }
        public Timer MyTimer { get; set; }

        public void Update(Village village)
        {
            Console.WriteLine(DateTime.Now + " Update Village: " + village.Id);
            village.Update();
            UpgradeBuildings(village);

        }

        public void UpgradeBuildings(Village village)
        {
            IEnumerable<Building> buildingsToUpgrade = default(IEnumerable<Building>);
            do
            {
                buildingsToUpgrade = village.Buildings.Where(each =>
                {

                    return BuildOrder.Contains(each.Name) & each.IsBuildeable & each.Level < each.MaxLevel;
                }
                );
                if (buildingsToUpgrade != null && buildingsToUpgrade.Count() > 0)
                {
                    Console.WriteLine(DateTime.Now + " Building: " + buildingsToUpgrade.First().Name + " in " + village.Id);
                    village.Build(buildingsToUpgrade.First());
                    MainPage mainPage = (MainPage)village.Pages.Where(each => each is MainPage).First();
                    mainPage.Update();
                    break;
                }
            } while (buildingsToUpgrade != default(IEnumerable<Building>) && buildingsToUpgrade.Count() > 0d);



        }

        public FirefoxDriver Driver { get; set; }
        #endregion
        public Client(Configuration configuration)
        {
            Config = configuration;
            options = new FirefoxOptions();

#if (!DEBUG)
                options.AddArgument("--headless");
#endif
            if (configuration.User.TorBrowserPath != null)
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
                TorProcess.StartInfo.FileName = Config.User.TorBrowserPath;
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
            Console.WriteLine("Starte versteckten Client.");

        }

        public void Connect()
        {
            try
            {
                Driver = new FirefoxDriver(options);
                Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(25);
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                Console.WriteLine("Starte Timer");

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
                bool contains = Driver.PageSource.Contains(Config.User.Name);
                if (!contains)
                {
                    Driver.FindElement(By.Id("user")).SendKeys(Config.User.Name);
                    Driver.FindElement(By.Id("password")).SendKeys(Config.User.Password);
                    Driver.FindElement(By.ClassName("btn-login")).Click();
                    Sleep();
                }

                try
                {
                    Driver.FindElements(By.ClassName("world_button_active")).Where(each => each.Text.Contains(Config.User.Server.ToString())).First().Click();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Einloggbutton wurde nicht gefunden");
                }

                Sleep();
                if (Driver.Url != urls[0])
                {
                    Config.User.Villages = GetVillages();
                }
                IsLoggedIn = true;


            }

        }

        private List<Village> GetVillages()
        {
            double[] ids = GetVillageIds();
            List<Village> villages = new List<Village>();
            for (int i = 0; i < ids.Length; i++)
            {
                Village village = new Village(ids[i], Config.User.Server, Driver);
                villages.Add(village);

            }
            Sleep();
            return villages;
        }

        private double[] GetVillageIds()
        {
            Sleep();
            GoTo(PathCreator.GetOverview(Config.User.Server.ToString()));
            Sleep();
            IWebElement trTag = Driver.FindElement(By.XPath("//tr[contains(@class,'nowrap')]"));

            IReadOnlyList<IWebElement> elements = trTag.FindElements(By.XPath("//span[@class='quickedit-vn']"));
            double[] dArray = new double[elements.Count];
            for (int i = 0; i < dArray.Length; i++)
            {
                dArray[i] = double.Parse(elements[i].GetAttribute("data-id"));
            }
            return dArray;
        }

        public void Logout()
        {
            Driver.Navigate().GoToUrl(PathCreator.GetLogout(Config.User.Server.ToString()));
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

    }
}

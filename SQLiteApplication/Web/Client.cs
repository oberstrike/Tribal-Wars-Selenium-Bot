using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using SQLiteApplication.Tools;
using SQLiteApplication.UserData;
using SQLiteApplication.VillageData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SQLiteApplication.Web
{
    public class Client
    {

        #region STATIC

        public static void Sleep()
        {
            Thread.Sleep((new Random().Next(1, 5) * 1000) + 245);
        }

        #endregion

        #region PRIVATE_ATTRIBUTES
        private Farmmanager _farmmanager;
        private bool _isConnected;
        private bool _isLoggedIn;
        private readonly List<string> urls = new List<string>() { "https://www.die-staemme.de/" };
        private FirefoxOptions options;
        #endregion

        #region Properties
        public Farmmanager Farmmanager { get => _farmmanager; set => _farmmanager = value; }
        public bool IsConnected { get => _isConnected; set => _isConnected = value; }

        public void Update()
        {
            foreach(var village in Config.User.Villages)
            {
                village.Update();
                Sleep();
            }
            
        }
        public bool IsLoggedIn { get => _isLoggedIn; set => _isLoggedIn = value; }
        public Configuration Config { get; set; }
        public FirefoxDriver Driver { get; set; }
        public Process TorProcess { get; private set; }
        #endregion


        public Client(Configuration configuration)
        {
            Config = configuration;
            options = new FirefoxOptions();

            #if (!DEBUG)
                options.AddArgument("--headless");
            #endif
            if (configuration.TorBrowserPath != null)
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
                options.SetLoggingPreference(LogType.Driver, LogLevel.Debug);

                FirefoxDriverService service = FirefoxDriverService.CreateDefaultService();
                Driver = new FirefoxDriver(service, options, TimeSpan.FromSeconds(10));
                Driver.Navigate().GoToUrl(urls[0]);
                
                if (Driver.Url != urls[0])
                {
                    throw new Exception("Fatal Error");
                }

                IsConnected = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Verbindung fehlgeschlagen.");
                Console.WriteLine(e.Message);
            }
        }
        public void Close()
        {
            Driver.Close();
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
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    Thread.Sleep(2000);
                }
                Sleep();
                if (Driver.Url != urls[0])
                {
                    WebDriverWait driverWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
                    GetVillages();

                }
                IsLoggedIn = true;


            }

        }
        public void Logout()
        {
            Village village = Config.User.Villages[0];
            Driver.Navigate().GoToUrl(village.Creator.GetLogout(village.Csrf));
            IsLoggedIn = false;
        }
        private void GetVillages()
        {
            List<double> ids = GetVillageIds();

            Config.User.Villages = new List<Village>();
           

            foreach (double id in ids)
            {
                Console.WriteLine(id);
                Village village = new Village(id.ToString(), Config.User.Server.ToString(), Driver, Config.User);
                village.Creator = new PathCreator(village);
                village.Csrf = (string) Driver.ExecuteScript("return TribalWars.getGameData().csrf");
                Config.User.Villages.Add(village);

            }


        }
        private List<double> GetVillageIds()
        {
            string path1 = $"https://de{Config.User.Server}.die-staemme.de/game.php?screen=overview_villages&mode=combined";
            string xpath1 = "//tr[contains(@class,'nowrap selected  row_a')]";

            string xpath2 = "//span[@class='quickedit-vn']";
            Driver.Navigate().GoToUrl(path1);
            Sleep();
            List<double> doubles = new List<double>();

            try
            {
                System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> elements = Driver.FindElements(By.XPath(xpath1));

                foreach (IWebElement element in elements)
                {
                    IWebElement span = Driver.FindElement(By.XPath(xpath2));
                    doubles.Add(double.Parse(element.GetAttribute("data-id")));
                }
            }
            catch
            {
                double id = double.Parse(Driver.ExecuteScript("return TribalWars.getGameData().village.id").ToString());
                doubles.Add(id);
               
            }
            return doubles;
        }
        public void GoTo(string url)
        {
            if (Driver.Url != url)
            {
                Sleep();
                Driver.Navigate().GoToUrl(url);
            }
        }     
    }
}

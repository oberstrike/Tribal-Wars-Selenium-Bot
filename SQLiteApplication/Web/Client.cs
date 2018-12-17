using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
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
            Thread.Sleep((new Random().Next(1, 3) * 1000) + 245);
        }
        #endregion

        private readonly List<string> urls = new List<string>() { "https://www.die-staemme.de/" };
        private FirefoxOptions options;

        #region Properties
        public bool IsConnected { get; set; }
        public bool IsLoggedIn { get; set; }
        public Configuration Config { get; set; }
        public Process TorProcess { get; set; }

        public FirefoxDriver Driver { get; set; }
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
             profile.SetPreference("general.useragent.override",
                                   "Mozilla/5.0 (Linux; Android 6.0; HTC One M9 Build/MRA58K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.98 Mobile Safari/537.36"); 
            options.Profile = profile;
            Console.WriteLine("Starte versteckten Client.");
            
        }

        public void Connect()
        {
            try
            {
                Driver = new FirefoxDriver(options);
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
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
                }catch(Exception e)
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
            GoTo(PathCreator.GetOverview(Config.User.Server.ToString()));
            IWebElement trTag = Driver.FindElement(By.XPath("//tr[contains(@class,'nowrap')]"));

            IReadOnlyList<IWebElement> elements = trTag.FindElements(By.XPath("//span[@class='quickedit-vn']"));
            double[] dArray = new double[elements.Count];
            for(int i = 0; i < dArray.Length; i++)
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

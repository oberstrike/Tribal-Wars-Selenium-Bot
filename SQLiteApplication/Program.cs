
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
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

        public static void Sleep()
        {
            Thread.Sleep((new Random().Next(1, 5) * 1000) + 245);

        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Starte test 6u" );
            string configPath = @"Config.json";

        

            ConfigurationManager manager = new ConfigurationManager(configPath);
            Configuration configuration = manager.Configuration;

            Client client = new Client(configuration);
            client.Connect();
            client.Login();
            client.Update();

            var village = configuration.User.Villages.First();
             



            client.Logout();








        }
    }

}

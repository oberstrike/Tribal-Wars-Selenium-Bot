using OpenQA.Selenium.Firefox;
using SQLiteApplication.Tools;
using SQLiteApplication.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.PagesData
{
    public abstract class Page
    {
        public Village PageVillage { get; set; }

        public FirefoxDriver Driver { get; set; }

        public abstract List<Updater> Updaters { get; }

        public Page(Village village, FirefoxDriver driver)
        {
            PageVillage = village;
            Driver = driver;
        }

        public virtual string Url()
        {
            return $"https://de{PageVillage.ServerId}.die-staemme.de/game.php?village={PageVillage.Id}";
        }

        protected void GoTo()
        {
            string url = Url();
            Console.WriteLine(url);

            if (Driver.Url != url)
            {
                Driver.Navigate().GoToUrl(url);
            }
        }

        protected void GoTo(string extension)
        {
            string url = Url() + extension;

            if (client.Url != url)
            {
                client.Navigate().GoToUrl(url);
            }
        }

        public void Update()
        {
            GoTo();
        
            foreach(var updater in Updaters)
            {
                updater.Update(PageVillage, Driver);
                
            }
            Client.Sleep();
        }
    }

}

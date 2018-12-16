using OpenQA.Selenium.Firefox;
using SQLiteApplication.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.Tools
{
    public abstract class AbstractPage
    {
        public abstract List<AbstractUpdater> Updaters { get; } 

        public FirefoxDriver Driver { get; set; }

        public Village Village { get; set; }

        public abstract string URL { get; }
        public void GoTo()
        {
            GoTo("");
        }

        public void GoTo(string extension)
        {
            if (Driver.Url != URL + extension)
            {
                Driver.Navigate().GoToUrl(URL);
                Client.Sleep();
            }
        }

        public AbstractPage(FirefoxDriver driver, Village village) {
            Driver = driver;
            Village = village;
        } 

        public virtual void Update()
        {
            GoTo();

            foreach(var updater in Updaters)
            {
                updater.Update(Driver, Village);
            }
           
        }

    }

}

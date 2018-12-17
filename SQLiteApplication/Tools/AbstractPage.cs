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


        public AbstractPage(FirefoxDriver driver, Village village) {
            Driver = driver;
            Village = village;
        } 

        public virtual void Update()
        {
            Driver.GoTo(URL);

            foreach(var updater in Updaters)
            {
                updater.Update(Driver, Village);
            }
           
        }

    }

}

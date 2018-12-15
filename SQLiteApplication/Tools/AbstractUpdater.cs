using OpenQA.Selenium.Firefox;
using SQLiteApplication.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.Tools
{
    public abstract class AbstractUpdater
    {
        public abstract Action<FirefoxDriver, Village> UpdateAction { get; }
        public virtual void Update(Village village, FirefoxDriver driver)
        {
            UpdateAction(driver,village);
        }
    }
}

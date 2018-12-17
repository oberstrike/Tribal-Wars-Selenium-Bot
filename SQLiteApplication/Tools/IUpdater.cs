using OpenQA.Selenium.Firefox;
using SQLiteApplication.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.Tools
{
    public interface IUpdater
    {

        void Update(FirefoxDriver driver, Village village);
    }
}

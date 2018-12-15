using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.Web;

namespace SQLiteApplication.Tools
{
    class SmithUpdater : AbstractUpdater
    {
        public override Action<FirefoxDriver, Village> UpdateAction => (driver, village) =>
        {
            village.Technologies = (Dictionary<string, object>)driver.ExecuteScript("return BuildingSmith.techs.available");
            
        };
        
    }
}

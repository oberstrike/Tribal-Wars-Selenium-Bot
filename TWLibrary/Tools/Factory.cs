using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using TWLibrary.UserData;
using TWLibrary.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWLibrary.Tools
{
    public static class Factory
    {
        public static Village GetVillage(double id, double serverId, IWebDriver driver, User user, string[] buildOrder)
        {
            return new Village(id, serverId, driver, user, buildOrder);
        }

        public static Client GetAdvancedClient(Configuration config)
        {
            return new AdvancedClient(config);
        }

        public static Client GetClient(Configuration config)
        {
            return new Client(config);
        }
        

    }
}

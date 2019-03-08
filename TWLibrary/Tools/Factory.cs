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
        public static Village GetVillage(double id, double serverId, IWebDriver driver, User user)
        {
            return new Village(id, serverId, driver, user);
        }

        public static Client GetAdvancedClient(User user)
        {
            return new AdvancedClient(user);
        }

        public static Client GetClient(User user)
        {
            return new Client(user);
        }
        

    }
}

using SQLiteApplication.UserData;
using SQLiteApplication.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test 1i");
            Configuration config = new ConfigurationManager("Config.json").Configuration;
            Client client = new Client(config);
            client.Connect();
            client.Login();

            foreach(var village in config.User.Villages)
            {
                village.Update();
            }
            
            client.Logout();



            Console.ReadLine();

        }
    }
}

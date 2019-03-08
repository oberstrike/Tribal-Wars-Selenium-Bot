
using TWLibrary.Tools;
using TWLibrary.UserData;
using TWLibrary.Web;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace OutputProject
{
    internal class Program
    {


        public static void Main(string[] args)
        {
            Configuration config = new ConfigurationManager(@"Config.json").Configuration;
            TimeSpan? timeSpan = null;

            EmailAccount account = config.EmailAccount;
            var randomizer = new Randomizer();
            int errorCount = 0;
            while (errorCount == 0)
            {
                Client.Print("Starte Build Routine 1.0");
                Client.Print(config.User);

                Client client = Factory.GetAdvancedClient(config);
                client.Connect();
                try
                {
                    client.Login();
                    client.Update();
                    client.Logout();
                    client.Close();

                    if (config.Build)
                    {
                        timeSpan = client.GetBestTimeToCanBuild();
                        if (!timeSpan.HasValue)
                        {
                            TimeSpan? timeForQueue = client.GetBestTimeForQueue();
                            if (!timeForQueue.HasValue)
                                timeSpan = new TimeSpan(new Random().Next(2, 3), new Random().Next(1, 20), new Random().Next(1, 20));
                        }
                        timeSpan = timeSpan.Value.Add(new TimeSpan(0, 1, 0));
                    }
                 
                    Client.Print("Schlafe für " + timeSpan);
                    Client.Print("Schlafe bis " + DateTime.Now.Add(timeSpan.Value));
                    Task.Delay(timeSpan.Value).Wait();

               }
                catch (Exception e)
                {
                   var weiter = ExceptionHandling(e, client, new EmailProvider(account));

                if (!weiter)
                     break;
               }

            }
        }

        public static bool ExceptionHandling(Exception e, Client client, EmailProvider provider)
        {
            
            if (e.Message.Contains("SecurityError"))
            {
                provider.SendEmail(e.Message);
                return false;
            }
            else if (e.Message.Contains("imeout") | e.Message.Contains("Tried to run"))
            {
                Client.Print("Upps there was a mistake.");
                Client.Print(e.Message);
                client.Close();
                return true;
            }
            else
            {
                Client.Print(e.Message);
                client.Close();
                return true;
            }
            
        }

    }
}

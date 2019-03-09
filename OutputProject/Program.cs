
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TWLibrary.Tools;
using TWLibrary.UserData;
using TWLibrary.Web;

namespace OutputProject
{
    internal class Program
    {
        private static Configuration configuration;
        private static EmailAccount emailAccount;

        public static void Main(string[] args)
        {
            configuration = new ConfigurationManager(@"Config.json").Configuration;
            emailAccount = configuration.EmailAccount;

            int errorCount = 0;
            List<TimeSpan> timeSpans = new List<TimeSpan>();

            while (errorCount == 0)
            {
               
                foreach(User user in configuration.Users)
                {
                    TimeSpan? zeitspanne = ComputeUser(user);
                    if(zeitspanne.HasValue)
                        timeSpans.Add(zeitspanne.Value);
                }

                TimeSpan timeToWait = timeSpans.OrderBy(each => each.Ticks).First();

                Client.Print("Warte für " + timeToWait + " Bis " + DateTime.Now.Add(timeToWait));
                Task.Delay(timeToWait).Wait();

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

        public static TimeSpan? ComputeUser(User user)
        {
            Client client = new Client(user);
            TimeSpan? timeSpan = null;
            Randomizer randomizer = new Randomizer();
            Client.Print("Starte Build Routine von " + user);
            try
            {
                client.Connect();
                client.Login();
                client.Update();
                client.Logout();
                client.Close();

                if (user.Build)
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
                if(!timeSpan.HasValue)
                    timeSpan = new TimeSpan(0, randomizer.Next(configuration.MinimumTimeToWait, configuration.MaximumTimeToWait), randomizer.Next(0, 60));
            }
            catch (Exception e)
            {
                try
                {
                    ExceptionHandling(e, client, new EmailProvider(emailAccount));
                }
                catch (Exception e2)
                {
                    Client.Print(e2.Message);
                }
            }
            return timeSpan;
        }

    }
}

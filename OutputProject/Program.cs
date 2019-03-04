
using System;
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
            while (errorCount == 0)
            {
                Client.Print("Starte Build Routine 1.0");
                foreach(User user in configuration.Users)
                {
                    bool weiter = ComputeUser(user);
                    if (!weiter)
                        errorCount++;
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

        public static bool ComputeUser(User user)
        {
            Client client = new Client(user);
            TimeSpan? timeSpan = null;
            Randomizer randomizer = new Randomizer();
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


                timeSpan = new TimeSpan(0, randomizer.Next(configuration.MinimumTimeToWait, configuration.MaximumTimeToWait), randomizer.Next(0, 60));
                Client.Print("Schlafe für " + timeSpan);
                Client.Print("Schlafe bis " + DateTime.Now.Add(timeSpan.Value));
                Task.Delay(timeSpan.Value).Wait();

            }
            catch (Exception e)
            {
                bool weiter = ExceptionHandling(e, client, new EmailProvider(emailAccount));
                return weiter;
            }
            return true;
        }

    }
}

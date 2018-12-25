using SQLiteApplication.UserData;
using System;
using System.Collections.Generic;
using System.Threading;

namespace OutputProject
{
    internal class Program
    {
        public static List<string> buildOrder = new List<string>() { "wood", "iron", "stone" };

        private static void Main(string[] args)
        {
            Configuration config = new ConfigurationManager("Config.json").Configuration;
            int errorCount = 0;

            while (errorCount < 2)
            {
                Console.WriteLine("Starte Build Routine");
                Console.WriteLine(config.User);

                SQLiteApplication.Web.Client client = new SQLiteApplication.Web.Client(config);
                client.Connect();
                client.Login();

                client.Update();
                client.MoveResources();

                TimeSpan? timeSpan = client.GetBestTime();

                client.Logout();
                client.Close();
                if (!timeSpan.HasValue)
                {
                    timeSpan = new TimeSpan(new Random().Next(2, 3), new Random().Next(1, 20), new Random().Next(1, 20));
                }
                timeSpan = timeSpan.Value.Add(new TimeSpan(0, 0, 1));

                Console.WriteLine("Schlafe für " + timeSpan);
                Console.WriteLine("Schlafe bis " + DateTime.Now.Add(timeSpan.Value));
                Thread.Sleep(timeSpan.Value);

            }

            Console.Read();
        }

    }
}

using SQLiteApplication.Tools;
using SQLiteApplication.UserData;
using SQLiteApplication.Web;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace OutputProject
{
    internal class Program
    {
        public static List<string> buildOrder = new List<string>() { "snob" };

        public static void Main(string[] args)
        {
            Configuration config = new ConfigurationManager(@"Config.json").Configuration;

            int errorCount = 0;
            while (errorCount == 0)
            {
                Client.Print("Starte Build Routine 1.0");
                Client.Print(config.User);

                Client client = Factory.GetAdvancedClient(config);
                try
                {
                    client.Connect();
                    client.Login();

                    client.Update();



                    TimeSpan? timeSpan = null;
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


                    timeSpan = new TimeSpan(0, new Random().Next(12, 20), new Random().Next(0, 60));
                    Client.Print("Schlafe für " + timeSpan);
                    Client.Print("Schlafe bis " + DateTime.Now.Add(timeSpan.Value));
                    Task.Delay(timeSpan.Value).Wait();


                }
                catch (Exception e)
                {
                    errorCount++;
                    Client.Print(e.Message);
                    Client.Print(e.Source);
                    SendEmail(e.Message);
                    try
                    {
                        client.Close();
                    }
                    catch
                    {

                    }
                    Console.ReadLine();
                }

            }
        }

        private static void SendEmail(string error)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("meine-email@hotmail.de"); //Absender 
            mail.To.Add("andere-email@gmx.de"); //Empfänger 
            mail.Subject = "BOT-SCHUTZ";
            mail.Body = $"{DateTime.Now} + \n {error}";

            SmtpClient client = new SmtpClient("smtp.live.com", 25); //SMTP Server von Hotmail und Outlook. 

            try
            {
                client.Credentials = new System.Net.NetworkCredential("meine-email@hotmail.de", "meinpasswort");//Anmeldedaten für den SMTP Server 

                client.EnableSsl = true; //Die meisten Anbieter verlangen eine SSL-Verschlüsselung 

                client.Send(mail); //Senden 

                Console.WriteLine("E-Mail wurde versendet");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler beim Senden der E-Mail\n\n{0}", ex.Message);
            }
            Console.ReadKey();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TWLibrary.Tools;

namespace TWLibrary.UserData
{
    public class EmailProvider
    {

        private EmailAccount _emailAccount;


        public EmailProvider(EmailAccount account)
        {
            _emailAccount = account;
            
        }

  
        public void SendEmail(string bodyText)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_emailAccount.SenderEmail); //Absender 
            mailMessage.To.Add(_emailAccount.TargetEmail); //Empfänger 
            mailMessage.Subject = "BOT-SCHUTZ";
            mailMessage.Body = $"{DateTime.Now} + \n {bodyText}";
            SmtpClient mailClient = default(SmtpClient);

            if (_emailAccount.SenderEmail.Contains("hotmail"))
                mailClient = new SmtpClient("smtp.live.com", 25); //SMTP Server von Hotmail und Outlook. 
            else
                throw new ArgumentException("Wrong email");

            try
            {
                mailClient.Credentials = new System.Net.NetworkCredential(_emailAccount.SenderEmail, _emailAccount.DecryptPassword() );//Anmeldedaten für den SMTP Server 

                mailClient.EnableSsl = true; //Die meisten Anbieter verlangen eine SSL-Verschlüsselung 

                mailClient.Send(mailMessage); //Senden 

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

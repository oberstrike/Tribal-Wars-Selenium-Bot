using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.Tools;
using SQLiteApplication.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.PagesData
{
    public abstract class Page
    {
        public Village PageVillage { get; set; }

        public FirefoxDriver Driver { get; set; }

        public abstract List<Updater> Updaters { get; }

        public Page(Village village, FirefoxDriver driver)
        {
            PageVillage = village;
            Driver = driver;
        }

        public virtual string Url()
        {
            return $"https://de{PageVillage.ServerId}.die-staemme.de/game.php?village={PageVillage.Id}";
        }

        protected void GoTo()
        {
            string url = Url();
            Console.WriteLine(url);

            if (Driver.Url != url)
            {
                Driver.Navigate().GoToUrl(url);

                try
                {
                    var element = Driver.FindElement(By.XPath(".//div[contains(@class, 'quest opened finished')]"));
                    if (element != null)
                    {

                        element.Click();
                        var quest = Driver.FindElement(By.XPath(".//a[contains(@onclick, 'Quests.getQuest(')]"));
                        if (quest != null)
                        {
                            quest.Click();
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
             
            }
        }

        protected void GoTo(string extension)
        {
            string url = Url() + extension;

            if (client.Url != url)
            {
                client.Navigate().GoToUrl(url);
            }
        }

        public void Update()
        {
            GoTo();
        
            foreach(var updater in Updaters)
            {
                try
                {
                    updater.Update(PageVillage, Driver);
                }catch(Exception e)
                {
                    Console.WriteLine(this.GetType().Name + " konnte nicht aktualisiert werden.");
                }


            }
            Client.Sleep();
        }
    }

}

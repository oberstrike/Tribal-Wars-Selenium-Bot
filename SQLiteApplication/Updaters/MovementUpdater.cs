using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.Web;

namespace SQLiteApplication.Tools
{
    class MovementUpdater : IUpdater
    {

        public void Update(FirefoxDriver driver, Village village)
        {
            string tr_Class = "command-row";
            Client.Sleep();
            try
            {
                var rows = driver.FindElement(By.Id("commands_outgoings")).FindElements(By.ClassName(tr_Class));
                ICollection<TroupMovement> movements = new List<TroupMovement>();

                foreach (var row in rows)
                {
                    var attackType = row.FindElement(By.ClassName("command_hover_details")).GetAttribute("data-command-type");
                    var targetElement = row.FindElement(By.ClassName("quickedit-out"));
                    var movementId = targetElement.GetAttribute("data-id");

                    movements.Add(new TroupMovement() { Type = attackType, MovementId = movementId });

                    Client.Sleep();
                }

                foreach (var movement in movements)
                {
                    var d = driver.FindElement(By.CssSelector($".quickedit-out[data-id='{movement.MovementId}']"));


                    if (movements.Where(move => move.MovementId == d.GetAttribute("")).Count() == 0)
                    {
                        d.Click();
                        var id = driver.FindElements(By.XPath("//*[@data-player]")).ToArray()[1].GetAttribute("data -id");
                        movement.TargetId = id;
                        Client.Sleep();
                        driver.Navigate().GoToUrl(village.pathCreator.GetPlace());
                    }
                }
                village.OutcomingTroops = movements;
            }
            catch
            {

            }
        }
    }
}

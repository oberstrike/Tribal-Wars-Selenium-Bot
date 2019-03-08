using OpenQA.Selenium;
using TWLibrary.Tools;
using TWLibrary.Updaters;
using TWLibrary.Web;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections.ObjectModel;

namespace TWLibrary.Page
{
    public class FarmassistPage : AbstractPage
    {
        public FarmassistPage(Village village) : base(village)
        {

        }

        public override void Update()
        {
            base.Update();
            Farm();
        }

        public void Farm()
        {
            Village.Driver.GoTo(URL);
            var showLossedAttacks = Village.Driver.FindElement(By.Id("full_losses_checkbox"));
            var isChecked = showLossedAttacks.GetAttribute("checked");
            if (isChecked != null)
            {
                Village.Driver.FindElement(By.Id("full_losses_checkbox")).Click();
                Village.Driver.Navigate().Refresh();
            }
            
            var showAllAttacks = Village.Driver.FindElement(By.Id("attacked_checkbox"));
            isChecked = showAllAttacks.GetAttribute("checked");
        
            if (isChecked == null)
            {
                Village.Driver.FindElement(By.Id("attacked_checkbox")).Click();
                Village.Driver.Navigate().Refresh();
            }
            var unit = "light";


            double aCount = Village.Units[(Unit)Enum.Parse(typeof(Unit), unit.ToUpper())];

            if (aCount < (Village.FarmingVillages.Length + 1) * 5)
                return;

            int attackCount = AttackAllVillages(ref aCount);
            if(attackCount == 100)
            {
                if(aCount > 100)
                {
                    Village.Driver.GoTo(Village.pathCreator.GetFarmAssistSecondPage());
                    int max = 25;
                    attackCount += AttackAllVillages(ref aCount, max);

                }

            }

            Client.Print($"Es  {(attackCount > 1 ? "wurden" : "wurde")} {attackCount} {(attackCount > 1 ? "Dörfer" : "Dorf")} angegriffen.");

            if (Village.FarmingVillages != null)
            {
                if (Village.FarmingVillages.Length > 0)
                    NormalAttack(aCount);
            }


        }

        private int AttackAllVillages(ref double anwesendeEinheiten, int max = 100)
        {
            ReadOnlyCollection<IWebElement> attacks = Village.Driver.FindElements(By.XPath("(//a[contains(@class, ' farm_icon farm_icon_a')])"));
            string unit = "light";
            double count = 1;
            Client.Print($"Einheit: {unit} werden {count} benötigt. Es sind {anwesendeEinheiten} vorhanden, buffer: {(Village.FarmingVillages.Length + 1) * 5}");
            Unit unitEnum = (Unit)Enum.Parse(typeof(Unit), unit.ToUpper());
            int attackCount = 0;
            foreach (IWebElement element in attacks)
            {
                if (anwesendeEinheiten < Village.FarmingVillages.Length + 1 * 5)
                    break;
                if (attackCount > max)
                    break;

                string script = element.GetAttribute("onclick");
                Village.Driver.ExecuteScript(script);
                Thread.Sleep(new Randomizer().Next(610, 810));

                anwesendeEinheiten -= count;
                Village.Units[unitEnum] = anwesendeEinheiten;
                attackCount++;
            }

            return attackCount;
        }

        private void NormalAttack(double vorhandene)
        {
            int count = 5;
            Dictionary<Unit, int> dictionary = new Dictionary<Unit, int>();
            dictionary.Add(Unit.LIGHT, count);
            int attackCount = 0;

            foreach (string village in Village.FarmingVillages)
            {
                Client.Print($"Einheit: {Unit.LIGHT} werden {count} benötigt. Es sind {vorhandene} vorhanden");

                if (vorhandene >= count)
                    Attack(village, dictionary);
                else
                    break;
                attackCount++;
                vorhandene -= count;
            }
            if(attackCount > 0)
                Client.Print($"Es wurden {attackCount} Dörfer angegriffen");
        }

       

        public void Attack(string target, Dictionary<Unit, int> unitAndCount)
        {
            Client.Print($"{Village.Name} greift {target} an.");

            Village.Driver.GoTo(Village.pathCreator.GetAttackLink(target));
            Thread.Sleep(1250);
            foreach (KeyValuePair<Unit, int> kvp in unitAndCount)
            {
                Village.Driver.FindElement(By.Id("unit_input_" + kvp.Key.ToString().ToLower())).SendKeys(kvp.Value.ToString());
                Thread.Sleep(1250);
            }
            Village.Driver.FindElement(By.Id("target_attack")).Click();
            Client.Sleep();
            Village.Driver.FindElement(By.Id("troop_confirm_go")).Click();
            Thread.Sleep(2231);
        }



        public override List<IUpdater> Updaters => new List<IUpdater>() { new FarmassistUpdater() };

        public override string URL => Village.pathCreator.GetFarmAssist();
    }
}

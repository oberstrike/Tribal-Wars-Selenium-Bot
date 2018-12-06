
namespace SQLiteApplication.PagesData
{
    public class StablePage : Page
    {
        public StablePage(Village village, FirefoxDriver driver) : base(village, driver)
        {

        }

        public override List<Updater> Updaters => new List<Updater>() { };

        public override string Url()
        {
            return base.Url() + "&screen=place";
        }
        
        public void TrainUnitsInStable(Dictionary<Unit, double> units)
        {
            GoTo();
            IWebElement spysInput = null;
            IWebElement lightInput = null;
            IWebElement heavyInput = null;
            try
            {
                spysInput = Driver.FindElementByXPath("//input[@id='spy_0']");
                lightInput = Driver.FindElementByXPath("//input[@id='light_0']");
                heavyInput = Driver.FindElementByXPath("//input[@id='heavy_0']");
            }
            catch(Exception e)
            {
              Console.Writeline(e.Message);
            
            }
            IWebElement trainBtn = Driver.FindElementByCssSelector(".btn.btn-recruit");
            FillForm(units, spysInput, "spy");
            FillForm(units, lightInput, "light");
            FillForm(units, heavyInput, "heavy");
           
            trainBtn.Click();
            Client.Sleep();
        }
        
        private void FillForm(Dictionary<Unit, double> units, IWebElement input, string unit)
        {
            if (units.ContainsKey(unit))
            {
                double count = units[unit];
                if (PageVillage.IsTrainable(count, unit) && input != null)
                {
                    input.SendKeys(count.ToString());
                }

            }
        }
    }
}

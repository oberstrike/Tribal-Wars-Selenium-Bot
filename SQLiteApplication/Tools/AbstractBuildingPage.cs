using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.Tools
{
    public abstract class AbstractBuildingPage : AbstractPage
    {

        public abstract string BUILDING { get; }

        public AbstractBuildingPage(FirefoxDriver driver, Village village) : base(driver, village)
        {

        }

        public override void Update()
        {
            var building = Village.GetBuilding(BUILDING);
            if (building != null)
                if (building.Level == 0)
                    return;
            if (Updaters.Count == 0)
                return;

            base.Update();
        }
    }
}

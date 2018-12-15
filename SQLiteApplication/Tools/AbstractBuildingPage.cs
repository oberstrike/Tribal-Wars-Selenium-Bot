using SQLiteApplication.VillageData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteApplication.Tools;
using OpenQA.Selenium.Firefox;

namespace SQLiteApplication.Tools
{
    public abstract class AbstractBuildingPage : AbstractPage
    {
        public abstract BuildingEnum MyBuilding { get; }

        public AbstractBuildingPage(Village village, FirefoxDriver driver) : base(village, driver)
        {

        }
        public override void Update()
        {
            var building = PageVillage.GetBuilding(MyBuilding.GetName());
            if (building != null)
                if (building.Level == 0)
                    return;


            base.Update();
        }

    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TWLibrary.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWLibrary.VillageData;

namespace TWLibrary.Tools.Tests
{
    [TestClass()]
    public class VillageBuilderTests
    {

        [TestMethod()]
        public void GetNextResourceBuilding()
        {
            Village village = new Village(10, 160, null, new UserData.User());
            var wood = new BuildingBuilder().WithLevel(30).WithPopulation(10).WithName("wood").Build();
            var stone = new BuildingBuilder().WithLevel(29).WithPopulation(15).WithName("stone").Build();
            var iron = new BuildingBuilder().WithLevel(29).WithPopulation(25).WithName("iron").Build();
            var farm = new BuildingBuilder().WithLevel(20).WithName("farm").Build();
            village.RManager = new ResourcesManager(village);
            village.RManager.Population = 100;
            village.RManager.MaxPopulation = 200;
            village.RManager.Wood = 10000;
            village.RManager.Stone = 10000;
            village.RManager.Iron = 10000;

            village.Buildings = new List<Building>();
            village.Buildings.Add(wood);
            village.Buildings.Add(stone);
            village.Buildings.Add(iron);
            village.Buildings.Add(farm);
           

            VillageBuilder builder = new VillageBuilder(village);
            var target = builder.GetNextResourceBuilding();
            Console.WriteLine(target);
            Assert.IsTrue(true);
        }
    }
}
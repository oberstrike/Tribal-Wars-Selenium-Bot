using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLiteApplication;
using SQLiteApplication.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.Tests
{
    [TestClass()]
    public class VillageTests
    {
        private Village _village;

        [TestInitialize]
        public void InitTest()
        {
            _village = new Village(1, 160, null, new UserData.User(), default(string[]));
            _village.OutcomingTroops = new List<TroupMovement>() { new TroupMovement() { TargetId = "2", MovementId = "3", Type = "ATTACK" } };
            _village.RManager = new VillageData.ResourcesManager(_village) { Iron = 100, Stone = 100, Wood = 100, IronProduction = 0.3, WoodProduction = 0.3, StoneProduction = 0.3, MaxPopulation = 200, Population = 100, StorageMax = 6400 };
            _village.TManager = new VillageData.TradeManager() { TotalTraders = 10, AvailableTraders = 10 };
            _village.pathCreator = new Web.PathCreator(_village.ServerId, _village.Id);
            _village.Name = "Mein Testdorf";
            _village.Buildings = new List<VillageData.Building>() { new VillageData.BuildingBuilder().WithBuildeable(true).WithBuildingTime(new TimeSpan(0, 0, 60)).WithName("Wood").Build() };
            _village.BuildOrder = new string[] { "Wood" };
        }


        [TestMethod()]
        public void VillageTest()
        {
            Assert.IsNotNull(_village);
        }

        [TestMethod()]
        public void GetAttackedVillagesTest()
        {
            var attackedVillages = _village.GetAttackedVillages();
            Assert.IsNotNull(attackedVillages);
            var id = attackedVillages.First<string>();
            Assert.AreEqual(id, "2");

        }

        [TestMethod()]
        public void CanConsumeTest()
        {
            var unit = Unit.AXE;
            var building = new VillageData.BuildingBuilder().WithWood(100).WithIron(120).WithStone(100).Build();

            var test1 = _village.CanConsume(unit);
            var test2 = _village.CanConsume(building);
            var test3 = _village.CanConsume(90, 90, 160, 1);

            Assert.AreEqual(test1, true);
            Assert.AreEqual(test2, false);
            Assert.AreEqual(test3, false);

        }

        [TestMethod()]
        public void GetNextBuildingTest()
        {
            var building = _village.GetNextBuilding();

            Assert.IsNotNull(building);
            Assert.AreEqual(building.Name, "Wood");


        }

        [TestMethod()]
        public void GetBuildingTest()
        {
            var building = _village.GetBuilding("Wood");
            Assert.IsNotNull(building);
            Assert.AreEqual("Wood", building.Name);

        }


        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildTest()
        {
            _village.Build("Woid");
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TrainTest()
        {
            var dictionary = new Dictionary<Unit, double>();
            dictionary.Add(Unit.AXE, 100);

            bool test = _village.Train(dictionary);
            Assert.IsFalse(test);

        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendRessourceToVillageTest()
        {
            var dictionary = new Dictionary<string, double>();
            dictionary.Add("wood", 100);



            bool test = _village.SendRessourceToVillage(dictionary, "13:37");
            Assert.IsFalse(test);

        }

        [TestMethod()]
        public void GetBuildingsInBuildOrderTest()
        {
            var buildings = _village.GetBuildingsInBuildOrder();
            Assert.IsNotNull(buildings);
            Assert.AreNotEqual(0, buildings.Count());
            var first = buildings.First();
            Assert.IsNotNull(first);
            Assert.AreEqual("Wood", first.Name);

        }

        [TestMethod()]
        public void EqualsTest()
        {
            Assert.AreEqual(_village, new Village(1, 161, null, null, null));
        }

        [TestMethod()]
        public void ToStringTest()
        {
            var str = _village.ToString();
            Assert.IsNotNull(str);
            Assert.IsTrue(str.Length > 1);
            
        }

        [TestMethod()]
        public void GetHashCodeTest()
        {
            Assert.AreEqual(_village.GetHashCode(), new Village(2, 160, null, null, null).GetHashCode());
        }
    }
}
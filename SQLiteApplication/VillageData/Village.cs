using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.Page;
using SQLiteApplication.Tools;
using SQLiteApplication.Web;

namespace SQLiteApplication
{
    public sealed class Village
    {
        public Village(double id, double serverId, FirefoxDriver driver)
        {
            Id = id;
            ServerId = serverId;
            pathCreator = new PathCreator(serverId.ToString(), id.ToString());
            Driver = driver;
        }


        #region PROPERTIES    
        public List<AbstractPage> Pages { get => new List<AbstractPage>() { new MainPage(Driver, this), new BarracksPage(Driver, this)}; }
        public ICollection<Building> Buildings { get; set; }
        public FirefoxDriver Driver { get; set; }
        public Dictionary<string, int> MaxBuildings { get; set; }
        public double Id { get; set; }
        public double ServerId { get; set; }
        public double Wood { get; set; }
        public double Stone { get; set; }
        public double Iron { get; set; }
        public double WoodProduction { get; set; }
        public double IronProduction { get; set; }
        public double StoneProduction { get; set; }
        public double StorageMax { get; set; }
        public double Population { get; set; }
        public double MaxPopulation { get; set; }
        public IList<KeyValuePair<string,TimeSpan>> BuildingsInQueue { get; set; }
        public int HaendlerCount { get; set; }
        public Village(Dictionary<string, int> maxBuildings) => MaxBuildings = maxBuildings;
        public void AddBuilding(Building building) => Buildings.Add(building);
        public ICollection<TroupMovement> OutcomingTroops { get; set; }
        public Dictionary<string, object> Technologies { get; set; }
        public Dictionary<Unit, double> Units { get; set; }
        public IList<string> GetAttackedVillages() => OutcomingTroops.Select(x => x.TargetId).ToList();
        public PathCreator pathCreator { get; set; }
        public string Csrf { get; internal set; }
        public double Traders { get; internal set; }
        #endregion

        #region METHODS
        public bool CanConsume(double wood, double stone, double iron, double population)
        {
            if (wood > Wood || stone > Stone || iron > Iron || Population + population > MaxPopulation)
            {
                return false;
            }
            Wood -= wood;
            Stone -= stone;
            Iron -= iron;
            return true;
        }
        public bool CanConsume(Unit unit)
        {
            return CanConsume(unit.GetWood(), unit.GetStone(), unit.GetIron(), unit.GetNeededPopulation());
        }
        public bool CanConsume(Building building)
        {
            return CanConsume(building.Wood, building.Stone, building.Iron, building.NeededPopulation);
        }
        public Building GetBuilding(string name)
        {
            return (from building in Buildings
                    where building.Name.Equals(name)
                    select building).First();
        }

        public void Update()
        {
            foreach(var page in Pages)
            {
                page.Update();
                Client.Sleep();
            }
        }

        public override string ToString()
        {
            return $"Wood: {Wood}, Stone:  {Stone}, Iron: {Iron}, {Buildings} " +
                $"\nWood Production: {WoodProduction}, Stone Production: {StoneProduction}, Iron Production: {IronProduction}";
        }

        public void Build(string name)
        {
            Build(GetBuilding(name));
        }
        public void Build(Building building)
        {
            var page = Pages.Where(each => each is MainPage).First() as MainPage;
            page.Build(building);

        }

        public void Train(Dictionary<Unit, double> units)
        {
            var page = (BarracksPage) Pages.Where(each => each is BarracksPage).First();

            foreach (var kvp in units)
            {
                var name = ((Unit)kvp.Key).GetName();
                page.Train(kvp.Key, kvp.Value);
                    

            }
        }

        #endregion
    }
}

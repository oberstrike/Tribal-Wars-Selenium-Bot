using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.Page;
using SQLiteApplication.Tools;
using SQLiteApplication.VillageData;
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
        public ResourcesManager Manager { get; set; }
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
            if (wood > Manager.Wood || stone > Manager.Stone || iron > Manager.Iron || Manager.Population + population > Manager.MaxPopulation)
            {
                return false;
            }
            Manager.Wood -= wood;
            Manager.Stone -= stone;
            Manager.Iron -= iron;
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
            return $"Wood: {Manager.Wood}, Stone:  {Manager.Stone}, Iron: {Manager.Iron}, {Buildings} " +
                $"\nWood Production: {Manager.WoodProduction}, Stone Production: {Manager.StoneProduction}, Iron Production: {Manager.IronProduction}";
        }

        public void Build(string name)
        {
            Build(GetBuilding(name));
        }
        public void Build(Building building)
        {

            var page = Pages.Where(each => each is MainPage).First() as MainPage;
            Driver.GoTo(page.URL);
            page.Build(building);
            Client.Sleep();
            

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

        public override bool Equals(object obj)
        {
            var village = obj as Village;
            if(village != null)  
                return village.Id == this.Id;
            
            return false;
        }

        public Dictionary<string, double> GetMissingRessourcesForBuilding(Building building)
        {
            string[] ressis = { "Wood", "Stone", "Iron" };
            Dictionary<string, double> resDictionary = new Dictionary<string, double>();

            foreach(var res in ressis)
            {
                double buildingValue = (double) building.GetType().GetProperty(res).GetValue(building);
                double villageValue = (double)this.GetType().GetProperty(res).GetValue(this);
                double diff = buildingValue - villageValue;
                if (diff > 0)
                    resDictionary.Add(res, diff);
            }
            return resDictionary;


        }

        public override int GetHashCode()
        {
            return 2108858624 + Id.GetHashCode();
        }

        public static bool operator ==(Village village1, Village village2)
        {
            return EqualityComparer<Village>.Default.Equals(village1, village2);
        }

        public static bool operator !=(Village village1, Village village2)
        {
            return !(village1 == village2);
        }



        #endregion
    }
}

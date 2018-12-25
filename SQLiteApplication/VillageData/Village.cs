using OpenQA.Selenium.Firefox;
using SQLiteApplication.Page;
using SQLiteApplication.Tools;
using SQLiteApplication.VillageData;
using SQLiteApplication.Web;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public List<AbstractPage> Pages => new List<AbstractPage>() { new MainPage(Driver, this), new MarketPage(Driver, this) };
        public ICollection<Building> Buildings { get; set; }
        public FirefoxDriver Driver { get; set; }
        public Dictionary<string, int> MaxBuildings { get; set; }
        public double Id { get; set; }
        public double ServerId { get; set; }
        public ResourcesManager RManager { get; set; }
        public TradeManager TManager { get; set; } = new TradeManager();
        public IList<KeyValuePair<string, TimeSpan>> BuildingsInQueue { get; set; }
        public ICollection<TroupMovement> OutcomingTroops { get; set; }
        public Dictionary<string, object> Technologies { get; set; }
        public Dictionary<Unit, double> Units { get; set; }
        public IList<string> GetAttackedVillages()
        {
            return OutcomingTroops.Select(x => x.TargetId).ToList();
        }
        public PathCreator pathCreator { get; set; }
        public string Csrf { get; internal set; }
        public string Coordinates { get; set; }
        #endregion

        #region METHODS
        public bool CanConsume(double wood, double stone, double iron, double population)
        {
            return RManager.CanConsume(wood, stone, iron, population);
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
            foreach (AbstractPage page in Pages)
            {
                page.Update();
                Client.Sleep();
            }
        }

        public void Build(string name)
        {
            Build(GetBuilding(name));
        }
        public void Build(Building building)
        {
            MainPage page = Pages.Where(each => each is MainPage).First() as MainPage;
            Driver.GoTo(page.URL);
            page.Build(building);
            Client.Sleep();
        }

        public void Train(Dictionary<Unit, double> units)
        {
            BarracksPage page = (BarracksPage)Pages.Where(each => each is BarracksPage).First();

            foreach (KeyValuePair<Unit, double> kvp in units)
            {
                string name = kvp.Key.GetName();
                page.Train(kvp.Key, kvp.Value);
            }
        }

        public void SendRessourceToVillage(Dictionary<string, double> resources, Village village)
        {
            MarketPage page = Pages.Select(each => each as MarketPage).First();
            var wood = resources["Wood"];
            var stone = resources["Stone"];
            var iron = resources["Iron"];
            var traders = Math.Round((wood + stone + iron)/1000 + 0.5);

            if(wood < RManager.Wood && stone < RManager.Stone && iron < RManager.Iron && TManager.AvailableTraders >= traders)
            {
                page.SendRessource(resources["Wood"], resources["Stone"], resources["Iron"], village.Coordinates);
                page.Update();
            }

        }
        #endregion

        #region OPERATORS
        public override bool Equals(object obj)
        {
            Village village = obj as Village;
            if (village != null)
                return village.Id == this.Id;

            return false;
        }
        public override string ToString()
        {
            return $"Wood: {RManager.Wood}, Stone:  {RManager.Stone}, Iron: {RManager.Iron}, {Buildings} " +
                $"\nWood Production: {RManager.WoodProduction}, Stone Production: {RManager.StoneProduction}, Iron Production: {RManager.IronProduction}";
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

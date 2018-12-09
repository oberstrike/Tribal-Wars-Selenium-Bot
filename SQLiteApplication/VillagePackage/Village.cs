using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.PagesData;
using SQLiteApplication.Tools;
using SQLiteApplication.UserData;
using SQLiteApplication.VillageData;
using SQLiteApplication.Web;

namespace SQLiteApplication
{
    public sealed class Village
    {
        #region CONSTRUCTORS
        public Village(string villageId, string serverId, FirefoxDriver driver, User owner)
        {
            Id = villageId;
            ServerId = serverId;
            Owner = owner;
            FManager = new Farmmanager();
            Pages = new List<Page>() { new MainPage(this, driver), new MapPage(this, driver), new SmithPage(this,driver) };
            
        }

        public Village(object villageId, object serverId, FirefoxDriver driver) : this(villageId.ToString(), serverId.ToString(), driver, null)
        {
           

        }
        #endregion

        #region Private_ATTRIBUTES
        private Dictionary<string, double> _units = new Dictionary<string, double>();
   
        #endregion

        #region PROPERTIES    
        public ICollection<Building> Buildings { get; set; }
        public Dictionary<Unit, double> Units { get; set; }
        public Farmmanager FManager { get; set; }
        public List<Page> Pages { get; set; }
        public string Id { get; set; }
        public string ServerId { get; set; }
        public double Wood { get; set; }
        public double Stone { get; set; }
        public double Iron { get; set; }
        public double WoodProduction { get; set; }
        public double IronProduction { get; set; }
        public User Owner{get; set;}     
        public double StoneProduction { get; set; }
        public double StorageMax { get; set; }
        public double Population { get; set; }
        public double MaxPopulation { get; set; }
        public IList<KeyValuePair<string,TimeSpan>> BuildingsInQueue { get; set; }
        public int HaendlerCount { get; set; }
        public void AddBuilding(Building building) => Buildings.Add(building);
        public ICollection<TroupMovement> OutcomingTroops { get; set; }
        public ICollection<TroupMovement> IncomingTroops { get; set; }
        public Dictionary<string, object> Technologies { get; set; }
        public string Csrf { get; internal set; }
        public Dictionary<string, double> GetUnits() => _units;
        public void SetUnits(Dictionary<string, double> value) => _units = value;
        public IList<string> GetAttackedVillages() => OutcomingTroops.Select(x => x.TargetId).ToList();
        #endregion

        #region METHODS
        public string GetLogout()
        {
            return $"https://de{ServerId}.die-staemme.de/game.php?village={Id}" + $"&screen=&action=logout&h={Csrf}";
        }
        public ICollection<Building> GetBuildings(Dictionary<string, object> keyValuePairs)
        {
            List<Building> newBuildings = new List<Building>();
            foreach (string key in keyValuePairs.Keys)
            {
                Dictionary<string, object> dictionary = (Dictionary<string, object>)keyValuePairs[key];

                string text = null;
                TimeSpan? timeSpan = null;
                if (dictionary.ContainsKey("error"))
                {
                    text = (string)dictionary["error"];
                    if (text != null)
                    {
                        if (text.Length > 1 && text.Contains("Genug") && text.Contains("um"))
                        {

                            timeSpan = StringToBuildTimeConverter.Convert(text);
                        }
                    }

                }
                try
                {
                    newBuildings.Add(new BuildingBuilder()
                                .WithName(key)
                                .WithWood((long)dictionary["wood"])
                                .WithStone((long)dictionary["stone"])
                                .WithIron((long)dictionary["iron"])
                                .WithLevel(double.Parse((string)dictionary["level"]))
                                .WithPopulation((long)dictionary["pop"])
                                .WithMaxLevel((long)dictionary["max_level"])
                                .WithBuildeable(text == null)
                                .WithBuildingTime(timeSpan)
                                .Build());

                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(key + " wurde nicht gefunden"); Thread.Sleep((new Random().Next(1, 5) * 1000) + 245);
                }
            }
            return newBuildings;
        }
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
        public override string ToString()
        {
            return $"Wood: {Wood}, Stone:  {Stone}, Iron: {Iron}, {Buildings} " +
                $"\nWood Production: {WoodProduction}, Stone Production: {StoneProduction}, Iron Production: {IronProduction}";
        }
        public bool IsTrainable(double count, Unit unit)
        {
            return CanConsume(unit.GetWood() * count, unit.GetIron() * count, unit.GetStone() * count, unit.GetPopulation() * count);

        }
        public void Update()
        {
            foreach(var page in Pages)
            {
                page.Update();
                Client.Sleep();
            }
        }  
        public bool Build(Building building)
        {
            if (CanConsume(building.Wood, building.Stone, building.Iron, building.NeededPopulation))
            {
                MainPage mainPage = Pages.Where(each => each is MainPage).Select(each => (MainPage)each).First();
                mainPage.Build(building);
                Client.Sleep();
                mainPage.Update();
                return true;
            }
            return false;
        }
        public void Build(string buildingName)
        {
            Build(Buildings.Where(x => x.Name == buildingName).First());
        }
        public void SendRessouce(int wood, int stone, int iron, string targetId)
        {
            if (CanConsume(wood, stone, iron, 0))
            {
                Pages.Where(each => each is MarketPage).Select(each => (MarketPage)each).First().SendRessource( wood, stone, iron, targetId);
            }
        }       
        public void Attack(Dictionary<Unit, double> units, string targetId){
            AttackPage page = Pages.Where(each => each is AttackPage).Select(each => (AttackPage)each).First();
            page.Attack(units, targetId);
            
        }

        public bool IsResearched(string technology)
        {
            if(Technologies == null)
            {
                return false;
            }
            var tech = (Dictionary<string, object>) Technologies[technology];
            var level = int.Parse((string)tech["level"]);

            return level != 0;
        }
        #endregion
    }
}

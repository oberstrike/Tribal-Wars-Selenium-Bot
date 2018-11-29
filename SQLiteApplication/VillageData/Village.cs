using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SQLiteApplication
{
    public sealed class Village
    {
        
        /*
            Man muss überlegen ob man nicht hier die ganzen Methoden reinschreibt also:
                1. Updater
                2. Attack
                3. Training
                etc.
            Dann ruft man im client folgendes auf:
                Config.User.GetVillage(id).Attack(targetId, units);
            Dazu braucht Village aber zugriff auf den Driver von Client...
            Warum Villages in User stecken?
                - dadurch kann ein Bot in Zukunft mehrere Benutzer steuern:
                    Markus V2 und oberstriker parallel
            Dadurch kann man einen Driver nutzen um sequenziell Aufgaben zu steuern.
        
        
        */
        #region STATIC ATTRIBUTES
        public static readonly Dictionary<string, Dictionary<string, double>> unit_Prices = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, double>>>("{" +
                                    "'spy':{'wood':50,'stone':50,'iron':20, 'population': 2} ," +
                                    "'light':{'wood':125,'stone':100,'iron':250, 'population': 4}," +
                                    "'heavy':{'wood':200,'stone':150,'iron':600, 'population': 6} ," +
                                    "'spears':{'wood':50,'stone':30,'iron':10, 'population': 1}," +
                                    "'sword':{'wood':30,'stone':30,'iron':70, 'population': 1}," +
                                    "'axe':{'wood':60,'stone':30,'iron':40, 'population': 1}}");        private Dictionary<string, double> _units = new Dictionary<string, double>();
        #endregion
            
        #region PROPERTIES    
        public ICollection<Building> Buildings { get; set; }
        public Dictionary<string, int> MaxBuildings { get; set; }

        public double Id { get; set; }
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
        public ICollection<TroupMovement> IncomingTroops { get; set; }
        public Dictionary<string, object> Technologies { get; set; }
        public Dictionary<string, double> GetUnits() => _units;
        public void SetUnits(Dictionary<string, double> value) => _units = value;
        public IList<string> GetAttackedVillages() => OutcomingTroops.Select(x => x.TargetId).ToList();   
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


        public TimeSpan GetTimeToBuild(Building building)
        {
            double wood = (building.Wood - Wood ) / WoodProduction;
            double stone = (building.Stone - Stone ) / StoneProduction;
            double iron = (building.Iron - Iron ) / IronProduction;

            double max = new double[] { wood, stone, iron }.Max();

            if (double.IsInfinity(max) || max < 0)
            {
                return new TimeSpan();
            }
            return new TimeSpan(Convert.ToInt32(Math.Floor(max)),Convert.ToInt32((max - Math.Floor(max))*60), 59);
        }

        #endregion
    }
}

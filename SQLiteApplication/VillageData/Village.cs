using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SQLiteApplication
{
    public class Village
    {
        /*  Neue Anforderungen:
         *  - neue daten: 
         *  wood_prod
         *  stone_prod
         *  iron_prod
         *  
         *  target_wood 10000
         *  present_wood 1000
         *  wood_prod 0.171
         *  
         *  present_wood + wood_prod * 60 * x = 10000 - present_wood - wood_prod 
         * 
         * 
         * 
         */ 

        private Dictionary<string, double> _units = new Dictionary<string, double>();

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

        public KeyValuePair<String,DateTime> BuildingsInQueue { get; set; }

        public int HaendlerCount { get; set; }

        public Village(Dictionary<string, int> maxBuildings) => MaxBuildings = maxBuildings;
        public void AddBuilding(Building building) => Buildings.Add(building);

        public ICollection<TroupMovement> OutcomingTroops { get; set; }

        public ICollection<TroupMovement> IncomingTroops { get; set; }

        public Dictionary<string, Dictionary<string, double>> Unit_Prices => _unit_Prices;

        private readonly Dictionary<string, Dictionary<string,double>> _unit_Prices = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, double>>>("{" +
            "'spy':{'wood':50,'stone':50,'iron':20}," +
            "'light':{'wood':125,'stone':100,'iron':250}," +
            "'heavy':{'wood':200,'stone':150,'iron':600} ," +
            "'spears':{'wood':50,'stone':30,'iron':10}," +
            "'sword':{'wood':30,'stone':30,'iron':70}," +
            "'axe':{'wood':60,'stone':30,'iron':40}}");
        public Dictionary<string, object> Technologies { get; set; }

        public Dictionary<string, double> GetUnits()
        {
            return _units;
        }

        public void SetUnits(Dictionary<string, double> value)
        {
            _units = value;
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

        public IList<string> GetAttackedVillages() => OutcomingTroops.Select(x => x.TargetId).ToList();

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication
{
    public class Village
    {
        private Dictionary<string, double> _units = new Dictionary<string, double>();

        public ICollection<Building> Buildings { get; set; }
        public Dictionary<string, int> MaxBuildings { get; set; }

        public double Id { get; set; }
        public double Wood { get; set; }
        public double Stone { get; set; }
        public double Iron { get; set; }
        public KeyValuePair<String,DateTime> BuildingsInQueue { get; set; }

        public int HaendlerCount { get; set; }

        public Village(Dictionary<string, int> maxBuildings) => MaxBuildings = maxBuildings;
        public void AddBuilding(Building building) => Buildings.Add(building);

        public ICollection<TroupMovement> OutcomingTroops { get; set; }

        public ICollection<TroupMovement> IncomingTroops { get; set; }

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
            return $"Wood: {Wood}, Stone:  {Stone}, Iron: {Iron}, {Buildings}";
        }

    }
}

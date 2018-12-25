using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.VillageData
{
    public class ResourcesManager
    {
        public double Wood { get; set; }
        public double Iron { get; set; }
        public double MaxPopulation { get; set; }
        public double Stone { get; set; }
        public double UnusedWood { get; set; }
        public double UnusedIron { get; set; }
        public double UnusedStone { get; set; }
        public double WoodProduction { get; set; }
        public double IronProduction { get; set; }
        public double StoneProduction { get; set; }
        public double StorageMax { get; set; }
        public double Population { get; set; }

        public Village MyVillage { get; set; }

        public ResourcesManager(Village village)
        {
            MyVillage = village;
        }

        public void GetMissingRessourcesForBuilding(Building building)
        {
            string[] ressis = { "Wood", "Stone", "Iron" };
            Dictionary<string, double> resDictionary = new Dictionary<string, double>();

            foreach (var res in ressis)
            {
                double buildingValue = (double)building.GetType().GetProperty(res).GetValue(building);
                double villageValue = (double)this.GetType().GetProperty(res).GetValue(this);
                double diff = villageValue - buildingValue;
                this.GetType().GetProperty($"Unused{res}").SetValue(this, diff);
            }
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

    }
}

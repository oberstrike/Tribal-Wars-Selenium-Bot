using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWLibrary.VillageData
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


        public void GetMissingRessourcesForBuildings(IEnumerable<Building> buildings)
        {
            if (buildings.Count() == 0)
                throw new ArgumentOutOfRangeException("Die Anzahl an zu überprüfenden Gebäude darf nicht \"0\" sein");

            string[] ressis = { "Wood", "Stone", "Iron" };
            Dictionary<string, double> resDictionary = new Dictionary<string, double>();

            foreach (var res in ressis)
            {
                double costValue = GetHighestRessourceCostForBuildings(buildings, res);
                double villageValue = (double)this.GetType().GetProperty(res).GetValue(this);
                double diff = villageValue - costValue;
                this.GetType().GetProperty($"Unused{res}").SetValue(this, diff);
            }
        }

        private double GetHighestRessourceCostForBuildings(IEnumerable<Building> buildings, string ressource)
        {
            if (buildings.Count() == 0)
                throw new ArgumentOutOfRangeException("Die Anzahl an zu überprüfenden Gebäude darf nicht \"0\" sein");
            double highestValue = (double)buildings.ElementAt(0).GetType().GetProperty(ressource).GetValue(buildings.ElementAt(0));
            for (int i = 1; i < buildings.Count(); i++)
            {
                double value = (double)buildings.ElementAt(i).GetType().GetProperty(ressource).GetValue(buildings.ElementAt(i));
                if (value > highestValue)
                    highestValue = value;
            }
            return highestValue;
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

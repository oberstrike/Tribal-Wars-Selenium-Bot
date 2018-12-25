using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication
{
    public class Building
    {
        public string Name { get; set; }
        public double Wood { get; set; }
        public double Stone { get; set; }
        public double Iron { get; set; }
        public double Level { get; set; }  
        public double NeededPopulation { get; set; }
        public double TargetLevel { get; set; }
        public Boolean IsBuildeable { get; set; }
        public double MaxLevel { get; set; }
        public TimeSpan TimeToCanBuild { get; set; }
        public Village Village { get; set; }

        public Building()
        {

        }

        public override bool Equals(object obj)
        {
            var building = obj as Building;
            if(building != null)
            {
                if(building.Name == this.Name)
                {
                    if(building.Village == this.Village)
                    {
                        return true;
                    }

                }
            }
            return false;



        }

        public override string ToString()
        {
            return $"{Name}, Wood: {Wood}, Stone: {Stone}, Iron: {Iron}, Level: {Level}, Population: {NeededPopulation}, TargetLevel: {TargetLevel}, MaxLevel: {MaxLevel}, IsBuildeable: {IsBuildeable}, {TimeToCanBuild}";
        }

        public override int GetHashCode()
        {
            var hashCode = 1843764473;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<Village>.Default.GetHashCode(Village);
            return hashCode;
        }

        public static bool operator ==(Building building1, Building building2)
        {
            return EqualityComparer<Building>.Default.Equals(building1, building2);
        }

        public static bool operator !=(Building building1, Building building2)
        {
            return !(building1 == building2);
        }
    }
}

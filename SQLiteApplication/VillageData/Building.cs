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


        public Building()
        {

        }

        public override string ToString()
        {
            return $"{Name}, Wood: {Wood}, Stone: {Stone}, Iron: {Iron}, Level: {Level}, Population: {NeededPopulation}, TargetLevel: {TargetLevel}, MaxLevel: {MaxLevel}, IsBuildeable: {IsBuildeable}, {TimeToCanBuild}";
        }

    }
}

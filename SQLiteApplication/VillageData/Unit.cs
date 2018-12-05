using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.VillageData
{
    public enum Unit {
        [Unit("spears", 50, 20, 20, 1)] SPEARS

    }

    public static class UnitExtension
    {
        public static UnitAttribute GetAttribute(this Unit unit)
        {
            return (UnitAttribute)Attribute.GetCustomAttribute(typeof(Unit).GetField(Enum.GetName(typeof(Unit), unit)), typeof(UnitAttribute));
        }
    }         

    
    public class UnitAttribute : Attribute
    {
        public string Name { get; set; }

        public int Wood { get; set; }
        public int Stone { get; set; }
        public int Iron { get; set; }

        public int NeededPopulation { get; set; }
        public UnitAttribute(string name, int wood, int stone, int iron, int neededPopulation)
        {
            (Name, Wood, Stone, Iron, NeededPopulation) = (name, wood, stone, iron, neededPopulation);
        }
    }


    
}

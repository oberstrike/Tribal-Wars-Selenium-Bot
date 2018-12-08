using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.VillageData
{
    public enum Unit {
        [Unit("spears", 50, 30, 20, 1)] SPEARS,
        [Unit("sword", 30, 30, 70, 1)] SWORD,
        [Unit("axe", 60, 30, 40, 1)] AXE,
        [Unit("spy", 50, 50, 20, 2)] SPY,
        [Unit("light", 125, 100, 250, 4)] LIGHT,
        [Unit("heavy", 200, 150, 600, 6)] HEAVY,
        [Unit("ram", 300, 200, 200, 5)] RAM,
        [Unit("kata", 320, 400, 10, 8)] KATA
    }

    public static class UnitExtension
    {
        public static UnitAttribute GetAttribute(this Unit unit)
        {
            return (UnitAttribute)Attribute.GetCustomAttribute(typeof(Unit).GetField(Enum.GetName(typeof(Unit), unit)), typeof(UnitAttribute));
        }
        
        public static int GetWood(this Unit unit){
            return unit.GetAttribute().Wood;
        }
        
        public static int GetStone(this Unit unit){
            return unit.GetAttribute().Stone;
        }
        
        
        public static int GetIron(this Unit unit){
            return unit.GetAttribute().Iron;
        }
        
        public static string GetName(this Unit unit){
            return unit.GetAttribute().Name;
        }

        public static int GetPopulation(this Unit unit)
        {
            return unit.GetAttribute().NeededPopulation;
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

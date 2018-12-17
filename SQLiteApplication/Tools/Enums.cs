using System;

namespace SQLiteApplication.Tools
{
    public enum Unit
    {
        [Unit(50, 30, 10)] SPEAR,
        [Unit(30, 30, 70)] SWORD,
        [Unit(60, 30, 40)] AXE,
        [Unit(50, 50, 20, 2)] SPY,
        [Unit(125, 100, 250, 4)] LIGHT,
        [Unit(200, 150, 600, 6)] HEAY,
        [Unit(300, 200, 200, 5)] RAM,
        [Unit(320, 400, 100, 8)] CATAPULT,
        [Unit(0, 0, 0, 100)] SNOB
    }
    public static class ExtensionClass
    {
        public static UnitAttribute GetAttribute(this Unit unit)
        {
            Type type = typeof(Unit);
            System.Reflection.MemberInfo[] memInfo = type.GetMember(unit.ToString());
            object[] attributes = memInfo[0].GetCustomAttributes(typeof(UnitAttribute), false);
            return attributes.Length > 0 ? (UnitAttribute)attributes[0] : null;
        }

        public static string GetName(this Unit unit)
        {
            return unit.ToString().ToLower();
        }

        public static int GetWood(this Unit unit)
        {
            return unit.GetAttribute().Wood;
        }

        public static int GetStone(this Unit unit)
        {
            return unit.GetAttribute().Stone;
        }

        public static int GetIron(this Unit unit)
        {
            return unit.GetAttribute().Iron;
        }

        public static int GetNeededPopulation(this Unit unit)
        {
            return unit.GetAttribute().NeededPopulation;
        }
        
        public static void GoTo(this FirefoxDriver driver, string url)
        {
            if(driver.URL != url)
            {
                driver.Navigate().GotoUrl(url);
                Client.Sleep();
            }
        }
    }
    public class UnitAttribute : Attribute
    {
        public int Wood { get; set; }
        public int Stone { get; set; }
        public int Iron { get; set; }
        public int NeededPopulation { get; set; }

        public UnitAttribute(int wood, int stone, int iron, int neededPopulation)
        {
            Wood = wood;
            Stone = stone;
            Iron = iron;
            NeededPopulation = neededPopulation;
        }

        public UnitAttribute(int wood, int stone, int iron) : this(wood, stone, iron, 1)
        {

        }
    }


}

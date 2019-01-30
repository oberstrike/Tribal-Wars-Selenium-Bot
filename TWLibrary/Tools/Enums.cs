using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SQLiteApplication.Web;
using System;
using System.Collections.Generic;

namespace SQLiteApplication.Tools
{
    public enum Unit
    {
        [Unit(50, 30, 10)] SPEAR,
        [Unit(30, 30, 70)] SWORD,
        [Unit(60, 30, 40)] AXE,
        [Unit(50, 50, 20, 2)] SPY,
        [Unit(125, 100, 250, 4)] LIGHT,
        [Unit(200, 150, 600, 6)] HEAVY,
        [Unit(300, 200, 200, 5)] RAM,
        [Unit(320, 400, 100, 8)] CATAPULT,
        [Unit(0, 0, 0, 100)] SNOB
    }
    public static class ExtensionClass
    {
        /// <summary>
        /// Shuffles the element order of the specified list.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        { 
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = new Randomizer().Next(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

        }

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
        
        public static void GoTo(this IWebDriver driver, string url)
        {
            if(driver.Url != url)
            {
                driver.Navigate().GoToUrl(url);
                Client.Sleep();
            }
        }
        public static object ExecuteScript(this IWebDriver driver,string script)
        {
            if (script == null)
                throw new ArgumentNullException();
            return ((IJavaScriptExecutor)driver).ExecuteScript(script);

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

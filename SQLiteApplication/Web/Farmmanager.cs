using OpenQA.Selenium;
using SQLiteApplication.UserData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.VillageData
{
    /*
    Ziel:
        Managen von Zielen die potentielle Ziele sind.

    */
    public class Farmmanager
    {
        public string[] Units { get; } = { "spear", "sword", "axe", "spy", "light", "heavy", "ram", "catapult", "snob"};

        public List<Dictionary<string, double>> Templates { get; set; }

        public List<TWVillage> Villages { get; set; }

        public List<TWVillage> GetVillagesInDistance(int distance, string id)
        {
            TWVillage village = Villages.Where(x => x.Id.Equals(id)).First();
            return Villages.Where(x => GetDistance(x.X, x.Y, village.X, village.Y) <= distance).ToList();


        }

        private double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(y1 - y2, 2) + Math.Pow(x1 - x2, 2));
        }

        public Farmmanager()
        {

        }
    }

    public class TWVillage
    {
        public TWVillage(Dictionary<string,object> webElement)
        {
            Id = (string) webElement["id"];
            Name = (string) webElement["name"];
            Points = (string) webElement["points"];
            Owner = (string) webElement["owner"];
            string xy = ((Int64)webElement["xy"]).ToString();
            X = Double.Parse(xy.Substring(0, 2));
            Y = Double.Parse(xy.Substring(3, 5));
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Points { get; set; }
        public string Owner { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
}

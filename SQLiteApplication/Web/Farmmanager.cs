using SQLiteApplication.UserData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.VillageData
{
    public class Farmmanager
    {
        public string[] Units { get; set; } = { "spear", "sword", "axe", "spy", "light", "heavy", "ram", "catapult", "snob"};

        public List<Dictionary<string, double>> Templates { get; set; }

        public string LastAttackedVillage { get; set; }

        public Farmmanager()
        {

        }
    }
}

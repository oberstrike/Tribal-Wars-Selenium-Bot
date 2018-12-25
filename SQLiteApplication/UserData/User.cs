using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLiteApplication.UserData
{
    public class User
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public int Server { get; set; }
        public String TorBrowserPath { get; set; }
        public bool HasFarmmanager { get; set; }

        public override string ToString()
        {
            return $"Name: {Name}, Server: {Server}";
        }

        public List<Village> Villages { get; set; } = new List<Village>();

        public Village GetVillage(int id)
        {
            return (from village in Villages
                    where village.Id == id
                    select village).First();
        }

        public Village GetVillage(double id){
            return GetVillage(Convert.ToInt32(id));
        }

    }
}

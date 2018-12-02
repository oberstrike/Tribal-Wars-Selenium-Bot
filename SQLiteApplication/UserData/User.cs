using Newtonsoft.Json;
using SQLiteApplication.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLiteApplication.UserData
{
    public class User
    {
        private IList<Village> _villages = new List<Village>();

        public string Name { get; set; }
        public string Password { get; set; }
        public int Server { get; set; }
        public bool HasFarmmanager { get; set; }
        public IList<Village> Villages { get => _villages; set => _villages = value; }

        public User()
        {

        }

        public override string ToString()
        {
            return $"Name: {Name}, Server: {Server}";
        }

        
        public Village GetVillage(string id)
        {
            return (from village in Villages
                    where village.Id == id
                    select village).First();
        }

        public Village GetVillage(object id)
        {
            return GetVillage(id.ToString());
        }
    }
}

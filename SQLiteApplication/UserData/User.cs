using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SQLiteApplication.UserData
{
    public class User
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public int Server { get; set; }
        /*
        private ICollection<Village> villages = new List<Village>();
        
        public ICollection<Village> GetVillages()
        {
            return villages;
        }

        public void SetVillages(ICollection<Village> value)
        {
            villages = value;
        }

        public Village GetVillage(int id)
        {
            return (from village in GetVillages()
                    where village.Id == id
                    select village).First();
        }
        */

    }
}
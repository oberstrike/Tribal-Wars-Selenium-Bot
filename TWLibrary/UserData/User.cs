using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TWLibrary.Tools;

namespace TWLibrary.UserData
{
    public class User : IEncrypted
    {
        public string Name { get; set; } = "Username";
        public string Password { get; set; } = "Passwort";
        public int Server { get; set; } = 160;
        public String TorBrowserPath { get; set; }
        public bool HasFarmmanager { get; set; } = true;
        public bool IsEncrypted { get; set; } = false;
        public bool IsPremium { get; set; } = true;
        public bool Farm { get; set; } = false;
        public bool Build { get; set; } = false;
        public bool Trade { get; set; } = false;
        public string[] FarmingVillages { get; set; } = new string[0];
        public string[] BuildOrder { get; set; } = new string[0];

        private static readonly string _geheim = "geheim";
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

        public void EncryptPassword()
        {
            Password = StringCipher.Encrypt(Password, _geheim);
        }

        public string DecryptPassword()
        {
            return StringCipher.Decrypt(Password, _geheim);
        }
    }
}

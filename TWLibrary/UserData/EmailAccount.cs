using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWLibrary.Tools;

namespace TWLibrary.UserData
{
    public class EmailAccount : IEncrypted
    {
        public string SenderEmail { get; set; } = "meine.email@web.de";
        public string Password { get; set; } = "mein password das verschlüsselt wird.";
        public bool IsEncrypted { get; set; } = false;
        private string _targetEmail = "meine.ziel-email@web.de";
        private readonly string _geheim = "geheim";

        public string TargetEmail {
            get
            {
                return _targetEmail;
            }
            set
            {
                _targetEmail = value;
            }
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

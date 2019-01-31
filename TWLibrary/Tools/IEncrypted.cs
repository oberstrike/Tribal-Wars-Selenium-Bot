using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWLibrary.Tools
{
    interface IEncrypted
    {
        bool IsEncrypted { get; set; }
        void EncryptPassword();
        string DecryptPassword();

    }
}

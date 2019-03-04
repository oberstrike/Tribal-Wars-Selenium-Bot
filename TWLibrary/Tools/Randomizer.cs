using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TWLibrary.Tools
{
    public class Randomizer
    {
        private readonly RandomNumberGenerator provider = new RNGCryptoServiceProvider();

        public int Next(int min, int max)
        {
            if(min > max)
                throw new ArgumentOutOfRangeException();
            return (int)Math.Floor(min + ((double)max - min) * NextDouble());
        }


        public double NextDouble()
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[8];
            provider.GetBytes(buffer);
            var randUint = BitConverter.ToUInt32(buffer, 0);
            return randUint / (uint.MaxValue + 1.0);
        }
    }
}

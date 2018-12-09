using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.VillageData
{
    public sealed class StringToBuildTimeConverter
    {
        public static TimeSpan Convert(string time)
        {
            DateTime now = DateTime.Now;
            string lastLetters = GetLastLetters(time, 5);
            DateTime targetTime = DateTime.Parse(lastLetters);

            if (time.Contains("morgen"))
            {
                targetTime = targetTime.AddHours(24);

            }else if (time.Contains(""))
            {

            }

            return new TimeSpan((targetTime - now).Ticks);
        }

        private static string GetLastLetters(string time, int v)
        {
            string lastLetters = "";
            for(int i = time.Length - 1; i > time.Length - 1 - v; i--)
            {
                lastLetters += time[i];
            }
            return String.Join("", lastLetters.Reverse());
        }
    }
}

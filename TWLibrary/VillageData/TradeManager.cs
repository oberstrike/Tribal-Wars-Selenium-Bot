using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.VillageData
{
    public class TradeManager
    {
        public int AvailableTraders { get; set; } = 0;
        public int TotalTraders { get; set; } = 0;
        public int UsedTrader { get { return TotalTraders - AvailableTraders; } }
        public int AvaibleTradingCapacity { get { return AvailableTraders * 1000; } }
        public int TotalTradingCapacity { get { return TotalTraders * 1000; } }

    }
}

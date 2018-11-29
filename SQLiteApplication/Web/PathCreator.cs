using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication.Web
{
    public class PathCreator
    {
        public string ServerId { get; set; }
        public string VillageId { get; set; }

        private string GetBasePath(){
           return $"https://de{ServerId}.die-staemme.de/game.php?";
        }
        

        public PathCreator(string pServerId, string pVillageId)
        {
            ServerId = pServerId;
            VillageId = pVillageId;

        }

        public string GetMain()
        {
            return $"{GetBasePath()}village={ServerId}&screen=main";
        }

        public string GetPlace()
        {
            return $"{GetBasePath()}village={VillageId}&screen=place&mode=command";
        }

        public string GetLogout(string csrf)
        {
            return $"{GetBasePath()}village={VillageId}&screen=&action=logout&h={csrf}";
        }

        public string GetAttackLink(string target)
        {
            return $"{GetBasePath()}village={VillageId}&screen=place&target={target}";
        }

        public string GetFarmAssist()
        {
            return $"{GetBasePath()}&screen=am_farm";
        }

        public string GetBuildingOverview()
        {
            return $"{GetBasePath()}&screen=overview_villages";
        }

        public string GetMarketModeSend()
        {
            return $"{GetBasePath()}&screen=market&mode=send";
        }

        public string GetBarracks()
        {
            return $"{GetBasePath()}&screen=barracks";
        }

        internal string GetStable()
        {
            return $"https://de{ServerId}.die-staemme.de/game.php?village={VillageId}&screen=stable";
        }

        internal string GetSmith()
        {
            return $"https://de{ServerId}.die-staemme.de/game.php?village={VillageId}&screen=smith";

        }
    }
}

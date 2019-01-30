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
           return $"https://de{ServerId}.die-staemme.de/game.php?village={VillageId}";
        }
        

        public PathCreator(string pServerId, string pVillageId)
        {
            ServerId = pServerId;
            VillageId = pVillageId;

        }
    
        public PathCreator(object pServerId, object pVillageId) : this(pServerId.ToString(), pVillageId.ToString())
        {

        }

        public string GetMain()
        {
            return $"{GetBasePath()}&screen=main";
        }

        public string GetPlace()
        {
            return $"{GetBasePath()}&screen=place&mode=command";
        }

 

        public string GetAttackLink(string target)
        {
            return $"{GetBasePath()}&screen=place&target={target}";
        }

        public string GetFarmAssist()
        {
            return $"{GetBasePath()}&screen=am_farm";
        }

        public string GetOverview()
        {
            return $"{GetBasePath()}&screen=overview_villages&mode=combined";
        }

        public string GetMarketModeSend()
        {
            return $"{GetBasePath()}&screen=market&mode=send";
        }

        public string GetBarracks()
        {
            return $"{GetBasePath()}&screen=barracks";
        }

        public string GetStable()
        {
            return $"{GetBasePath()}&screen=stable";
        }

        public string GetSmith()
        {
            return $"{GetBasePath()}&screen=smith";

        }

        public static string GetOverview(string serverId)
        {
            return $"https://de{serverId}.die-staemme.de/game.php?&screen=overview_villages&mode=combined";
        }

        public static string GetLogout(string serverId)
        {
            return $"https://de{serverId}.die-staemme.de/game.php?&action=logout";
        }
    }
}

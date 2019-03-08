using System.Collections.Generic;
using System.Linq;

namespace TWLibrary.Tools
{
    public class VillageBuilder
    {

        private double _woodLevel;
        private double _stoneLevel;
        private double _ironLevel;
        private double _farmLevel;
        private double _maxStorage;
        private double _wood;
        private double _iron;
        private double _stone;
        private double _woodPop;
        private double _stonePop;
        private double _ironPop;
        private double _freePop;

        public VillageBuilder(Village village)
        {

            _woodLevel = village.GetBuilding("wood").Level;
            _stoneLevel = village.GetBuilding("stone").Level;
            _ironLevel = village.GetBuilding("iron").Level;
            //_farmLevel = 10;
            _farmLevel = village.GetBuilding("farm").Level;
            _maxStorage = village.RManager.StorageMax;
            _wood = village.RManager.Wood;
            _iron = village.RManager.Iron;
            _stone = village.RManager.Stone;


            _woodPop = village.GetBuilding("wood").NeededPopulation;
            _stonePop = village.GetBuilding("stone").NeededPopulation;
            _ironPop = village.GetBuilding("iron").NeededPopulation;
            _freePop = village.RManager.MaxPopulation - village.RManager.Population;
           

        }

        public string[] GetNextRessourceBuildings(int count)
        {
            string[] targets = new string[count];

            for(int i = 0; i < count; i++)
            {
                string target = GetNextResourceBuilding();


            }


            return null;
        }

        public string GetNextResourceBuilding()
        {



            KeyValuePair<string, double> target = GetNextResource();

            double targetPop = target.Value;
            if (targetPop > _freePop)
            {
                if (_farmLevel < 30)
                    return "farm";
                else
                    return null;
            }
            else
            {

                return target.Key;
            }
        }

        private KeyValuePair<string, double> GetNextResource()
        {
            KeyValuePair<string, double> target = new KeyValuePair<string, double>();

            if (_woodLevel - _stoneLevel > 0)
            {
                if (_stoneLevel - _ironLevel > 2)
                    target = new KeyValuePair<string, double>("iron", _ironPop);
                else
                    target = new KeyValuePair<string, double>("stone", _stonePop);
            }
            else if (_woodLevel - _ironLevel > 0)
            {
                target = new KeyValuePair<string, double>("iron", _ironPop);
            }
            else
            {
                if (_stone > _wood)
                    target = new KeyValuePair<string, double>("wood", _woodPop);
                else
                    target = new KeyValuePair<string, double>("stone", _stonePop);
            }

            return target;
        }
    }
}

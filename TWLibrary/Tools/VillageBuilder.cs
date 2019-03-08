using System;
using System.Collections.Generic;
using System.Reflection;

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

        

        public double WoodLevel { get => _woodLevel; set => _woodLevel = value; }
        public double StoneLevel { get => _stoneLevel; set => _stoneLevel = value; }
        public double IronLevel { get => _ironLevel; set => _ironLevel = value; }
        public double FarmLevel { get => _farmLevel; set => _farmLevel = value; }
        public double MaxStorage { get => _maxStorage; set => _maxStorage = value; }
        public double Wood { get => _wood; set => _wood = value; }
        public double Iron { get => _iron; set => _iron = value; }
        public double Stone { get => _stone; set => _stone = value; }
        public double WoodPop { get => _woodPop; set => _woodPop = value; }
        public double StonePop { get => _stonePop; set => _stonePop = value; }
        public double IronPop { get => _ironPop; set => _ironPop = value; }
        public double FreePop { get => _freePop; set => _freePop = value; }

        public VillageBuilder(Village village)
        {

            _woodLevel = village.GetBuilding("wood").Level;
            StoneLevel = village.GetBuilding("stone").Level;
            IronLevel = village.GetBuilding("iron").Level;
            //_farmLevel = 10;
            FarmLevel = village.GetBuilding("farm").Level;
            MaxStorage = village.RManager.StorageMax;
            Wood = village.RManager.Wood;
            Iron = village.RManager.Iron;
            Stone = village.RManager.Stone;


            WoodPop = village.GetBuilding("wood").NeededPopulation;
            StonePop = village.GetBuilding("stone").NeededPopulation;
            IronPop = village.GetBuilding("iron").NeededPopulation;
            FreePop = village.RManager.MaxPopulation - village.RManager.Population;


        }

        public string[] GetNextRessourceBuildings(int count)
        {
            string[] targets = new string[count];

            for (int i = 0; i < count; i++)
            {
                string target = GetNextResourceBuilding();
                Console.WriteLine("Target= " + target);
                UpdateTarget(target);
            }


            return null;
        }

        private void UpdateTarget(string target)
        {
            PropertyInfo[] types = GetType().GetProperties();
            Console.WriteLine(types.Length);

            foreach (PropertyInfo type in types)
            {

                if (type.Name.ToLower().Contains(target) && type.Name.Contains("Level"))
                {
                    Console.WriteLine("Update: " + type.Name);
                    double value = (double)type.GetValue(this);
                    double newValue = value + 1;


                    Console.WriteLine("Old: " + value);
                    Console.WriteLine("New: " + newValue);
                    type.SetValue(this, newValue);

                }

            }


        }

        public string GetNextResourceBuilding()
        {


            KeyValuePair<string, double> target = GetNextResource();

            double targetPop = target.Value;
            if (targetPop > FreePop)
            {
                if (FarmLevel < 30)
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
            Console.WriteLine(this);
            if (WoodLevel > StoneLevel | WoodLevel < StoneLevel)
            {
                if (StoneLevel > IronLevel + 2)
                    target = new KeyValuePair<string, double>("iron", IronPop);
                else if (WoodLevel > StoneLevel)
                    target = new KeyValuePair<string, double>("stone", StonePop);
                else
                    target = new KeyValuePair<string, double>("wood", WoodPop);
            }
            else if (WoodLevel < IronLevel)
            {
                target = new KeyValuePair<string, double>("iron", IronPop);
            }
            else
            {
                if (Stone > Wood)
                    target = new KeyValuePair<string, double>("wood", WoodPop);
                else
                    target = new KeyValuePair<string, double>("stone", StonePop);
            }

            return target;
        }

        public override string ToString()
        {
            return $"Wood: {WoodLevel}, Stone: {StoneLevel}, Iron: {IronLevel}";
        }

    }
}

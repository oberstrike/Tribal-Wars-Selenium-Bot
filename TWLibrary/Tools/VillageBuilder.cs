using System;
using System.Collections.Generic;
using System.Reflection;

namespace TWLibrary.Tools
{
    public class VillageBuilder
    {

        public double StorageLevel { get; private set; }
        public double WoodLevel { get; set; }
        public double StoneLevel { get; set; }
        public double IronLevel { get; set; }
        public double FarmLevel { get; set; }
        public double MainLevel { get; set; }
        public double MaxStorage { get; set; }
        public double Wood { get; set; }
        public double Iron { get; set; }
        public double Stone { get; set; }
        public double WoodPop { get; }
        public double StonePop { get; }
        public double IronPop { get; }
        public double FreePop { get; }
        public double MainPop { get; }
        public double WoodProd { get; set; }
        public double StoneProd { get; set; }
        public double IronProd { get; set; }
        public VillageBuilder(Village village)
        {

            WoodLevel = village.GetBuilding("wood").Level;
            StoneLevel = village.GetBuilding("stone").Level;
            IronLevel = village.GetBuilding("iron").Level;
            StorageLevel = village.GetBuilding("storage").Level;
            FarmLevel = village.GetBuilding("farm").Level;
            MainLevel = village.GetBuilding("main").Level;



            MaxStorage = village.RManager.StorageMax;
            Wood = village.RManager.Wood;
            Iron = village.RManager.Iron;
            Stone = village.RManager.Stone;


            WoodPop = village.GetBuilding("wood").NeededPopulation;
            StonePop = village.GetBuilding("stone").NeededPopulation;
            IronPop = village.GetBuilding("iron").NeededPopulation;
            FreePop = village.RManager.MaxPopulation - village.RManager.Population;
            MainPop = village.GetBuilding("main").NeededPopulation;

            WoodProd = village.RManager.WoodProduction;
            StoneProd = village.RManager.StoneProduction;
            IronProd = village.RManager.IronProduction;


        }
        public string[] GetNextRessourceBuildings(int count)
        {
            string[] targets = new string[count];

            for (int i = 0; i < count; i++)
            {
                string target = GetNextResourceBuilding();
                UpdateTarget(target);
            }
            return targets;
        }
        private void UpdateTarget(string target)
        {
            PropertyInfo[] types = GetType().GetProperties();
            foreach (PropertyInfo type in types)
            {

                if (type.Name.ToLower().Contains(target) && type.Name.Contains("Level"))
                {
                    double value = (double)type.GetValue(this);
                    double newValue = value + 1;
                    type.SetValue(this, newValue);
                }
            }
        }
        public string GetNextResourceBuilding()
        {
            KeyValuePair<string, double> target = GetNextResource();
            if (target.Key == null)
                return null;

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
        public string[] GetNextNormalBuildings(int count)
        {
            string[] targets = new string[count];

            for (int i = 0; i < count; i++)
            {
                string target = GetNextNormalBuilding();
                if (target != null)
                    UpdateTarget(target);
            }
            return targets;
        }
        public string GetNextNormalBuilding()
        {
            KeyValuePair<string, double> target = GetNextNormal();
            if (target.Key != null)
            {
                if (target.Key == "main")
                    if (FreePop >= MainPop)
                        return target.Key;
                return "storage";
            }
            return null;

        }
        private KeyValuePair<string, double> GetNextNormal()
        {
            KeyValuePair<string, double> target = new KeyValuePair<string, double>();
            Console.WriteLine((WoodLevel + IronLevel + StoneLevel) / 3.4);
            if ((WoodLevel + IronLevel + StoneLevel) / 3.4 - 1 > MainLevel && MainLevel < 21)
            {
                target = new KeyValuePair<string, double>("main", MainPop);
            }
            else if (MaxStorage < Wood + WoodProd || MaxStorage < Iron + IronProd || MaxStorage < Stone + StoneProd)
            {
                target = new KeyValuePair<string, double>("storage", 0);
            }
            return target;
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
                if (StoneLevel == 30 && IronLevel == 30 && StoneLevel == 30)
                    return target;
                else if (Stone > Wood)
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

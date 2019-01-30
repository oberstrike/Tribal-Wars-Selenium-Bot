using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteApplication
{
    public class BuildingBuilder
    {

        private Building instance;

        public BuildingBuilder()
        {
            instance = new Building();
        }

        public BuildingBuilder WithWood(double wood)
        {
            instance.Wood = wood;
            return this;
        }

        public BuildingBuilder WithStone(double stone)
        {
            instance.Stone = stone;
            return this;
        }

        public BuildingBuilder WithName(string name)
        {
            instance.Name = name;
            return this;
        }

        public BuildingBuilder WithIron(double iron)
        {
            instance.Iron = iron;
            return this;
        }
        public BuildingBuilder WithPopulation(double population)
        {
            instance.NeededPopulation = population;
            return this;
        }
        public BuildingBuilder WithLevel(double level)
        {
            instance.Level = level;
            return this;
        }

        public BuildingBuilder WithTargetLevel(double targetLevel)
        {
            instance.TargetLevel = targetLevel;
            return this;
        }

        public BuildingBuilder WithMaxLevel(double maxLevel)
        {
            instance.MaxLevel = maxLevel;
            return this;
        }
       
        public BuildingBuilder WithBuildeable(bool isBuildeable)
        {
            instance.IsBuildeable = isBuildeable;
            return this;
        }

        public Building Build()
        {
            if (instance.Level == instance.MaxLevel)
                instance.IsBuildeable = false;
            return instance;
        }

        public BuildingBuilder WithBuildingTime(TimeSpan? dateTime)
        {
            if (dateTime.HasValue)
            {
                instance.TimeToCanBuild = dateTime.Value;
            }
            return this;
        }

        internal BuildingBuilder WithVillage(Village village)
        {
            instance.Village = village;
            return this;

        }
    }
}

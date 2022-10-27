using UnityEngine;
using System.Collections.Generic;
using System;

namespace KK
{
    public class PlayerStats : MonoBehaviour
    {
        public Dictionary<StatType, int> Stats { get; private set; }

        public enum StatType
        {
            Strength,
            Agility,
            Endurance,
            Intelligence,
            Spirit,
            Armor
        }

        void Awake()
        {
            Stats = new Dictionary<StatType, int>();

            foreach (StatType statType in Enum.GetValues(typeof(StatType))){
                Stats.Add(statType, 0);
            }
        }

        public void AddToStat(StatType statType, int value)
        {
            Stats[statType] += value;
        }

        public int GetValue(StatType statType)
        {
            return Stats[statType];
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new AllModifiersHolder", menuName = "Single/AllModifiersHolder")]
public class SOAllModifiersHolder : ScriptableObject
{
    [SerializeField] private List<SOStatLevelModifier> AllModifiersHolders = new List<SOStatLevelModifier>();

    public float? GetStatValue(EStatType stat, int level)
    {
        foreach (var statModifier in AllModifiersHolders)
        {
            if (statModifier.StatType == stat)
            {
                return GetVal(level, statModifier.StatGrowthValue, 
                    statModifier.BaseStatValue) / statModifier.StatFactor;
            }
        }

        return null;
    }

    public int? GetStatThreshold(EStatType stat, int currentLevel)
    {
        foreach (var statModifier in AllModifiersHolders)
        {
            if (statModifier.StatType == stat)
            {
                return (int)GetVal(currentLevel, statModifier.LevelThresholdModifier,
                        statModifier.BaseLevelThreshold);
            }
        }

        return null;
    }

    private float GetVal(int n, float value, float baseVal)
    {
        if (n < 0) return baseVal;

        var res = GetVal(n - 1, value, baseVal);
        var add = n * value;
        res += add != 0 ? add : 1;

        return res;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity: MonoBehaviour
{
    [Header("Entity settings")] [SerializeField]
    private SOAllModifiersHolder modifiers;

    // StatType, CurrentLevel, CurrentDataSize
    private Dictionary<EStatType, CurrentStatStatus> Stats = new Dictionary<EStatType, CurrentStatStatus>();

    protected float msDisadvantage = 0;

    protected void SetupEntity()
    {
        foreach (var stat in (EStatType[])Enum.GetValues(typeof(EStatType)))
        {
            Stats.Add(stat, new CurrentStatStatus(0, 0));
        }
    }

    protected float GetStat(EStatType statType)
    {
        var retVal = modifiers.GetStatValue(statType, Stats[statType].currentLevel);
        return retVal ?? 1;
    }

    protected int GetStatLevel(EStatType statType)
    {
        return Stats[statType].currentLevel;
    }

    protected int GetStatDataSize(EStatType statType)
    {
        return Stats[statType].currentDataSize;
    }

    protected int GetNextLevelCount(EStatType statType)
    {
        var retVal = modifiers.GetStatThreshold(statType, Stats[statType].currentLevel);
        return retVal ?? 1;
    }

    /// <summary>
    /// Entity base function.
    /// </summary>
    /// <param name="statType">Type of stat to check.</param>
    /// <returns>Data quantity needed to level up given stat</returns>
    protected int GetMissingDataSize(EStatType statType)
    {
        return GetNextLevelCount(statType) - GetStatDataSize(statType);
    }

    [System.Serializable]
    private struct CurrentStatStatus
    {
        public int currentLevel;
        public int currentDataSize;

        public CurrentStatStatus(int level, int data)
        {
            currentLevel = level;
            currentDataSize = data;
        }
    }
}
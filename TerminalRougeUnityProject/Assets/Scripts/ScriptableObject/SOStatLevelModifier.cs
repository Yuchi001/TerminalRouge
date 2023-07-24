using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new StatLevelModifier", menuName = "StatLevelModifier", order = 3)]
public class SOStatLevelModifier : UnityEngine.ScriptableObject
{
    public EStatType StatType;
    
    [Header("Stats")]
    [Range(0, 100)] public float BaseStatValue = 0;
    [Range(0, 20)] public float StatGrowthValue = 0;
    [Range(1, 1000)] public float StatFactor = 1;

    [Header("Level")]
    [Range(0, 100)] public int BaseLevelThreshold = 0;
    [Range(0, 20)] public int LevelThresholdModifier = 0;
}
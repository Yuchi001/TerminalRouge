using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new StatLevelModifier", menuName = "StatLevelModifier", order = 0)]
public class SOStatLevelModifier : ScriptableObject
{
    public EStatType StatType;
    public List<SStatModifier> ModifierList = new List<SStatModifier>();
}

[System.Serializable]
public struct SStatModifier
{
    public int levelUpCount;
    public float modifier;
}
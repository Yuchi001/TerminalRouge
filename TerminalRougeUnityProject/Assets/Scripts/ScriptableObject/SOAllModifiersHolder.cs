using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new AllModifiersHolder", menuName = "Single/AllModifiersHolder")]
public class SOAllModifiersHolder : ScriptableObject
{
    [SerializeField] private List<SOStatLevelModifier> AllModifiersHolders = new List<SOStatLevelModifier>();

    public float? GetStatModifier(EStatType stat, int level)
    {
        foreach (var statModifier in AllModifiersHolders)
        {
            if (statModifier.StatType == stat)
            {
                return statModifier.ModifierList.Count < level ? statModifier.ModifierList[level].modifier : null;
            }
        }

        return null;
    }

    public int? GetStatNextLevelCount(EStatType stat, int currentLevel)
    {
        foreach (var statModifier in AllModifiersHolders)
        {
            if (statModifier.StatType == stat)
            {
                return statModifier.ModifierList.Count < currentLevel ? statModifier.ModifierList[currentLevel].levelUpCount : null;
            }
        }

        return null;
    }
}
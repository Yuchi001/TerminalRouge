using System.Collections.Generic;
using System.Linq;
using Helpers;
using UnityEngine;

public class SOMethod : ScriptableObject
{
    [SerializeField] protected string methodName;
    [SerializeField, Range(-1, 5)] protected int parametersCount = 0;
    [SerializeField] protected List<EFlagType> flags = new List<EFlagType>();

    [SerializeField, TextArea(5, 20)] protected string generalInfo;
    [SerializeField] protected List<SerializableKeyValPair<string, string>> parametersInfo;
    [SerializeField] protected List<SerializableKeyValPair<EFlagType, string>> flagInfo;
    protected int flagsCount => flags.Count;

    public void RunMethod(Terminal terminal, List<string>
        passedParameters = default, List<string> passedFlags = default)
    {
        passedParameters ??= new List<string>();
        passedFlags ??= new List<string>();

        if (passedParameters.Count != parametersCount && parametersCount != -1)
        {
            terminal.Error_WrongParametersCount(parametersCount, passedParameters.Count);
            return;
        }
        
        OverrideMethod(terminal, passedParameters, passedFlags);
    }

    public string GetMethodName()
    {
        return methodName;
    }
    protected virtual void OverrideMethod(Terminal terminal, List<string>
        passedParameters, List<string> passedFlags)
    {
        
    }

    public string GetMethodInfo()
    {
        var info = "==================== I N F O =====================@";
        info +=$"{methodName.Capitalize()}: {generalInfo}@{(parametersCount == 0 ? "" : "---------------------------- p a r a m s ----------------------------")}@";

        var paramInfoCount = parametersCount != -1 ? parametersCount : 1;
        if (paramInfoCount > parametersInfo.Count)
            PopulateInfoLists_EM();
        
        info = parametersInfo.Aggregate(info, (current, param) => current + $"{param.key}: {param.value}.@");
        info += $"{(flagsCount == 0 ? "" : "------------------------------ f l a g s --------------------------------")}@";
        info = flagInfo.Aggregate(info, (current, flag) => current + $"-{flag.key}: {flag.value}.@");
        info += "===================== E N D ======================@";
        return info;
    }

    /// <summary>
    /// IT IS EDITOR RUNTIME ONLY METHOD!!!
    /// </summary>
    public void PopulateInfoLists_EM()
    {
        Debug.Log("EditorMethod!");

        var paramRealCount = parametersCount != -1 ? parametersCount : 1;
        if (parametersInfo.Count > paramRealCount)
        {
            parametersInfo.RemoveRange(paramRealCount, parametersInfo.Count - paramRealCount);
        }
        
        for (var i = 0; i < paramRealCount; i++)
        {
            if (parametersInfo.Count > i)
                continue;
            
            var pair = new SerializableKeyValPair<string, string>("<param_name>", "<param_description>");
            parametersInfo.Add(pair);
        }
        
        if (flagInfo.Count > flagsCount)
        {
            flagInfo.RemoveRange(flagsCount, flagInfo.Count - flagsCount);
        }
        for (var i = 0; i < flagsCount; i++)
        {
            if (flagInfo.Count > i)
                if(flagInfo[i].key == flags[i])
                    continue;

            var pair = new SerializableKeyValPair<EFlagType, string>(flags[i], "<flag_description>");
            flagInfo.Add(pair);
        }
    }
}
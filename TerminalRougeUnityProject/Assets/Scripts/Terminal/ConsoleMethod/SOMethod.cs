using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public  class SOMethod : ScriptableObject
{
    [SerializeField] protected string methodName;
    [SerializeField, Range(0, 5)] protected int parametersCount = 0;
    [SerializeField] protected List<EFlagType> flags = new List<EFlagType>();

    protected int flagsCount => flags.Count;

    public void RunMethod(Terminal terminal, List<string>
        passedParameters = default, List<string> passedFlags = default)
    {
        passedParameters ??= new List<string>();
        passedFlags ??= new List<string>();

        if (passedParameters.Count != parametersCount)
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
    
}
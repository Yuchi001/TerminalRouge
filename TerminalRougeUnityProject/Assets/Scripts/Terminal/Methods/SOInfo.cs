using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Methods/Info", fileName = "new InfoMethod")]
public class SOInfo : SOMethod
{
    [SerializeField] private SOAllMethods allMethods;

    [SerializeField, TextArea(5, 20)] private string methodNameNotFoundErrorMessage;
    protected override void OverrideMethod(Terminal terminal, List<string>
        passedParameters, List<string> passedFlags)
    {
        var parameterMethodName = passedParameters[0].ToLower();
        foreach (var method in allMethods.AllMethods)
        {
            if (method.GetMethodName().ToLower() != parameterMethodName) continue;
            
            var info = method.GetMethodInfo();
            terminal.Print(info, true);
            return;
        }
        
        terminal.Error_CustomError(methodNameNotFoundErrorMessage);
    }
}
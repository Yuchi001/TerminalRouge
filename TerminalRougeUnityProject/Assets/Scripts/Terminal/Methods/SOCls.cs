using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Methods/Cls", fileName = "new ClsMethod")]
public class SOCls : SOMethod
{
    protected override void OverrideMethod(Terminal terminal, List<string> passedParameters, List<string> passedFlags)
    {
        terminal.ClearConsole();
    }
}
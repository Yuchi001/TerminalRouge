
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Methods/Echo", fileName = "new EchoMethod")]
public sealed class SOEcho : SOMethod
{
    protected override void OverrideMethod(Terminal terminal, List<string> passedParameters, List<string> passedFlags)
    {
        terminal.Print(passedParameters[0], true);
    }
}
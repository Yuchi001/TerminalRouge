using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Methods/Count", fileName = "new CountMethod")]
public class SOCount : SOMethod
{
    [SerializeField, TextArea(5, 20)] private string errorMessage;
    protected override void OverrideMethod(Terminal terminal, List<string> passedParameters, List<string> passedFlags)
    {
        if (int.TryParse(passedParameters[0], out var number))
        {
            for (var i = 0; i < number; i++)
            {
                terminal.Print(i.ToString(), true);
            }

            return;
        }
        
        terminal.Error_CustomError(errorMessage);
    }
}
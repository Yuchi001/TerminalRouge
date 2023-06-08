using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Methods/Count", fileName = "new CountMethod")]
public class SOCount : SOMethod
{
    [SerializeField, Range(1, 999)] private int countLimit = 100;
    [SerializeField, TextArea(5, 20)] private string parseToIntErrorMessage;
    [SerializeField, TextArea(5, 20)] private string tooBigNumErrorMessage;
    protected override void OverrideMethod(Terminal terminal, List<string> passedParameters, List<string> passedFlags)
    {
        if (int.TryParse(passedParameters[0], out var number) == false)
        {
            terminal.Error_CustomError(parseToIntErrorMessage);
            return;
        }

        if (number > countLimit)
        {
            terminal.Error_CustomError(tooBigNumErrorMessage);
            return;
        }
            
        for (var i = 0; i <= number; i++)
        {
            terminal.Print(i.ToString(), true);
        }
    }
}
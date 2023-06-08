using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubmitedTextBehaviour : MonoBehaviour
{
    [SerializeField] private Vector2 spawnOffset;
    [SerializeField, Range(0, 20)] private int maxWordLength = 8;
    private TextMeshProUGUI textField => GetComponentInChildren<TextMeshProUGUI>();
    public (int lines, float yPos) SetTextField(string text, int maxLineCount, bool asText)
    {
        var lines = ManageSetTextField(text, maxLineCount, asText);
        transform.position += (Vector3)spawnOffset;
        var yPos = textField.preferredHeight;
        return (lines, yPos);
    }

    public TextMeshProUGUI GetTextField()
    {
        return textField;
    }
    private int ManageSetTextField(string text, int maxLineCount, bool asText)
    {
        text = text.Replace('\n', '\0');
        text = text.Replace("PS > ", "");
        var lines = 1;
        var wordLength = 0;
        var insertLine = false;
        for (var i = 0; i < text.Length; i++)
        {
            wordLength += text[i] == ' ' ? -wordLength : 1;
            if ((i >= maxLineCount && i % maxLineCount == 0) && text[i] != '@')
                insertLine = true;

            if ((insertLine && (text[i] == ' ' || wordLength >= maxWordLength)) || text[i] == '@')
            {
                lines++;
                if (text[i] == '@')
                {
                    var sb = new StringBuilder(text){
                        [i] = '\n'
                    };
                    text = sb.ToString();
                }
                else text = text.Insert(i + 1, "\n");
                
                wordLength = 0;
                insertLine = false;
            }
        }
        textField.text = (asText ? "" : "PS > ") +  text;

        return lines;
    }
}
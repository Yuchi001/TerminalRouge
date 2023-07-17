using System;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubmitedTextBehaviour : MonoBehaviour
{
    [SerializeField, Range(0, 100)] private float maxLinePercent = 90;
    [SerializeField] private Vector2 spawnOffset;
    [SerializeField, Range(0, 20)] private int maxWordLength = 8;
    private TextMeshProUGUI textField => GetComponentInChildren<TextMeshProUGUI>();
    private RectTransform viewPortRect => Terminal.Instance.GetViewPortRect();
    public float SetTextField(string text, bool asText)
    {
        ManageSetTextField(text, asText);
        transform.position += (Vector3)spawnOffset;
        var height = textField.preferredHeight;
        return height;
    }

    public TextMeshProUGUI GetTextField()
    {
        return textField;
    }
    private void ManageSetTextField(string text, bool asText)
    {
        //text = text.Replace('\n', '\0');
        var maxLineWidth = viewPortRect.rect.width * (maxLinePercent / 100f);
        textField.text = asText ? "" : "PS > ";
        for (var i = 0; i < text.Length; i++)
        {
            textField.text += text[i];
            if(textField.preferredWidth < maxLineWidth)
                continue;

            var wordLength = 0;
            for (var j = textField.text.Length - 1; j < textField.text.Length; j--)
            {
                wordLength++;
                if (textField.text[j] == ' ' || wordLength >= maxWordLength)
                    break;
            }
            textField.text = textField.text.Insert(textField.text.Length - wordLength + 1, "\n");
        }

        //textField.text = (asText ? "" : "PS > ") + text;
    }
}
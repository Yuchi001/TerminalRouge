using TMPro;
using UnityEngine;

public class SubmitedTextBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject preText;
    [SerializeField, Range(-1, 0)] private float xOffSet = -0.33f;
    [SerializeField, Range(0, 20)] private int maxWordLength = 8; 
    [SerializeField, Range(0, 20)] private int lineHeight = 12; 
    [SerializeField, Range(0, 20)] private float characterWidth = 1.5f;
    [SerializeField, Range(0, 5)] private float lineMultiplier = 1.75f;
    [SerializeField, Range(0, 1000)] private int defaultWidth = 700;
    private TMP_InputField textField => GetComponent<TMP_InputField>();
    public int SetTextField(string text, int maxLineCount, bool asText)
    {
        var result = ManageSetTextField(text, maxLineCount);
        var lines = result.lines;
        var maxLineWidth = result.maxLineWidth;
        
        transform.position += new Vector3(asText ? xOffSet : 0, 0, 0);
        textField.readOnly = true;
        preText.SetActive(!asText);
        
        var parentRectTransform = textField.textComponent.transform.parent.GetComponent<RectTransform>(); 
        parentRectTransform.offsetMin -= new Vector2(0, lineHeight * lines);
        
        var offsetMaxXScale = defaultWidth - (maxLineWidth * characterWidth) * lineMultiplier - (maxLineWidth < maxLineCount ? maxLineWidth : 0);
        parentRectTransform.offsetMax = new Vector2(-offsetMaxXScale, parentRectTransform.offsetMax.y);
        
        return lines;
    }

    private (int lines, float maxLineWidth) ManageSetTextField(string text, int maxLineCount)
    {
        text = text.Replace('\n', '\0');
        var lines = 1;
        var wordLength = 0;
        var insertLine = false;
        var lineStartIndex = 0;
        var maxLineWidth = 0;
        for (var i = 0; i < text.Length; i++)
        {
            wordLength += text[i] == ' ' ? -wordLength : 1;
            if (i >= maxLineCount && i % maxLineCount == 0)
                insertLine = true;

            if (insertLine && (text[i] == ' ' || wordLength >= maxWordLength))
            {
                lines++;
                var singleLineLength = text.Substring(lineStartIndex, i - lineStartIndex).Length;
                maxLineWidth = maxLineWidth < singleLineLength ? singleLineLength : maxLineWidth;
                text = text.Insert(i + 1, "\n");
                lineStartIndex = i;
                wordLength = 0;
                insertLine = false;
            }

            if (i == text.Length - 1)
            {
                var singleLineLength = text.Substring(lineStartIndex, i - lineStartIndex).Length;
                maxLineWidth = maxLineWidth < singleLineLength ? singleLineLength : maxLineWidth;
            }
        }
        textField.text = text;

        return (lines, maxLineWidth);
    }
}
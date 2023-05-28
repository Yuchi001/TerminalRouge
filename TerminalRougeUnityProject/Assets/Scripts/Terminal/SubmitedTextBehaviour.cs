using TMPro;
using UnityEngine;

public class SubmitedTextBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject preText;
    [SerializeField, Range(-1, 0)] private float xOffSet = -0.33f;
    [SerializeField, Range(0, 20)] private int maxWordLength = 8; 
    [SerializeField, Range(0, 20)] private int lineHeight = 12; 
    [SerializeField, Range(0, 5)] private float characterWidth = 1.5f; 
    private TMP_InputField textField => GetComponent<TMP_InputField>();

    public int SetTextField(string text, int maxLineCount, bool asText)
    {
        transform.position += new Vector3(asText ? xOffSet : 0, 0, 0);
        text = text.Replace('\n', '\0');
        var lines = 1;
        var wordLength = 0;
        var insertLine = false;
        var lineStartIndex = 0;
        var maxLineWidth = 0;
        for (var i = 0; i < text.Length; i++)
        {
            wordLength += 1;
            if (i >= maxLineCount && i % maxLineCount == 0)
                insertLine = true;

            if (insertLine && (text[i] == ' ' || wordLength >= maxWordLength))
            {
                lines++;
                var singleLineLength = text.Substring(lineStartIndex, i - lineStartIndex).Length;
                maxLineWidth = maxLineWidth < singleLineLength ? singleLineLength : maxLineWidth;
                text = text.Insert(i, "\n");
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
        textField.readOnly = true;
        var parentRectTransform = textField.textComponent.transform.parent.GetComponent<RectTransform>(); 
        parentRectTransform.offsetMin -= new Vector2(0, lineHeight * lines);
        parentRectTransform.offsetMax -= new Vector2(maxLineWidth * characterWidth, 0);
        preText.SetActive(!asText);
        Debug.Log(parentRectTransform.offsetMax.x);
        return lines;
    }
}
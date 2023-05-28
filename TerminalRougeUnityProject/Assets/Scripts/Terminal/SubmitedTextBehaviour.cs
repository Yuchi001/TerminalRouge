using TMPro;
using UnityEngine;

public class SubmitedTextBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject preText;
    [SerializeField, Range(-1, 0)] private float xOffSet = -0.33f;
    [SerializeField, Range(0, 20)] private int maxWordLength = 8; 
    [SerializeField, Range(0, 20)] private int lineHeight = 12; 
    private TMP_InputField textField => GetComponent<TMP_InputField>();

    public int SetTextField(string text, int maxLineCount, bool asText)
    {
        preText.SetActive(!asText);
        transform.position += new Vector3(asText ? xOffSet : 0, 0, 0);
        text = text.Replace('\n', ' ');
        var lines = 1;
        var wordLength = 0;
        var insertLine = false;
        for (var i = 0; i < text.Length; i++)
        {
            wordLength += 1;
            if (i >= maxLineCount && i % maxLineCount == 0)
                insertLine = true;

            if (insertLine && (text[i] == ' ' || wordLength >= maxWordLength))
            {
                lines++;
                text = text.Insert(i, "\n");
                wordLength = 0;
                insertLine = false;
            }
        }
        textField.text = text;
        textField.readOnly = true;
        textField.textComponent.transform.parent.GetComponent<RectTransform>().offsetMin -= new Vector2(0, lineHeight * lines);
        return lines;
    }
}
using TMPro;
using UnityEngine;

public class SubmitedTextBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject preText;
    private TMP_InputField textField => GetComponent<TMP_InputField>();

    public int SetTextField(string text, int maxLineCount, bool asText)
    {
        preText.SetActive(!asText);
        var lines = 1;
        var insertLine = false;
        for (var i = 0; i < text.Length; i++)
        {
            if (i >= maxLineCount && i % maxLineCount == 0)
                insertLine = true;

            if (insertLine && text[i] == ' ')
            {
                lines++;
                text = text.Insert(i, "\n");
                insertLine = false;
            }
        }
        textField.text = text;
        textField.readOnly = true;
        textField.textComponent.transform.parent.GetComponent<RectTransform>().offsetMin -= new Vector2(0, 14 * (lines - 1));
        return lines;
    }
}
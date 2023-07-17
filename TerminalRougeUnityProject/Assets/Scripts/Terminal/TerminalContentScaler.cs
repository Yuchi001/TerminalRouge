using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerminalContentScaler : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Terminal terminal;

    private void Awake()
    {
        Terminal.OnLinesPrintedEvent += SetContentHeight;
    }

    private void OnDisable()
    {
        Terminal.OnLinesPrintedEvent -= SetContentHeight;
    }

    public void SetContentHeight()
    {
        float height = 0;
        foreach (Transform child in transform)
        {
            if(!child.TryGetComponent<SubmitedTextBehaviour>(out var textBehaviour))
                continue;

            var textField = textBehaviour.GetTextField();
            float childHeight = textField.preferredHeight;
            height += childHeight + terminal.GetPadding();
        }

        content.sizeDelta = new Vector2(content.sizeDelta.x, height);
        scrollRect.verticalNormalizedPosition = 0;
    }
}

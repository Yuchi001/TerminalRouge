using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerminalContentScaler : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    [SerializeField] private Scrollbar scrollbar;

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
        float height = content.anchoredPosition.y;
        foreach (Transform child in transform)
        {
            if(!child.TryGetComponent<SubmitedTextBehaviour>(out var textBehaviour))
                continue;

            var textField = textBehaviour.GetTextField();
            float childHeight = textField.preferredHeight;
            height += childHeight;
        }

        content.sizeDelta = new Vector2(content.sizeDelta.x, height);
        scrollbar.SetValueWithoutNotify(0);
    }
}

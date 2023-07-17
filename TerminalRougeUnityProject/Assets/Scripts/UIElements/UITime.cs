using System;
using TMPro;
using UnityEngine;

namespace UIElements
{
    public class UITime : MonoBehaviour
    {
        private TextMeshProUGUI timeText;

        private void Awake()
        {
            timeText = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            timeText.text = $"{DateTime.Now:h:mm tt}";
        }
    }
}
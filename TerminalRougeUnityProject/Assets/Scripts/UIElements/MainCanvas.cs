using System;
using UnityEngine;

namespace UIElements
{
    public class MainCanvas : MonoBehaviour
    {
        public static MainCanvas Instance { get; private set; }
        private Canvas canvas;

        private void Awake()
        {
            if (Instance != this && Instance != null) Destroy(gameObject);
            else Instance = this;

            canvas = GetComponent<Canvas>();
        }

        public Canvas GetCanvas()
        {
            return canvas;
        }
    }
}
using System;
using UnityEngine;

namespace UIElements
{
    public class Desktop : MonoBehaviour
    {
        public void OnMouseDown()
        {
            var focusableUIList = FindObjectsOfType<FocusableUIElement>();
            foreach (var item in focusableUIList)
            {
                item.LoseFocus(false);
            }
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements
{
    public class Cursor : MonoBehaviour
    {
        [SerializeField] private Sprite defaultCursor;
        [SerializeField] private Sprite canSelectCursor;
        [SerializeField] private Sprite resizeVerticalCursor;
        [SerializeField] private Sprite resizeHorizontalCursor;
        [SerializeField] private Sprite resizeCursor;

        private Image cursorImage;
        private void Awake()
        {
            UnityEngine.Cursor.visible = false;
            //UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            cursorImage = GetComponent<Image>();
            cursorImage.sprite = defaultCursor;
        }

        private void Update()
        {
            UnityEngine.Cursor.visible = false;
            
            var mousePosition = Input.mousePosition;
            mousePosition.z = 10;
            var worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            transform.position = worldPosition;
            
            CheckHoveredObject();
        }

        private void CheckHoveredObject()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                var objectUnderMouse = hit.collider.gameObject;
                Debug.Log("Wykryto obiekt: " + objectUnderMouse.name);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Helpers;

namespace UIElements
{
    public class RetroCursor : MonoBehaviour
    {
        [SerializeField] private Sprite defaultCursor;
        
        [SerializeField] public List<SerializableKeyValPair<ECursorType, Sprite>> cursors 
            = new List<SerializableKeyValPair<ECursorType, Sprite>>();
        
        public static RetroCursor Instance { get; private set; }

        private Image cursorImage;
        private void Awake()
        {
            if (Instance != this && Instance != null)
                Destroy(this);
            else Instance = this;
            
            Cursor.visible = false;
            //UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            cursorImage = GetComponent<Image>();
            cursorImage.sprite = defaultCursor;
        }

        public void SetCursorSprite(ECursorType cursorType)
        {
            var cursorSprite = 
                cursors.FirstOrDefault(c => c.key == cursorType);

            cursorImage.sprite = cursorSprite == default ? defaultCursor : cursorSprite.value;
        }

        public void SetCursorDefault()
        {
            cursorImage.sprite = defaultCursor;
        }

        private void Update()
        {
            Cursor.visible = false;
            
            var mousePosition = Input.mousePosition;
            mousePosition.z = 10;
            var worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            transform.position = worldPosition;
        }
    }
}
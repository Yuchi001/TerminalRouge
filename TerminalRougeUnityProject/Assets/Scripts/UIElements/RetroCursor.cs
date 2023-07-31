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
        [SerializeField] private Texture2D defaultCursor;
        
        [SerializeField] public List<SerializableKeyValPair<ECursorType, Texture2D>> cursors 
            = new List<SerializableKeyValPair<ECursorType, Texture2D>>();
        
        public static RetroCursor Instance { get; private set; }
        private void Awake()
        {
            if (Instance != this && Instance != null)
                Destroy(this);
            else Instance = this;

            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
        }

        public void SetCursorSprite(ECursorType cursorType)
        {
            var cursorSprite = 
                cursors.FirstOrDefault(c => c.key == cursorType);

            Cursor.SetCursor(cursorSprite == default ? defaultCursor : cursorSprite.value, Vector2.zero, CursorMode.ForceSoftware);
        }

        public void SetCursorDefault()
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
        }
    }
}
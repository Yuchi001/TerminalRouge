using System.Collections.Generic;
using System.Linq;
using ScriptableObject.Apps;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements
{
    [RequireComponent(typeof(Button))]
    public class MenuItemPrefab : FocusableUIElement
    {
        [SerializeField] private TextMeshProUGUI programName;
        [SerializeField] private Image programImage;
        [SerializeField] private Image arrowImage;
        [SerializeField] private Color activeColor;
        [SerializeField] private Color normalColor;

        private Button button;
        private Image componentImage;
        private bool isFolder;
        private SOApp app;

        private bool isClicked = false;

        private new int ID { get; set; }
        private int ContextID { get; set; }

        public override void LoseFocus(bool recursive)
        {
            if(recursive) base.LoseFocus(true);

            isClicked = false;
            OnMouseExit();
        }

        private void Awake()
        {
            button = GetComponent<Button>();
            componentImage = GetComponent<Image>();
            button.onClick.AddListener(OnClick);
        }

        private void OnMouseEnter()
        {
            componentImage.color = activeColor;
            programName.color = Color.white;
            arrowImage.color = Color.white;
            
            var menuPrefabs = FindObjectsOfType<MenuItemPrefab>()
                .Where(m => m.ContextID == ContextID && m.ID != ID).ToList();
            foreach (var menuPrefab in menuPrefabs)
            {
                menuPrefab.UnFocusFolder();
            }

            if (isFolder) OnClick();
        }

        private void OnMouseExit()
        {
            var isFolderClicked = isClicked && isFolder;
            if (!isFolderClicked)
            {
                componentImage.color = normalColor;
                programName.color = Color.black;
                arrowImage.color = Color.black;
                return;
            }

            var menuPrefabs = FindObjectsOfType<MenuItemPrefab>()
                .Where(m => m.ContextID == ContextID && m.ID != ID).ToList();
            var isAnythingElseClicked = false;
            foreach (var menuPrefab in menuPrefabs)
            {
                if (!menuPrefab.isClicked) continue;
                
                isAnythingElseClicked = true;
                break;
            }

            if (!isAnythingElseClicked) return;
            
            var contextMenus = FindObjectsOfType<ContextMenu>();
            foreach (var contextMenu in contextMenus)
            {
                contextMenu.LoseFocus(false);
            }
            
            UnFocusFolder();
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            if (transform.parent == null)
            {
                Debug.LogError("Menu item should have parent but its missing!");
                return;
            }
            
            var menuPrefabs = FindObjectsOfType<MenuItemPrefab>()
                .Where(m => m.ContextID == ContextID && m.ID != ID).ToList();
            foreach (var menuPrefab in menuPrefabs)
            {
                menuPrefab.UnFocusFolder();
            }

            if (isFolder && isClicked) return;
            
            var appPrefab = Instantiate(app.appPrefab, 
                Vector3.zero, Quaternion.identity, isFolder ? transform : null)
                .GetComponent<AppPrefab>();
            appPrefab.Setup(app);

            if (!transform.parent.TryGetComponent<FocusableUIElement>(out var ui))
                return;
                
            if(!isFolder) ui.LoseFocus(true);

            isClicked = isFolder;
        }

        public void UnFocusFolder()
        {
            isClicked = false;
            SetColorNormal();
            foreach (Transform child in transform)
            {
                if (!child.TryGetComponent<ContextMenu>(out var context))
                    continue;

                context.LoseFocus(false);
            }
        }

        public void SetColorNormal()
        {
            componentImage.color = normalColor;
            programName.color = Color.black;
            arrowImage.color = Color.black;
        }

        public void Initiate(SOApp app, int ID, int ContextID)
        {
            this.ID = ID;
            this.app = app;
            this.ContextID = ContextID;
            isFolder = app.isFolder;
            programImage.sprite = app.appSprite;
            programName.text = app.appName;
            arrowImage.gameObject.SetActive(app.isFolder);
        }
    }
}
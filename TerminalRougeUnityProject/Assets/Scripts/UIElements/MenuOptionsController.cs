using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements
{
    public class MenuOptionsController : FocusableUIElement
    {
        [SerializeField] private RectTransform menuParent;
        [SerializeField] private List<SOApp> menuItemPresets;
        [SerializeField] private GameObject menuItemPrefab;

        private List<MenuItemPrefab> menuItems;
        private bool menuOpened = false;

        public override void LoseFocus(bool recursive)
        {
            menuOpened = false;
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            for (var i = 0; i < menuItemPresets.Count; i++)
            {
                var currentPreset = menuItemPresets[i];
                var item = Instantiate(menuItemPrefab, 
                    menuParent.position, 
                    Quaternion.identity, 
                    menuParent);
                var itemPrefabScript = item.GetComponent<MenuItemPrefab>();
                itemPrefabScript.Initiate(currentPreset, i, GetInstanceID());
            }
            
            SetContentHeight();
            gameObject.SetActive(menuOpened);
        }
        
        public void SetContentHeight()
        {
            var layoutGroup = GetComponent<VerticalLayoutGroup>();
            var spacing = layoutGroup.spacing;
            var paddingTop = layoutGroup.padding.top;
            var paddingBottom = layoutGroup.padding.bottom;
            var height = paddingTop + (from Transform child in 
                menuParent select child.GetComponent<RectTransform>().rect.height 
                into childHeight select childHeight + spacing).Sum();
            height += paddingBottom;

            menuParent.sizeDelta = new Vector2(menuParent.sizeDelta.x, height);
        }

        public void ToggleMenu()
        {
            menuOpened = !menuOpened;
            gameObject.SetActive(menuOpened);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menuOpened = false;
                gameObject.SetActive(menuOpened);
            }
        }
    }
}
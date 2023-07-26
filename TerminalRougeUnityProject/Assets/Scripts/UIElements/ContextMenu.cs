using System.Collections.Generic;
using System.Linq;
using ScriptableObject.Apps;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements
{
    [RequireComponent(typeof(RectTransform))]
    public class ContextMenu : AppPrefab
    {
        [SerializeField] private RectTransform menuParent;
        [SerializeField] private GameObject menuItemPrefab;
        public override void Setup(SOApp app)
        {
            base.Setup(app);

            if (app is not SOFolder folder)
            {
                Debug.LogError("Folder was not of 'folder' type.");
                return;
            }

            SetupApps(folder.apps);
        }

        private void SetupApps(List<SOApp> apps)
        {
            for (var i = 0; i < apps.Count; i++)
            {
                var currentPreset = apps[i];
                var item = Instantiate(menuItemPrefab, 
                    menuParent.position, 
                    Quaternion.identity, 
                    menuParent);
                var itemPrefabScript = item.GetComponent<MenuItemPrefab>();
                itemPrefabScript.Initiate(currentPreset, i, ID);
            }
            
            SetContextPositionAndHeight();
        }

        private void SetContextPositionAndHeight()
        {
            menuParent.anchorMax = new Vector2(0.5f, 1);
            menuParent.anchorMin = new Vector2(0.5f, 1);
            
            var layoutGroup = GetComponent<VerticalLayoutGroup>();
            var spacing = layoutGroup.spacing;
            var paddingTop = layoutGroup.padding.top;
            var paddingBottom = layoutGroup.padding.bottom;
            var height = paddingTop + (from Transform child in 
                    menuParent select child.GetComponent<RectTransform>().rect.height 
                into childHeight select childHeight + spacing).Sum();
            height += paddingBottom;

            menuParent.sizeDelta = new Vector2(menuParent.sizeDelta.x, height);

            var parentRect = transform.parent.GetComponent<RectTransform>();
            
            menuParent.localPosition = Vector3.zero;
            menuParent.anchoredPosition += 
                new Vector2(menuParent.sizeDelta.x, 
                    parentRect.rect.height / (menuParent.childCount + 1) + spacing * (menuParent.childCount + 1));
        }
    }
}
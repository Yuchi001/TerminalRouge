using System.Collections.Generic;
using System.Linq;
using ScriptableObject.Apps;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements
{
    [RequireComponent(typeof(RectTransform))]
    public class ContextMenu : AppPrefab
    {
        [SerializeField] private RectTransform menuParent;
        [SerializeField] private GameObject menuItemPrefab;
        [SerializeField] private bool anchorTop = true;

        private RectTransform rectTransform;
        private int ID = 0;
        public override void Setup(SOApp app)
        {
            base.Setup(app);

            ID = FindObjectsOfType<ContextMenu>().Length;
            
            if (app is not SOFolder folder)
            {
                Debug.LogError("Folder was not of 'folder' type.");
                return;
            }

            SetupApps(folder.apps);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.localPosition = Vector3.zero;
            rectTransform.anchoredPosition += 
                new Vector2(rectTransform.sizeDelta.x, -rectTransform.sizeDelta.y / 2);
        }

        private void SetupApps(List<SOApp> apps)
        {
            rectTransform = GetComponent<RectTransform>();
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
            
            SetContextHeight();
        }

        private void SetContextHeight()
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
    }
}
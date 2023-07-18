using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements
{
    [RequireComponent(typeof(Button))]
    public class MenuItemPrefab : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI programName;
        [SerializeField] private Image programImage;
        [SerializeField] private Image arrowImage;
        [SerializeField] private Color activeColor;
        [SerializeField] private Color normalColor;

        private Button button;
        private Image componentImage;
        private bool isFolder;
        private List<MenuItemPrefab> apps;

        public int ID { get; private set; }

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
            
            if(isFolder) OnClick();
        }

        private void OnMouseExit()
        {
            componentImage.color = normalColor;
            programName.color = Color.black;
            arrowImage.color = Color.black;
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            
        }

        public void SetColorNormal()
        {
            componentImage.color = normalColor;
        }

        public void Initiate(SOApp app, List<MenuItemPrefab> apps, int ID)
        {
            this.apps = apps;
            this.ID = ID;
            isFolder = app.isFolder;
            programImage.sprite = app.appSprite;
            programName.text = app.appName;
        }
    }
}
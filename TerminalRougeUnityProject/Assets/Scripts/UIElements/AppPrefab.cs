using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements
{
    public abstract class AppPrefab : FocusableUIElement
    {
        [SerializeField] private bool useButtons = true;
        [SerializeField] private bool isApp = true;
        
        [SerializeField] private Button closeButton;
        [SerializeField] private Button maximizeButton;
        [SerializeField] private Button minimizeButton;
        
        protected SOApp app;

        public override void LoseFocus(bool recursive)
        {
            if (isApp) return;
            
            base.LoseFocus(recursive);
        }

        public void Focus()
        {
            
        }

        public virtual void Setup(SOApp app)
        {
            this.app = app;
            
            gameObject.SetActive(true);
            
            if(!useButtons) return;
            
            closeButton.onClick.AddListener(CloseApp);
            minimizeButton.onClick.AddListener(MinimizeApp);    
        }

        private void OnDisable()
        {
            if(!useButtons) return;
            
            closeButton.onClick.RemoveListener(CloseApp);
            minimizeButton.onClick.RemoveListener(MinimizeApp);
        }

        private void MinimizeApp()
        {
            AppController.Instance.MinimizeApp(GetInstanceID());
        }

        private void CloseApp()
        {
            AppController.Instance.CloseApp(GetInstanceID());
        }

        private void MaximizeApp()
        {
            
        }
    }
}
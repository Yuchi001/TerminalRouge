using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements
{
    public class AppController : MonoBehaviour
    {
        public static AppController Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        [SerializeField] private GameObject buttonParent;
        [SerializeField] private GameObject buttonPrefab;
        private List<App> activeApps = new List<App>();
        
        public void OpenApp(SOApp appData, GameObject appObject)
        {
            var app = new App(appData, appObject, this);
            activeApps.Add(app);
            
            app.animator.Play("OpenApp");
        }

        public void ReOpenApp(string appName)
        {
            var app = AppOpened(appName);
            if (app is null) return;
            
            app.appObject.SetActive(true);
            app.animator.Play("OpenMap");
        }

        public void MinimizeApp(string appName)
        {
            var app = AppOpened(appName);
            if (app is null) return;
            
            app.animator.Play("MinimizeApp");
            StartCoroutine(ActionDelay(() => app.appObject.SetActive(false), 0.3f));
        }

        public void CloseApp(string appName)
        {
            var app = AppOpened(appName);
            if (app is null) return;
            
            app.animator.Play("CloseApp");
            StartCoroutine(ActionDelay(() =>
            {
                activeApps.Remove(app);
                app.DestroyApp();
            }, 0.3f));
        }

        IEnumerator ActionDelay(Action action, float time)
        {
            yield return new WaitForSeconds(time);
            
            action.Invoke();
        }

        [CanBeNull]
        private App AppOpened(string appName)
        {
            var app = activeApps.FirstOrDefault(app => app.appData.appName == appName);
            if (app != default) return app;
            
            Debug.LogError($"Cant close app {appName} - couldnt find app name.");
            return null;

        }
        
        private class App
        {
            public App(SOApp appData, GameObject appObject, AppController appController)
            {
                this.appController = appController;
                this.appData = appData;
                this.appObject = appObject;
                animator = appObject.GetComponent<Animator>();
                
                InstantiateButton();
                isOpened = true;
            }

            private void InstantiateButton()
            {
                buttonPrefabInstance = Instantiate(appController.buttonPrefab,
                    appController.buttonParent.transform.position,
                    Quaternion.identity, appController.buttonParent.transform);

                var appImage = buttonPrefabInstance.transform.GetChild(0);
                appImage.GetComponent<Image>().sprite = appData.appSprite;

                var appText = buttonPrefabInstance.transform.GetChild(1);
                appText.GetComponent<TextMeshProUGUI>().text = appData.appName;

                buttonComponent = buttonPrefabInstance.GetComponent<Button>();
                buttonComponent.onClick.AddListener(OnClick);
            }

            public void DestroyApp()
            {
                buttonComponent.onClick.RemoveListener(OnClick);
                Destroy(appObject);
                Destroy(buttonPrefabInstance);
            }

            private void OnClick()
            {
                if (isOpened)
                {
                    appController.CloseApp(appData.appName);
                    isOpened = false;
                    return;
                }
                
                appController.ReOpenApp(appData.appName);
                isOpened = true;
            }

            private AppController appController;

            public SOApp appData;
            public GameObject appObject;
            public Animator animator;

            private GameObject buttonPrefabInstance;
            private Button buttonComponent;
            private bool isOpened = false;
        }
    }
}
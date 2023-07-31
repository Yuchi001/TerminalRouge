using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
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

        [SerializeField] private RectTransform appParent;
        [SerializeField] private GameObject buttonParent;
        [SerializeField] private GameObject buttonPrefab;
        private List<App> activeApps = new List<App>();
        
        public void OpenApp(SOApp appData, Transform parent = null)
        {
            var spawnedApp = Instantiate(appData.appPrefab, 
                    Vector3.zero, Quaternion.identity, parent == null ? appParent : parent)
                .GetComponent<AppPrefab>();
            spawnedApp.Setup(appData);

            if (appData.isFolder) return;
            
            var app = new App(appData, spawnedApp.GetInstanceID(), spawnedApp.gameObject, this);
            activeApps.Add(app);
            //app.animator.Play("OpenApp");
            
            SetWidth();
        }

        public void ReOpenApp(int appId)
        {
            var app = AppOpened(appId);
            if (app is null) return;
            
            app.appObject.SetActive(true);
            app.appPrefabScript.Focus();
            
            app.appObject.transform.SetAsLastSibling();
            //app.animator.Play("OpenMap");
        }

        public void MinimizeApp(int appId)
        {
            var app = AppOpened(appId);
            if (app is null) return;
            
            //app.animator.Play("MinimizeApp");
            StartCoroutine(ActionDelay(() =>
            {
                app.appObject.SetActive(false);
                app.appObject.transform.SetAsFirstSibling();
            }, 0.1f));
        }

        public void CloseApp(int appId)
        {
            var app = AppOpened(appId);
            if (app is null) return;
            
            //app.animator.Play("CloseApp");
            StartCoroutine(ActionDelay(() =>
            {
                activeApps.Remove(app);
                app.DestroyApp();
            }, 0.1f, SetWidth));
        }

        private void SetWidth()
        {
            var rectTransform = GetComponent<RectTransform>();
            
            var layoutGroup = GetComponent<HorizontalLayoutGroup>();
            var spacing = layoutGroup.spacing;
            var paddingLeft = layoutGroup.padding.left;
            var paddingRight = layoutGroup.padding.right;
            var width = paddingLeft + (from Transform child in 
                    rectTransform select child.GetComponent<RectTransform>().rect.width 
                into childWidth select childWidth + spacing).Sum() + paddingRight;

            rectTransform.sizeDelta = new Vector2(width / 2, rectTransform.rect.height);
        }

        IEnumerator ActionDelay(Action action, float time, Action afterAction = null)
        {
            yield return new WaitForSeconds(time);
            
            action.Invoke();

            yield return new WaitForNextFrameUnit();
            
            afterAction?.Invoke();
        }

        [CanBeNull]
        private App AppOpened(int appId)
        {
            var app = activeApps.FirstOrDefault(app => app.UniqueID == appId);
            if (app != default) return app;
            
            Debug.LogError($"Cant close app {appId} - app not found.");
            return null;

        }
        
        private class App
        {
            private AppController appController;

            public SOApp appData;
            public GameObject appObject;
            public Animator animator;

            public AppPrefab appPrefabScript;

            private GameObject buttonPrefabInstance;
            private Button buttonComponent;
            private bool isOpened = false;
            public int UniqueID { get; }
            
            public App(SOApp appData, int uniqueID, GameObject appObject, AppController appController)
            {
                this.appController = appController;
                this.appData = appData;
                this.appObject = appObject;
                UniqueID = uniqueID;
                animator = appObject.GetComponent<Animator>();
                appPrefabScript = appObject.GetComponent<AppPrefab>();

                var appParent = appObject.transform.parent;
                var apps = appParent.GetComponentsInChildren<AppPrefab>().ToList(); 
                if(apps.Count >= 2)
                    appObject.transform.position =
                        apps[^2].transform.position + new Vector3(0.3f, -0.3f);
                
                appObject.transform.SetAsLastSibling();
                
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

                /*var appText = buttonPrefabInstance.transform.GetChild(1);
                appText.GetComponent<TextMeshProUGUI>().text = appData.appName;*/

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
                    isOpened = false;
                    appController.MinimizeApp(UniqueID);
                    return;
                }
                
                isOpened = true;
                appController.ReOpenApp(UniqueID);
            }
        }
    }
}
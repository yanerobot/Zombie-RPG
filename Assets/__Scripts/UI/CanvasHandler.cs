using UnityEngine;
using System.Collections.Generic;

namespace KK
{
    public class CanvasHandler : MonoBehaviour
    {
        public List<GameObject> UIElements;
        #if UNITY_EDITOR
        void OnValidate()
        {
            StoreUIElements();
        }
        #endif
        void Awake()
        {
            StoreUIElements();
        }
        void StoreUIElements()
        {
            UIElements = new List<GameObject>();
            foreach (Transform child in transform)
            {
                UIElements.Add(child.gameObject);
            }
        }
        public GameObject GetReference(string name)
        {
            var obj = transform.Find(name)?.gameObject;

            return obj;
        }

        public bool IsFolderActive(string name)
        {
            var folder = transform.Find(name);
            if (folder == null)
                return false;

            return folder.gameObject.activeSelf;
        }

        public void Show(string name)
        {
            transform.Find(name)?.gameObject.SetActive(true);
        }
        public void HideAll()
        {
            foreach (var UIBlock in UIElements)
            {
                UIBlock.SetActive(false);
            }
        }

        public void Hide(string name)
        {
            transform.Find(name)?.gameObject.SetActive(false);
        }

        public void ToggleScreen(string name)
        {
            var obj = transform.Find(name)?.gameObject;
            obj.SetActive(!obj.activeSelf);
        }

        public void ToggleLoadingScreen(bool flag)
        {
            if (flag) Show("LoadingScreen");
            else Hide("LoadingScreen");
        }
        public void ToggleGameOverScreen(bool flag)
        {
            if (flag) Show("GameOverScreen");
            else Hide("GameOverScreen");
        }
        public void ToggleLevelCompletedScreen(bool flag)
        {
            if (flag) Show("LevelCompletedScreen");
            else Hide("LevelCompletedScreen");
        }
        public void ShowGameplayUI()
        {
            HideAll();
            Show("IngameUI");
        }
    } 
}

using System;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

namespace JayoCanvasPlugin.Util
{
    public class TabSection : MonoBehaviour
    {
        public Button[] TabButtons = [];
        public RectTransform[] TabAreas = [];
        public int DefaultTab = 0;
        public int ActiveIndex { get { return lastActiveIndex; } }

        private int lastActiveIndex = -1;

        void Awake()
        {
            lastActiveIndex = DefaultTab;
        }

        void Start()
        {
            if (TabButtons.Length == 0) return;
            int i = 0;

            foreach (Button button in TabButtons)
            {
                int index = i;
                button.onClick.AddListener(() => { ActivateTab(index); });
                i++;
            }
            ActivateTab(DefaultTab);
        }

        public void Restart()
        {
            Start();
        }

        public void ActivateTab(int index)
        {
            int i = 0;
            foreach (Button button in TabButtons)
            {
                if (index == i)
                {
                    button.interactable = false;
                    button.transform.SetAsLastSibling();
                    TabAreas[i].gameObject.SetActive(true);
                    lastActiveIndex = i;
                }
                else
                {
                    button.interactable = true;
                    TabAreas[i].gameObject.SetActive(false);
                }
                i++;
            }
        }
    }
}

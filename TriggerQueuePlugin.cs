using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UnityEditor;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;
using VNyanInterface;
using Archipelago.MultiClient.Net;
using Logger = JayoCanvasPlugin.TriggerQueuePluginLogger;
using Archipelago.MultiClient.Net.Enums;
using UnityEngine.PlayerLoop;

namespace JayoCanvasPlugin
{
    public class TriggerQueueItem
    {
        public string triggerName;
        public int v1;
        public int v2;
        public int v3;
        public string t1;
        public string t2;
        public string t3;
        public float time;
    }

    public class TriggerQueuePlugin : MonoBehaviour, IButtonClickedHandler
    {
        public static TriggerQueuePlugin Instance { get { return _instance; } }
        private static TriggerQueuePlugin _instance;
        private static Dictionary<string, Queue<TriggerQueueItem>> triggerQueue = [];
        private static Dictionary<string, Coroutine> queueCoroutines = [];
        private static List<string> waitingQueues = [];

        private Util.PluginUpdater updater;
        public GameObject windowPrefab;
        private GameObject window;

        private string currentVersion = "v0.1.0";
        private string repoName = "jayo-exe/JayoTriggerQueuePlugin";
        private string updateLink = "https://jayo-exe.itch.io/trigger-queue-for-vnyan";

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            updater = new Util.PluginUpdater(repoName, currentVersion, updateLink);
            updater.OpenUrlRequested += (url) => Util.MainThreadDispatcher.Enqueue(() => { Application.OpenURL(url); });

            updater.PrepareUpdateUI(
                window.transform.Find("Panel/UpdateRow/VersionText").gameObject,
                window.transform.Find("Panel/UpdateRow/UpdateText").gameObject,
                window.transform.Find("Panel/UpdateRow/UpdateButton").gameObject
            );

            VNyanInterface.VNyanInterface.VNyanTrigger.registerTriggerListener(TriggerQueueTriggerHandler.Instance);
            VNyanInterface.VNyanInterface.VNyanUI.registerPluginButton("Jayo's Trigger Queue", this);
            window = (GameObject)VNyanInterface.VNyanInterface.VNyanUI.instantiateUIPrefab(windowPrefab);

            if (window != null)
            {
                window.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                window.SetActive(false);
                window.transform.Find("Panel/TitleBar/CloseButton").GetComponent<Button>().onClick.AddListener(() => { window.SetActive(false); });
            }
        }

        void Update()
        {
            foreach(var channel in triggerQueue)
            {
                if (waitingQueues.Contains(channel.Key)) continue;
                if (channel.Value.Count == 0) continue;

                lock(channel.Value)
                {
                    var item = channel.Value.Dequeue();
                    VNyanInterface.VNyanInterface.VNyanTrigger.callTrigger(item.triggerName, item.v1, item.v2, item.v3, item.t1, item.t2, item.t3);
                    queueCoroutines[channel.Key] = StartCoroutine(HoldChannelForSeconds(channel.Key, item.time));
                }
            }
        }

        public void pluginButtonClicked()
        {
            window.SetActive(!window.activeSelf);
            if (window.activeSelf) window.transform.SetAsLastSibling();
        }

        IEnumerator HoldChannelForSeconds(string queueName, float seconds)
        {
            waitingQueues.Add(queueName);
            yield return new WaitForSeconds(seconds);
            queueCoroutines.Remove(queueName);
            waitingQueues.Remove(queueName);
        }

        public static void EnqueueTrigger(string queueName, float holdTime, string triggerName, int v1, int v2, int v3, string t1, string t2, string t3)
        {
            if (!triggerQueue.ContainsKey(queueName)) triggerQueue[queueName] = new();
            var item = new TriggerQueueItem { triggerName = triggerName, v1 = v1, v2 = v2, v3 = v3, t1 = t1, t2 = t2, t3 = t3, time = holdTime };
            triggerQueue[queueName].Enqueue(item);
        }

        public static void NextInQueue(string queueName)
        {
            if (!waitingQueues.Contains(queueName)) return;
            _instance.StopCoroutine(queueCoroutines[queueName]);
            queueCoroutines.Remove(queueName);
            waitingQueues.Remove(queueName);
        }

        public static void ClearQueue(string queueName)
        {
            if (triggerQueue.ContainsKey(queueName)) { triggerQueue[queueName].Clear(); }
        }

        public static void ClearAllQueues()
        {
            foreach (var channel in triggerQueue) channel.Value.Clear();
        }
    }
}

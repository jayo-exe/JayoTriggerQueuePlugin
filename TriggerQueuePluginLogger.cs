using UnityEngine;

namespace JayoCanvasPlugin
{
    static class TriggerQueuePluginLogger
    {
        private static readonly string identifier = "TriggerQueuePlugin";
        public static void LogInfo(object message) => Debug.Log($"[{identifier}] {message}");
        public static void LogWarning(object message) => Debug.LogWarning($"[{identifier}] {message}");
        public static void LogError(object message) => Debug.LogError($"[{identifier}] {message}");
    }
}

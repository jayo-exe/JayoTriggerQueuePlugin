using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace JayoCanvasPlugin;

public delegate void TriggerAction(int v1, int v2, int v3, string t1, string t2, string t3);

public class TriggerQueueTriggerHandler : VNyanInterface.ITriggerHandler
{
    private static readonly TriggerQueueTriggerHandler _instance = new();
    public static TriggerQueueTriggerHandler Instance { get => _instance; }

    private static string prefix = "_jq";

    private static Dictionary<string, TriggerAction> actionHandlers = new()
    {
        ["_null"] = (v1, v2, v3, t1, t2, t3) => { },
        ["_next"] = handleNext,
        ["_clear"] = handleClear,
        ["_clear_all"] = handleClearAll,
    };

    private static string parseStringArgument(string arg)
    {
        if (arg.StartsWith("<") && arg.EndsWith(">"))
        {
            return VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterString(arg.Substring(1, arg.Length - 2));
        }
        return arg;
    }

    private static float parseFloatArgument(string arg)
    {
        if (arg.StartsWith("[") && arg.EndsWith("]"))
        {
            return VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(arg.Substring(1, arg.Length - 2));
        }

        if (arg.StartsWith("<") && arg.EndsWith(">"))
        {
            arg = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterString(arg.Substring(1, arg.Length - 2));
        }

        float returnVal = 0f;
        float.TryParse(arg, NumberStyles.Any, CultureInfo.InvariantCulture, out returnVal);
        return returnVal;
    }

    // general trigger router, sends relevant incoming triggers to matching handers
    public void triggerCalled(string triggerName, int value1, int value2, int value3, string text1, string text2, string text3)
    {
        if (!triggerName.StartsWith(prefix)) return;

        if(triggerName.StartsWith("_jq:")) {
            string triggerString = triggerName.Substring(4);
            string[] triggerParts = triggerString.Split(new string[] {";;"}, 3, StringSplitOptions.None);
            if (triggerParts.Length < 3) return;
            string channel = triggerParts[0];
            float time = float.Parse(triggerParts[1]);
            string targetTriggerName = triggerParts[2];

            TriggerQueuePlugin.EnqueueTrigger(channel, time, targetTriggerName, value1, value2, value3, text1, text2, text3);
            return;
        }

        string triggerAction = triggerName.Substring(prefix.Length);
        if (actionHandlers.ContainsKey(triggerAction)) actionHandlers[triggerAction](value1, value2, value3, text1, text2, text3);
    }

    private static void handleNext(int v1, int v2, int v3, string t1, string t2, string t3)
    {
        string itemName = parseStringArgument(t1);
        TriggerQueuePlugin.NextInQueue(itemName);
    }

    private static void handleClear(int v1, int v2, int v3, string t1, string t2, string t3)
    {
        string itemName = parseStringArgument(t1);
        TriggerQueuePlugin.ClearQueue(itemName);
    }

    private static void handleClearAll(int v1, int v2, int v3, string t1, string t2, string t3)
    {
        TriggerQueuePlugin.ClearAllQueues();
    }
}


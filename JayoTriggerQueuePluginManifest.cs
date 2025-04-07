using System.IO;
using System.Reflection;
using UnityEngine;
using VNyanInterface;

namespace JayoTriggerQueuePlugin;

public class JayoTriggerQueuePluginManifest : IVNyanPluginManifest
{
    public string PluginName { get; } = "JayoTriggerQueuePlugin";
    public string Version { get; } = "v0.1.0";
    public string Title { get; } = "Jayo's Trigger Queue Plugin";
    public string Author { get; } = "Jayo";

    public string Website { get; } = "https://vtuber.works/trigger-queue-plugin";

    private string bundleName { get; } = "JayoTriggerQueuePlugin.vnobj";

    public void InitializePlugin()
    {
        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(this.GetType(), bundleName))
        {
            byte[] bundleData = new byte[stream.Length];
            stream.Read(bundleData, 0, bundleData.Length);
            AssetBundle bundle = AssetBundle.LoadFromMemory(bundleData);
            GameObject.Instantiate(bundle.LoadAsset<GameObject>(bundle.GetAllAssetNames()[0]));
        }
    }
}
#nullable enable
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using BetterPingDistance.Patches;

namespace BetterPingDistance;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static Plugin instance { get; private set; } = null!;
    internal static ManualLogSource Log => instance._logger;
    private ManualLogSource _logger => base.Logger;
    private readonly Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

    internal static PingDistanceManager PDM { get; private set; } = null!;


    private void Awake()
    {
        if (instance == null) instance = this;

        // Prevent the plugin from being deleted
        gameObject.transform.parent = null;
        gameObject.hideFlags = HideFlags.HideAndDontSave;

        PDM = new PingDistanceManager();

        harmony.PatchAll(typeof(GUIManagerPatch));
        harmony.PatchAll(typeof(PointPingerPatch));

        Log.LogInfo($"{MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} by {MyPluginInfo.PLUGIN_AUTHOR} initialized!");

    }
    private void OnDestroy()
    {
        harmony.UnpatchSelf();
        PDM.OnDestroy();
    }
}

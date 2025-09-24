
using HarmonyLib;
using static BetterPingDistance.Plugin;

namespace BetterPingDistance.Patches;

[HarmonyPatch(typeof(GUIManager))]
static class GUIManagerPatch
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    private static void Start_Postfix(GUIManager __instance)
    {
        PDM.Initialize();
    }

    [HarmonyPatch("LateUpdate")]
    [HarmonyPostfix]
    private static void LateUpdate_Postfix(GUIManager __instance)
    {
        if (!PDM.isInitialized)
            PDM.Initialize();
    }

    [HarmonyPatch("OnDestroy")]
    [HarmonyPostfix]
    private static void OnDestroy_Postfix(GUIManager __instance)
    {
        PDM.OnDestroy();
    }

}

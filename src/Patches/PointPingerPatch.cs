using HarmonyLib;
using UnityEngine;
using static BetterPingDistance.Plugin;

namespace BetterPingDistance.Patches;

[HarmonyPatch(typeof(PointPinger))]
static class PointPingerPatch
{
    [HarmonyPatch("ReceivePoint_Rpc")]
    [HarmonyPrefix]
    private static void ReceivePoint_Rpc_Prefix(PointPinger __instance, ref Vector3 point, ref Vector3 hitNormal)
    {
        PDM.CreateInstance(__instance.character, point + hitNormal);
    }
}

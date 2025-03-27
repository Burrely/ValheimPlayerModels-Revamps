using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace ValheimPlayerModels {
    [HarmonyPatch(typeof (PlayerController), "TakeInput")]
    [HarmonyPriority(0)]
    public static class PlayerController_TakeInput_PreventInput
    {
        public static void Postfix(ref bool __result)
        {
            __result = __result && !Plugin.DisplayingWindow;
        }
    }
}
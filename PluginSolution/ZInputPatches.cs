#if PLUGIN
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
            __result = __result && !Plugin.ShouldBlockUserInput;
        }
    }
    
    [HarmonyPatch(typeof (Player), "SetControls")]
    [HarmonyPriority(0)]
    public static class Player_SetControls_PreventAttackControls
    {
        public static void Prefix(
            ref bool attack,
            ref bool attackHold,
            ref bool secondaryAttack,
            ref bool secondaryAttackHold,
            ref bool block,
            ref bool blockHold
            ) {
            if (!Plugin.ShouldBlockAttackControls) return;
            attack = false;
            attackHold = false;
            secondaryAttack = false;
            secondaryAttackHold = false;
            block = false;
            blockHold = false;
        }
    }
}
#endif
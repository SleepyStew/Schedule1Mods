using HarmonyLib;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(EasyRemove.Core), "EasyRemove", "1.0.0", "SleepyStew", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace EasyRemove
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }

        [HarmonyPatch(typeof(Il2CppScheduleOne.EntityFramework.BuildableItem), "CanBePickedUp")]
        private static class Patch_CanBePickedUp
        {
            private static bool Prefix(ref bool __result)
            {
                if (Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift))
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }
    }
}
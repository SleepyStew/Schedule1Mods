using MelonLoader;
using Il2CppScheduleOne.ItemFramework;
using UnityEngine;
using Il2CppScheduleOne.ObjectScripts;
using HarmonyLib;

[assembly: MelonInfo(typeof(BigStackz.Core), "BigStackz", "1.0.2", "SleepyStew", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace BigStackz
{
    public class Core : MelonMod
    {

        public static int SetStackLimit = 60;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
            var configcategory = MelonPreferences.CreateCategory("BigStackz");
            configcategory.SetFilePath("UserData/BigStackz.conf");
            var stacklimit = configcategory.CreateEntry<int>("StackLimit", 60, "Set the stack limit for items in the game.");
            SetStackLimit = stacklimit.Value;
            configcategory.SaveToFile();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            foreach (var item in Resources.FindObjectsOfTypeAll<ItemDefinition>())
            {
                item.StackLimit = SetStackLimit;
            }

            foreach (var dryingrack in Resources.FindObjectsOfTypeAll<DryingRack>())
            {
                dryingrack.ItemCapacity = SetStackLimit;
            }

            foreach (var dryingrack in Resources.FindObjectsOfTypeAll<MixingStation>())
            {
                dryingrack.MaxMixQuantity = SetStackLimit;
            }

            // AccessViolation...
            //Traverse.Create<LabOven>().Property("SOLID_INGREDIENT_COOK_LIMIT").SetValue(SetStackLimit);
        }
    }
}
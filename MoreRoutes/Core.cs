using HarmonyLib;
using Il2CppFluffyUnderware.DevTools.Extensions;
using Il2CppScheduleOne.Employees;
using Il2CppScheduleOne.Management;
using Il2CppScheduleOne.UI.Management;
using MelonLoader;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(MoreRoutes.Core), "MoreRoutes", "1.0.0", "SleepyStew", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace MoreRoutes
{
    public class Core : MelonMod
    {
        private bool loaded = false;
        private static int MAX_ROUTES = 15;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (loaded | sceneName != "Main")
                return;

            LoggerInstance.Msg($"Setup running... {MAX_ROUTES} routes available.");
            InitializeRouteOverride();
            loaded = true;
        }

        public void InitializeRouteOverride()
        {
            var routeListFieldUI = Resources.FindObjectsOfTypeAll<RouteListFieldUI>().First(o => o.isActiveAndEnabled == false).GetComponentInChildren<RouteListFieldUI>();

            if (routeListFieldUI.gameObject.transform.FindChild("Contents")?.childCount < MAX_ROUTES + 1)
            {
                MelonLogger.Msg("Generating more components...");
                for (int i = 5; i < MAX_ROUTES; i++)
                {
                    if (!routeListFieldUI.gameObject.transform.FindChild("Contents")?.FindChild($"Entry ({i})")?.gameObject)
                    {
                        GameObject e = routeListFieldUI.gameObject.transform.FindChild("Contents")?.FindChild("Entry")?.gameObject;
                        GameObject n = e.DuplicateGameObject(e.transform.parent);
                        n.name = $"Entry ({i})";
                    }
                }
                routeListFieldUI.gameObject.transform.FindChild("Contents")?.FindChild("AddNew").transform.SetAsLastSibling();
            }

            routeListFieldUI = Resources.FindObjectsOfTypeAll<RouteListFieldUI>().First(o => o.isActiveAndEnabled == false).GetComponentInChildren<RouteListFieldUI>();

            if (routeListFieldUI != null)
            {
                var array = routeListFieldUI.RouteEntries;
                if (array != null)
                {
                    var newArray = new RouteEntryUI[MAX_ROUTES];
                    for (int i = 0; i < MAX_ROUTES; i++)
                    {
                        if (i < array.Length)
                            newArray[i] = array[i];

                        if (true)
                        {
                            var t = routeListFieldUI.gameObject.transform.FindChild("Contents")?.FindChild(i != 0 ? $"Entry ({i})" : "Entry")?.gameObject.GetComponentInChildren<RouteEntryUI>();
                            if (!t)
                            {
                                continue;
                            }

                            newArray[i] = t;
                        }
                    }
                    routeListFieldUI.RouteEntries = newArray;
                }
            }

            MakeScrollable(routeListFieldUI);

        }

        // these are disabled as it seems to have some unintended consequences on behaviour
        //[HarmonyPatch(typeof(ManagementClipboard), "Open")]
        //private static class PatchClipboardOpen
        //{
        //    private static void Prefix()
        //    {
        //        Resources.FindObjectsOfTypeAll<Il2CppScheduleOne.PlayerScripts.PlayerInventory>().First().SetEquippingEnabled(false);
        //    }
        //}

        //[HarmonyPatch(typeof(ManagementClipboard), "Close")]
        //private static class PatchClipboardClose
        //{
        //    private static void Prefix()
        //    {
        //        Resources.FindObjectsOfTypeAll<Il2CppScheduleOne.PlayerScripts.PlayerInventory>().First().SetEquippingEnabled(true);
        //    }
        //}

        [HarmonyPatch(typeof(PackagerConfiguration), "ShouldSave")]
        private static class PatchPackagerConfigurationShouldSave
        {
            private static bool Prefix(ref bool __result)
            {
                __result = true;
                return false;
            }
        }

        [HarmonyPatch(typeof(Packager), "NetworkInitialize___Early")]
        private static class PatchPackagerAwake
        {
            private static void Postfix(ref Packager __instance)
            {
                MelonCoroutines.Start(ConfigureDelayed(__instance));
            }
        }

        public static IEnumerator ConfigureDelayed(Packager __instance)
        {
            // preventing any race conditions
            yield return (object)new WaitForSeconds(1f);
            if (__instance?.configuration == null)
            {
                MelonLogger.Msg("Instance was null");
            }
            else
            {
                if (__instance.configuration.Routes.MaxRoutes != MAX_ROUTES)
                {
                    __instance.configuration.Routes.MaxRoutes = MAX_ROUTES;
                    MelonLogger.Msg($"Max routes set to {MAX_ROUTES} for a packager.");
                }
            }
        }

        private void MakeScrollable(RouteListFieldUI parent)
        {

            if (parent.transform.FindChild("ScrollArea"))
                return;

            parent.FieldText = "Routes (Drag to Scroll)";

            GameObject scrollArea = parent.AddChildGameObject("ScrollArea");

            scrollArea.AddComponent<RectTransform>();
            scrollArea.AddComponent<RectMask2D>();

            RectTransform scrollAreaTransform = scrollArea.transform.GetComponent<RectTransform>();
            ScrollRect scrollAreaScrollRect = scrollArea.AddComponent<ScrollRect>();

            scrollAreaTransform.anchoredPosition = new Vector2(0, -62.52f);
            scrollAreaTransform.sizeDelta = new Vector2(0, 170);
            scrollAreaTransform.pivot = new Vector2(0, 1);
            scrollAreaTransform.anchorMax = new Vector2(1, 1);
            scrollAreaTransform.anchorMin = new Vector2(0, 1);

            GameObject contents = parent.transform.FindChild("Contents").gameObject;

            ContentSizeFitter contentsSizeFilter = contents.AddComponent<ContentSizeFitter>();
            contentsSizeFilter.verticalFit = ContentSizeFitter.FitMode.MinSize;

            contents.transform.SetParent(scrollArea.transform);

            RectTransform contentsTransform = contents.transform.GetComponent<RectTransform>();

            contentsTransform.anchoredPosition = new Vector2(0, 0);
            contentsTransform.pivot = new Vector2(0, 1);

            scrollAreaScrollRect.content = contentsTransform;
            scrollAreaScrollRect.horizontal = false;
            scrollAreaScrollRect.scrollSensitivity = 5;
            scrollAreaScrollRect.viewport = scrollAreaTransform;
        }
    }
}
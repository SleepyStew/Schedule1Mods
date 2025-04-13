using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Tools;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(ClickTP.Core), "ClickTP", "1.0.0", "SleepyStew", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace ClickTP
{
    public class Core : MelonMod
    {
        private string currentScene = string.Empty;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }
        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            currentScene = sceneName;
            LoggerInstance.Msg($"Scene initialized: {sceneName}");
        }
        public override void OnUpdate()
        {   

            if ( currentScene != "Main" ) {
                return;
            }

            if (Input.GetMouseButtonDown(4) || Input.GetKeyDown(KeyCode.LeftAlt))
            {
                Player player = GameObject.FindObjectsOfType<Player>().First(p => p.IsLocalPlayer);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                LayerMask hitMask = LayerMask.NameToLayer("PlayerBlocker");


                if (Physics.Raycast(ray, out hitInfo, 11f, hitMask.value))
                {
                    if (Vector3.Distance(hitInfo.point, player.transform.position) < 1)
                    {
                        return;
                    }
                    player.transform.position = hitInfo.point + new Vector3(0, 1, 0);
                } else
                {
                    Vector3 cameraForward = Camera.main.transform.forward;
                    Vector3 teleportDirection = new Vector3(cameraForward.x, Math.Max(cameraForward.y, 0), cameraForward.z).normalized;
                    player.transform.position += teleportDirection * 10f;
                }
            }
        }
    }
}
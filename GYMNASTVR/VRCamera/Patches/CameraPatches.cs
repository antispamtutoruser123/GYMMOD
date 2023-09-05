using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Profiling;
using Valve.VR;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

namespace GYMNASTVR
{
    [HarmonyPatch]
    class CameraPatches
    {
        public static Vector3 startpos, startrot, offset;
        public static RenderTexture rt=null;
        public static GameObject newUI, clip, poster;
        public static GameObject worldcam=null;

        public static GameObject DummyCamera, VRCamera, VRPlayer, SecondEye, DummyCamera2;
        public static Camera SecondCam;
        public static bool fpmode=true;

        private static readonly string[] canvasesToIgnore =
    {
        "com.sinai.unityexplorer_Root", // UnityExplorer.
        "com.sinai.unityexplorer.MouseInspector_Root", // UnityExplorer.
        "com.sinai.universelib.resizeCursor_Root",
        "IntroCanvas"
    };
        private static readonly string[] canvasesToWorld =
    {
        "OverlayCanvas"
    };



            [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelCompleteMenu), "Show")]
        private static void LevelComplete(LevelCompleteMenu __instance)
        {
            VRCamera.transform.parent = DummyCamera2.transform;
            SecondEye.transform.parent = DummyCamera2.transform;
        }

            [HarmonyPostfix]
        [HarmonyPatch(typeof(CanvasScaler), "OnEnable")]
        private static void OnCameraRigEnabled(CanvasScaler __instance)
        {
            Logs.WriteInfo("LLLLL: CanvasScaler OnEnable");

            if (IsCanvasToIgnore(__instance.name)) return;

            Logs.WriteInfo($"Hiding Canvas:  {__instance.name}");
            var canvas = __instance.GetComponent<Canvas>();

            if(__instance.name == "GameCanvas")
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;

                if (!worldcam)
                {
                    worldcam = new GameObject("WorldCam");
                    worldcam.AddComponent<Camera>();
                    worldcam.GetComponent<Camera>().cullingMask = 100000;

                }

                worldcam.GetComponent<Camera>().enabled = true;
                canvas.worldCamera = worldcam.GetComponent<Camera>();


               // if (!rt)
               // {
                    rt = new RenderTexture(1310, 800, 24);

                    worldcam.GetComponent<Camera>().targetTexture = rt;

                    Logs.WriteInfo($"LLLLL newUI:");
                    newUI = new GameObject("newUI");
                    newUI.AddComponent<Canvas>();
                    newUI.AddComponent<RawImage>();
                    canvas = newUI.GetComponent<Canvas>();
                    canvas.renderMode = RenderMode.WorldSpace;
                    newUI.transform.localPosition = new Vector3(-8.2f,8f,29f);
                    newUI.transform.localScale = new Vector3(.16f, .09f, .1f);
                    newUI.transform.eulerAngles = new Vector3(0, -18f, 0);
                    newUI.GetComponent<RawImage>().texture = rt;

               // }

            }
            if (GameObject.Find("ClipboardRoot"))
            {
                clip = GameObject.Find("ClipboardRoot");
                clip.transform.position = new Vector3(2.1982f, -0.1573f, -10.1218f);
                clip.transform.localPosition = new Vector3(-2.1982f, .6127f, 2.3218f);
                clip.transform.localScale = new Vector3(1.9281f, 80.8271f, 1f);
                clip.transform.eulerAngles = new Vector3(90f, 221.049f, 0);
            }
            if (GameObject.Find("PosterCanvas"))
            {
                poster = GameObject.Find("PosterCanvas");
                poster.transform.position = new Vector3(2.1982f, -0.1573f, -10.1218f);
                poster.transform.localPosition = new Vector3(-2.1982f, .6127f, 3.8218f);
                //   poster.transform.localScale = new Vector3(1.9281f, 80.8271f, 1f);
                poster.transform.eulerAngles = new Vector3(352.3932f, 42.308f, 0);
            }
            __instance.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            __instance.scaleFactor = 1.5f;


            AttachedUi.Create<StaticUi>(canvas, 0.00045f);
         
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GymnastController), "Update")]
        private static void GetInput(GymnastController __instance)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (DummyCamera)
                {
                    if (fpmode)
                    {
                        VRCamera.transform.parent = DummyCamera2.transform;
                        SecondEye.transform.parent = DummyCamera2.transform;
                    }
                    else
                    {
                        VRCamera.transform.parent = DummyCamera.transform;
                        SecondEye.transform.parent = DummyCamera.transform;
                    }

                    fpmode = !fpmode;
                }

            }
        }

            [HarmonyPostfix]
        [HarmonyPatch(typeof(GymnastController), "Awake")]
        private static void MakeCamera(GymnastController __instance)
        {
            Logs.WriteInfo($"LLLLL: GymnastController");

            if (DummyCamera)
            {
                // first person mode
                VRCamera.transform.parent = DummyCamera.transform;
                SecondEye.transform.parent = DummyCamera.transform;
            }

            if (!DummyCamera)
            {
                Logs.WriteInfo($"LLLLL: CREATING DUMMY CAMERA:  {__instance.name} {__instance.tag}");

                VRPlayer = __instance.transform.Find("Head").gameObject;
                

                DummyCamera = new GameObject("DummyCamera");
                DummyCamera2 = new GameObject("DummyCamera2");
                DummyCamera2.transform.localPosition = new Vector3(0, 0, -10f);


                DummyCamera.transform.parent = __instance.transform.Find("Head");
                DummyCamera.transform.localPosition = new Vector3(-1.149f, -1.4554f, .1164f);
                DummyCamera.transform.eulerAngles = new Vector3(64.256f, 99.1011f, 6.3288f);
              
                VRCamera = GameObject.Find("Main Camera");


                VRCamera.transform.parent = DummyCamera.transform;
                VRCamera.GetComponent<PostProcessLayer>().enabled = false;
                VRCamera.GetComponent<Hydroform.HydroMultiCamComp>().enabled = false;
                VRCamera.AddComponent<SteamVR_TrackedObject>();

                SecondEye = new GameObject("SecondEye");
                SecondCam = SecondEye.AddComponent<Camera>();
                SecondCam.gameObject.AddComponent<SteamVR_TrackedObject>();
                SecondCam.gameObject.AddComponent<Cinemachine.CinemachineBrain>();
               // SecondCam.gameObject.AddComponent<Hydroform.HydroMultiCamComp>();
                SecondCam.CopyFrom(Camera.main);

                SecondCam.transform.parent = Camera.main.transform.parent;

            }
        }

        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FullScreenBlur), "OnEnable")]
        private static void REmoveBlur(FullScreenBlur __instance)
        {

            __instance.gameObject.GetComponent<PostProcessLayer>().enabled = false;

        }



        [HarmonyPrefix]
        [HarmonyPatch(typeof(Camera), "fieldOfView", MethodType.Setter)]
        private static bool suppressWarnings(Camera __instance)
        {
            if (__instance.stereoEnabled)
            {
                return false;
            }
            return true;
        }


        private static bool IsCanvasToIgnore(string canvasName)
        {
            foreach (var s in canvasesToIgnore)
                if (Equals(s, canvasName))
                    return true;
            return false;
        }

    }



}

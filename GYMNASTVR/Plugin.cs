using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using Valve.VR;
using UnityEngine.XR.Management;
using Unity.XR.OpenVR;
using System;

namespace GYMNASTVR
{

    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        
        public const string PLUGIN_GUID = "com.HerrFristi.VRMods.VRath";
        public const string PLUGIN_NAME = "VRMaker";
        public const string PLUGIN_VERSION = "0.0.2";

        public static string gameExePath = Process.GetCurrentProcess().MainModule.FileName;
        public static string gamePath = Path.GetDirectoryName(gameExePath);
        public static string HMDModel = "";

        public static UnityEngine.XR.Management.XRManagerSettings managerSettings = null;

        public static List<UnityEngine.XR.XRDisplaySubsystemDescriptor> displaysDescs = new List<UnityEngine.XR.XRDisplaySubsystemDescriptor>();
        public static List<UnityEngine.XR.XRDisplaySubsystem> displays = new List<UnityEngine.XR.XRDisplaySubsystem>();
        public static UnityEngine.XR.XRDisplaySubsystem MyDisplay = null;

        public static GameObject SecondEye = null;
        public static Camera SecondCam = null;
        public static OpenVRLoader xrLoader;
        public static GameObject Dummy1;

        //Create a class that actually inherits from MonoBehaviour
        public class MyStaticMB : MonoBehaviour
        {
        }

        //Variable reference for the class
        public static MyStaticMB myStaticMB;


        private void Awake()
        {
            
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

            //  new AssetLoader();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            //If the instance not exit the first time we call the static class
            if (myStaticMB == null)
            {
                //Create an empty object called MyStatic
                GameObject gameObject = new GameObject("MyStatic");


                //Add this script to the object
                myStaticMB = gameObject.AddComponent<MyStaticMB>();
            }

        }

        private void Start()
        {
         
            Logs.WriteInfo("LLLLLL: InitVRLoader: ");
            SteamVR_Actions.PreInitialize();

            var generalSettings = ScriptableObject.CreateInstance<XRGeneralSettings>();
            managerSettings = ScriptableObject.CreateInstance<XRManagerSettings>();
            xrLoader = ScriptableObject.CreateInstance<OpenVRLoader>();

            var settings = OpenVRSettings.GetSettings();
            settings.StereoRenderingMode = OpenVRSettings.StereoRenderingModes.MultiPass;

            generalSettings.Manager = managerSettings;

            managerSettings.loaders.Clear();
            managerSettings.loaders.Add(xrLoader);

            managerSettings.InitializeLoaderSync();


            Logs.WriteInfo("LLLLL:SteamVR Init");
            SteamVR.Initialize(true);
            Logs.WriteInfo("LLLLL: SteamVR Init Done");

            XRGeneralSettings.Instance.Manager.StartSubsystems();

            //Without this there is no headtracking
            Logs.WriteInfo("LLLLL: CanvasScaler SECOND CAM");
            Camera.main.gameObject.AddComponent<SteamVR_TrackedObject>();

            Dummy1 = new GameObject("Dummy1");
            Dummy1.transform.localPosition = new Vector3(.6f,-1.5f,-11.86f);
            Camera.main.transform.parent = Dummy1.transform;

            Plugin.SecondEye = new GameObject("SecondEye");
            Plugin.SecondCam = Plugin.SecondEye.AddComponent<Camera>();
            Plugin.SecondCam.gameObject.AddComponent<SteamVR_TrackedObject>();
            Plugin.SecondCam.gameObject.AddComponent<Cinemachine.CinemachineBrain>();
            Plugin.SecondCam.gameObject.AddComponent<Hydroform.HydroMultiCamComp>();
            Plugin.SecondCam.CopyFrom(Camera.main);

         
            Plugin.SecondCam.transform.parent = Camera.main.transform.parent;

            DontDestroyOnLoad(Dummy1);
           
            SubsystemManager.GetInstances(displays);
            MyDisplay = displays[0];
            MyDisplay.Start();
          
             Logs.WriteInfo("SteamVR hmd modelnumber: " + SteamVR.instance.hmd_ModelNumber);
               HMDModel = SteamVR.instance.hmd_ModelNumber;

         
        }



    }




    }



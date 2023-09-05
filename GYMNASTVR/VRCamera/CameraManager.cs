using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Valve.VR;

namespace GYMNASTVR
{
    public static class CameraManager
    {
        static CameraManager()
        {
            Logs.WriteInfo("LLLLLLL: CameraManager");
            CurrentCameraMode = VRCameraMode.UI;
            //Fix near plance clipping for main camera
            if (Camera.main != null)
            {
                Camera.main.nearClipPlane = NearClipPlaneDistance;
                Camera.main.farClipPlane = FarClipPlaneDistance;
            }
            // Fix it for the second stereo camera
            if (Plugin.SecondCam != null)
            {
                Plugin.SecondCam.nearClipPlane = NearClipPlaneDistance;
                Plugin.SecondCam.farClipPlane = FarClipPlaneDistance;
            }

        }

        public static void ReduceNearClipping()
        {
            Camera CurrentCamera = Camera.main;
            CurrentCamera.nearClipPlane = NearClipPlaneDistance;
            CurrentCamera.farClipPlane = FarClipPlaneDistance;

            // Fix it for the second stereo camera
            if (Plugin.SecondCam != null)
            {
                Plugin.SecondCam.nearClipPlane = NearClipPlaneDistance;
                Plugin.SecondCam.farClipPlane = FarClipPlaneDistance;
            }
        }
    

      
        public static void HandleStereoRendering()
        {

          //  Logs.WriteInfo($"LLLLLLL: HandleStereoRendering {Camera.main.gameObject.name}");
            Camera.main.fieldOfView = SteamVR.instance.fieldOfView;
            Camera.main.stereoTargetEye = StereoTargetEyeMask.Left;
            Camera.main.projectionMatrix = Camera.main.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
            Camera.main.targetTexture = Plugin.MyDisplay.GetRenderTextureForRenderPass(0);



            if (CameraPatches.DummyCamera)
            {
                Plugin.SecondEye.SetActive(false);

                CameraPatches.VRCamera.GetComponent<Hydroform.HydroMultiCamComp>().enabled = false;
                CameraPatches.SecondEye.transform.position = Camera.main.transform.position;
                CameraPatches.SecondEye.transform.rotation = Camera.main.transform.rotation;
                CameraPatches.SecondEye.transform.localScale = Camera.main.transform.localScale;
                CameraPatches.SecondCam.enabled = true;
                CameraPatches.SecondCam.stereoTargetEye = StereoTargetEyeMask.Right;
                CameraPatches.SecondCam.projectionMatrix = CameraPatches.SecondCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                CameraPatches.SecondCam.targetTexture = Plugin.MyDisplay.GetRenderTextureForRenderPass(1);
            }
            else
            {
                Plugin.SecondEye.transform.position = Camera.main.transform.position;
                Plugin.SecondEye.transform.rotation = Camera.main.transform.rotation;
                Plugin.SecondEye.transform.localScale = Camera.main.transform.localScale;
                Plugin.SecondCam.enabled = true;
                Plugin.SecondCam.stereoTargetEye = StereoTargetEyeMask.Right;
                Plugin.SecondCam.projectionMatrix = Plugin.SecondCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                Plugin.SecondCam.targetTexture = Plugin.MyDisplay.GetRenderTextureForRenderPass(1);
            }

        }



        public enum VRCameraMode
        {
            DemeoLike,
            FirstPerson,
            Cutscene,
            UI
        }

        //Strictly camera stuff
        public static VRCameraMode CurrentCameraMode;
        public static float NearClipPlaneDistance = 0.01f;
        public static float FarClipPlaneDistance = 59999f;
        public static bool DisableParticles = false;

        // VR Origin and body stuff
        public static Transform OriginalCameraParent = null;
        public static GameObject VROrigin = new GameObject();
        public static GameObject LeftHand = null;
        public static GameObject RightHand = null;
        

        // VR Input stuff
        public static bool RightHandGrab = false;
        public static bool LeftHandGrab = false;
        public static Vector2 LeftJoystick = Vector2.zero;
        public static Vector2 RightJoystick = Vector2.zero;

        // Demeo-like camera stuff
        public static float InitialHandDistance = 0f;
        public static bool InitialRotation = true;
        public static Vector3 PreviousRotationVector = Vector3.zero;
        public static Vector3 InitialRotationPoint = Vector3.zero;
        public static Vector3 ZoomOrigin = Vector3.zero;
        public static float SpeedScalingFactor = 1f;

        // FIrst person camera stuff
        public static float Turnrate = 3f;

    }
    
}

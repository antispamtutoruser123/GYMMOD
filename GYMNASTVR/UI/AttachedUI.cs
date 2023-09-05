using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace GYMNASTVR
{
    class AttachedUi : MonoBehaviour
    {
        private Transform targetTransform;

        public static void Create<TAttachedUi>(Canvas canvas, float scale = 0)
            where TAttachedUi : AttachedUi
        {
            var instance = canvas.gameObject.AddComponent<TAttachedUi>();
         //   if (scale > 0) canvas.transform.localScale = Vector3.one * scale;
         //   canvas.renderMode = RenderMode.WorldSpace;
        }

        protected virtual void Update()
        {
            UpdateTransform();
            // The only way i seem to call this stuff
            //Logs.WriteInfo("Update hook called");
   
            if(Plugin.SecondCam && Camera.main)
            {
                CameraManager.HandleStereoRendering();
            }

        }

        public void SetTargetTransform(Transform target)
        {
            targetTransform = target;
        }

        private void UpdateTransform()
        {
         /*   if (Camera.main)
            {
                transform.position = Camera.main.transform.position + Camera.main.transform.forward;
                transform.rotation = Camera.main.transform.rotation;
            }
            */
        }
    }
}

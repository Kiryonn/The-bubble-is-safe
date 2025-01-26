using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Artngame.SKYMASTER
{
    public class depthBufferSecondCameraSM : MonoBehaviour
    {
        public bool useDepthHistory = false;
        public RenderTexture historyRT;
        public RenderTexture historyRTtmp;
        // Start is called before the first frame update
        void Start()
        {
            if (topDownCamera != null)
            {
                topDownCamera.depthTextureMode = DepthTextureMode.Depth;
                Camera mainCamera = Camera.main;
                if (targetColor == null)
                {
                    targetColor = new RenderTexture(sizeX, sizeY, 16, RenderTextureFormat.Default);
                }
                if (targetDepth == null)
                {
                    targetDepth = new RenderTexture(sizeX, sizeY, 24, RenderTextureFormat.Depth);
                }
                topDownCamera.targetTexture = targetDepth;
                topDownCamera.SetTargetBuffers(targetColor.colorBuffer, targetDepth.depthBuffer);
                topDownCamera.targetTexture = targetDepth;
                Shader.SetGlobalTexture(targetColorTexName, targetColor);
                Shader.SetGlobalTexture(targetDepthTexName, targetDepth);
            }

            historyRT = new RenderTexture(sizeX, sizeY, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            historyRTtmp = new RenderTexture(sizeX, sizeY, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            if (useDepthHistory)
            {
                historyMat.SetFloat("resetInit", 1);
                Graphics.Blit(historyRT, historyRTtmp);
                historyMat.SetTexture("_historyTex", historyRTtmp);
                Graphics.Blit(targetDepth, historyRT, historyMat);
                Shader.SetGlobalTexture(targetDepthTexName, historyRT);
            }
        }
        public RenderTexture targetColor;
        public RenderTexture targetDepth;
        public string targetColorTexName = "_topDownColor";
        public string targetDepthTexName = "_topDownDepth";
        public Camera topDownCamera;
        public int sizeX = 512;
        public int sizeY = 512;

        public Material historyMat;

        public float RestoreSnowPower = 0;
        public float restoreSnowFrequency = 0.002f;
        public float restoreSnowDetail = 512;

        // Update is called once per frame
        void Update()
        {
            if (useDepthHistory)
            {
                historyMat.SetFloat("resetInit",0);
                historyMat.SetFloat("_RestoreSnowPower", RestoreSnowPower);
                historyMat.SetFloat("_restoreSnowFrequency", restoreSnowFrequency);
                historyMat.SetFloat("_restoreSnowDetail", restoreSnowDetail);

                Graphics.Blit(historyRT, historyRTtmp);
                historyMat.SetTexture("_historyTex", historyRTtmp);
                Graphics.Blit(targetDepth, historyRT, historyMat);
            }
        }
    }
}
using Artngame.SKYMASTER;
using PlanetaryTerrain.Atmosphere;
using UnityEngine.Experimental.Rendering;

#if UNITY_2023_3_OR_NEWER
//GRAPH
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
#endif

namespace UnityEngine.Rendering.Universal
{
    /// <summary>
    /// Copy the given color buffer to the given destination color buffer.
    ///
    /// You can use this pass to copy a color buffer to the destination,
    /// so you can use it later in rendering. For example, you can copy
    /// the opaque texture to use it for distortion effects.
    /// </summary>
    internal class BlitPassAtmosSRP : UnityEngine.Rendering.Universal.ScriptableRenderPass
    {

#if UNITY_2023_3_OR_NEWER
        /// <summary>
        /// ///////// GRAPH
        /// </summary>
        // This class stores the data needed by the pass, passed as parameter to the delegate function that executes the pass
        private class PassData
        {    //v0.1               
            internal TextureHandle src;
            //internal TextureHandle tmpBuffer1;
            // internal TextureHandle copySourceTexture;
            public Material BlitMaterial { get; set; }
            // public TextureHandle SourceTexture { get; set; }
        }
        private Material m_BlitMaterial;
        TextureHandle tmpBuffer1A;
        TextureHandle tmpBuffer2A;
        TextureHandle tmpBuffer3A;
        TextureHandle previousFrameTextureA;
        TextureHandle previousDepthTextureA;
        TextureHandle currentDepth;

        RTHandle _handleTAART;
        TextureHandle _handleTAA;

        RTHandle _handleA; RTHandle _handleB; RTHandle _handleC; RTHandle _handleD; RTHandle _handleE;

        Camera currentCamera;
        float prevDownscaleFactor;//v0.1

        //TextureHandle currentDepth;
        //TextureHandle currentNormal;
        // Each ScriptableRenderPass can use the RenderGraph handle to add multiple render passes to the render graph
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (Camera.main != null)
            {
                m_BlitMaterial = blitMaterial;
                Material CloudMaterial = blitMaterial;

                Camera.main.depthTextureMode = DepthTextureMode.Depth;

                UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                RenderTextureDescriptor desc = cameraData.cameraTargetDescriptor;
                UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
                UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

                if (Camera.main != null && cameraData.camera != Camera.main)
                {
                    return;
                }
                desc.msaaSamples = 1;
                desc.depthBufferBits = 0;
                int rtW = desc.width;
                int rtH = desc.height;
                int xres = (int)(rtW / ((float)downSample));
                int yres = (int)(rtH / ((float)downSample));
                if (_handleA == null || _handleA.rt.width != xres || _handleA.rt.height != yres)
                {
                    _handleA = RTHandles.Alloc(xres, yres, colorFormat: GraphicsFormat.R32G32B32A32_SFloat, dimension: TextureDimension.Tex2D);
                }
                if (_handleB == null || _handleB.rt.width != xres || _handleB.rt.height != yres)
                {
                    _handleB = RTHandles.Alloc(xres, yres, colorFormat: GraphicsFormat.R32G32B32A32_SFloat, dimension: TextureDimension.Tex2D);
                }
                if (_handleC == null || _handleC.rt.width != xres || _handleC.rt.height != yres)
                {
                    _handleC = RTHandles.Alloc(xres, yres, colorFormat: GraphicsFormat.R32G32B32A32_SFloat, dimension: TextureDimension.Tex2D);
                }
                tmpBuffer2A = renderGraph.ImportTexture(_handleA);
                previousFrameTextureA = renderGraph.ImportTexture(_handleB);
                previousDepthTextureA = renderGraph.ImportTexture(_handleC);
                tmpBuffer1A = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "tmpBuffer1A", true);

                if (_handleD == null || _handleD.rt.width != xres || _handleD.rt.height != yres)
                {
                    _handleD = RTHandles.Alloc(xres, yres, colorFormat: GraphicsFormat.R32G32B32A32_SFloat, dimension: TextureDimension.Tex2D);
                }
                tmpBuffer3A = renderGraph.ImportTexture(_handleD);// UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "tmpBuffer3A", true);
                
                if (_handleE == null || _handleE.rt.width != xres || _handleE.rt.height != yres)
                {
                    _handleE = RTHandles.Alloc(xres, yres, colorFormat: GraphicsFormat.R32G32B32A32_SFloat, dimension: TextureDimension.Tex2D);
                }
                currentDepth = renderGraph.ImportTexture(_handleE);

                //currentDepth = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "currentDepth", true);
                if (_handleTAART == null || _handleTAART.rt.width != xres || _handleTAART.rt.height != yres)
                {
                    _handleTAART = RTHandles.Alloc(xres, yres, colorFormat: GraphicsFormat.R32G32B32A32_SFloat, dimension: TextureDimension.Tex2D);
                    _handleTAART.rt.wrapMode = TextureWrapMode.Clamp;
                    _handleTAART.rt.filterMode = FilterMode.Bilinear;
                }
                _handleTAA = renderGraph.ImportTexture(_handleTAART);

                //currentDepth = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "currentDepth", true);
                //currentNormal = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "currentNormal", true);              
                TextureHandle sourceTexture = resourceData.activeColorTexture;
                //Debug.Log("IN2");
                //grab settings if script on scene camera

                connector = cameraData.camera.GetComponent<connectSuntoAtmosURP>();
                // currentCamera = Camera.main;// renderingData.cameraData.camera;
                currentCamera = cameraData.camera;

                if (connector == null)
                {
                    if (connector == null && Camera.main != null)
                    {
                        connector = Camera.main.GetComponent<connectSuntoAtmosURP>();
                    }
                }

                //Debug.Log(Camera.main.GetComponent<connectSuntoSunShaftsURP>().sun.transform.position);
                if (connector != null)
                {
                    this.blendCouds = connector.blendCouds;

                    this.enableShafts = connector.enableShafts;
                    this.sunTransform = connector.sun.transform.position;
                    this.screenBlendMode = connector.screenBlendMode;
                    //public Vector3 sunTransform = new Vector3(0f, 0f, 0f); 
                    this.radialBlurIterations = connector.radialBlurIterations;
                    this.sunColor = connector.sunColor;
                    this.sunThreshold = connector.sunThreshold;
                    this.sunShaftBlurRadius = connector.sunShaftBlurRadius;
                    this.sunShaftIntensity = connector.sunShaftIntensity;
                    this.maxRadius = connector.maxRadius;
                    this.useDepthTexture = connector.useDepthTexture;

                    //////////// ATMOS
                    toggleDepthMode = connector.toggleDepthMode;
                    kSunAngularRadius = connector.kSunAngularRadius;
                    planet = connector.planet;
                    Sun = connector.Sun;
                    topBottomRadiusRatio = connector.topBottomRadiusRatio;
                    seaLevelHeight = connector.seaLevelHeight;
                    UseConstantSolarSpectrum = connector.UseConstantSolarSpectrum;
                    UseOzone = connector.UseOzone;
                    UseCombinedTextures = connector.UseCombinedTextures;
                    UseHalfPrecision = connector.UseHalfPrecision;
                    DoWhiteBalance = connector.DoWhiteBalance;
                    UseLuminance = connector.UseLuminance;
                    Exposure = connector.Exposure;
                    m_compute = connector.m_compute;
                    m_material = connector.m_material;
                    m_model = connector.m_model;
                    m_camera = connector.m_camera;
                    kBottomRadius = connector.kBottomRadius;
                    kLengthUnitInMeters = connector.kLengthUnitInMeters;

                    CAMERA_FOV = connector.CAMERA_FOV;
                    CAMERA_ASPECT_RATIO = connector.CAMERA_ASPECT_RATIO;
                    CAMERA_NEAR = connector.CAMERA_NEAR;
                    CAMERA_FAR = connector.CAMERA_FAR;
                    fovWHalf = connector.fovWHalf;
                    c1 = connector.c1;
                    c2 = connector.c2;

                    AtmoBrightness = connector.AtmoBrightness;
                    Fade = connector.Fade;
                    FadeStart = connector.FadeStart;
                    FadeEnd = connector.FadeEnd;
                    Beta = connector.Beta;
                    blendControls = connector.blendControls;
                }

                //if still null, disable effect
                bool connectorFound = true;
                if (connector == null)
                {
                    connectorFound = false;
                }

                if (enableShafts && connectorFound && connector.enabled && currentCamera == Camera.main)
                {
                    CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

                    RenderTextureDescriptor opaqueDesc = cameraData.cameraTargetDescriptor;
                    opaqueDesc.depthBufferBits = 0;




                    Camera camera = Camera.main;
                    var formatA = camera.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default; //v3.4.9
                                                                                                                  //       RenderTexture lrDepthBufferATM = RenderTexture.GetTemporary(opaqueDesc.width, opaqueDesc.height, 0, formatA);
                                                                                                                  //       RenderTexture tmpBuffer = RenderTexture.GetTemporary(opaqueDesc.width, opaqueDesc.height, 0, formatA);
                                                                                                                  //       RenderTexture tmpDEPTH = RenderTexture.GetTemporary(opaqueDesc.width, opaqueDesc.height, 0, formatA);

                    //       cmd.Blit(source, tmpBuffer2A);

                    //connector.SetUp();


                    string passName = "BLIT1 Keep Source";
                    using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
                    {
                        passData.src = resourceData.activeColorTexture;
                        desc.msaaSamples = 1; desc.depthBufferBits = 0;
                        builder.UseTexture(passData.src, AccessFlags.Read);
                        builder.SetRenderAttachment(tmpBuffer1A, 0, AccessFlags.Write);
                        builder.AllowPassCulling(false);
                        passData.BlitMaterial = m_BlitMaterial;
                        builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                            ExecuteBlitPass(data, context, 9, passData.src));
                    }

                    //DEPTH
                    //       cmd.Blit(source, tmpBuffer1A, m_material, 6);

                    int depthpass = 6;
                    if (toggleDepthMode)
                    {
                        depthpass = 11;
                    }

                    passName = "GetCameraDepthTexture";
                    using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
                    {
                        passData.src = resourceData.activeColorTexture;
                        desc.msaaSamples = 1; desc.depthBufferBits = 0;
                        builder.UseTexture(tmpBuffer1A, AccessFlags.Read);
                        builder.SetRenderAttachment(currentDepth, 0, AccessFlags.Write);
                        builder.AllowPassCulling(false);
                        passData.BlitMaterial = m_BlitMaterial;
                        builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                           // ExecuteBlitPass(data, context, LUMINA.Pass.GetCameraDepthTexture, tmpBuffer1A));
                           ExecuteBlitPass(data, context, depthpass, tmpBuffer1A));
                    }

                              


                 

                    m_material.SetFloat("divideSky", blendCouds);
                    m_material.SetFloat("_Brightness", AtmoBrightness);
                    m_material.SetFloat("_Fade", Fade);
                    m_material.SetFloat("_FadeStart", FadeStart);
                    m_material.SetFloat("_FadeEnd", FadeEnd);
                    m_material.SetFloat("_Beta", Beta);
                    m_material.SetVector("_blendControls", blendControls);
                    //v0.1
                    //GL.ClearWithSkybox(false, camera);
                    //renderATMOS(tmpBuffer2A, lrDepthBufferATM, cmd);
                    
                    //cmd.Blit(tmpDEPTH, source);
         //           m_material.SetTexture("_CameraDepthCustom", currentDepth);/////////////////////////////////////
         //           m_material.SetTexture("_MainTex", src);

                    var p = GL.GetGPUProjectionMatrix(m_camera.projectionMatrix, false);
                    p[2, 3] = p[3, 2] = 0.0f;
                    p[3, 3] = 1.0f;
                    var clipToWorld = Matrix4x4.Inverse(p * m_camera.worldToCameraMatrix) * Matrix4x4.TRS(new Vector3(0, 0, -p[2, 2]), Quaternion.identity, Vector3.one);

                    m_material.SetMatrix("clip_to_world", clipToWorld);

                    m_material.SetVector("earth_center", planet.transform.position);

                    m_material.SetVector("sun_direction", ((Sun == null) ? Vector3.up : Sun.transform.forward) * -1.0f);

                    Matrix4x4 frustumCorners = Matrix4x4.identity;

                    Vector3 toRight = m_camera.transform.right * c1;
                    Vector3 toTop = m_camera.transform.up * c2;

                    Vector3 topLeft = (m_camera.transform.forward * CAMERA_NEAR - toRight + toTop);
                    float CAMERA_SCALE = topLeft.magnitude * CAMERA_FAR / CAMERA_NEAR;

                    topLeft.Normalize();
                    topLeft *= CAMERA_SCALE;

                    Vector3 topRight = (m_camera.transform.forward * CAMERA_NEAR + toRight + toTop);
                    topRight.Normalize();
                    topRight *= CAMERA_SCALE;

                    Vector3 bottomRight = (m_camera.transform.forward * CAMERA_NEAR + toRight - toTop);
                    bottomRight.Normalize();
                    bottomRight *= CAMERA_SCALE;

                    Vector3 bottomLeft = (m_camera.transform.forward * CAMERA_NEAR - toRight - toTop);
                    bottomLeft.Normalize();
                    bottomLeft *= CAMERA_SCALE;

                    frustumCorners.SetRow(0, topLeft);
                    frustumCorners.SetRow(1, topRight);
                    frustumCorners.SetRow(2, bottomRight);
                    frustumCorners.SetRow(3, bottomLeft);

                    //Debug.Log("GRAPH");
                    //Debug.Log(topLeft);
                    //Debug.Log(topRight);
                    //Debug.Log(bottomRight);
                    //Debug.Log(bottomLeft);


                    Vector3[] frustumCornersA = new Vector3[4];
                    camera.CalculateFrustumCorners(new Rect(0,0,4,4), camera.farClipPlane*1000, Camera.MonoOrStereoscopicEye.Mono, frustumCornersA);
                    //frustumCorners.SetRow(0, frustumCornersA[0]);
                    //frustumCorners.SetRow(1, frustumCornersA[1]);
                    //frustumCorners.SetRow(2, frustumCornersA[2]);
                    //frustumCorners.SetRow(3, frustumCornersA[3]);

                    m_material.SetMatrix("frustumCorners", frustumCorners);


                    passName = "Blend Clouds";
                    using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
                    {
                        passData.src = resourceData.activeColorTexture;
                        desc.msaaSamples = 1; desc.depthBufferBits = 0;
                        builder.UseTexture(tmpBuffer1A, AccessFlags.Read);
                        builder.UseTexture(currentDepth, AccessFlags.Read);
                        builder.SetRenderAttachment(tmpBuffer3A, 0, AccessFlags.Write);
                        builder.AllowPassCulling(false);
                        passData.BlitMaterial = m_BlitMaterial;
                        builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                            ExecuteBlitPassTWO_MATRIX(data, context, 10, tmpBuffer1A, currentDepth, frustumCorners));
                    }
                    //                  cmd.Blit(src, dest, m_material, 7);


                    // Debug.Log("IN");
                    //TESTER CODE
                    using (var builder = renderGraph.AddRasterRenderPass<PassData>("Color Blit Resolve", out var passData, m_ProfilingSampler))
                    {
                        passData.BlitMaterial = m_BlitMaterial;
                        // Similar to the previous pass, however now we set destination texture as input and source as output.
                        builder.UseTexture(tmpBuffer3A, AccessFlags.Read);
                        passData.src = tmpBuffer3A;

                       builder.SetRenderAttachment(sourceTexture, 0, AccessFlags.Write);
                        // We use the same BlitTexture API to perform the Blit operation.
                        builder.SetRenderFunc((PassData data, RasterGraphContext rgContext) => ExecutePass(data, rgContext));
                    }
                    return;
                    //END TESTER CODE


                    //                 cmd.Blit(lrDepthBufferATM, source);

                    //context.ExecuteCommandBuffer(cmd);
                    //CommandBufferPool.Release(cmd);
                    //RenderTexture.ReleaseTemporary(lrDepthBufferATM);
                    //RenderTexture.ReleaseTemporary(tmpBuffer2A);
                    //RenderTexture.ReleaseTemporary(tmpDEPTH);




                }//END ENABLE SHAFTS


            }//END CAMERA CHECK
        }
        static void ExecuteBlitPassTEX9NAME(PassData data, RasterGraphContext context, int pass,
         string texname1, TextureHandle tmpBuffer1,
         string texname2, TextureHandle tmpBuffer2,
         string texname3, TextureHandle tmpBuffer3,
         string texname4, TextureHandle tmpBuffer4,
         string texname5, TextureHandle tmpBuffer5,
         string texname6, TextureHandle tmpBuffer6,
         string texname7, TextureHandle tmpBuffer7,
         string texname8, TextureHandle tmpBuffer8,
         string texname9, TextureHandle tmpBuffer9,
         string texname10, TextureHandle tmpBuffer10
         )
        {
            data.BlitMaterial.SetTexture(texname1, tmpBuffer1);
            data.BlitMaterial.SetTexture(texname2, tmpBuffer2);
            data.BlitMaterial.SetTexture(texname3, tmpBuffer3);
            data.BlitMaterial.SetTexture(texname4, tmpBuffer4);
            data.BlitMaterial.SetTexture(texname5, tmpBuffer5);
            data.BlitMaterial.SetTexture(texname6, tmpBuffer6);
            data.BlitMaterial.SetTexture(texname7, tmpBuffer7);
            data.BlitMaterial.SetTexture(texname8, tmpBuffer8);
            data.BlitMaterial.SetTexture(texname9, tmpBuffer9);
            data.BlitMaterial.SetTexture(texname10, tmpBuffer10);
            Blitter.BlitTexture(context.cmd, data.src, new Vector4(1, 1, 0, 0), data.BlitMaterial, pass);
        }
        //temporal
        static void ExecuteBlitPassTEN(PassData data, RasterGraphContext context, int pass,
            TextureHandle tmpBuffer1, TextureHandle tmpBuffer2, TextureHandle tmpBuffer3,
            string varname1, float var1,
            string varname2, float var2,
            string varname3, Matrix4x4 var3,
            string varname4, Matrix4x4 var4,
            string varname5, Matrix4x4 var5,
            string varname6, Matrix4x4 var6,
            string varname7, Matrix4x4 var7
            )
        {
            data.BlitMaterial.SetTexture("_CloudTex", tmpBuffer1);
            data.BlitMaterial.SetTexture("_PreviousColor", tmpBuffer2);
            data.BlitMaterial.SetTexture("_PreviousDepth", tmpBuffer3);
            Blitter.BlitTexture(context.cmd, data.src, new Vector4(1, 1, 0, 0), data.BlitMaterial, pass);
            //lastFrameViewProjectionMatrix = viewProjectionMatrix;
            //lastFrameInverseViewProjectionMatrix = viewProjectionMatrix.inverse;
        }

        static void ExecuteBlitPassTHREE(PassData data, RasterGraphContext context, int pass,
            TextureHandle tmpBuffer1, TextureHandle tmpBuffer2, TextureHandle tmpBuffer3)
        {
            data.BlitMaterial.SetTexture("_ColorBuffer", tmpBuffer1);
            data.BlitMaterial.SetTexture("_PreviousColor", tmpBuffer2);
            data.BlitMaterial.SetTexture("_PreviousDepth", tmpBuffer3);
            Blitter.BlitTexture(context.cmd, data.src, new Vector4(1, 1, 0, 0), data.BlitMaterial, pass);
        }
        static void ExecuteBlitPass(PassData data, RasterGraphContext context, int pass, TextureHandle tmpBuffer1aa)
        {
            data.BlitMaterial.SetTexture("_MainTex", tmpBuffer1aa);
            Blitter.BlitTexture(context.cmd, data.src, new Vector4(1, 1, 0, 0), data.BlitMaterial, pass);
        }
        static void ExecuteBlitPassTWO(PassData data, RasterGraphContext context, int pass, TextureHandle tmpBuffer1, TextureHandle tmpBuffer2)
        {
            data.BlitMaterial.SetTexture("_MainTex", tmpBuffer1);// _CloudTexP", tmpBuffer1);
            data.BlitMaterial.SetTexture("_CameraDepthCustom", tmpBuffer2);
            Blitter.BlitTexture(context.cmd, data.src, new Vector4(1, 1, 0, 0), data.BlitMaterial, pass);
        }
        static void ExecuteBlitPassTWO_MATRIX(PassData data, RasterGraphContext context, int pass, TextureHandle tmpBuffer1, TextureHandle tmpBuffer2, Matrix4x4 matrix)
        {
            data.BlitMaterial.SetTexture("_MainTex", tmpBuffer1);// _CloudTexP", tmpBuffer1);
            data.BlitMaterial.SetTexture("_CameraDepthCustom", tmpBuffer2);
            data.BlitMaterial.SetMatrix("frustumCorners", matrix);
            Blitter.BlitTexture(context.cmd, data.src, new Vector4(1, 1, 0, 0), data.BlitMaterial, pass);
        }
        static void ExecuteBlitPassTEXNAME(PassData data, RasterGraphContext context, int pass, TextureHandle tmpBuffer1aa, string texname)
        {
            data.BlitMaterial.SetTexture(texname, tmpBuffer1aa);
            Blitter.BlitTexture(context.cmd, data.src, new Vector4(1, 1, 0, 0), data.BlitMaterial, pass);
        }
        static void ExecuteBlitPassTEX5NAME(PassData data, RasterGraphContext context, int pass,
            string texname1, TextureHandle tmpBuffer1,
            string texname2, TextureHandle tmpBuffer2,
            string texname3, TextureHandle tmpBuffer3,
            string texname4, TextureHandle tmpBuffer4,
            string texname5, TextureHandle tmpBuffer5
            )
        {
            data.BlitMaterial.SetTexture(texname1, tmpBuffer1);
            data.BlitMaterial.SetTexture(texname2, tmpBuffer2);
            data.BlitMaterial.SetTexture(texname3, tmpBuffer3);
            data.BlitMaterial.SetTexture(texname4, tmpBuffer4);
            data.BlitMaterial.SetTexture(texname5, tmpBuffer5);
            Blitter.BlitTexture(context.cmd, data.src, new Vector4(1, 1, 0, 0), data.BlitMaterial, pass);
        }
        // It is static to avoid using member variables which could cause unintended behaviour.
        static void ExecutePass(PassData data, RasterGraphContext rgContext)
        {
            Blitter.BlitTexture(rgContext.cmd, data.src, new Vector4(1, 1, 0, 0), data.BlitMaterial, 8);
        }
        //private Material m_BlitMaterial;
        private ProfilingSampler m_ProfilingSampler = new ProfilingSampler("After Opaques");
        ////// END GRAPH
#endif








        public int downSample = 1;
        public bool toggleDepthMode = false;



        //v0.4  - Unity 2020.1
#if UNITY_2020_2_OR_NEWER
        public BlitAtmosSRP.BlitSettings settings;


#if UNITY_2022_1_OR_NEWER
        RTHandle _handle;
#else
        RenderTargetHandle _handle;
#endif

        public override void OnCameraSetup(CommandBuffer cmd, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
        {
            //_handle.Init(settings.textureId);
            //destination = (settings.destination == BlitSunShaftsSRP.Target.Color)
            //    ? UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget
            //    : _handle;

            //var renderer = renderingData.cameraData.renderer;
            //source = renderer.cameraColorTarget;
            var renderer = renderingData.cameraData.renderer;

#if UNITY_2022_1_OR_NEWER
            //v0.1
            //_handle.Init(settings.textureId);
            _handle = RTHandles.Alloc(settings.textureId, name: settings.textureId);
            destination = (settings.destination == BlitAtmosSRP.Target.Color)
                ? renderingData.cameraData.renderer.cameraColorTargetHandle //cameraColorTarget//  cameraTargetDescriptor //  UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget
                : _handle;

            //v0.1
            //source = renderer.cameraColorTarget;
            source = renderer.cameraColorTargetHandle;
#else
            //v0.1
            //_handle.Init(settings.textureId);
            _handle.Init(settings.textureId);
            destination = (settings.destination == BlitAtmosSRP.Target.Color)
                ?  UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget
                : _handle;

            //v0.1
            source = renderer.cameraColorTarget;            
#endif
        }
#endif

        public float blendCouds = 3.87f;
        public bool enableShafts = true;
        //SUN SHAFTS         
        public BlitAtmosSRP.BlitSettings.SunShaftsResolution resolution = BlitAtmosSRP.BlitSettings.SunShaftsResolution.Normal;
        public BlitAtmosSRP.BlitSettings.ShaftsScreenBlendMode screenBlendMode = BlitAtmosSRP.BlitSettings.ShaftsScreenBlendMode.Screen;
        public Vector3 sunTransform = new Vector3(0f, 0f, 0f); // Transform sunTransform;
        public int radialBlurIterations = 2;
        public Color sunColor = Color.white;
        public Color sunThreshold = new Color(0.87f, 0.74f, 0.65f);
        public float sunShaftBlurRadius = 2.5f;
        public float sunShaftIntensity = 1.15f;
        public float maxRadius = 0.75f;
        public bool useDepthTexture = true;
        public float blend = 0.5f;

        ////////////////////////// ATMOS //////////////////////////////////
        public float kSunAngularRadius = 0.00935f / 2.0f;
        public PlanetaryTerrain.Planet planet;
        public Light Sun;
        public double topBottomRadiusRatio = 1.01711;
        public float seaLevelHeight = 0;
        public bool UseConstantSolarSpectrum = false;
        public bool UseOzone = true;
        public bool UseCombinedTextures = true;
        public bool UseHalfPrecision = false;
        public bool DoWhiteBalance = false;
        public LUMINANCE UseLuminance = LUMINANCE.NONE;
        public float Exposure = 10.0f;
        public ComputeShader m_compute;
        public Material m_material;
        public Model m_model;
        public Camera m_camera;
        public float kBottomRadius = 6371000f;
        public float kLengthUnitInMeters;

        public float CAMERA_FOV;
        public float CAMERA_ASPECT_RATIO;
        public float CAMERA_NEAR;
        public float CAMERA_FAR;
        public float fovWHalf;
        public float c1, c2;

        //v0.1
        public float AtmoBrightness = 1.8f;
        public float Fade = 1;
        public float FadeStart = 10000;
        public float FadeEnd = 110000;
        public float Beta = 5;
        public Vector4 blendControls = new Vector4(1, 1, 1, 1);

        /// <summary>
        /// //////////// END ATMOS
        /// </summary>


        public enum RenderTarget
        {
            Color,
            RenderTexture,
        }

        public Material blitMaterial = null;
        public int blitShaderPassIndex = 0;
        public FilterMode filterMode { get; set; }

        private RenderTargetIdentifier source { get; set; }

        //private UnityEngine.Rendering.Universal.RenderTargetHandle destination { get; set; }
#if UNITY_2022_1_OR_NEWER
        private RTHandle destination { get; set; }
#else
        private RenderTargetHandle destination { get; set; }
#endif

        //RTHandle m_TemporaryColorTexture;
        string m_ProfilerTag;


        //SUN SHAFTS
        RenderTexture lrColorB;
       // RenderTexture lrDepthBuffer;
       // RenderTargetHandle lrColorB;
       // RTHandle lrDepthBuffer;

        /// <summary>
        /// Create the CopyColorPass
        /// </summary>
        public BlitPassAtmosSRP(UnityEngine.Rendering.Universal.RenderPassEvent renderPassEvent, Material blitMaterial, int blitShaderPassIndex, string tag, BlitAtmosSRP.BlitSettings settings)
        {
            this.renderPassEvent = renderPassEvent;
            this.blitMaterial = blitMaterial;
            this.blitShaderPassIndex = blitShaderPassIndex;
            m_ProfilerTag = tag;
            //m_TemporaryColorTexture.Init("_TemporaryColorTexture");
           // m_TemporaryColorTexture = RTHandles.Alloc("_TemporaryColorTexture", name: "_TemporaryColorTexture");
            //lrDepthBuffer = RTHandles.Alloc("lrDepthBuffer", name: "lrDepthBuffer");

            //SUN SHAFTS
            this.resolution = settings.resolution;
            this.screenBlendMode = settings.screenBlendMode;
            this.sunTransform = settings.sunTransform;
            this.radialBlurIterations = settings.radialBlurIterations;
            this.sunColor = settings.sunColor;
            this.sunThreshold = settings.sunThreshold;
            this.sunShaftBlurRadius = settings.sunShaftBlurRadius;
            this.sunShaftIntensity = settings.sunShaftIntensity;
            this.maxRadius = settings.maxRadius;
            this.useDepthTexture = settings.useDepthTexture;
            this.blend = settings.blend;
    }

        /// <summary>
        /// Configure the pass with the source and destination to execute on.
        /// </summary>
        /// <param name="source">Source Render Target</param>
        /// <param name="destination">Destination Render Target</param>
#if UNITY_2022_1_OR_NEWER
        public void Setup(RenderTargetIdentifier source, RTHandle destination)
        {
            this.source = source;
            this.destination = destination;
        }
#else
        public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination)
        {
            this.source = source;
            this.destination = destination;
        }
#endif


        connectSuntoAtmosURP connector;

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
        {            
            //grab settings if script on scene camera
            if (connector == null)
            {
                connector = renderingData.cameraData.camera.GetComponent<connectSuntoAtmosURP>();
                if(connector == null && Camera.main != null)
                {
                    connector = Camera.main.GetComponent<connectSuntoAtmosURP>();

                    //v0.2
                    if (connector == null)
                    {
                        try
                        {
                            GameObject effects = GameObject.FindWithTag("SkyMasterEffects");
                            if (effects != null)
                            {
                                connector = effects.GetComponent<connectSuntoAtmosURP>();
                            }
                        }
                        catch
                        {}
                    }
                }                
            }
            //Debug.Log(Camera.main.GetComponent<connectSuntoSunShaftsURP>().sun.transform.position);
            if (connector != null)
            {
                this.blendCouds = connector.blendCouds;

                this.enableShafts = connector.enableShafts;
                this.sunTransform = connector.sun.transform.position;
                this.screenBlendMode = connector.screenBlendMode;
                //public Vector3 sunTransform = new Vector3(0f, 0f, 0f); 
                this.radialBlurIterations = connector.radialBlurIterations;
                this.sunColor = connector.sunColor;
                this.sunThreshold = connector.sunThreshold;
                this.sunShaftBlurRadius = connector.sunShaftBlurRadius;
                this.sunShaftIntensity = connector.sunShaftIntensity;
                this.maxRadius = connector.maxRadius;
                this.useDepthTexture = connector.useDepthTexture;

                //////////// ATMOS
                toggleDepthMode = connector.toggleDepthMode;
                kSunAngularRadius = connector.kSunAngularRadius;
                planet = connector.planet;
                Sun = connector.Sun;
                topBottomRadiusRatio = connector.topBottomRadiusRatio;
                seaLevelHeight  = connector.seaLevelHeight;
                UseConstantSolarSpectrum  = connector.UseConstantSolarSpectrum;
                UseOzone  = connector.UseOzone;
                UseCombinedTextures = connector.UseCombinedTextures;
                UseHalfPrecision = connector.UseHalfPrecision;
                DoWhiteBalance = connector.DoWhiteBalance;
                UseLuminance = connector.UseLuminance;
                Exposure = connector.Exposure;
                m_compute = connector.m_compute;
                m_material = connector.m_material;
                m_model = connector.m_model;
                m_camera = connector.m_camera;
                kBottomRadius = connector.kBottomRadius;
                kLengthUnitInMeters = connector.kLengthUnitInMeters;

                CAMERA_FOV = connector.CAMERA_FOV;
                CAMERA_ASPECT_RATIO = connector.CAMERA_ASPECT_RATIO;
                CAMERA_NEAR = connector.CAMERA_NEAR;
                CAMERA_FAR = connector.CAMERA_FAR;
                fovWHalf = connector.fovWHalf;
                c1 = connector.c1;
                c2 = connector.c2;

                AtmoBrightness = connector.AtmoBrightness;
                Fade = connector.Fade;
                FadeStart = connector.FadeStart;
                FadeEnd = connector.FadeEnd;
                Beta = connector.Beta;
                blendControls = connector.blendControls;
    }

            //if still null, disable effect
            bool connectorFound = true;
            if (connector == null)
            {
                connectorFound = false;
            }

            if (enableShafts && connectorFound && connector.enabled && renderingData.cameraData.camera == Camera.main)
            {
                CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

                RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
                opaqueDesc.depthBufferBits = 0;

                // Can't read and write to same color target, create a temp render target to blit. 
#if UNITY_2022_1_OR_NEWER
                // Can't read and write to same color target, create a temp render target to blit. 
                if (destination == renderingData.cameraData.renderer.cameraColorTargetHandle)//  UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget) //v0.1
                {
#else
                // Can't read and write to same color target, create a temp render target to blit. 
                if (destination == UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget) //v0.1
                {
#endif
                    //cmd.GetTemporaryRT(Shader.PropertyToID(m_TemporaryColorTexture.name), opaqueDesc, filterMode);
                    //Blit(cmd, source, m_TemporaryColorTexture.Identifier(), blitMaterial, 0);// blitShaderPassIndex);
                    //Blit(cmd, m_TemporaryColorTexture.Identifier(), source);

                    ////blitMaterial.SetFloat("_Delta",100);
                    //Blit(cmd, source, m_TemporaryColorTexture.Identifier(), blitMaterial, 0);// blitShaderPassIndex);
                    //Blit(cmd, m_TemporaryColorTexture.Identifier(), source);

                    //RenderShafts(context, renderingData, cmd, opaqueDesc);

                    Camera camera = Camera.main;
                    var formatA = camera.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default; //v3.4.9
                    RenderTexture lrDepthBufferATM = RenderTexture.GetTemporary(opaqueDesc.width, opaqueDesc.height, 0, formatA);
                    RenderTexture tmpBuffer = RenderTexture.GetTemporary(opaqueDesc.width, opaqueDesc.height, 0, formatA);
                    RenderTexture tmpDEPTH = RenderTexture.GetTemporary(opaqueDesc.width, opaqueDesc.height, 0, formatA);
                    //RenderTexture.active = tmpBuffer;
                    cmd.Blit(source, tmpBuffer);

                    //DEPTH
                    if (toggleDepthMode)
                    {
                        cmd.Blit(tmpBuffer, tmpDEPTH, m_material, 11);
                    }
                    else
                    {
                        cmd.Blit(tmpBuffer, tmpDEPTH, m_material, 6);
                    }
                    //cmd.Blit(tmpDEPTH, source);
                    m_material.SetTexture("_CameraDepthCustom", tmpDEPTH);

                    m_material.SetFloat("divideSky", blendCouds);

                    m_material.SetFloat("_Brightness", AtmoBrightness);
                    m_material.SetFloat("_Fade", Fade);
                    m_material.SetFloat("_FadeStart", FadeStart);
                    m_material.SetFloat("_FadeEnd", FadeEnd);
                    m_material.SetFloat("_Beta", Beta);
                    m_material.SetVector("_blendControls", blendControls);
                    //v0.1

                    //GL.ClearWithSkybox(false, camera);
                    renderATMOS(tmpBuffer,lrDepthBufferATM, cmd);
                    cmd.Blit(lrDepthBufferATM, source);                

                    context.ExecuteCommandBuffer(cmd);
                    CommandBufferPool.Release(cmd);
                    RenderTexture.ReleaseTemporary(lrDepthBufferATM);
                    RenderTexture.ReleaseTemporary(tmpBuffer);
                    RenderTexture.ReleaseTemporary(tmpDEPTH);
                }
                else
                {
                    //Blit(cmd, source, destination.Identifier(), blitMaterial, blitShaderPassIndex);
                }

                // RenderShafts(context, renderingData);
                //Camera camera = Camera.main;
                //cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
                //cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, blitMaterial);
                //cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);

                //context.ExecuteCommandBuffer(cmd);
                // CommandBufferPool.Release(cmd);
            }
        }

        /// <inheritdoc/>
        public override void FrameCleanup(CommandBuffer cmd)
        {
#if UNITY_2022_1_OR_NEWER
#else
           // if (destination == UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget)
           // {
               // cmd.ReleaseTemporaryRT(m_TemporaryColorTexture.id);

               // cmd.ReleaseTemporaryRT(lrColorB.id);
                //cmd.ReleaseTemporaryRT(lrDepthBuffer.id);
                //RenderTexture.ReleaseTemporary(lrColorBACK);
            //}
#endif
        }

        //RenderTexture lrColorBACK;
        //RenderTargetHandle lrColorBACK;



        ////////ATMOS

        void renderATMOS(RenderTexture src, RenderTexture dest, CommandBuffer cmd)
        {
            if (connector != null)
            {
                //connector.SetUp();
            }

            var p = GL.GetGPUProjectionMatrix(m_camera.projectionMatrix, false);
            p[2, 3] = p[3, 2] = 0.0f;
            p[3, 3] = 1.0f;
            var clipToWorld = Matrix4x4.Inverse(p * m_camera.worldToCameraMatrix) * Matrix4x4.TRS(new Vector3(0, 0, -p[2, 2]), Quaternion.identity, Vector3.one);

            m_material.SetMatrix("clip_to_world", clipToWorld);

            m_material.SetVector("earth_center", planet.transform.position);

            m_material.SetVector("sun_direction", ((Sun == null) ? Vector3.up : Sun.transform.forward) * -1.0f);

            Matrix4x4 frustumCorners = Matrix4x4.identity;

            Vector3 toRight = m_camera.transform.right * c1;
            Vector3 toTop = m_camera.transform.up * c2;

            Vector3 topLeft = (m_camera.transform.forward * CAMERA_NEAR - toRight + toTop);
            float CAMERA_SCALE = topLeft.magnitude * CAMERA_FAR / CAMERA_NEAR;

            topLeft.Normalize();
            topLeft *= CAMERA_SCALE;

            Vector3 topRight = (m_camera.transform.forward * CAMERA_NEAR + toRight + toTop);
            topRight.Normalize();
            topRight *= CAMERA_SCALE;

            Vector3 bottomRight = (m_camera.transform.forward * CAMERA_NEAR + toRight - toTop);
            bottomRight.Normalize();
            bottomRight *= CAMERA_SCALE;

            Vector3 bottomLeft = (m_camera.transform.forward * CAMERA_NEAR - toRight - toTop);
            bottomLeft.Normalize();
            bottomLeft *= CAMERA_SCALE;

            frustumCorners.SetRow(0, topLeft);
            frustumCorners.SetRow(1, topRight);
            frustumCorners.SetRow(2, bottomRight);
            frustumCorners.SetRow(3, bottomLeft);

            m_material.SetMatrix("frustumCorners", frustumCorners);

            //CustomGraphicsBlit(src, dest, m_material, 6);//

            m_material.SetTexture("_MainTex", src);
            cmd.Blit(src, dest, m_material, 7);
        }

        private void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material mat, int passNr)
        {
            RenderTexture.active = dest;

            mat.SetTexture("_MainTex", source);

            GL.PushMatrix();
            GL.LoadOrtho();

            mat.SetPass(passNr);

            GL.Begin(GL.QUADS);

            //This custom blit is needed as infomation about what corner verts relate to what frustum corners is needed
            //A index to the frustum corner is store in the z pos of vert

            GL.MultiTexCoord2(0, 0.0f, 0.0f);
            GL.Vertex3(0.0f, 0.0f, 3.0f); // BL //0, 0, 3

            GL.MultiTexCoord2(0, 1.0f, 0.0f);
            GL.Vertex3(1.0f, 0.0f, 2.0f); // BR

            GL.MultiTexCoord2(0, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f); // TR

            GL.MultiTexCoord2(0, 0.0f, 1.0f);
            GL.Vertex3(0.0f, 1.0f, 0.0f); // TL

            GL.End();
            GL.PopMatrix();
        }




        //SUN SHAFTS
        public void RenderShafts(ScriptableRenderContext context, UnityEngine.Rendering.Universal.RenderingData renderingData, CommandBuffer cmd, RenderTextureDescriptor opaqueDesc)
        {

            //CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            //RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            //var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/GrayscaleShafts"));

            ////           var sheetSHAFTS = context.propertySheets.Get(Shader.Find("Hidden/Custom/GrayscaleShafts"));
            Material sheetSHAFTS = blitMaterial;

            //sheet.properties.SetFloat("_Blend", settings.blend);
            sheetSHAFTS.SetFloat("_Blend", blend);

            //scontext.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);

            //if (CheckResources() == false)
            //{
            //    Graphics.Blit(source, destination);
            //    return;
            //}
            Camera camera = Camera.main;
            // we actually need to check this every frame
            if (useDepthTexture)
            {
                // GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
                camera.depthTextureMode |= DepthTextureMode.Depth;
            }
            //int divider = 4;
            //if (settings.resolution == SunShaftsHDRP.SunShaftsResolution.Normal)
            //    divider = 2;
            // else if (settings.resolution == SunShaftsHDRP.SunShaftsResolution.High)
            //    divider = 1;

            Vector3 v = Vector3.one * 0.5f;
           // Debug.Log(sunTransform);
            if (sunTransform != Vector3.zero) 
            {
                //v = camera.WorldToViewportPoint(sunTransform);
                //v = sunTransform;
                //v = camera.WorldToViewportPoint(-sunTransform);
                v = Camera.main.WorldToViewportPoint(sunTransform);// - Camera.main.transform.position;
            }
            else 
            {
                v = new Vector3(0.5f, 0.5f, 0.0f);
            }
            //Debug.Log("v="+v);


            //TextureDimension dim = renderingData.cameraData.cameraTargetDescriptor.dimension;


            //v0.1
            int rtW = opaqueDesc.width;///context.width; //source.width / divider;
            int rtH = opaqueDesc.height;// context.width; //source.height / divider;

            // Debug.Log(rtW + " ... " + rtH);

            var formatA = camera.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default; //v3.4.9
            RenderTexture m_TemporaryColorTexture = RenderTexture.GetTemporary(opaqueDesc.width, opaqueDesc.height, 0, formatA);
            RenderTexture lrDepthBuffer = RenderTexture.GetTemporary(opaqueDesc.width, opaqueDesc.height, 0, formatA);


            // lrColorB = RenderTexture.GetTemporary(rtW, rtH, 0);
            //        lrDepthBuffer = RenderTexture.GetTemporary(rtW, rtH, 0);
            cmd.GetTemporaryRT(Shader.PropertyToID(lrDepthBuffer.name), opaqueDesc, filterMode);

            //TEST1
            // Blit(cmd, source, lrDepthBuffer.Identifier(), blitMaterial,1);// blitShaderPassIndex);
            // Blit(cmd, lrDepthBuffer.Identifier(), source);
            // cmd.ReleaseTemporaryRT(lrDepthBuffer.id);
            // return;


            // mask out everything except the skybox
            // we have 2 methods, one of which requires depth buffer support, the other one is just comparing images

            //    sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(1.0f, 1.0f, 0.0f, 0.0f) * sunShaftBlurRadius);
            //    sunShaftsMaterial.SetVector("_SunPosition", new Vector4(v.x, v.y, v.z, maxRadius));
            //    sunShaftsMaterial.SetVector("_SunThreshold", sunThreshold);
            sheetSHAFTS.SetVector("_BlurRadius4", new Vector4(1.0f, 1.0f, 0.0f, 0.0f) * sunShaftBlurRadius);
           // sheetSHAFTS.SetVector("_SunPosition", new Vector4(v.x*0.5f+0.5f, v.y , v.z, maxRadius)); //new Vector4(v.x+0.25f, v.y, v.z, maxRadius));
            //Debug.Log(v.x);
            //Debug.Log(v.y);
            sheetSHAFTS.SetVector("_SunThreshold", sunThreshold);

            if (!useDepthTexture)
            {
                //var format= GetComponent<Camera>().hdr ? RenderTextureFormat.DefaultHDR: RenderTextureFormat.Default;
                var format = camera.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default; //v3.4.9
                RenderTexture tmpBuffer = RenderTexture.GetTemporary(rtW, rtH, 0, format);
                RenderTexture.active = tmpBuffer;
                GL.ClearWithSkybox(false, camera);

                //sunShaftsMaterial.SetTexture("_Skybox", tmpBuffer);
                sheetSHAFTS.SetTexture("_Skybox", tmpBuffer);
                //        Graphics.Blit(source, lrDepthBuffer, sunShaftsMaterial, 3);

                //             context.command.BlitFullscreenTriangle(source, lrDepthBuffer, sheetSHAFTS, 3);
                cmd.Blit( source, lrDepthBuffer, sheetSHAFTS, 3);

                RenderTexture.ReleaseTemporary(tmpBuffer);
            }
            else
            {
                //          Graphics.Blit(source, lrDepthBuffer, sunShaftsMaterial, 2);
                //              context.command.BlitFullscreenTriangle(source, lrDepthBuffer, sheetSHAFTS, 2);
                cmd.Blit( source, lrDepthBuffer, sheetSHAFTS, 2);
            }
            //  context.command.BlitFullscreenTriangle(lrDepthBuffer, context.destination, sheet, 5);
            // return;
            // paint a small black small border to get rid of clamping problems
            //      DrawBorder(lrDepthBuffer, simpleClearMaterial);

            // radial blur:

            //Blit(cmd, source, lrDepthBuffer.Identifier(), blitMaterial,1);// blitShaderPassIndex);
            //cmd.SetGlobalTexture("_ColorBuffer", lrDepthBuffer.Identifier());
            //Blit(cmd, source, lrDepthBuffer.Identifier(), blitMaterial, 5);   
            // Blit(cmd, source, lrColorB, blitMaterial, 5);
            // Blit(cmd, lrColorB, source);
            //cmd.ReleaseTemporaryRT(lrDepthBuffer.id);
            // return;

            
            //cmd.GetTemporaryRT(Shader.PropertyToID(m_TemporaryC



            //        lrColorBACK = RenderTexture.GetTemporary(rtW, rtH, 0);
            // cmd.GetTemporaryRT(lrColorBACK.id, opaqueDesc, FilterMode.Bilinear);
            cmd.Blit( source, m_TemporaryColorTexture); //KEEP BACKGROUND
            //Blit(cmd, source, lrColorBACK.Identifier());

            //settings.radialBlurIterations =  Mathf.Clamp((int)settings.radialBlurIterations, 1, 4);
            radialBlurIterations = Mathf.Clamp(radialBlurIterations, 1, 4);

            float ofs = sunShaftBlurRadius * (1.0f / 768.0f);

            //sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));
            //sunShaftsMaterial.SetVector("_SunPosition", new Vector4(v.x, v.y, v.z, maxRadius));
            sheetSHAFTS.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));

            float adjustX = 0.5f;
            if (v.x < 0.5f) 
            {
                //adjustX = -0.5f;
                float diff = 0.5f - v.x;
                adjustX = adjustX - 0.5f * diff;
            }
            float adjustY = 0.5f;
            if (v.y > 1.25f)
            {
                //adjustX = -0.5f;
                float diff2 = v.y - 1.25f;
                adjustY = adjustY - 0.3f * diff2;
            }
            if (v.y > 1.8f)
            {
                //adjustX = -0.5f;
                v.y = 1.8f;
                float diff3 = v.y - 1.25f;
                adjustY = 0.5f - 0.3f * diff3;
            }

            sheetSHAFTS.SetVector("_SunPosition", new Vector4(v.x * 0.5f + adjustX, v.y * 0.5f + adjustY, v.z, maxRadius));
            //Debug.Log(v.y);

            //TEST2
            //Blit(cmd, lrDepthBuffer.Identifier(), source);
            //cmd.GetTemporaryRT(lrColorB.id, opaqueDesc, filterMode);
            //RenderTexture lrColorBA = RenderTexture.GetTemporary(rtW, rtH, 0);
            // Blit(cmd, lrDepthBuffer.Identifier(), lrColorBA, sheetSHAFTS, 1);
            // Blit(cmd, lrColorBA, source);
            // Blit(cmd, lrDepthBuffer.Identifier(), source);
            // return;
            //RenderTexture.ReleaseTemporary(lrColorB);
            for (int it2 = 0; it2 < radialBlurIterations; it2++)
            {
                // each iteration takes 2 * 6 samples
                // we update _BlurRadius each time to cheaply get a very smooth look

                lrColorB = RenderTexture.GetTemporary(rtW, rtH, 0);
                cmd.Blit(lrDepthBuffer, lrColorB, sheetSHAFTS, 1);//Blit(cmd, lrDepthBuffer, lrColorB, sheetSHAFTS, 1); //Blit(cmd, lrDepthBuffer.Identifier(), lrColorB, sheetSHAFTS, 1);//v0.1
                cmd.ReleaseTemporaryRT(Shader.PropertyToID(lrDepthBuffer.name));//  lrDepthBuffer.id);//v0.1

                ofs = sunShaftBlurRadius * (((it2 * 2.0f + 1.0f) * 6.0f)) / 768.0f;
                sheetSHAFTS.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));
                cmd.GetTemporaryRT(Shader.PropertyToID(lrDepthBuffer.name), opaqueDesc, filterMode);   //    lrDepthBuffer.id, opaqueDesc, filterMode);   //v0.1 

                cmd.Blit(lrColorB, lrDepthBuffer, sheetSHAFTS, 1); //Blit(cmd, lrColorB, lrDepthBuffer.Identifier(), sheetSHAFTS, 1);//v0.1
                RenderTexture.ReleaseTemporary(lrColorB);  //v0.1

                ofs = sunShaftBlurRadius * (((it2 * 2.0f + 2.0f) * 6.0f)) / 768.0f;
                sheetSHAFTS.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));

                /*
               lrColorB = RenderTexture.GetTemporary(rtW, rtH, 0);               
               
 //               cmd.GetTemporaryRT(lrColorB.id, opaqueDesc, filterMode);
                // Graphics.Blit(lrDepthBuffer, lrColorB, sunShaftsMaterial, 1);

                //             context.command.BlitFullscreenTriangle(lrDepthBuffer, lrColorB, sheetSHAFTS, 1);
                Blit(cmd, lrDepthBuffer.Identifier(), lrColorB, sheetSHAFTS, 1);

 //              RenderTexture.ReleaseTemporary(lrDepthBuffer.Identifier());
                cmd.ReleaseTemporaryRT(lrDepthBuffer.id);
                ofs = sunShaftBlurRadius * (((it2 * 2.0f + 1.0f) * 6.0f)) / 768.0f;
                //sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));
                sheetSHAFTS.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));

 //               lrDepthBuffer = RenderTexture.GetTemporary(rtW, rtH, 0);
                cmd.GetTemporaryRT(lrDepthBuffer.id, opaqueDesc, filterMode);

                // Graphics.Blit(lrColorB, lrDepthBuffer, sunShaftsMaterial, 1);
                //              context.command.BlitFullscreenTriangle(lrColorB, lrDepthBuffer, sheetSHAFTS, 1);
                Blit(cmd, lrColorB, lrDepthBuffer.Identifier(), sheetSHAFTS, 1);

               RenderTexture.ReleaseTemporary(lrColorB);
  //              cmd.ReleaseTemporaryRT(lrColorB.id);
                ofs = sunShaftBlurRadius * (((it2 * 2.0f + 2.0f) * 6.0f)) / 768.0f;
                // sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));
                sheetSHAFTS.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));
                */
            }
            
            // put together:

            if (v.z >= 0.0f)
            {
                //sunShaftsMaterial.SetVector("_SunColor", new Vector4(sunColor.r, sunColor.g, sunColor.b, sunColor.a) * sunShaftIntensity);
                sheetSHAFTS.SetVector("_SunColor", new Vector4(sunColor.r, sunColor.g, sunColor.b, sunColor.a) * sunShaftIntensity);
            }
            else
            {
                // sunShaftsMaterial.SetVector("_SunColor", Vector4.zero); // no backprojection !
                sheetSHAFTS.SetVector("_SunColor", Vector4.zero); // no backprojection !
            }
            //sunShaftsMaterial.SetTexture("_ColorBuffer", lrDepthBuffer);
            //         sheetSHAFTS.SetTexture("_ColorBuffer", lrDepthBuffer.);
            cmd.SetGlobalTexture("_ColorBuffer", lrDepthBuffer);
            //    Graphics.Blit(context.source, context.destination, sunShaftsMaterial, (screenBlendMode == ShaftsScreenBlendMode.Screen) ? 0 : 4);


            //          context.command.BlitFullscreenTriangle(context.source, context.destination, sheetSHAFTS, (screenBlendMode == BlitSunShaftsSRP.BlitSettings.ShaftsScreenBlendMode.Screen) ? 0 : 4);
            //Blit(cmd, source, destination.Identifier(), sheetSHAFTS, (screenBlendMode == BlitSunShaftsSRP.BlitSettings.ShaftsScreenBlendMode.Screen) ? 0 : 4);
            // Blit(cmd, source, destination.Identifier(), sheetSHAFTS, (screenBlendMode == BlitSunShaftsSRP.BlitSettings.ShaftsScreenBlendMode.Screen) ? 0 : 4);

            // Blit(cmd, source, lrDepthBuffer.Identifier(), blitMaterial, 5);
            cmd.Blit( m_TemporaryColorTexture, source, sheetSHAFTS, (screenBlendMode == BlitAtmosSRP.BlitSettings.ShaftsScreenBlendMode.Screen) ? 0 : 4);
            //Blit(cmd, lrColorBACK.Identifier(), source, sheetSHAFTS, (screenBlendMode == BlitSunShaftsSRP.BlitSettings.ShaftsScreenBlendMode.Screen) ? 0 : 4);
//
           // cmd.ReleaseTemporaryRT(Shader.PropertyToID(lrDepthBuffer.name));//m_TemporaryColorTexture.id);            
           // cmd.ReleaseTemporaryRT(Shader.PropertyToID(m_TemporaryColorTexture.name));//lrDepthBuffer.id);
            RenderTexture.ReleaseTemporary(lrDepthBuffer);
            RenderTexture.ReleaseTemporary(m_TemporaryColorTexture);
            //cmd.ReleaseTemporaryRT(lrColorBACK.id);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            //cmd.ReleaseTemporaryRT(lrColorB.id);
            RenderTexture.ReleaseTemporary(lrColorB);
            //RenderTexture.ReleaseTemporary(lrColorBACK);
            //          RenderTexture.ReleaseTemporary(lrDepthBuffer);

        }



    }
}

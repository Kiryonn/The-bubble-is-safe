using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;

#if UNITY_2023_3_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
#endif

namespace Artngame.SKYMASTER
{
    public class ScreenSpaceSnowLITE_SMFeature : ScriptableRendererFeature
    {
        class WeatherEffectsSkyMasterPass : ScriptableRenderPass
        {
            public bool isForReflections = false;
            public int downSample = 1;

#if UNITY_2023_3_OR_NEWER
            //v0.1
            public class PassData
            {
                public RenderingData renderingData;
                public UniversalCameraData cameraData;
                public CullingResults cullResults;
                public TextureHandle colorTargetHandleA;
                public void Init(ContextContainer frameData, IUnsafeRenderGraphBuilder builder = null)
                {
                    cameraData = frameData.Get<UniversalCameraData>();
                    cullResults = frameData.Get<UniversalRenderingData>().cullResults;
                }
            }
            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                string passName = "SM SNOW LITE";
                using (var builder = renderGraph.AddUnsafePass<PassData>(passName,
                    out var data))
                {
                    builder.AllowPassCulling(false);
                    data.Init(frameData, builder);
                    builder.AllowGlobalStateModification(true);
                    UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
                    data.colorTargetHandleA = resourceData.activeColorTexture;
                    builder.UseTexture(data.colorTargetHandleA, AccessFlags.ReadWrite);

                    builder.SetRenderFunc<PassData>((data, ctx) =>
                    {
                        var cmd = CommandBufferHelpers.GetNativeCommandBuffer(ctx.cmd);
                        //var renderContext = ctx.GetRenderContext();//ctx.GetRenderContext();
                        OnCameraSetupA(cmd, data);
                        ExecutePass(cmd, data, ctx);
                    });
                }
            }
            //public static FieldInfo GraphRenderContext = typeof(InternalRenderGraphContext).GetField("renderContext", BindingFlags.NonPublic | BindingFlags.Instance);
            //public static FieldInfo IntRenderGraphContext = typeof(UnsafeGraphContext).GetField("wrappedContext", BindingFlags.NonPublic | BindingFlags.Instance);
            //public static ScriptableRenderContext GetRenderContextA( UnsafeGraphContext unsafeContext)
            //{
            //    return (ScriptableRenderContext)GraphRenderContext.GetValue(GetInternalRenderGraphContext(unsafeContext));
            //}
            //public static InternalRenderGraphContext GetInternalRenderGraphContext(UnsafeGraphContext unsafeContext)
            //{
            //    return (InternalRenderGraphContext)IntRenderGraphContext.GetValue(unsafeContext);
            //}
            //public override void Execute(ScriptableRenderContext context, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
            void ExecutePass(CommandBuffer command, PassData data, UnsafeGraphContext ctx)//, RasterGraphContext context)
            {
                //TEST
                CommandBuffer unsafeCmd = command;// CommandBufferHelpers.GetNativeCommandBuffer(ctx.cmd);

                //v1.1.0
                if (Camera.main == null || (data.cameraData.camera != Camera.main && !isForReflections))
                {
                    return;
                }

                //connector = data.cameraData.camera.GetComponent<connectSuntoFullVolumeNebulaURP>();
                //currentCamera = data.cameraData.camera;

                //if (connector == null)
                //{
                //    connector = data.cameraData.camera.gameObject.GetComponent<connectSuntoFullVolumeNebulaURP>();

                //    if (connector == null && Camera.main != null)
                //    {
                //        connector = Camera.main.GetComponent<connectSuntoFullVolumeNebulaURP>(); //v0.1
                //    }
                //}

                ////URP
                //if (_needsReset) ResetResources();

                //prevDownscaleFactor = downScaleFactor;//v0.1 - check what scale was before get from connector
                //prevlowerRefrReflResFactor = lowerRefrReflResFactor;

                ////Debug.Log(Camera.main.GetComponent<connectSuntoVolumeFogURP>().sun.transform.position);
                //if (inheritFromController && connector != null)
                //{
                //    setNebulaVariables(connector);
                //}

                ////v0.1 - after get connector, check if resolution matches, otherwise reset
                //if (prevDownscaleFactor != downScaleFactor || lowerRefrReflResFactor != prevlowerRefrReflResFactor) ResetResources();

                ////if still null, disable effect
                //bool connectorFound = true;
                //if (connector == null)
                //{
                //    connectorFound = false;
                //}

                if (1==1)//enableFog && connectorFound)
                {
                    CommandBuffer cmd = unsafeCmd;// CommandBufferPool.Get(m_ProfilerTag);
                    RenderTextureDescriptor opaqueDesc = data.cameraData.cameraTargetDescriptor;
                    opaqueDesc.depthBufferBits = 0;




                    // CommandBuffer cmd = CommandBufferPool.Get("Outline Pass");
                    //RenderTextureDescriptor opaqueDescriptor = renderingData.cameraData.cameraTargetDescriptor;
                    //opaqueDescriptor.depthBufferBits = 0;

                    //v1.6
                    if (Camera.main != null// && renderingData.cameraData.camera == Camera.main
                        && Camera.main.GetComponent<ScreenSpaceSnowLITE_SM_URP>() != null)
                    {

                        outlineMaterial.SetMatrix("_CamToWorld", Camera.main.cameraToWorldMatrix);
                        //cmd.Blit(source, destination, outlineMaterial, 0);

                        cmd.Blit(source, source, outlineMaterial, 0);
                        //Debug.Log("aa");
                        //context.ExecuteCommandBuffer(cmd);
                        //CommandBufferPool.Release(cmd);
                    }



                    //if (!fastest && downScale)
                    //{
                    //    if (cloudChoice == 0)
                    //    {
                    //        data.cameraData.camera.backgroundColor = new Color(0, 0, 0, 0);
                    //        data.cameraData.camera.clearFlags = CameraClearFlags.SolidColor;
                    //    }
                    //}
                    //else
                    //{
                    //    data.cameraData.camera.clearFlags = CameraClearFlags.Skybox;
                    //}

                    ////if (destination == data.cameraData.renderer.cameraColorTargetHandle)//UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget) //v0.1
                    //{
                    //    //cmd.GetTemporaryRT(Shader.PropertyToID(m_TemporaryColorTexture.name), opaqueDesc, filterMode);//v0.1
                    //    if (cloudChoice == 0)
                    //    {
                    //        //RenderFog(context, data, cmd, opaqueDesc);
                    //    }
                    //    else if (cloudChoice == 1)
                    //    {
                    //        RenderFullVolumetricClouds(currentCamera, cmd, opaqueDesc);
                    //    }
                    //}
                    //CommandBufferPool.Release(cmd);
                }






               





            }
            public void OnCameraSetupA(CommandBuffer cmd, PassData renderingData)//(CommandBuffer cmd, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
            {
                //v0.5a
                //if (m_CameraColorTarget != null)
                {
                    //ConfigureTarget(m_CameraColorTarget);
                }
                //https://docs.unity3d.com/Packages/com.unity.render-pipelines.core@12.0/manual/rthandle-system-using.html
                //RTHandle rtHandleUsingFunctor = RTHandles.Alloc(ComputeRTHandleSize, colorFormat: GraphicsFormat.R32_SFloat, dimension: TextureDimension.Tex2D);
                RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
                int rtW = opaqueDesc.width;
                int rtH = opaqueDesc.height;
                int xres = (int)(rtW / ((float)downSample));
                int yres = (int)(rtH / ((float)downSample));
                if (_handleA == null || _handleA.rt.width != xres || _handleA.rt.height != yres)
                {
                    //Debug.Log("Alloc");
                    _handleA = RTHandles.Alloc(xres, yres, colorFormat: GraphicsFormat.R32G32B32A32_SFloat, dimension: TextureDimension.Tex2D);
                }

                var renderer = renderingData.cameraData.renderer;
                //v0.1
                //_handle.Init(settings.textureId);

                //_handle = RTHandles.Alloc(settings.textureId, name: settings.textureId);
                destination = renderingData.colorTargetHandleA;// (settings.destination == BlitFullVolumeNebulaSRP.Target.Color)
                                                               //? renderer.cameraColorTargetHandle //UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget //v0.1
                                                               //: _handle;
                //v0.1
                //source = renderer.cameraColorTarget;
                source = renderingData.colorTargetHandleA;// renderer.cameraColorTargetHandle;
            }
            //END GRAPH
#endif






            RTHandle _handleA;
            RTHandle _handle; //v0.1







            private RenderTargetIdentifier source { get; set; }

#if UNITY_2022_1_OR_NEWER
            private RTHandle destination { get; set; } //v0.1
#else
            private RenderTargetHandle destination { get; set; } //v0.1
#endif

            public Material outlineMaterial = null;

#if UNITY_2022_1_OR_NEWER
            public void Setup(RenderTargetIdentifier source, RTHandle destination)//v0.1
            {
                this.source = source;
                this.destination = destination;
                //temporaryColorTexture = RTHandles.Alloc("temporaryColorTexture", name: "temporaryColorTexture"); //v0.1
            }
#else
            RenderTargetHandle temporaryColorTexture; //v0.1
            public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination)//v0.1
            {
                this.source = source;
                this.destination = destination;
            }
#endif

            public WeatherEffectsSkyMasterPass(Material outlineMaterial)
            {
                this.outlineMaterial = outlineMaterial;
            }

            //v1.5
#if UNITY_2020_2_OR_NEWER
            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
                // get a copy of the current camera’s RenderTextureDescriptor
                // this descriptor contains all the information you need to create a new texture
                //RenderTextureDescriptor cameraTextureDescriptor = renderingData.cameraData.cameraTargetDescriptor;

                // _handle = RTHandles.Alloc(settings.textureId, name: settings.textureId); //v0.1

                var renderer = renderingData.cameraData.renderer;
#if UNITY_2022_1_OR_NEWER
                destination = renderingData.cameraData.renderer.cameraColorTargetHandle; //UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget //v0.1                          
                source = renderingData.cameraData.renderer.cameraColorTargetHandle; 
#else
                destination = UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget; //v0.1                          
                source = renderingData.cameraData.renderer.cameraColorTarget;
#endif

            }
#endif

            // This method is called before executing the render pass.
            // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
            // When empty this render pass will render to the active camera render target.
            // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
            // The render pipeline will ensure target setup and clearing happens in an performance manner.
            //public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            //{

            // }

            // Here you can implement the rendering logic.
            // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
            // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
            // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer cmd = CommandBufferPool.Get("Outline Pass");

                RenderTextureDescriptor opaqueDescriptor = renderingData.cameraData.cameraTargetDescriptor;
                opaqueDescriptor.depthBufferBits = 0;

                //v1.6
                if (Camera.main != null && renderingData.cameraData.camera == Camera.main
                    && Camera.main.GetComponent<ScreenSpaceSnowLITE_SM_URP>() != null)
                {
                    //if (destination == renderingData.cameraData.renderer.cameraColorTargetHandle)//RenderTargetHandle.CameraTarget) //v0.1
                    //{

                    //temporaryColorTexture = RTHandles.Alloc("temporaryColorTexture", name: "temporaryColorTexture"); //v0.1

                    //cmd.GetTemporaryRT(Shader.PropertyToID(temporaryColorTexture.name), opaqueDescriptor, FilterMode.Point); //v0.1
                    //cmd.Blit( source, temporaryColorTexture, outlineMaterial, 0); //v0.1
                    //cmd.Blit( temporaryColorTexture, destination); //v0.1
                   
                    outlineMaterial.SetMatrix("_CamToWorld", Camera.main.cameraToWorldMatrix);

#if UNITY_2022_1_OR_NEWER
                    cmd.Blit(source, destination, outlineMaterial, 0); //v0.1
#else
                    cmd.GetTemporaryRT(temporaryColorTexture.id, opaqueDescriptor, FilterMode.Bilinear);// FilterMode.Point);
                    ////UnityEngine.RenderTexture.active = source;
                    ////GL.ClearWithSkybox(true, Camera.main);
                    Blit(cmd, source, temporaryColorTexture.Identifier(), outlineMaterial, 0);
                    Blit(cmd, temporaryColorTexture.Identifier(), source);

                    //Blit(cmd, source, source, outlineMaterial, 0);
#endif

                    //}
                    //else cmd.Blit( source, destination, outlineMaterial, 0); //v0.1

                    //cmd.ReleaseTemporaryRT(Shader.PropertyToID(temporaryColorTexture.name));//v0.1
                    context.ExecuteCommandBuffer(cmd);
                    CommandBufferPool.Release(cmd);
                }


            }

            /// Cleanup any allocated resources that were created during the execution of this render pass.
            public override void FrameCleanup(CommandBuffer cmd)
            {
#if UNITY_2022_1_OR_NEWER
                
#else
                if (destination == RenderTargetHandle.CameraTarget)
                { //v0.1
                    cmd.ReleaseTemporaryRT(temporaryColorTexture.id);
                }
#endif
            }
        }

        [System.Serializable]
        public class OutlineSettings
        {
            public Material outlineMaterial = null;
        }

        public OutlineSettings settings = new OutlineSettings();
        WeatherEffectsSkyMasterPass weatherEffectsSkyMaster;

#if UNITY_2022_1_OR_NEWER
        RTHandle outlineTexture; //v0.1
#else
        RenderTargetHandle outlineTexture; //v0.1
#endif

        [SerializeField] private RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;//v1.6a RenderPassEvent.BeforeRenderingPostProcessing;// RenderPassEvent.AfterRenderingOpaques;//v1.6a
        public override void Create()
        {
            weatherEffectsSkyMaster = new WeatherEffectsSkyMasterPass(settings.outlineMaterial);
            weatherEffectsSkyMaster.renderPassEvent = renderPassEvent; //RenderPassEvent.AfterRenderingTransparents;//v1.6a

            //
#if UNITY_2022_1_OR_NEWER
            outlineTexture = RTHandles.Alloc("_OutlineTexture", name: "_OutlineTexture"); //v0.1
#else
            outlineTexture.Init("_OutlineTexture"); //v0.1
#endif

        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (settings.outlineMaterial == null)
            {
                Debug.LogWarningFormat("Missing Outline Material");
                return;
            }
            //outlinePass.Setup(renderer.cameraColorTarget, RenderTargetHandle.CameraTarget);//v1.5
            renderer.EnqueuePass(weatherEffectsSkyMaster);
        }
    }


}

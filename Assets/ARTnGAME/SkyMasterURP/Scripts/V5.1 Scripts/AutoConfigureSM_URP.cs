using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Artngame.SKYMASTER
{
    [ExecuteAlways]
    public class AutoConfigureSM_URP : MonoBehaviour
    {
        //v24d - Fly Through
        public bool configFlyThrough = false;

        //v24d - Fog of war
        public bool configFogOfWar = false;
        public Transform depthRenderCamera;

        //v24d - Planets
        public bool configPlanet = false;
        public Vector3 planetOverlookPos = new Vector3(-28000,173145, 121971);
        public Vector3 planetOverlookRot = new Vector3(25, 164, 0);
        public float cameraFarDist = 140000;
        public setPlanetCloudsAtmosphere planetScript;
        public Transform getPlanetScaleFrom;//if set, assign this scale, if not the below
        public float planetRadius = 598000;

        //v24d - Atmosphere sun shafts
        public bool configAtmosphere = false;
        public bool subtleAtmosphere = false;

        //v24d - Biomes
        public bool configBiomes = false;
        public biomesSkyMaster biomeController;

        //v24d
        public void setupFogOfWar(connectSuntoFullVolumeCloudsURP cloudsScriptA)
        {
            //connectSuntoFullVolumeCloudsURP cloudsScriptA = Camera.main.gameObject.GetComponent<connectSuntoFullVolumeCloudsURP>();
            if (cloudsScriptA.maskHistoryTexture == null)
            {
                cloudsScriptA.maskHistoryTexture = (RenderTexture)Resources.Load("FOG of WAR MASK");
                cloudsScriptA.maskScale = 25;
                cloudsScriptA.fogOfWarRadius = 0.65f;
                cloudsScriptA.fogOfWarPower = 140;
                cloudsScriptA.playerPos = Camera.main.transform;
                cloudsScriptA.extendFarPlaneAboveClouds = 0.16f;
                cloudsScriptA.cameraScale = 1;
                cloudsScriptA.steps = 451;
                cloudsScriptA.startHeight = 130;
                cloudsScriptA.thickness = 900;
                cloudsScriptA.planetSize = 10000000;
                cloudsScriptA.scale = 0.85f;
                cloudsScriptA.noiseCuttoff = 0.01f;
                cloudsScriptA.coverage = 2.5f;
                cloudsScriptA.weatherScale = 0.427f;
                cloudsScriptA.lowFreqMin = 1;
                cloudsScriptA.lowFreqMax = 0.345f;
                cloudsScriptA.highFreqModifier = 0;
                cloudsScriptA.maskMat = (Material)Resources.Load("FogOfWarMaskMaterial");
                cloudsScriptA.topDownMaskCamera = depthRenderCamera.GetComponent<Camera>();
            }
        }
        public void setupFlyThrough(connectSuntoFullVolumeCloudsURP cloudsScriptA)
        {
                cloudsScriptA.allowFlyingInClouds = true;
                cloudsScriptA.extendFarPlaneAboveClouds = 0.4f;
                cloudsScriptA.cameraScale = 1;
                cloudsScriptA.steps = 451;
                cloudsScriptA.autoAdjustTranspOrder = true;
                cloudsScriptA.heightBetweenCloudAndWater = 250;
        }


        public SkyMasterManager SkyManager;
        // Start is called before the first frame update
        void Start()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) //v5.4.2
            {
                PrefabUtility.UnpackPrefabInstance(this.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }
#endif
        }
        public bool configure = false;
        public bool enableWater = true;
        public bool configured = false;

        //v5.3.0
        public bool addFullVolumeClouds = false;
        public bool addSmoothSun = false;
        public controlWeatherTOD_SM_URP sunController;

        public int reflectRendererID = 1;
        public int refractRendererID = 1;
        public int topDownDepthRendererID = 2;

        public WeatherScript weatherTexture;

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            int stage = GameObjectTypeLoggingSM.postStageInformation(this.gameObject);
            if (stage == 4)
            {
                Debug.Log("System is in prefab edit mode");//Debug.Log("Grass is in prefab edit mode"); //v5.3.0
                return;
            }
#endif
            //configure and close

            if (configure && !configured && SkyManager != null)
            {
#if UNITY_EDITOR
                LayerMaskCreateSM.CreateLayer("Background");
                LayerMaskCreateSM.CreateLayer("Conductor");
#endif
                if (Camera.main == null)
                {
                    //add main camera
                    GameObject cameraMain = new GameObject();
                    cameraMain.tag = "MainCamera";
                    cameraMain.AddComponent<Camera>();
                    //v5.4.2
                    cameraMain.name = "Main Camera";
                }

                if (Camera.main != null)
                {
                    //v4.8.4
                    Camera.main.farClipPlane = 30000;
                    Camera.main.transform.position = Camera.main.transform.position + new Vector3(0, 10, 0);
                    SkyManager.Current_Time = 10.3f;

                    if (!addFullVolumeClouds)
                    {
                        //ADD CLOUDS
                        connectSuntoFullVolumeCloudsURP cloudsScript = Camera.main.gameObject.GetComponent<connectSuntoFullVolumeCloudsURP>();
                        if (cloudsScript != null)
                        {
                            cloudsScript.sun = SkyManager.SunObj.transform;
                        }
                        else
                        {
                            cloudsScript = Camera.main.gameObject.AddComponent<connectSuntoFullVolumeCloudsURP>();
                            cloudsScript.sun = SkyManager.SunObj.transform;
                        }

                        //v5.1.5c - EXTRA CONFIG
                        cloudsScript._SampleCount0 = 11;

                        //ADD REFLECTIONS if water active
                        connectSuntoFullVolumeCloudsURP CloudsScript = cloudsScript;// SkyManager.volumeClouds;
                        int layerToCheck = LayerMask.NameToLayer("Background");

                        if (CloudsScript != null)
                        {
                            if (SkyManager.water != null)
                            { 

                            }
                            else
                            {
                                Debug.Log("No Water in scene, please add water component first in Water section");
                            }

                            //ADD SHADOWS

                            //v4.8.5
                            //var layerToCheck = LayerMask.NameToLayer("Background");
                            //if (layerToCheck > -1)
                            //{
                            //    if (CloudsScript != null && CloudsScript.backgroundCam == null)
                            //    {
                            //        CloudsScript.setupDepth = true;
                            //        CloudsScript.createDepthSetup();
                            //        CloudsScript.setupDepth = true;
                            //        CloudsScript.blendBackground = true;

                            //        CloudsScript.reflectClouds.backgroundCam = CloudsScript.backgroundCam;//v5.0.5
                            //    }
                            //    else
                            //    {
                            //        Debug.Log("Depth camera already setup");
                            //    }
                            //}
                            //else
                            //{
                            //    Debug.Log("Please add the Background layer to proceed with the setup");
                            //}

                            //LIGHTNING
                            if (cloudsScript != null && cloudsScript.gameObject.GetComponent<lightningCameraVolumeCloudsSM_URP>() == null)
                            {
                                lightningCameraVolumeCloudsSM_URP lightning = cloudsScript.gameObject.AddComponent<lightningCameraVolumeCloudsSM_URP>();
                                lightning.setupLightning = true;
                                lightning.createLightningBox();
                                //script.CloudsScript.shadowsUpdate ();
                                lightning.EnableLightning = true;
                                lightning.lightning_every = 5;
                                lightning.max_lightning_time = 9;
                            }
                            else
                            {
                                Debug.Log("Lightning components already setup");
                            }

                        }
                        else
                        {
                            Debug.Log("No Clouds");
                        }
                    }
                    else
                    {
                        //////////////////////////////////////// FULL VOLUMETRIC CLOUDS ////////////////////////////////////////
                        //ADD CLOUDS
                        connectSuntoFullVolumeCloudsURP cloudsScript = Camera.main.gameObject.GetComponent<connectSuntoFullVolumeCloudsURP>();
                        if (cloudsScript != null)
                        {
                            cloudsScript.sunLight = SkyManager.SUN_LIGHT.GetComponent<Light>();
                        }
                        else
                        {
                            cloudsScript = Camera.main.gameObject.AddComponent<connectSuntoFullVolumeCloudsURP>();
                            cloudsScript.sunLight = SkyManager.SUN_LIGHT.GetComponent<Light>();
                        }

                        //ADD FOG
                        connectSuntoVolumeFogURP fogScript = Camera.main.gameObject.GetComponent<connectSuntoVolumeFogURP>();
                        if (fogScript != null)
                        {
                            fogScript.sun = SkyManager.SUN_LIGHT.transform;
                            fogScript.lightControlA = new Vector4(0.5f,100,1,1);
                        }
                        else
                        {
                            fogScript = Camera.main.gameObject.AddComponent<connectSuntoVolumeFogURP>();
                            fogScript.sun = SkyManager.SUN_LIGHT.transform;
                            fogScript.lightControlA = new Vector4(0.5f, 100, 1, 1);
                        }

                        //LIGHTNING
                        if (cloudsScript != null && cloudsScript.gameObject.GetComponent<lightningCameraVolumeCloudsSM_URP>() == null)
                        {
                            lightningCameraVolumeCloudsSM_URP lightning = cloudsScript.gameObject.AddComponent<lightningCameraVolumeCloudsSM_URP>();
                            lightning.setupLightning = true;
                            lightning.createLightningBox();
                            lightning.EnableLightning = true;
                            lightning.lightning_every = 5;
                            lightning.max_lightning_time = 9;

                            lightning.fullvolumeCloudsScript = cloudsScript;
                            lightning.SkyManager = SkyManager;
                        }
                        else
                        {
                            Debug.Log("Lightning components already setup");
                        }

                        //WATER REFLECTIONS
                        //if (SkyManager.water != null)
                        //{ 
                        //    if (Camera.main.GetComponent<PlanarReflectionsSM_LWRP>().outReflectionCamera != null)
                        //    {
                        //        //WATER enable
                        //        if (enableWater)
                        //        {
                        //            SkyManager.water.transform.parent.gameObject.SetActive(true);                                   
                        //        }

                        //        if (Camera.main.GetComponent<PlanarReflectionsSM_LWRP>().outReflectionCamera != null) //v4.8.5
                        //        {
                        //            PlanarReflectionsSM_LWRP reflClouds = Camera.main.gameObject.GetComponent<PlanarReflectionsSM_LWRP>();
                        //        }
                        //        else
                        //        {
                        //            PlanarReflectionsSM_LWRP reflector = Camera.main.gameObject.AddComponent<PlanarReflectionsSM_LWRP>();
                        //        }
                        //    }
                        //    else
                        //    {
                        //        //v4.8
                        //        PlanarReflectionsSM_LWRP reflClouds = Camera.main.GetComponent<PlanarReflectionsSM_LWRP>();
                        //    }
                        //}
                        //else
                        //{
                        //    Debug.Log("No Water in scene, please add water component first in Water section");
                        //}

                        //REFLECT - REFRACT
                        if(Camera.main.GetComponent<PlanarReflectionsSM_LWRP>() != null)
                        {
                            PlanarReflectionsSM_LWRP reflections = Camera.main.gameObject.GetComponent<PlanarReflectionsSM_LWRP>();
                            reflections.rendererID = reflectRendererID;
                        }
                        else
                        {
                            PlanarReflectionsSM_LWRP reflections = Camera.main.gameObject.AddComponent<PlanarReflectionsSM_LWRP>();
                            reflections.rendererID = reflectRendererID;
                        }

                        if (Camera.main.GetComponent<PlanarRefractionsSM_LWRP>() != null)
                        {
                            PlanarRefractionsSM_LWRP refractions = Camera.main.gameObject.GetComponent<PlanarRefractionsSM_LWRP>();
                            refractions.rendererID = refractRendererID;
                        }
                        else
                        {
                            PlanarRefractionsSM_LWRP refractions = Camera.main.gameObject.AddComponent<PlanarRefractionsSM_LWRP>();
                            refractions.rendererID = refractRendererID;
                        }

                        if (Camera.main.GetComponent<DepthRendererSM_LWRP>() != null)
                        {
                            DepthRendererSM_LWRP depthR = Camera.main.gameObject.GetComponent<DepthRendererSM_LWRP>();
                            depthR.rendererID = topDownDepthRendererID;
                        }
                        else
                        {
                            DepthRendererSM_LWRP depthR = Camera.main.gameObject.AddComponent<DepthRendererSM_LWRP>();
                            depthR.rendererID = topDownDepthRendererID;
                        }


                        if (sunController != null && addSmoothSun)
                        {
                            //SkyManager.SunObj = sunController.temporalSunMoon;

                            sunController.fullVolumeClouds = cloudsScript;
                            sunController.lightningController = cloudsScript.gameObject.GetComponent<lightningCameraVolumeCloudsSM_URP>();
                            sunController.enabled = true;

                            cloudsScript.sun = sunController.temporalSunMoon.transform;
                        }

                    }//END v5.3.0

                    //VOLUME FOG
                    SeasonalTerrainSKYMASTER TerrainManager = SkyManager.gameObject.GetComponent<SeasonalTerrainSKYMASTER>();

                    //v4.8
                    //TerrainManager.setVFogCurvesPresetE();

                    //ADD FOG
                    if (sunController != null && addSmoothSun)
                    {
                        sunController.fullVolumeClouds = Camera.main.gameObject.GetComponent<connectSuntoFullVolumeCloudsURP>();
                        sunController.etherealVolFog = Camera.main.gameObject.GetComponent<connectSuntoVolumeFogURP>();
                        sunController.reflection = Camera.main.gameObject.GetComponent<PlanarReflectionsSM_LWRP>();
                        sunController.refraction = Camera.main.gameObject.GetComponent<PlanarRefractionsSM_LWRP>();
                        sunController.depthRenderer = Camera.main.gameObject.GetComponent<DepthRendererSM_LWRP>();
                        sunController.lightningController = Camera.main.gameObject.GetComponent<lightningCameraVolumeCloudsSM_URP>();
                    }

                    //SHAFTS

                    //END SHAFTS
                }
                else //find if camera in scene
                {

                }

                //v5.4.2
                connectSuntoFullVolumeCloudsURP cloudsScriptA = Camera.main.gameObject.GetComponent<connectSuntoFullVolumeCloudsURP>();
                if (cloudsScriptA.WeatherTexture == null)
                {
                    cloudsScriptA.WeatherTexture = (Texture2D)Resources.Load("weather3");
                    cloudsScriptA.blueNoiseTexture = (Texture2D)Resources.Load("blueNoise");
                    cloudsScriptA.curlNoise = (Texture2D)Resources.Load("curlNoise");
                    cloudsScriptA.lowFreqNoise = (TextAsset)Resources.Load("noiseShapePacked");
                    cloudsScriptA.highFreqNoise = (TextAsset)Resources.Load("noiseErosionPacked");
                    cloudsScriptA.cloudsHighTexture = (Texture2D)Resources.Load("wispyClouds1");

                    //VelocityBuffer cloudsScriptA1 = Camera.main.gameObject.GetComponent<VelocityBuffer>();
                    //cloudsScriptA1.velocityShader = (Shader)Resources.Load("VelocityBuffer");

                    //GlobalFogSkyMaster foggA = Camera.main.gameObject.GetComponent<GlobalFogSkyMaster>();
                    //foggA.fogShader = (Shader)Resources.Load("GlobalFogSkyMaster");

                    //SunShaftsSkyMaster sunShaftsA = Camera.main.gameObject.GetComponent<SunShaftsSkyMaster>();
                    //sunShaftsA.sunShaftsShader = (Shader)Resources.Load("SunShaftsCompositeSM");
                    //sunShaftsA.simpleClearShader = (Shader)Resources.Load("SimpleClearSM");
                }


                //v24d
                if (configFlyThrough)
                {
                    setupFlyThrough(cloudsScriptA);
                    connectSuntoVolumeFogURP etherealVolFog = Camera.main.gameObject.GetComponent<connectSuntoVolumeFogURP>();
                    if(etherealVolFog != null) {
                        etherealVolFog.GlobalFogNoisePower = 0.5f;
                        etherealVolFog.GlobalFogPower = 0.5f;
                        etherealVolFog.enableComposite = true;
                    }
                }
                if (configFogOfWar)
                {
                    setupFogOfWar(cloudsScriptA);
                    cloudsScriptA.sunLightFactor = 1.2f;
                    connectSuntoVolumeFogURP fog = Camera.main.gameObject.GetComponent<connectSuntoVolumeFogURP>();
                    if (fog != null)
                    {
                        fog.enabled = false;
                    }
                    if (Camera.main != null)
                    {
                        Camera.main.transform.position = new Vector3(0,4000,0);
                        Camera.main.transform.eulerAngles = new Vector3(35, 0, 0);
                    }
                }
                if (configAtmosphere)
                {

                    connectSuntoVolumeFogURP etherealVolFog = Camera.main.gameObject.GetComponent<connectSuntoVolumeFogURP>();
                    if (etherealVolFog != null)
                    {
                        etherealVolFog.GlobalFogNoisePower = 0.1f;
                        etherealVolFog.GlobalFogPower = 0.1f;
                        //etherealVolFog.enableComposite = true;
                        etherealVolFog.LightRaySamples = 8;
                    }

                    cloudsScriptA.cloudDistanceParams = new Vector3(10000,0,0);
                    //cloudsScriptA.
                    if (Camera.main != null)
                    {
                        Camera.main.transform.position = new Vector3(0, 220, 0);
                        Camera.main.farClipPlane = 120000;
                    }

                    if (subtleAtmosphere)
                    {
                        cloudsScriptA.lightStepLength = 25;
                        cloudsScriptA.sunLightFactor = 3;
                        cloudsScriptA.shadowPower = 3;
                        cloudsScriptA.globalMultiplier = 150;
                        //cloudsScriptA.sunLightFactor = 1.2f;
                        cloudsScriptA.scatterOn = 1;
                        cloudsScriptA.sunRaysOn = 6;
                        cloudsScriptA.sunRaysPower = 0.2f;
                        cloudsScriptA.raysResolution = new Vector4(0.6f, 510, 0.3f, 2);
                        cloudsScriptA.rayShadowing = new Vector4(1, 2.48f, 5.41f, 1.64f);
                        cloudsScriptA.enableShadowCastOnClouds = false;
                        //cloudsScriptA.shadowPower = 0.8f;
                        cloudsScriptA.steps = 891;
                        cloudsScriptA.downSample = 3;
                        cloudsScriptA.startHeight = 5900;
                        cloudsScriptA.thickness = 5500;
                        cloudsScriptA.planetSize = 10000000;
                        cloudsScriptA.scale = 0.15f;
                        cloudsScriptA.noiseCuttoff = 0.25f;
                        cloudsScriptA.lowFreqMin = 0.554f;
                        cloudsScriptA.lowFreqMax = 0.767f;
                        cloudsScriptA.highFreqModifier = 0.298f;
                        cloudsScriptA.coverage = 1.26f;
                        cloudsScriptA.cloudSampleMultiplier = 1.46f;
                    }
                    else
                    {
                        cloudsScriptA.weatherScale = 0.4f;// 0.056f;
                        cloudsScriptA.occlusionDrop = 0.04f;
                        cloudsScriptA.luminance = 0.05f;
                        cloudsScriptA.lumFac = 9;
                        cloudsScriptA.ScatterFac = 5000000;
                        cloudsScriptA.TurbFac = 11;
                        cloudsScriptA.mieCoefficient = 5.28f;
                        cloudsScriptA.mieDirectionalG = 0.68f;
                        cloudsScriptA.bias = 2.1f;
                        cloudsScriptA.contrast = 18.95f;

                        //BASIC
                        cloudsScriptA._Extinct = 0.021f;
                        SkyManager.Current_Time = 15.5f;
                        SkyManager.Horizontal_factor = -74;

                        cloudsScriptA.windSpeed = 1000;

                        cloudsScriptA.autoRegulateEdgeMode = false;
                        cloudsScriptA.controlBackAlphaPower = 0.31f;
                        cloudsScriptA.controlCloudAlphaPower = 0.01f;
                        cloudsScriptA.controlCloudEdgeA.y = 1.075f;

                        cloudsScriptA.extendFarPlaneAboveClouds = 0.71f;

                        cloudsScriptA.scatterOn = 1;
                        cloudsScriptA.sunRaysOn = 6;
                        cloudsScriptA.sunRaysPower = 0.4f;

                        cloudsScriptA.raysResolution = new Vector4(0.21f, 200, 0.1f, 2);
                        cloudsScriptA.rayShadowing = new Vector4(1, 2.48f, 5.41f, 1.84f);

                        cloudsScriptA.enableShadowCastOnClouds = true;
                        cloudsScriptA.shadowPower = -1;                          
                        
                        cloudsScriptA.steps = 843;
                        cloudsScriptA.downSample = 2;
                        cloudsScriptA.startHeight = 4500;
                        cloudsScriptA.thickness = 5500;
                        cloudsScriptA.planetSize = 2300000;
                        cloudsScriptA.scale = 0.1f;
                        cloudsScriptA.noiseCuttoff = 0.4f;
                        cloudsScriptA.lowFreqMin = 0.604f;
                        cloudsScriptA.lowFreqMax = 0.775f;
                        cloudsScriptA.highFreqModifier = 0.309f;
                        cloudsScriptA.coverage = 1.6f;

                        cloudsScriptA.lightStepLength = 35;
                        cloudsScriptA.sunLightFactor = 3;
                        cloudsScriptA.globalMultiplier = 2;

                        cloudsScriptA.cloudSampleMultiplier = 1.46f;
                    }
                }
                if (configPlanet)
                {
                    if(Camera.main != null)
                    {
                        Camera.main.transform.position = planetOverlookPos;
                        Camera.main.transform.eulerAngles = planetOverlookRot;
                        Camera.main.farClipPlane = cameraFarDist;

                        lightningCameraVolumeCloudsSM_URP lightning = Camera.main.gameObject.GetComponent<lightningCameraVolumeCloudsSM_URP>();
                        if(lightning != null)
                        {
                            //place near top of  planet
                            lightning.LightningBox.position = planetOverlookPos;
                            lightning.LightningBox.localScale = new Vector3(cameraFarDist, lightning.LightningBox.localScale.y, cameraFarDist);
                            lightning.max_lightning_time = 1;
                        }

                        connectSuntoVolumeFogURP fog = Camera.main.gameObject.GetComponent<connectSuntoVolumeFogURP>();
                        if(fog != null)
                        {
                            fog.enabled = false;
                            fog._FogColor = Color.grey * 0.78f;
                            fog.GlobalFogPower = 0;
                            fog.startDistance = 250000;
                            fog.lightControlA.x = 0.1f;
                            fog.LightRaySamples = 7;
                        }
                    }
                    cloudsScriptA.currentLocalLightIntensity = 110;
                    cloudsScriptA.cloudDistanceParams.x = 10000;
                    cloudsScriptA.thickness = 4500;
                    cloudsScriptA.scale = 0.1f;
                    planetScript.planetRadius = planetRadius;
                    planetScript.clouds = cloudsScriptA;
                    if(getPlanetScaleFrom != null)
                    {
                        planetScript.getScaleFrom = getPlanetScaleFrom;
                    }
                }
                if (configBiomes && biomeController != null && Camera.main != null)
                {
                    biomeController.Player = Camera.main.transform;
                    biomeController.fullVolumeClouds = cloudsScriptA;
                    if (Camera.main.gameObject.GetComponent<lightningCameraVolumeCloudsSM_URP>() != null)
                    {
                        biomeController.lightning = Camera.main.gameObject.GetComponent<lightningCameraVolumeCloudsSM_URP>();
                    }
                    if(Camera.main.gameObject.GetComponent<connectSuntoVolumeFogURP>() != null)
                    {
                        biomeController.fog = Camera.main.gameObject.GetComponent<connectSuntoVolumeFogURP>();
                    }
                }


                this.transform.position = Vector3.zero;
                SkyManager.Current_Time = 13.2f;

                if (sunController != null && addSmoothSun)
                {
                    //SkyManager.SunObj = sunController.temporalSunMoon;
                    //SkyManager.SUN_LIGHT = sunController.temporalSunMoon;
                    Camera.main.gameObject.GetComponent<connectSuntoVolumeFogURP>().sun = sunController.temporalSunMoon.transform;
                    Camera.main.gameObject.GetComponent<connectSuntoFullVolumeCloudsURP>().sun = sunController.temporalSunMoon.transform;
                    Camera.main.gameObject.GetComponent<connectSuntoFullVolumeCloudsURP>().sunLight = sunController.temporalSunMoon.GetComponent<Light>();
                }

                //find weather controller
                if (addFullVolumeClouds && weatherTexture != null)
                {
                    weatherTexture.clouds = Camera.main.gameObject.GetComponent<connectSuntoFullVolumeCloudsURP>();
                    //if (enableWater && SkyManager.water != null)
                    //{
                        //connectSuntoFullVolumeCloudsURP reflCloudsA = SkyManager.water.GetComponent<PlanarReflectionSM>().m_ReflectionCameraOut.gameObject.GetComponent<connectSuntoFullVolumeCloudsURP>();
                        //if (reflCloudsA != null)
                        //{
                        //    weatherTexture.cloudsREFL = reflCloudsA;
                        //}
                    //}
                    weatherTexture.enabled = true;
                    weatherTexture.gameObject.SetActive(true);
                }

                if (Camera.main.transform.eulerAngles.x == 0 && Camera.main.transform.eulerAngles.y == 0 && Camera.main.transform.eulerAngles.z == 0)
                {
                    Camera.main.transform.eulerAngles = new Vector3(0.04f,0,0);// SOLVE issue where scene load gets stuck, probably because camera in zero rotation
                }

                configured = true;
                this.enabled = false;
            }

        }
    }
}
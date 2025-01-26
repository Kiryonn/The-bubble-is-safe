using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Artngame.SKYMASTER
{
    [ExecuteInEditMode]
    public class setPlanetCloudsAtmosphere : MonoBehaviour
    {
        public connectSuntoFullVolumeCloudsURP clouds;
        public bool changeClouds = false;
        public Transform getScaleFrom;
        public float planetRadius = 598000;

        public bool enableDualLayer = false;
        public bool enableCloudAtmosphere = false;
        public bool enableCloudShaping = false;

        public float atmosphereFarScale = 1;//extend far plane above scale
        public float atmosphereRadius = 80; // rays resolution X 0.8f;
        public int atmosphereType = 6; //sun rays On
        public float atmospherePower = 10; //Sun rays power 1f
        public float atmosphereBrightness = 20;//ray shadowing W 2f
        public int scatterOn = 1;//scatter
        public float shadowPower = 1000; // rays resolution Y
        public float shadowTransparency = 10;// rays resolution Z 0.1f
        public bool shadowCastOnClouds = false;
        public float shadowCastPower = 10;//shadow power 1f
        public float cloudBrightnessNoShadows = 1; //if no shadows, give extra brightness
        public float sunLightPower = 4; //sun light factor
        public float ambientPower = 10; //ambient light factor 1f
        public float cloudDensity = 20;//density 2f
        public float cloudTransparency = 20; //cloud sample multiplier 2f

        public float weatherScale = 20; //0.02f
        public float cloudThickness = 5000;
        public float cloudStartHeight = 3000;
        public float cloudNoiseScale = 10; //0.1f;

        public float windSpeed = 1000;
        public float windCoverageSpeed = 900;
        public float windMultiplier = 20;
        public float windDirection = 10;

        public float darkSidePower = 0;
        public float darkSideRatio = 0;

        //VORTEX

        //SUPERCELLS
        public float extraCloudLayerHeight = 15000;//-15000 super cell X
        public float extraCloudLayerDensity = 10;//super cell Y 1f
        public float extraCloudLayerRatio = 20;
        public float cloudLayersCoverage = -20;//-0.1f

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (changeClouds && clouds != null)
            {
                float planetRadiusFinal = planetRadius;
                if (getScaleFrom != null)
                {
                    planetRadiusFinal = getScaleFrom.localScale.x;
                }
                clouds.planetSize = planetRadiusFinal / 2;
                clouds.planetZeroCoordinate = new Vector3(0, clouds.planetSize / 2, 0);


                /////
                clouds.extendFarPlaneAboveClouds = atmosphereFarScale;//extend far plane above scale

                if (enableCloudAtmosphere)
                {
                    clouds.raysResolution.x = atmosphereRadius / 100; // rays resolution X 0.8
                    clouds.sunRaysOn = atmosphereType; //sun rays On
                    clouds.sunRaysPower = atmospherePower / 10; //Sun rays power
                    clouds.rayShadowing.w = atmosphereBrightness / 10;//ray shadowing W
                    clouds.scatterOn = scatterOn;//scatter

                    clouds.raysResolution.y = shadowPower; // rays resolution Y
                    clouds.raysResolution.z = shadowTransparency / 100;// rays resolution Z

                    clouds.enableShadowCastOnClouds = shadowCastOnClouds;
                    if (shadowCastOnClouds)
                    {
                        clouds.shadowPower = shadowCastPower / 10;//shadow power
                    }
                    else
                    {
                        clouds.shadowPower = cloudBrightnessNoShadows / 10; //if no shadows, give extra brightness
                    }

                    clouds.specialFX.y = darkSidePower;
                    clouds.specialFX.z = darkSideRatio;
                }

                if (enableCloudShaping) {
                    clouds.sunLightFactor = sunLightPower; //sun light factor
                    clouds.ambientLightFactor = ambientPower / 10; //ambient light factor
                    clouds.density = cloudDensity / 10;//density
                    clouds.cloudSampleMultiplier = cloudTransparency / 10; //cloud sample multiplier

                    clouds.thickness = cloudThickness;
                    clouds.startHeight = cloudStartHeight;
                    clouds.scale = cloudNoiseScale / 100;

                    clouds.windSpeed = windSpeed;
                    clouds.coverageWindSpeed = windCoverageSpeed;
                    clouds.globalMultiplier = windMultiplier / 10;
                    clouds.windDirection = windDirection / 10;

                    clouds.weatherScale = weatherScale / 100;
                }

                //VORTEX

                //SUPERCELLS
                if (enableDualLayer)
                {
                    clouds.superCellControls.x = -extraCloudLayerHeight;//-15000 super cell X
                    clouds.superCellControls.y = extraCloudLayerDensity / 10;//super cell Y
                    clouds.superCellControls.z = extraCloudLayerRatio;
                    clouds.superCellControls.w = -cloudLayersCoverage / 100;
                }

            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artngame.SKYMASTER;
public class ORION_SkyMaster_Controller : MonoBehaviour
{
    public connectSuntoFullVolumeCloudsURP clouds;
    public float maxExtend = 1.5f;

    public float distanceForExtendOne = 4450;
    public float cloudsTopDistance = 4850; public float cloudsBottomDistance = 4450;
    public float extendMin = 0.2f;//decrease to lower noise
    public float decreaseSpeed = 1;

    public float depthDilationAboveClouds = 60;
    public float increaseExtendFactor = 0.5f;

    public int stepsHigh = 955;
    public int stepsLow = 301;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //1 in water surface - 3208.713, 2 in 3083.475 etc
        //4488
        //1.5

        //4688
        //+238

        //4450
        //1

        //4212
        //0.5
        float extend = 1 + increaseExtendFactor * ((Camera.main.transform.position.magnitude - distanceForExtendOne) / (238));

        if (Camera.main.transform.position.magnitude < cloudsTopDistance)
        {
            if (extend < maxExtend)
            {
                clouds.extendFarPlaneAboveClouds = Mathf.Lerp(clouds.extendFarPlaneAboveClouds, extend, Time.deltaTime * decreaseSpeed);      //extend;
            }

            //TOP CLOUDS
            clouds.coverageHigh = Mathf.Lerp(clouds.coverageHigh, 2, Time.deltaTime * decreaseSpeed);

        }
        else
        {
            clouds.extendFarPlaneAboveClouds = Mathf.Lerp(clouds.extendFarPlaneAboveClouds, extendMin, Time.deltaTime*decreaseSpeed);

            //TOP CLOUDS
            clouds.coverageHigh = Mathf.Lerp(clouds.coverageHigh, 0, Time.deltaTime * decreaseSpeed *1.5f);
        }

        if (Camera.main.transform.position.magnitude < cloudsBottomDistance)
        {           
            clouds.depthDilation = depthDilationAboveClouds;

            //STEPS control - high when under clouds - lower above
            clouds.steps = stepsHigh;
        }
        else
        {
            clouds.depthDilation = 0;

            //STEPS control - high when under clouds - lower above
            clouds.steps = stepsLow;
        }
    }
}

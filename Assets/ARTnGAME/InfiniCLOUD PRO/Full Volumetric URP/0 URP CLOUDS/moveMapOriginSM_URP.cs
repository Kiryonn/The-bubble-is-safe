using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Artngame.SKYMASTER
{
    [ExecuteInEditMode]
    public class moveMapOriginSM_URP : MonoBehaviour
    {
        //v0.1a
        public bool fullyThreeDim = false;

        public Transform MapToMove;
        public float moveAfterDist = 1000;
        public Transform player;
        //public CloudScript fullvolumeClouds;
        public connectSuntoFullVolumeCloudsURP fullvolumeClouds;
        public Material shadowsMat;
        public bool operateOnlyXZ = true;// do the move only in X,Z

        //v0.1
        public bool moveCloudsToPlanets = false;
        public float moveDistance = 1000;
        public List<Transform> planetCenters = new List<Transform>();
        public int currentPlanet = 0;
        public bool getFromBiomes = false;
        public biomesSkyMaster biomeController;

        Vector3 initPlayerPos;

        // Start is called before the first frame update
        void Start()
        {
            if (shadowsMat != null && shadowsMat.HasProperty("cameraWSOffset"))
            {
                //Vector3 currentOffset = shadowsMat.GetVector("cameraWSOffset");
                shadowsMat.SetVector("cameraWSOffset", Vector3.zero);
            }
            if (fullvolumeClouds != null)
            {
                fullvolumeClouds.cameraWSOffset = Vector3.zero;
            }
            initPlayerPos = player.transform.position;
        }

        //v0.1
        Vector3 planetShift = Vector3.zero;
        public bool initializedMotion = false;

        void LateUpdate()
        {
            if (useLateUpdate || useBothUpdates)
            {
                myUpdate();
            }
        }
        void Update()
        {
            if (!useLateUpdate || useBothUpdates)
            {
                myUpdate();
            }
        }

        void FixedUpdate()
        {
            if (useBothUpdates)
            {
                myUpdate();
            }
        }

        public bool useLateUpdate = false;
        public bool useBothUpdates = false;
        // Update is called once per frame
        void myUpdate()
        {
            //v0.1
            //Vector3 planetShift = Vector3.zero;
            if (moveCloudsToPlanets)
            {                
                if (getFromBiomes)
                {
                    for (int i = 0; i < biomeController.biomesCenters.Count; i++)
                    {
                        //find closest and switch
                        float distA = Vector3.Distance(player.transform.position, biomeController.biomesCenters[i].position);
                        if (distA < moveDistance)
                        {
                            planetShift = biomeController.biomesCenters[i].position;
                            currentPlanet = i;
                            break; //assume non overlaps
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < planetCenters.Count; i++)
                    {
                        //find closest and switch
                        float distA = Vector3.Distance(player.transform.position, planetCenters[i].position);
                        //Debug.Log("Dist from " + i + "is: " + distA );
                        if (distA < moveDistance)
                        {
                            planetShift = planetCenters[i].position;
                            currentPlanet = i;
                            break; //assume non overlaps
                        }
                    }
                }
            }

            if (!initializedMotion)
            {
                if (getFromBiomes)
                {
                    planetShift = biomeController.biomesCenters[0].position;
                    currentPlanet = 0;
                }
                else
                {
                    planetShift = planetCenters[0].position;
                    currentPlanet = 0;
                }
            }

            float dist = Vector3.Distance(player.transform.position, initPlayerPos);
            if (operateOnlyXZ)
            {
                dist = Vector2.Distance(
                    new Vector2(player.transform.position.x, player.transform.position.z),
                    new Vector2(initPlayerPos.x, initPlayerPos.z));
            }

         

            if (MapToMove != null && dist >= moveAfterDist && Application.isPlaying)// && !initializedMotion) //v0.1
            {
                //v0.1
                initializedMotion = true;

                //move all by the current offset
                if (fullvolumeClouds != null)
                {
                    if (operateOnlyXZ)
                    {
                        fullvolumeClouds.cameraWSOffset += new Vector3(player.transform.position.x, 0, player.transform.position.z);
                    }
                    else
                    {
                        fullvolumeClouds.cameraWSOffset += player.transform.position;

                        //v0.1
                        if (getFromBiomes)
                        {
                            planetShift = biomeController.biomesCenters[currentPlanet].position;
                        }
                        else
                        {
                            planetShift = planetCenters[currentPlanet].position;
                        }
                        //fullvolumeClouds.planetZeroCoordinate = planetShift + fullvolumeClouds.cameraWSOffset + new Vector3(0, fullvolumeClouds.planetSize/4, 0);
                        Vector3 planShift = planetShift + fullvolumeClouds.cameraWSOffset;

                        if (fullyThreeDim)
                        {
                            //fullvolumeClouds.planetZeroCoordinate = planShift;
                            //planShift.y = -planetShift.y - fullvolumeClouds.cameraWSOffset.y;
                            fullvolumeClouds.planetZeroCoordinate = new Vector3(planShift.x,  fullvolumeClouds.planetSize + planShift.y, planShift.z);
                        }
                        else
                        {
                            planShift.y = -planetShift.y - fullvolumeClouds.cameraWSOffset.y;
                            //fullvolumeClouds.planetZeroCoordinate = new Vector3(planShift.x, planShift.y +  planetShift.y, planShift.z);// + new Vector3(0, fullvolumeClouds.planetSize / 4, 0);
                            fullvolumeClouds.planetZeroCoordinate = new Vector3(planShift.x, planetShift.y + fullvolumeClouds.planetSize + fullvolumeClouds.cameraWSOffset.y, planShift.z);
                        }
                    }

                    if (shadowsMat != null && shadowsMat.HasProperty("cameraWSOffset"))
                    {
                        if (Application.isPlaying)
                        {
                            //Vector3 currentOffset = shadowsMat.GetVector("cameraWSOffset");
                            shadowsMat.SetVector("cameraWSOffset", fullvolumeClouds.cameraWSOffset + planetShift);// currentOffset + player.transform.position);
                        }
                        else
                        {
                            shadowsMat.SetVector("cameraWSOffset", Vector3.zero);
                        }
                    }
                }
                //player.transform.position = Vector3.zero;
                if (operateOnlyXZ)
                {
                    MapToMove.transform.position -= new Vector3(player.transform.position.x, 0, player.transform.position.z);
                }
                else
                {
                    MapToMove.transform.position -= player.transform.position;
                }

                //v0.1
               // initPlayerPos = player.transform.position;

                //NOW reset player to zero
                if (operateOnlyXZ)
                {
                    player.transform.position = new Vector3(0, player.transform.position.y, 0);
                }
                else
                {
                    player.transform.position = Vector3.zero;
                }

                //v0.1
                initPlayerPos = player.transform.position;
            }


           
        }
    }
}
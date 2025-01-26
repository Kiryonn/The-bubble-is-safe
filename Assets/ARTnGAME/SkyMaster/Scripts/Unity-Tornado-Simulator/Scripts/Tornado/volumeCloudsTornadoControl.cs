using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artngame.SKYMASTER
{
    public class volumeCloudsTornadoControl : MonoBehaviour
    {
        public connectSuntoFullVolumeCloudsURP volumeclouds;
        public Transform tornadoTop;
        public float posMultiplierX = 1;
        public float posMultiplierZ = 1;
        public float posOffsetX = 0;
        public float posOffsetZ = 0;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (volumeclouds != null && tornadoTop != null)
            {
                volumeclouds.vortexPosition = new Vector3(tornadoTop.position.x * posMultiplierX + posOffsetX, 0 , tornadoTop.position.z * posMultiplierZ + posOffsetZ);
            }
        }
    }
}
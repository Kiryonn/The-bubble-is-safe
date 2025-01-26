using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Artngame.SKYMASTER
{
    public class disableParticleShadowReceiveSM : MonoBehaviour
    {

        public ParticleSystem particle;
        public bool receiveShadows = false;
        public bool enableAtStart = true;

        // Start is called before the first frame update
        void Start()
        {
            ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
            renderer.receiveShadows = receiveShadows;
            //renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            //renderer.ligh
        }

        // Update is called once per frame
        void Update()
        {
            if (!enableAtStart)
            {
                ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
                renderer.receiveShadows = receiveShadows;
            }
        }
    }
}
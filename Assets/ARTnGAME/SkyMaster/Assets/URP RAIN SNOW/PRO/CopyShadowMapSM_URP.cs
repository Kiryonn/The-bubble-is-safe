//https://forum.unity.com/threads/solved-shadow-mask-in-a-postprocess-shader.707888/
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Artngame.SKYMASTER
{
    [ExecuteInEditMode]
    public class CopyShadowMapSM_URP : MonoBehaviour
    {
        CommandBuffer cb = null;

        void OnEnable()
        {
            var light = GetComponent<Light>();
            if (light)
            {
                cb = new CommandBuffer();
                cb.name = "CopyShadowMap";
                cb.SetGlobalTexture("_DirectionalShadowMask", new RenderTargetIdentifier(BuiltinRenderTextureType.CurrentActive));
                light.AddCommandBuffer(UnityEngine.Rendering.LightEvent.AfterScreenspaceMask, cb);
            }
        }

        void OnDisable()
        {
            var light = GetComponent<Light>();
            if (light)
            {
                light.RemoveCommandBuffer(UnityEngine.Rendering.LightEvent.AfterScreenspaceMask, cb);
            }
        }
    }
}
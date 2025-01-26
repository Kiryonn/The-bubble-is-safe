// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "PlanetaryTerrain/PlanetFadeShaderBumpSM_URP" {
	
    // The properties block of the Unity shader. In this example this block is empty
    // because the output color is predefined in the fragment shader code.
    Properties
    { 
	
		_Tex1 ("Texture 0 (Beach)", 2D) = "white" {}
		_Nor1 ("Normal", 2D) = "bump" {}
		_Glossiness1 ("Smoothness", Range(0,1)) = 0.5
		_Metallic1 ("Metallic", Range(0,1)) = 0.0
		_TexScale1 ("Texture Scale", Float) = 1

		[Space(20)]
		_Tex2 ("Texture 1", 2D) = "white" {}
		_Nor2 ("Normal", 2D) = "bump" {}
		_Glossiness2 ("Smoothness", Range(0,1)) = 0.5
		_Metallic2 ("Metallic", Range(0,1)) = 0.0
		_TexScale2 ("Texture Scale", Float) = 1

		[Space(20)]
		_Tex3 ("Texture 2", 2D) = "white" {}
		_Nor3 ("Normal", 2D) = "bump" {}
		_Glossiness3 ("Smoothness", Range(0,1)) = 0.5
		_Metallic3 ("Metallic", Range(0,1)) = 0.0
		_TexScale3 ("Texture Scale", Float) = 1

		[Space(20)]
		_Tex4 ("Texture 3", 2D) = "white" {}
		_Nor4 ("Normal", 2D) = "bump" {}
		_Glossiness4 ("Smoothness", Range(0,1)) = 0.5
		_Metallic4 ("Metallic", Range(0,1)) = 0.0
		_TexScale4 ("Texture Scale", Float) = 1

		[Space(20)]
		_Tex5 ("Texture 4", 2D) = "white" {}
		_Nor5 ("Normal", 2D) = "bump" {}
		_Glossiness5 ("Smoothness", Range(0,1)) = 0.5
		_Metallic5 ("Metallic", Range(0,1)) = 0.0
		_TexScale5 ("Texture Scale", Float) = 1

		[Space(20)]
		_Tex6 ("Texture 5 (Mountains)", 2D) = "white" {}
		_Nor6 ("Normal", 2D) = "bump" {}
		_Glossiness6 ("Smoothness", Range(0,1)) = 0.5
		_Metallic6 ("Metallic", Range(0,1)) = 0.0
		_TexScale6 ("Texture Scale", Float) = 1

		[Space(50)]
		[Header(Textures faded to when far away)]
		[Space]

		_Tex1Fade ("Texture 0 Fade", 2D) = "white" {}
		_Nor1Fade ("Normal", 2D) = "bump" {}
		_TexScale1Fade ("Texture Scale", Float) = 1
		[Space(20)]

		_Tex2Fade ("Texture 1 Fade", 2D) = "white" {}
		_Nor2Fade ("Normal", 2D) = "bump" {}
		_TexScale2Fade ("Texture Scale", Float) = 1
		[Space(20)]

		_Tex3Fade ("Texture 2 Fade", 2D) = "white" {}
		_Nor3Fade ("Normal", 2D) = "bump" {}
		_TexScale3Fade ("Texture Scale", Float) = 1
		[Space(20)]

		_Tex4Fade ("Texture 3 Fade", 2D) = "white" {}
		_Nor4Fade ("Normal", 2D) = "bump" {}
		_TexScale4Fade ("Texture Scale", Float) = 1
		[Space(20)]

		_Tex5Fade ("Texture 4 Fade", 2D) = "white" {}
		_Nor5Fade ("Normal", 2D) = "bump" {}
		_TexScale5Fade ("Texture Scale", Float) = 1
		[Space(20)]
		_Tex6Fade ("Texture 5 Fade", 2D) = "white" {}
		_Nor6Fade ("Normal", 2D) = "bump" {}
		_TexScale6Fade ("Texture Scale", Float) = 1

		[Space(50)]

		_FadeRangeMin ("Fade Start", Float) = 1
		_FadeRangeMax ("Fade End", Float) = 1
	
	}

    // The SubShader block containing the Shader code.
    SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
			Tags { "LIGHTMODE"="UniversalForward" "SHADOWSUPPORT"="true" "RenderType"="Opaque" }
			ZWrite On

            // The HLSL code block. Unity SRP uses the HLSL language.
            HLSLPROGRAM
            // This line defines the name of the vertex shader.
            #pragma vertex vert
            // This line defines the name of the fragment shader.
            #pragma fragment frag


			   #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _FOG
            #pragma shader_feature_local _VERTEX_COLOR
            #pragma shader_feature_local _ADDITIONAL_LIGHTS_ENABLED
            #pragma shader_feature_local _SPECULAR
            #pragma shader_feature_local _ANISO_SPECULAR
            #pragma shader_feature_local _ADDITIONAL_LIGHTS_SPECULAR
            #pragma shader_feature_local _ENVIRONMENT_LIGHTING_ENABLED
            #pragma shader_feature_local _SHADOW_MASK
            
            #pragma shader_feature_local_fragment _FRESNEL
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _RAMP_TRIPLE
            #pragma shader_feature_local_fragment _RAMP_MAP
            #pragma shader_feature_local_fragment _PURE_SHADOW_COLOR

            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON

            #pragma shader_feature_local_fragment _REFLECTIONS
            #pragma shader_feature_local_fragment _REFLECTION_PROBES
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            
            
            // Unity
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog

			#pragma multi_compile_instancing



            // The Core.hlsl file contains definitions of frequently used HLSL
            // macros and functions, and also contains #include references to other
            // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // The structure definition defines which variables it contains.
            // This example uses the Attributes structure as an input structure in
            // the vertex shader.
            struct Attributes
            {
                // The positionOS variable contains the vertex positions in object
                // space.
                float4 positionOS   : POSITION;
            };

            struct Varyings
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionHCS  : SV_POSITION;
            };

            // The vertex shader definition with properties defined in the Varyings
            // structure. The type of the vert function must match the type (struct)
            // that it returns.
            Varyings vert(Attributes IN)
            {
                // Declaring the output object (OUT) with the Varyings struct.
                Varyings OUT;
                // The TransformObjectToHClip function transforms vertex positions
                // from object space to homogenous clip space.
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                // Returning the output.
                return OUT;
            }

            // The fragment shader definition.
            half4 frag() : SV_Target
            {
                // Defining the color variable and returning it.
                half4 customColor = half4(0.5, 0, 0, 1);
                return customColor;
            }
            ENDHLSL
        }

		 // Used for rendering shadowmaps
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"

        // Used for depth prepass
        // If shadows cascade are enabled we need to perform a depth prepass. 
        // We also need to use a depth prepass in some cases camera require depth texture
        // (e.g, MSAA is enabled and we can't resolve with Texture2DMS
        UsePass "Universal Render Pipeline/Lit/DepthOnly"

        // Used for Baking GI. This pass is stripped from build.
        UsePass "Universal Render Pipeline/Lit/Meta"
    }
}
Shader "Unlit/AtmosSRP_FORWARD_URP_MULTIPLANET"
{

	Properties
	{
		[HideInInspector]_MainTex("Base (RGB)", 2D) = "white" {}
	//[HideInInspector]_ColorBuffer("Base (RGB)", 2D) = "white" {}
	 
		//[HideInInspector]_MainTex("Base (RGB)", 2D) = "white" {}
		//_Delta("Line Thickness", Range(0.0005, 0.0025)) = 0.001
		//[Toggle(RAW_OUTLINE)]_Raw("Outline Only", Float) = 0
		//[Toggle(POSTERIZE)]_Poseterize("Posterize", Float) = 0
		//_PosterizationCount("Count", int) = 8

		_SunThreshold("sun thres", Color) = (0.87, 0.74, 0.65,1)
		_SunColor("sun color", Color) = (1.87, 1.74, 1.65,1)
		_BlurRadius4("blur", Color) = (0.00325, 0.00325, 0,0)
		_SunPosition("sun pos", Color) = (111, 11,339, 11)


		///////////// AtmosSRP_FORWARD_URP
		_Brightness ("Brightness", Range(0, 2)) = 1

		_Beta ("Beta", Range(0, 10)) = 0

		_FadeStart ("Fade Start", Float) = 10000
		_FadeEnd ("Fade End", Float) = 40000
		
		divideSky ("divide Sky", Float) = 2

		//v0.1
		skyControlsA ("skyControlsA", Vector) = (1,1,1,1)
		skyControlsPlanetB("skyControlsB", Vector) = (1,1,1,1)

		[KeywordEnum(None, DistanceToFragment, DistanceToPlanetCenter)] _Fade("Fade mode", Float) = 1
	}

		HLSLINCLUDE

		//#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl" //unity 2018.3
//#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl" 
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
		//#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/SurfaceInput.hlsl"
		//#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
		//#include "PostProcessing/Shaders/StdLib.hlsl" //unity 2018.1-2
		//#include "UnityCG.cginc"

		TEXTURE2D(_MainTex);
	TEXTURE2D(_ColorBuffer);
	TEXTURE2D(_Skybox);

	SAMPLER(sampler_MainTex);
	SAMPLER(sampler_ColorBuffer);
	SAMPLER(sampler_Skybox);
	float _Blend;

	//sampler2D _MainTex;
	//sampler2D _ColorBuffer;
	//sampler2D _Skybox;
	//sampler2D_float _CameraDepthTexture;
	TEXTURE2D(_CameraDepthTexture);
	SAMPLER(sampler_CameraDepthTexture);
	half4 _CameraDepthTexture_ST;

	half4 _SunThreshold = half4(0.87, 0.74, 0.65, 1);

	half4 _SunColor = half4(0.87, 0.74, 0.65, 1);
	uniform half4 _BlurRadius4 = half4(2.5 / 768, 2.5 / 768, 0.0, 0.0);
	uniform half4 _SunPosition = half4(1,1,1,1);
	uniform half4 _MainTex_TexelSize;

#define SAMPLES_FLOAT 16.0f
#define SAMPLES_INT 16

	// Vertex manipulation
	float2 TransformTriangleVertexToUV(float2 vertex)
	{
		float2 uv = (vertex + 1.0) * 0.5;
		return uv;
	}

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
#if UNITY_UV_STARTS_AT_TOP
		float2 uv1 : TEXCOORD1;
#endif		
	};

	struct v2f_radial {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float2 blurVector : TEXCOORD1;
	};

	struct Varyings
	{
		float2 uv        : TEXCOORD0;
		float4 vertex : SV_POSITION;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	float Linear01DepthA(float2 uv)
	{
#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		return SAMPLE_TEXTURE2D_ARRAY(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex).r;
#else
		return SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv);
#endif
	}

	float4 FragGrey(v2f i) : SV_Target
	{
		float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy);
		half4 colorB = SAMPLE_TEXTURE2D(_ColorBuffer, sampler_ColorBuffer, i.uv.xy);
		//float luminance = dot(color.rgb, float3(0.2126729, 0.7151522, 0.0721750));
		//color.rgb = lerp(color.rgb, luminance.xxx, _Blend.xxx);
		//return color/2 + colorB/2;
		return color ;
	}

	float4 FragDEPTH(v2f i) : SV_Target
	{	
		float color = Linear01DepthA(i.uv);		
		return float4(1,1,1,1)*color*1 ;		
	}

	half4 fragScreen(v2f i) : SV_Target{

				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

			//half4 colorA = tex2D(_MainTex, i.uv.xy);
			half4 colorA = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy); // half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
		#if !UNITY_UV_STARTS_AT_TOP
																				 ///half4 colorB = tex2D(_ColorBuffer, i.uv1.xy);
			half4 colorB = SAMPLE_TEXTURE2D(_ColorBuffer, sampler_ColorBuffer, i.uv.xy);//v0.2 //i.uv1.xy);//v0.2
		#else
																				 //half4 colorB = tex2D(_ColorBuffer, i.uv.xy);
			half4 colorB = SAMPLE_TEXTURE2D(_ColorBuffer, sampler_ColorBuffer, i.uv.xy);//v1.1
		#endif
			half4 depthMask = saturate(colorB * _SunColor);
			return  1.0f - (1.0f - colorA) * (1.0f - depthMask);//colorA * 5.6;// 1.0f - (1.0f - colorA) * (1.0f - depthMask);
	}


	half4 fragAdd(v2f i) : SV_Target{

		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

		//half4 colorA = tex2D(_MainTex, i.uv.xy);
		half4 colorA = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy);
#if !UNITY_UV_STARTS_AT_TOP
		//half4 colorB = tex2D(_ColorBuffer, i.uv1.xy);
		half4 colorB = SAMPLE_TEXTURE2D(_ColorBuffer, sampler_ColorBuffer, i.uv.xy); //v0.1 - i.uv1.xy
#else
		//half4 colorB = tex2D(_ColorBuffer, i.uv.xy);
		half4 colorB = SAMPLE_TEXTURE2D(_ColorBuffer, sampler_ColorBuffer, i.uv.xy);
#endif
		half4 depthMask = saturate(colorB * _SunColor);
		return 1 * colorA + depthMask;
	}

	struct Attributes
	{
		float4 positionOS       : POSITION;
		float2 uv               : TEXCOORD0;
	};

	v2f vert(Attributes v) {//v2f vert(AttributesDefault v) { //appdata_img v) {
							//v2f o;
		v2f o = (v2f)0;
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		//VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
		//o.pos = vertexInput.positionCS;
		//o.uv = v.uv;
		//Varyings output = (Varyings)0;
		//UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
		VertexPositionInputs vertexInput = GetVertexPositionInputs(v.positionOS.xyz);
		//output.vertex = vertexInput.positionCS;
		//output.uv = input.uv;
		//return output;


		//o.pos = UnityObjectToClipPos(v.vertex);
		//	o.pos = float4(v.vertex.xy, 0.0, 1.0);
		//	float2 uv = TransformTriangleVertexToUV(v.vertex.xy);

		o.pos = float4(vertexInput.positionCS.xy, 0.0, 1.0);
		float2 uv = v.uv;

		//o.uv = uv;// v.texcoord.xy;

		//o.uv1 = uv.xy;



		//// NEW 1
		//o.pos = float4(v.positionOS.xy, 0.0, 1.0);
		//uv = TransformTriangleVertexToUV(v.positionOS.xy);

#if !UNITY_UV_STARTS_AT_TOP
		uv = uv * float2(1.0, -1.0) + float2(0.0, 1.0);
		//uv.y = 1-uv.y;
#endif

		o.uv = uv;// v.texcoord.xy;

#if !UNITY_UV_STARTS_AT_TOP
		o.uv = uv.xy;//o.uv1 = uv.xy;
		if (_MainTex_TexelSize.y < 0)
			o.uv.y = 1 - o.uv.y;//o.uv1.y = 1 - o.uv1.y;
#endif	




		return o;
	}

	v2f_radial vert_radial(Attributes v) {//v2f_radial vert_radial(AttributesDefault v) { //appdata_img v) {
		//v2f_radial o;

		v2f_radial o = (v2f_radial)0;
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		////		o.pos = UnityObjectToClipPos(v.vertex);

		//o.pos = float4(v.vertex.xyz,1);
		//o.pos = float4(v.vertex.xy, 0.0, 1.0);
		//float2 uv = TransformTriangleVertexToUV(v.vertex.xy);

		VertexPositionInputs vertexInput = GetVertexPositionInputs(v.positionOS.xyz);
		o.pos = float4(vertexInput.positionCS.xy, 0.0, 1.0);
		float2 uv = v.uv;
		//output.vertex = vertexInput.positionCS;

		//uv = TransformTriangleVertexToUV(vertexInput.positionCS.xy);

		#if !UNITY_UV_STARTS_AT_TOP
				//uv = uv * float2(1.0, -1.0) + float2(0.0, 1.0);
		#endif

		o.uv.xy = uv;//v.texcoord.xy;
					 //o.blurVector = (_SunPosition.xy - v.texcoord.xy) * _BlurRadius4.xy;
		//o.uv1 = uv.xy;
		//o.uv.y = 1 - o.uv.y;
		//uv.y = 1 - uv.y;
		//o.uv.y = 1 - o.uv.y;
		//_SunPosition.y = _SunPosition.y*0.5 + 0.5;
		//_SunPosition.x = _SunPosition.x*0.5 + 0.5;
		o.blurVector = (_SunPosition.xy - uv.xy) * _BlurRadius4.xy;

		return o;
	}

	half4 frag_radial(v2f_radial i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

		half4 color = half4(0,0,0,0);
		for (int j = 0; j < SAMPLES_INT; j++)
		{
			//half4 tmpColor = tex2D(_MainTex, i.uv.xy);
			half4 tmpColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy);
			color += tmpColor;
			i.uv.xy += i.blurVector;
		}
		return color / SAMPLES_FLOAT;
	}

	half TransformColor(half4 skyboxValue) {
		return dot(max(skyboxValue.rgb - _SunThreshold.rgb, half3(0, 0, 0)), half3(1, 1, 1)); // threshold and convert to greyscale
	}

	half4 frag_depth(v2f i) : SV_Target{

		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

	#if !UNITY_UV_STARTS_AT_TOP
			//float depthSample = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv1.xy);
			float depthSample = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.uv.xy), _ZBufferParams); //v0.1 URP i.uv1.xy
	#else
			//float depthSample = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv.xy);
			float depthSample = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.uv.xy), _ZBufferParams);
	#endif

		//half4 tex = tex2D(_MainTex, i.uv.xy);
		half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy);
		//depthSample = Linear01Depth(depthSample, _ZBufferParams);

		// consider maximum radius
	#if !UNITY_UV_STARTS_AT_TOP
		half2 vec = _SunPosition.xy - i.uv.xy; //i.uv1.xy;
	#else
		half2 vec = _SunPosition.xy - i.uv.xy;
	#endif
		half dist = saturate(_SunPosition.w - length(vec.xy));

		half4 outColor = 0;

		// consider shafts blockers
		//if (depthSample > 0.99)
		//if (depthSample > 0.103)
		if (depthSample > 1- 0.018) {//if (depthSample < 0.018) {
			//outColor = TransformColor(tex) * dist;
		}





#if !UNITY_UV_STARTS_AT_TOP
		if (depthSample < 0.018) {
			outColor = TransformColor(tex) * dist;
		}
#else
		if (depthSample > 1 - 0.018) {
			outColor = TransformColor(tex) * dist;
		}
#endif

		return outColor * 1;
	}

	//inline half Luminance(half3 rgb)
	//{
		//return dot(rgb, unity_ColorSpaceLuminance.rgb);
	//	return dot(rgb, rgb);
	//}

	half4 frag_nodepth(v2f i) : SV_Target{

		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

	#if !UNITY_UV_STARTS_AT_TOP
			//float4 sky = (tex2D(_Skybox, i.uv1.xy));
			float4 sky = SAMPLE_TEXTURE2D(_Skybox, sampler_Skybox, i.uv.xy);
	#else
			//float4 sky = (tex2D(_Skybox, i.uv.xy));
			float4 sky = SAMPLE_TEXTURE2D(_Skybox, sampler_Skybox, i.uv.xy); //i.uv1.xy;
	#endif

			//float4 tex = (tex2D(_MainTex, i.uv.xy));
			half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy);
			//sky = float4(0.3, 0.05, 0.05,  1);
			/// consider maximum radius
	#if !UNITY_UV_STARTS_AT_TOP
			half2 vec = _SunPosition.xy - i.uv.xy;
	#else
			half2 vec = _SunPosition.xy - i.uv.xy;//i.uv1.xy;
	#endif
			half dist = saturate(_SunPosition.w - length(vec));

			half4 outColor = 0;

			// find unoccluded sky pixels
			// consider pixel values that differ significantly between framebuffer and sky-only buffer as occluded


			if (Luminance(abs(sky.rgb - tex.rgb)) < 0.2) {
				outColor = TransformColor(tex) * dist;
				//outColor = TransformColor(sky) * dist;
			}

			return outColor * 1;
	}

		ENDHLSL

		//		SubShader
		//	{
		//		//Cull Off ZWrite Off ZTest Always
		//
		//			Pass
		//		{
		//			HLSLPROGRAM
		//
		//#pragma vertex VertDefault
		//#pragma fragment Frag
		//
		//			ENDHLSL
		//		}
		//	}
		Subshader {
		//Tags{ "RenderType" = "Opaque" }
			Pass{
			ZTest Always Cull Off ZWrite Off

			HLSLPROGRAM

#pragma vertex vert
#pragma fragment fragScreen

			ENDHLSL
		}

			Pass{
			ZTest Always Cull Off ZWrite Off

			HLSLPROGRAM

#pragma vertex vert_radial
#pragma fragment frag_radial

			ENDHLSL
		}

			Pass{
			ZTest Always Cull Off ZWrite Off

			HLSLPROGRAM

#pragma vertex vert
#pragma fragment frag_depth

			ENDHLSL
		}

			Pass{
			ZTest Always Cull Off ZWrite Off

			HLSLPROGRAM

#pragma vertex vert
#pragma fragment frag_nodepth

			ENDHLSL
		}

			Pass{
			ZTest Always Cull Off ZWrite Off

			HLSLPROGRAM

#pragma vertex vert
#pragma fragment fragAdd

			ENDHLSL
		}


			//PASS 5
			Pass{
			ZTest Always Cull Off ZWrite Off

			HLSLPROGRAM

#pragma vertex vert
#pragma fragment FragGrey

			ENDHLSL
		}

			//PASS 6 DEPTH
			Pass{
			ZTest Always Cull Off ZWrite Off

			HLSLPROGRAM

#pragma vertex vert
#pragma fragment FragDEPTH

			ENDHLSL
		}

		/////ATMOS PASS 7
		Pass
		{
			CGPROGRAM//HLSLPROGRAM//CGPROGRAM
			#pragma vertex vertATMOS
			#pragma fragment frag

			#pragma multi_compile __ RADIANCE_API_ENABLED
			#pragma multi_compile __ COMBINED_SCATTERING_TEXTURES
			
			// Z buffer to linear depth
			inline float LinearEyeDepth( float z )
			{
				return 1.0 / (_ZBufferParams.z * z + _ZBufferParams.w);
			}
		//	#include "UnityCG.cginc"
			#include "Definitions.cginc"
			#include "UtilityFunctions.cginc"
			#include "TransmittanceFunctions.cginc"
			#include "ScatteringFunctions.cginc"
			#include "IrradianceFunctions.cginc"
			#include "RenderingFunctions.cginc"
			//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			//v0.1
			float divideSky;
			float4 skyControlsA;
			float4 skyControlsPlanetB;
		

			float4 _blendControls;
			//	TEXTURE2D(_CameraDepthTexture);
	//SAMPLER(sampler_CameraDepthTexture);
	//half4 _CameraDepthTexture_ST;


			static const float3 kSphereCenter = float3(0.0, 1.0, 0.0);
			static const float kSphereRadius = 0.0;
			static const float3 kSphereAlbedo = float3(0.8, 0.8, 0.8);
			static const float3 kGroundAlbedo = float3(0.282, 0.314, 0.110);

			float exposure;
			float3 white_point;
			float3 earth_center;
			float3 sun_direction;
			float2 sun_size;
			float max_terrain_radius;

			float _Brightness;
			float _Fade;
			float _FadeStart;
			float _FadeEnd;
			float _Beta;

			float4x4 frustumCorners;

			float4x4 clip_to_world;

			sampler2D transmittance_texture;
			sampler2D irradiance_texture;
			sampler3D scattering_texture;
			sampler3D single_mie_scattering_texture;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2fATMOS
			{
				float3 view_ray : TEXCOORD1;
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldDirection : TEXCOORD2;
			};

			v2fATMOS vertATMOS (appdata v, uint vertexID : SV_VertexID)//v0.1
			{
				v2fATMOS o;

				int index = v.vertex.z;
				v.vertex.z = 0.0;

				o.vertex = UnityObjectToClipPos(v.vertex);


				//https://forum.unity.com/threads/how-to-find-the-index-of-a-vertex-in-a-fragment-shader.735839/
				//o.view_ray = frustumCorners[index].xyz;
				float x = (vertexID != 1) ? -1 : 3;
				float y = (vertexID == 2) ? -3 : 1;
				//float3 vpos = float3(x, y, 1.0); 
				// Perspective: view space vertex position of the far plane
				//float3 rayPers = mul(unity_CameraInvProjection, vpos.xyzz ).xyz;
				//index = 0;
				if(vertexID == 0){
					index = 3;
				}
				if(vertexID == 1){
					index = 0;
				}
				if(vertexID == 2){
					index = 1;
				}
				if(vertexID == 3){
					index = 2;
				}
				o.view_ray = frustumCorners[index].xyz;

				o.uv = v.uv;

				float4 clip = float4(o.uv.xy * 2 - 1, 0.0, 1.0);
                o.worldDirection = mul(clip_to_world, clip) - _WorldSpaceCameraPos;

				return o;
			}

			/*
			The functions to compute shadows and light shafts must be defined before we
			can use them in the main shader function, so we define them first. Testing if
			a point is in the shadow of the sphere S is equivalent to test if the
			corresponding light ray intersects the sphere, which is very simple to do.
			However, this is only valid for a punctual light source, which is not the case
			of the Sun. In the following function we compute an approximate (and biased)
			soft shadow by taking the angular size of the Sun into account:
			*/
			float GetSunVisibility(float3 _point, float3 sun_direction)
			{
				float3 p = _point - kSphereCenter;
				float p_dot_v = dot(p, sun_direction);
				float p_dot_p = dot(p, p);
				float ray_sphere_center_squared_distance = p_dot_p - p_dot_v * p_dot_v;
				float distance_to_intersection = -p_dot_v - sqrt(max(0.0, kSphereRadius * kSphereRadius - ray_sphere_center_squared_distance));

				if (distance_to_intersection > 0.0) 
				{
					// Compute the distance between the view ray and the sphere, and the
					// corresponding (tangent of the) subtended angle. Finally, use this to
					// compute an approximate sun visibility.
					float ray_sphere_distance = kSphereRadius - sqrt(ray_sphere_center_squared_distance);
					float ray_sphere_angular_distance = -ray_sphere_distance / p_dot_v;

					return smoothstep(1.0, 0.0, ray_sphere_angular_distance / sun_size.x);
				}

				return 1.0;
			}

			/*
			The sphere also partially occludes the sky light, and we approximate this
			effect with an ambient occlusion factor. The ambient occlusion factor due to a
			sphere is given in <a href=
			"http://webserver.dmt.upm.es/~isidoro/tc3/Radiation%20View%20factors.pdf"
			>Radiation View Factors</a> (Isidoro Martinez, 1995). In the simple case where
			the sphere is fully visible, it is given by the following function:
			*/
			float GetSkyVisibility(float3 _point) 
			{
				float3 p = _point - kSphereCenter;
				float p_dot_p = dot(p, p);
				return 1.0 + p.y / sqrt(p_dot_p) * kSphereRadius * kSphereRadius / p_dot_p;
			}

			float GetFragmentDistance(float fragment_depth) 
			{

				float near = _ProjectionParams.y;
				float far = _ProjectionParams.z;
				fragment_depth = 1 - fragment_depth;
				float depth_sample = 2.0 * fragment_depth - 1.0;
    			float fragment_distance = 2.0 * near * far / (far + near - depth_sample * (far - near));
				//float fragment_distance = (fragment_depth * fragment_depth) * (far - near);

				return fragment_distance;
			}

			/*
			To compute light shafts we need the intersections of the view ray with the
			shadow volume of the sphere S. Since the Sun is not a punctual light source this
			shadow volume is not a cylinder but a cone (for the umbra, plus another cone for
			the penumbra, but we ignore it here):
			*/
			void GetSphereShadowInOut(float3 view_direction, float3 sun_direction, out float d_in, out float d_out)
			{
				float3 camera = _WorldSpaceCameraPos;
				float3 pos = camera - kSphereCenter;
				float pos_dot_sun = dot(pos, sun_direction);
				float view_dot_sun = dot(view_direction, sun_direction);
				float k = sun_size.x;
				float l = 1.0 + k * k;
				float a = 1.0 - l * view_dot_sun * view_dot_sun;
				float b = dot(pos, view_direction) - l * pos_dot_sun * view_dot_sun -
					k * kSphereRadius * view_dot_sun;
				float c = dot(pos, pos) - l * pos_dot_sun * pos_dot_sun -
					2.0 * k * kSphereRadius * pos_dot_sun - kSphereRadius * kSphereRadius;
				float discriminant = b * b - a * c;
				if (discriminant > 0.0) 
				{
					d_in = max(0.0, (-b - sqrt(discriminant)) / a);
					d_out = (-b + sqrt(discriminant)) / a;
					// The values of d for which delta is equal to 0 and kSphereRadius / k.
					float d_base = -pos_dot_sun / view_dot_sun;
					float d_apex = -(pos_dot_sun + kSphereRadius / k) / view_dot_sun;

					if (view_dot_sun > 0.0) 
					{
						d_in = max(d_in, d_apex);
						d_out = a > 0.0 ? min(d_out, d_base) : d_base;
					}
					else 
					{
						d_in = a > 0.0 ? max(d_in, d_base) : d_base;
						d_out = min(d_out, d_apex);
					}
				}
				else 
				{
					d_in = 0.0;
					d_out = 0.0;
				}
			}

#ifdef RADIANCE_API_ENABLED
			RadianceSpectrum GetSolarRadiance() 
			{
				return solar_irradiance / (PI * sun_angular_radius * sun_angular_radius);
			}

			RadianceSpectrum GetSkyRadiance(
				Position camera, Direction view_ray, Length shadow_length,
				Direction sun_direction, out DimensionlessSpectrum transmittance) 
			{
				return GetSkyRadiance(transmittance_texture,
					scattering_texture, single_mie_scattering_texture,
					camera, view_ray, shadow_length, sun_direction, transmittance);
			}

			RadianceSpectrum GetSkyRadianceToPoint(
				Position camera, Position _point, Length shadow_length,
				Direction sun_direction, out DimensionlessSpectrum transmittance) 
			{
				return GetSkyRadianceToPoint(transmittance_texture,
					scattering_texture, single_mie_scattering_texture,
					camera, _point, shadow_length, sun_direction, transmittance);
			}

			IrradianceSpectrum GetSunAndSkyIrradiance(
				Position p, Direction normal, Direction sun_direction,
				out IrradianceSpectrum sky_irradiance) 
			{
				return GetSunAndSkyIrradiance(transmittance_texture,
					irradiance_texture, p, normal, sun_direction, sky_irradiance);
			}
#else
			Luminance3 GetSolarRadiance()
			{
				return solar_irradiance /
					(PI * sun_angular_radius * sun_angular_radius) *
					SUN_SPECTRAL_RADIANCE_TO_LUMINANCE;
			}

			Luminance3 GetSkyRadiance(
				Position camera, Direction view_ray, Length shadow_length,
				Direction sun_direction, out DimensionlessSpectrum transmittance) 
			{
				return GetSkyRadiance(transmittance_texture,
					scattering_texture, single_mie_scattering_texture,
					camera, view_ray, shadow_length, sun_direction, transmittance) *
					SKY_SPECTRAL_RADIANCE_TO_LUMINANCE;
			}

			Luminance3 GetSkyRadianceToPoint(
				Position camera, Position _point, Length shadow_length,
				Direction sun_direction, out DimensionlessSpectrum transmittance) 
			{
				return GetSkyRadianceToPoint(transmittance_texture,
					scattering_texture, single_mie_scattering_texture,
					camera, _point, shadow_length, sun_direction, transmittance) *
					SKY_SPECTRAL_RADIANCE_TO_LUMINANCE;
			}

			Illuminance3 GetSunAndSkyIrradiance(
				Position p, Direction normal, Direction sun_direction,
				out IrradianceSpectrum sky_irradiance) 
			{
				IrradianceSpectrum sun_irradiance = GetSunAndSkyIrradiance(
					transmittance_texture, irradiance_texture, p, normal,
					sun_direction, sky_irradiance);
				sky_irradiance *= SKY_SPECTRAL_RADIANCE_TO_LUMINANCE;
				return sun_irradiance * SUN_SPECTRAL_RADIANCE_TO_LUMINANCE;
			}
#endif

			sampler2D _MainTex;
			sampler2D _LastCameraDepthTexture;
			sampler2D _CameraDepthTexture ;

			sampler2D _CameraDepthCustom;

			float Linear01Depth( float z )
			{
				return 1.0 / (_ZBufferParams.x * z + _ZBufferParams.y);
			}

			float4 frag(v2fATMOS i) : SV_Target
			{

				float3 accum = 0;

				float4 col = tex2D(_MainTex, i.uv);
				float2 fragment_size = 1 / _ScreenParams.xy;

				float3 camera = _WorldSpaceCameraPos;
				// Normalized view direction vector.
				float3 view_direction = normalize(i.view_ray);

				int fragment_ms = 0;

				float depthSample = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv.xy));//, _ZBufferParams);

				//Multi-Sampling depth texture for smoother transition
				/*
				float fragment_depth = tex2D(_CameraDepthCustom, i.uv - float2(0, fragment_size.y)).r;
				fragment_ms += fragment_depth != 0;
				fragment_depth = tex2D(_CameraDepthCustom, i.uv - float2(fragment_size.x, 0)).r;
				fragment_ms += fragment_depth != 0;
				fragment_depth = tex2D(_CameraDepthCustom, i.uv + float2(0, fragment_size.y)).r;
				fragment_ms += fragment_depth != 0;
				fragment_depth = tex2D(_CameraDepthCustom, i.uv + float2(fragment_size.x, 0)).r;
				fragment_ms += fragment_depth != 0;
				fragment_depth = tex2D(_CameraDepthCustom, i.uv).r;
				fragment_ms += fragment_depth != 0;
				*/
				//fragment_depth = fragment_ms = 0;

				
				//Multi-Sampling with 9 samples
				float fragment_depth = tex2D(_LastCameraDepthTexture, i.uv + fragment_size).r;
				fragment_ms += fragment_depth != 0;

				fragment_depth = tex2D(_LastCameraDepthTexture, i.uv + float2(0, fragment_size.y)).r;
				fragment_ms += fragment_depth != 0;

				fragment_depth = tex2D(_LastCameraDepthTexture, i.uv + float2(fragment_size.x, 0)).r;
				fragment_ms += fragment_depth != 0;

				fragment_depth = tex2D(_LastCameraDepthTexture, i.uv + float2(-fragment_size.x, fragment_size.y)).r;
				fragment_ms += fragment_depth != 0;

				fragment_depth = tex2D(_LastCameraDepthTexture, i.uv - fragment_size).r;
				fragment_ms += fragment_depth != 0;

				fragment_depth = tex2D(_LastCameraDepthTexture, i.uv - float2(0, fragment_size.y)).r;
				fragment_ms += fragment_depth != 0;

				fragment_depth = tex2D(_LastCameraDepthTexture, i.uv - float2(fragment_size.x, 0)).r;
				fragment_ms += fragment_depth != 0;

				fragment_depth = tex2D(_LastCameraDepthTexture, i.uv + float2(fragment_size.x, -fragment_size.y)).r;
				fragment_ms += fragment_depth != 0;

				fragment_depth = tex2D(_LastCameraDepthTexture, i.uv).r;
				fragment_ms += fragment_depth != 0;
				
				//>6

				//return float4(1,1,1,1)*depthSample * 0.05;
				//return float4(1,1,1,1)*fragment_depth * 20110;

				float shadow_in;
				float shadow_out;
				GetSphereShadowInOut(view_direction, sun_direction, shadow_in, shadow_out);
				float fade = 1.0;
				for (int i1 = 0; i1 < 2; i1++) {
							
									

					

					// Hack to fade out light shafts when the Sun is very close to the horizon.
					float lightshaft_fadein_hack = smoothstep(
						0.02, 0.04, dot(normalize(camera - earth_center), sun_direction));

					// Compute the radiance of the sky.
					float shadow_length = max(0.0, shadow_out - shadow_in) * lightshaft_fadein_hack;
					float3 transmittance;
					float3 radiance = GetSkyRadiance(camera - earth_center, view_direction, shadow_length, sun_direction, transmittance);



					//v0.2
				/*	if (i1==0) {
						skyControlsPlanetB.x = 1;
					}
					if (length(camera - earth_center) > 639100 * 2.3625 * skyControlsPlanetB.y && i1==1) {
						skyControlsPlanetB.x = 1;
					}*/

					
					bool add_background_color = false;

					float3 ground_radiance = float3(0, 0, 0);

					// Compute the radiance reflected by the ground, if the ray intersects it.
					if (fragment_ms > 9 )//4)//if (fragment_ms > 6)//4) //depthSample < 0.0015)//  
					{
						float fragment_distance = LinearEyeDepth(fragment_depth);
						float3 fragment_in_worldspace = i.worldDirection * fragment_distance + camera;
						float planet_radius_at_fragment = length(fragment_in_worldspace - earth_center);

						// Compute the distance between the view ray line and the Earth center,
						// and the distance between the camera and the intersection of the view
						// ray with the ground (or NaN if there is no intersection).
						float3 p = camera - earth_center;
						float p_dot_v = dot(p, view_direction);
						float p_dot_p = dot(p, p);
						float ray_earth_center_squared_distance = p_dot_p - p_dot_v * p_dot_v;
						float distance_to_intersection = -p_dot_v - sqrt(planet_radius_at_fragment * planet_radius_at_fragment - ray_earth_center_squared_distance);

						float3 normal = normalize(fragment_in_worldspace - earth_center);

						// Compute the radiance reflected by the ground.
						float3 sky_irradiance;
						float3 sun_irradiance = GetSunAndSkyIrradiance(fragment_in_worldspace - earth_center, normal, sun_direction, sky_irradiance);

						float sunVis = GetSunVisibility(fragment_in_worldspace, sun_direction);
						float skyVis = GetSkyVisibility(fragment_in_worldspace);

						ground_radiance = (col * _Brightness) * (1.0 / PI) * (sun_irradiance * sunVis + sky_irradiance * skyVis);

						float shadow_length = max(0.0, min(shadow_out, distance_to_intersection) - shadow_in) * lightshaft_fadein_hack;

						float3 transmittance;
						float3 in_scatter = GetSkyRadianceToPoint((camera - earth_center) * skyControlsA.x * skyControlsPlanetB.x, (fragment_in_worldspace - earth_center) * skyControlsA.y, shadow_length, sun_direction, transmittance);

						radiance = ground_radiance * transmittance + in_scatter;

						if (_Fade == 1) {
							fade = clamp((fragment_distance - _FadeStart) / _FadeEnd, 0, 1);
						}

						if (_Fade == 2)
						{
							float altitude = length(camera - earth_center) - bottom_radius;
							fade = clamp((altitude - _FadeStart) / _FadeEnd, 0, 1);
						}
						//ground_radiance = 0;
						//radiance = 1;
						//ground_radiance *= 100;
					}
					
					

					radiance = pow(float3(1, 1, 1) - exp(-radiance / white_point * exposure * _blendControls.w), 1.0 / 2.2);
				//	radiance = lerp(col, radiance, fade);


					if ((length(accum.rgb) > 1.35112 * 1 ||   (depthSample < 0.5 * 0.1 && length(camera - earth_center) > 639100*2.4)  ) && i1 == 1)//  
					{
						radiance = 0;
					}

					//return float4(radiance*1,1);
					earth_center = earth_center + float3(0, 1500110, 1500110);
					//return float4(i.view_ray, 1);
					//return float4(i.worldDirection, 1);
					//return float4(1,0, i.uv.y, 1);
					accum += radiance;
					//saturate(float4(radiance * _blendControls.x + radiance * col * _blendControls.y / divideSky + 4 * _blendControls.z * (1 - radiance) * col / divideSky, 1));
				}

				float4 accumR = saturate(float4(accum * _blendControls.x + accum * col * _blendControls.y / divideSky + 4 * _blendControls.z * (1 - accum) * col / divideSky, 1));
				//float4 accumR = saturate(float4(accum * _blendControls.x + accum * col * 1 / divideSky + 4 * 1 * (1 - accum) * col / divideSky, 1));

				return accumR;
				//return float4(radiance +radiance*col/divideSky + 4*(1-radiance)*col/divideSky, 1);
				//return saturate(float4(radiance * _blendControls.x +radiance*col* _blendControls.y/divideSky + 4* _blendControls.z*(1-radiance)*col/divideSky, 1));
			}

			ENDCG//ENDHLSL//ENDCG
		}


		///////
	}
}
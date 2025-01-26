// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Skybox/Procedural_SkyMaster_Galaxy" {
Properties {
	_SunSize ("Sun Size", Range (0.0, 30.0)) = 1
	_SunTint ("Sun Tint", Color) = (1, .925, .737, 1)
	_SkyExponent ("Sky Gradient", Float) = 1.5
	_SkyTopColor ("Sky Top", Color) = (.008, .296, .586, 1)
	_SkyMidColor ("Sky Middle", Color) = (.570, .734, 1, 1)
	_SkyEquatorColor ("Sky Equator", Color) = (.917, .992, 1, 1)
	_GroundColor ("Ground", Color) = (.369, .349, .341, 1)	

	//v0.1
		_Color("Light Color", Color) = (1,1,1,1)
		_MainTexGalaxy("Base (RGB)", 2D) = "white" {}
		_Jitter("Jitter", 2D) = "white" {}
		_SunDir("Sun Direction", Vector) = (0,1,0,0)
		_CloudCover("Cloud Cover", Range(0,1)) = 0.5
		_CloudSharpness("Cloud Sharpness", Range(1,30)) = 8
		_CloudDensity("Density", Range(0,5)) = 1
		_CloudSpeed("Cloud Speed", Vector) = (0.001, 0, 0, 0)
		_Light("Sun Intensity", Range(0,10)) = 1
		_Bump("Bump", 2D) = "white" {}

		_galaxySpeedHor("Galaxy rotation Horizontal", Float) = 0
		_galaxySpeedVer("Galaxy rotation Vertical", Float) = 0
}

SubShader {
	Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
	Cull Off ZWrite Off

	Pass {
		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		//#pragma target 3.0
		//#pragma glsl	


			//v0.1a
				float _galaxySpeedHor;
				float _galaxySpeedVer;

				//v0.1
				sampler2D _MainTexGalaxy;
				float4 _MainTexGalaxy_ST;
				//float4 _Bump_ST;
				sampler2D _Jitter;
				//sampler2D _Bump;
				float4 _Color;
				float4 _Ambient;
				float4 _SunDir;
				float4 _CloudSpeed;
				float _Light;
				float _CloudCover;
				uniform float _CloudDensity;
				float _CloudSharpness;

				//v0.1
			//https://docs.unity3d.com/Packages/com.unity.shadergraph@6.9/manual/Rotate-About-Axis-Node.html
				void RotateAboutAxis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
				{
					Rotation = radians(Rotation);
					float s = sin(Rotation);
					float c = cos(Rotation);
					float one_minus_c = 1.0 - c;

					Axis = normalize(Axis);
					float3x3 rot_mat =
					{ one_minus_c * Axis.x * Axis.x + c, one_minus_c * Axis.x * Axis.y - Axis.z * s, one_minus_c * Axis.z * Axis.x + Axis.y * s,
						one_minus_c * Axis.x * Axis.y + Axis.z * s, one_minus_c * Axis.y * Axis.y + c, one_minus_c * Axis.y * Axis.z - Axis.x * s,
						one_minus_c * Axis.z * Axis.x - Axis.y * s, one_minus_c * Axis.y * Axis.z + Axis.x * s, one_minus_c * Axis.z * Axis.z + c
					};
					Out = mul(rot_mat, In);
				}


#define PI 3.141592653589793	


	
		half _SunSize =1;
		half4 _SunTint;
		half _SkyExponent;
		half4 _SkyTopColor;
		half4 _SkyEquatorColor;
		half4 _SkyMidColor;
		half4 _GroundColor;
		
			struct fragIO 
			{				
    			float4 pos : SV_POSITION;    			
    			float2 uv : TEXCOORD1;
    			half4 normalAndSunExp : TEXCOORD2;  
				float3 v3DirectionA : TEXCOORD3;
			};		

		fragIO vert (appdata_base v)
		{				
		    fragIO OUTPUT;
    		OUTPUT.pos = UnityObjectToClipPos(v.vertex);
    		OUTPUT.uv = v.texcoord.xy;
			OUTPUT.normalAndSunExp.xyz = normalize(mul((float3x3)unity_ObjectToWorld, v.vertex.xyz));
			OUTPUT.normalAndSunExp.w = (_SunSize > 0)? (256.0/_SunSize) : 0.0;

			//v0.1
			OUTPUT.v3DirectionA = mul(unity_ObjectToWorld, v.vertex).xyz - _WorldSpaceCameraPos;

			return OUTPUT;
		}	
 	
		fixed4 frag (fragIO IN) : COLOR // SV_Target
		{	
			half3 normal = normalize(IN.normalAndSunExp.xyz);
			half t = normal.y;

			half3 sunColor =  _LightColor0.rgb * 2 * _SunTint * 2 *_SunSize;
			half3 sunDir =_WorldSpaceLightPos0.xyz; //v3LightDir;//
			half3 sun = (IN.normalAndSunExp.w > 0) ? pow(max(0.0, dot(normal, sunDir)), IN.normalAndSunExp.w) : 0.0;

			half3 c; 
			if (t > 0)
			{
				half skyT = 1-pow (1-t, _SkyExponent);
				if (skyT < 0.25){
					c = lerp (_SkyEquatorColor.rgb, _SkyMidColor.rgb,skyT*4);					
				}
				else{
					c = lerp (_SkyMidColor.rgb, _SkyTopColor.rgb, (skyT-0.25)*(4.0/3.0));	
					}
			}
			else
			{
				half groundT = 1-pow (1+t, 10.0);
				c = lerp (_SkyEquatorColor.rgb, _GroundColor.rgb, groundT);
				sun *= (1-groundT);
			}

			c = lerp(c, max(c, sunColor), sun);


			//v0.1				
			float3 pos = normalize(IN.v3DirectionA);
			//v0.1a
			RotateAboutAxis_Degrees_float(pos, float3(1, 0, 0), _galaxySpeedVer * 0.001 * _Time.y, pos);
			float2 newUV;
			newUV.x = 0.5 + atan2(pos.z, pos.x) / (PI * 2);
			newUV.y = 0.5 - asin(pos.y) / PI;
			float2 dx = ddx(newUV);
			float2 dy = ddy(newUV);
			float2 du = float2(dx.x, dy.x);
			du -= (abs(du) > 0.5f) * sign(du);
			dx.x = du.x;
			dy.x = du.y;
			newUV.x += _MainTexGalaxy_ST.z;
			//float4  galaxy = tex2Dgrad(_MainTexGalaxy, newUV, dx, dy);
			float2 offset = _Time.y * _CloudSpeed.xy;
			float4  tex = tex2Dgrad(_MainTexGalaxy, newUV * _MainTexGalaxy_ST.xy + float2(_galaxySpeedHor * 0.001, 0) * _Time.y, dx, dy);
			float Bumped_lightingN = 1;
			float4 tex2N = tex2D(_Jitter, (newUV + offset / 15) * _CloudDensity * 11);
			tex = max(tex - (1 - _CloudCover * 2), 0);
			float4 res = _Color.a * lerp(pow(tex2N, 2), 0.6, _CloudSharpness)
				* float4 (0.95 * _Color.r * Bumped_lightingN * tex.r,
					0.95 * _Color.g * Bumped_lightingN * tex.g,
					0.95 * _Color.b * Bumped_lightingN * tex.b,
					_Color.a);
			float4 galaxy = pow(abs(res), _Light);



			return half4(c, 1) + galaxy;
		}
		ENDCG 
	}
} 	

Fallback Off

}

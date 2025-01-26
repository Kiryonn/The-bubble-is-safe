Shader "Hidden/NebulaBlender"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Cull Off ZWrite Off ZTest Off
		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 3.0

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 pos : SV_POSITION;
	};

	sampler2D _MainTex;
	sampler2D _Clouds;
	float4 _MainTex_TexelSize;
	float4 _Clouds_TexelSize;//v5.1.1
	int enableEdges;
	float4 edgeControl;

	//v0.6
	float4 controlCloudEdgeA;// = float4(0.65, 1.22, 1.14, 1.125);
	float controlCloudAlphaPower;// = 2;
	float controlBackAlphaPower;
	float depthDilation;
	uniform sampler2D_float _CameraDepthTexture;

	//BACKGROUND BLEND MODE
	float backgroundOnly;

	v2f vert(appdata v)
	{
		v2f o;

		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv.xy;

#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			o.uv.y = 1 - o.uv.y;
#endif

		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		//fixed4 back = tex2D(_MainTex, float2(i.uv.x, i.uv.y * 0.9978));// i.uv); // sample background //v5.1.0
		//fixed4 cloud = tex2D(_Clouds, i.uv); // sample cloud rendertexture
		//return fixed4(back.rgb * (1.0 - cloud.a) + cloud.rgb, 1.0); // blend them

		float4 back = tex2D(_MainTex, float2(i.uv.x, i.uv.y));// i.uv); // sample background //v5.1.0
		float4 cloud = tex2D(_Clouds, i.uv); // sample cloud rendertexture

		//if (enableEdges == 11) {
		//	float texel_offset = 0.3 * edgeControl.x * 5;
		//	float thres = 0.04 * edgeControl.y * 0.15 * 2.1;
		//	//float thresA = 0.88;
		//	float increase = 20;
		//	float transp = edgeControl.z * 1.7;// cloud.a;
		//	float divider = 4;

		//	float4 up = tex2D(_Clouds, i.uv + float2(0, _Clouds_TexelSize.y*texel_offset));//v5.1.1
		//	float4 down = tex2D(_Clouds, i.uv - float2(0, _Clouds_TexelSize.y*texel_offset));
		//	float4 left = tex2D(_Clouds, i.uv - float2(_Clouds_TexelSize.x*texel_offset, 0));
		//	float4 right = tex2D(_Clouds, i.uv + float2(_Clouds_TexelSize.x*texel_offset, 0));//v5.1.1

		//	float4 upL = tex2D(_Clouds, i.uv + float2(_Clouds_TexelSize.x*texel_offset, _MainTex_TexelSize.y*texel_offset));//v5.1.1
		//	float4 upR = tex2D(_Clouds, i.uv + float2(_Clouds_TexelSize.x*texel_offset, -_MainTex_TexelSize.y*texel_offset));
		//	float4 downL = tex2D(_Clouds, i.uv + float2(-_Clouds_TexelSize.x*texel_offset, _MainTex_TexelSize.y*texel_offset));
		//	float4 downR = tex2D(_Clouds, i.uv + float2(-_Clouds_TexelSize.x*texel_offset, -_MainTex_TexelSize.y*texel_offset));//v5.1.1


		//	if ((cloud.r < thres && cloud.g < thres && cloud.b < thres) && (upL.r > increase*thres || upL.g > increase*thres || upL.b > increase*thres)) {
		//		cloud.rgb = (cloud + up + down + left + right + upL + upR + downL + downR) / divider;
		//		cloud.a = transp * (cloud.a + up.a + down.a + left.a + right.a + upL.a + upR.a + downL.a + downR.a) / 8;
		//	}
		//	else if ((cloud.r < thres && cloud.g < thres && cloud.b < thres) && (upR.r > increase*thres || upR.g > increase*thres || upR.b > increase*thres)) {
		//		cloud.rgb = (cloud + up + down + left + right + upL + upR + downL + downR) / divider;
		//		cloud.a = transp * (cloud.a + up.a + down.a + left.a + right.a + upL.a + upR.a + downL.a + downR.a) / 8;
		//	}
		//	else if ((cloud.r < thres && cloud.g < thres && cloud.b < thres) && (downL.r > increase*thres || downL.g > increase*thres || downL.b > increase*thres)) {
		//		cloud.rgb = (cloud + up + down + left + right + upL + upR + downL + downR) / divider;
		//		cloud.a = transp * (cloud.a + up.a + down.a + left.a + right.a + upL.a + upR.a + downL.a + downR.a) / 8;
		//	}
		//	else if ((cloud.r < thres && cloud.g < thres && cloud.b < thres) && (downR.r > increase*thres || downR.g > increase*thres || downR.b > increase*thres)) {
		//		cloud.rgb = (cloud + up + down + left + right + upL + upR + downL + downR) / divider;
		//		cloud.a = transp * (cloud.a + up.a + down.a + left.a + right.a + upL.a + upR.a + downL.a + downR.a) / 8;
		//	}
		//	else if ((cloud.r < thres && cloud.g < thres && cloud.b < thres) && (up.r > increase*thres || up.g > increase*thres || up.b > increase*thres)) {
		//		cloud.rgb = (cloud + up + down + left + right + upL + upR + downL + downR) / divider;
		//		cloud.a = transp * (cloud.a + up.a + down.a + left.a + right.a + upL.a + upR.a + downL.a + downR.a) / 8;// transp * (cloud.r + 0.44);
		//	}
		//	else if ((cloud.r < thres && cloud.g < thres && cloud.b < thres) && (left.r > increase*thres || left.g > increase*thres || left.b > increase*thres)) {
		//		cloud.rgb = (cloud + up + down + left + right + upL + upR + downL + downR) / divider;
		//		cloud.a = transp * (cloud.a + up.a + down.a + left.a + right.a + upL.a + upR.a + downL.a + downR.a) / 8;
		//	}
		//	else if ((cloud.r < thres && cloud.g < thres && cloud.b < thres) && (right.r > increase*thres || right.g > increase*thres || right.b > increase*thres)) {
		//		cloud.rgb = (cloud + up + down + left + right + upL + upR + downL + downR) / divider;
		//		cloud.a = transp * (cloud.a + up.a + down.a + left.a + right.a + upL.a + upR.a + downL.a + downR.a) / 8;
		//	}
		//	else if ((cloud.r < thres && cloud.g < thres && cloud.b < thres) && (down.r > increase*thres || down.g > increase*thres || down.b > increase*thres)) {
		//		cloud.rgb = (cloud + up + down + left + right + upL + upR + downL + downR) / divider;
		//		cloud.a = transp * (cloud.a + up.a + down.a + left.a + right.a + upL.a + upR.a + downL.a + downR.a) / 8;
		//	}
		//}

		//if (enableEdges == 1) {
		//	float texel_offset = 0.3 * edgeControl.x;
		//	float thres = 0.04 * edgeControl.y * 0.15;
		//	float thresA = 0.88 * edgeControl.z;
		//	float increase = 0;
		//	float transp = 1;// cloud.a;


		//	float4 up = tex2D(_Clouds, i.uv + fixed2(0, _Clouds_TexelSize.y*texel_offset));//v5.1.1
		//	float4 down = tex2D(_Clouds, i.uv - fixed2(0, _Clouds_TexelSize.y*texel_offset));
		//	float4 left = tex2D(_Clouds, i.uv - fixed2(_Clouds_TexelSize.x*texel_offset, 0));
		//	float4 right = tex2D(_Clouds, i.uv + fixed2(_Clouds_TexelSize.x*texel_offset, 0));//v5.1.1

		//	float4 upL = tex2D(_Clouds, i.uv + fixed2(_Clouds_TexelSize.x*texel_offset, _Clouds_TexelSize.y*texel_offset));//v5.1.1
		//	float4 upR = tex2D(_Clouds, i.uv + fixed2(_Clouds_TexelSize.x*texel_offset, -_Clouds_TexelSize.y*texel_offset));
		//	float4 downL = tex2D(_Clouds, i.uv + fixed2(-_Clouds_TexelSize.x*texel_offset, _Clouds_TexelSize.y*texel_offset));
		//	float4 downR = tex2D(_Clouds, i.uv + fixed2(-_Clouds_TexelSize.x*texel_offset, -_Clouds_TexelSize.y*texel_offset));//v5.1.1


		//	if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (upL.r < thres && upL.g < thres && upL.b < thres)) {
		//		//cloud = upL * increase; 
		//		cloud.a = transp;
		//	}
		//	else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (upR.r < thres && upR.g < thres && upR.b < thres)) {
		//		//cloud = upR * increase; 
		//		cloud.a = transp;
		//	}
		//	else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (downL.r < thres && downL.g < thres && downL.b < thres)) {
		//		//cloud = downL * increase; 
		//		cloud.a = transp;
		//	}
		//	else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (downR.r < thres && downR.g < thres && downR.b < thres)) {
		//		//cloud = downR * increase; 
		//		cloud.a = transp;
		//	}
		//	else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (up.r < thres && up.g < thres && up.b < thres)) {
		//		//cloud =  up * increase; 
		//		cloud.a = transp;
		//	}
		//	else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (down.r < thres && down.g < thres && down.b < thres)) {
		//		//cloud = down * increase; 
		//		cloud.a = transp;
		//	}
		//	else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (left.r < thres && left.g < thres && left.b < thres)) {
		//		//cloud =  left * increase; 
		//		cloud.a = transp;
		//	}
		//	else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (right.r < thres && right.g < thres && right.b < thres)) {
		//		//cloud = right * increase; 
		//		cloud.a = transp;
		//	}

		//	texel_offset = 1.2* edgeControl.x;
		//	increase = 1;
		//	transp = 3.05* edgeControl.w;

		//	up = tex2D(_Clouds, i.uv + fixed2(0, _Clouds_TexelSize.y*texel_offset));//v5.1.1
		//	down = tex2D(_Clouds, i.uv - fixed2(0, _Clouds_TexelSize.y*texel_offset));
		//	left = tex2D(_Clouds, i.uv - fixed2(_Clouds_TexelSize.x*texel_offset, 0));
		//	right = tex2D(_Clouds, i.uv + fixed2(_Clouds_TexelSize.x*texel_offset, 0));//v5.1.1

		//	upL = tex2D(_Clouds, i.uv + fixed2(_Clouds_TexelSize.x*texel_offset, _Clouds_TexelSize.y*texel_offset));//v5.1.1
		//	upR = tex2D(_Clouds, i.uv + fixed2(_Clouds_TexelSize.x*texel_offset, -_Clouds_TexelSize.y*texel_offset));
		//	downL = tex2D(_Clouds, i.uv + fixed2(-_Clouds_TexelSize.x*texel_offset, _Clouds_TexelSize.y*texel_offset));
		//	downR = tex2D(_Clouds, i.uv + fixed2(-_Clouds_TexelSize.x*texel_offset, -_Clouds_TexelSize.y*texel_offset));//v5.1.1


		//	if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (upL.r < thres && upL.g < thres && upL.b < thres)) {
		//		//cloud = back * increase;
		//		cloud.a = transp * (cloud.r + 0.14);
		//	}
		//	else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (upR.r < thres && upR.g < thres && upR.b < thres)) {
		//		//cloud = cloud * increase;
		//		cloud.a = transp * (cloud.r + 0.14);
		//	}
		//	else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (downL.r < thres && downL.g < thres && downL.b < thres)) {
		//		//cloud = cloud * increase;
		//		cloud.a = transp * (cloud.r + 0.14);
		//	}
		//	else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (downR.r < thres && downR.g < thres && downR.b < thres)) {
		//		//cloud = cloud * increase;
		//		cloud.a = transp * (cloud.r + 0.14);
		//	}
		//	else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (up.r < thres && up.g < thres && up.b < thres)) {
		//		//cloud = cloud * increase;
		//		cloud.a = transp * (cloud.r + 0.14);
		//	}
		//	else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (down.r < thres && down.g < thres && down.b < thres)) {
		//		//cloud = cloud * increase;
		//		cloud.a = transp * (cloud.r + 0.14);
		//	}
		//	else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (left.r < thres && left.g < thres && left.b < thres)) {
		//		//cloud = cloud * increase;
		//		cloud.a = transp * (cloud.r + 0.14);
		//	}
		//	else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (right.r < thres && right.g < thres && right.b < thres)) {
		//		//cloud = cloud * increase;
		//		cloud.a = transp * (cloud.r + 0.14);
		//	}
		//}

		//v0.6
		//float4 controlCloudEdgeA = float4(0.65, 1.22, 1.14, 1.125);// (1, 1, 1, 1);//(0.65, 1.22, 1.14, 1.125)
		//float controlCloudAlphaPower = 2; //2 //0
		//float controlBackAlphaPower = 2;  //2 //1
		//float depthDilation = 450;
		//v0.6
		//float zsample = Linear01DepthA(i.uv.xy);
		//float depth = Linear01Depth(zsample * (zsample < 1.0), _ZBufferParams);
		float2 duv = i.uv;
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0) {
			duv.y = 1 - duv.y;
		}
		#endif
		float depth = Linear01Depth(tex2D(_CameraDepthTexture, duv).r);
		//return float4(back.rgb * (1.0 - cloud.a) + cloud.rgb, 1.0)*depth + float4((1 - depth) * back.rgb, 0); // blend them

		//BACKGROUND MODE
		if (backgroundOnly > 0 && depth < backgroundOnly) {
			return back;
		}

		if (depthDilation == 0) {
			depth = 1;
		}
		return (controlCloudEdgeA.x*float4(back.rgb* (controlCloudEdgeA.y - pow(cloud.a, controlBackAlphaPower)), 1)
			+ controlCloudEdgeA.z*float4(cloud.rgb* pow(cloud.a, controlCloudAlphaPower + 0.001), 1))
			*controlCloudEdgeA.w *depth + float4((1 - depth) * back.rgb, 1);

		//return  float4(back.rgb * (1.0 - cloud.a) + cloud.rgb, 1.0); // blend them
		//return  float4(back.rgb * (1.0 - cloud.a) + cloud.rgb * cloud.a, 1.0); // blend them //v0.6

	}
		ENDCG
	}



	//PASS 1
	Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 3.0

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 pos : SV_POSITION;
	};

	sampler2D _MainTex;
	sampler2D _Clouds;
	float4 _MainTex_TexelSize;
	float4 _Clouds_TexelSize;//v5.1.1
	int enableEdges;
	float4 edgeControl;

	v2f vert(appdata v)
	{
		v2f o;

		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv.xy;

#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			o.uv.y = 1 - o.uv.y;
#endif

		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		///fixed4 back = tex2D(_MainTex, float2(i.uv.x, i.uv.y * 0.9978));// i.uv); // sample background //v5.1.0
		//fixed4 cloud = tex2D(_Clouds, i.uv); // sample cloud rendertexture
		//return fixed4(back.rgb * (1.0 - cloud.a) + cloud.rgb, 1.0); // blend them

		//float4 back = tex2D(_MainTex, float2(i.uv.x, i.uv.y));// i.uv); // sample background //v5.1.0
		float4 cloud = tex2D(_MainTex,  float2(i.uv.x, i.uv.y)); // sample cloud rendertexture

		float alpha = cloud.a;

		if (enableEdges == 1) {
			float texel_offset = 0.3 * edgeControl.x * 5;
			float thres = 0.04 * edgeControl.y * 0.15 * 2.1;
			//float thresA = 0.88;
			float increase = 20;
			float transp = edgeControl.z * 1.7;// cloud.a;
			float divider = 4;

			float4 up = tex2D(_MainTex, i.uv + float2(0, _MainTex_TexelSize.y*texel_offset));//v5.1.1
			float4 down = tex2D(_MainTex, i.uv - float2(0, _MainTex_TexelSize.y*texel_offset));
			float4 left = tex2D(_MainTex, i.uv - float2(_MainTex_TexelSize.x*texel_offset, 0));
			float4 right = tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x*texel_offset, 0));//v5.1.1

			float4 upL = tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x*texel_offset, _MainTex_TexelSize.y*texel_offset));//v5.1.1
			float4 upR = tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x*texel_offset, -_MainTex_TexelSize.y*texel_offset));
			float4 downL = tex2D(_MainTex, i.uv + float2(-_MainTex_TexelSize.x*texel_offset, _MainTex_TexelSize.y*texel_offset));
			float4 downR = tex2D(_MainTex, i.uv + float2(-_MainTex_TexelSize.x*texel_offset, -_MainTex_TexelSize.y*texel_offset));//v5.1.1

			
			if ((cloud.r < thres && cloud.g < thres && cloud.b < thres) && (upL.r > increase*thres || upL.g > increase*thres || upL.b > increase*thres) ) {
				cloud.rgb = (cloud + up + down + left + right + upL + upR + downL + downR) / divider;
				cloud.a = transp*(cloud.a + up.a + down.a + left.a + right.a + upL.a + upR.a + downL.a + downR.a) / 8;
			}
			else if ((cloud.r < thres && cloud.g < thres && cloud.b < thres) && (upR.r > increase*thres || upR.g > increase*thres || upR.b > increase*thres)) {
				cloud.rgb = (cloud + up + down + left + right + upL + upR + downL + downR) / divider;
				cloud.a = transp * (cloud.a + up.a + down.a + left.a + right.a + upL.a + upR.a + downL.a + downR.a) / 8;
			}
			else if ((cloud.r < thres && cloud.g < thres && cloud.b < thres) && (downL.r > increase*thres || downL.g > increase*thres || downL.b > increase*thres)) {
				cloud.rgb = (cloud + up + down + left + right + upL + upR + downL + downR) / divider;
				cloud.a = transp * (cloud.a + up.a + down.a + left.a + right.a + upL.a + upR.a + downL.a + downR.a) / 8;
			}
			else if ((cloud.r < thres && cloud.g < thres && cloud.b < thres) && (downR.r > increase*thres || downR.g > increase*thres || downR.b > increase*thres)) {
				cloud.rgb = (cloud + up + down + left + right + upL + upR + downL + downR) / divider;
				cloud.a = transp * (cloud.a + up.a + down.a + left.a + right.a + upL.a + upR.a + downL.a + downR.a) / 8;
			}
			else if ((cloud.r < thres && cloud.g < thres && cloud.b < thres) && (up.r > increase*thres || up.g > increase*thres || up.b > increase*thres)) {
				cloud.rgb = (cloud + up + down + left + right + upL + upR + downL + downR) / divider;
				cloud.a = transp * (cloud.a + up.a + down.a + left.a + right.a + upL.a + upR.a + downL.a + downR.a) / 8;// transp * (cloud.r + 0.44);
			}
			else if ((cloud.r < thres && cloud.g < thres && cloud.b < thres) && (left.r > increase*thres || left.g > increase*thres || left.b > increase*thres)) {
				cloud.rgb = (cloud + up + down + left + right + upL + upR + downL + downR) / divider;
				cloud.a = transp * (cloud.a + up.a + down.a + left.a + right.a + upL.a + upR.a + downL.a + downR.a) / 8;
			}
			else if ((cloud.r < thres && cloud.g < thres && cloud.b < thres) && (right.r > increase*thres || right.g > increase*thres || right.b > increase*thres)) {
				cloud.rgb = (cloud + up + down + left + right + upL + upR + downL + downR) / divider;
				cloud.a = transp * (cloud.a + up.a + down.a + left.a + right.a + upL.a + upR.a + downL.a + downR.a) / 8;
			}
			else if ((cloud.r < thres && cloud.g < thres && cloud.b < thres) && (down.r > increase*thres || down.g > increase*thres || down.b > increase*thres)) {
				cloud.rgb = (cloud + up + down + left + right + upL + upR + downL + downR) / divider;
				cloud.a = transp * (cloud.a + up.a + down.a + left.a + right.a + upL.a + upR.a + downL.a + downR.a) / 8;
			}
			//cloud.a = alpha;

			//if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (upL.r < thres && upL.g < thres && upL.b < thres)) {
			//	//cloud = upL * increase; 
			//	cloud.a = transp;
			//}
			//else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (upR.r < thres && upR.g < thres && upR.b < thres)) {
			//	//cloud = upR * increase; 
			//	cloud.a = transp;
			//}
			//else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (downL.r < thres && downL.g < thres && downL.b < thres)) {
			//	//cloud = downL * increase; 
			//	cloud.a = transp;
			//}
			//else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (downR.r < thres && downR.g < thres && downR.b < thres)) {
			//	//cloud = downR * increase; 
			//	cloud.a = transp;
			//}
			//else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (up.r < thres && up.g < thres && up.b < thres)) {
			//	//cloud =  up * increase; 
			//	cloud.a = transp;
			//}
			//else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (down.r < thres && down.g < thres && down.b < thres)) {
			//	//cloud = down * increase; 
			//	cloud.a = transp;
			//}
			//else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (left.r < thres && left.g < thres && left.b < thres)) {
			//	//cloud =  left * increase; 
			//	cloud.a = transp;
			//}
			//else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (right.r < thres && right.g < thres && right.b < thres)) {
			//	//cloud = right * increase; 
			//	cloud.a = transp;
			//}

			//texel_offset = 1.2* edgeControl.x;
			//increase = 1;
			//transp = 3.05* edgeControl.w;

			//up = tex2D(_MainTex, i.uv + float2(0, _MainTex_TexelSize.y*texel_offset));//v5.1.1
			//down = tex2D(_MainTex, i.uv - float2(0, _MainTex_TexelSize.y*texel_offset));
			//left = tex2D(_MainTex, i.uv - float2(_MainTex_TexelSize.x*texel_offset, 0));
			//right = tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x*texel_offset, 0));//v5.1.1

			//upL = tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x*texel_offset, _MainTex_TexelSize.y*texel_offset));//v5.1.1
			//upR = tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x*texel_offset, -_MainTex_TexelSize.y*texel_offset));
			//downL = tex2D(_MainTex, i.uv + float2(-_MainTex_TexelSize.x*texel_offset, _MainTex_TexelSize.y*texel_offset));
			//downR = tex2D(_MainTex, i.uv + float2(-_MainTex_TexelSize.x*texel_offset, -_MainTex_TexelSize.y*texel_offset));//v5.1.1


			//if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (upL.r < thres && upL.g < thres && upL.b < thres)) {
			//	//cloud = back * increase;
			//	cloud.a = transp * (cloud.r + 0.14);
			//}
			//else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (upR.r < thres && upR.g < thres && upR.b < thres)) {
			//	//cloud = cloud * increase;
			//	cloud.a = transp * (cloud.r + 0.14);
			//}
			//else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (downL.r < thres && downL.g < thres && downL.b < thres)) {
			//	//cloud = cloud * increase;
			//	cloud.a = transp * (cloud.r + 0.14);
			//}
			//else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (downR.r < thres && downR.g < thres && downR.b < thres)) {
			//	//cloud = cloud * increase;
			//	cloud.a = transp * (cloud.r + 0.14);
			//}
			//else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (up.r < thres && up.g < thres && up.b < thres)) {
			//	//cloud = cloud * increase;
			//	cloud.a = transp * (cloud.r + 0.14);
			//}
			//else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (down.r < thres && down.g < thres && down.b < thres)) {
			//	//cloud = cloud * increase;
			//	cloud.a = transp * (cloud.r + 0.14);
			//}
			//else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (left.r < thres && left.g < thres && left.b < thres)) {
			//	//cloud = cloud * increase;
			//	cloud.a = transp * (cloud.r + 0.14);
			//}
			//else if (cloud.a < thresA && (cloud.r > thres && cloud.g > thres && cloud.b > thres) && (right.r < thres && right.g < thres && right.b < thres)) {
			//	//cloud = cloud * increase;
			//	cloud.a = transp * (cloud.r + 0.14);
			//}

			//float thres3 = 0.16;
			//if (cloud.r < thres && cloud.g < thres && cloud.b < thres) {

			//	//cloud = float4(0, 0, 0, cloud.a);
			//}
		}

		return cloud;// float4(back.rgb * (1.0 - cloud.a) + cloud.rgb, 1.0); // blend them

	}
		ENDCG
	}




	}
}

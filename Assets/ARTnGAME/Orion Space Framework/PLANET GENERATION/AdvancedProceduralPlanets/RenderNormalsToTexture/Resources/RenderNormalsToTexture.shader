﻿﻿Shader "Custom/Render Normals To Texture" {
	SubShader{
		Pass{
		ZWrite On
		ZTest Always
		Cull Off

		CGPROGRAM

#pragma vertex vert
#pragma fragment frag

#define PACK_NORMAL(NORMAL) ((NORMAL + 1) / 2)

		struct appdata {
		//float4 vertex : POSITION;
		float3 normal : NORMAL;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f {
		float4 pos : SV_POSITION;
		float3 normal : NORMAL;
	};

	v2f vert(appdata v) {
		v2f o;

		/*
		Unity camera space coords

		-1,-1 --- -1,0 --- 1,1
		|         |       |
		0,-1 ---  0,0 --- 0,1
		|         |       |
		1,-1 ---  1,0 --- 1,1
		*/

		o.pos = float4(v.texcoord.x * 2 - 1, -(v.texcoord.y * 2 - 1), 0.5, 1);
		o.normal = v.normal;
		return o;
	}

	float4 frag(v2f i) : SV_Target{
		return float4(PACK_NORMAL(i.normal), 1);
	}

		ENDCG
	}
	}
}
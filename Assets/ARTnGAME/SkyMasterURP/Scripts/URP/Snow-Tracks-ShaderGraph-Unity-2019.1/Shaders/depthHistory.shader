Shader "Unlit/depthHistory"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			 sampler2D _historyTex;



			//v0.1		
			float resetInit;
			float _RestoreSnowPower;
			float _restoreSnowFrequency;
			float _restoreSnowDetail;
			void Unity_Multiply_float_float(float A, float B, out float Out)
			{
			Out = A * B;
			}
			void Unity_Cosine_float(float In, out float Out)
			{
				Out = cos(In);
			}
			void Unity_Add_float(float A, float B, out float Out)
			{
				Out = A + B;
			}
			float Unity_SimpleNoise_RandomValue_float (float2 uv)
			{
				float angle = dot(uv, float2(12.9898, 78.233));
				#if defined(SHADER_API_MOBILE) && (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_VULKAN))
					// 'sin()' has bad precision on Mali GPUs for inputs > 10000
					angle = fmod(angle, TWO_PI); // Avoid large inputs to sin()
				#endif
				return frac(sin(angle)*43758.5453);
			}
			float Unity_SimpleNnoise_Interpolate_float (float a, float b, float t)
			{
				return (1.0-t)*a + (t*b);
			}
			float Unity_SimpleNoise_ValueNoise_float (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac(uv);
				f = f * f * (3.0 - 2.0 * f);

				uv = abs(frac(uv) - 0.5);
				float2 c0 = i + float2(0.0, 0.0);
				float2 c1 = i + float2(1.0, 0.0);
				float2 c2 = i + float2(0.0, 1.0);
				float2 c3 = i + float2(1.0, 1.0);
				float r0 = Unity_SimpleNoise_RandomValue_float(c0);
				float r1 = Unity_SimpleNoise_RandomValue_float(c1);
				float r2 = Unity_SimpleNoise_RandomValue_float(c2);
				float r3 = Unity_SimpleNoise_RandomValue_float(c3);

				float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
				float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
				float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
				return t;
			}
			float Unity_SimpleNoise_floatA(float2 UV, float Scale)
			{
				float t = 0.0;

				float freq = pow(2.0, float(0));
				float amp = pow(0.5, float(3-0));
				t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

				return t;//Out = t;
			}
			float Unity_RandomRange_float(float2 Seed, float Min, float Max)
			{
				 float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
				 return lerp(Min, Max, randomno);
			}//


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 historyTex = tex2D(_historyTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

				if(resetInit == 1){
					return 0;
				}

				float mul1 =0;
				if(_RestoreSnowPower > 0){
					float scale = _restoreSnowDetail+ 100*cos(_restoreSnowFrequency * _Time.y) ;
					mul1 = 0.001 * _RestoreSnowPower * (-(Unity_RandomRange_float(float2(1,2),0,1)* Unity_SimpleNoise_floatA(i.uv, scale) ));
				}				

                return (col + historyTex + (float4(mul1,0,0,0)));
            }
            ENDCG
        }
    }
}

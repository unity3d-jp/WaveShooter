/* -*- mode:Shader coding:utf-8-with-signature -*-
 */

Shader "Custom/wave_equation"
{
	Properties
	{
		_InputTex ("Input", 2D) = "black" {}
		_PrevTex ("Prev", 2D) = "black" {}
		_PrevPrevTex ("PrevPrev", 2D) = "black" {}
		[HideInInspector] _RoundAdjuster ("Adjuster", Float) = 0
	}
	SubShader
	{
		Pass
		{
			ZTest Always
			Cull Off
			ZWrite Off
			ZTest Always
			Tags { "RenderType"="Opaque" }
			LOD 100

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_nicest
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D_half _InputTex;
			sampler2D_half _PrevTex;
			sampler2D_half _PrevPrevTex;
			float2 _Stride;
			float _RoundAdjuster;

			v2f vert (appdata_img v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos (v.vertex);
				o.uv = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord.xy);
				return o; 
			}
			
			/*
			  +-+-+-+-+
			  | |B|A| |
			  +-+-+-+-+
			  |G|R|G|R|
			  +-+-+-+-+
			  |A|B|A|B|
			  +-+-+-+-+
			  | |R|G| |
			  +-+-+-+-+
			*/
			float4 frag (v2f i) : COLOR
			{
#if 1
				float2 stride = _Stride;
				half4 prev = (tex2D(_PrevTex, i.uv)*2)-1;
				half4 prevL = (tex2D(_PrevTex, float2(i.uv.x-stride.x, i.uv.y))*2)-1;
				half4 prevR = (tex2D(_PrevTex, float2(i.uv.x+stride.x, i.uv.y))*2)-1;
				half4 prevT = (tex2D(_PrevTex, float2(i.uv.x, i.uv.y-stride.y))*2)-1;
				half4 prevB = (tex2D(_PrevTex, float2(i.uv.x, i.uv.y+stride.y))*2)-1;
				half4 prevprev = (tex2D(_PrevPrevTex, i.uv)*2)-1;
				half paramC = 0.05;
				half vr = (prev.r*2 - prevprev.r + (prev.g + prevL.g + prev.b + prevT.b - prev.r*4) * paramC);
				half vg = (prev.g*2 - prevprev.g + (prevR.r + prev.r + prev.a + prevT.a - prev.g*4) * paramC);
				half vb = (prev.b*2 - prevprev.b + (prev.a + prevL.a + prevB.r + prev.r - prev.b*4) * paramC);
				half va = (prev.a*2 - prevprev.a + (prevR.b + prev.b + prevB.g + prev.g - prev.a*4) * paramC);
				float4 value = float4(vr, vg, vb, va);
				value += (tex2D(_InputTex, i.uv)*2)-1;
				value *= 0.994;
				value = (value+1)*0.5;
				value += _RoundAdjuster;
				return value;
#else
				float2 stride = _Stride;
				half4 prev = (tex2D(_PrevTex, i.uv)*2)-1;
				half value = 
					(prev.r*2 -
					 (tex2D(_PrevPrevTex, i.uv).r*2-1) +
					 ((tex2D(_PrevTex, half2(i.uv.x+stride.x, i.uv.y)).r*2-1) +
					  (tex2D(_PrevTex, half2(i.uv.x-stride.x, i.uv.y)).r*2-1) +
					  (tex2D(_PrevTex, half2(i.uv.x, i.uv.y+stride.y)).r*2-1) +
					  (tex2D(_PrevTex, half2(i.uv.x, i.uv.y-stride.y)).r*2-1) -
					  prev.r*4) * (0.1));
				value += (tex2D(_InputTex, i.uv).r*2)-1;
				value *= 0.994;
				value = (value+1)*0.5;
				value += _RoundAdjuster;
				return fixed4(value, value, value, value);
#endif
			}					 
			ENDCG
		}
	}
}

/*
 * End of wave_equation.shader
 */

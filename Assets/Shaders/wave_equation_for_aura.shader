/* -*- mode:Shader coding:utf-8-with-signature -*-
 */

Shader "Custom/wave_equation_for_aura"
{
	Properties
	{
		_InputTex ("Input", 2D) = "black" {}
		_PrevTex ("Prev", 2D) = "black" {}
		_PrevPrevTex ("PrevPrev", 2D) = "black" {}
	}
	SubShader
	{
		Pass
		{
			ZTest Always
			Cull Off
			ZWrite Off
			Tags { "RenderType"="Opaque" }
			LOD 100

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _InputTex;
			sampler2D _PrevTex;
			sampler2D _PrevPrevTex;
			float2 _Stride;

			v2f vert (appdata_img v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos (v.vertex);
				o.uv = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord.xy);
				return o; 
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				half2 stride = _Stride;
				half2 prev = tex2D(_PrevTex, i.uv).rg*2-1;
				half2 value = 
					(prev*2 -
					 (tex2D(_PrevPrevTex, i.uv).rg*2-1) +
					 ((tex2D(_PrevTex, float2(i.uv.x+stride.x, i.uv.y)).rg*2-1) +
					  (tex2D(_PrevTex, float2(i.uv.x-stride.x, i.uv.y)).rg*2-1) +
					  (tex2D(_PrevTex, float2(i.uv.x, i.uv.y+stride.y)).rg*2-1) +
					  (tex2D(_PrevTex, float2(i.uv.x, i.uv.y-stride.y)).rg*2-1) -
					  prev*4) * half2(0.05, 0.25));
				half input = tex2D(_InputTex, i.uv).a;
				value += half2(-floor(input)*frac(_Time.w*4), frac(input));
				value *= half2(0.93, 0.9);
				value = (value+1)*0.5;
				return fixed4(value, 0, 0);
			}					 
			ENDCG
		}
	}
}

/*
 * End of wave_equation_for_aura.shader
 */

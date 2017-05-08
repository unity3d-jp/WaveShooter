/* -*- mode:Shader coding:utf-8-with-signature -*-
 */

Shader "Custom/aura"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_WaveTex ("Wave", 2D) = "yellow" {}
		_BaseColor ("Base Color (RGB)", Color) = (0.2, 0.2, 0.2, 0)
		_AuraColor ("Base Color (RGB)", Color) = (0.5, 0.4, 0.1, 0)
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _WaveTex;
			half4 _BaseColor;
			half4 _AuraColor;

			fixed4 frag (v2f i) : SV_Target
			{
				half3 col0 = tex2D(_MainTex, i.uv).rgb;
				half3 v = tex2D(_WaveTex, i.uv).rgb;
				v -= 0.5;
				v *= 2;
				v = clamp(v, 0, 1);
				return fixed4(col0 + _AuraColor.rgb * v.r + _BaseColor.rgb * v.g, 0);
			}
			ENDCG
		}
	}
}

/*
 * End of aura.shader
 */

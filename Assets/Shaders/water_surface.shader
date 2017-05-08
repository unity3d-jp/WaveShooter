/* -*- mode:Shader coding:utf-8-with-signature -*-
 */

Shader "Custom/water_surface"
{
	Properties
	{
		_WaveSpeed ("Wave speed (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)
		_BaseColor ("Base Color (RGB)", Color) = (0.8, 0.9, 1, 1)
		[NoScaleOffset] _BumpMap ("BumpMap", 2D) = "bump" {}
		[NoScaleOffset] _ReflectiveColor ("Reflective color (RGB) fresnel (A) ", 2D) = "" {}
		[HideInInspector] _ReflectionTex ("Texture", 2D) = "green" {}
		_WaveTex ("Wave", 2D) = "yellow" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		// ColorMask RGBA

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_nicest
			#include "UnityCG.cginc"

			float4 _WaveScale4;
			float4 _WaveOffset;
			half4 _BaseColor;
			float2 _Scale;
			float2 _RScale;
			float2 _InterestPoint;
			float2 _Stride;

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 ref : TEXCOORD0;
				float4 bump_uv : TEXCOORD1;
				half4 normal_and_height : TEXCOORD2;
				half3 viewDir : TEXCOORD3;
				float4 vertex : SV_POSITION;
			};

			sampler2D _BumpMap;
			sampler2D _ReflectiveColor;
			sampler2D _ReflectionTex;
			sampler2D _WaveTex;
			
			v2f vert(appdata v)
			{
				float4 wpos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));
				float2 uv = wpos.xz*_RScale + 0.5;
				float height = tex2Dlod(_WaveTex, float4(uv, 0, 0)).r*2-1;
				wpos.y = height;
				float up =    tex2Dlod(_WaveTex, float4(uv.x, uv.y-_Stride.y, 0, 0)).r*2-1;
				float down =  tex2Dlod(_WaveTex, float4(uv.x, uv.y+_Stride.y, 0, 0)).r*2-1;
				float left =  tex2Dlod(_WaveTex, float4(uv.x-_Stride.x, uv.y, 0, 0)).r*2-1;
				float right = tex2Dlod(_WaveTex, float4(uv.x+_Stride.x, uv.y, 0, 0)).r*2-1;
				float3 normal = float3((left - right)*1, 1, (up - down)*1);
				normal = normalize(normal);

				v2f o;
				o.vertex = mul(UNITY_MATRIX_VP, wpos);
				o.ref = ComputeNonStereoScreenPos(o.vertex);
				o.bump_uv = wpos.xzxz * _WaveScale4 + _WaveOffset;
				half3 n = UnityObjectToWorldNormal(normal);
				height = (height-0.75)*4;
				height = clamp(height, 0, 1);
				height = height * height;
				o.normal_and_height = half4(n.xyz, height);
				o.viewDir.xzy = WorldSpaceViewDir(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half3 bump1 = UnpackNormal(tex2D( _BumpMap, i.bump_uv.xy )).rgb;
				half3 bump2 = UnpackNormal(tex2D( _BumpMap, i.bump_uv.zw )).rgb;
				half3 bump = half3((bump1.xz + bump2.xz) * 0.5, bump1.y);
				half3 norm = half3((i.normal_and_height.xz + bump.xz)*0.5, i.normal_and_height.y);
				norm = normalize(norm);
				i.viewDir = normalize(i.viewDir);
				half fresnel = dot(i.viewDir, norm);

				float4 uv = i.ref;
				uv.xy += norm.xy;
				half4 refl = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(uv));
				float fresnel16 = fresnel*16;
				half fu = frac(fresnel16);
				half4 water = tex2D(_ReflectiveColor, float2(fu, 1-fresnel));
				// half4 water = tex2D(_ReflectiveColor, float2(fresnel, 0));
				refl.rgb = lerp(_BaseColor.rgb*water.r, refl.rgb, water.a);

				half height = i.normal_and_height.w;
				// height *= height;
				half v = height*bump1.y*bump2.y;
				refl.rgb += _BaseColor.rgb*v;
				// refl.rgb += half3(v, v, v);
				return fixed4(refl.rgb, 0);
			}
			ENDCG
		}
	}
}

/*
 * End of water_surface.shader
 */

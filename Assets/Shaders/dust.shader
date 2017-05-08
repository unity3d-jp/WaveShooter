/* -*- mode:Shader coding:utf-8-with-signature -*-
 */

Shader "Custom/dust" {
    Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BaseColor ("Base Color (RGB)", Color) = (0.2, 0.2, 0.2, 0)
    }
	SubShader {
   		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		ZWrite Off
		Cull Off
		Blend SrcAlpha One
		// Blend SrcAlpha OneMinusSrcAlpha // alpha blending
		// Blend One OneMinusSrcAlpha // premultiplied alpha blending
		ColorMask RGB
		
        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
 			#pragma target 3.0
 			
 			#include "UnityCG.cginc"

            uniform sampler2D _MainTex;

 			struct appdata_custom {
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
				float4 texcoord2 : TEXCOORD1;
				float4 normal : NORMAL;
			};

 			struct v2f {
 				float4 pos : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 col : COLOR;
 			};
 			
			float3 _TargetPosition;
			float _Range;
			float _RangeR;
			float _CurrentTime;
			float3 _CamUp;
			half4 _BaseColor;
   
            v2f vert(appdata_custom v)
            {
				float4 tv = v.vertex;
				float3 target = _TargetPosition;
				float3 trip = floor( ((target - tv.xyz)*_RangeR + 1) * 0.5 );
				trip *= (_Range * 2);
				tv.xyz += trip;

				float theta = _CurrentTime*v.texcoord2.x;
				half s = (sin(theta)-0.75)*4;

				half size = step(0, s) * 0.1;
				half3 up = _CamUp;
				half3 eye = normalize(ObjSpaceViewDir(tv));
				half3 side = cross(eye, up);
				half3 vec = ((v.texcoord.x-0.5f)*side + (v.texcoord.y-0.5f)*up)*size;

				float3 v0 = tv.xyz + vec;
            	v2f o;
			    o.pos = UnityObjectToClipPos(float4(v0.xyz,1));
				o.texcoord = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);

				fixed alpha = saturate(s);
				o.col = fixed4(_BaseColor.rgb, alpha);
            	return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
				return i.col * tex2D(_MainTex, i.texcoord).a;
            }

            ENDCG
        }
    }
}

/*
 * End of explosion.shader
 */

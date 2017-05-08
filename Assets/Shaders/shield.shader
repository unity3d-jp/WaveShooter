/* -*- mode:Shader coding:utf-8-with-signature -*-
 */

Shader "Custom/shield" {
    Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
    }
	SubShader {
   		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		ZWrite Off
		Cull Off
		Blend SrcAlpha One
		// Blend SrcAlpha OneMinusSrcAlpha // alpha blending
		
        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
 			#pragma target 3.0
 			
 			#include "UnityCG.cginc"

            uniform sampler2D _MainTex;
			uniform fixed4 _Colors[3];

 			struct appdata_custom {
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				fixed4 color : COLOR;
				float4 texcoord : TEXCOORD0;
				float4 texcoord2 : TEXCOORD1;
			};

 			struct v2f {
 				float4 pos : SV_POSITION;
 				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
 			};
 			
			float  _CurrentTime;
   
            v2f vert(appdata_custom v)
            {
				float elapsed = (_CurrentTime - v.texcoord2.x);
				half alpha = clamp(1-elapsed*2, 0, 1);
				half size = 0.5;
				size = size * step(0.01, alpha);
				float3 up = float3(0,1,0);
				float3 tangent = normalize(cross(v.normal.xyz, up));
				float3 binormal = cross(v.normal.xyz, tangent);
				float3 vec = ((v.texcoord.x-0.5f)*tangent + (v.texcoord.y-0.5f)*binormal) *size + v.vertex.xyz;

            	v2f o;
				o.pos = UnityObjectToClipPos(float4(vec.xyz,1));
				int color_index = (int)v.texcoord2.y;
				o.color = _Colors[color_index];
				o.color.a = alpha;
				o.texcoord = MultiplyUV(UNITY_MATRIX_TEXTURE0,
										v.texcoord);
            	return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
				return fixed4(tex2D(_MainTex, i.texcoord).a * i.color.rgb, i.color.a);
            }

            ENDCG
        }
    }
}

/*
 * End of shield.shader
 */

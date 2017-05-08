/* -*- mode:Shader coding:utf-8-with-signature -*-
 */

Shader "Custom/sight" {
    Properties {
    }
	SubShader {
        Tags {"Queue" = "Geometry" "RenderType" = "Opaque"}
		ZWrite Off
		ZTest Always
		Cull Off
		
        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
 			#pragma target 3.0
 			
 			#include "UnityCG.cginc"

 			struct appdata_custom {
				float4 vertex : POSITION;
			};

 			struct v2f {
 				float4 pos : SV_POSITION;
 			};
 			
            v2f vert(appdata_custom v)
            {
            	v2f o;
			    o.pos = UnityObjectToClipPos(float4(v.vertex.xy, 0, 1));
            	return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
				return fixed4(0,1,0,1);
            }

            ENDCG
        }
    }
}

/*
 * End of sight.shader
 */

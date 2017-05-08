/* -*- mode:Shader coding:utf-8-with-signature -*-
 */

Shader "Custom/debris" {
	Properties {
	}
	SubShader {
   		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		ZWrite Off
		Blend SrcAlpha One 				// alpha additive
		ColorMask RGB
		
        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
 			#pragma target 3.0
 			
 			#include "UnityCG.cginc"

 			struct appdata_custom {
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

 			struct v2f {
 				float4 pos:SV_POSITION;
 				fixed4 color:COLOR;
 			};
 			
 			float4x4 _PrevInvMatrix;
			float3   _TargetPosition;
			float    _Range;
			float    _RangeR;
			float3   _BaseColor;
   
            v2f vert(appdata_custom v)
            {
				float3 target = _TargetPosition;
				float3 diff = target - v.vertex.xyz;
				float3 trip = floor( (diff*_RangeR + 1) * 0.5 );
				trip *= (_Range * 2);
				v.vertex.xyz += trip;

            	float4 tv0 = v.vertex;
            	tv0 = UnityObjectToClipPos(tv0);
				tv0 *= v.texcoord.x;
            	
            	float4 tv1 = v.vertex;
				tv1 = float4(UnityObjectToViewPos(tv1), 1);
            	tv1 = mul(_PrevInvMatrix, tv1);
            	tv1 = mul(UNITY_MATRIX_P, tv1);
				tv1.y -= 0.04*v.texcoord.y;
				tv1 *= v.texcoord.y;
            	
            	v2f o;
            	o.pos = tv0 + tv1;
            	float depth = o.pos.z * 0.02;
            	float normalized_depth = (1 - depth);
				fixed alpha = v.color.a * normalized_depth;
				half3 col = _BaseColor * 0.2;
				o.color = fixed4(col, alpha);
            	return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
				return i.color;
            }

            ENDCG
        }
    }
}

/*
 * End of debris.shader
 */

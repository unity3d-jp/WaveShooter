/* -*- mode:Shader coding:utf-8-with-signature -*-
 */

Shader "Custom/hahen" {
    Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
    }
	SubShader {
		Tags { "RenderType" = "Opaque" }
		ZWrite On
		// Blend SrcAlpha OneMinusSrcAlpha // alpha blending
		// Blend SrcAlpha One 				// alpha additive
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
				float4 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 texcoord2 : TEXCOORD1;
				float4 texcoord3 : TEXCOORD2;
			};

 			struct v2f {
 				float4 pos:SV_POSITION;
				float2 uv:TEXCOORD0;
 			};
 			
			float _CurrentTime;
   
            v2f vert(appdata_custom v)
            {
				float elapsed = (_CurrentTime - v.texcoord3.y);

            	float3 vec = v.vertex.xyz;
				float theta = elapsed * 16;
				float3 n = v.normal;
				// float3 rvec;
				/* rotate matrix for an arbitrary axis
				 * Vx*Vx*(1-cos) + cos	Vx*Vy*(1-cos) - Vz*sin	Vz*Vx*(1-cos) + Vy*sin;
				 * Vx*Vy*(1-cos) + Vz*sin	Vy*Vy*(1-cos) + cos	Vy*Vz*(1-cos) - Vx*sin;
				 * Vz*Vx*(1-cos) - Vy*sin	Vy*Vz*(1-cos) + Vx*sin	Vz*Vz*(1-cos) + cos;
				 */
				half s, c;
				sincos(theta, s, c);
				half3 n1c = n * (1-c);
				half3 ns = n * s;
				half3x3 mat = {
					(n.x*n1c.x + c),   (n.x*n1c.y - ns.z), (n.z*n1c.x + ns.y),
					(n.x*n1c.y + ns.z), (n.y*n1c.y + c),   (n.y*n1c.z - ns.x),
					(n.z*n1c.x - ns.y), (n.y*n1c.z + ns.x),   (n.z*n1c.z + c),
				};
				half3 rvec = mul(mat, vec);

				float4 tv0 = float4(rvec, 1);
				tv0.xyz += v.normal.xyz * (elapsed+0.1) * 4;
				tv0.y -= 0.5 * 9.8 * elapsed*elapsed;
				tv0.xy += v.texcoord2.xy;
				tv0.z += v.texcoord3.x;
				tv0.y += step(2, elapsed) * 1000;
            	tv0 = UnityObjectToClipPos(tv0);
            	
            	v2f o;
            	o.pos = tv0;
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
            	return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
				return fixed4(tex2D(_MainTex, i.uv));
            }

            ENDCG
        }
    }
}

/*
 * End of hahen.shader
 */

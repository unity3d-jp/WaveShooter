/* -*- mode:Shader coding:utf-8-with-signature -*-
 */

Shader "Custom/water_splash" {
    Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BaseColor ("Base Color (RGB)", Color) = (0, 0.5, 1, 1)
    }
	SubShader {
   		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		ZWrite Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha // alpha blending
		// Blend One OneMinusSrcAlpha // premultiplied alpha blending
		// ColorMask RGB
		
        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
 			#pragma target 3.0
 			
 			#include "UnityCG.cginc"

            sampler2D _MainTex;
			half4 _BaseColor;

 			struct appdata_custom {
				float4 vertex : POSITION;
				float4 velocity : NORMAL;
				fixed4 color : COLOR;
				float4 texcoord : TEXCOORD0;
				float4 texcoord2 : TEXCOORD1;
			};

 			struct v2f {
 				half4 pos : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				half p : TEXCOORD1;
				// half3 refl : TEXCOORD2;
 			};
 			
			float  _CurrentTime;
			float  _Gravity;
			float3  _CamUp;
   
            v2f vert(appdata_custom v)
            {
				float4 tv = v.vertex;
				float elapsed = _CurrentTime - v.texcoord2.x;
				tv += v.velocity * elapsed;
				tv.y += -0.5*_Gravity*elapsed*elapsed;

				half size = 0.1 + clamp(elapsed, 0, 1)*2;
				// half size = 2;
				half3 up = float3(0, 1, 0);
				half3 eye = normalize(ObjSpaceViewDir(tv));
				half3 side = cross(eye, up);

				// rotate
				half3 vec = ((v.texcoord.x-0.5f)*side + (v.texcoord.y-0.5f)*up) *size;
				half3 n = eye;
				half theta = v.texcoord2.y;
				/* rotate matrix for an arbitrary axis
				 * Vx*Vx*(1-cos) + cos  	Vx*Vy*(1-cos) - Vz*sin	Vz*Vx*(1-cos) + Vy*sin;
				 * Vx*Vy*(1-cos) + Vz*sin	Vy*Vy*(1-cos) + cos 	Vy*Vz*(1-cos) - Vx*sin;
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
				tv.xyz += rvec;

            	v2f o;
			    o.pos = UnityObjectToClipPos(float4(tv.xyz,1));
				o.texcoord = MultiplyUV(UNITY_MATRIX_TEXTURE0,
										float4(v.texcoord.xy, 0, 0));

				half p = max(elapsed*1.25-0.5, 0);
				o.p = p;
            	return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
				half src_a = tex2D(_MainTex, i.texcoord).a;
				half dst_a = (min(i.p.x + 1, 2.25) * src_a) - i.p.x;
				half alpha = smoothstep(0, 0.5, dst_a);
				half3 col = lerp(half3(1, 1, 1), _BaseColor.rgb, 1-alpha);
				return fixed4(col.rgb, alpha);
            }

            ENDCG
        }
    }
}

/*
 * End of water_splash.shader
 */

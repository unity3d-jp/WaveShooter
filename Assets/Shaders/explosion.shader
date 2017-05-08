/* -*- mode:Shader coding:utf-8-with-signature -*-
 */

Shader "Custom/explosion" {
    Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		//_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
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
				fixed4 color : COLOR;
				float4 texcoord : TEXCOORD0;
				float4 texcoord2 : TEXCOORD1;
			};

 			struct v2f {
 				float4 pos : SV_POSITION;
				float2 texcoord : TEXCOORD0;
 			};
 			
			float   _CurrentTime;
			float3   _CamUp;
   
            v2f vert(appdata_custom v)
            {
				float4 tv = v.vertex;
				float elapsed = _CurrentTime - v.texcoord2.x;
				half size = 3;
				size = size * step(elapsed, 2);
				half3 up = _CamUp;
				half3 eye = normalize(ObjSpaceViewDir(tv));
				half3 side = cross(eye, up);

				// rotate
				half3 vec = ((v.texcoord.x-0.5f)*side + (v.texcoord.y-0.5f)*up)*size;
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

				float rW = 1.0/8.0;
				float rH = 1.0/8.0;
				float fps = 60;
				float loop0 = 1.0/(fps*rW*rH);
				elapsed = clamp(elapsed, 0, loop0);

				float texu = floor(elapsed * fps) * rW - floor(elapsed*fps*rW);
				float texv = 1 - floor(elapsed * fps * rW) * rH;
				texu += v.texcoord.x * rW;
				texv += -v.texcoord.y * rH;
				
            	v2f o;
			    o.pos = UnityObjectToClipPos(float4(tv.xyz,1));
				o.texcoord = MultiplyUV(UNITY_MATRIX_TEXTURE0,
										float4(texu, texv, 0, 0));
            	return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
				return tex2D(_MainTex, i.texcoord);
            }

            ENDCG
        }
    }
}

/*
 * End of explosion.shader
 */

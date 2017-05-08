/* -*- mode:Shader coding:utf-8-with-signature -*-
 */

/**
 * refered from http://kylehalladay.com/blog/tutorial/bestof/2013/10/13/Multi-Light-Diffuse.html
 *
 */
Shader "Custom/diffuse_reflection"
{
    Properties 
    {
        [NoScaleOffset] _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		[NoScaleOffset] _CubeTex ("Cubemap", Cube) = "grey" {}
		_Specular ("Specular", Range (0, 1)) = 1
    }
    SubShader 
    {
        Tags {"Queue" = "Geometry" "RenderType" = "Opaque"}
		ColorMask RGB

        Pass 
        {
            Tags {"LightMode" = "ForwardBase"}

           	CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            
            struct vertex_input
            {
            	float4 vertex : POSITION;
            	float3 normal : NORMAL;
            	float2 texcoord : TEXCOORD0;
            };
            
            struct vertex_output
            {
                float4 pos         : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 lightDir    : TEXCOORD1;
                float3 normal      : TEXCOORD2;
				float3 I : TEXCOORD3;
				float3 model       : TEXCOORD4;
            };
            
            sampler2D _MainTex;
			samplerCUBE _CubeTex;
            float4 _MainTex_ST;
            fixed4 _LightColor0; 
			float _Specular;
			float _SwitchSkybox;
            
            vertex_output vert (vertex_input v)
            {
                vertex_output o;
                o.pos = UnityObjectToClipPos( v.vertex);
                o.uv = v.texcoord.xy;
				o.lightDir = ObjSpaceLightDir(v.vertex);
				o.normal = v.normal;
				o.model = v.vertex.xyz;

				float3 viewDir = WorldSpaceViewDir(v.vertex);
				float3 worldN = UnityObjectToWorldNormal(v.normal);
				o.I = reflect(-viewDir, worldN);
				o.I.y = abs(o.I.y)+0.002 /* remove artifact on horizon */;
				// o.I.y = (step(0.5, 1-_SwitchSkybox)*2-1) * (abs(o.I.y)+0.002 /* remove artifact on horizon. */);
				
                return o;
            }
            
            fixed4 frag(vertex_output i) : COLOR
            {
                // i.lightDir = normalize(i.lightDir);
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed diff = saturate(dot(i.normal, i.lightDir));
                fixed4 c = fixed4((UNITY_LIGHTMODEL_AMBIENT.rgb * 2 * tex.rgb), 1);
                c.rgb += (tex.rgb * _LightColor0.rgb * diff) * 2;
				// i.I.x += sin(i.model.x*43)*0.5; // distortion
				// i.I.y += cos(i.model.y*67)*0.5; // distortion
				fixed4 reflcol0 = texCUBE(_CubeTex, i.I);
				i.I.y = -i.I.y;
				fixed4 reflcol1 = texCUBE(_CubeTex, i.I);
				// c.rgb += reflcol.rgb * _Specular;
				fixed4 reflcol = fixed4(lerp(reflcol0.rgb, reflcol1.rgb, _SwitchSkybox), 0);
				c.rgb += reflcol.rgb * reflcol.rgb * 0.75;
                return c;
            }
            ENDCG
        }
    }
    FallBack "VertexLit"    // Use VertexLit's shadow caster/receiver passes.
}
/*
 * End of diffuse_reflection.shader
 */

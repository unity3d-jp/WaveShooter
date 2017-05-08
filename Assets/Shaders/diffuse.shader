/* -*- mode:Shader coding:utf-8-with-signature -*-
 */
/**
 * refered from http://kylehalladay.com/blog/tutorial/bestof/2013/10/13/Multi-Light-Diffuse.html
 *
 */
Shader "Custom/diffuse"
{
    Properties 
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_AlphaValue ("Alpha Value", float) = 0
    }
    SubShader 
    {
        Tags {"Queue" = "Geometry" "RenderType" = "Opaque"}
		// ColorMask RGB

        Pass 
        {
            Tags {"LightMode" = "ForwardBase"}                      // This Pass tag is important or Unity may not give it the correct light information.

           	CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase                       // This line tells Unity to compile this pass for forward base.
            
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
                float4  pos         : SV_POSITION;
                float2  uv          : TEXCOORD0;
                float3  lightDir    : TEXCOORD1;
                float3  normal		: TEXCOORD2;
                LIGHTING_COORDS(3,4)                            // Macro to send shadow & attenuation to the vertex shader.
            	float3  vertexLighting : TEXCOORD5;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _LightColor0; 
			fixed _AlphaValue;
            
            vertex_output vert (vertex_input v)
            {
                vertex_output o;
                o.pos = UnityObjectToClipPos( v.vertex);
                o.uv = v.texcoord.xy;
				o.lightDir = ObjSpaceLightDir(v.vertex);
				o.normal = v.normal;
                TRANSFER_VERTEX_TO_FRAGMENT(o);                 // Macro to send shadow & attenuation to the fragment shader.
                o.vertexLighting = float3(0.0, 0.0, 0.0);

		        #ifdef VERTEXLIGHT_ON
  				float3 worldN = mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL);
		      	float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
		        for (int index = 0; index < 4; index++)
		        {    
		           float4 lightPosition = float4(unity_4LightPosX0[index], 
												 unity_4LightPosY0[index], 
												 unity_4LightPosZ0[index], 1.0);
		           float3 vertexToLightSource = lightPosition.xyz - worldPos.xyz;
		           float3 lightDirection = normalize(vertexToLightSource);
		           float squaredDistance = dot(vertexToLightSource, vertexToLightSource);
		           float attenuation = 1.0 / (1.0  + unity_4LightAtten0[index] * squaredDistance);
		           float3 diffuseReflection = (unity_LightColor[index].xyz *
											   float3(_Color.rgb) *
											   max(0.0, dot(worldN, lightDirection)) *
											   attenuation);
		           o.vertexLighting = o.vertexLighting + diffuseReflection * 2;
		        }
		        #endif
                
                return o;
            }
            
            fixed4 frag(vertex_output i) : COLOR
            {
                i.lightDir = normalize(i.lightDir);
                fixed atten = LIGHT_ATTENUATION(i); // Macro to get you the combined shadow & attenuation value.
                fixed4 tex = tex2D(_MainTex, i.uv);
                tex *= _Color + fixed4(i.vertexLighting, 1.0);
                fixed diff = saturate(dot(i.normal, i.lightDir));
                fixed4 c;
                c.rgb = (UNITY_LIGHTMODEL_AMBIENT.rgb * 2 * tex.rgb);         // Ambient term. Only do this in Forward Base. It only needs calculating once.
                c.rgb += (tex.rgb * _LightColor0.rgb * diff) * (atten * 2); // Diffuse and specular.
				// c.a = tex.a + _LightColor0.a * atten;
				c.a = _AlphaValue;
                return c;
            }
            ENDCG
        }
    }
    FallBack "VertexLit"    // Use VertexLit's shadow caster/receiver passes.
}
/*
 * End of diffuse.shader
 */

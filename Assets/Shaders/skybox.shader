/* -*- mode:Shader coding:utf-8-with-signature -*-
 */

Shader "Custom/skybox" {
Properties {
	[NoScaleOffset] _Tex ("Cubemap", Cube) = "grey" {}
	_SwitchSkybox ("switch skybox", Range (0, 1)) = 0
}

SubShader {
	Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
		Cull Off ZWrite Off
		ColorMask RGBA
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragSimple
			#pragma target 2.0
			#include "skybox.cginc"
			ENDCG 
		}
	} 	
	Fallback Off
}

/*
 * End of skybox.shader
 */

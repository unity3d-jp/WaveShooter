/* -*- mode:Shader coding:utf-8-with-signature -*-
 */

Shader "Custom/skybox_mix" {
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
			#pragma vertex vertMix
			#pragma fragment fragMix
			#pragma target 2.0
			#include "skybox.cginc"
			ENDCG 
		}
	} 	
	Fallback Off
}

/*
 * End of skybox_mix.shader
 */

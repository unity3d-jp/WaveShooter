/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class WaterSplashRenderer : MonoBehaviour {
	// singleton
	static WaterSplashRenderer instance_;
	public static WaterSplashRenderer Instance { get { return instance_ ?? (instance_ = GameObject.Find("water_splash_renderer").GetComponent<WaterSplashRenderer>()); } }

	private MeshFilter mf_;
	private MeshRenderer mr_;

	public void init(WaterSplash water_splash)
	{
		mf_ = GetComponent<MeshFilter>();
		mr_ = GetComponent<MeshRenderer>();
		mf_.sharedMesh = water_splash.getMesh();
		mr_.sharedMaterial = water_splash.getMaterial();
	}
}

} // namespace UTJ {

/*
 * End of WaterSplashRenderer.cs
 */

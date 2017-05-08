/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class HahenRenderer : MonoBehaviour {
	// singleton
	static HahenRenderer instance_;
	public static HahenRenderer Instance { get { return instance_ ?? (instance_ = GameObject.Find("hahen_renderer").GetComponent<HahenRenderer>()); } }

	private MeshFilter mf_;
	private MeshRenderer mr_;

	public void init(Hahen hahen)
	{
		mf_ = GetComponent<MeshFilter>();
		mr_ = GetComponent<MeshRenderer>();
		mf_.sharedMesh = hahen.getMesh();
		mr_.sharedMaterial = hahen.getMaterial();
	}
}

} // namespace UTJ {

/*
 * End of HahenRenderer.cs
 */

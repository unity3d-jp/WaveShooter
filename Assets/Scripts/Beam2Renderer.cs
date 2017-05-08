/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class Beam2Renderer : MonoBehaviour {
	// singleton
	static Beam2Renderer instance_;
	public static Beam2Renderer Instance { get { return instance_ ?? (instance_ = GameObject.Find("beam2_renderer").GetComponent<Beam2Renderer>()); } }

	private MeshFilter mf_;
	private MeshRenderer mr_;

	public void init(Beam2 beam)
	{
		mf_ = GetComponent<MeshFilter>();
		mr_ = GetComponent<MeshRenderer>();
		mf_.sharedMesh = beam.getMesh();
		mr_.sharedMaterial = beam.getMaterial();
		mr_.SetPropertyBlock(Beam2.Instance.getMaterialPropertyBlock());
	}
}

} // namespace UTJ {

/*
 * End of Beam2Renderer.cs
 */

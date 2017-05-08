/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class BeamRenderer : MonoBehaviour {
	// singleton
	static BeamRenderer instance_;
	public static BeamRenderer Instance { get { return instance_ ?? (instance_ = GameObject.Find("beam_renderer").GetComponent<BeamRenderer>()); } }

	private MeshFilter mf_;
	private MeshRenderer mr_;

	public void init(Beam beam)
	{
		mf_ = GetComponent<MeshFilter>();
		mr_ = GetComponent<MeshRenderer>();
		mf_.sharedMesh = beam.getMesh();
		mr_.sharedMaterial = beam.getMaterial();
		mr_.SetPropertyBlock(beam.getMaterialPropertyBlock());
	}
}

} // namespace UTJ {

/*
 * End of BeamRenderer.cs
 */

/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class ShieldRenderer : MonoBehaviour {
	// singleton
	static ShieldRenderer instance_;
	public static ShieldRenderer Instance { get { return instance_ ?? (instance_ = GameObject.Find("shield_renderer").GetComponent<ShieldRenderer>()); } }

	private MeshFilter mf_;
	private MeshRenderer mr_;

	public void init(Shield shield)
	{
		mf_ = GetComponent<MeshFilter>();
		mr_ = GetComponent<MeshRenderer>();
		mf_.sharedMesh = shield.getMesh();
		mr_.sharedMaterial = shield.getMaterial();
		mr_.SetPropertyBlock(shield.getMaterialPropertyBlock());
	}
}

} // namespace UTJ {

/*
 * End of ShieldRenderer.cs
 */

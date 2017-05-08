/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class ExplosionRenderer : MonoBehaviour {
	// singleton
	static ExplosionRenderer instance_;
	public static ExplosionRenderer Instance { get { return instance_ ?? (instance_ = GameObject.Find("explosion_renderer").GetComponent<ExplosionRenderer>()); } }

	private MeshFilter mf_;
	private MeshRenderer mr_;

	public void init(Explosion explosion)
	{
		mf_ = GetComponent<MeshFilter>();
		mr_ = GetComponent<MeshRenderer>();
		mf_.sharedMesh = explosion.getMesh();
		mr_.sharedMaterial = explosion.getMaterial();
	}
}

} // namespace UTJ {

/*
 * End of ExplosionRenderer.cs
 */

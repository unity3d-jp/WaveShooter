/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class SightRenderer : MonoBehaviour
{
	// singleton
	static SightRenderer instance_;
	public static SightRenderer Instance { get { return instance_ ?? (instance_ = GameObject.Find("sight_renderer").GetComponent<SightRenderer>()); } }
	private MeshFilter mf_;
	private MeshRenderer mr_;
	
	public void init(Sight sight)
	{
		mf_ = GetComponent<MeshFilter>();
		mr_ = GetComponent<MeshRenderer>();
		mf_.sharedMesh = sight.getMesh();
		mr_.sharedMaterial = sight.getMaterial();
	}
}

} // namespace UTJ {
/*
 * End of SightRenderer.cs
 */

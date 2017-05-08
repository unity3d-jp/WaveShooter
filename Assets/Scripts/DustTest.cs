/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

public class DustTest : MonoBehaviour {

	public Camera camera_;
	public Material material_;

	void Awake()
	{
		Dust.Instance.init(material_);
	}

	void Update()
	{
		camera_.transform.Rotate(new Vector3(10f, 20f, 30f)*Time.deltaTime);
		camera_.transform.position = (camera_.transform.position + 
									  camera_.transform.TransformVector(Vector3.forward * 10f *
																		Time.deltaTime));
		Dust.Instance.render(0 /* front */, camera_, (double)Time.time);
	}
}

} // namespace UTJ {

/*
 * End of DustTest.cs
 */

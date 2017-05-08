/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

public class DebrisTest : MonoBehaviour {

	public Material material_;

	void Awake()
	{
		Debris.Instance.init(material_);
	}
	
	void Update()
	{
		Debris.Instance.render(0 /* front */, Camera.main, Time.realtimeSinceStartup);

		Camera.main.gameObject.transform.Rotate(0, 20f*Time.deltaTime, 0);
	}
}

} // namespace UTJ {

/*
 * End of DebrisTest.cs
 */

/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpringTorqueTest : MonoBehaviour {

	public GameObject target_;
	public GameObject vector_show_;
	private VectorShowTest vector_show_test_;

	void Start()
	{
		Time.fixedDeltaTime = 1f/60f;
		vector_show_test_ = vector_show_.GetComponent<VectorShowTest>();
	}

	void FixedUpdate()
	{
		if (target_ == null) {
			return;
		}

		var target_rot = target_.transform.rotation;
		var rot = target_rot * Quaternion.Inverse(transform.rotation);
		if (rot.w < 0f) {
			rot.x = -rot.x;
			rot.y = -rot.y;
			rot.z = -rot.z;
			rot.w = -rot.w;
		}
		var torque = new Vector3(rot.x, rot.y, rot.z) * 100f;
		vector_show_test_.show(ref torque);
		GetComponent<Rigidbody>().AddTorque(torque);
	}
}

/*
 * End of SpringTorqueTest.cs
 */

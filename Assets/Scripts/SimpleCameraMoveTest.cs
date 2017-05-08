/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleCameraMoveTest : MonoBehaviour {

	void FixedUpdate()
	{
		var hori = Input.GetAxisRaw("Horizontal");
		var vert = Input.GetAxisRaw("Vertical");

		var rb = GetComponent<Rigidbody>();
		rb.useGravity = false;
		rb.drag = 10f;
		rb.angularDrag = 10f;
		rb.AddRelativeTorque(vert * 10f, hori * 10f, 0f);
		var left = transform.TransformVector(Vector3.left);
		var hori_left = new Vector3(left.x, 0f, left.z);
		rb.AddTorque(Vector3.Cross(left, hori_left)*20f);
		var forward = transform.TransformVector(Vector3.forward);
		var hori_forward = new Vector3(forward.x, 0f, forward.z);
		rb.AddTorque(Vector3.Cross(forward, hori_forward)*20f);
		rb.AddRelativeForce(0f, 0f, 100f);
	}
}

/*
 * End of SimpleCameraMoveTest.cs
 */

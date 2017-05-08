/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorShowTest : MonoBehaviour {

	private Vector3 center_;

	void Awake()
	{
		center_ = transform.position;
	}

	public void show(ref Vector3 vector) {
		const float mag = 0.005f;
		var len = vector.magnitude * mag;
		transform.position = center_ + vector*mag;
		transform.rotation = Quaternion.LookRotation(vector) * Quaternion.Euler(90f, 0f, 0f);
		transform.localScale = new Vector3(0.05f, len, 0.05f);
	}
}

/*
 * End of VectorShowTest.cs
 */

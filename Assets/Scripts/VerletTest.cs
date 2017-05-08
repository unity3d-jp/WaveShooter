/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ {

public class VerletTest : MonoBehaviour {

	public GameObject[] go_list_;
	private RigidbodyTransform[] rb_list_;

	void Start ()
	{
		rb_list_ = new RigidbodyTransform[go_list_.Length];
		for (var i = 0; i < rb_list_.Length; ++i) {
			var pos = go_list_[i].transform.position;
			rb_list_[i].init(ref pos, ref CV.QuaternionIdentity);
		}
	}
	
	void Update ()
	{
		// for (var i = 1; i < rb_list_.Length; ++i) {
		// 	var gravity = new Vector3(0f, -9.8f, 0f);
		// 	rb_list_[i].addForce(ref gravity);
		// }
		for (var i = 1; i < rb_list_.Length; ++i) {
			rb_list_[i].updateVerlet(Time.deltaTime, 0.5f /* damper */);
		}
		{
			var head_offset = new Vector3(0f, 0f, -3f);
			var pos = rb_list_[0].transform_.transformPosition(ref head_offset);
			rb_list_[1].restrictPositionVerletFixed(ref pos, 0f /* length */);
			for (var i = 2; i < rb_list_.Length; ++i) {
				rb_list_[i].restrictPositionVerlet(ref rb_list_[i-2], ref rb_list_[i-1],
												   3f /* length */, 60f /* max_degree */);
			}
		}
		// for (var i = 1; i < rb_list_.Length; ++i) {
		// 	rb_list_[i-1].collideSphere(Vector3.zero /* center */, 5f /* radius */);
		// }
		for (var i = 1; i < rb_list_.Length - 1; ++i) {
			rb_list_[i].solveRotationVerlet(ref rb_list_[i-1], ref rb_list_[i+1]);
		}

		// fix
		{
			var pos = go_list_[0].transform.position;
			rb_list_[0].setPosition(ref pos);
			var rot = go_list_[0].transform.rotation;
			rb_list_[0].setRotation(ref rot);
		}
		// {
		// 	var idx = go_list_.Length-1;
		// 	var pos = go_list_[idx].transform.position;
		// 	rb_list_[idx].setPosition(ref pos);
		// 	var rot = go_list_[idx].transform.rotation;
		// 	rb_list_[idx].setRotation(ref rot);
		// }

		// render
		for (var i = 1; i < rb_list_.Length; ++i) {
			go_list_[i].transform.position = rb_list_[i].transform_.position_;
			go_list_[i].transform.rotation = rb_list_[i].transform_.rotation_;
		}
	}
}

} // namespace UTJ {

/*
 * End of VerletTest.cs
 */

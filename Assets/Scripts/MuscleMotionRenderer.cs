/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ {

public class MuscleMotionRenderer : MonoBehaviour {

	private Transform[] transform_list_;

	public void init()
	{
		transform_list_ = new Transform[(int)MuscleMotion.Parts.Max];
		var tfm = transform;
		for (var i = 0; i < transform_list_.Length; ++i) {
			transform_list_[i] = tfm.Find(((MuscleMotion.Parts)i).ToString());
		}
	}

	public void render(ref DrawBuffer.ObjectBuffer obj)
	{
		int parts = obj.versatile_data_;
		if (transform_list_[parts] != null) {
			transform_list_[parts].localPosition = obj.transform_.position_;
			transform_list_[parts].localRotation = obj.transform_.rotation_;
		}
	}

	public Transform getRootTransform()
	{
		return transform_list_[(int)MuscleMotion.Parts.Root];
	}
}

} // namespace UTJ {

/*
 * End of MuscleMotionRenderer.cs
 */

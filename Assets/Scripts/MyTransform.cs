/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;

namespace UTJ {

public struct MyTransform
{
	public Vector3 position_;
	public Quaternion rotation_;

	public void init()
	{
		init(ref CV.Vector3Zero, ref CV.QuaternionIdentity);
	}
	
	public void init(ref Vector3 position, ref Quaternion rotation)
	{
		position_ = position;
		rotation_ = rotation;
	}

	public Vector3 transformPosition(ref Vector3 pos)
	{
		return position_ + rotation_ * pos;
	}

	public Vector3 transformVector(ref Vector3 dir)
	{
		return rotation_ * dir;
	}

	public Matrix4x4 getTRS()
	{
		return Matrix4x4.TRS(position_, rotation_, new Vector3(1f, 1f, 1f));
	}

	public Matrix4x4 getInverseR()
	{
		var mat_rot = Matrix4x4.TRS(CV.Vector3Zero,
									rotation_,
									CV.Vector3One);
		var mat = mat_rot.transpose;
		// mat.SetColumn(3, new Vector4(-position_.x, -position_.y, -position_.z, 1f));
		return mat;
	}

	public MyTransform add(ref Vector3 offset) {
		var transform = new MyTransform();
		transform.position_ = transformPosition(ref offset);
		transform.rotation_ = rotation_;
		return transform;
	}
}

} // namespace UTJ {

/*
 * End of MyTransform.cs
 */

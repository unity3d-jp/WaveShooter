/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;

namespace UTJ {

public abstract class CameraBase : Task
{
	private const float SCREEN_WIDTH = 568;
	private const float SCREEN_HEIGHT = 320f; // size of FinalCamera
	public bool active_;
	protected RigidbodyTransform rigidbody_;
	private Matrix4x4 screen_matrix_;

	public override void init()
	{
		base.init();
		rigidbody_.init();
		active_ = true;
	}

	public override void renderUpdate(int front, CameraBase dummy, ref DrawBuffer draw_buffer)
	{
		if (active_) {
			draw_buffer.registCamera(ref rigidbody_.transform_);

			var view_matrix = Matrix4x4.TRS(rigidbody_.transform_.position_,
											rigidbody_.transform_.rotation_,
											CV.Vector3One);
			var projection_matrix = SystemManager.Instance.ProjectionMatrix;
			screen_matrix_ = projection_matrix * view_matrix.inverse;
		}
	}

	public Vector3 getScreenPoint(ref Vector3 world_position)
	{
		var v = screen_matrix_.MultiplyPoint(world_position);
		float w = SCREEN_WIDTH;
		float h = SCREEN_HEIGHT;
		return new Vector3(v.x * (-w), v.y * (-h), v.z);
	}
}

} // namespace UTJ {

/*
 * End of CameraBase.cs
 */

/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;

namespace UTJ {

public class MyCamera : CameraBase
{
	// const float SCREEN_WIDTH2 = 1280;
	// const float SCREEN_HEIGHT2 = 720;
	// private Matrix4x4 screen_matrix_;
	private Vector3 PLAYER_OFFSET = new Vector3(0f, 2f, -5f);

	public static MyCamera create()
	{
		var camera = new MyCamera();
		camera.init();
		return camera;
	}

	public override void init()
	{
		base.init();
		var player = Player.Instance;
		var target_position = player.rigidbody_.transform_.transformPosition(ref PLAYER_OFFSET);
		rigidbody_.setPosition(ref target_position);
		rigidbody_.setRotation(ref player.rigidbody_.transform_.rotation_);
		rigidbody_.setDamper(10f);
		rigidbody_.setRotateDamper(10f);
	}

	public override void update(float dt, double update_time)
	{
		var player = Player.Instance;
		var target_position = player.rigidbody_.transform_.transformPosition(ref PLAYER_OFFSET);
		rigidbody_.addSpringForce(ref target_position, 45f /* ratio */);
		var target_rotation = player.rigidbody_.transform_.rotation_;
		rigidbody_.addSpringTorque(ref target_rotation, 25f /* ratio */);
		rigidbody_.update(dt);
	}

	// public override void renderUpdate(int front, CameraBase camera, ref DrawBuffer draw_buffer)
	// {
	// 	draw_buffer.registCamera(ref rigidbody_.transform_);
	// }
}

} // namespace UTJ {

/*
 * End of MyCamera.cs
 */

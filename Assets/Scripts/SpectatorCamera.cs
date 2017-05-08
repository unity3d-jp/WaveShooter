/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;

namespace UTJ {

public class SpectatorCamera : CameraBase
{
	public static SpectatorCamera create()
	{
		var camera = new SpectatorCamera();
		camera.init();
		return camera;
	}

	readonly float[] spring_ratio_table_ = new float[8] { 4f, 8f, 5f, 3f, 4f, 3f, 5f, 3f, };
	readonly float[] relative_force_table_ = new float[8] { 100f, -50f, 80f, -200f, 100f, -150f, 80f, -200f, };
	int spring_ratio_idx_;

	public override void init()
	{
		base.init();
		var pos = new Vector3(0f, 0f, 20f);
		rigidbody_.setPosition(ref pos);
		var player = Player.Instance;
		var diff = player.rigidbody_.transform_.position_ - rigidbody_.transform_.position_;
		rigidbody_.transform_.rotation_ = Quaternion.LookRotation(diff);
		rigidbody_.setDamper(8f);
		spring_ratio_idx_ = 0;
	}

	public override void update(float dt, double update_time)
	{
		var player = Player.Instance;
		var diff = player.rigidbody_.transform_.position_ - rigidbody_.transform_.position_;
		var dir = new Vector3(0f, 0f, diff.magnitude*0.5f);
		var lookat = player.rigidbody_.transform_.transformPosition(ref dir);
		diff = lookat - rigidbody_.transform_.position_;
		rigidbody_.transform_.rotation_ = (Quaternion.LookRotation(diff) *
										   Quaternion.Euler((Mathf.PerlinNoise((float)update_time*0.4f, 0f)-0.5f) * 10f,
															(Mathf.PerlinNoise((float)update_time*0.4f, 0.3f)-0.5f) * 10f,
															(Mathf.PerlinNoise((float)update_time*0.4f, 0.6f)-0.5f) * 10f));
		var phase = update_time * 4f;
		var mag = 3f;
		var point = new Vector3((float)System.Math.Sin(phase)*mag, 2f, (float)System.Math.Cos(phase)*mag);
		var target = player.rigidbody_.transform_.transformPosition(ref point);
		if (MyRandom.Probability(0.005f)) {
			spring_ratio_idx_ = MyRandom.Range(0, spring_ratio_table_.Length);
		}
		float spring_ratio = spring_ratio_table_[spring_ratio_idx_];
		rigidbody_.addSpringForce(ref target, spring_ratio);
		rigidbody_.addRelativeForceX(relative_force_table_[spring_ratio_idx_]);
		rigidbody_.update(dt);
		rigidbody_.solveForGround(1f, dt);
	}

}

} // namespace UTJ {

/*
 * End of SpectatorCamera.cs
 */

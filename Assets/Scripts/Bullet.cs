/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections.Generic;

namespace UTJ {

public class Bullet : Task
{
	const int POOL_MAX = 64;
	private static Bullet[] pool_;
	private static int pool_index_;

	public static void createPool()
	{
		pool_ = new Bullet[POOL_MAX];
		for (var i = 0; i < POOL_MAX; ++i) {
			var task = new Bullet();
			task.alive_ = false;
			pool_[i] = task;
		}
		pool_index_ = 0;
	}

	public static Bullet create(ref Vector3 position,
								ref Quaternion rotation)
	{
		int cnt = 0;
		while (pool_[pool_index_].alive_) {
			++pool_index_;
			if (pool_index_ >= POOL_MAX)
				pool_index_ = 0;
			++cnt;
			if (cnt >= POOL_MAX) {
				Debug.LogError("EXCEED Bullet POOL!");
				break;
			}
		}
		var task = pool_[pool_index_];
		task.init(ref position, ref rotation);
		return task;
	}

	private RigidbodyTransform rigidbody_;
	private int collider_;
	private int beam_id_;
	private bool is_held_;
	private double start_;
	private Vector3 prev_position_;

	public void init(ref Vector3 position, ref Quaternion rotation)
	{
		base.init();
		rigidbody_.init(ref position, ref rotation);
		collider_ = MyCollider.createBullet();
		MyCollider.initSphereBullet(collider_, ref position, 0.25f /* radius */);
		MyCollider.disableForBullet(collider_, true);
		is_held_ = true;
		start_ = 0;
		beam_id_ = Beam.Instance.spawn(0.25f /* width */, Beam.Type.Bullet);
	}

	public void setPosition(ref Vector3 pos)
	{
		prev_position_ = rigidbody_.transform_.position_;
		rigidbody_.setPosition(ref pos);
	}

	public void setPower(float power)
	{
		Beam.Instance.setWidth(beam_id_, power);
		MyCollider.setPowerForBullet(collider_, power * 80f);
	}

	public void release(ref Vector3 velocity, double update_time)
	{
		rigidbody_.setVelocity(ref velocity);
		start_ = update_time;
		is_held_ = false;
	}

	public override void destroy()
	{
		Beam.Instance.destroy(beam_id_);
		beam_id_ = -1;
		MyCollider.destroyBullet(collider_);
		base.destroy();
	}

	public override void update(float dt, double update_time)
	{
		if (is_held_) {
			return;
		}

		if (MyCollider.isDisabledBullet(collider_) && update_time - start_ > 0.1) {
			MyCollider.disableForBullet(collider_, false);
		}

		var collider_type = MyCollider.getHitOpponentForBullet(collider_);
		if (collider_type != MyCollider.Type.None) {
			if (collider_type == MyCollider.Type.EnemyHoming) {
				Vector3 target;
				MyCollider.getHitOpponentInfoPositionForBullet(collider_, out target);
				var diff = target - rigidbody_.transform_.position_;
				Vector3.Normalize(diff);
				const float speed = 10f;
				diff *= speed;
				rigidbody_.setVelocity(ref diff);
			} else {
				Spark.Instance.spawn(ref rigidbody_.transform_.position_, Spark.Type.Bullet, update_time);
				destroy();
				return;
			}
		}
		
		if (rigidbody_.transform_.position_.y < 0f) {
			WaterSurface.Instance.makeBump(ref rigidbody_.transform_.position_, 0.5f /* value */, 1f /* size */);
			destroy();
			var pos = new Vector3(rigidbody_.transform_.position_.x, -0.5f, rigidbody_.transform_.position_.z);
			for (var i = 0; i < 8; ++i) {
				var spread = 2f;
				var vel = new Vector3(Mathf.Cos(i*Mathf.PI*(2f/8f))*spread,
									  MyRandom.Range(5f, 7f),
									  Mathf.Sin(i*Mathf.PI*(2f/8f))*spread);
				WaterSplash.Instance.spawn(ref pos, ref vel, update_time);
			}
			return;
		}

		prev_position_ = rigidbody_.transform_.position_;
		rigidbody_.update(dt);
		MyCollider.updateBullet(collider_, ref rigidbody_.transform_.position_);

		if (update_time - start_ > MyRandom.Range(2f, 2.5f)) { // 寿命
			destroy();
			return;
		}
	}

	public override void renderUpdate(int front, CameraBase camera, ref DrawBuffer draw_buffer)
	{
		// const float LENGTH = 2.5f;
		// var tail = rigidbody_.transform_.position_ - rigidbody_.velocity_.normalized * LENGTH;
		Beam.Instance.renderUpdate(front,
								   beam_id_,
								   ref rigidbody_.transform_.position_,
								   ref prev_position_);
	}

}

} // namespace UTJ {

/*
 * End of Bullet.cs
 */

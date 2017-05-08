/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections.Generic;

namespace UTJ {

public class EnemyBullet : Task
{
	const int POOL_MAX = 256;
	private static EnemyBullet[] pool_;
	private static int pool_index_;

	public static void createPool()
	{
		pool_ = new EnemyBullet[POOL_MAX];
		for (var i = 0; i < POOL_MAX; ++i) {
			var task = new EnemyBullet();
			task.alive_ = false;
			pool_[i] = task;
		}
		pool_index_ = 0;
	}

	public static void create(ref Vector3 position, ref Quaternion rotation, float speed, double update_time)
	{
		int cnt = 0;
		while (pool_[pool_index_].alive_) {
			++pool_index_;
			if (pool_index_ >= POOL_MAX)
				pool_index_ = 0;
			++cnt;
			if (cnt >= POOL_MAX) {
				Debug.LogError("EXCEED EnemyBullet POOL!");
				break;
			}
		}
		var task = pool_[pool_index_];
		task.init(ref position, ref rotation, speed, update_time);
	}

	private RigidbodyTransform rigidbody_;
	private int collider_;
	private int beam_id_;
	private double start_;

	public void init(ref Vector3 position, ref Quaternion rotation, float speed, double update_time)
	{
		base.init();
		var dir = rotation * CV.Vector3Forward;
		var velocity = dir * speed;
		var pos = position + velocity * 0.02f;
		rigidbody_.init(ref pos, ref rotation);
		rigidbody_.setVelocity(velocity);
		collider_ = MyCollider.createEnemyBullet();
		MyCollider.initSphereEnemyBullet(collider_, ref position, 0.5f /* radius */);
		start_ = update_time;
		beam_id_ = Beam.Instance.spawn(0.5f /* width */, Beam.Type.EnemyBullet);
	}

	public override void destroy()
	{
		Beam.Instance.destroy(beam_id_);
		beam_id_ = -1;
		MyCollider.destroyEnemyBullet(collider_);
		base.destroy();
	}

	public override void update(float dt, double update_time)
	{
		if (MyCollider.getHitOpponentForEnemyBullet(collider_) != MyCollider.Type.None) {
			Vector3 intersect_point;
			MyCollider.getIntersectPointForEnemyBullet(collider_, out intersect_point);
			Spark.Instance.spawn(ref intersect_point, Spark.Type.EnemyBullet, update_time);
			destroy();
			return;
		}
		if (rigidbody_.transform_.position_.y < 0f) {
			WaterSurface.Instance.makeBump(ref rigidbody_.transform_.position_, 0.5f /* value */, 0.5f /* size */);
			destroy();
			var pos = new Vector3(rigidbody_.transform_.position_.x, -0.5f, rigidbody_.transform_.position_.z);
			for (var i = 0; i < 4; ++i) {
				var spread = 2f;
				var vel = new Vector3(Mathf.Cos(i*Mathf.PI*(2f/8f))*spread,
									  MyRandom.Range(5f, 7f),
									  Mathf.Sin(i*Mathf.PI*(2f/8f))*spread);
				WaterSplash.Instance.spawn(ref pos, ref vel, update_time);
			}
			return;
		}

		rigidbody_.update(dt);
		MyCollider.updateEnemyBullet(collider_, ref rigidbody_.transform_.position_);

		if (update_time - start_ > MyRandom.Range(2f, 2.5f)) { // 寿命
			destroy();
			return;
		}
	}

	public override void renderUpdate(int front, CameraBase camera, ref DrawBuffer draw_buffer)
	{
		const float LENGTH = 2.5f;
		var tail = rigidbody_.transform_.position_ - rigidbody_.velocity_.normalized * LENGTH;
		Beam.Instance.renderUpdate(front,
								   beam_id_,
								   ref rigidbody_.transform_.position_,
								   ref tail);
	}

}

} // namespace UTJ {

/*
 * End of EnemyBullet.cs
 */

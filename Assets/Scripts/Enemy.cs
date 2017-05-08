/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UTJ {

public partial class Enemy : Task
{
	const int POOL_MAX = 2048;
	private static Enemy[] pool_;
	private static int pool_index_;
	private static Dragon dragon_pool_;

	public static void createPool()
	{
		pool_ = new Enemy[POOL_MAX];
		for (var i = 0; i < POOL_MAX; ++i) {
			var enemy = new Enemy();
			enemy.alive_ = false;
			// enemy.type_ = Type.None;
			pool_[i] = enemy;
		}
		pool_index_ = 0;

		dragon_pool_ = new Dragon();
		dragon_pool_.createPool();
	}
	

	public enum Type {
		None,
		Zako,
		Dragon,
	}
	private enum Phase {
		Alive,
		Dying,
	}

	private RigidbodyTransform rigidbody_;
	private int collider_;
	private int collider_homing_;
	private float life_;
	private IEnumerator enumerator_;
	private double update_time_;
	private Phase phase_;
	private Vector3 target_position_;
	private delegate void OnUpdateFunc(float dt);
	private OnUpdateFunc on_update_;
	private delegate void OnRenderUpdateFunc(int front, ref DrawBuffer draw_buffer);
	private OnRenderUpdateFunc on_render_update_;

	public static Enemy create(Type type, ref Vector3 position, ref Quaternion rotation)
	{
		Enemy enemy = Enemy.create();
		enemy.phase_ = Phase.Alive;
		enemy.init();
		switch (type) {
			case Type.None:
				Debug.Assert(false);
				break;
			case Type.Zako:
				enemy.zako_init(ref position, ref rotation);
				break;
			case Type.Dragon:
				enemy.dragon_init(ref position, ref rotation);
				break;
		}
		return enemy;
	}

	private static Enemy create()
	{
		int cnt = 0;
		while (pool_[pool_index_].alive_) {
			++pool_index_;
			if (pool_index_ >= POOL_MAX)
				pool_index_ = 0;
			++cnt;
			if (cnt >= POOL_MAX) {
				Debug.LogError("EXCEED Enemy POOL!");
				break;
			}
		}
		var enemy = pool_[pool_index_];
		return enemy;
	}

	private void calc_lock_position_center(ref Vector3 position)
	{
		position = rigidbody_.transform_.position_;
	}

	public override void destroy()
	{
		enumerator_ = null;
		if (collider_ >= 0) {
			MyCollider.destroyEnemy(collider_);
			collider_ = -1;
		}
		if (collider_homing_ >= 0) {
			MyCollider.destroyEnemyHoming(collider_homing_);
			collider_homing_ = -1;
		}
		base.destroy();
	}

	public override void update(float dt, double update_time)
	{
		if (phase_ == Phase.Dying) {
			destroy();
			return;
		}

		update_time_ = update_time;
		if (enumerator_ != null) {
			enumerator_.MoveNext();
		}
		if (alive_) {
			on_update_(dt);
		}
	}

	public override void renderUpdate(int front, CameraBase camera, ref DrawBuffer draw_buffer)
	{
		on_render_update_(front, ref draw_buffer);
	}
}

} // namespace UTJ {

/*
 * End of Enemy.cs
 */

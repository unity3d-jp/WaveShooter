/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

public struct MyCollider
{
	private static MyCollider player_;
	const int POOL_BULLET_MAX = 128;
	private static MyCollider[] pool_bullet_;
	private static int pool_bullet_index_;
	const int POOL_ENEMY_MAX = 1024;
	private static MyCollider[] pool_enemy_;
	private static int pool_enemy_index_;
	const int POOL_ENEMY_HOMING_MAX = 1024;
	private static MyCollider[] pool_enemy_homing_;
	private static int pool_enemy_homing_index_;
	const int POOL_ENEMY_BULLET_MAX = 2048;
	private static MyCollider[] pool_enemy_bullet_;
	private static int pool_enemy_bullet_index_;
	private static int nearest_enemy_index_;

	public static void createPool()
	{
		player_.alive_ = false;
		player_.id_ = 0;
		player_.type_ = Type.Player;

		pool_bullet_ = new MyCollider[POOL_BULLET_MAX];
		for (var i = 0; i < POOL_BULLET_MAX; ++i) {
			pool_bullet_[i].alive_ = false;
			pool_bullet_[i].id_ = i;
			pool_bullet_[i].type_ = Type.Bullet;
		}
		pool_bullet_index_ = 0;

		pool_enemy_ = new MyCollider[POOL_ENEMY_MAX];
		for (var i = 0; i < POOL_ENEMY_MAX; ++i) {
			pool_enemy_[i].alive_ = false;
			pool_enemy_[i].id_ = i;
			pool_enemy_[i].type_ = Type.Enemy;
		}
		pool_enemy_index_ = 0;

		pool_enemy_homing_ = new MyCollider[POOL_ENEMY_HOMING_MAX];
		for (var i = 0; i < POOL_ENEMY_HOMING_MAX; ++i) {
			pool_enemy_homing_[i].alive_ = false;
			pool_enemy_homing_[i].id_ = i;
			pool_enemy_homing_[i].type_ = Type.EnemyHoming;
		}
		pool_enemy_homing_index_ = 0;

		pool_enemy_bullet_ = new MyCollider[POOL_ENEMY_BULLET_MAX];
		for (var i = 0; i < POOL_ENEMY_BULLET_MAX; ++i) {
			pool_enemy_bullet_[i].alive_ = false;
			pool_enemy_bullet_[i].id_ = i;
			pool_enemy_bullet_[i].type_ = Type.EnemyBullet;
		}
		pool_enemy_bullet_index_ = 0;
	}

	private static void clear(ref MyCollider[] pool)
	{
		for (var i = 0; i < pool.Length; ++i) {
			pool[i].alive_ = false;
			pool[i].disabled_ = false;
			pool[i].id_ = 0;
			pool[i].opponent_info_.clear();
		}
	}

	public static void restart()
	{
		clear(ref pool_bullet_);
		clear(ref pool_enemy_);
		clear(ref pool_enemy_homing_);
		clear(ref pool_enemy_bullet_);
	}

	private static void create(ref MyCollider[] pool, ref int pool_index)
	{
		int cnt = 0;
		while (pool[pool_index].alive_) {
			++pool_index;
			if (pool_index >= pool.Length)
				pool_index = 0;
			++cnt;
			if (cnt >= pool.Length) {
				Debug.LogError("EXCEED Collider POOL!");
				Debug.Assert(false);
				break;
			}
		}
		pool[pool_index].alive_ = true;
		pool[pool_index].disabled_ = false;
		pool[pool_index].opponent_info_.clear();
		pool[pool_index].phase_ = 0;
		pool[pool_index].power_ = 0f; // set later
	}

	public static int createPlayer()
	{
		player_.alive_ = true;
		player_.disabled_ = false;
		player_.opponent_info_.clear();
		player_.phase_ = 0;
		player_.power_ = 0f;
		return 0;
	}
	
	public static void setPowerForBullet(int id, float power)
	{
		Debug.Assert(pool_bullet_[id].alive_);
		pool_bullet_[id].power_ = power;
	}

	public static int createBullet()
	{
		create(ref pool_bullet_, ref pool_bullet_index_);
		return pool_bullet_index_;
	}
	public static int createEnemy()
	{
		create(ref pool_enemy_, ref pool_enemy_index_);
		return pool_enemy_index_;
	}
	public static int createEnemyHoming()
	{
		create(ref pool_enemy_homing_, ref pool_enemy_homing_index_);
		return pool_enemy_homing_index_;
	}
	public static int createEnemyBullet()
	{
		create(ref pool_enemy_bullet_, ref pool_enemy_bullet_index_);
		return pool_enemy_bullet_index_;
	}

	public static void initSpherePlayer(int id, ref Vector3 pos, float radius)
	{
		player_.initSphere(ref pos, radius);
	}

	public static void initSphereBullet(int id, ref Vector3 pos, float radius)
	{
		Debug.Assert(pool_bullet_[id].alive_);
		pool_bullet_[id].initSphere(ref pos, radius);
	}

	public static void initSphereEnemy(int id, ref Vector3 pos, float radius)
	{
		Debug.Assert(pool_enemy_[id].alive_);
		pool_enemy_[id].initSphere(ref pos, radius);
	}

	public static void initSphereEnemyHoming(int id, ref Vector3 pos, float radius)
	{
		Debug.Assert(pool_enemy_homing_[id].alive_);
		pool_enemy_homing_[id].initSphere(ref pos, radius);
	}

	public static void initSphereEnemyBullet(int id, ref Vector3 pos, float radius)
	{
		Debug.Assert(pool_enemy_bullet_[id].alive_);
		pool_enemy_bullet_[id].initSphere(ref pos, radius);
	}

	public static void updatePlayer(int id, ref Vector3 pos)
	{
		player_.update(ref pos);
	}

	public static void updateBullet(int id, ref Vector3 pos)
	{
		Debug.Assert(pool_bullet_[id].alive_);
		pool_bullet_[id].update(ref pos);
	}

	public static void updateEnemy(int id, ref Vector3 pos)
	{
		Debug.Assert(pool_enemy_[id].alive_);
		pool_enemy_[id].update(ref pos);
	}

	public static void updateEnemyHoming(int id, ref Vector3 pos)
	{
		Debug.Assert(pool_enemy_homing_[id].alive_);
		pool_enemy_homing_[id].update(ref pos);
	}

	public static void updateEnemyBullet(int id, ref Vector3 pos)
	{
		Debug.Assert(pool_enemy_bullet_[id].alive_);
		pool_enemy_bullet_[id].update(ref pos);
	}

	public static void destroyPlayer(int id)
	{
		player_.alive_ = false;
		player_.opponent_info_.clear();
	}

	public static void destroyBullet(int id)
	{
		Debug.Assert(pool_bullet_[id].alive_);
		pool_bullet_[id].alive_ = false;
		pool_bullet_[id].opponent_info_.clear();
	}

	public static void destroyEnemy(int id)
	{
		Debug.Assert(pool_enemy_[id].alive_);
		pool_enemy_[id].alive_ = false;
		pool_enemy_[id].opponent_info_.clear();
	}

	public static void destroyEnemyHoming(int id)
	{
		Debug.Assert(pool_enemy_homing_[id].alive_);
		pool_enemy_homing_[id].alive_ = false;
		pool_enemy_homing_[id].opponent_info_.clear();
	}

	public static void destroyEnemyBullet(int id)
	{
		Debug.Assert(pool_enemy_bullet_[id].alive_);
		pool_enemy_bullet_[id].alive_ = false;
		pool_enemy_bullet_[id].opponent_info_.clear();
	}

	public static void calculate()
	{
		// clear
		player_.opponent_info_.clear();
		for (var i = 0; i < pool_bullet_.Length; ++i) {
			pool_bullet_[i].opponent_info_.clear();
		}
		for (var i = 0; i < pool_enemy_.Length; ++i) {
			pool_enemy_[i].opponent_info_.clear();
		}
		for (var i = 0; i < pool_enemy_homing_.Length; ++i) {
			pool_enemy_homing_[i].opponent_info_.clear();
		}
		for (var i = 0; i < pool_enemy_bullet_.Length; ++i) {
			pool_enemy_bullet_[i].opponent_info_.clear();
		}

		// player - enemy
		nearest_enemy_index_ = -1;
		float nearest_dist2 = System.Single.MaxValue;
		for (var i = 0; i < pool_enemy_.Length; ++i) {
			if (pool_enemy_[i].alive_ && !pool_enemy_[i].disabled_) {
				var dist2 = check_intersection(ref player_, ref pool_enemy_[i]);
				if (dist2 < nearest_dist2) {
					nearest_dist2 = dist2;
					nearest_enemy_index_ = i;
				}
			}
		}
		// player - enemy_bullet
		for (var i = 0; i < pool_enemy_bullet_.Length; ++i) {
			if (pool_enemy_bullet_[i].alive_) {
				check_intersection(ref player_, ref pool_enemy_bullet_[i]);
			}
		}
		// enemy - bullet
		for (var i = 0; i < pool_enemy_.Length; ++i) {
			if (pool_enemy_[i].alive_ && !pool_enemy_[i].disabled_) {
				for (var j = 0; j < pool_bullet_.Length; ++j) {
					if (pool_bullet_[j].alive_) {
						check_intersection(ref pool_enemy_[i], ref pool_bullet_[j]);
					}
				}
			}
		}
		// enemy_homing - bullet
		for (var i = 0; i < pool_enemy_homing_.Length; ++i) {
			if (pool_enemy_homing_[i].alive_ && !pool_enemy_homing_[i].disabled_) {
				for (var j = 0; j < pool_bullet_.Length; ++j) {
					if (pool_bullet_[j].alive_ && !pool_bullet_[j].disabled_ && pool_bullet_[j].phase_ == 0) {
						if (pool_enemy_homing_[i].center_.y > 0f) {
							check_homing(ref pool_enemy_homing_[i], ref pool_bullet_[j]);
						}
					}
				}
			}
		}
	}

	private static float check_intersection(ref MyCollider col0, ref MyCollider col1)
	{
		var diff = col1.center_ - col0.center_;
		var len2 = (diff.x * diff.x +
					diff.y * diff.y +
					diff.z * diff.z);
		var rad2 = col0.radius_+col1.radius_;
		rad2 = rad2 * rad2;
		if (len2 < rad2) {
			float len = Mathf.Sqrt(len2);
			var intersect_point = col0.center_ + (diff * (col0.radius_/len));
			col0.opponent_info_.set(col1.type_, ref intersect_point, col1.power_);
			col1.opponent_info_.set(col0.type_, ref intersect_point, col0.power_);
		}
		return rad2;
	}
	private static void check_homing(ref MyCollider col0, ref MyCollider col1)
	{
		var diff = col1.center_ - col0.center_;
		var len2 = (diff.x * diff.x +
					diff.y * diff.y +
					diff.z * diff.z);
		var rad2 = col0.radius_+col1.radius_;
		rad2 = rad2 * rad2;
		if (len2 < rad2) {
			col0.opponent_info_.set(col1.type_, ref col1.center_, col1.power_);
			++col0.phase_;
			col1.opponent_info_.set(col0.type_, ref col0.center_, col0.power_);
			++col1.phase_;
		}
	}

	public static Type getHitOpponentForPlayer(int id, ref Vector3 pos)
	{
		Debug.Assert(player_.alive_);
		pos = player_.opponent_info_.intersect_point_;
		return player_.opponent_info_.type_;
	}

	public static Type getHitOpponentForBullet(int id)
	{
		Debug.Assert(pool_bullet_[id].alive_);
		return pool_bullet_[id].opponent_info_.type_;
	}

	public static void getHitOpponentInfoPositionForBullet(int id, out Vector3 pos)
	{
		Debug.Assert(pool_bullet_[id].alive_);
		pos = pool_bullet_[id].opponent_info_.intersect_point_;
	}

	public static Type getHitOpponentForEnemy(int id)
	{
		Debug.Assert(pool_enemy_[id].alive_);
		return pool_enemy_[id].opponent_info_.type_;
	}

	public static float getHitPowerForEnemy(int id)
	{
		Debug.Assert(pool_enemy_[id].alive_);
		return pool_enemy_[id].opponent_info_.power_;
	}

	public static void getIntersectPointForEnemy(int id, out Vector3 pos)
	{
		Debug.Assert(pool_enemy_[id].alive_);
		pos = pool_enemy_[id].opponent_info_.intersect_point_;
	}

	public static void getIntersectPointForEnemyBullet(int id, out Vector3 pos)
	{
		Debug.Assert(pool_enemy_bullet_[id].alive_);
		pos = pool_enemy_bullet_[id].opponent_info_.intersect_point_;
	}

	public static Type getHitOpponentForEnemyHoming(int id)
	{
		Debug.Assert(pool_enemy_homing_[id].alive_);
		return pool_enemy_homing_[id].opponent_info_.type_;
	}

	public static Type getHitOpponentForEnemyBullet(int id)
	{
		Debug.Assert(pool_enemy_bullet_[id].alive_);
		return pool_enemy_bullet_[id].opponent_info_.type_;
	}

	public static bool isDisabledBullet(int id)
	{
		return pool_bullet_[id].disabled_;
	}

	public static bool getNearestEnemyPosition(out Vector3 pos)
	{
		if (nearest_enemy_index_ >= 0) {
			pos = pool_enemy_[nearest_enemy_index_].center_;
			return true;
		} else {
			pos = CV.Vector3Zero;
			return false;
		}
	}

	public static void disableForBullet(int id, bool flg)
	{
		pool_bullet_[id].disabled_ = flg;
		pool_bullet_[id].opponent_info_.clear();
	}

	public static void disableForEnemy(int id)
	{
		pool_enemy_[id].disabled_ = true;
		pool_enemy_[id].opponent_info_.clear();
	}

	public static void disableForEnemyHoming(int id)
	{
		pool_enemy_homing_[id].disabled_ = true;
		pool_enemy_homing_[id].opponent_info_.clear();
	}

	public enum Type {
		None,
		Player,
		Bullet,
		Enemy,
		EnemyHoming,
		EnemyBullet,
	}

	public enum Shape {
		Sphere,
	}

	public struct OpponentInfo {
		public Type type_;
		public Vector3 intersect_point_;
		public float power_;
		public void clear() {
			type_ = Type.None;
		}
		public void set(Type type, ref Vector3 pos, float power) {
			type_ = type;
			intersect_point_ = pos;
			power_ = power;
		}
	}

	public bool alive_;
	public bool disabled_;
	public int id_;
	public Type type_;
	public OpponentInfo opponent_info_;
	public Vector3 center_;
	public float radius_;
	public Shape shape_;
	public int phase_;
	public float power_;
	
	public void initSphere(ref Vector3 pos, float radius)
	{
		center_ = pos;
		radius_ = radius;
		shape_ = Shape.Sphere;
	}

	public void update(ref Vector3 pos)
	{
		center_ = pos;
	}
}

} // namespace UTJ {

/*
 * End of MyCollider.cs
 */

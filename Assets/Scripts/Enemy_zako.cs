/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UTJ {

public partial class Enemy : Task
{
	public void zako_init(ref Vector3 position, ref Quaternion rotation)
	{
		rigidbody_.init(ref position, ref rotation);
		collider_ = MyCollider.createEnemy();
		MyCollider.initSphereEnemy(collider_, ref rigidbody_.transform_.position_, 0.5f /* radius */);
		collider_homing_ = MyCollider.createEnemyHoming();
		MyCollider.initSphereEnemyHoming(collider_homing_, ref rigidbody_.transform_.position_, 5f /* radius */);
		enumerator_ = zako_act(); // この瞬間は実行されない
		on_update_ = new OnUpdateFunc(zako_update);
		on_render_update_ = new OnRenderUpdateFunc(zako_render_update);
		life_ = 50f;
	}

	public IEnumerator zako_act()
	{
		rigidbody_.setDamper(2f);
		rigidbody_.setRotateDamper(4f);
		yield return null;
		for (var i = new Utility.WaitForSeconds(0.8f, update_time_); !i.end(update_time_);) {
			rigidbody_.addSpringTorque(ref Player.Instance.rigidbody_.transform_.position_, 3f);
			rigidbody_.addSpringForceY(4f /* target_y */, 4f /* ratio */);
			yield return null;
		}
		WaterSurface.Instance.makeBump(ref rigidbody_.transform_.position_, 1f /* value */, 1f /* size */);
		for (var i = new Utility.WaitForSeconds(1f, update_time_); !i.end(update_time_);) {
			rigidbody_.addSpringTorque(ref Player.Instance.rigidbody_.transform_.position_, 3f);
			rigidbody_.addSpringForceY(4f /* target_y */, 4f /* ratio */);
			if (MyRandom.Probability(0.1f)) {
				WaterSplash.Instance.spawn(ref rigidbody_.transform_.position_,
										   ref CV.Vector3Zero, update_time_-0.2f);
			}
			yield return null;
		}
		for (var i = new Utility.WaitForSeconds(10f, update_time_); !i.end(update_time_);) {
			var pos = Player.Instance.rigidbody_.transform_.position_;
			// pos.y -= 1f;
			rigidbody_.addSpringTorque(ref pos, 8f);
			rigidbody_.addSpringForceY(4f /* target_y */, 4f /* ratio */);
			if (MyRandom.Probability(0.04f)) {
				EnemyBullet.create(ref rigidbody_.transform_.position_,
								   ref rigidbody_.transform_.rotation_,
								   60f /* speed */, update_time_);
			}
			yield return null;
		}
		for (var i = new Utility.WaitForSeconds(3f, update_time_); !i.end(update_time_);) {
			rigidbody_.addHorizontalStableTorque(8f /* torque_level */);
			rigidbody_.addRelativeForceZ(80f);
			yield return null;
		}
		destroy();
	}

	private void zako_update(float dt)
	{
		if (phase_ == Phase.Alive) {
			if (MyCollider.getHitOpponentForEnemy(collider_) != MyCollider.Type.None) {
				float p = 100f;
				rigidbody_.addTorque(MyRandom.Range(-p, p),
									 MyRandom.Range(-p, p),
									 MyRandom.Range(-p, p));
				life_ -= MyCollider.getHitPowerForEnemy(collider_);

				if (life_ <= 0f) {
					Explosion.Instance.spawn(ref rigidbody_.transform_.position_, update_time_);
					Hahen.Instance.spawn(ref rigidbody_.transform_.position_, update_time_);
					SystemManager.Instance.registSound(DrawBuffer.SE.Explosion);
					MyCollider.disableForEnemy(collider_);
					phase_ = Phase.Dying;
				} else {
					Vector3 pos;
					MyCollider.getIntersectPointForEnemy(collider_, out pos);
					Spark.Instance.spawn(ref pos, Spark.Type.Bullet, update_time_);
				}
			}
		}
		rigidbody_.update(dt);
		MyCollider.updateEnemy(collider_, ref rigidbody_.transform_.position_);
		MyCollider.updateEnemyHoming(collider_homing_, ref rigidbody_.transform_.position_);
	}

	private void zako_render_update(int front, ref DrawBuffer draw_buffer)
	{
		draw_buffer.regist(ref rigidbody_.transform_, DrawBuffer.Type.Zako);
		if (rigidbody_.transform_.position_.y > 0f) {
			Sight.Instance.regist(front, ref rigidbody_.transform_.position_);
		}
	}
}

} // namespace UTJ {

/*
 * End of Enemy_zako.cs
 */

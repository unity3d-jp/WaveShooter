/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UTJ {

class DragonNode {
	public RigidbodyTransform rigidbody_;
	public int collider_;
	public int collider_homing_;
	
	private int idx_;
	private Vector3 locator_;
	private DrawBuffer.Type draw_type_; 

	private void calc_lock_position(ref Vector3 position)
	{
		position = rigidbody_.transform_.position_;
	}

	public void init(Task task,
					 int idx,
					 ref Vector3 position,
					 ref Quaternion rotation,
					 DrawBuffer.Type draw_type)
	{
		rigidbody_.init(ref position, ref rotation);
		rigidbody_.setRotateDamper(8);
		collider_ = MyCollider.createEnemy();
		MyCollider.initSphereEnemy(collider_, ref position, 1.5f /* radius */);
		collider_homing_ = MyCollider.createEnemyHoming();
		MyCollider.initSphereEnemyHoming(collider_homing_, ref rigidbody_.transform_.position_, 1.5f /* radius */);
		idx_ = idx;
		locator_ = (idx_ == 0 ?
					new Vector3(0f, 0f, 0f) :
					new Vector3(0f, 0f, -3f));
		draw_type_ = draw_type;
	}

	public void reset(ref MyTransform parent_transform)
	{
		var pos = parent_transform.transformPosition(ref locator_);
		rigidbody_.transform_.position_ = pos;
		rigidbody_.transform_.rotation_ = parent_transform.rotation_;
		rigidbody_.velocity_ = CV.Vector3Zero;
		rigidbody_.r_velocity_ = CV.Vector3Zero;
	}

	public void update(float dt,
					   double update_time,
					   ref RigidbodyTransform grand_parent_rigidbody,
					   ref RigidbodyTransform parent_rigidbody,
					   ref RigidbodyTransform child_rigidbody,
					   bool head,
					   bool tail)
	{
#if false
		if (MyCollider.getHitOpponentForEnemy(collider_) != MyCollider.Type.None) {
			rigidbody_.addTorque(MyRandom.Range(-20f, 20f),
								 MyRandom.Range(-20f, 20f),
								 MyRandom.Range(-20f, 20f));
		}
		rigidbody_.addSpringTorque(ref parent_transform.rotation_, 10f /* torque_level */);
		rigidbody_.transform_.position_ = parent_transform.transformPosition(ref locator_);
		rigidbody_.update(dt);
#else
		if (head)
			rigidbody_.updateVerlet(dt, 0.8f /* damper */);
		else
			rigidbody_.updateVerlet(dt, 0.8f /* damper */);
		if (head) {
			var head_offset = new Vector3(0f, 0f, -3f);
			var pos = parent_rigidbody.transform_.transformPosition(ref head_offset);
			rigidbody_.restrictPositionVerletFixed(ref pos, 0f /* length */);
		} else {
			rigidbody_.restrictPositionVerlet(ref grand_parent_rigidbody,
											  ref parent_rigidbody,
											  3f /* length */, 60f /* max_degree */);
		}
		if (!tail) {
			rigidbody_.solveRotationVerlet(ref parent_rigidbody, ref child_rigidbody);
		}
#endif
		MyCollider.updateEnemy(collider_, ref rigidbody_.transform_.position_);
		MyCollider.updateEnemyHoming(collider_homing_, ref rigidbody_.transform_.position_);

		if (-2f < rigidbody_.transform_.position_.y && rigidbody_.transform_.position_.y < 2f) {
			WaterSurface.Instance.makeBump(ref rigidbody_.transform_.position_, -0.025f /* value */, 1f /* size */);
			var wpos = rigidbody_.transform_.position_;
			wpos.y = -1.5f;
			var th = MyRandom.Range(0f, Mathf.PI*2f);
			var cos = Mathf.Cos(th);
			var sin = Mathf.Sin(th);
			wpos.x += cos * 1.25f;
			wpos.z += sin * 1.25f;
			var vel = new Vector3(cos * 0.8f,
								  MyRandom.Range(5f, 8f),
								  sin * 0.8f);
			WaterSplash.Instance.spawn(ref wpos, ref vel, update_time);
		}
		if (MyRandom.Probability(0.1f)) {
			var pos = rigidbody_.transform_.position_;
			pos.x += MyRandom.Range(-1f, 1f);
			pos.z += MyRandom.Range(-1f, 1f);
			WaterSplash.Instance.spawn(ref pos,
									   ref CV.Vector3Zero, update_time-0.2f);
		}
	}

	public void renderUpdate(int front, ref DrawBuffer draw_buffer)
	{
		draw_buffer.regist(ref rigidbody_.transform_, draw_type_);
	}
}

public class Dragon {
	public enum Mode {
		Attack,
		Chase,
		Farewell,
		LastAttack,
	}

	const int NODE_NUM = 8;
	private DragonNode[] nodes_;
	public Mode mode_;
	// public LightBall lightball_;
	public bool is_charging_ = false;

	public void createPool()
	{
		nodes_ = new DragonNode[NODE_NUM];
		for (var i = 0; i < nodes_.Length; ++i) {
			nodes_[i] = new DragonNode();
		}		
	}

	public void init(Task task,
					 ref Vector3 position,
					 ref Quaternion rotation)
	{
		for (var i = 0; i < nodes_.Length; ++i) {
			var pos = position + rotation * new Vector3(0f, 0f, -3f*i);
			DrawBuffer.Type draw_type;
			if (i < nodes_.Length - 2) {
				draw_type = DrawBuffer.Type.DragonBody;
			} else if (i == nodes_.Length - 2) {
				draw_type = DrawBuffer.Type.DragonTail;
			} else {
				draw_type = DrawBuffer.Type.Empty;
			}
			nodes_[i].init(task, i, ref pos, ref rotation, draw_type);
		}
		mode_ = Mode.Attack;
		// lightball_.init();
		is_charging_ = false;
	}

	public void reset(ref MyTransform head_transform)
	{
		for (var i = 0; i < nodes_.Length; ++i) {
			if (i == 0) {
				nodes_[i].reset(ref head_transform);
			} else {
				nodes_[i].reset(ref nodes_[i-1].rigidbody_.transform_);
			}
		}
	}

	public void update(float dt, double update_time, ref RigidbodyTransform head_rigidbody)
	{
		for (var i = 0; i < nodes_.Length; ++i) {
			if (i == 0) {
				nodes_[i].update(dt, update_time,
								 ref head_rigidbody /* dummy */,
								 ref head_rigidbody,
								 ref nodes_[i+1].rigidbody_,
								 true /* head */,
								 false /* tail */);
			} else if (i == 1) {
				nodes_[i].update(dt, update_time, 
								 ref head_rigidbody,
								 ref nodes_[i-1].rigidbody_, 
								 ref nodes_[i+1].rigidbody_,
								 false /* head */,
								 false /* tail */);
			} else if (i <= nodes_.Length - 2) {
				nodes_[i].update(dt, update_time, 
								 ref nodes_[i-2].rigidbody_, 
								 ref nodes_[i-1].rigidbody_, 
								 ref nodes_[i+1].rigidbody_,
								 false /* head */,
								 false /* tail */);
			} else {
				nodes_[i].update(dt, update_time,
								 ref nodes_[i-2].rigidbody_, 
								 ref nodes_[i-1].rigidbody_, 
								 ref nodes_[i-1].rigidbody_ /* dummy */,
								 false /* head */,
								 true /* tail */);
			}
		}
		// lightball_.update(dt);
	}

	public void renderUpdate(int front, ref MyTransform head_transform, ref DrawBuffer draw_buffer)
	{
		for (var i = 0; i < nodes_.Length; ++i) {
			nodes_[i].renderUpdate(front, ref draw_buffer);
		}
		// if (is_charging_) {
		// 	var offset = new Vector3(0, 0, 2.5f);
		// 	lightball_.renderUpdate(front, ref head_transform, ref offset);
		// }
	}
}

public partial class Enemy : Task
{
	private Dragon dragon_;
	private Vector3 lock_point_offset_ = new Vector3(0f, 0f, 2.5f);
	// private Vector3 MUZZLE_OFFSET = new Vector3(0f, 0f, 2.5f);
	// private Vector3 HEAD_OFFSET = new Vector3(0f, 0f, -3f);

	public void setMode(Dragon.Mode mode)
	{
		dragon_.mode_ = mode;
	}

	private void calc_lock_position_dragon(ref Vector3 position)
	{
		position = rigidbody_.transform_.transformPosition(ref lock_point_offset_);
	}

	public void dragon_init(ref Vector3 position, ref Quaternion rotation)
	{
		rigidbody_.init(ref position, ref rotation);
		rigidbody_.setDamper(2f);
		rigidbody_.setRotateDamper(20f);
		collider_ = MyCollider.createEnemy();
		MyCollider.initSphereEnemy(collider_, ref position, 1.5f /* radius */);
		collider_homing_ = MyCollider.createEnemyHoming();
		MyCollider.initSphereEnemyHoming(collider_homing_, ref rigidbody_.transform_.position_, 5f /* radius */);
		enumerator_ = dragon_act(); // この瞬間は実行されない
		on_update_ = new OnUpdateFunc(dragon_update);
		on_render_update_ = new OnRenderUpdateFunc(dragon_render_update);
		life_ = 10000000f;
		dragon_ = dragon_pool_;
		dragon_.init(this, ref rigidbody_.transform_.position_, ref rigidbody_.transform_.rotation_);
	}

	private void limit_target(ref Vector3 position, float limit)
	{
		float len = Mathf.Sqrt(position.x*position.x + position.y*position.y);
		if (len > limit) {
			float r = limit/len;
			position.x *= r;
			position.y *= r;
		}
	}

	private void reset(ref Vector3 position, ref Quaternion rotation)
	{
		rigidbody_.transform_.position_ = position;
		rigidbody_.velocity_ = CV.Vector3Zero;
		rigidbody_.transform_.rotation_ = rotation;
		rigidbody_.r_velocity_ = CV.Vector3Zero;
		dragon_.reset(ref rigidbody_.transform_);
	}

	public IEnumerator dragon_act()
	{
		for (;;) {
			while (rigidbody_.transform_.position_.y < 0f) {
				rigidbody_.addRelativeForceZ(20f);
				var target = new Vector3(0f, 0f, 0f);
				rigidbody_.addTargetTorque(ref target, 20f);
				yield return null;
			}
			for (var i = new Utility.WaitForSeconds(4f, update_time_); !i.end(update_time_);) {
				rigidbody_.addSpringForceY(8f, 2f);
				rigidbody_.addRelativeTorqueZ(25f);
				var target = Player.Instance.rigidbody_.transform_.position_;
				rigidbody_.addSpringTorque(ref target, 20f);
				yield return null;
			}
			for (var i = new Utility.WaitForSeconds(4f, update_time_); !i.end(update_time_);) {
				rigidbody_.addSpringForceY(8f, 2f);
				var target = Player.Instance.rigidbody_.transform_.position_;
				rigidbody_.addSpringTorque(ref target, 20f);
				yield return null;
			}
			for (var i = new Utility.WaitForSeconds(4f, update_time_); !i.end(update_time_);) {
				rigidbody_.addSpringForceY(4f, 3f);
				// rigidbody_.addRelativeForceX(5f);
				var target = Player.Instance.rigidbody_.transform_.position_;
				rigidbody_.addSpringTorque(ref target, 20f);
				yield return null;
			}
			for (var i = new Utility.WaitForSeconds(1f, update_time_); !i.end(update_time_);) {
				rigidbody_.addRelativeForceZ(20f);
				var target = new Vector3(0f, 20f, 0f);
				rigidbody_.addTargetTorque(ref target, 10f);
				yield return null;
			}
			for (var i = new Utility.WaitForSeconds(4f, update_time_); !i.end(update_time_);) {
				rigidbody_.addRelativeForceZ(40f);
				var target = new Vector3(0f, -20f, 0f);
				rigidbody_.addTargetTorque(ref target, 20f);
				yield return null;
			}
			yield return null;
		}
	}

	private void dragon_update(float dt)
	{
		if (MyCollider.getHitOpponentForEnemy(collider_) != MyCollider.Type.None) {
			float p = 100f;
			rigidbody_.addTorque(MyRandom.Range(-p, p),
								 MyRandom.Range(-p, p),
								 MyRandom.Range(-p, p));
			life_ -= 20f;
		}
		if (life_ <= 0f && phase_ == Phase.Alive) {
			MyCollider.disableForEnemy(collider_);
			phase_ = Phase.Dying;
		}
		
		rigidbody_.update(dt);
		MyCollider.updateEnemy(collider_, ref rigidbody_.transform_.position_);
		MyCollider.updateEnemyHoming(collider_homing_, ref rigidbody_.transform_.position_);
		// var head_transform = rigidbody_.transform_.add(ref HEAD_OFFSET);
		dragon_.update(dt, update_time_, ref rigidbody_);

		if (MyRandom.Probability(0.01f)) {
			var head_pos = new Vector3(0f, 0f, 2f);
			var pos = rigidbody_.transform_.transformPosition(ref head_pos);
			if (pos.y > 0f) {
				EnemyLaser.create(ref pos, ref rigidbody_.transform_.rotation_, 40f /* speed */, update_time_);
				SystemManager.Instance.registSound(DrawBuffer.SE.Laser);
			}
		}

		if (MyRandom.Probability(0.1f)) {
			var pos = rigidbody_.transform_.position_;
			pos.x += MyRandom.Range(-1f, 1f);
			pos.z += MyRandom.Range(-1f, 1f);
			WaterSplash.Instance.spawn(ref pos,
									   ref CV.Vector3Zero, update_time_-0.2f);
		}

		if (-1f < rigidbody_.transform_.position_.y && rigidbody_.transform_.position_.y < 1f) {
			WaterSurface.Instance.makeBump(ref rigidbody_.transform_.position_, -0.1f /* value */, 1f /* size */);
			if (MyRandom.Probability(0.1f)) {
				var wpos = rigidbody_.transform_.position_;
				wpos.y = -2f;
				var vel = new Vector3(0f, MyRandom.Range(5f, 8f), 0f);
				WaterSplash.Instance.spawn(ref wpos, ref vel, update_time_);
			}
		}
	}

	private void dragon_render_update(int front, ref DrawBuffer draw_buffer)
	{
		draw_buffer.regist(ref rigidbody_.transform_, DrawBuffer.Type.DragonHead);
		dragon_.renderUpdate(front, ref rigidbody_.transform_, ref draw_buffer);
	}

}

} // namespace UTJ {

/*
 * End of Enemy_dragon.cs
 */

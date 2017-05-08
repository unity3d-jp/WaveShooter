/* -*- mode:CSharp:utf-8-with-signature -*-
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ {

public class Player : Task {
	// singleton
	static Player instance_;
	public static Player Instance { get { return instance_ ?? (instance_ = new Player()); } }
	
	private Posture posture_apose_;
	private Posture posture_pre_throw_l_arm_;
	private Posture posture_pre_throw_r_arm_;
	private Posture posture_throw_l_arm_;
	private Posture posture_throw_r_arm_;
	private Posture posture_pre_jump_;
	private int throwing_cnt_l_;
	private int throwing_cnt_r_;
	private float jump_tame_duration_;
	private float jump_propel_remain_;
	private bool on_ground_;
	private float on_ground_time_;
	private float bullet_tame_left_;
	private float bullet_tame_right_;
	// private float hit_time_;
	// private Vector3 hit_position_;
	private bool somersault_;

	private MuscleMotion muscle_motion_;
	public RigidbodyTransform rigidbody_;
	private int collider_;
	private Vector3 look_at_;

	private Bullet left_held_bullet_; 
	private Bullet right_held_bullet_; 

	public MuscleMotion getMuscleMotion() { return muscle_motion_; }
	public float TameGaugeValue() { return Mathf.Clamp(jump_tame_duration_*2f, 0f, 1f); }
	public float getAuraValue() { return jump_tame_duration_ > 0f || !on_ground_ ? 1f : 0f; }

	public IEnumerator initialize()
	{
		base.init();
		
		yield return FileUtil.preparePath("apose.dat");
		posture_apose_ = JsonUtility.FromJson<Posture>(FileUtil.content);
		Debug.Assert(posture_apose_ != null);
		yield return FileUtil.preparePath("pre_throw_l_arm.dat");
		posture_pre_throw_l_arm_ = JsonUtility.FromJson<Posture>(FileUtil.content);
		Debug.Assert(posture_pre_throw_l_arm_ != null);
		yield return FileUtil.preparePath("pre_throw_r_arm.dat");
		posture_pre_throw_r_arm_ = JsonUtility.FromJson<Posture>(FileUtil.content);
		Debug.Assert(posture_pre_throw_r_arm_ != null);
		yield return FileUtil.preparePath("throw_l_arm.dat");
		posture_throw_l_arm_ = JsonUtility.FromJson<Posture>(FileUtil.content);
		Debug.Assert(posture_throw_l_arm_ != null);
		yield return FileUtil.preparePath("throw_r_arm.dat");
		posture_throw_r_arm_ = JsonUtility.FromJson<Posture>(FileUtil.content);
		Debug.Assert(posture_throw_r_arm_ != null);
		yield return FileUtil.preparePath("pre_jump.dat");
		posture_pre_jump_ = JsonUtility.FromJson<Posture>(FileUtil.content);
		Debug.Assert(posture_throw_r_arm_ != null);
		throwing_cnt_l_ = 0;
		throwing_cnt_r_ = 0;
		jump_tame_duration_ = 0f;
		jump_propel_remain_ = 0f;
		on_ground_ = true;
		on_ground_time_ = 100f;
		bullet_tame_left_ = 0f;
		bullet_tame_right_ = 0f;
		// hit_time_ = 0f;
		somersault_ = false;

		muscle_motion_ = new MuscleMotion();
		muscle_motion_.init(posture_apose_, 80f /* damper */, 1500f /* spring */);
		MuscleMotion.Node root_node = muscle_motion_.getRootNode();
		look_at_ = new Vector3(0f, 1f, 0f);
		rigidbody_.setPosition(0f, 1f, 15f);
		var rot = Quaternion.LookRotation(look_at_ - rigidbody_.transform_.position_);
		rigidbody_.setRotation(ref rot);
		rigidbody_.setDamper(10f);
		rigidbody_.setRotateDamper(50f);
		root_node.rigidbody_.setDamper(10f);
		root_node.rigidbody_.setRotateDamper(40f);

		collider_ = MyCollider.createPlayer();
		MyCollider.initSpherePlayer(collider_, ref rigidbody_.transform_.position_, 1f /* radius */);

		// muscle_motion_.fix(MuscleMotion.Parts.Ribs, 0.4f /* interpolate_ratio */);
		// muscle_motion_.fix(MuscleMotion.Parts.Ribs2);
		// muscle_motion_.fix(MuscleMotion.Parts.Ribs3, 0.4f /* interpolate_ratio */);
		// muscle_motion_.fix(MuscleMotion.Parts.Hip, 0.1f /* interpolate_ratio */);
		{   /* body */
			float damper = 60f;
			float spring_ratio = 800f;
			muscle_motion_.setParams(MuscleMotion.Parts.Ribs, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.Ribs2, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.Ribs3, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.Hip, damper, spring_ratio);
		}
		{   /* arms */
			float damper = 50f;
			float spring_ratio = 1000f;
			muscle_motion_.setParams(MuscleMotion.Parts.L_Shoulder, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.L_UpperArm, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.L_ForeArm, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.L_Wrist, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.R_Shoulder, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.R_UpperArm, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.R_ForeArm, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.R_Wrist, damper, spring_ratio);
		}
		{   /* legs */
			float damper = 30f;
			float spring_ratio = 500f;
			muscle_motion_.setParams(MuscleMotion.Parts.L_Thigh, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.L_Knee, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.L_Ancle, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.L_Toe, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.R_Thigh, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.R_Knee, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.R_Ancle, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.R_Toe, damper, spring_ratio);
		}
		{   /* tales */
			float damper = 10f;
			float spring_ratio = 200f;
			muscle_motion_.setParams(MuscleMotion.Parts.L_Tale1, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.L_Tale2, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.L_Tale3, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.L_Tale4, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.R_Tale1, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.R_Tale2, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.R_Tale3, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.R_Tale4, damper, spring_ratio);
		}
		{   /* ribbons */
			float damper = 2f;
			float spring_ratio = 100f;
			muscle_motion_.setParams(MuscleMotion.Parts.L_Ribbon1, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.L_Ribbon2, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.R_Ribbon1, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.R_Ribbon2, damper, spring_ratio);
		}
		{   /* suso */
			float damper = 10f;
			float spring_ratio = 200f;
			muscle_motion_.setParams(MuscleMotion.Parts.L_SusoBack, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.L_SusoFront, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.R_SusoBack, damper, spring_ratio);
			muscle_motion_.setParams(MuscleMotion.Parts.R_SusoFront, damper, spring_ratio);
		}
	}

	private void fire(Bullet bullet, double update_time)
	{
		const float BULLET_SPEED = 40f;
		var velocity = rigidbody_.transform_.rotation_ * CV.Vector3Forward * BULLET_SPEED;
		bullet.release(ref velocity, update_time);
		SystemManager.Instance.registSound(DrawBuffer.SE.Bullet);
	}
	private void fire_left(double update_time)
	{
		if (left_held_bullet_ != null) {
			fire(left_held_bullet_, update_time);
			left_held_bullet_ = null;
		}
		bullet_tame_left_ = 0f;
	}
	private void fire_right(double update_time)
	{
		if (right_held_bullet_ != null) {
			fire(right_held_bullet_, update_time);
			right_held_bullet_ = null;
		}
		bullet_tame_right_ = 0f;
	}

	public override void update(float dt, double update_time)
	{
		var controller = Controller.Instance.getLatest();
		
		muscle_motion_.setTarget(posture_apose_);
		if (controller.isLeftButtonUp()) {
			throwing_cnt_l_ = (int)(0.5f/dt);
		}
		if (controller.isRightButtonUp()) {
			throwing_cnt_r_ = (int)(0.5f/dt);
		}
		if (throwing_cnt_l_ > 0) {
			muscle_motion_.setTarget(posture_throw_l_arm_);
			// muscle_motion_.addTorqueY(MuscleMotion.Parts.Ribs2, 8000f);
			// muscle_motion_.addTorqueY(MuscleMotion.Parts.Hip, 8000f);
			// muscle_motion_.addTorqueX(MuscleMotion.Parts.L_Thigh, 8000f);
			// muscle_motion_.addTorqueX(MuscleMotion.Parts.L_Knee, 8000f);
			if (throwing_cnt_l_ < (int)(0.45f/dt)) {
				fire_left(update_time);
			}
			--throwing_cnt_l_;
		}
		if (throwing_cnt_r_ > 0) {
			muscle_motion_.setTarget(posture_throw_r_arm_);
			// muscle_motion_.addTorqueY(MuscleMotion.Parts.Ribs2, -8000f);
			// muscle_motion_.addTorqueY(MuscleMotion.Parts.Hip, -8000f);
			// muscle_motion_.addTorqueX(MuscleMotion.Parts.R_Thigh, 8000f);
			// muscle_motion_.addTorqueX(MuscleMotion.Parts.R_Knee, 8000f);
			if (throwing_cnt_r_ < (int)(0.45f/dt)) {
				fire_right(update_time);
			}
			--throwing_cnt_r_;
		}
		if (controller.isLeftButtonDown()) {
			MuscleMotion.Node node = muscle_motion_.getNode(MuscleMotion.Parts.L_Wrist);
			fire_left(update_time);
			left_held_bullet_ = Bullet.create(ref node.rigidbody_.transform_.position_,
											  ref CV.QuaternionIdentity);
		}
		if (controller.isRightButtonDown()) {
			MuscleMotion.Node node = muscle_motion_.getNode(MuscleMotion.Parts.R_Wrist);
			fire_right(update_time);
			right_held_bullet_ = Bullet.create(ref node.rigidbody_.transform_.position_,
											   ref CV.QuaternionIdentity);
		}
		if (controller.isLeftButton()) {
			throwing_cnt_l_ = 0;
			throwing_cnt_r_ -= (int)(0.25f/dt);
			muscle_motion_.setTarget(posture_pre_throw_l_arm_);
			bullet_tame_left_ += dt;
		}
		if (controller.isRightButton()) {
			throwing_cnt_l_ -= (int)(0.25f/dt);
			throwing_cnt_r_ = 0;
			muscle_motion_.setTarget(posture_pre_throw_r_arm_);
			bullet_tame_right_ += dt;
		}
		if (left_held_bullet_ != null) {
			MuscleMotion.Node node = muscle_motion_.getNode(MuscleMotion.Parts.L_Wrist);
			left_held_bullet_.setPosition(ref node.rigidbody_.transform_.position_);
			left_held_bullet_.setPower(Mathf.Clamp(bullet_tame_left_, 0.25f, 2f));
		}
		if (right_held_bullet_ != null) {
			MuscleMotion.Node node = muscle_motion_.getNode(MuscleMotion.Parts.R_Wrist);
			right_held_bullet_.setPosition(ref node.rigidbody_.transform_.position_);
			right_held_bullet_.setPower(Mathf.Clamp(bullet_tame_right_, 0.25f, 2f));
		}

		if (controller.isJumpButton() && on_ground_) {
			muscle_motion_.getRootNode().rigidbody_.addRelativeTorqueX(1000f);
			muscle_motion_.setTarget(posture_pre_jump_,
									 MuscleMotion.PartsBit.LowerBody |
									 MuscleMotion.PartsBit.Ribs |
									 MuscleMotion.PartsBit.Ribs2 |
									 MuscleMotion.PartsBit.Ribs3);
			jump_tame_duration_ += dt;
		}

		if (controller.isJumpButtonUp()) {
			if (jump_tame_duration_ > 0.5f) {
				jump_propel_remain_ = Mathf.Min((jump_tame_duration_ - 0.5f) + 0.5f, 1f) * 2f;
				rigidbody_.addForceY(1000f);
				WaterSurface.Instance.makeBump(ref rigidbody_.transform_.position_, 
											   -1f /* value */, 1f /* size */);
				on_ground_time_ = 0f;
				somersault_ = MyRandom.Probability(0.25f);
			}
			jump_tame_duration_ = 0f;
		}

		float hori = controller.getHorizontal();
		float ground_height = 1.25f;
		if (hori != 0f) {
			ground_height = 1f;
		}

		if (jump_propel_remain_ > 0f) {
			rigidbody_.addForceY(100f);
			jump_propel_remain_ -= dt;
		}
		on_ground_ = (rigidbody_.transform_.position_.y <= 1f);

		rigidbody_.addTargetTorque(ref look_at_, 500f /* torque_level */, -1f /* max_level */);
		rigidbody_.addRelativeForceX(hori * 64f);
		if (hori != 0f) {
			rigidbody_.addRelativeForceZ(10f);
		}
		rigidbody_.addForceY(-9.8f*5f);	// gravity
		if (rigidbody_.transform_.position_.y < ground_height) {
			rigidbody_.addSpringForceY(ground_height, 100f);
		}
		rigidbody_.solveForGround(0.0f /* ground_height */, dt);
		rigidbody_.addRelativeTorqueZ(-hori * 100f);
		{
			var forward = rigidbody_.transform_.rotation_ * CV.Vector3Forward;
			var q = Quaternion.LookRotation(forward);
			rigidbody_.addSpringTorque(ref q, 10000f);
		}
		rigidbody_.update(dt);

		if (rigidbody_.transform_.position_.y < 5f) {
			WaterSurface.Instance.makeBump(ref rigidbody_.transform_.position_, -0.05f /* value */, 0.6f /* size */);
			var pos = rigidbody_.transform_.position_;
			pos.y = -2f;
			float vel_y;
			if (hori != 0f) {
				vel_y = MyRandom.Range(7f, 9f);
			} else {
				vel_y = MyRandom.Range(5f, 7f);
			}
			var vel = new Vector3(0f,
								  vel_y,
								  0f);
			if (MyRandom.Probability(0.2f)) {
				WaterSplash.Instance.spawn(ref pos, ref vel, update_time);
			}
		}

		var root_node = muscle_motion_.getRootNode();
		root_node.rigidbody_.transform_.position_ = 
			rigidbody_.transform_.position_ + new Vector3((Mathf.PerlinNoise((float)update_time*4f, 0.0f)-0.5f)*0.04f,
														  (Mathf.PerlinNoise((float)update_time*4f, 0.5f)-0.5f)*0.04f,
														  (Mathf.PerlinNoise((float)update_time*4f, 1.0f)-0.5f)*0.04f);
														  
		if (somersault_ && on_ground_time_ < 0.25f) {
			root_node.rigidbody_.addRelativeTorqueX(-3000f);
			muscle_motion_.getNode(MuscleMotion.Parts.Ribs).rigidbody_.addRelativeTorqueX(-3000f);
			muscle_motion_.getNode(MuscleMotion.Parts.Ribs2).rigidbody_.addRelativeTorqueX(-3000f);
			muscle_motion_.getNode(MuscleMotion.Parts.Ribs3).rigidbody_.addRelativeTorqueX(-3000f);
			muscle_motion_.getNode(MuscleMotion.Parts.Hip).rigidbody_.addRelativeTorqueX(-3000f);
		} else {
			root_node.rigidbody_.addTorqueY(hori*1000f);
			root_node.rigidbody_.addRelativeTorqueZ(-hori*1000f);
			root_node.rigidbody_.addSpringTorque(ref rigidbody_.transform_.rotation_, 4000f);
		}

		muscle_motion_.addTorqueX(MuscleMotion.Parts.L_Tale1, MyRandom.Range(500f, 1200f));
		muscle_motion_.addTorqueX(MuscleMotion.Parts.L_Tale4, MyRandom.Range(-4000f, 4000f));
		muscle_motion_.addTorqueX(MuscleMotion.Parts.R_Tale1, MyRandom.Range(500f, 1200f));
		muscle_motion_.addTorqueX(MuscleMotion.Parts.R_Tale4, MyRandom.Range(-4000f, 4000f));
		muscle_motion_.addTorqueX(MuscleMotion.Parts.L_SusoBack, MyRandom.Range(300f, 600f));
		muscle_motion_.addTorqueX(MuscleMotion.Parts.L_SusoFront, MyRandom.Range(-600f, -300f));
		muscle_motion_.addTorqueX(MuscleMotion.Parts.R_SusoBack, MyRandom.Range(300f, 600f));
		muscle_motion_.addTorqueX(MuscleMotion.Parts.R_SusoFront, MyRandom.Range(-600f, -300f));

		Vector3 e_pos;
		if (jump_tame_duration_ > 0.75f || on_ground_time_ < 1f) {
			var node = muscle_motion_.getNode(MuscleMotion.Parts.Head);
			node.rigidbody_.addRelativeTorqueX(-4000f);
		} else if (MyCollider.getNearestEnemyPosition(out e_pos)) {
			muscle_motion_.getNode(MuscleMotion.Parts.Head).rigidbody_.addSpringTorque(ref e_pos, 4000f);
		}

		{
			var intersect_point = CV.Vector3Zero;
			if (MyCollider.getHitOpponentForPlayer(collider_, ref intersect_point) == MyCollider.Type.EnemyBullet) {
				var node = muscle_motion_.getNode(MuscleMotion.Parts.Ribs3);
				var torque = MyRandom.onSphere(1f)*5000f;
				node.rigidbody_.addTorque(ref torque);
				Shield.Instance.spawn(ref intersect_point,
									  ref rigidbody_.transform_.position_,
									  update_time,
									  Shield.Type.Green);
				SystemManager.Instance.registSound(DrawBuffer.SE.Shield);
				// hit_time_ = (float)update_time;
				// hit_position_ = intersect_point;
			}
			MyCollider.updatePlayer(collider_, ref rigidbody_.transform_.position_);
		}

		muscle_motion_.update(dt);
		on_ground_time_ += dt;
	}

	public override void renderUpdate(int front, CameraBase camra, ref DrawBuffer draw_buffer)
	{
		muscle_motion_.renderUpdate(ref draw_buffer, DrawBuffer.Type.MuscleMotionPlayer);
	}
}

} // namespace UTJ {

/*
 * End of Player.cs
 */

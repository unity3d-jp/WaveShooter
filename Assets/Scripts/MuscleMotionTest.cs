/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ {

public class MuscleMotionTest : MonoBehaviour {

	public GameObject lookat_sphere_;

	private Posture posture_apose_;
	private Posture posture_pre_throw_l_arm_;
	private Posture posture_pre_throw_r_arm_;
	private Posture posture_throw_l_arm_;
	private Posture posture_throw_r_arm_;
	private Posture posture_pre_jump_;
	private MuscleMotion muscle_motion_;

	private float body_damper_ = 60f;
	private float body_spring_ = 800f;
	private float arm_damper_ = 50f;
	private float arm_spring_ = 1000f;
	private float leg_damper_ = 30f;
	private float leg_spring_ = 500f;

	private Posture current_posture_;
	private float gui_lookat_ = 0f;
	private bool gui_hand_ = false;
	private bool gui_rotate_ = false;

	IEnumerator Start()
	{
		yield return FileUtil.preparePath("apose.dat");
		posture_apose_ = JsonUtility.FromJson<Posture>(FileUtil.content);
		yield return FileUtil.preparePath("pre_throw_l_arm.dat");
		posture_pre_throw_l_arm_ = JsonUtility.FromJson<Posture>(FileUtil.content);
		yield return FileUtil.preparePath("pre_throw_r_arm.dat");
		posture_pre_throw_r_arm_ = JsonUtility.FromJson<Posture>(FileUtil.content);
		yield return FileUtil.preparePath("throw_l_arm.dat");
		posture_throw_l_arm_ = JsonUtility.FromJson<Posture>(FileUtil.content);
		yield return FileUtil.preparePath("throw_r_arm.dat");
		posture_throw_r_arm_ = JsonUtility.FromJson<Posture>(FileUtil.content);
		yield return FileUtil.preparePath("pre_jump.dat");
		posture_pre_jump_ = JsonUtility.FromJson<Posture>(FileUtil.content);

		muscle_motion_ = new MuscleMotion();
		muscle_motion_.init(posture_apose_, 80f /* damper */, 1500f /* spring */);

		set_params_body(body_damper_, body_spring_);
		set_params_arm(arm_damper_, arm_spring_);
		set_params_leg(leg_damper_, leg_spring_);

		{   /* tales */
			float damper = 4f;
			float spring_ratio = 40f;
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

		current_posture_ = posture_apose_;
	}

	void set_params_body(float damper, float spring_ratio)
	{
		muscle_motion_.setParams(MuscleMotion.Parts.Ribs, damper, spring_ratio);
		muscle_motion_.setParams(MuscleMotion.Parts.Ribs2, damper, spring_ratio);
		muscle_motion_.setParams(MuscleMotion.Parts.Ribs3, damper, spring_ratio);
		muscle_motion_.setParams(MuscleMotion.Parts.Hip, damper, spring_ratio);
	}

	void set_params_arm(float damper, float spring_ratio)
	{
		muscle_motion_.setParams(MuscleMotion.Parts.L_Shoulder, damper, spring_ratio);
		muscle_motion_.setParams(MuscleMotion.Parts.L_UpperArm, damper, spring_ratio);
		muscle_motion_.setParams(MuscleMotion.Parts.L_ForeArm, damper, spring_ratio);
		muscle_motion_.setParams(MuscleMotion.Parts.L_Wrist, damper, spring_ratio);
		muscle_motion_.setParams(MuscleMotion.Parts.R_Shoulder, damper, spring_ratio);
		muscle_motion_.setParams(MuscleMotion.Parts.R_UpperArm, damper, spring_ratio);
		muscle_motion_.setParams(MuscleMotion.Parts.R_ForeArm, damper, spring_ratio);
		muscle_motion_.setParams(MuscleMotion.Parts.R_Wrist, damper, spring_ratio);
	}

	void set_params_leg(float damper, float spring_ratio)
	{
		muscle_motion_.setParams(MuscleMotion.Parts.L_Thigh, damper, spring_ratio);
		muscle_motion_.setParams(MuscleMotion.Parts.L_Knee, damper, spring_ratio);
		muscle_motion_.setParams(MuscleMotion.Parts.L_Ancle, damper, spring_ratio);
		muscle_motion_.setParams(MuscleMotion.Parts.L_Toe, damper, spring_ratio);
		muscle_motion_.setParams(MuscleMotion.Parts.R_Thigh, damper, spring_ratio);
		muscle_motion_.setParams(MuscleMotion.Parts.R_Knee, damper, spring_ratio);
		muscle_motion_.setParams(MuscleMotion.Parts.R_Ancle, damper, spring_ratio);
		muscle_motion_.setParams(MuscleMotion.Parts.R_Toe, damper, spring_ratio);
	}

	bool update_parts(Transform tfm, string name, MuscleMotion.Parts parts)
	{
		if (tfm.gameObject.name == name) {
			var node = muscle_motion_.getNode(parts);
			tfm.transform.position = node.rigidbody_.transform_.position_;
			tfm.transform.rotation = node.rigidbody_.transform_.rotation_ * node.original_rotation_;
			return true;
		} else {
			return false;
		}
	}

	void Update()
	{
		if (muscle_motion_ == null) {
			return;
		}

		set_params_body(body_damper_, body_spring_);
		set_params_arm(arm_damper_, arm_spring_);
		set_params_leg(leg_damper_, leg_spring_);

		muscle_motion_.setTarget(current_posture_);

		if (lookat_sphere_ != null) {
			var radius = 0.5f;
			lookat_sphere_.transform.position = new Vector3(Mathf.Sin(Time.time*2f) * radius,
															Mathf.Cos(Time.time*2.5f) * radius,
															Mathf.Cos(Time.time*1f) * radius) + new Vector3(0f, 0.5f, 0f);
		}

		if (gui_hand_ || gui_lookat_ > 0f) {
			lookat_sphere_.SetActive(true);
		} else {
			lookat_sphere_.SetActive(false);
		}

		if (gui_lookat_ > 0f) {
			var node_head = muscle_motion_.getNode(MuscleMotion.Parts.Head);
			var pos = lookat_sphere_.transform.position;
			node_head.rigidbody_.addTargetTorque(ref pos, gui_lookat_ * 10000f);
		}

		if (lookat_sphere_ != null && gui_hand_) {
			var diff = lookat_sphere_.transform.position - muscle_motion_.getNode(MuscleMotion.Parts.Head).rigidbody_.transform_.position_;
			diff = Quaternion.Inverse(muscle_motion_.getRootNode().rigidbody_.transform_.rotation_) * diff;
			if (diff.x < 0) {
				var node_upper_arm = muscle_motion_.getNode(MuscleMotion.Parts.L_UpperArm);
				var node_fore_arm = muscle_motion_.getNode(MuscleMotion.Parts.L_ForeArm);
				var node_wrist = muscle_motion_.getNode(MuscleMotion.Parts.L_Wrist);
				var rot = Quaternion.LookRotation(diff);
				rot = rot * Quaternion.Euler(40, 90, 0);
				node_upper_arm.setTarget(ref rot);
				node_fore_arm.setTargetDirect(ref CV.QuaternionIdentity);
				var rot0 = Quaternion.Euler(0, 0, -45);
				node_wrist.setTargetDirect(ref rot0);
			} else {
				var node_upper_arm = muscle_motion_.getNode(MuscleMotion.Parts.R_UpperArm);
				var node_fore_arm = muscle_motion_.getNode(MuscleMotion.Parts.R_ForeArm);
				var node_wrist = muscle_motion_.getNode(MuscleMotion.Parts.R_Wrist);
				var rot = Quaternion.LookRotation(diff);
				rot = rot * Quaternion.Euler(0, 90, 0);
				node_upper_arm.setTarget(ref rot);
				node_fore_arm.setTargetDirect(ref CV.QuaternionIdentity);
				var rot0 = Quaternion.Euler(0, 0, 45);
				node_wrist.setTargetDirect(ref rot0);
			}
		}

		if (gui_rotate_) {
			muscle_motion_.getRootNode().rigidbody_.addTorqueY(400f);
		}
		muscle_motion_.update(Time.deltaTime);

		for (var i = 0; i < transform.childCount; ++i) {
			Transform child = transform.GetChild(i);
			if (update_parts(child, "Hip", MuscleMotion.Parts.Hip)) {
				continue;
			}
			if (update_parts(child, "R_SusoBack", MuscleMotion.Parts.R_SusoBack)) {
				continue;
			}
			if (update_parts(child, "R_SusoFront", MuscleMotion.Parts.R_SusoFront)) {
				continue;
			}
			if (update_parts(child, "R_Thigh", MuscleMotion.Parts.R_Thigh)) {
				continue;
			}
			if (update_parts(child, "R_Knee", MuscleMotion.Parts.R_Knee)) {
				continue;
			}
			if (update_parts(child, "R_Ancle", MuscleMotion.Parts.R_Ancle)) {
				continue;
			}
			if (update_parts(child, "R_Toe", MuscleMotion.Parts.R_Toe)) {
				continue;
			}
			if (update_parts(child, "L_SusoBack", MuscleMotion.Parts.L_SusoBack)) {
				continue;
			}
			if (update_parts(child, "L_SusoFront", MuscleMotion.Parts.L_SusoFront)) {
				continue;
			}
			if (update_parts(child, "L_Thigh", MuscleMotion.Parts.L_Thigh)) {
				continue;
			}
			if (update_parts(child, "L_Knee", MuscleMotion.Parts.L_Knee)) {
				continue;
			}
			if (update_parts(child, "L_Ancle", MuscleMotion.Parts.L_Ancle)) {
				continue;
			}
			if (update_parts(child, "L_Toe", MuscleMotion.Parts.L_Toe)) {
				continue;
			}
			if (update_parts(child, "Ribs", MuscleMotion.Parts.Ribs)) {
				continue;
			}
			if (update_parts(child, "Ribs2", MuscleMotion.Parts.Ribs2)) {
				continue;
			}
			if (update_parts(child, "Ribs3", MuscleMotion.Parts.Ribs3)) {
				continue;
			}
			if (update_parts(child, "R_Shoulder", MuscleMotion.Parts.R_Shoulder)) {
				continue;
			}
			if (update_parts(child, "R_UpperArm", MuscleMotion.Parts.R_UpperArm)) {
				continue;
			}
			if (update_parts(child, "R_ForeArm", MuscleMotion.Parts.R_ForeArm)) {
				continue;
			}
			if (update_parts(child, "R_Wrist", MuscleMotion.Parts.R_Wrist)) {
				continue;
			}
			if (update_parts(child, "R_FingerIndexA", MuscleMotion.Parts.R_FingerIndexA)) {
				continue;
			}
			if (update_parts(child, "R_FingerIndexB", MuscleMotion.Parts.R_FingerIndexB)) {
				continue;
			}
			if (update_parts(child, "R_FingerMiddleA", MuscleMotion.Parts.R_FingerMiddleA)) {
				continue;
			}
			if (update_parts(child, "R_FingerMiddleB", MuscleMotion.Parts.R_FingerMiddleB)) {
				continue;
			}
			if (update_parts(child, "R_FingerPinkyA", MuscleMotion.Parts.R_FingerPinkyA)) {
				continue;
			}
			if (update_parts(child, "R_FingerPinkyB", MuscleMotion.Parts.R_FingerPinkyB)) {
				continue;
			}
			if (update_parts(child, "R_FingerRingA", MuscleMotion.Parts.R_FingerRingA)) {
				continue;
			}
			if (update_parts(child, "R_FingerRingB", MuscleMotion.Parts.R_FingerRingB)) {
				continue;
			}
			if (update_parts(child, "R_FingerThumbA", MuscleMotion.Parts.R_FingerThumbA)) {
				continue;
			}
			if (update_parts(child, "R_FingerThumbB", MuscleMotion.Parts.R_FingerThumbB)) {
				continue;
			}
			if (update_parts(child, "Neck", MuscleMotion.Parts.Neck)) {
				continue;
			}
			if (update_parts(child, "Head", MuscleMotion.Parts.Head)) {
				continue;
			}
			if (update_parts(child, "R_Ribbon1", MuscleMotion.Parts.R_Ribbon1)) {
				continue;
			}
			if (update_parts(child, "R_Ribbon2", MuscleMotion.Parts.R_Ribbon2)) {
				continue;
			}
			if (update_parts(child, "R_Tale1", MuscleMotion.Parts.R_Tale1)) {
				continue;
			}
			if (update_parts(child, "R_Tale2", MuscleMotion.Parts.R_Tale2)) {
				continue;
			}
			if (update_parts(child, "R_Tale3", MuscleMotion.Parts.R_Tale3)) {
				continue;
			}
			if (update_parts(child, "R_Tale4", MuscleMotion.Parts.R_Tale4)) {
				continue;
			}
			if (update_parts(child, "L_Ribbon1", MuscleMotion.Parts.L_Ribbon1)) {
				continue;
			}
			if (update_parts(child, "L_Ribbon2", MuscleMotion.Parts.L_Ribbon2)) {
				continue;
			}
			if (update_parts(child, "L_Tale1", MuscleMotion.Parts.L_Tale1)) {
				continue;
			}
			if (update_parts(child, "L_Tale2", MuscleMotion.Parts.L_Tale2)) {
				continue;
			}
			if (update_parts(child, "L_Tale3", MuscleMotion.Parts.L_Tale3)) {
				continue;
			}
			if (update_parts(child, "L_Tale4", MuscleMotion.Parts.L_Tale4)) {
				continue;
			}
			if (update_parts(child, "L_Shoulder", MuscleMotion.Parts.L_Shoulder)) {
				continue;
			}
			if (update_parts(child, "L_UpperArm", MuscleMotion.Parts.L_UpperArm)) {
				continue;
			}
			if (update_parts(child, "L_ForeArm", MuscleMotion.Parts.L_ForeArm)) {
				continue;
			}
			if (update_parts(child, "L_Wrist", MuscleMotion.Parts.L_Wrist)) {
				continue;
			}
			if (update_parts(child, "L_FingerIndexA", MuscleMotion.Parts.L_FingerIndexA)) {
				continue;
			}
			if (update_parts(child, "L_FingerIndexB", MuscleMotion.Parts.L_FingerIndexB)) {
				continue;
			}
			if (update_parts(child, "L_FingerMiddleA", MuscleMotion.Parts.L_FingerMiddleA)) {
				continue;
			}
			if (update_parts(child, "L_FingerMiddleB", MuscleMotion.Parts.L_FingerMiddleB)) {
				continue;
			}
			if (update_parts(child, "L_FingerPinkyA", MuscleMotion.Parts.L_FingerPinkyA)) {
				continue;
			}
			if (update_parts(child, "L_FingerPinkyB", MuscleMotion.Parts.L_FingerPinkyB)) {
				continue;
			}
			if (update_parts(child, "L_FingerRingA", MuscleMotion.Parts.L_FingerRingA)) {
				continue;
			}
			if (update_parts(child, "L_FingerRingB", MuscleMotion.Parts.L_FingerRingB)) {
				continue;
			}
			if (update_parts(child, "L_FingerThumbA", MuscleMotion.Parts.L_FingerThumbA)) {
				continue;
			}
			if (update_parts(child, "L_FingerThumbB", MuscleMotion.Parts.L_FingerThumbB)) {
				continue;
			}
		}
	}

	void OnGUI()
	{
		int x = 20;
		int y = 60;
		int w = 240;
		int h = 40;
        GUI.Box(new Rect(Screen.width-(x+w+10), y-40, w, y+h*10+10), "Posture Param");
		GUI.Label(new Rect(Screen.width-(x+w), y, w-20, h), "body damper");
		y += h/2;
		float body_damper = GUI.HorizontalSlider(new Rect(Screen.width-(x+w), y, w-20, h), body_damper_, 0f, 100f);
		if (body_damper_ != body_damper) { body_damper_ = body_damper; }
		y += h;
		GUI.Label(new Rect(Screen.width-(x+w), y, w-20, h), "body spring");
		y += h/2;
		float body_spring = GUI.HorizontalSlider(new Rect(Screen.width-(x+w), y, w-20, h), body_spring_, 0f, 2000f);
		if (body_spring_ != body_spring) { body_spring_ = body_spring; }
		y += h;
		GUI.Label(new Rect(Screen.width-(x+w), y, w-20, h), "arm damper");
		y += h/2;
		float arm_damper = GUI.HorizontalSlider(new Rect(Screen.width-(x+w), y, w-20, h), arm_damper_, 0f, 100f);
		if (arm_damper_ != arm_damper) { arm_damper_ = arm_damper; }
		y += h;
		GUI.Label(new Rect(Screen.width-(x+w), y, w-20, h), "arm spring");
		y += h/2;
		float arm_spring = GUI.HorizontalSlider(new Rect(Screen.width-(x+w), y, w-20, h), arm_spring_, 0f, 2000f);
		if (arm_spring_ != arm_spring) { arm_spring_ = arm_spring; }
		y += h;
		GUI.Label(new Rect(Screen.width-(x+w), y, w-20, h), "leg damper");
		y += h/2;
		float leg_damper = GUI.HorizontalSlider(new Rect(Screen.width-(x+w), y, w-20, h), leg_damper_, 0f, 100f);
		if (leg_damper_ != leg_damper) { leg_damper_ = leg_damper; }
		y += h;
		GUI.Label(new Rect(Screen.width-(x+w), y, w-20, h), "leg spring");
		y += h/2;
		float leg_spring = GUI.HorizontalSlider(new Rect(Screen.width-(x+w), y, w-20, h), leg_spring_, 0f, 2000f);
		if (leg_spring_ != leg_spring) { leg_spring_ = leg_spring; }
		y += h;
		

		x = 20;
		y = 40;
		w = 180;
		h = 30;
        GUI.Box(new Rect(x-10, y-20, x+w, y+h*10+10), "Posture Menu");

        if (GUI.Button(new Rect(x, y, w, h), "apose") || Input.GetKeyDown(KeyCode.Alpha0)) {
			current_posture_ = posture_apose_;
        }
		y += h;
        if (GUI.Button(new Rect(x, y, w, h), "pre_throw_l_arm") || Input.GetKeyDown(KeyCode.Alpha1)) {
			current_posture_ = posture_pre_throw_l_arm_;
        }
		y += h;
        if (GUI.Button(new Rect(x, y, w, h), "throw_l_arm") || Input.GetKeyDown(KeyCode.Alpha2)) {
			current_posture_ = posture_throw_l_arm_;
        }
		y += h;
        if (GUI.Button(new Rect(x, y, w, h), "pre_throw_r_arm") || Input.GetKeyDown(KeyCode.Alpha3)) {
			current_posture_ = posture_pre_throw_r_arm_;
        }
		y += h;
        if (GUI.Button(new Rect(x, y, w, h), "throw_r_arm") || Input.GetKeyDown(KeyCode.Alpha4)) {
			current_posture_ = posture_throw_r_arm_;
        }
		y += h;
        if (GUI.Button(new Rect(x, y, w, h), "pre_jump") || Input.GetKeyDown(KeyCode.Alpha5)) {
			current_posture_ = posture_pre_jump_;
        }
		y += h;
        if (GUI.Button(new Rect(x, y, w, h), "hit") || Input.GetKeyDown(KeyCode.Alpha6)) {
			var torque0 = MyRandom.onSphere(1f)*2000f;
			muscle_motion_.getNode(MuscleMotion.Parts.Ribs).rigidbody_.addTorque(ref torque0);
			var torque1 = MyRandom.onSphere(1f)*2000f;
			muscle_motion_.getNode(MuscleMotion.Parts.Ribs2).rigidbody_.addTorque(ref torque1);
			var torque2 = MyRandom.onSphere(1f)*2000f;
			muscle_motion_.getNode(MuscleMotion.Parts.Ribs3).rigidbody_.addTorque(ref torque2);
        }
		y += h;

		y += 8;
		gui_lookat_ = GUI.HorizontalSlider(new Rect(x, y, w, h), gui_lookat_, 0f, 1f);
		y += 10;
		GUI.Label(new Rect(x, y, w, h), "value:" + gui_lookat_);
		y += h;
		gui_hand_ = GUI.Toggle(new Rect(x, y, w, h), gui_hand_, "hand");
		y += h;
		gui_rotate_ = GUI.Toggle(new Rect(x, y, w, h), gui_rotate_, "rotate");
		y += h;
	}
}

} // namespace UTJ {
/*
 * End of MuscleMotionTest.cs
 */

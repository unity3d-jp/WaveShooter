/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections.Generic;

namespace UTJ {

public class MuscleMotion
{
	public enum Parts {
		Root,
		Hip,
		L_SusoBack,
		L_SusoFront,
		L_Thigh,
		L_Knee,
		L_Ancle,
		L_Toe,
		R_SusoBack,
		R_SusoFront,
		R_Thigh,
		R_Knee,
		R_Ancle,
		R_Toe,
		Ribs,
		Ribs2,
		Ribs3,
		L_Shoulder,
		L_UpperArm,
		L_ForeArm,
		L_Wrist,
		L_FingerIndexA,
		L_FingerIndexB,
		// L_FingerIndexC,
		L_FingerMiddleA,
		L_FingerMiddleB,
		// L_FingerMiddleC,
		L_FingerPinkyA,
		L_FingerPinkyB,
		// L_FingerPinkyC,
		L_FingerRingA,
		L_FingerRingB,
		// L_FingerRingC,
		L_FingerThumbA,
		L_FingerThumbB,
		Neck,
		Head,
		L_Ribbon1,
		L_Ribbon2,
		L_Tale1,
		L_Tale2,
		L_Tale3,
		L_Tale4,
		R_Ribbon1,
		R_Ribbon2,
		R_Tale1,
		R_Tale2,
		R_Tale3,
		R_Tale4,
		R_Shoulder,
		R_UpperArm,
		R_ForeArm,
		R_Wrist,
		R_FingerIndexA,
		R_FingerIndexB,
		// R_FingerIndexC,
		R_FingerMiddleA,
		R_FingerMiddleB,
		// R_FingerMiddleC,
		R_FingerPinkyA,
		R_FingerPinkyB,
		// R_FingerPinkyC,
		R_FingerRingA,
		R_FingerRingB,
		// R_FingerRingC,
		R_FingerThumbA,
		R_FingerThumbB,

		Max,
	}
	public class PartsBit {
		public const ulong Root = 1UL<<(int)Parts.Root;
		public const ulong Hip = 1UL<<(int)Parts.Hip;
		public const ulong L_SusoBack = 1UL<<(int)Parts.L_SusoBack;
		public const ulong L_SusoFront = 1UL<<(int)Parts.L_SusoFront;
		public const ulong L_Thigh = 1UL<<(int)Parts.L_Thigh;
		public const ulong L_Knee = 1UL<<(int)Parts.L_Knee;
		public const ulong L_Ancle = 1UL<<(int)Parts.L_Ancle;
		public const ulong L_Toe = 1UL<<(int)Parts.L_Toe;
		public const ulong R_Thigh = 1UL<<(int)Parts.R_Thigh;
		public const ulong R_Knee = 1UL<<(int)Parts.R_Knee;
		public const ulong R_Ancle = 1UL<<(int)Parts.R_Ancle;
		public const ulong R_Toe = 1UL<<(int)Parts.R_Toe;
		public const ulong Ribs = 1UL<<(int)Parts.Ribs;
		public const ulong Ribs2 = 1UL<<(int)Parts.Ribs2;
		public const ulong Ribs3 = 1UL<<(int)Parts.Ribs3;
		public const ulong R_SusoBack = 1UL<<(int)Parts.R_SusoBack;
		public const ulong R_SusoFront = 1UL<<(int)Parts.R_SusoFront;
		public const ulong L_Shoulder = 1UL<<(int)Parts.L_Shoulder;
		public const ulong L_UpperArm = 1UL<<(int)Parts.L_UpperArm;
		public const ulong L_ForeArm = 1UL<<(int)Parts.L_ForeArm;
		public const ulong L_Wrist = 1UL<<(int)Parts.L_Wrist;
		public const ulong L_FingerIndexA = 1UL<<(int)Parts.L_FingerIndexA;
		public const ulong L_FingerIndexB = 1UL<<(int)Parts.L_FingerIndexB;
		// public const ulong L_FingerIndexC = 1UL<<(int)Parts.L_FingerIndexC;
		public const ulong L_FingerMiddleA = 1UL<<(int)Parts.L_FingerMiddleA;
		public const ulong L_FingerMiddleB = 1UL<<(int)Parts.L_FingerMiddleB;
		// public const ulong L_FingerMiddleC = 1UL<<(int)Parts.L_FingerMiddleC;
		public const ulong L_FingerPinkyA = 1UL<<(int)Parts.L_FingerPinkyA;
		public const ulong L_FingerPinkyB = 1UL<<(int)Parts.L_FingerPinkyB;
		// public const ulong L_FingerPinkyC = 1UL<<(int)Parts.L_FingerPinkyC;
		public const ulong L_FingerRingA = 1UL<<(int)Parts.L_FingerRingA;
		public const ulong L_FingerRingB = 1UL<<(int)Parts.L_FingerRingB;
		// public const ulong L_FingerRingC = 1UL<<(int)Parts.L_FingerRingC;
		public const ulong L_FingerThumbA = 1UL<<(int)Parts.L_FingerThumbA;
		public const ulong L_FingerThumbB = 1UL<<(int)Parts.L_FingerThumbB;
		public const ulong Neck = 1UL<<(int)Parts.Neck;
		public const ulong Head = 1UL<<(int)Parts.Head;
		public const ulong L_Ribbon1 = 1UL<<(int)Parts.L_Ribbon1;
		public const ulong L_Ribbon2 = 1UL<<(int)Parts.L_Ribbon2;
		public const ulong L_Tale1 = 1UL<<(int)Parts.L_Tale1;
		public const ulong L_Tale2 = 1UL<<(int)Parts.L_Tale2;
		public const ulong L_Tale3 = 1UL<<(int)Parts.L_Tale3;
		public const ulong L_Tale4 = 1UL<<(int)Parts.L_Tale4;
		public const ulong R_Ribbon1 = 1UL<<(int)Parts.R_Ribbon1;
		public const ulong R_Ribbon2 = 1UL<<(int)Parts.R_Ribbon2;
		public const ulong R_Tale1 = 1UL<<(int)Parts.R_Tale1;
		public const ulong R_Tale2 = 1UL<<(int)Parts.R_Tale2;
		public const ulong R_Tale3 = 1UL<<(int)Parts.R_Tale3;
		public const ulong R_Tale4 = 1UL<<(int)Parts.R_Tale4;
		public const ulong R_Shoulder = 1UL<<(int)Parts.R_Shoulder;
		public const ulong R_UpperArm = 1UL<<(int)Parts.R_UpperArm;
		public const ulong R_ForeArm = 1UL<<(int)Parts.R_ForeArm;
		public const ulong R_Wrist = 1UL<<(int)Parts.R_Wrist;
		public const ulong R_FingerIndexA = 1UL<<(int)Parts.R_FingerIndexA;
		public const ulong R_FingerIndexB = 1UL<<(int)Parts.R_FingerIndexB;
		// public const ulong R_FingerIndexC = 1UL<<(int)Parts.R_FingerIndexC;
		public const ulong R_FingerMiddleA = 1UL<<(int)Parts.R_FingerMiddleA;
		public const ulong R_FingerMiddleB = 1UL<<(int)Parts.R_FingerMiddleB;
		// public const ulong R_FingerMiddleC = 1UL<<(int)Parts.R_FingerMiddleC;
		public const ulong R_FingerPinkyA = 1UL<<(int)Parts.R_FingerPinkyA;
		public const ulong R_FingerPinkyB = 1UL<<(int)Parts.R_FingerPinkyB;
		// public const ulong R_FingerPinkyC = 1UL<<(int)Parts.R_FingerPinkyC;
		public const ulong R_FingerRingA = 1UL<<(int)Parts.R_FingerRingA;
		public const ulong R_FingerRingB = 1UL<<(int)Parts.R_FingerRingB;
		// public const ulong R_FingerRingC = 1UL<<(int)Parts.R_FingerRingC;
		public const ulong R_FingerThumbA = 1UL<<(int)Parts.R_FingerThumbA;
		public const ulong R_FingerThumbB = 1UL<<(int)Parts.R_FingerThumbB;
		public const ulong Whole = 0xffffffffffffffffL;
		public const ulong L_Leg = L_Thigh | L_Knee | L_Ancle | L_Toe;
		public const ulong R_Leg = R_Thigh | R_Knee | R_Ancle | R_Toe;
		public const ulong LowerBody = L_Leg | R_Leg | Hip;
		public const ulong L_Arm = L_Wrist | L_ForeArm | L_UpperArm | L_Shoulder;
		public const ulong R_Arm = R_Wrist | R_ForeArm | R_UpperArm | R_Shoulder;
		public const ulong UpperBody = L_Arm | R_Arm | Ribs | Ribs2 | Ribs3;
		public const ulong Body = LowerBody | UpperBody;
		// public const ulong L_FingerIndex = L_FingerIndexA | L_FingerIndexB | L_FingerIndexC;
		// public const ulong L_FingerMiddle = L_FingerMiddleA | L_FingerMiddleB;
		// public const ulong L_FingerMiddle = L_FingerMiddleA | L_FingerMiddleB | L_FingerMiddleC;
		// public const ulong L_FingerPinky = L_FingerPinkyA | L_FingerPinkyB | L_FingerPinkyC;
		// public const ulong L_FingerRing = L_FingerRingA | L_FingerRingB | L_FingerRingC;
		public const ulong L_FingerIndex = L_FingerIndexA | L_FingerIndexB;
		public const ulong L_FingerMiddle = L_FingerMiddleA | L_FingerMiddleB;
		public const ulong L_FingerPinky = L_FingerPinkyA | L_FingerPinkyB;
		public const ulong L_FingerRing = L_FingerRingA | L_FingerRingB;
		public const ulong L_FingerThumb = L_FingerThumbA | L_FingerThumbB;
		public const ulong L_Hand = L_FingerIndex | L_FingerMiddle | L_FingerPinky | L_FingerRing | L_FingerThumb;
		// public const ulong R_FingerIndex = R_FingerIndexA | R_FingerIndexB | R_FingerIndexC;
		// public const ulong R_FingerMiddle = R_FingerMiddleA | R_FingerMiddleB | R_FingerMiddleC;
		// public const ulong R_FingerPinky = R_FingerPinkyA | R_FingerPinkyB | R_FingerPinkyC;
		// public const ulong R_FingerRing = R_FingerRingA | R_FingerRingB | R_FingerRingC;
		public const ulong R_FingerIndex = R_FingerIndexA | R_FingerIndexB;
		public const ulong R_FingerMiddle = R_FingerMiddleA | R_FingerMiddleB;
		public const ulong R_FingerPinky = R_FingerPinkyA | R_FingerPinkyB;
		public const ulong R_FingerRing = R_FingerRingA | R_FingerRingB;
		public const ulong R_FingerThumb = R_FingerThumbA | R_FingerThumbB;
		public const ulong R_Hand = R_FingerIndex | R_FingerMiddle | R_FingerPinky | R_FingerRing | R_FingerThumb;
	}
	public static readonly Parts[] ReverseTable = new Parts[(int)Parts.Max] {
		Parts.Root,
		Parts.Hip,
		Parts.R_SusoBack,
		Parts.R_SusoFront,
		Parts.R_Thigh,
		Parts.R_Knee,
		Parts.R_Ancle,
		Parts.R_Toe,
		Parts.L_SusoBack,
		Parts.L_SusoFront,
		Parts.L_Thigh,
		Parts.L_Knee,
		Parts.L_Ancle,
		Parts.L_Toe,
		Parts.Ribs,
		Parts.Ribs2,
		Parts.Ribs3,
		Parts.R_Shoulder,
		Parts.R_UpperArm,
		Parts.R_ForeArm,
		Parts.R_Wrist,
		Parts.R_FingerIndexA,
		Parts.R_FingerIndexB,
		// Parts.R_FingerIndexC,
		Parts.R_FingerMiddleA,
		Parts.R_FingerMiddleB,
		// Parts.R_FingerMiddleC,
		Parts.R_FingerPinkyA,
		Parts.R_FingerPinkyB,
		// Parts.R_FingerPinkyC,
		Parts.R_FingerRingA,
		Parts.R_FingerRingB,
		// Parts.R_FingerRingC,
		Parts.R_FingerThumbA,
		Parts.R_FingerThumbB,
		Parts.Neck,
		Parts.Head,
		Parts.R_Ribbon1,
		Parts.R_Ribbon2,
		Parts.R_Tale1,
		Parts.R_Tale2,
		Parts.R_Tale3,
		Parts.R_Tale4,
		Parts.L_Ribbon1,
		Parts.L_Ribbon2,
		Parts.L_Tale1,
		Parts.L_Tale2,
		Parts.L_Tale3,
		Parts.L_Tale4,
		Parts.L_Shoulder,
		Parts.L_UpperArm,
		Parts.L_ForeArm,
		Parts.L_Wrist,
		Parts.L_FingerIndexA,
		Parts.L_FingerIndexB,
		// Parts.L_FingerIndexC,
		Parts.L_FingerMiddleA,
		Parts.L_FingerMiddleB,
		// Parts.L_FingerMiddleC,
		Parts.L_FingerPinkyA,
		Parts.L_FingerPinkyB,
		// Parts.L_FingerPinkyC,
		Parts.L_FingerRingA,
		Parts.L_FingerRingB,
		// Parts.L_FingerRingC,
		Parts.L_FingerThumbA,
		Parts.L_FingerThumbB,
	};

	public class Node
	{
		public bool valid_;
		public Node parent_;
		public Vector3 local_position_;
		public Quaternion original_rotation_; // original 'unfreezed' rotation of A-pose
		public Quaternion original_rotation_inverse_;
		public Quaternion base_rotation_;	  // original 'unfreezed' rotation of one pose
		public Quaternion base_rotation_inverse_;
		public Quaternion target_rotation_;	  // target relative to parent's rigidbody
		public RigidbodyTransform rigidbody_;
		public float spring_ratio_;
		public bool fixed_;
		private bool initialized_ = false;
		
		#if UNITY_EDITOR
		public void set(Transform tfm)
		{
			parent_ = null;
			var pos = tfm.position;
			var rot = tfm.rotation;
			rigidbody_.init(ref pos, ref rot);
		}
		#endif // UNITY_EDITOR

		public void init(NodeData node_data,
						 Node parent,
						 float damper,
						 float spring_ratio)
		{
			if (initialized_) {
				Debug.Assert(false);
				return;
			}

			valid_ = (Parts)node_data.node_idx_ != Parts.Max;
			if (!valid_) {
				return;
			}
			if (parent == null) {
				parent_ = null;
				local_position_ = Vector3.zero;
				rigidbody_.transform_.position_ = Vector3.zero;
				rigidbody_.transform_.rotation_ = Quaternion.identity;
				rigidbody_.setRotateDamper(damper);
				spring_ratio_ = 0f;
				original_rotation_ = Quaternion.identity;
				base_rotation_ = Quaternion.identity;
				target_rotation_ = Quaternion.identity;
			} else {
				local_position_ = parent.original_rotation_ * node_data.local_position_;
				rigidbody_.transform_.position_ = node_data.position_;
				rigidbody_.transform_.rotation_ = Quaternion.identity;
				parent_ = parent;
				rigidbody_.setRotateDamper(damper);
				spring_ratio_ = spring_ratio;
				original_rotation_ = node_data.rotation_;
				base_rotation_ = node_data.rotation_;
				target_rotation_ = Quaternion.identity;
			}
			original_rotation_inverse_ = Quaternion.Inverse(original_rotation_);
			base_rotation_inverse_ = Quaternion.Inverse(base_rotation_);
			fixed_ = false;
			initialized_ = true;
		}

		public void setBaseRotation(NodeData node_data)
		{
			base_rotation_ = node_data.rotation_;
			base_rotation_inverse_ = Quaternion.Inverse(base_rotation_);
		}

		public void setTarget(NodeData node_data)
		{
			if (!valid_) {
				return;
			}
			if (parent_ != null) {
				target_rotation_ = (parent_.original_rotation_ *
									parent_.base_rotation_inverse_ *
									base_rotation_ *
									original_rotation_inverse_);
			}
		}

		public void setTarget(ref Quaternion rot)
		{
			target_rotation_ = (parent_.original_rotation_ *
								parent_.base_rotation_inverse_ *
								rot *
								original_rotation_inverse_);
		} 

		public void setTargetDirect(ref Quaternion rot)
		{
			target_rotation_ = rot;
		} 

		public void setParams(float damper, float spring_ratio)
		{
			rigidbody_.setRotateDamper(damper);
			spring_ratio_ = spring_ratio;
		}

		public void addTorque(ref Vector3 torque)
		{
			rigidbody_.addTorque(ref torque);
		}
		
		public void update(float dt)
		{
			if (!valid_) {
				return;
			}
			const int LOOP = 8;
			float dt0 = dt/(float)LOOP;
			for (var i = 0; i < LOOP; ++i) {
				inner_update(dt0);
			}
		}

		private void inner_update(float dt)
		{
			if (parent_ != null) {
				Quaternion target = parent_.rigidbody_.transform_.rotation_ * target_rotation_;
				if (!fixed_) {
					rigidbody_.addSpringTorque(ref target, spring_ratio_);
				} else {
					rigidbody_.transform_.rotation_ = Quaternion.Lerp(rigidbody_.transform_.rotation_,
																	  target,
																	  spring_ratio_);
				}
			}
			rigidbody_.update(dt);
			if (parent_ != null) {
				rigidbody_.transform_.position_ = (parent_.rigidbody_.transform_.position_ +
												   (parent_.rigidbody_.transform_.rotation_ *
													local_position_));
			}
		}

		public void renderUpdate(ref DrawBuffer draw_buffer, DrawBuffer.Type type, Parts parts)
		{
			if (!valid_) {
				return;
			}
			var rotation = rigidbody_.transform_.rotation_ * original_rotation_;
			draw_buffer.regist(ref rigidbody_.transform_.position_,
							   ref rotation,
							   DrawBuffer.Type.MuscleMotionPlayer, (int)parts);
		}

	}

	private Node[] node_list_;

	public void init(Posture posture, float damper, float spring_ratio)
	{
		node_list_ = new Node[(int)Parts.Max];
		for (var i = 0; i < (int)Parts.Max; ++i) {
			node_list_[i] = new Node();
		}

		foreach (var node_data in posture.getNodeDataList()) {
			// node_data.dump();
			var node_idx = node_data.node_idx_;
			var node = node_list_[node_idx];
			var parent_node_idx = node_data.parent_node_idx_;
			var parent_node = parent_node_idx < (int)Parts.Max ? node_list_[parent_node_idx] : null;
			node.init(node_data, parent_node, damper, spring_ratio);
		}
	}

	public Node getRootNode() { return node_list_[(int)Parts.Root]; }
	public Node getNode(Parts parts) { return node_list_[(int)parts]; }

	public void setTarget(Posture posture, ulong mask = (ulong)PartsBit.Whole)
	{
		// pass 1
		foreach (var node_data in posture.getNodeDataList()) {
			var node_idx = node_data.node_idx_;
			if (node_idx != (int)Parts.Root &&
				(1UL<<node_idx & mask) != 0) {
				var node = node_list_[node_idx];
				node.setBaseRotation(node_data);
			}
		}
		// pass 2
		foreach (var node_data in posture.getNodeDataList()) {
			var node_idx = node_data.node_idx_;
			if (node_idx != (int)Parts.Root &&
				(1UL<<node_idx & mask) != 0) {
				var node = node_list_[node_idx];
				node.setTarget(node_data);
			}
		}
	}

	public void fix(Parts parts, float interoplate_ratio)
	{
		node_list_[(int)parts].fixed_ = true;
		node_list_[(int)parts].spring_ratio_ = interoplate_ratio;
	}

	public void setParams(Parts parts, float damper, float spring_ratio)
	{
		node_list_[(int)parts].setParams(damper, spring_ratio);
	}

	public void addTorque(Parts parts, ref Vector3 torque)
	{
		var root_node = getRootNode();
		var t = root_node.rigidbody_.transform_.rotation_ * torque;
		node_list_[(int)parts].addTorque(ref t);
	}
	public void addTorqueX(Parts parts, float torque)
	{
		var root_node = getRootNode();
		var t = root_node.rigidbody_.transform_.rotation_ * new Vector3(torque, 0f, 0f);
		node_list_[(int)parts].addTorque(ref t);
	}
	public void addTorqueY(Parts parts, float torque)
	{
		var root_node = getRootNode();
		var t = root_node.rigidbody_.transform_.rotation_ * new Vector3(0f, torque, 0f);
		node_list_[(int)parts].addTorque(ref t);
	}
	public void addTorqueZ(Parts parts, float torque)
	{
		var root_node = getRootNode();
		var t = root_node.rigidbody_.transform_.rotation_ * new Vector3(0f, 0, torque);
		node_list_[(int)parts].addTorque(ref t);
	}
	public void addTorqueXY(Parts parts, float torque_x, float torque_y)
	{
		var root_node = getRootNode();
		var t = root_node.rigidbody_.transform_.rotation_ * new Vector3(torque_x, torque_y, 0f);
		node_list_[(int)parts].addTorque(ref t);
	}
	public void addTorqueYZ(Parts parts, float torque_y, float torque_z)
	{
		var root_node = getRootNode();
		var t = root_node.rigidbody_.transform_.rotation_ * new Vector3(0f, torque_y, torque_z);
		node_list_[(int)parts].addTorque(ref t);
	}

	public void update(float dt)
	{
		for (var i = 0; i < (int)Parts.Max; ++i) {
			node_list_[i].update(dt);
		}
	}

	#if UNITY_EDITOR
	public void set(Transform tfm, Parts parts)
	{
		node_list_[(int)parts].set(tfm);
	}
	#endif // UNITY_EDITOR

	public void renderUpdate(ref DrawBuffer draw_buffer, DrawBuffer.Type type)
	{
		for (var i = 0; i < (int)Parts.Max; ++i) {
			node_list_[i].renderUpdate(ref draw_buffer, type, (Parts)i);
		}
	}

}

} // namespace UTJ {
/*
 * End of MuscleMotion.cs
 */

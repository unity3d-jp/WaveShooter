/* -*- mode:C++; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections.Generic;

namespace UTJ {

#if UNITY_EDITOR

public class MuscleMotionEditor : UnityEngine.MonoBehaviour
{
	private Mesh mesh_;

	private Mesh create_mesh()
	{
		var mesh = new Mesh();
		var vertices = new Vector3[4] {
			new Vector3(   0f,     0f, -0.25f),
			new Vector3(   0f,  -0.1f, 0.75f),
			new Vector3( 0.1f,   0.1f, 0.75f),
			new Vector3(-0.1f,   0.1f, 0.75f),
		};
		mesh.vertices = vertices;
		mesh.normals = new Vector3[4] {
			vertices[0].normalized,
			vertices[1].normalized,
			vertices[2].normalized,
			vertices[3].normalized,
		};
		mesh.triangles = new int[4*3] {
			0, 1, 2,
			0, 2, 3,
			0, 3, 1,
			1, 2, 3,
		};
		return mesh;
	}

    void OnDrawGizmos()
	{
		if (mesh_ == null) {
			mesh_ = create_mesh();
		}
		var scale = new Vector3(0.2f, 0.2f, 0.2f);
		var list = collect_transforms();
		Gizmos.color = new Color(0.5f, 1f, 1f);
		foreach (var node in list) {
			if (node.parent_transform_ == null) {
				continue;
			}
			var rot = Quaternion.LookRotation(node.parent_transform_.position - node.transform_.position);
			Gizmos.DrawWireMesh(mesh_,
								node.transform_.position, rot, scale);
		}
    }

	public class TransformNode {
		public Transform transform_;
		public Transform parent_transform_;
		public MuscleMotion.Parts parts_;
		public MuscleMotion.Parts parent_parts_;
		public TransformNode(Transform transform,
							 Transform parent_transform,
							 MuscleMotion.Parts parts,
							 MuscleMotion.Parts parent_parts)
		{
			transform_ = transform;
			parent_transform_ = parent_transform;
			parts_ = parts;
			parent_parts_ = parent_parts;
		}							 
	}

	private const string BASE_POSTURE_NAME = "base";

	public List<TransformNode> collectTransforms()
	{
		return collect_transforms();
	}

	public List<TransformNode> collectTransformsReverse()
	{
		return collect_transforms_reverse();
	}

	private void collect_transforms_internal(List<TransformNode> transform_node_list,
											 Transform parent_tfm, Transform tfm)
	{
		MuscleMotion.Parts parts = MuscleMotionEditorUtil.conv(tfm);
		if (parts == MuscleMotion.Parts.Max) {
			return;
		}
		MuscleMotion.Parts parent_parts = MuscleMotionEditorUtil.conv(parent_tfm);
		transform_node_list.Add(new TransformNode(tfm, parent_tfm, parts, parent_parts));

		int num = tfm.childCount;
		for (var i = 0; i < num; ++i) {
			var child_tfm = tfm.GetChild(i);
			collect_transforms_internal(transform_node_list, tfm, child_tfm);
		}
	}

	private List<TransformNode> collect_transforms()
	{
		var transform_node_list = new List<TransformNode>();
		// var tfm = (target as MuscleMotionEditor).transform;
		var tfm = transform;
		var root = tfm.Find("Root");
		if (root == null) {
			Debug.LogError("no Root child found.");
			return null;
		}
		if (root.name != MuscleMotion.Parts.Root.ToString()) {
			Debug.LogError("wrong name, aborted.");
			return null;
		}
		collect_transforms_internal(transform_node_list, null, root);
		return transform_node_list;
	}

	private void collect_transforms_reverse_internal(List<TransformNode> transform_node_list,
													 Transform parent_tfm, Transform tfm)
	{
		MuscleMotion.Parts parts = MuscleMotionEditorUtil.conv_reverse(tfm);
		if (parts == MuscleMotion.Parts.Max) {
			return;
		}
		MuscleMotion.Parts parent_parts = MuscleMotionEditorUtil.conv_reverse(parent_tfm);
		transform_node_list.Add(new TransformNode(tfm, parent_tfm, parts, parent_parts));

		int num = tfm.childCount;
		for (var i = 0; i < num; ++i) {
			var child_tfm = tfm.GetChild(i);
			collect_transforms_reverse_internal(transform_node_list, tfm, child_tfm);
		}
	}

	private List<TransformNode> collect_transforms_reverse()
	{
		var transform_node_list = new List<TransformNode>();
		// var tfm = (target as MuscleMotionEditor).transform;
		var tfm = transform;
		var root = tfm.Find("Root");
		if (root == null) {
			Debug.LogError("no Root child found.");
			return null;
		}
		if (root.name != MuscleMotion.Parts.Root.ToString()) {
			Debug.LogError("wrong name, aborted.");
			return null;
		}
		collect_transforms_reverse_internal(transform_node_list, null, root);
		return transform_node_list;
	}
}

#endif // #if UNITY_EDITOR

} // namespace UTJ {

/*
 * End of MuscleMotionEditor.cs
 */

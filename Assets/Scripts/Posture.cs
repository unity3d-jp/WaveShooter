/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UTJ {

[System.Serializable]
public struct NodeData
{
	public Vector3 position_;
	public Vector3 local_position_;
	public Quaternion rotation_;
	public Quaternion local_rotation_;
	public int node_idx_;	// parts
	public int parent_node_idx_; // parent's parts
	public string name_;

	public NodeData deepCopy()
	{
		var new_node_data = new NodeData();
		new_node_data.position_ = position_;
		new_node_data.local_position_ = local_position_;
		new_node_data.rotation_ = rotation_;
		new_node_data.local_rotation_ = local_rotation_;
		new_node_data.node_idx_ = node_idx_;
		new_node_data.parent_node_idx_ = parent_node_idx_;
		new_node_data.name_ = "(copied)" + name_;
		return new_node_data;
	}

	public void set(Transform tfm, MuscleMotion.Parts parts, MuscleMotion.Parts parent_parts)
	{
		if (parent_parts == MuscleMotion.Parts.Root) {
			position_ = tfm.position;
			rotation_ = tfm.rotation;
			local_position_ = tfm.position;
			local_rotation_ = tfm.rotation;
		} else {
			position_ = tfm.position;
			rotation_ = tfm.rotation;
			local_position_ = tfm.localPosition;
			local_rotation_ = tfm.localRotation;
		}
		node_idx_ = (int)parts;
		parent_node_idx_ = (int)parent_parts;
		name_ = parts.ToString();
	}

	public void dump()
	{
		Debug.LogFormat("node_idx_:{0}", node_idx_);
		Debug.LogFormat("rotation_:{0}", rotation_);
		Debug.LogFormat("local_rotation_:{0}", local_rotation_);
	}
}

[System.Serializable]
public class Posture
{
	public NodeData[] node_data_list_;
	
	public Posture()
	{
		node_data_list_ = null;
	}

	public NodeData[] getNodeDataList() { return node_data_list_; }

	public List<NodeData> beginSet() { return new List<NodeData>(); }

	public void set(List<NodeData> list,
					Transform tfm,
					MuscleMotion.Parts parts,
					MuscleMotion.Parts parent_parts,
					ulong mask)
	{
		if ((mask & (1UL<<(int)parts)) != 0) {
			var node_data = new NodeData();
			node_data.set(tfm, parts, parent_parts);
			list.Add(node_data);
		}
	}

	public void endSet(List<NodeData> list)
	{
		node_data_list_ = new NodeData[list.Count];
		for (var i = 0; i < list.Count; ++i) {
			node_data_list_[i] = list[i];
		}
	}

	public bool find(MuscleMotion.Parts parts, out NodeData node_data)
	{
		for (var i = 0; i < node_data_list_.Length; ++i) {
			if (node_data_list_[i].node_idx_ == (int)parts) {
				node_data = node_data_list_[i];
				return true;
			}
		}
		node_data = new NodeData();
		return false;
	}

	public Posture deepCopy()
	{
		var posture = new Posture();
		posture.node_data_list_ = new NodeData[node_data_list_.Length];
		for (var i = 0; i < node_data_list_.Length; ++i) {
			posture.node_data_list_[i] = node_data_list_[i].deepCopy();
		}
		return posture;
	}

	public NodeData getNodeData(MuscleMotion.Parts parts)
	{
		foreach (var node_data in node_data_list_) {
			if (node_data.node_idx_ == (int)parts) {
				return node_data;
			}
		}
		Debug.Assert(false);
		return new NodeData();
	}

	public void dump()
	{
		foreach (var node_data in node_data_list_) {
			Debug.LogFormat("node_data[{0}]", (MuscleMotion.Parts)node_data.node_idx_);
			node_data.dump();
		}
	}
}

} // namespace UTJ {

/*
 * End of Posture.cs
 */

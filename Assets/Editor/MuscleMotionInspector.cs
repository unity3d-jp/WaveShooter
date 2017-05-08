/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 *
 * MuscleMotionInspector.cs - Project AnotherThread2
 * Wed Jan 25 15:05:34 2017
 *
 * Copyright (c) 2017 Yuji YASUHARA
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace UTJ {

[CustomEditor(typeof(MuscleMotionEditor))]  
class MuscleMotionInspector : UnityEditor.Editor
{
	private const string BASE_POSTURE_NAME = "base";

	private List<MuscleMotionEditor.TransformNode> collect_transforms()
	{
		return (target as MuscleMotionEditor).collectTransforms();
	}

	private List<MuscleMotionEditor.TransformNode> collect_transforms_reverse()
	{
		return (target as MuscleMotionEditor).collectTransformsReverse();
	}

	public override void OnInspectorGUI()
	{
		if (GUILayout.Button("Save Base")) {
			write_data(BASE_POSTURE_NAME, collect_transforms(), MuscleMotion.PartsBit.Whole);
		}
		if (GUILayout.Button("Load Base")) {
			read_data(BASE_POSTURE_NAME, collect_transforms());
		}
		if (GUILayout.Button("Save Data A")) {
			write_data("data_A", collect_transforms(), MuscleMotion.PartsBit.Body);
		}
		if (GUILayout.Button("Load Data A")) {
			read_data("data_A", collect_transforms());
		}
		if (GUILayout.Button("Save Data B")) {
			write_data("data_B", collect_transforms(), MuscleMotion.PartsBit.Body);
		}
		if (GUILayout.Button("Load Data B")) {
			read_data("data_B", collect_transforms());
		}
		if (GUILayout.Button("Load Data B Reverse")) {
			read_data_reverse("data_B", collect_transforms_reverse());
		}
	}

	private Posture read_posture(string filename)
	{
		var path = Application.streamingAssetsPath+"/"+filename+".dat";
 		var json = System.IO.File.ReadAllText(path);
		var posture = JsonUtility.FromJson<Posture>(json);
		return posture;
	}

	private void read_data(string filename, List<MuscleMotionEditor.TransformNode> transform_list)
	{
		var posture = read_posture(filename);
		var root = (target as MuscleMotionEditor).transform;
		for (var i = 0; i < posture.getNodeDataList().Length; ++i) {
			var node_data = posture.getNodeDataList()[i];
			var parts = (MuscleMotion.Parts)node_data.node_idx_;
			foreach (var tnode in transform_list) {
				if (tnode.parts_ == parts) {
					tnode.transform_.position = node_data.position_;
					tnode.transform_.rotation = node_data.rotation_;
					break;
				}
			}
		}
		Debug.LogFormat("read {0} done.", filename);
	}

	private void read_data_reverse(string filename, List<MuscleMotionEditor.TransformNode> rev_transform_list)
	{
		var base_posture = read_posture(BASE_POSTURE_NAME);
		var posture = read_posture(filename);
		var root = (target as MuscleMotionEditor).transform;
		for (var i = 0; i < posture.getNodeDataList().Length; ++i) {
			var node_data = posture.getNodeDataList()[i];
			var parts = (MuscleMotion.Parts)node_data.node_idx_;
			NodeData base_node_data;
			var success = base_posture.find(parts, out base_node_data);
			Debug.Assert(success);
			var rev_parts = (MuscleMotion.Parts)MuscleMotion.ReverseTable[node_data.node_idx_];
			NodeData base_rev_node_data;
			success = base_posture.find(rev_parts, out base_rev_node_data);
			Debug.Assert(success);
			foreach (var tnode in rev_transform_list) {
				if (tnode.parts_ == parts) {
					var rot = node_data.rotation_;
					var base_rot = base_node_data.rotation_;
					var relative_rot = rot * Quaternion.Inverse(base_rot);
					Utility.MirrorX(ref relative_rot);
					var rot0 = relative_rot * base_rev_node_data.rotation_;
					tnode.transform_.rotation = rot0;
					break;
				}
			}
		}
		Debug.LogFormat("read reverse {0} done.", filename);
	}

	private void write_data(string filename, List<MuscleMotionEditor.TransformNode> transform_node_list, ulong mask)
	{
		var root = (target as MuscleMotionEditor).transform;
		root.position = Vector3.zero;
		var posture = new Posture();
		var serialize = posture.beginSet();
		foreach (var transform_node in transform_node_list) {
			posture.set(serialize,
						transform_node.transform_,
						transform_node.parts_,
						transform_node.parent_parts_,
						mask);
		}
		posture.endSet(serialize);
		string json = JsonUtility.ToJson(posture);
		var path = Application.streamingAssetsPath+"/"+filename+".dat";
		System.IO.File.WriteAllText(path, json);
		Debug.LogFormat("write {0} done.", path);
	}
}

} // namespace UTJ {

/*
 * End of MuscleMotionInspector.cs
 */

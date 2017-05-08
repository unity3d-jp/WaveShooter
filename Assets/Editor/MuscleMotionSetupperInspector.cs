/* -*- mode:C++; coding:utf-8-with-signature -*-
 *
 * MuscleMotionSetupperInspector.cs - Project AnotherThread2
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
 *
 * since Sat Jan 28 12:14:59 2017
 */

using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace UTJ {

[CustomEditor(typeof(MuscleMotionSetupper))]  
class MuscleMotionSetupperInspector : UnityEditor.Editor
{
	private void flaten_internal(List<Transform> nodes, Transform node)
	{
		if (MuscleMotionEditorUtil.conv(node) != MuscleMotion.Parts.Max) {
			nodes.Add(node);
			var children_num = node.childCount;
			for (var i = 0; i < children_num; ++i) {
				flaten_internal(nodes, node.GetChild(i));
			}
		}
	}

    public override void OnInspectorGUI()
    {
        var root = (target as MuscleMotionSetupper).transform.Find("Root");
		if (root == null) {
			Debug.LogError("no Root object found.");
			return;
		}
		var parent = root.parent;
		var nodes = new List<Transform>();
		if (GUILayout.Button("Flaten")) {
			flaten_internal(nodes, root);
		}
		foreach (var node in nodes) {
			node.SetParent(parent);
		}
	}
}

} // namespace UTJ {

/*
 * End of MuscleMotionFlatenerInspector.cs
 */

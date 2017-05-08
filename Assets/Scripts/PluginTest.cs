/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ {

public class PluginTest : MonoBehaviour {

	private List<Vector3> list_;
    // private System.Runtime.InteropServices.GCHandle gc_handle_;

	void Start()
	{
		UnityPluginIF.Load("UnityPlugin");
		// list_ = new List<Vector3>();
		// list_.Capacity = 100;
		// list_.Add(new Vector3(111f, 222f, 333f));
		// gc_handle_ = System.Runtime.InteropServices.GCHandle.Alloc(list_,
		// 														   System.Runtime.InteropServices.GCHandleType.Pinned);
		
	}

	void Update()
	{
		// Debug.Log(UnityPluginIF.getValue(gc_handle_.AddrOfPinnedObject(), 0));
		// unsafe {
		// 	fixed(void* ptr = list_) {
		// 		UnityPluginIF.getValue(new System.IntPtr(ptr), 0);
		// 	}
		// }
	}

	void OnDestroy()
	{
		UnityPluginIF.Unload();
	}
}

} // namespace UTJ {

/*
 * End of PluginTest.cs
 */

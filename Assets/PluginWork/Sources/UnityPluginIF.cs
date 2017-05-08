/* -*- mode:C++; coding:utf-8-with-signature -*-
 *
 * UnityPluginIF.cs - Project AnotherThread2
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
 * since Wed Jan 25 10:00:09 2017
 */

namespace UTJ {

using UnityEngine;
using System.Runtime.InteropServices;

public class UnityPluginIF {
	#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
	public static void Load(string name)
	{
		#if UNITY_EDITOR_WIN
		EditorWindowsIF.Load(name);
		#elif UNITY_EDITOR_OSX
		EditorMacIF.Load(name);
		#else
		Debug.Assert(false);
		#endif
	}
	#else
	public static void Load(string name) {}
	#endif

	#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
	public static void Unload()
	{
		#if UNITY_EDITOR_WIN
		EditorWindowsIF.Unload();
		#elif UNITY_EDITOR_OSX
		EditorMacIF.Unload();
		#else
		Debug.Assert(false);
		#endif
	}
	#else
	public static void Unload() { return; }
	#endif

	#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
	delegate float foo_signature(float value);
	public static float foo(float value) {
		#if UNITY_EDITOR_WIN
		var func = EditorWindowsIF.GetDelegate<foo_signature>("foo"); // consumes GC memory
		return (float)func.DynamicInvoke(value);
		#elif UNITY_EDITOR_OSX
		var func = EditorMacIF.GetDelegate<foo_signature>("foo"); // consumes GC memory
		return func(value);
		#else
		Debug.Assert(false);
		return 0f;
		#endif
	}
	#else
	# if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
	# else
	[DllImport("UnityPlugin")]
	# endif
	public static extern float foo(float value);
	#endif


	#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
	delegate int ConvertArrayFromList_signature(int unit_max,
												System.IntPtr alive_table_items_ptr,
												System.IntPtr positions_items_ptr,
												System.IntPtr velocities_items_ptr,
												System.IntPtr uv2_items_ptr,
												System.IntPtr vertices_items_ptr,
												System.IntPtr normals_items_ptr,
												System.IntPtr uv2s_items_ptr);
	 public static int ConvertArrayFromList(int unit_max,
											System.IntPtr alive_table_items_ptr,
											System.IntPtr positions_items_ptr,
											System.IntPtr velocities_items_ptr,
											System.IntPtr uv2_items_ptr,
											System.IntPtr vertices_items_ptr,
											System.IntPtr normals_items_ptr,
											System.IntPtr uv2s_items_ptr)
	{
		#if UNITY_EDITOR_WIN
		var func = EditorWindowsIF.GetDelegate<ConvertArrayFromList_signature>("ConvertArrayFromList");
		object[] args = new object[] {
			unit_max,
			alive_table_items_ptr,
			positions_items_ptr,
			velocities_items_ptr,
			uv2_items_ptr,
			vertices_items_ptr,
			normals_items_ptr,
			uv2s_items_ptr,
		};
		return (int)func.DynamicInvoke(args);
		#elif UNITY_EDITOR_OSX
		var func = EditorMacIF.GetDelegate<ConvertArrayFromList_signature>("ConvertArrayFromList");
		object[] args = new object[] {
			unit_max,
			alive_table_items_ptr,
			positions_items_ptr,
			velocities_items_ptr,
			uv2_items_ptr,
			vertices_items_ptr,
			normals_items_ptr,
			uv2s_items_ptr,
		};
		return (int)func.DynamicInvoke(args);
		#else
		Debug.Assert(false);
		#endif
	}
	#else
	# if UNITY_IPHONE || UNITY_XBOX360 || UNITY_SWITCH
	[DllImport("__Internal")]
	# else
	[DllImport("UnityPlugin")]
	# endif
	public static extern int ConvertArrayFromList(int unit_max,
												  System.IntPtr alive_table_items_ptr,
												  System.IntPtr positions_items_ptr,
												  System.IntPtr velocities_items_ptr,
												  System.IntPtr uv2_items_ptr,
												  System.IntPtr vertices_items_ptr,
												  System.IntPtr normals_items_ptr,
												  System.IntPtr uv2s_items_ptr);
	#endif
}

} // namespace UTJ {

/*
 * End of UnityPluginIF.cs
 */

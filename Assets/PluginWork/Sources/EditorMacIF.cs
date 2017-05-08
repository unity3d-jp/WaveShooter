/* -*- mode:C++; coding:utf-8-with-signature -*-
 *
 * EditorMacIF.cs - Project AnotherThread2
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
 * since Wed Jan 25 10:18:41 2017
 */

namespace UTJ {

using System;
using System.Runtime.InteropServices;
using UnityEngine;
 
public static class EditorMacIF
{
#if UNITY_EDITOR_OSX
	private const int RTLD_LAZY     = 0x0001;
	private static IntPtr native_library_ptr_ = IntPtr.Zero;

	[DllImport("__Internal")]
	public static extern IntPtr dlopen(string filename, int flag);
	[DllImport("__Internal")]
	public static extern IntPtr dlsym(IntPtr handle, string symbol);
	[DllImport("__Internal")]
	public static extern int dlclose(IntPtr handle);

	public static void Load(string dllname)
	{
        Debug.Assert(native_library_ptr_ == IntPtr.Zero);
		var path = Application.dataPath+"/PluginWork/Plugins/OSX/UnityPlugin.bundle/Contents/MacOS/UnityPlugin";
		native_library_ptr_ = dlopen(path, RTLD_LAZY);
        if (native_library_ptr_ == IntPtr.Zero) {
            Debug.LogError("Failed to load native library:"+path);
			return;
        }
	}
	
	public static void Unload()
	{
        if (native_library_ptr_ == IntPtr.Zero) {
			return;
		}
		int ret = dlclose(native_library_ptr_);
        Debug.Log(ret == 0 ?
				  "Native library successfully unloaded." :
				  "Native library could not be unloaded.");
		native_library_ptr_ = IntPtr.Zero;
	}

	public static T GetDelegate<T>(string name) where T : class
	{
		return Marshal.GetDelegateForFunctionPointer(dlsym(native_library_ptr_, name), typeof(T)) as T;
	}

#endif // #if UNITY_EDITOR_OSX
}

} // namespace UTJ {

/*
 * End of MacEditorNative.cs
 */

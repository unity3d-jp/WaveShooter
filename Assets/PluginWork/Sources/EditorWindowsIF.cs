/* -*- mode:C++; coding:utf-8-with-signature -*-
 *
 * WindowsEditorNative.cs - Project AnotherThread2
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

using System;
using System.Runtime.InteropServices;
using UnityEngine;
 
public static class EditorWindowsIF
{
#if UNITY_EDITOR_WIN
	private static IntPtr native_library_ptr_ = IntPtr.Zero;

    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool FreeLibrary(IntPtr hModule);
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadLibrary(string lpFileName);
    [DllImport("kernel32")]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

	public static void Load(string dllname)
	{
        Debug.Assert(native_library_ptr_ == IntPtr.Zero);
		var path = Application.dataPath+"/PluginWork/Plugins/x86_64/"+dllname+"/"+dllname+".dll";
		native_library_ptr_ = LoadLibrary(path);
        if (native_library_ptr_ == IntPtr.Zero) {
            Debug.LogError("Failed to load native library");
        }
	}
	
	public static void Unload()
	{
        if (native_library_ptr_ == IntPtr.Zero) {
			return;
		}
		bool success = FreeLibrary(native_library_ptr_);
        Debug.Log(success ?
				  "Native library successfully unloaded." :
				  "Native library could not be unloaded.");
		native_library_ptr_ = IntPtr.Zero;
	}

	public static System.Delegate GetDelegate<T>(string funcname)
	{
		IntPtr funcPtr = GetProcAddress(native_library_ptr_, funcname);
		if (funcPtr == IntPtr.Zero) {
            Debug.LogErrorFormat("Could not gain reference to method[{0}] address.", funcname);
			return null;
		}
		var func = Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(T));
		return func;
	}	

#endif // #if UNITY_EDITOR_WIN
}
/*
 * End of WindowsEditorNative.cs
 */

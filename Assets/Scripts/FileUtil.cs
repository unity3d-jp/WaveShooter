/* -*- mode:C++; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

public class FileUtil {

	public static string path_;

	public static IEnumerator preparePath(string filename)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		string orgpath = Application.streamingAssetsPath + "/" + filename;
		Debug.Log(orgpath);
		var www = new WWW(orgpath);
		yield return www;
		var path = Application.persistentDataPath + "/" + filename;
		System.IO.File.WriteAllBytes (path, www.bytes);
		#else
		var path = Application.streamingAssetsPath + "/" + filename;
		#endif
		path_ = path;
		yield return null;
	}

	public static string path { get { return path_; } }
	public static string content { get { return System.IO.File.ReadAllText(path_); } }
}

} // namespace UTJ {

/*
 * End of FileUtil.cs
 */

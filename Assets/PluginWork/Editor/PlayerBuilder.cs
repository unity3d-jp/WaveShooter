/* -*- mode:C++; coding:utf-8-with-signature -*-
 *
 * PlayerBuilder.cs - Project AnotherThread2
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
 * since Wed Jan 25 11:14:03 2017
 */

using UnityEngine;
using System.Collections;
using UnityEditor;

public class PlayerBuilder {

    [MenuItem ("Custom/Build iOS Player")]
	public static void buildiOS()
	{
		var success = PluginBuilder.BuildPluginIOS();
		if (!success) {
			return;
		}
		BuildPipeline.BuildPlayer(EditorBuildSettings.scenes,
								  "build/iOS",
								  BuildTarget.iOS,
								  BuildOptions.AcceptExternalModificationsToPlayer | BuildOptions.AutoRunPlayer);
								  // BuildOptions.None);
	}

    [MenuItem ("Custom/Build Android Player")]
	public static void buildAndroid()
	{
		var success = PluginBuilder.BuildPluginAndroid();
		if (!success) {
			return;
		}
		BuildPipeline.BuildPlayer(EditorBuildSettings.scenes,
								  "build/Android",
								  BuildTarget.Android,
								  BuildOptions.AcceptExternalModificationsToPlayer | BuildOptions.AutoRunPlayer);
								  // BuildOptions.None);
	}

    [MenuItem ("Custom/Build PS4 Player")]
	public static void buildPS4()
	{
		var success = PluginBuilder.BuildPluginPS4();
		if (!success) {
			return;
		}
		BuildPipeline.BuildPlayer(EditorBuildSettings.scenes,
								  "build/PS4",
								  BuildTarget.PS4,
								  BuildOptions.AcceptExternalModificationsToPlayer | BuildOptions.AutoRunPlayer);
								  // BuildOptions.None);
	}

}

/*
 * End of PlayerBuilder.cs
 */

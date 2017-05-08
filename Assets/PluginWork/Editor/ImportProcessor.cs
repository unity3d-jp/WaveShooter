/* -*- mode:C++; coding:utf-8-with-signature -*-
 *
 * ImportProcessor.cs - Project AnotherThread2
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
 * since Wed Jan 25 11:13:48 2017
 */

using UnityEngine;
using System.Collections;
using UnityEditor;

public class ImportProcessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets,
									   string[] deletedAssets,
									   string[] movedAssets,
									   string[] movedFromAssetPaths)
    {
		bool rebuild = false;
        foreach( string path in importedAssets ) {
			if (path.StartsWith("Assets/PluginWork/Sources/") &&
				(path.EndsWith(".cpp") ||
				 path.EndsWith(".hpp"))) {
				rebuild = true;
				break;
			}
        }

		if (rebuild) {
			PluginBuilder.BuildPluginEditor();
		}
    }
}

/*
 * End of ImportProcessor.cs
 */

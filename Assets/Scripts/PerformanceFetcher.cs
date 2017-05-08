/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

public class PerformanceFetcher : MonoBehaviour {

	void OnPreRender()
	{
		PerformanceMeter.Instance.beginConsoleRender();
	}

	void OnPreCull()
	{
		PerformanceMeter.Instance.endConsoleRender();
	}

}

} // namespace UTJ {

/*
 * End of PerformanceFetcher.cs
 */

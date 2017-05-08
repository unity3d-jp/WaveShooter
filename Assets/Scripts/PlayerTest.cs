/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

public class PlayerTest : MonoBehaviour {

	void Start()
	{
		TaskManager.Instance.init();
		Player.Instance.initialize();
	}

	void Update()
	{
		TaskManager.Instance.update(1f/60f, Time.time);
	}

}

} // namespace UTJ {

/*
 * End of PlayerTest.cs
 */

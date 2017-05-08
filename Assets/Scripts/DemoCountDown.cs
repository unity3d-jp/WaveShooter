using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoCountDown : MonoBehaviour {

	private float start_;
	private float period_ = 31f;

	void Start()
	{
		start_ = Time.time;
	}

	void Update()
	{
		var elapsed = Time.time - start_;
		var v = period_ - elapsed;
		if (v < 0f) {
			v = 0f;
		}
		GetComponent<Text>().text = string.Format("{0}", (int)v);
	}
}

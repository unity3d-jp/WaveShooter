/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class WaterSplashTest : MonoBehaviour {

	[SerializeField, Range(0, 1)]
	public float time = 0f;
	public Camera camera_;
	public Material material_;
	// private bool ready_ = false;
	private float time_ = 0f;

	private void spawn(ref Vector3 pos, ref Vector3 velocity)
	{
		WaterSplash.Instance.update(time_);
		WaterSplash.Instance.spawn(ref pos, ref velocity, time_);
		WaterSplash.Instance.renderUpdate(0 /* front */);
	}

	IEnumerator loop()
	{
		// ready_ = true;

		GetComponent<MeshRenderer>().sharedMaterial = material_;
		const float range = 0.05f;
		for (;;) {
			var vel = new Vector3(2f, 4f, 0f);
			var pos = new Vector3(Random.Range(-range, range),
								  Random.Range(-range, range),
								  Random.Range(-range, range));
			spawn(ref pos, ref vel);
			yield return new WaitForSeconds(0.1f);
			yield return null;
		}
	}

	void Start()
	{
#if UNITY_EDITOR
		UnityPluginIF.Load("UnityPlugin");
#endif
		BoxingPool.init();
		WaterSplash.Instance.init(material_, null /* reflection texture */);
		var col = new Color(1f, 1f, 1f);
		WaterSplash.Instance.SetBaseColor(ref col);
		WaterSplash.Instance.SetGravity(0f);
		var pos = new Vector3(0f, 0f, 0f);
		var vel = new Vector3(0f, 0f, 0f);
		spawn(ref pos, ref vel);
		// StartCoroutine(loop());
	}

    void OnApplicationQuit()
	{
#if UNITY_EDITOR
		UnityPluginIF.Unload();
#endif
    }

	void Update()
	{
		// if (!ready_) {
		// 	return;
		// }
		WaterSplash.Instance.render(0 /* front */, camera_, time_);
		var mesh = WaterSplash.Instance.getMesh();
		GetComponent<MeshFilter>().sharedMesh = mesh;
		var material = WaterSplash.Instance.getMaterial();
		GetComponent<MeshRenderer>().material = material;

		// time_ += 1f/60f;
		time_ = time;
	}
}

} // namespace UTJ {

/*
 * End of WaterSplashTest.cs
 */

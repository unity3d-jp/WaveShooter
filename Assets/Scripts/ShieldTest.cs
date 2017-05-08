/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class ShieldTest : MonoBehaviour {

	public Material material_;
	private bool ready_ = false;
	private MeshFilter mf_;
	private MeshRenderer mr_;

	IEnumerator loop()
	{
		ready_ = true;

		GetComponent<MeshRenderer>().sharedMaterial = material_;
		var range = 2.5f;
		for (;;) {
			Shield.Instance.begin();
			var pos = new Vector3(Random.Range(-range, range),
								  Random.Range(-range, range),
								  Random.Range(-range, range));
			var target = new Vector3(0,0,0);
			Shield.Instance.spawn(ref pos,
								  ref target,
								  Time.time,
								  (MyRandom.Range(0f, 1f) < 0.5f ?
								   Shield.Type.Green :
								   Shield.Type.Red));
			Shield.Instance.end(0 /* front */);
			yield return null;
		}
	}

	void Awake()
	{
		Shield.Instance.init(material_);
	}

	void Start()
	{
		mf_ = GetComponent<MeshFilter>();
		mr_ = GetComponent<MeshRenderer>();
		StartCoroutine(loop());
	}
	
	void Update()
	{
		if (!ready_) {
			return;
		}
		Shield.Instance.render(0 /* front */, Time.time);
		mf_.sharedMesh = Shield.Instance.getMesh();
		mr_.material = Shield.Instance.getMaterial();
		mr_.SetPropertyBlock(Shield.Instance.getMaterialPropertyBlock());
	}
}

} // namespace UTJ {

/*
 * End of ShieldTest.cs
 */

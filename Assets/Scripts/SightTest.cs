/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

public class SightTest : MonoBehaviour {

	public Camera main_camera_;
	public GameObject go_;
	public Material material_;

	IEnumerator loop()
	{
		for (;;) {
			var campos = main_camera_.transform.position;
			var camrot = main_camera_.transform.rotation;

			var view_matrix = Matrix4x4.TRS(campos,
											camrot,
											CV.Vector3One);
			var projection_matrix = main_camera_.projectionMatrix;
			var screen_matrix = projection_matrix * view_matrix.inverse;
			


			Sight.Instance.begin(0 /* front */);

			var pos = go_.transform.position;
			pos = screen_matrix.MultiplyPoint(pos);
			pos = new Vector3(pos.x*(-640), pos.y*(-360), 0f);

			Sight.Instance.regist(0 /* front */, ref pos);
			Sight.Instance.end(0 /* front */, null /* camera */);
			yield return null;
		}
	}

	void Start()
	{
		Sight.Instance.init(material_);
		SightRenderer.Instance.init(Sight.Instance);
		main_camera_ = GameObject.Find("Main Camera").GetComponent<Camera>();
		StartCoroutine(loop());
	}
	
	void Update()
	{
		Sight.Instance.render(0 /* front */);
		// var mesh = Sight.Instance.getMesh();
		// GetComponent<MeshFilter>().sharedMesh = mesh;
		// var material = Sight.Instance.getMaterial();
		// GetComponent<MeshRenderer>().material = material;
	}
}

} // namespace UTJ {

/*
 * End of SightTest.cs
 */

/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ {

public class WaterSurfaceTest : MonoBehaviour {

	public Material input_material_;

	void Awake()
	{
		WaterSurface.Instance.init(input_material_, true /* distortion */, true /* line_render */);
		WaterSurfaceRenderer.Instance.init(Camera.main.transform);
	}

	void Update()
	{
		bool clicked = false;
		var clicked_position = new Vector2(0f, 0f);
		if (Input.touchCount > 0) {
			clicked_position = Input.GetTouch(0).position;
			clicked = true;
		} else if (Input.GetMouseButton(0)) {
			clicked_position = Input.mousePosition;
			clicked = true;
		}

		if (clicked) {
            Ray ray = Camera.main.ScreenPointToRay(clicked_position);
			var plane = new Plane(new Vector3(0f, 1f, 0f), 0f);
            float distance;
            if (plane.Raycast(ray, out distance)) {
                var hit_pos = ray.GetPoint(distance);
				var point = new Vector2(hit_pos.x, hit_pos.z);
				WaterSurface.Instance.makeBump(ref point, 0.02f /* value */, 1f /* size */);
			}
		}

		WaterSurface.Instance.renderUpdate(0 /* front */);
		WaterSurface.Instance.render(0 /* front */);
	}

}

} // namespace UTJ {

/*
 * End of WaterSurfaceTest.cs
 */

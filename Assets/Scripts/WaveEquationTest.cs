/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UTJ {

public class WaveEquationTest : MonoBehaviour {
	private const float SCALE = 100f;
	public Material surface_material_;
	public Material wave_equation_material_;
	public Material water_input_material_;
	private WaveEquation wave_equation_;
	private WaterInputDrawer water_input_drawer_;

	void Start()
	{
		wave_equation_ = new WaveEquation();
		wave_equation_.init(256 /* width */, RenderTextureFormat.ARGB32, false /* clamp */);
		water_input_drawer_ = new WaterInputDrawer();
		water_input_drawer_.init(water_input_material_, SCALE);
	}

	void Update()
	{
		if (Time.timeSinceLevelLoad < 1f) {
			water_input_drawer_.putPoint(2f, 2f, -0.5f, 0.1f);
		}
		water_input_drawer_.renderUpdate(0 /* front */);
		water_input_drawer_.render(0 /* front */);
		wave_equation_.render(Time.time,
							  wave_equation_material_,
							  water_input_drawer_.getRenderTexture());
		surface_material_.SetTexture("_MainTex", wave_equation_.getLatestRenderTexture());
		wave_equation_.bind(surface_material_);
	}

	void OnDestroy()
	{
		water_input_drawer_.finalize();
	}
}

} // namespace UTJ {
/*
 * End of WaveEquationTest.cs
 */

/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ {

public class LightEnvironmentController : MonoBehaviour {
	// singleton
	static LightEnvironmentController instance_;
	public static LightEnvironmentController Instance { get { return instance_ ?? (instance_ = GameObject.Find("light_environment_controller").GetComponent<LightEnvironmentController>()); } }

	public bool reverse_ = false;
	public GameObject light_go_;
	public Material skybox_material_;
	public Material skybox_mix_material_;
	public Material aura_material_;
	public Material dust_material_;
	public Material debris_material_;
	public Material zako_material_;
	public Material snake_material_;
	private Light light_;
	private Transform light_transform_;
	private float ratio_;
	private Quaternion[] light_rot_list_;
	private Color[] color_list_;
	private float ratio_speed_;
	private int SwitchSkyBoxId;
	private int BaseColorId;
	private double prev_render_time_;

	void Start()
	{
		light_transform_ = light_go_.transform;
		ratio_ = 0f;
		ratio_speed_ = 1f;
		light_rot_list_ = new Quaternion[2] {
			Quaternion.Euler(12, 32, 0),
			Quaternion.Euler(30, 181, 0),
		};
		color_list_ = new Color[2] {
			new Color(1f, 0.8f, 0.5f, 1f),
			new Color(0.8f, 1f, 0.9f, 1f),
		};
		light_ = light_go_.GetComponent<Light>();
		SwitchSkyBoxId = Shader.PropertyToID("_SwitchSkybox");
		BaseColorId = Shader.PropertyToID("_BaseColor");
	}
	
	public void init()
	{}

	public void render(double render_time)
	{
		float dt = (float)(prev_render_time_ - render_time);
		if (dt == 0f) {
			return;
		}
		prev_render_time_ = render_time;

		ratio_ += ratio_speed_ * dt;
		if (ratio_ > 10f || ratio_ < -10f) {
			ratio_speed_ = -ratio_speed_;
		}

		var r = Mathf.Clamp(ratio_, 0f, 1f);
		var col = Color.LerpUnclamped(color_list_[0], color_list_[1], reverse_ ? 1f-r : r);
		light_transform_.rotation = Quaternion.Lerp(light_rot_list_[0],
													light_rot_list_[1],
													r);
		if (r <= 0f || 1f <= r) {
			RenderSettings.skybox = skybox_material_;
			skybox_material_.SetFloat(SwitchSkyBoxId, r);
			zako_material_.SetFloat(SwitchSkyBoxId, r);
			snake_material_.SetFloat(SwitchSkyBoxId, r);
		} else {
			RenderSettings.skybox = skybox_mix_material_;
			skybox_mix_material_.SetFloat(SwitchSkyBoxId, r);
			zako_material_.SetFloat(SwitchSkyBoxId, r);
			snake_material_.SetFloat(SwitchSkyBoxId, r);
		}
		aura_material_.SetColor(BaseColorId, col*0.2f);
		dust_material_.SetColor(BaseColorId, col);
		debris_material_.SetColor(BaseColorId, col);
		light_.color = col;
		WaterSplash.Instance.SetBaseColor(ref col);
		
		col = Color.LerpUnclamped(col, new Color(0.5f, 0.75f, 1f), 0.5f);
		WaterSurfaceRenderer.Instance.SetBaseColor(ref col);
	}
}

} // namespace UTJ {
/*
 * End of LightEnvironmentController.cs
 */

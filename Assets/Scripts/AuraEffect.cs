/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ {

public class AuraEffect : MonoBehaviour {
	// singleton
	static AuraEffect instance_;
	public static AuraEffect Instance { get { return instance_ ?? (instance_ = GameObject.Find("Main Camera").GetComponent<AuraEffect>()); } }

	public Material wave_equation_material_;
	public Material aura_material_;
	private WaveEquation wave_equation_;
	private Color color_;
	private int AuraColorId;
	private float value_;
	private double render_time_;

	void Start()
	{
		wave_equation_ = new WaveEquation();
		// wave_equation_.init(128, RenderTextureFormat.ARGB2101010, true /* clamp */);
		wave_equation_.init(128, RenderTextureFormat.ARGB32, true /* clamp */);
		color_ = new Color(0.5f, 0.4f, 0.1f, 1f);
		AuraColorId = Shader.PropertyToID("_AuraColor");
		value_ = 0f;
		render_time_ = 0;
	}

	public void init()
	{}

	public void render(double render_time)
	{
		render_time_ = render_time;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		float v = 0f;
		if (Player.Instance != null) {
			v = Player.Instance.getAuraValue();
		}
		value_ = Mathf.Lerp(value_, v, 0.1f);
		var col = color_ * value_;
		aura_material_.SetColor(AuraColorId, col);
		wave_equation_.render(render_time_, wave_equation_material_, source);
		wave_equation_.bind(aura_material_);
		Graphics.Blit(source, destination, aura_material_);
	}
}

} // namespace UTJ {

/*
 * End of AuraEffect.cs
 */

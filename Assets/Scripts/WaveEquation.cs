/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEquation {

	private RenderTexture[] render_texture_list_;
	private int front_;
	private int latest_;
	private int width_;
	private double last_rendered_time_;
	private float round_adjuster_;
	public float getRenderTextureWidth() { return width_; }
	static readonly int material_PrevTex = Shader.PropertyToID("_PrevTex");
	static readonly int material_PrevPrevTex = Shader.PropertyToID("_PrevPrevTex");
	static readonly int material_Stride = Shader.PropertyToID("_Stride");
	static readonly int material_InputTex = Shader.PropertyToID("_InputTex");
	static readonly int material_WaveTex = Shader.PropertyToID("_WaveTex");
	static readonly int material_RoundAdjuster = Shader.PropertyToID("_RoundAdjuster");

	public void init(int width, RenderTextureFormat format, bool clamp)
	{
		if (!SystemInfo.SupportsRenderTextureFormat(format)) {
			format = RenderTextureFormat.ARGB32;
		}

		width_ = width;
		render_texture_list_ = new RenderTexture[3];
		for (var i = 0; i < render_texture_list_.Length; ++i) {
			var rt = new RenderTexture(width_, width_, 0 /* depth */,
									   format);
			Debug.Assert(rt != null);
			rt.wrapMode = clamp ? TextureWrapMode.Clamp : TextureWrapMode.Repeat;
			rt.Create();
			render_texture_list_[i] = rt;
			Graphics.SetRenderTarget(rt);
			GL.Clear(false /* clearDepth */, true /* clearColor */,
					 new Color(0.5f, 0.5f, 0.5f, 0.5f));
		}
		front_ = 0;
		latest_ = 0;
		last_rendered_time_ = 0;
		// Debug.Log(string.Format("{0}:{1}:{2}",
		// 						SystemInfo.graphicsDeviceName,
		// 						SystemInfo.graphicsDeviceID,
		// 						SystemInfo.graphicsDeviceVendorID));
		round_adjuster_ = 0f;
		if (SystemInfo.graphicsDeviceName == "PowerVR SGX 543") {
			round_adjuster_ = 0.75f/255f;
		} else if (SystemInfo.graphicsDeviceName == "Apple A7 GPU") {
			round_adjuster_ = -0.5f/255f;
		} else if (SystemInfo.graphicsDeviceName == "Apple A8 GPU") {
			round_adjuster_ = 0f;
		}
	}
	
	public void render(double render_time, Material material, Texture input_texture)
	{
		if (render_time <= last_rendered_time_) {
			return;
		}
		last_rendered_time_ = render_time;

		int prev = (front_ + 2) % 3;
		int prevprev = (front_ + 1) % 3;
		material.SetTexture(material_PrevTex, render_texture_list_[prev]);
		material.SetTexture(material_PrevPrevTex, render_texture_list_[prevprev]);
		material.SetVector(material_Stride, new Vector2(1f/(float)width_, 1f/(float)width_));
		material.SetTexture(material_InputTex, input_texture);
		material.SetFloat(material_RoundAdjuster, round_adjuster_);
		Graphics.SetRenderTarget(render_texture_list_[front_]);
		GL.Clear(false /* clearDepth */, true /* clearColor */,
				 new Color(0.5f, 0.5f, 0.5f, 0.5f));
		Graphics.Blit(render_texture_list_[prev],
					  render_texture_list_[front_],
					  material);
		latest_ = front_;
		front_ = prevprev;
	}

	public void bind(Material material)
	{
		material.SetTexture(material_WaveTex, render_texture_list_[latest_]);
	}

	public RenderTexture getLatestRenderTexture()
	{
		return render_texture_list_[latest_];
	}
}

/*
 * End of WaveEquation.cs
 */

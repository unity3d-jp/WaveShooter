/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UTJ {

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaterSurfaceRenderer : MonoBehaviour {
	// singleton
	static WaterSurfaceRenderer instance_;
	public static WaterSurfaceRenderer Instance { get { return instance_ ?? (instance_ = GameObject.Find("water_surface_renderer").GetComponent<WaterSurfaceRenderer>()); } }

	public LayerMask reflectLayers = -1;
	public Material surface_material_;
	public Material wave_equation_material_;
	private Vector4 reflectionPlane = new Vector4(0f, 1f, 0f, 0f);
	private Dictionary<Camera, Camera> reflection_camera_hash_ = new Dictionary<Camera, Camera>();
	private RenderTexture reflection_texture_;
	private MeshFilter mf_;
	private Transform centered_transform_;
	private Color current_base_color_;
	
	private WaveEquation wave_equation_;
	private float scale_;
	private class MaterialInfo {
		public static readonly int WaveSpeed = Shader.PropertyToID("_WaveSpeed");
		public static readonly int WaveScale = Shader.PropertyToID("_WaveScale");
		public static readonly int WaveOffset = Shader.PropertyToID("_WaveOffset");
		public static readonly int WaveScale4 = Shader.PropertyToID("_WaveScale4");
		public static readonly int Scale = Shader.PropertyToID("_Scale");
		public static readonly int RScale = Shader.PropertyToID("_RScale");
		public static readonly int Stride = Shader.PropertyToID("_Stride");
		public static readonly int ReflectionTex = Shader.PropertyToID("_ReflectionTex");
		public static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
	}

	public Texture getReflectionTexture() { return reflection_texture_; }
	public void SetBaseColor(ref Color col) { current_base_color_ = col; }

	private void calc_reflection_matrix(ref Matrix4x4 mat, Vector4 plane)
	{
		mat.m00 = (1f - 2f * plane.x * plane.x);
		mat.m01 = (- 2f * plane.x * plane.y);
		mat.m02 = (- 2f * plane.x * plane.z);
		mat.m03 = (- 2f * plane.x * plane.w);
		mat.m10 = (- 2f * plane.y * plane.x);
		mat.m11 = (1f - 2f * plane.y * plane.y);
		mat.m12 = (- 2f * plane.y * plane.z);
		mat.m13 = (- 2f * plane.y * plane.w);
		mat.m20 = (- 2f * plane.z * plane.x);
		mat.m21 = (- 2f * plane.z * plane.y);
		mat.m22 = (1f - 2f * plane.z * plane.z);
		mat.m23 = (- 2f * plane.z * plane.w);
		mat.m30 = 0f;
		mat.m31 = 0f;
		mat.m32 = 0f;
		mat.m33 = 1f;
	}

	private void create_objects(Camera camera, out Camera reflection_camera)
	{
		if (reflection_texture_ == null) {
			const int textureSize = 128;
			var format = RenderTextureFormat.RGB565;
			if (!SystemInfo.SupportsRenderTextureFormat(format)) {
				format = RenderTextureFormat.Default;
			}
			reflection_texture_ = new RenderTexture(textureSize, textureSize, 16 /* depth */, format);
			reflection_texture_.name = "__WaterReflection" + GetInstanceID();
			reflection_texture_.isPowerOfTwo = true;
			// reflection_texture_.hideFlags = HideFlags.DontSave;
		}

		reflection_camera_hash_.TryGetValue(camera, out reflection_camera);
		if (reflection_camera == null) {
			var go = new GameObject("ReflectionCamera" + GetInstanceID() + " for " + camera.GetInstanceID(), typeof(Camera), typeof(Skybox));
			reflection_camera = go.GetComponent<Camera>();
			reflection_camera.enabled = false;
			reflection_camera.useOcclusionCulling = false;
			reflection_camera.transform.position = transform.position;
			reflection_camera.transform.rotation = transform.rotation;
			go.hideFlags = HideFlags.HideAndDontSave;
			reflection_camera_hash_[camera] = reflection_camera;
		}
	}

	public void init(Transform centered_transform)
	{
		centered_transform_ = centered_transform;
		mf_ = GetComponent<MeshFilter>();
		mf_.sharedMesh = WaterSurface.Instance.getMesh();
		scale_ = WaterSurface.Instance.getScale();
		wave_equation_ = new WaveEquation();
		wave_equation_.init(256 /* width */, RenderTextureFormat.ARGB32, false /* clamp */);
		// wave_equation_.init(512 /* width */, RenderTextureFormat.R8, false /* clamp */);
		var mr = GetComponent<MeshRenderer>();
		mr.sharedMaterial = surface_material_;
	}

	void OnDisable()
	{
		DestroyImmediate(reflection_texture_);
		reflection_texture_ = null;
		foreach (var kvp in reflection_camera_hash_) {
			DestroyImmediate((kvp.Value).gameObject);
		}
		reflection_camera_hash_.Clear();
		WaterSurface.Instance.finalize();
	}

	public void render(double render_time)
	{
		if (centered_transform_ != null) {
			var dist = WaterSurface.Instance.getScale()*0.5f;
			var pos = centered_transform_.TransformPoint(new Vector3(0f, 0f, dist));
			pos.y = 0f;			// ground
			transform.localPosition = pos;
			float y = centered_transform_.rotation.eulerAngles.y;
			transform.localRotation = Quaternion.Euler(0f, y, 0f);
		}

		Material mat = surface_material_;
		wave_equation_.render(render_time,
							  wave_equation_material_,
							  WaterSurface.Instance.getWaterInput());
		wave_equation_.bind(mat);

		Vector4 waveSpeed = mat.GetVector(MaterialInfo.WaveSpeed);
		const float waveScale = 0.75f;
		Vector4 waveScale4 = new Vector4(waveScale, waveScale, waveScale * 0.2f, waveScale * 0.225f);
		float t = (float)render_time / 20.0f;
		Vector4 offsetClamped = new Vector4(Mathf.Repeat(waveSpeed.x*waveScale4.x*t, 1f),
											Mathf.Repeat(waveSpeed.y*waveScale4.y*t, 1f),
											Mathf.Repeat(waveSpeed.z*waveScale4.z*t, 1f),
											Mathf.Repeat(waveSpeed.w*waveScale4.w*t, 1f));

		mat.SetVector(MaterialInfo.WaveOffset, offsetClamped);
		mat.SetVector(MaterialInfo.WaveScale4, waveScale4);
		mat.SetVector(MaterialInfo.Scale, new Vector2(scale_, scale_));
		mat.SetVector(MaterialInfo.RScale, new Vector2(1f/(scale_), 1f/(scale_)));
		var stride = 1f/wave_equation_.getRenderTextureWidth();
		mat.SetVector(MaterialInfo.Stride, new Vector2(stride, stride));
		mat.SetColor(MaterialInfo.BaseColor, current_base_color_);
	}

	void OnWillRenderObject()
	{
		Camera camera = Camera.current;
		Camera reflection_camera;
		create_objects(camera, out reflection_camera);

		var reflection_matrix = new Matrix4x4();
		calc_reflection_matrix(ref reflection_matrix, reflectionPlane);

		var local_reflection_matrix = camera.worldToCameraMatrix * reflection_matrix;
	    {
			var normal = new Vector3(reflectionPlane.x, reflectionPlane.y, reflectionPlane.z);
			Vector3 cnormal = local_reflection_matrix.MultiplyVector(normal);
			Vector3 cpos = local_reflection_matrix.MultiplyPoint(Vector3.zero);
			Vector4 clip_plane = new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
			reflection_camera.worldToCameraMatrix = local_reflection_matrix;
			reflection_camera.projectionMatrix = camera.CalculateObliqueMatrix(clip_plane);
			// reflection_camera.cullingMatrix = camera.projectionMatrix * camera.worldToCameraMatrix;
			reflection_camera.cullingMask = ((~(1<<4))&(~(1<<5))) & reflectLayers.value; // never render water and UI layer
			reflection_camera.targetTexture = reflection_texture_;
		}
	    {
			Vector3 campos = camera.transform.position;
			Vector3 refcampos = reflection_matrix.MultiplyPoint(campos);
			reflection_camera.farClipPlane = camera.farClipPlane;
			reflection_camera.nearClipPlane = camera.nearClipPlane;
			reflection_camera.transform.position = refcampos;
			var euler = camera.transform.eulerAngles;
			reflection_camera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);
		}
		{
			bool prev = GL.invertCulling;
			GL.invertCulling = !prev;
			reflection_camera.Render();
			GL.invertCulling = prev;
			surface_material_.SetTexture(MaterialInfo.ReflectionTex, reflection_texture_);
		}
	}
}

} // namespace UTJ {

/*
 * End of WaterSurfaceRenderer.cs
 */

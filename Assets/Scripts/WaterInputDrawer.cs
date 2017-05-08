/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ {

public class WaterInputDrawer {
	struct InputInfo {
		public enum Type {
			Circle,
		}
		public Type type_;
		public float x_;
		public float y_;
		public float value_;
		public float size_;
		public InputInfo(Type type, float x, float y, float value, float size) {
			type_ = type;
			x_ = x;
			y_ = y;
			value_ = value;
			size_ = size;
		}
	}
	private List<InputInfo> input_info_list_;

	private Material water_input_material_;
	private float scale_;
	private float scale_half_;
	private Mesh mesh_;
	private List<Vector3>[] vertices_;
	private List<int>[] indices_;
	private List<Vector3> empty_vertices_;
	private List<int> empty_indices_;
	private RenderTexture render_texture_;
	private Camera camera_;

	const int WATER_INPUT_LAYER = 8;
	const int VERTICES_MAX = 256;

	public RenderTexture getRenderTexture() { return render_texture_; }

	public void init(Material water_input_material,
					 float scale)
	{
		input_info_list_ = new List<InputInfo>();
		input_info_list_.Capacity = VERTICES_MAX/4;

		water_input_material_ = water_input_material;
		scale_ = scale;
		scale_half_ = scale*0.5f;
		mesh_ = new Mesh();
		mesh_.MarkDynamic();
		mesh_.name = "input_draw";
		mesh_.bounds = new Bounds(Vector3.zero, Vector3.one * 99999999);
		vertices_ = new List<Vector3>[2];
		vertices_[0] = new List<Vector3>();
		vertices_[0].Capacity = VERTICES_MAX;
		vertices_[1] = new List<Vector3>();
		vertices_[1].Capacity = VERTICES_MAX;
		indices_ = new List<int>[2];
		indices_[0] = new List<int>();
		indices_[0].Capacity = VERTICES_MAX;
		indices_[1] = new List<int>();
		indices_[1].Capacity = VERTICES_MAX;
		empty_vertices_ = new List<Vector3>();
		empty_indices_ = new List<int>();

		render_texture_ = new RenderTexture(128, 128, 0 /* depth */,
											RenderTextureFormat.ARGB32);
		// render_texture_.hideFlags = HideFlags.HideAndDontSave;
		render_texture_.name = "water_input";
		render_texture_.filterMode = FilterMode.Bilinear;
		render_texture_.autoGenerateMips = false;
		render_texture_.wrapMode = TextureWrapMode.Repeat;
		render_texture_.anisoLevel = 0;
		render_texture_.Create();
		Graphics.SetRenderTarget(render_texture_);
		GL.Clear(false /* clearDepth */, true /* clearColor */,
				 new Color(0.5f, 0.5f, 0.5f, 0.5f));

		var camera_go = new GameObject();
		camera_go.name = "water_input";
		// camera_go.hideFlags = HideFlags.HideAndDontSave;
		camera_ = camera_go.AddComponent<Camera>();
		camera_.useOcclusionCulling = false;
		camera_.cullingMask = 1 << WATER_INPUT_LAYER;
		camera_.orthographic = true;
		camera_.orthographicSize = scale_half_;
		camera_.nearClipPlane = -1f;
		camera_.farClipPlane = 1f;
		camera_.clearFlags = CameraClearFlags.SolidColor;
		camera_.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		camera_.targetTexture = render_texture_;
	}

	public void finalize()
	{
		render_texture_.Release();
		if (camera_ != null) {
			Object.Destroy(camera_.gameObject);
		}
	}

	public void putPoint(float x, float y, float value, float size)
	{
		if (input_info_list_.Count >= input_info_list_.Capacity) {
			Debug.LogError("exceed water input buffer.");
			return;
		}
		input_info_list_.Add(new InputInfo(InputInfo.Type.Circle,
										   Mathf.Repeat(x+scale_half_, scale_)-scale_half_,
										   Mathf.Repeat(y+scale_half_, scale_)-scale_half_,
										   value,
										   size));
	}

	public void renderUpdate(int front)
	{
		foreach (var input_info in input_info_list_) {
			var posx = input_info.x_;
			var posy = input_info.y_;
			var size = input_info.size_;
			var value = input_info.value_;
			vertices_[front].Add(new Vector3(posx-size, posy-size, value));
			vertices_[front].Add(new Vector3(posx+size, posy-size, value));
			vertices_[front].Add(new Vector3(posx-size, posy+size, value));
			vertices_[front].Add(new Vector3(posx+size, posy+size, value));
			int idx = vertices_[front].Count-4;
			indices_[front].Add(idx+0);
			indices_[front].Add(idx+1);
			indices_[front].Add(idx+2);
			indices_[front].Add(idx+2);
			indices_[front].Add(idx+1);
			indices_[front].Add(idx+3);
		}
		input_info_list_.Clear();
	}					 

	public void render(int front)
	{
		mesh_.SetTriangles(empty_indices_, 0, false); // avoid error
		mesh_.SetVertices(empty_vertices_);		   // avoid error
		mesh_.SetVertices(vertices_[front]);
		mesh_.SetTriangles(indices_[front],
						   0 /* submesh */,
						   false /* calculateBounds */);
		Graphics.DrawMesh(mesh_,
						  Vector3.zero,
						  Quaternion.identity,
						  water_input_material_,
						  WATER_INPUT_LAYER,
						  camera_,
						  0 /* submeshIndex */,
						  null /* properties */,
						  false /* castShadow */,
						  false /* receiveShadow */,
						  false /* useLightProbe */);
		vertices_[front].Clear();
		indices_[front].Clear();
	}
}

} // namespace UTJ {

/*
 * End of WaterInputDrawer.cs
 */

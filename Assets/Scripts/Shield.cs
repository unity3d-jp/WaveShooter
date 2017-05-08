/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

public class Shield
{
	// singleton
	static Shield instance_;
	public static Shield Instance { get { return instance_ ?? (instance_ = new Shield()); } }

	public enum Type
	{
		None,
		Green,
		Red,
	}

	const int SHIELD_MAX = 8;

	private Vector3[] positions_;
	private Vector3[] dirs_;
	private Vector2[] uv2_list_;

	private Vector3[][] vertices_;
	private Vector3[][] normals_;
	private Vector2[][] uv2s_;
	private int spawn_index_;
	private Mesh mesh_;
	private Material material_;
	private MaterialPropertyBlock material_property_block_;
	static readonly int material_CurrentTime = Shader.PropertyToID("_CurrentTime");

	public Mesh getMesh() { return mesh_; }
	public Material getMaterial() { return material_; }
	public MaterialPropertyBlock getMaterialPropertyBlock() { return material_property_block_; }

	public void init(Material material)
	{
		positions_ = new Vector3[SHIELD_MAX];
		dirs_ = new Vector3[SHIELD_MAX];
		uv2_list_ = new Vector2[SHIELD_MAX];

		vertices_ = new Vector3[2][] { new Vector3[SHIELD_MAX*4], new Vector3[SHIELD_MAX*4], };
		normals_ = new Vector3[2][] { new Vector3[SHIELD_MAX*4], new Vector3[SHIELD_MAX*4], };
		uv2s_ = new Vector2[2][] { new Vector2[SHIELD_MAX*4], new Vector2[SHIELD_MAX*4], };

		var triangles = new int[SHIELD_MAX * 6];
		for (var i = 0; i < SHIELD_MAX; ++i) {
			triangles[i*6+0] = i*4+0;
			triangles[i*6+1] = i*4+1;
			triangles[i*6+2] = i*4+2;
			triangles[i*6+3] = i*4+2;
			triangles[i*6+4] = i*4+1;
			triangles[i*6+5] = i*4+3;
		}

		var uvs = new Vector2[SHIELD_MAX*4];
		for (var i = 0; i < SHIELD_MAX; ++i) {
			uvs[i*4+0] = new Vector2(0f, 0f);
			uvs[i*4+1] = new Vector2(1f, 0f);
			uvs[i*4+2] = new Vector2(0f, 1f);
			uvs[i*4+3] = new Vector2(1f, 1f);
		}

		mesh_ = new Mesh();
		mesh_.MarkDynamic();
		mesh_.name = "shield";
		mesh_.vertices = vertices_[0];
		mesh_.normals = normals_[0];
		mesh_.triangles = triangles;
		mesh_.uv = uvs;
		mesh_.uv2 = uv2s_[0];
		mesh_.bounds = new Bounds(CV.Vector3Zero, CV.Vector3One * 99999999);
		material_ = material;
		material_property_block_ = new MaterialPropertyBlock();
#if UNITY_5_3 || UNITY_SWITCH
		material_.SetColor("_Colors0", new Color(0f, 0f, 0f, 0f));
		material_.SetColor("_Colors1", new Color(0f, 1f, 0.5f));
		material_.SetColor("_Colors2", new Color(1f, 0.5f, 0.25f));
#else
		var col_list = new Vector4[] {
			new Vector4(0f, 0f, 0f, 0f),
			new Vector4(0f, 1f, 0.5f, 1f),
			new Vector4(1f, 0.1f, 0.1f, 1f),
		};
		material_property_block_.SetVectorArray("_Colors", col_list);
#endif

		spawn_index_ = 0;
	}

	public void begin()
	{
	}

	public void end(int front)
	{
		for (var i = 0; i < SHIELD_MAX; ++i) {
			int idx = i*4;
			vertices_[front][idx+0] = positions_[i];
			vertices_[front][idx+1] = positions_[i];
			vertices_[front][idx+2] = positions_[i];
			vertices_[front][idx+3] = positions_[i];
			normals_[front][idx+0] = dirs_[i];
			normals_[front][idx+1] = dirs_[i];
			normals_[front][idx+2] = dirs_[i];
			normals_[front][idx+3] = dirs_[i];
			uv2s_[front][idx+0] = uv2_list_[i];
			uv2s_[front][idx+1] = uv2_list_[i];
			uv2s_[front][idx+2] = uv2_list_[i];
			uv2s_[front][idx+3] = uv2_list_[i];
		}
	}

	public void render(int front, double render_time)
	{
		mesh_.vertices = vertices_[front];
		mesh_.normals = normals_[front];
		mesh_.uv2 = uv2s_[front];
		material_.SetFloat(material_CurrentTime, (float)render_time);
	}

	public void spawn(ref Vector3 pos,
					  ref Vector3 target,
					  double update_time,
					  Type type)
	{
		int id = spawn_index_;
		++spawn_index_;
		if (spawn_index_ >= SHIELD_MAX) {
			spawn_index_ = 0;
		}

		positions_[id] = pos;
		dirs_[id] = (target - pos).normalized;
		uv2_list_[id] = new Vector2((float)update_time, (float)type);
	}
}

} // namespace UTJ {
/*
 * End of Shield.cs
 */

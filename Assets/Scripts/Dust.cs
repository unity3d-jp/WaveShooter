/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

public class Dust
{
	// singleton
	static Dust instance_;
	public static Dust Instance { get { return instance_ ?? (instance_ = new Dust()); } }

	const int DUST_MAX = 128;
	private float range_;
	private float rangeR_;

	private Mesh mesh_;
	private Material material_;
	static readonly int material_TargetPosition = Shader.PropertyToID("_TargetPosition");
	static readonly int material_CamUp = Shader.PropertyToID("_CamUp");
	static readonly int material_CurrentTime = Shader.PropertyToID("_CurrentTime");

	public Mesh getMesh() { return mesh_; }
	public Material getMaterial() { return material_; }

	public void init(Material material)
	{
		range_ = 16;
		rangeR_ = 1.0f/range_;
		var vertices = new Vector3[DUST_MAX*4];
		for (var i = 0; i < DUST_MAX; ++i) {
			float x = Random.Range(-range_, range_);
			float y = Random.Range(-range_, range_);
			float z = Random.Range(-range_, range_);
			var pos = new Vector3(x, y, z);
			vertices[i*4+0] = pos;
			vertices[i*4+1] = pos;
			vertices[i*4+2] = pos;
			vertices[i*4+3] = pos;
		}

		var triangles = new int[DUST_MAX * 6];
		for (var i = 0; i < DUST_MAX; ++i) {
			triangles[i*6+0] = i*4+0;
			triangles[i*6+1] = i*4+1;
			triangles[i*6+2] = i*4+2;
			triangles[i*6+3] = i*4+2;
			triangles[i*6+4] = i*4+1;
			triangles[i*6+5] = i*4+3;
		}

		var normals = new Vector3[DUST_MAX*4];
		for (var i = 0; i < DUST_MAX; ++i) {
			float x = Random.Range(-range_, range_);
			float y = Random.Range(-range_, range_);
			float z = Random.Range(-range_, range_);
			var normal = new Vector3(x, y, z).normalized;
			normals[i*4+0] = normal;
			normals[i*4+1] = normal;
			normals[i*4+2] = normal;
			normals[i*4+3] = normal;
		}

		var uvs = new Vector2[DUST_MAX*4];
		for (var i = 0; i < DUST_MAX; ++i) {
			uvs[i*4+0] = new Vector2(0f, 0f);
			uvs[i*4+1] = new Vector2(1f, 0f);
			uvs[i*4+2] = new Vector2(0f, 1f);
			uvs[i*4+3] = new Vector2(1f, 1f);
		}

		var uv2s = new Vector2[DUST_MAX*4];
		for (var i = 0; i < DUST_MAX; ++i) {
			var v = new Vector2(Random.Range(1f, 8f), 0f);
			uv2s[i*4+0] = v;
			uv2s[i*4+1] = v;
			uv2s[i*4+2] = v;
			uv2s[i*4+3] = v;
		}

		mesh_ = new Mesh();
		mesh_.name = "dust";
		mesh_.vertices = vertices;
		mesh_.triangles = triangles;
		mesh_.normals = normals;
		mesh_.uv = uvs;
		mesh_.uv2 = uv2s;
		mesh_.bounds = new Bounds(Vector3.zero, Vector3.one * 99999999);
		mesh_.UploadMeshData(true /* markNoLogerReadable */);
		material_ = material;
		material_.SetFloat("_Range", range_);
		material_.SetFloat("_RangeR", rangeR_);
	}

	public void render(int front, Camera camera, double render_time)
	{
		var target_position = camera.transform.TransformPoint(Vector3.forward * range_*0.5f);
		material_.SetVector(material_TargetPosition, target_position);
		material_.SetVector(material_CamUp, camera.transform.up);
		material_.SetFloat(material_CurrentTime, (float)render_time);
	}
}

} // namespace UTJ {
/*
 * End of Dust.cs
 */

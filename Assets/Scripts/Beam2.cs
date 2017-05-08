/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

public class Beam2
{
	// singleton
	static Beam2 instance_;
	public static Beam2 Instance { get { return instance_ ?? (instance_ = new Beam2()); } }

	public enum Type
	{
		None,
		NoneA,
		EnemyBullet,
		EnemyBulletA,
	}

	const int BEAM2_MAX = 256;

	private bool[] alive_table_;
	private int spawn_index_;

	private Vector2[] uv2_list_;

	private Vector3[][] vertices_;
	private Vector3[][] normals_;
	private Vector2[][] uv2s_;
	private Mesh mesh_;
	private Material material_;
	private MaterialPropertyBlock material_property_block_;

	public Mesh getMesh() { return mesh_; }
	public Material getMaterial() { return material_; }
	public MaterialPropertyBlock getMaterialPropertyBlock() { return material_property_block_; }

	public void init(Material material)
	{
		alive_table_ = new bool[BEAM2_MAX];
		for (var i = 0; i < BEAM2_MAX; ++i) {
			alive_table_[i] = false;
		}
		spawn_index_ = 0;

		uv2_list_ = new Vector2[BEAM2_MAX];

		vertices_ = new Vector3[2][] { new Vector3[BEAM2_MAX*6], new Vector3[BEAM2_MAX*6], };
		normals_ = new Vector3[2][] { new Vector3[BEAM2_MAX*6], new Vector3[BEAM2_MAX*6], };
		uv2s_ = new Vector2[2][] { new Vector2[BEAM2_MAX*6], new Vector2[BEAM2_MAX*6], };
		for (var i = 0; i < BEAM2_MAX; ++i) {
			destroy(i);
		}

		var triangles = new int[BEAM2_MAX * 12];
		for (var i = 0; i < BEAM2_MAX; ++i) {
			triangles[i*12+ 0] = i*6+0;
			triangles[i*12+ 1] = i*6+1;
			triangles[i*12+ 2] = i*6+2;
			triangles[i*12+ 3] = i*6+2;
			triangles[i*12+ 4] = i*6+1;
			triangles[i*12+ 5] = i*6+3;
			triangles[i*12+ 6] = i*6+2;
			triangles[i*12+ 7] = i*6+3;
			triangles[i*12+ 8] = i*6+4;
			triangles[i*12+ 9] = i*6+4;
			triangles[i*12+10] = i*6+3;
			triangles[i*12+11] = i*6+5;
		}

		var uvs = new Vector2[BEAM2_MAX*6];
		for (var i = 0; i < BEAM2_MAX; ++i) {
			uvs[i*6+0] = new Vector2(0f, 0f);
			uvs[i*6+1] = new Vector2(1f, 0f);
			uvs[i*6+2] = new Vector2(0f, 0.5f);
			uvs[i*6+3] = new Vector2(1f, 0.5f);
			uvs[i*6+4] = new Vector2(0f, 0.5f);
			uvs[i*6+5] = new Vector2(1f, 0.5f);
		}

		mesh_ = new Mesh();
		mesh_.MarkDynamic();
		mesh_.name = "beam2";
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
		material_.SetColor("_Colors1", new Color(0f, 0f, 0f, 0f));
		material_.SetColor("_Colors2", new Color(1f, 0.5f, 0.25f, 1f));
		material_.SetColor("_Colors3", new Color(1f, 0.5f, 0.25f, 0f));
#else
		var col_list = new Vector4[] {
			new Vector4(0f, 0f, 0f, 0f),
			new Vector4(0f, 0f, 0f, 0f),
			new Vector4(1f, 0.5f, 0.25f, 1f),
			new Vector4(1f, 0.5f, 0.25f, 0f),
		};
		material_property_block_.SetVectorArray("_Colors", col_list);
#endif
	}

	public void render(int front)
	{
		mesh_.vertices = vertices_[front];
		mesh_.normals = normals_[front];
		mesh_.uv2 = uv2s_[front];
	}

	public void begin(int front)
	{
		var far = new Vector3(0f, 0f, 0f);
		for (var i = 0; i < BEAM2_MAX*6; ++i) {
			vertices_[front][i] = far;
		}
		var zero = new Vector2(0f, 0f);
		for (var i = 0; i < BEAM2_MAX*6; ++i) {
			uv2s_[front][i] = zero;
		}
	}

	public void end()
	{
	}

	public int spawn(float width, Type type)
	{
		int cnt = 0;
		while (alive_table_[spawn_index_]) {
			++spawn_index_;
			if (spawn_index_ >= BEAM2_MAX) {
				spawn_index_ = 0;
			}
			++cnt;
			if (cnt >= BEAM2_MAX) {
				Debug.LogError("EXCEED Beam2 POOL!");
				Debug.Assert(false);
				return -1;
			}
		}
		alive_table_[spawn_index_] = true;
		int id = spawn_index_;
		uv2_list_[id] = new Vector2(width, (float)type);
		return id;
	}

	public void renderUpdate(int front, int id, ref Vector3 head, ref Vector3 tail)
	{
		int idx = id * 6;
		vertices_[front][idx+0] = head;
		vertices_[front][idx+1] = head;
		vertices_[front][idx+2] = head;
		vertices_[front][idx+3] = head;
		vertices_[front][idx+4] = tail;
		vertices_[front][idx+5] = tail;

		var dx = tail.x - head.x;
		var dy = tail.y - head.y;
		var dz = tail.z - head.z;
		var len2 = dx*dx + dy*dy + dz*dz;
		float rlen;
		if (len2 <= 0f) {
			rlen = 1f;
			dx = 0f;
			dy = 1f;
			dz = 0f;
		} else {
			var len = Mathf.Sqrt(len2);
			rlen = 1f/len;
		}
		normals_[front][idx+0].x = dx*rlen;
		normals_[front][idx+0].y = dy*rlen;
		normals_[front][idx+0].z = dz*rlen;
		normals_[front][idx+1].x = dx*rlen;
		normals_[front][idx+1].y = dy*rlen;
		normals_[front][idx+1].z = dz*rlen;
		normals_[front][idx+2].x = dx*rlen;
		normals_[front][idx+2].y = dy*rlen;
		normals_[front][idx+2].z = dz*rlen;
		normals_[front][idx+3].x = dx*rlen;
		normals_[front][idx+3].y = dy*rlen;
		normals_[front][idx+3].z = dz*rlen;
		normals_[front][idx+4].x = dx*rlen;
		normals_[front][idx+4].y = dy*rlen;
		normals_[front][idx+4].z = dz*rlen;
		normals_[front][idx+5].x = dx*rlen;
		normals_[front][idx+5].y = dy*rlen;
		normals_[front][idx+5].z = dz*rlen;
		uv2s_[front][idx+0] = uv2_list_[id];
		uv2s_[front][idx+1] = uv2_list_[id];
		uv2s_[front][idx+2] = uv2_list_[id];
		uv2s_[front][idx+3] = uv2_list_[id];
		uv2s_[front][idx+4] = uv2_list_[id];
		uv2s_[front][idx+4].y += 1f;
		uv2s_[front][idx+5] = uv2_list_[id];
		uv2s_[front][idx+5].y += 1f;
	}
	
	public void destroy(int id)
	{
		alive_table_[id] = false;
	}
}

} // namespace UTJ {

/*
 * End of Beam2.cs
 */

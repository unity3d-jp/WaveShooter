/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class GeodegicDome : MonoBehaviour {

	Mesh mesh_;

	private Vector3[] normalize(Vector3[] vertices)
	{
		var result = new Vector3[vertices.Length];
		for (var i = 0; i < result.Length; ++i) {
			result[i] = vertices[i].normalized;
		}
		return result;
	}

	private int make_hash(int a, int b)
	{
		int a0 = a < b ? a : b;
		int b0 = a < b ? b : a;
		return (a0<<16)+b0;
	}

	private int proc_edge(int a, int b,
						  Dictionary<int, int> dict,
						  Vector3[] dst_vertices,
						  ref int append_idx)
	{
		Debug.Assert(a < append_idx);
		Debug.Assert(b < append_idx);
		int edge = -1;
		var hash = make_hash(a, b);
		if (dict.ContainsKey(hash)) {
			edge = dict[hash];
		} else {
			var v = (dst_vertices[a] + dst_vertices[b])*0.5f;
			dst_vertices[append_idx] = v;
			edge = append_idx;
			dict[hash] = append_idx;
			++append_idx;
		}
		return edge;
	}

	private void submesh(Vector3[] vertices,
						 int[] triangles,
						 out Vector3[] dst_vertices,
						 out int[] dst_triangles)
	{
		var edge_num = triangles.Length*2; // (face_num*4)*3/2
		var dict = new Dictionary<int, int>();
		dst_vertices = new Vector3[vertices.Length + edge_num];
		dst_triangles = new int[triangles.Length * 4];
		for (var i = 0; i < vertices.Length; ++i) {
			dst_vertices[i] = vertices[i];
		}
		var idx = 0;
		var append_idx = vertices.Length;
		for (var face = 0; face < triangles.Length/3; ++face) {
			var i0 = triangles[face*3+0];
			var i1 = triangles[face*3+1];
			var i2 = triangles[face*3+2];
			var i01 = proc_edge(i0, i1, dict, dst_vertices, ref append_idx);
			var i12 = proc_edge(i1, i2, dict, dst_vertices, ref append_idx);
			var i20 = proc_edge(i2, i0, dict, dst_vertices, ref append_idx);
			dst_triangles[idx*3+0] = i0;
			dst_triangles[idx*3+1] = i01;
			dst_triangles[idx*3+2] = i20;
			++idx;
			dst_triangles[idx*3+0] = i01;
			dst_triangles[idx*3+1] = i1;
			dst_triangles[idx*3+2] = i12;
			++idx;
			dst_triangles[idx*3+0] = i2;
			dst_triangles[idx*3+1] = i20;
			dst_triangles[idx*3+2] = i12;
			++idx;
			dst_triangles[idx*3+0] = i01;
			dst_triangles[idx*3+1] = i12;
			dst_triangles[idx*3+2] = i20;
			++idx;
		}
	}

	private void triming(Vector3[] vertices,
						 int[] triangles,
						 float limit_upper_y,
						 out Vector3[] dst_vertices,
						 out int[] dst_triangles)
	{
		var tmp_triangles = new List<int>();
		for (var face = 0; face < triangles.Length/3; ++face) {
			var i0 = triangles[face*3+0];
			var i1 = triangles[face*3+1];
			var i2 = triangles[face*3+2];
			int over_cnt = 0;
			if (vertices[i0].y <= limit_upper_y) {
				++over_cnt;
			}
			if (vertices[i1].y <= limit_upper_y) {
				++over_cnt;
			}
			if (vertices[i2].y <= limit_upper_y) {
				++over_cnt;
			}
			if (over_cnt < 3) {
				tmp_triangles.Add(i0);
				tmp_triangles.Add(i1);
				tmp_triangles.Add(i2);
			}
		}
		var cnt_list = new int[vertices.Length];
		System.Array.Clear(cnt_list, 0, cnt_list.Length);
		foreach (var idx in tmp_triangles) {
			++cnt_list[idx];
		}
		var tmp_vertices = new List<Vector3>();
		int slide_idx = 0;
		for (var i = 0; i < cnt_list.Length; ++i) {
			if (cnt_list[i] <= 0) {
				for (var j = 0; j < tmp_triangles.Count; ++j) {
					if (i-slide_idx == tmp_triangles[j]) {
						Debug.LogFormat("{0}:{1}:{2}", i-slide_idx, j, tmp_triangles[j]);
					}
					if (i-slide_idx < tmp_triangles[j]) {
						--tmp_triangles[j];
					}
				}
				++slide_idx;
			} else {
				var v = vertices[i];
				if (v.y < limit_upper_y) {
					v.y = limit_upper_y;
				}
				tmp_vertices.Add(v);
			}
		}
		dst_vertices = tmp_vertices.ToArray();
		dst_triangles = tmp_triangles.ToArray();
	}


	void Start()
	{
		const float G = 1.618033988749895f;
		var vertices = new Vector3[14] {
			new Vector3(  G,  0f,  1f),
			new Vector3(  G,  0f, -1f),
			new Vector3( -G,  0f,  1f),
			new Vector3( -G,  0f, -1f),
			new Vector3( 1f,   G,  0f),
			new Vector3(-1f,   G,  0f),
			new Vector3( 1f,  -G,  0f),
			new Vector3(-1f,  -G,  0f),
			new Vector3( 0f,  1f,   G),
			new Vector3( 0f, -1f,   G),
			new Vector3( 0f,  1f,  -G),
			new Vector3( 0f, -1f,  -G),
			new Vector3( 0f,  0f,   G), // extra
			new Vector3( 0f,  0f,  -G), // extra
		};
		var triangles = new int[24*3] {
			0, 4, 1,
			0, 1, 6,
			0, 8, 4,
			0, 6, 9,

			// 0, 9, 8,
			0, 9, 12,
			0, 12, 8,

			1, 11, 6,
			1, 4, 10,

			// 1, 10, 11,
			1, 10, 13,
			1, 13, 11,
			
			2, 3, 5,
			2, 5, 8,

			// 2, 8, 9,
			2, 12, 9,
			2, 8, 12,

			2, 9, 7,
			2, 7, 3,
			3, 10, 5,

			// 3, 11, 10,
			3, 13, 10,
			3, 11, 13,

			3, 7, 11,
			5, 4, 8,
			5, 10, 4,
			6, 7, 9,
			6, 11, 7,
		};

		// submesh(vertices, triangles, out vertices, out triangles);
		// submesh(vertices, triangles, out vertices, out triangles);
		// submesh(vertices, triangles, out vertices, out triangles);

		Vector3[] vertices_t;
		int[] triangles_t;
		triming(vertices, triangles, -0.2f /* limit_upper_y */, out vertices_t, out triangles_t);

		Vector3[] vertices_n = normalize(vertices_t);
		int[] triangles_n = triangles_t;

		mesh_ = new Mesh();
		mesh_.name = "skybox";
		mesh_.vertices = vertices_n;
		mesh_.triangles = triangles_n;
		mesh_.bounds = new Bounds(Vector3.zero, Vector3.one * 99999999);
		GetComponent<MeshFilter>().sharedMesh = mesh_;
	}
}

/*
 * End of GeodegicDome.cs
 */

/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

// #if UNITY_IPHONE || UNITY_ANDROID || UNITY_PS4 || UNITY_SWITCH || UNITY_EDITOR
// # define NOT_RECOMENDED_OPTIMIZATION
// #endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace UTJ {

public class WaterSplash
{
	// singleton
	static WaterSplash instance_;
	public static WaterSplash Instance { get { return instance_ ?? (instance_ = new WaterSplash()); } }

	const int WATERSPLASH_MAX = 512;
	const float f3 = 0.292893218813452f;
	const float f7 = 0.707106781186548f;

	private int[] alive_table_;
	private int spawn_index_;
	private Vector3[] positions_;
	private Vector3[] velocities_;
	private Vector2[] uv2_list_;

	private List<Vector3>[] vertices_;
	private List<Vector3>[] normals_;
	private List<Vector2>[] uvs_;
	private List<Vector2>[] uv2s_;
	private List<int>[] triangles_;

#if NOT_RECOMENDED_OPTIMIZATION
	private GCHandle gc_handle_alive_table_items_;
	private GCHandle gc_handle_positions_items_;
	private GCHandle gc_handle_velocities_items_;
	private GCHandle gc_handle_uv2_list_items_;
	private GCHandle[] gc_handle_vertices_items_;
	private GCHandle[] gc_handle_normals_items_;
	private GCHandle[] gc_handle_uv2s_items_;
	private FieldInfo field_info_vector3_size_;
	private FieldInfo field_info_vector2_size_;
	private FieldInfo field_info_int_size_;
	SetListSizeOperator set_list_size_;
#endif

	private Mesh mesh_;
	private Material material_;
	static readonly int material_CamUp = Shader.PropertyToID("_CamUp");
	static readonly int material_Gravity = Shader.PropertyToID("_Gravity");
	static readonly int material_CurrentTime = Shader.PropertyToID("_CurrentTime");
	static readonly int materlal_BaseColor = Shader.PropertyToID("_BaseColor");

	private Color current_base_color_;

	public Mesh getMesh() { return mesh_; }
	public Material getMaterial() { return material_; }
	public void SetBaseColor(ref Color col) { current_base_color_ = col; }
	public void SetGravity(float value) { material_.SetFloat(material_Gravity, value); }

	public void init(Material material, Texture reflection_texture)
	{
		alive_table_ = new int[WATERSPLASH_MAX];
		for (var i = 0; i < WATERSPLASH_MAX; ++i) {
			alive_table_[i] = 0;
		}
		spawn_index_ = 0;
		positions_ = new Vector3[WATERSPLASH_MAX];
		velocities_ = new Vector3[WATERSPLASH_MAX];
		for (var i = 0; i < velocities_.Length; ++i) {
			velocities_[i] = new Vector3(0f, 0f, 0f);
		}
		uv2_list_ = new Vector2[WATERSPLASH_MAX];

		vertices_ = new List<Vector3>[2] { new List<Vector3>(), new List<Vector3>(), };
		vertices_[0].Capacity = WATERSPLASH_MAX*8;
		vertices_[1].Capacity = WATERSPLASH_MAX*8;

		normals_ = new List<Vector3>[2] { new List<Vector3>(), new List<Vector3>(), };
		normals_[0].Capacity = WATERSPLASH_MAX*8;
		normals_[1].Capacity = WATERSPLASH_MAX*8;

		uvs_ = new List<Vector2>[2];
		uvs_[0] = new List<Vector2>();
		uvs_[0].Capacity = WATERSPLASH_MAX*8;
		uvs_[1] = new List<Vector2>();
		uvs_[1].Capacity = WATERSPLASH_MAX*8;
#if NOT_RECOMENDED_OPTIMIZATION
		for (var i = 0; i < WATERSPLASH_MAX; ++i) {
			for (var j = 0; j < 2; ++j) {
				uvs_[j].Add(new Vector2(f3, 0f));
				uvs_[j].Add(new Vector2(f7, 0f));
				uvs_[j].Add(new Vector2(0f, f3));
				uvs_[j].Add(new Vector2(1f, f3));
				uvs_[j].Add(new Vector2(0f, f7));
				uvs_[j].Add(new Vector2(1f, f7));
				uvs_[j].Add(new Vector2(f3, 1f));
				uvs_[j].Add(new Vector2(f7, 1f));
			}
		}
#endif

		uv2s_ = new List<Vector2>[2] { new List<Vector2>(), new List<Vector2>(), };
		uv2s_[0].Capacity = WATERSPLASH_MAX*8;
		uv2s_[1].Capacity = WATERSPLASH_MAX*8;

		triangles_ = new List<int>[2];
		triangles_[0] = new List<int>();
		triangles_[0].Capacity = WATERSPLASH_MAX*18;
		triangles_[1] = new List<int>();
		triangles_[1].Capacity = WATERSPLASH_MAX*18;
#if NOT_RECOMENDED_OPTIMIZATION
		for (var i = 0; i < WATERSPLASH_MAX; ++i) {
			for (var j = 0; j < 2; ++j) {
				triangles_[j].Add(i*8+0);
				triangles_[j].Add(i*8+1);
				triangles_[j].Add(i*8+3);
				triangles_[j].Add(i*8+2);
				triangles_[j].Add(i*8+0);
				triangles_[j].Add(i*8+3);
				triangles_[j].Add(i*8+2);
				triangles_[j].Add(i*8+3);
				triangles_[j].Add(i*8+5);
				triangles_[j].Add(i*8+2);
				triangles_[j].Add(i*8+5);
				triangles_[j].Add(i*8+4);
				triangles_[j].Add(i*8+4);
				triangles_[j].Add(i*8+5);
				triangles_[j].Add(i*8+7);
				triangles_[j].Add(i*8+4);
				triangles_[j].Add(i*8+7);
				triangles_[j].Add(i*8+6);
			}
		}
		field_info_vector2_size_ = uvs_[0].GetType().GetField("_size", BindingFlags.NonPublic | BindingFlags.Instance);
		field_info_vector2_size_.SetValue(uvs_[0], 0);
		field_info_vector2_size_.SetValue(uvs_[1], 0);
		field_info_int_size_ = triangles_[0].GetType().GetField("_size", BindingFlags.NonPublic | BindingFlags.Instance);
		field_info_int_size_.SetValue(triangles_[0], 0);
		field_info_int_size_.SetValue(triangles_[1], 0);

		gc_handle_alive_table_items_ = GCHandle.Alloc(alive_table_, GCHandleType.Pinned);
		gc_handle_positions_items_ = GCHandle.Alloc(positions_, GCHandleType.Pinned);
		gc_handle_velocities_items_ = GCHandle.Alloc(velocities_, GCHandleType.Pinned);
		gc_handle_uv2_list_items_ = GCHandle.Alloc(uv2_list_, GCHandleType.Pinned);

		FieldInfo fiv = vertices_[0].GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
		gc_handle_vertices_items_ = new GCHandle[2];
		gc_handle_vertices_items_[0] = GCHandle.Alloc(fiv.GetValue(vertices_[0]), GCHandleType.Pinned);
		gc_handle_vertices_items_[1] = GCHandle.Alloc(fiv.GetValue(vertices_[1]), GCHandleType.Pinned);
		field_info_vector3_size_ = vertices_[0].GetType().GetField("_size", BindingFlags.NonPublic | BindingFlags.Instance);
		FieldInfo fin = normals_[0].GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
		gc_handle_normals_items_ = new GCHandle[2];
		gc_handle_normals_items_[0] = GCHandle.Alloc(fin.GetValue(normals_[0]), GCHandleType.Pinned);
		gc_handle_normals_items_[1] = GCHandle.Alloc(fin.GetValue(normals_[1]), GCHandleType.Pinned);
		FieldInfo fiu = uv2s_[0].GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
		gc_handle_uv2s_items_ = new GCHandle[2];
		gc_handle_uv2s_items_[0] = GCHandle.Alloc(fiu.GetValue(uv2s_[0]), GCHandleType.Pinned);;
		gc_handle_uv2s_items_[1] = GCHandle.Alloc(fiu.GetValue(uv2s_[1]), GCHandleType.Pinned);;
		set_list_size_ = new SetListSizeOperator(field_info_int_size_);
#endif

		mesh_ = new Mesh();
		mesh_.MarkDynamic();
		mesh_.name = "watersplash";
		mesh_.SetVertices(vertices_[0]);
		mesh_.SetNormals(normals_[0]);
		mesh_.SetUVs(0, uvs_[0]);
		mesh_.SetTriangles(triangles_[0], 0 /* submesh */, false /* calculateBounds */);
		mesh_.SetUVs(1, uv2s_[0]);
		mesh_.bounds = new Bounds(Vector3.zero, Vector3.one * 99999999);
		material_ = material;
		material_.SetFloat(material_Gravity, 9.8f);
		// material_.SetTexture("_ReflectionTex", reflection_texture);
	}

	public void restart()
	{
		for (var i = 0; i < alive_table_.Length; ++i) {
			alive_table_[i] = 0;
		}
	}

	public void update(double update_time)
	{
		for (var i = 0; i < WATERSPLASH_MAX; ++i) {
			if (alive_table_[i] != 0) {
				if (update_time - uv2_list_[i].x > MyRandom.Range(0.9f, 1.5f)) {
					destroy(i);
				}
			}
		}
	}

	public void renderUpdate(int front)
	{
#if NOT_RECOMENDED_OPTIMIZATION
		// var sw = new System.Diagnostics.Stopwatch();
		// sw.Start();
		int num = 
			UnityPluginIF.ConvertArrayFromList(WATERSPLASH_MAX,
											   gc_handle_alive_table_items_.AddrOfPinnedObject(),
											   gc_handle_positions_items_.AddrOfPinnedObject(),
											   gc_handle_velocities_items_.AddrOfPinnedObject(),
											   gc_handle_uv2_list_items_.AddrOfPinnedObject(),
											   gc_handle_vertices_items_[front].AddrOfPinnedObject(),
											   gc_handle_normals_items_[front].AddrOfPinnedObject(),
											   gc_handle_uv2s_items_[front].AddrOfPinnedObject());
		object boxed_num8 = BoxingPool.IntList[num*8];
		object boxed_num18 = BoxingPool.IntList[num*18];
		set_list_size_.invoke(field_info_vector3_size_, vertices_[front], boxed_num8);
		set_list_size_.invoke(field_info_vector3_size_, normals_[front], boxed_num8);
		set_list_size_.invoke(field_info_vector2_size_, uv2s_[front], boxed_num8);
		set_list_size_.invoke(field_info_int_size_, triangles_[front], boxed_num18);
		set_list_size_.invoke(field_info_vector2_size_, uvs_[front], boxed_num8);
		// sw.Stop();
		// var plugin_time = sw.ElapsedTicks;

		// sw.Start();
		// int num1 = 
		// 	UnityPluginIF.ConvertArrayFromList(WATERSPLASH_MAX,
		// 									   gc_handle_alive_table_items_.AddrOfPinnedObject(),
		// 									   gc_handle_positions_items_.AddrOfPinnedObject(),
		// 									   gc_handle_velocities_items_.AddrOfPinnedObject(),
		// 									   gc_handle_uv2_list_items_.AddrOfPinnedObject(),
		// 									   gc_handle_vertices_items_[front].AddrOfPinnedObject(),
		// 									   gc_handle_normals_items_[front].AddrOfPinnedObject(),
		// 									   gc_handle_uv2s_items_[front].AddrOfPinnedObject());
		// object boxed_num40 = BoxingPool.IntList[num1*4];
		// object boxed_num60 = BoxingPool.IntList[num1*6];
		// set_list_size_.invoke_slower(field_info_vector3_size_, vertices_[front], boxed_num40);
		// set_list_size_.invoke_slower(field_info_vector3_size_, normals_[front], boxed_num40);
		// set_list_size_.invoke_slower(field_info_vector2_size_, uv2s_[front], boxed_num40);
		// set_list_size_.invoke_slower(field_info_int_size_, triangles_[front], boxed_num60);
		// set_list_size_.invoke_slower(field_info_vector2_size_, uvs_[front], boxed_num40);
		// sw.Stop();
		// var plugin1_time = sw.ElapsedTicks;

		// sw.Start();
		// int num2 = 
		// 	UnityPluginIF.ConvertArrayFromList(WATERSPLASH_MAX,
		// 									   gc_handle_alive_table_items_.AddrOfPinnedObject(),
		// 									   gc_handle_positions_items_.AddrOfPinnedObject(),
		// 									   gc_handle_velocities_items_.AddrOfPinnedObject(),
		// 									   gc_handle_uv2_list_items_.AddrOfPinnedObject(),
		// 									   gc_handle_vertices_items_[front].AddrOfPinnedObject(),
		// 									   gc_handle_normals_items_[front].AddrOfPinnedObject(),
		// 									   gc_handle_uv2s_items_[front].AddrOfPinnedObject());
		// field_info_vector3_size_.SetValue(vertices_[front], num2*4);
		// field_info_vector3_size_.SetValue(normals_[front], num2*4);
		// field_info_vector2_size_.SetValue(uv2s_[front], num2*4);
		// field_info_int_size_.SetValue(triangles_[front], num2*6);
		// field_info_vector2_size_.SetValue(uvs_[front], num2*4);
		// sw.Stop();
		// var plugin2_time = sw.ElapsedTicks;

		// sw.Start();
		// vertices_[front].Clear();
		// normals_[front].Clear();
		// uv2s_[front].Clear();
		// int idx = 0;
		// for (var i = 0; i < WATERSPLASH_MAX; ++i) {
		// 	if (alive_table_[i] != 0) {
		// 		for (var j = 0; j < 4; ++j) {
		// 			vertices_[front].Add(positions_[i]);
		// 			normals_[front].Add(velocities_[i]);
		// 			uv2s_[front].Add(uv2_list_[i]);
		// 		}
		// 		++idx;
		// 	}
		// }
		// field_info_int_size_.SetValue(triangles_[front], idx*6);
		// field_info_vector2_size_.SetValue(uvs_[front], idx*4);
		// sw.Stop();
		// var mono1_time = sw.ElapsedTicks;

		// sw.Start();
		// vertices_[front].Clear();
		// normals_[front].Clear();
		// uv2s_[front].Clear();
		// triangles_[front].Clear();
		// uvs_[front].Clear();
		// idx = 0;
		// for (var i = 0; i < WATERSPLASH_MAX; ++i) {
		// 	if (alive_table_[i] != 0) {
		// 		for (var j = 0; j < 4; ++j) {
		// 			vertices_[front].Add(positions_[i]);
		// 			normals_[front].Add(velocities_[i]);
		// 			uv2s_[front].Add(uv2_list_[i]);
		// 		}					
		// 		triangles_[front].Add(idx*4+0);
		// 		triangles_[front].Add(idx*4+1);
		// 		triangles_[front].Add(idx*4+2);
		// 		triangles_[front].Add(idx*4+2);
		// 		triangles_[front].Add(idx*4+1);
		// 		triangles_[front].Add(idx*4+3);

		// 		uvs_[front].Add(new Vector2(0f, 0f));
		// 		uvs_[front].Add(new Vector2(1f, 0f));
		// 		uvs_[front].Add(new Vector2(0f, 1f));
		// 		uvs_[front].Add(new Vector2(1f, 1f));

		// 		++idx;
		// 	}
		// }
		// sw.Stop();
		// var mono2_time = sw.ElapsedTicks;

		// PerformanceMeter.Instance.setValue(0, (int)plugin_time);
		// PerformanceMeter.Instance.setValue(1, (int)plugin1_time);
		// PerformanceMeter.Instance.setValue(2, (int)plugin2_time);
#else
		vertices_[front].Clear();
		normals_[front].Clear();
		uv2s_[front].Clear();
		triangles_[front].Clear();
		uvs_[front].Clear();
		int idx = 0;
		for (var i = 0; i < WATERSPLASH_MAX; ++i) {
			if (alive_table_[i] != 0) {
				for (var j = 0; j < 8; ++j) {
					vertices_[front].Add(positions_[i]);
					normals_[front].Add(velocities_[i]);
					uv2s_[front].Add(uv2_list_[i]);
				}					
				triangles_[front].Add(idx*8+0);
				triangles_[front].Add(idx*8+1);
				triangles_[front].Add(idx*8+3);
				triangles_[front].Add(idx*8+2);
				triangles_[front].Add(idx*8+0);
				triangles_[front].Add(idx*8+3);
				triangles_[front].Add(idx*8+2);
				triangles_[front].Add(idx*8+3);
				triangles_[front].Add(idx*8+5);
				triangles_[front].Add(idx*8+2);
				triangles_[front].Add(idx*8+5);
				triangles_[front].Add(idx*8+4);
				triangles_[front].Add(idx*8+4);
				triangles_[front].Add(idx*8+5);
				triangles_[front].Add(idx*8+7);
				triangles_[front].Add(idx*8+4);
				triangles_[front].Add(idx*8+7);
				triangles_[front].Add(idx*8+6);
				uvs_[front].Add(new Vector2(f3, 0f));
				uvs_[front].Add(new Vector2(f7, 0f));
				uvs_[front].Add(new Vector2(0f, f3));
				uvs_[front].Add(new Vector2(1f, f3));
				uvs_[front].Add(new Vector2(0f, f7));
				uvs_[front].Add(new Vector2(1f, f7));
				uvs_[front].Add(new Vector2(f3, 1f));
				uvs_[front].Add(new Vector2(f7, 1f));

				++idx;
			}
		}
		// vertices_[front].Clear();
		// normals_[front].Clear();
		// uv2s_[front].Clear();
		// int idx = 0;
		// for (var i = 0; i < WATERSPLASH_MAX; ++i) {
		// 	if (alive_table_[i] != 0) {
		// 		for (var j = 0; j < 4; ++j) {
		// 			vertices_[front].Add(positions_[i]);
		// 			normals_[front].Add(velocities_[i]);
		// 			uv2s_[front].Add(uv2_list_[i]);
		// 		}
		// 		++idx;
		// 	}
		// }
		// field_info_triangles_size_.SetValue(triangles_[front], idx*6);
		// field_info_uvs_size_.SetValue(uvs_[front], idx*4);
#endif
	}

	public void render(int front, Camera camera, double render_time)
	{
		mesh_.triangles = CV.IntArrayEmpty;
		mesh_.vertices = CV.Vector3ArrayEmpty;
		mesh_.normals = CV.Vector3ArrayEmpty;
		mesh_.uv = CV.Vector2ArrayEmpty;
		mesh_.uv2 = CV.Vector2ArrayEmpty;
		mesh_.SetVertices(vertices_[front]);
		mesh_.SetNormals(normals_[front]);
		mesh_.SetUVs(0, uvs_[front]);
		mesh_.SetUVs(1, uv2s_[front]);
		mesh_.SetTriangles(triangles_[front], 0 /* submesh */, false /* calculateBounds */);
		mesh_.bounds = new Bounds(Vector3.zero, Vector3.one * 99999999); // somehow necessary..
		material_.SetVector(material_CamUp, camera.transform.up);
		material_.SetFloat(material_CurrentTime, (float)render_time);
		material_.SetColor(materlal_BaseColor, current_base_color_);
	}

	public void spawn(ref Vector3 pos, ref Vector3 velocity, double update_time)
	{
		if (velocity.y <= 0f && pos.y < 0f) {
			return;
		}
		int cnt = 0;
		while (alive_table_[spawn_index_] != 0) {
			++spawn_index_;
			if (spawn_index_ >= WATERSPLASH_MAX) {
				spawn_index_ = 0;
			}
			++cnt;
			if (cnt >= WATERSPLASH_MAX) {
				Debug.LogError("EXCEED WaterSplash POOL!");
				Debug.Assert(false);
				return;
			}
		}
		alive_table_[spawn_index_] = 1;
		int id = spawn_index_;
		var rot = MyRandom.Range(0, Mathf.PI*2f);
		positions_[id] = pos;
		velocities_[id] = velocity;
		uv2_list_[id] = new Vector2((float)update_time, rot);
	}

	public void destroy(int id)
	{
		alive_table_[id] = 0;
	}
}

} // namespace UTJ {

/*
 * End of WaterSplash.cs
 */

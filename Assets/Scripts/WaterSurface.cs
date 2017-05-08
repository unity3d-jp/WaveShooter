/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UTJ {

public class WaterSurface {
	static WaterSurface instance_;
	public static WaterSurface Instance { get { return instance_ ?? (instance_ = new WaterSurface()); } }

	private const int X_NUM = 128;
	private const int Y_NUM = 128;
	private const int VERTICES_NUM = X_NUM*Y_NUM;
	private const int RECT_NUM = (X_NUM - 1) * (Y_NUM - 1);
	private const int TRIS_NUM = RECT_NUM * 2 * 3;
	private const float SCALE = 100f;

    private Vector3[] vertices_list_;

	private Mesh mesh_;
	public Mesh getMesh() { return mesh_; }
	public float getScale() { return SCALE; }

	private WaterInputDrawer water_input_drawer_;

	private float create_vertex(int xi, int yi, ref Vector3 vertex, float prev_y, bool distortion)
	{
		float new_y = 0f;
		float x = (float)xi;
		float y = (float)yi;
		if (distortion) {
			x = x/(X_NUM-1)*2f - 1f;
			y = y/(Y_NUM-1);
			
			if (y < 0.005f) {
				new_y = prev_y+0.1f;
			} else if (y < 0.8f) {
				new_y = prev_y+0.004f;
			} else if (y < 0.9f) {
				new_y = prev_y+0.025f;
			} else {
				new_y = prev_y+0.15f;
			}
			
			x = x*(new_y-0.1f)*2.5f;
			
			x = (x+1f)*(X_NUM-1)*0.5f;
			y = (new_y-0.1f)*(Y_NUM-1);
		}
		vertex.x = x * (SCALE/(X_NUM-1)) - SCALE*0.5f;
		vertex.y = 0f;
		vertex.z = y * (SCALE/(Y_NUM-1)) - SCALE*0.5f;

		return new_y;
	}

	public void init(Material water_input_material, bool distortion, bool line_render)
	{
		vertices_list_ = new Vector3[VERTICES_NUM];
		float prev_y = 0f;
		for (var y = 0; y < Y_NUM; ++y) {
			float last_y = 0f;
			for (var x = 0; x < X_NUM; ++x) {
				last_y = create_vertex(x, y, ref vertices_list_[x+y*X_NUM], prev_y, distortion);
			}
			prev_y = last_y;
		}
		var triangles = new int[TRIS_NUM];
		{
			var i = 0;
			for (var y = 0; y < Y_NUM-1; ++y) {
				for (var x = 0; x < X_NUM-1; ++x) {
					triangles[i] = (y + 0) * (X_NUM) + x + 0; ++i;
					triangles[i] = (y + 1) * (X_NUM) + x + 0; ++i;
					triangles[i] = (y + 0) * (X_NUM) + x + 1; ++i;
					triangles[i] = (y + 1) * (X_NUM) + x + 0; ++i;
					triangles[i] = (y + 1) * (X_NUM) + x + 1; ++i;
					triangles[i] = (y + 0) * (X_NUM) + x + 1; ++i;
				}
			}
		}
		var uvs_list = new Vector2[VERTICES_NUM];
		for (var y = 0; y < Y_NUM; ++y) {
			for (var x = 0; x < X_NUM; ++x) {
				uvs_list[x+y*X_NUM].x = 1.0f/(X_NUM-1)*x;
				uvs_list[x+y*X_NUM].y = 1.0f/(Y_NUM-1)*y;
			}
		}

		mesh_ = new Mesh();
		mesh_.name = "WaterSurface";
		mesh_.vertices = vertices_list_;
		mesh_.uv = uvs_list;
		mesh_.triangles = triangles;

		// line mesh rendering for debug.
		if (line_render) {
			int[] indices = new int[2 * triangles.Length];
			int i = 0;
			for (int t = 0; t < triangles.Length; t += 3) {
				indices[i++] = triangles[t+0];
				indices[i++] = triangles[t+1];
				indices[i++] = triangles[t+1];
				indices[i++] = triangles[t+2];
				indices[i++] = triangles[t+2];
				indices[i++] = triangles[t+0];
			}
			mesh_.SetIndices(indices, MeshTopology.Lines, 0);
		}
		mesh_.RecalculateBounds();

		water_input_drawer_ = new WaterInputDrawer();
		water_input_drawer_.init(water_input_material, SCALE);
	}
	
	public void finalize()
	{
		water_input_drawer_.finalize();
	}

	public RenderTexture getWaterInput()
	{
		return water_input_drawer_.getRenderTexture();
	}

	public void makeBump(ref Vector3 pos, float value, float size)
	{
		makeBump(pos.x, pos.z, value, size);
	}

	public void makeBump(ref Vector2 pos, float value, float size)
	{
		makeBump(pos.x, pos.y, value, size);
	}

	public void makeBump(float x, float z, float value, float size)
	{
		water_input_drawer_.putPoint(x, z, value, size);
	}

	public void renderUpdate(int front)
	{
		water_input_drawer_.renderUpdate(front);
	}

	public void render(int front)
	{
		water_input_drawer_.render(front);
	}
}

} // namespace UTJ {

/*
 * End of WaterSurface.cs
 */

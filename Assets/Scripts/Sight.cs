/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

public class Sight
{
	// singleton
	static Sight instance_;
	public static Sight Instance { get { return instance_ ?? (instance_ = new Sight()); } }

	const int SIGHT_MAX = 16;

	private int regist_index_;
	private Vector3[] positions_;
	private Vector3[][] vertices_;
	private Mesh mesh_;
	private Material material_;

	private int phase_;
	private int cnt_;
	private int cnt1_;

	public void init(Material material)
	{
		regist_index_ = 0;
		positions_ = new Vector3[SIGHT_MAX];
		vertices_ = new Vector3[2][] { new Vector3[SIGHT_MAX*6], new Vector3[SIGHT_MAX*6], };
		var indices = new int[SIGHT_MAX * 12];
		for (var i = 0; i < SIGHT_MAX; ++i) {
			indices[i*12+ 0] = i*6+0;
			indices[i*12+ 1] = i*6+1;
			indices[i*12+ 2] = i*6+1;
			indices[i*12+ 3] = i*6+2;
			indices[i*12+ 4] = i*6+2;
			indices[i*12+ 5] = i*6+3;
			indices[i*12+ 6] = i*6+3;
			indices[i*12+ 7] = i*6+0;
			indices[i*12+ 8] = i*6+3;
			indices[i*12+ 9] = i*6+4;
			indices[i*12+10] = i*6+4;
			indices[i*12+11] = i*6+5;
		}
		mesh_ = new Mesh();
		mesh_.MarkDynamic();
		mesh_.name = "sight";
		mesh_.vertices = vertices_[0];
		mesh_.SetIndices(indices, MeshTopology.Lines, 0);
		mesh_.bounds = new Bounds(Vector3.zero, Vector3.one * 99999999);
		material_ = material;

		phase_ = 0;
		cnt_ = 0;
		cnt1_ = 0;
	}

	public void begin(int front)
	{
		regist_index_ = 0;
		for (var i = 0; i < SIGHT_MAX*6; ++i) {
			vertices_[front][i] = CV.Vector3Zero;
		}
	}

	public void regist(int front, ref Vector3 position)
	{
		if (regist_index_ >= SIGHT_MAX) {
			return;
		}
		positions_[regist_index_] = position;
		++regist_index_;
	}

	public void end(int front, CameraBase camera)
	{
		const float SIZE = 16f;
		float size = SIZE;
		switch (phase_) {
			case 0:
				cnt_ = MyRandom.Range(300, 400);
				cnt1_ = 0;
				++phase_;
				break;
			case 1:
				--cnt_;
				if (cnt_ < 0) {
					cnt_ = 16;
					size = 0f;
					++phase_;
				}
				break;
			case 2:
				size = (float)(16 - cnt_)/16f * SIZE;
				--cnt_;
				if (cnt_ < 0) {
					cnt_ = 12;
					++phase_;
				}
				break;
			case 3:
				--cnt_;
				if (cnt_ < 0) {
					cnt_ = 4;
					++phase_;
				}
				break;
			case 4:
				--cnt_;
				if (cnt_ < 0) {
					cnt_ = 12;
					++cnt1_;
					if (cnt1_ > 8) {
						phase_ = 0;
					} else {
						--phase_;
					}
				}
				break;
		}

		if (phase_ != 2 && phase_ != 3) {
			return;
		}

		for (var i = 0; i < regist_index_; ++i) {
			var spos = (camera != null ?
						camera.getScreenPoint(ref positions_[i]) :
						positions_[i]);
			float size0 = spos.z > 1f ? size : 0f;
			vertices_[front][i*6+0].x = spos.x + size0;
			vertices_[front][i*6+0].y = spos.y;
			vertices_[front][i*6+1].x = spos.x;
			vertices_[front][i*6+1].y = spos.y + size0;
			vertices_[front][i*6+2].x = spos.x - size0;
			vertices_[front][i*6+2].y = spos.y;
			vertices_[front][i*6+3].x = spos.x;
			vertices_[front][i*6+3].y = spos.y - size0;
			vertices_[front][i*6+4].x = spos.x + size0*2f;
			vertices_[front][i*6+4].y = spos.y - size0*3f;
			vertices_[front][i*6+5].x = spos.x + size0*4f;
			vertices_[front][i*6+5].y = spos.y - size0*3f;
		}
	}

	public void render(int front)
	{
		mesh_.vertices = vertices_[front];
	}

	public Mesh getMesh() { return mesh_; }
	public Material getMaterial() { return material_; }

}

} // namespace UTJ {

/*
 * End of Sight.cs
 */

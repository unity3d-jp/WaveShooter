/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

public class GaugeJump : Task {
	
	private float value_;

	public static void Create()
	{
		var task = new GaugeJump();
		task.init();
	}

	public override void update(float dt, double update_time)
	{
		value_ = Player.Instance.TameGaugeValue();
	}

	public override void renderUpdate(int front, CameraBase camra, ref DrawBuffer draw_buffer)
	{
		var step = 32f;
		var ratio = 1f;
		var x = 80f;
		int v = (int)(value_*8f);
		for (var i = 0; i < 8; ++i) {
			MySprite.Kind kind;
			float y;
			if (i < 4) {
				kind = v <= i ? MySprite.Kind.GaugeB_S : MySprite.Kind.GaugeR_S;
				step = 16f;
				y = -290f;
			} else if (i < 7) {
				kind = v <= i ? MySprite.Kind.GaugeB_M : MySprite.Kind.GaugeR_M;
				step = 24f;
				y = -288f;
			} else {
				kind = v <= i ? MySprite.Kind.GaugeB_L : MySprite.Kind.GaugeR_L;
				step = 34f;
				y = -280f;
			}
			x += step;
			MySprite.Instance.put(front,-x, y, ratio, kind, MySprite.Type.Full);
			MySprite.Instance.put(front, x, y, ratio, kind, MySprite.Type.Full, true /* reverse */);
		}
	}
}

} // namespace UTJ {

/*
 * End of GaugeJump.cs
 */

/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */
using UnityEngine;

namespace UTJ {

public class PerformanceMeter
{
	// singleton
	static PerformanceMeter instance_;
	public static PerformanceMeter Instance { get { return instance_ ?? (instance_ = new PerformanceMeter()); } }

	private System.Diagnostics.Stopwatch stopwatch_;
	private const float FRAME_PERIOD = 1f/59.94f;
	private float fps_;
	private float display_fps_;

	private long update_start_tick_;
	private long update_tick_;

	private long render_update_start_tick_;
	private long render_update_tick_;

	private long render_start_tick_;
	private long render_tick_;

	private long behaviour_update_start_tick_;
	private long behaviour_update_tick_;

#if !UNITY_EDITOR && (UNITY_PS4 || UNITY_PSP2 || UNITY_SWITCH)
	private long console_render_start_tick_;
	private long console_render_tick_;
#endif

	private int gc_start_count_;
	private int frame_count_;
	private bool recording_;

	private bool compare_ = false;
	private int[] values_;
	private MyFont.Type[] types_;

	public void init()
	{
		gc_start_count_ = System.GC.CollectionCount(0 /* generation */);
		stopwatch_ = new System.Diagnostics.Stopwatch();
		stopwatch_.Start();
		frame_count_ = 0;
		recording_ = false;
		values_ = new int[3];
		types_ = new MyFont.Type[3];
	}

	public void setRecording() { recording_ = true; }

	public void setValue(int idx, int value)
	{
		values_[idx] = value;
		compare_ = true;
	}

	public void beginUpdate()
	{
		update_start_tick_ = stopwatch_.ElapsedTicks;
	}
	public void endUpdate()
	{
		update_tick_ = stopwatch_.ElapsedTicks - update_start_tick_;
	}

	public void beginRenderUpdate()
	{
		render_update_start_tick_ = stopwatch_.ElapsedTicks;
	}
	public void endRenderUpdate()
	{
		render_update_tick_ = stopwatch_.ElapsedTicks - render_update_start_tick_;
	}

	public void beginBehaviourUpdate()
	{
		behaviour_update_start_tick_ = stopwatch_.ElapsedTicks;
	}
	public void endBehaviourUpdate()
	{
		behaviour_update_tick_ = stopwatch_.ElapsedTicks - behaviour_update_start_tick_;
		++frame_count_;
	}

	public void beginConsoleRender()
	{
#if !UNITY_EDITOR && (UNITY_PS4 || UNITY_PSP2 || UNITY_SWITCH)
		console_render_start_tick_ = stopwatch_.ElapsedTicks;
#endif
	}
	public void endConsoleRender()
	{
#if !UNITY_EDITOR && (UNITY_PS4 || UNITY_PSP2 || UNITY_SWITCH)
		console_render_tick_ = stopwatch_.ElapsedTicks - console_render_start_tick_;
#endif
	}

	public void beginRender()
	{
		long period = stopwatch_.ElapsedTicks - render_start_tick_;
		fps_ = (float)((double)System.Diagnostics.Stopwatch.Frequency / (double)period);
		display_fps_ = Mathf.Lerp(display_fps_, fps_, 0.05f);
		render_start_tick_ = stopwatch_.ElapsedTicks;
	}
	public void endRender()
	{
		render_tick_ = stopwatch_.ElapsedTicks - render_start_tick_;
	}

	public bool wasSlowLoop() {
		// return render_tick_/(float)System.Diagnostics.Stopwatch.Frequency > (1f/50f);
		return !recording_ && fps_ < 50f;
	}

	public float getDisplayFPS() { return display_fps_; }
	public float getFPS() { return fps_; }

	
	public void drawMeters(int front, bool multi_threading)
	{
		if (recording_) {
			return;
		}
#if UNITY_EDITOR		
		return;
#endif
#pragma warning disable 162
		const int bar_x = -560;
		const int bar_y = 300;
		const int width = 560;
		const int height = 4;
		float frame_tick = FRAME_PERIOD * (float)System.Diagnostics.Stopwatch.Frequency;

		int x = bar_x;
		int y = bar_y;
		int w;
		int h = height;

		if (multi_threading) {
			MySprite.Instance.put(front, x, y, width, height, MySprite.Kind.Square, MySprite.Type.Black);
			w = (int)((float)update_tick_/frame_tick * (float)width);
			MySprite.Instance.put(front, x, y, w, h, MySprite.Kind.Square, MySprite.Type.Blue);
			x += w;
			w = (int)((float)render_update_tick_/frame_tick * (float)width);
			MySprite.Instance.put(front, x, y, w, h, MySprite.Kind.Square, MySprite.Type.Green);
			x = bar_x;
			y -= 8;
		}

#if !UNITY_EDITOR && (UNITY_PS4 || UNITY_PSP2 || UNITY_SWITCH)
		MySprite.Instance.put(front, x, y, width, height, MySprite.Kind.Square, MySprite.Type.Black);
		w = (int)((float)console_render_tick_/frame_tick * (float)width);
		MySprite.Instance.put(front, x, y, w, h, MySprite.Kind.Square, MySprite.Type.Red);
		x = bar_x;
		y -= 8;
#else
		MySprite.Instance.put(front, x, y, width, height, MySprite.Kind.Square, MySprite.Type.Black);
		w = (int)((float)render_tick_/frame_tick * (float)width);
		MySprite.Instance.put(front, x, y, w, h, MySprite.Kind.Square, MySprite.Type.Red);
		w = (int)((float)behaviour_update_tick_/frame_tick * (float)width);
		MySprite.Instance.put(front, x, y, w, h, MySprite.Kind.Square, MySprite.Type.Yellow);
		x = bar_x;
		y -= 8;
#endif
		y += 32;
		int fps100 = (int)(display_fps_ * 100f);
		MyFont.Instance.putNumber(front, fps100, 5 /* keta */, 0.5f /* scale */,
								  x, y, MyFont.Type.White);
		int gc_count = System.GC.CollectionCount(0 /* generation */) - gc_start_count_;
		MyFont.Instance.putNumber(front, gc_count, 8 /* keta */, 0.5f /* scale */,
								  x+80, y, MyFont.Type.Yellow);

		MyFont.Instance.putNumber(front, frame_count_, 8 /* keta */, 0.5f /* scale */,
								  x+180, y, MyFont.Type.Green);

		if (compare_) {
			x = bar_x;
			y -= 8;
			types_[0] = types_[1] = types_[2] = MyFont.Type.White;
			if (values_[0] < values_[1] && values_[0] < values_[2]) {
				types_[0] = MyFont.Type.Blue;
			}
			if (values_[0] < values_[2] && values_[1] < values_[2]) {
				types_[2] = MyFont.Type.Red;
			}
			for (var i = 0; i < 3; ++i) {
				MyFont.Instance.putNumber(front, values_[i], 8 /* keta */, 0.5f /* scale */,
										  x + 100*i, y, types_[i]);
			}
		}
	}
}

} // namespace UTJ {

/*
 * End of PerformanceMeter.cs
 */

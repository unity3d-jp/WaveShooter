/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;

namespace UTJ {

public class Utility {

	public struct WaitForSeconds
	{
		private float period_;
		private double start_;
		public WaitForSeconds(float period, double update_time)
		{
			period_ = period;
			start_ = update_time;
		}
		public bool end(double update_time)
		{
			return update_time - start_ > period_;
		}
	}

	public static void MirrorX(ref Quaternion q)
	{
		q.y = -q.y;
		q.z = -q.z;
	}

	public static Quaternion Inverse(ref Quaternion q)
	{
		return new Quaternion(-q.x, -q.y, -q.z, q.w);
	}

	public static Color Lerp3FacttorUnclamped(ref Color a, ref Color b, float t)
	{
		return new Color(a.r + (b.r - a.r) * t, a.g + (b.g - a.g) * t, a.b + (b.b - a.b) * t, 1f);		
	}
}

} // namespace UTJ {

/*
 * End of Utility.cs
 */

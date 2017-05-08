/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

#if UNITY_EDITOR

using UnityEngine;

namespace UTJ {

public class MuscleMotionEditorUtil {

	public static MuscleMotion.Parts conv(Transform transform)
	{
		if (transform == null) {
			return MuscleMotion.Parts.Max;
		}
		try {
			var parts = (MuscleMotion.Parts)System.Enum.Parse(typeof(MuscleMotion.Parts), transform.name);
			return parts;
		} catch (System.ArgumentException) {
			return MuscleMotion.Parts.Max;
		}
	}

	public static MuscleMotion.Parts conv_reverse(Transform transform)
	{
		if (transform == null) {
			return MuscleMotion.Parts.Max;
		}
		try {
			var parts = (MuscleMotion.Parts)System.Enum.Parse(typeof(MuscleMotion.Parts), transform.name);
			return MuscleMotion.ReverseTable[(int)parts];
		} catch (System.ArgumentException) {
			return MuscleMotion.Parts.Max;
		}
	}

}

} // namespace UTJ {

#endif // UNITY_EDITOR
/*
 * End of MuscleMotionEditorUtil.cs
 */

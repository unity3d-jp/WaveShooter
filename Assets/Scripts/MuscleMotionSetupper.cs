/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;

namespace UTJ {

[RequireComponent(typeof(MuscleMotionRenderer))]
public class MuscleMotionSetupper : UnityEngine.MonoBehaviour
{
	private DrawBuffer draw_buffer_;
	private Posture posture_;
	private MuscleMotion muscle_motion_;
	private bool initialized_ = false;
	
	IEnumerator Start()
	{
		draw_buffer_.init();
		yield return FileUtil.preparePath("whole.dat");
		posture_ = JsonUtility.FromJson<Posture>(FileUtil.content);
		muscle_motion_ = new MuscleMotion();
		muscle_motion_.init(posture_, 40f /* damper */, 800f /* spring_ratio */);

		GetComponent<MuscleMotionRenderer>().init();
		initialized_ = true;
	}

	void Update()
	{
		if (!initialized_) {
			return;
		}
#if UNITY_EDITOR
		muscle_motion_.getRootNode().set(GetComponent<MuscleMotionRenderer>().getRootTransform());

		muscle_motion_.setTarget(posture_);
		muscle_motion_.set(GetComponent<MuscleMotionRenderer>().getRootTransform(), MuscleMotion.Parts.Root);
		muscle_motion_.update(1f/60f);
 		draw_buffer_.beginRender();
		muscle_motion_.renderUpdate(ref draw_buffer_, DrawBuffer.Type.MuscleMotionPlayer);
 		draw_buffer_.endRender();

		for (var i = 0; i < draw_buffer_.object_num_; ++i) {
			switch (draw_buffer_.object_buffer_[i].type_) {
				case DrawBuffer.Type.MuscleMotionPlayer:
					int parts = draw_buffer_.object_buffer_[i].versatile_data_;
					if (parts == (int)MuscleMotion.Parts.Root) {
						break;
					}
					GetComponent<MuscleMotionRenderer>().render(ref draw_buffer_.object_buffer_[i]);
					break;
			}
		}
#endif // UNITY_EDITOR
	}
}

} // namespace UTJ {

/*
 * End of MuscleMotionSetupper.cs
 */

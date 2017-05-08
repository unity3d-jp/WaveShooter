/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;

namespace UTJ {

public struct InputBuffer
{
	public const int INPUT_MAX = 8;
	public int[] buttons_;
	public bool[] touched_;
	public Vector2[] touched_position_;
}

public class InputManager
{
	// singleton
	static InputManager instance_;
	public static InputManager Instance { get { return instance_ ?? (instance_ = new InputManager()); } }

	public const int ONE = 4096;
	public const float INV_ONE = 1f/((float)ONE);

	public enum Button {
		Horizontal,
		Vertical,
		Left,
		Right,
		Jump,
	}

	public InputBuffer[] input_buffer_;
	public void init()
	{
		input_buffer_ = new InputBuffer[2];
		for (int i = 0; i < 2; ++i) {
			input_buffer_[i].buttons_ = new int[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, };
			input_buffer_[i].touched_ = new bool[2] { false, false };
			input_buffer_[i].touched_position_ = new Vector2[2] { Vector2.zero, Vector2.zero, };
		}
	}

	public int getButton(int front, Button button)
	{
		return input_buffer_[front].buttons_[(int)button];
	}
	public bool isButton(int front, Button button)
	{
		return input_buffer_[front].buttons_[(int)button] != 0;
	}
	public float getAnalog(int front, Button button)
	{
		return (float)(input_buffer_[front].buttons_[(int)button]) * INV_ONE;
	}
	public bool touched(int front, int index)
	{
		return input_buffer_[front].touched_[index];
	}
	public Vector2 getTouchedPosition(int front, int index)
	{
		return input_buffer_[front].touched_position_[index];
	}

	private void set_buttons(int front)
	{
		int[] buttons = input_buffer_[front].buttons_;
		buttons[(int)InputManager.Button.Horizontal] = (int)(Input.GetAxisRaw("Horizontal") * InputManager.ONE);
		buttons[(int)InputManager.Button.Vertical] = (int)(Input.GetAxisRaw("Vertical") * InputManager.ONE);
		buttons[(int)InputManager.Button.Left] = (Input.GetKey(KeyCode.Z) ? 1 : 0) | (Input.GetButton("Fire3") ? 1 : 0);
		buttons[(int)InputManager.Button.Right] = (Input.GetKey(KeyCode.X) ? 1 : 0) | (Input.GetButton("Fire2") ? 1 : 0);
		bool jump = false;
		if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow)) {
			jump = true;
		} else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) {
			jump = true;
		}
		buttons[(int)InputManager.Button.Jump] = (int)(jump ? 1 : 0);
	}
	private void set_touched(int front, bool touched, ref Vector2 pos, int index)
	{
		input_buffer_[front].touched_[index] = touched;
		input_buffer_[front].touched_position_[index] = pos;
	}

	public void update(int front)
	{
		set_buttons(front);

		bool clicked0 = false;
		bool clicked1 = false;
		var clicked_position0 = new Vector2(0f, 0f);
		var clicked_position1 = new Vector2(0f, 0f);
		if (Input.touchCount > 0) {
			clicked_position0 = Input.GetTouch(0).position;
			clicked0 = true;
			if (Input.touchCount > 1) {
				clicked_position1 = Input.GetTouch(1).position;
				clicked1 = true;
			}
		} else if (Input.GetMouseButton(0)) {
			clicked_position0 = Input.mousePosition;
			clicked0 = true;
		}
		clicked_position0.x -= Screen.width*0.5f;
		clicked_position0.y -= Screen.height*0.5f;
		clicked_position1.x -= Screen.width*0.5f;
		clicked_position1.y -= Screen.height*0.5f;
		set_touched(front, clicked0, ref clicked_position0, 0 /* index */);
		set_touched(front, clicked1, ref clicked_position1, 1 /* index */);
	}
}

} // namespace UTJ {

/*
 * End of InputManager.cs
 */

using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;

public class SystemInput
{
	//TODO: Add Keyboard input (see bottom of script)
	
	//Keys
	private const int _VK_LBUTTON = 0x01; //Left Mouse Button
	private const int _VK_RBUTTON = 0x02; //Right Mouse Button
	private const int _VK_MBUTTON = 0x02; //Middle Mouse Button (Mouse wheel button)
	private const int _SM_SWAPBUTTON = 23; //0 = default, non-zero = LMB/RMB swapped

	//Key states
	private const int _BUTTONDOWNFRAME = -32767;
	private const int _BUTTONDOWN = -32768;
	private const int _BUTTONUP = 0; //Not sure if there's a specific buttonUp

	[DllImport("user32.dll", EntryPoint = "SetCursorPos")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool SetCursorPos(int x, int y);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool GetCursorPos(out Vector2Int lpMousePoint);	//Cursor coordinates start top-left, rather than Unity's bottom-left, so y axis will need to be modified

	[DllImport("user32.dll")]
	public static extern short GetAsyncKeyState(int virtualKeyCode);

	[DllImport("user32.dll")]
	public static extern short GetSystemMetrics(int metricsCode);

	//TODO: Work out a way to handle generic key states, so we don't need multiple bools for each key 
	private static bool _mouseButton0Down = false;
	private static bool _mouseButton1Down = false;
	private static bool _lastMouseButton0Down = false;
	private static bool _lastMouseButton1Down = false;
	private static bool _hasPressedButton0 = false;
	private static bool _hasPressedButton1 = false;

	/// <summary>
	///   <para>Returns whether the given mouse button is held down.</para>
	/// </summary>
	/// <param name="button"></param>
	public static bool GetMouseButton(int button = 0)
	{
		return (button == 0) ? _hasPressedButton0 : _hasPressedButton1;
	}

	/// <summary>
	///   <para>Returns true during the frame the user pressed the given mouse button.</para>
	/// </summary>
	/// <param name="button"></param>
	public static bool GetMouseButtonDown(int button = 0)
	{
		return (button == 0) ? _mouseButton0Down : _mouseButton1Down;
	}

	/// <summary>
	///   <para>Returns true during the frame the user releases the given mouse button.</para>
	/// </summary>
	/// <param name="button"></param>
	public static bool GetMouseButtonUp(int button = 0)
	{
		return (button == 0) ? (!_hasPressedButton0 && _lastMouseButton0Down) : (!_hasPressedButton1 && _lastMouseButton1Down);
	}
	
	public static Vector2Int GetCursorPosition()
	{
		GetCursorPos(out var __point);
		return __point;
	}

	public static void SetCursorPosition(Vector2Int point)
	{
		SetCursorPos(point.x, point.y);
	}

	public static void Process()
	{
		CheckMouseButtons();
	}

	private static void CheckMouseButtons()
	{
		_lastMouseButton0Down = _hasPressedButton0;
		_lastMouseButton1Down = _hasPressedButton1;
		_mouseButton0Down = false;
		_mouseButton1Down = false;

		var __mbp0 = MouseButtonPressed(0);
		var __mbp1 = MouseButtonPressed(1);

		//Check MouseButton0
		if (!_hasPressedButton0 && __mbp0)
		{
			_hasPressedButton0 = true;
			_mouseButton0Down = true;
		}
		else if (_hasPressedButton0 && !__mbp0)
		{
			_hasPressedButton0 = false;
		}

		//Check MouseButton1
		if (!_hasPressedButton1 && __mbp1)
		{
			_hasPressedButton1 = true;
			_mouseButton1Down = true;
		}
		else if (_hasPressedButton1 && !__mbp1)
		{
			_hasPressedButton1 = false;
		}
	}

	private static bool MouseButtonPressed(int button)
	{
		bool __state = false;
		bool __swapped = GetSystemMetrics(_SM_SWAPBUTTON) > 0;
		switch (button)
		{
			case 0:
				__state = GetAsyncKeyState(__swapped ? _VK_RBUTTON : _VK_LBUTTON) == _BUTTONDOWN;
				break;
			case 1:
				__state = GetAsyncKeyState(__swapped ? _VK_LBUTTON : _VK_RBUTTON) == _BUTTONDOWN;
				break;
			case 2:
				__state = GetAsyncKeyState(_VK_MBUTTON) == _BUTTONDOWN;
				break;
			default:
				return false;
		}

		return __state;
	}

	//TODO: Keyboard Input stuff
	public static bool GetKey(KeyCode key)
	{
		if (_vk_keyCodes.TryGetValue(key, out var __value))
		{
			return GetAsyncKeyState(__value) == _BUTTONDOWN;
		}

		return false;
	}

	private static bool KeyCodePressed(int value)
	{
		return GetAsyncKeyState(value) == _BUTTONDOWN;
	}

	//Is there an easier way than just adding each key combo manually?
	private static Dictionary<KeyCode, int> _vk_keyCodes = new Dictionary<KeyCode, int>()
	{
		{KeyCode.Keypad8, 0x68},
		{KeyCode.Keypad4, 0x64},
		{KeyCode.Keypad6, 0x66},
		{KeyCode.Keypad2, 0x62},
	};

	private static Dictionary<KeyCode, KeyState> _keyStates = new Dictionary<KeyCode, KeyState>();
	public struct KeyState
	{
		public KeyCode KeyCode;
		public KeyPressType KeyPressType;
	}

	public enum KeyPressType
	{
		None,
		Down,
		Hold,
		Up,
	}
}
﻿using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class TransparentWindow : MonoBehaviour
{
	public static TransparentWindow Main = null;
	public static Camera Camera = null;	//Used instead of Camera.main

	[Tooltip("What GameObject layers should trigger window focus when the mouse passes over objects?")] //
	[SerializeField]
	private LayerMask clickLayerMask = ~0;

	[Tooltip("Allows Input to be detected even when focus is lost")] //
	[SerializeField]
	private bool useSystemInput = false;

	[Tooltip("Should the window be fullscreen?")] //
	[SerializeField]
	private bool fullscreen = true;

	[Tooltip("Force the window to match ScreenResolution")] //
	[SerializeField]
	private bool customResolution = false;

	[Tooltip("Resolution the overlay should run at")] //
	[SerializeField]
	private Vector2Int screenResolution = new Vector2Int(1280, 720);

	[Tooltip("The framerate the overlay should try to run at")] //
	[SerializeField]
	private int targetFrameRate = 30;

	
	/////////////////////
	//Windows DLL stuff//
	/////////////////////
	
	[DllImport("user32.dll")]
	private static extern IntPtr GetActiveWindow();
	
	[DllImport("user32.dll")]
	private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

	[DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
	private static extern int SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, int dwFlags);

	[DllImport("user32.dll", EntryPoint = "GetWindowRect")]
	private static extern bool GetWindowRect(IntPtr hwnd, out Rectangle rect);
	
	[DllImport("user32.dll")]
	private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

	[DllImportAttribute("user32.dll")]
	private static extern bool ReleaseCapture();

	[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
	private static extern int SetWindowPos(IntPtr hwnd, int hwndInsertAfter, int x, int y, int cx, int cy, int uFlags);

	[DllImport("Dwmapi.dll")]
	private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Rectangle margins);

	private const int GWL_STYLE = -16;
	private const uint WS_POPUP = 0x80000000;
	private const uint WS_VISIBLE = 0x10000000;
	private const int HWND_TOPMOST = -1;

	private const int WM_SYSCOMMAND = 0x112;
	private const int WM_MOUSE_MOVE = 0xF012;

	private int fWidth;
	private int fHeight;
	private IntPtr hwnd = IntPtr.Zero;
	private Rectangle margins;
	private Rectangle windowRect;

	//BUG: Sometimes fails to SetResolution if not focused on startup - if using Start(), WindowBoundsCollider2D sometimes fails to set the correct size
	private void Awake()
	{
		Main = this;

		Camera = GetComponent<Camera>();
		Camera.backgroundColor = new Color();
		Camera.clearFlags = CameraClearFlags.SolidColor;

		if (fullscreen && !customResolution)
		{
			screenResolution = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);
		}
		
		Screen.SetResolution(screenResolution.x, screenResolution.y, fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);

		Application.targetFrameRate = targetFrameRate;
		Application.runInBackground = true;

#if !UNITY_EDITOR
		fWidth = screenResolution.x;
		fHeight = screenResolution.y;
		margins = new Rectangle() {Left = -1};
		hwnd = GetActiveWindow();

		if (GetWindowRect(hwnd, out windowRect))
		{
			Debug.LogError("Couldn't get Window Rect");
		}

		SetWindowLong(hwnd, GWL_STYLE, WS_POPUP | WS_VISIBLE);
		SetWindowPos(hwnd, HWND_TOPMOST, windowRect.Left, windowRect.Top, fWidth, fHeight, 32 | 64);
		DwmExtendFrameIntoClientArea(hwnd, ref margins);
#endif
	}

	private void Update()
	{
		if (useSystemInput)
		{
			SystemInput.Process();
		}

		SetClickThrough();
	}

	//Returns true if the cursor is over a UI element or 2D physics object
	private bool FocusForInput()
	{
		EventSystem eventSystem = EventSystem.current;
		if (eventSystem && eventSystem.IsPointerOverGameObject())
		{
			return true;
		}

		Vector2 pos = Camera.ScreenToWorldPoint(Input.mousePosition);
		return Physics2D.OverlapPoint(pos, clickLayerMask);
	}

	private void SetClickThrough()
	{
		var focusWindow = FocusForInput();

		//Get window position
		GetWindowRect(hwnd, out windowRect);

#if !UNITY_EDITOR
		if (focusWindow)
		{
			SetWindowLong (hwnd, -20, ~(((uint)524288) | ((uint)32)));
			SetWindowPos(hwnd, HWND_TOPMOST, windowRect.Left, windowRect.Top, fWidth, fHeight, 32 | 64);
		}
		else
		{
			SetWindowLong(hwnd, GWL_STYLE, WS_POPUP | WS_VISIBLE);
			SetWindowLong (hwnd, -20, (uint)524288 | (uint)32);
			SetLayeredWindowAttributes (hwnd, 0, 255, 2);
			SetWindowPos(hwnd, HWND_TOPMOST, windowRect.Left, windowRect.Top, fWidth, fHeight, 32 | 64);
		}
#endif
	}

	public static void DragWindow()
	{
#if !UNITY_EDITOR
		if (Screen.fullScreenMode != FullScreenMode.Windowed)
		{
			return;
		}
		ReleaseCapture ();
		SendMessage(Main.hwnd, WM_SYSCOMMAND, WM_MOUSE_MOVE, 0);
		Input.ResetInputAxes();
#endif		
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Rectangle
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;
	}
}
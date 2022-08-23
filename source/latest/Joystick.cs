using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GUITexture))]
public class Joystick : MonoBehaviour
{
	public bool touchPad;

	public bool fadeGUI;

	public Vector2 deadZone = Vector2.zero;

	public bool normalize;

	public int tapCount = -1;

	private Rect touchZone;

	private int lastFingerId = -1;

	private float tapTimeWindow;

	private Vector2 fingerDownPos;

	private float firstDeltaTime;

	private GUITexture gui;

	private Rect defaultRect;

	private Boundary guiBoundary = new Boundary();

	private Vector2 guiTouchOffset;

	private Vector2 guiCenter;

	private static string joysticksTag = "joystick";

	private static List<Joystick> joysticks;

	private static bool enumeratedJoysticks;

	private static float tapTimeDelta = 0.3f;

	public bool isFingerDown
	{
		get
		{
			return lastFingerId != -1;
		}
	}

	public int latchedFinger
	{
		set
		{
			if (lastFingerId == value)
			{
				Restart();
			}
		}
	}

	public Vector2 position { get; private set; }

	private void Reset()
	{
		//Discarded unreachable code: IL_0031
		try
		{
			base.gameObject.tag = joysticksTag;
		}
		catch (Exception)
		{
			Debug.LogError("The \"" + joysticksTag + "\" tag has not yet been defined in the Tag Manager.");
			throw;
		}
	}

	private void Awake()
	{
		//Discarded unreachable code: IL_00ea
		gui = GetComponent<GUITexture>();
		if (gui.texture == null)
		{
			Debug.LogError("Joystick object requires a valid texture!");
			base.gameObject.SetActive(false);
			return;
		}
		if (Application.platform != RuntimePlatform.Android && Application.platform == RuntimePlatform.IPhonePlayer)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (!enumeratedJoysticks)
		{
			try
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag(joysticksTag);
				joysticks = new List<Joystick>(array.Length);
				GameObject[] array2 = array;
				foreach (GameObject gameObject in array2)
				{
					Joystick component = gameObject.GetComponent<Joystick>();
					if (component == null)
					{
						throw new NullReferenceException("Joystick gameObject found without a suitable Joystick component.");
					}
					joysticks.Add(component);
				}
				enumeratedJoysticks = true;
			}
			catch (Exception ex)
			{
				Debug.LogError("Error collecting Joystick objects: " + ex.Message);
				throw;
			}
		}
		defaultRect = gui.pixelInset;
		defaultRect.x += base.transform.position.x * (float)Screen.width;
		defaultRect.y += base.transform.position.y * (float)Screen.height;
		base.transform.position = new Vector3(0f, 0f, base.transform.position.z);
		if (touchPad)
		{
			touchZone = defaultRect;
			return;
		}
		guiTouchOffset.x = defaultRect.width * 0.5f;
		guiTouchOffset.y = defaultRect.height * 0.5f;
		guiCenter.x = defaultRect.x + guiTouchOffset.x;
		guiCenter.y = defaultRect.y + guiTouchOffset.y;
		guiBoundary.min.x = defaultRect.x - guiTouchOffset.x;
		guiBoundary.max.x = defaultRect.x + guiTouchOffset.x;
		guiBoundary.min.y = defaultRect.y - guiTouchOffset.y;
		guiBoundary.max.y = defaultRect.y + guiTouchOffset.y;
	}

	public void Enable()
	{
		base.enabled = true;
	}

	public void Disable()
	{
		base.enabled = false;
	}

	public void Restart()
	{
		gui.pixelInset = defaultRect;
		lastFingerId = -1;
		position = Vector2.zero;
		fingerDownPos = Vector2.zero;
		if (touchPad && fadeGUI)
		{
			gui.color = new Color(gui.color.r, gui.color.g, gui.color.b, 0.025f);
		}
	}

	private void Update()
	{
		int touchCount = Input.touchCount;
		if (tapTimeWindow > 0f)
		{
			tapTimeWindow -= Time.deltaTime;
		}
		else
		{
			tapCount = 0;
		}
		if (touchCount == 0)
		{
			Restart();
		}
		else
		{
			for (int i = 0; i < touchCount; i++)
			{
				Touch touch = Input.GetTouch(i);
				Vector2 vector = touch.position - guiTouchOffset;
				bool flag = false;
				if (touchPad && touchZone.Contains(touch.position))
				{
					flag = true;
				}
				else if (gui.HitTest(touch.position))
				{
					flag = true;
				}
				if (flag && (lastFingerId == -1 || lastFingerId != touch.fingerId))
				{
					if (touchPad)
					{
						if (fadeGUI)
						{
							gui.color = new Color(gui.color.r, gui.color.g, gui.color.b, 0.15f);
						}
						lastFingerId = touch.fingerId;
						fingerDownPos = touch.position;
					}
					lastFingerId = touch.fingerId;
					if (tapTimeWindow > 0f)
					{
						tapCount++;
					}
					else
					{
						tapCount = 1;
						tapTimeWindow = tapTimeDelta;
					}
					foreach (Joystick joystick in joysticks)
					{
						if (!(joystick == this))
						{
							joystick.latchedFinger = touch.fingerId;
						}
					}
				}
				if (lastFingerId == touch.fingerId)
				{
					if (touch.tapCount > tapCount)
					{
						tapCount = touch.tapCount;
					}
					if (touchPad)
					{
						position = new Vector2(Mathf.Clamp((touch.position.x - fingerDownPos.x) / (touchZone.width / 2f), -1f, 1f), Mathf.Clamp((touch.position.y - fingerDownPos.y) / (touchZone.height / 2f), -1f, 1f));
					}
					else
					{
						gui.pixelInset = new Rect(Mathf.Clamp(vector.x, guiBoundary.min.x, guiBoundary.max.x), Mathf.Clamp(vector.y, guiBoundary.min.y, guiBoundary.max.y), gui.pixelInset.width, gui.pixelInset.height);
					}
					if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
					{
						Restart();
					}
				}
			}
		}
		if (!touchPad)
		{
			position = new Vector2((gui.pixelInset.x + guiTouchOffset.x - guiCenter.x) / guiTouchOffset.x, (gui.pixelInset.y + guiTouchOffset.y - guiCenter.y) / guiTouchOffset.y);
		}
		float num = Mathf.Abs(position.x);
		float num2 = Mathf.Abs(position.y);
		if (num < deadZone.x)
		{
			position = new Vector2(0f, position.y);
		}
		else if (normalize)
		{
			position = new Vector2(Mathf.Sign(position.x) * (num - deadZone.x) / (1f - deadZone.x), position.y);
		}
		if (num2 < deadZone.y)
		{
			position = new Vector2(position.x, 0f);
		}
		else if (normalize)
		{
			position = new Vector2(position.x, Mathf.Sign(position.y) * (num2 - deadZone.y) / (1f - deadZone.y));
		}
	}
}

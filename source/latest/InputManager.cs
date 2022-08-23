using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
	private Dictionary<EAction, InputData> m_Actions = new Dictionary<EAction, InputData>(new EActionKeyComparer());

	public float this[EAction pKey]
	{
		get
		{
			InputData value;
			if (m_Actions.TryGetValue(pKey, out value))
			{
				return value.Value;
			}
			return 0f;
		}
		set
		{
			InputData value2;
			if (m_Actions.TryGetValue(pKey, out value2))
			{
				value2.NextValue = value;
				value2.LifeTime = 1;
			}
		}
	}

	public InputManager()
	{
		for (int i = 0; i < Enum.GetValues(typeof(EAction)).Length; i++)
		{
			m_Actions.Add((EAction)i, new InputData());
		}
	}

	public void Init()
	{
	}

	public void Term()
	{
	}

	public void Update()
	{
		foreach (KeyValuePair<EAction, InputData> action in m_Actions)
		{
			InputData value = action.Value;
			if (value.NextValue == 0f && value.Value == 0f)
			{
				continue;
			}
			if (value.LifeTime > 0)
			{
				if (value.NextValue != value.Value)
				{
					value.Value = value.NextValue;
				}
				value.LifeTime--;
			}
			else
			{
				value.Value = 0f;
			}
		}
		if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
		{
			SetAction(EAction.Accelerate, Input.GetAxis("Vertical"));
			SetAction(EAction.Steer, Input.GetAxis("Horizontal"));
			SetAction(EAction.Drift, Convert.ToSingle(Input.GetButton("Fire1")));
			SetAction(EAction.DriftJump, Convert.ToSingle(Input.GetButtonDown("Fire1")));
			SetAction(EAction.LaunchBonus, Convert.ToSingle(Input.GetButtonDown("Fire2")));
			SetAction(EAction.DropBonus, Convert.ToSingle(Input.GetButtonDown("DropBonus")));
			if (Debug.isDebugBuild)
			{
				SetAction(EAction.Respawn, Convert.ToSingle(Input.GetButtonDown("Jump")));
			}
			SetAction(EAction.Pause, Convert.ToSingle(Input.GetButtonDown("Pause")));
		}
	}

	public void SetAction(EAction _Action, float _Value)
	{
		SetAction(_Action, _Value, 1);
	}

	public void SetAction(EAction _Action, float _Value, int _LifeTime)
	{
		InputData value;
		if (m_Actions.TryGetValue(_Action, out value))
		{
			value.NextValue = _Value;
			value.LifeTime = _LifeTime;
		}
	}

	public float GetAction(EAction _Action)
	{
		InputData value;
		if (m_Actions.TryGetValue(_Action, out value))
		{
			return value.Value;
		}
		return 0f;
	}
}

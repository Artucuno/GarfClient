using UnityEngine;

public class MessageButton : UIButtonMessage
{
	public enum EnumParamType
	{
		TypeInt,
		TypeFloat,
		TypeVector2,
		TypeVector3,
		TypeString,
		TypeGameObject,
		TypeEMenus
	}

	public int m_iParamInt;

	public float m_fParamFloat;

	public Vector2 m_oParamVector2;

	public Vector3 m_oParamVector3;

	public string m_sParamString;

	public GameObject m_oParamGameObject;

	private MenuEntryPoint m_oMenuEntryPoint;

	public int m_iParamType;

	public override void Start()
	{
		base.Start();
		GameObject gameObject = GameObject.Find("MenuEntryPoint");
		if ((bool)gameObject)
		{
			m_oMenuEntryPoint = gameObject.GetComponent<MenuEntryPoint>();
		}
	}

	public override void Send()
	{
		if (string.IsNullOrEmpty(functionName))
		{
			return;
		}
		if (target == null)
		{
			target = base.gameObject;
		}
		object value;
		switch (m_iParamType)
		{
		default:
			value = m_iParamInt;
			break;
		case 1:
			value = m_fParamFloat;
			break;
		case 2:
			value = m_oParamVector2;
			break;
		case 3:
			value = m_oParamVector3;
			break;
		case 4:
			value = m_sParamString;
			break;
		case 5:
			value = m_oParamGameObject;
			break;
		case 6:
			value = m_iParamInt;
			break;
		}
		if (includeChildren)
		{
			Transform[] componentsInChildren = target.GetComponentsInChildren<Transform>();
			int i = 0;
			for (int num = componentsInChildren.Length; i < num; i++)
			{
				Transform transform = componentsInChildren[i];
				transform.gameObject.SendMessage(functionName, value, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			target.SendMessage(functionName, value, SendMessageOptions.DontRequireReceiver);
		}
	}

	public override void OnHover(bool isOver)
	{
		base.OnHover(isOver);
		if (base.enabled && isOver && (bool)m_oMenuEntryPoint)
		{
			m_oMenuEntryPoint.PlayHoverSound();
		}
	}

	public override void OnClick()
	{
		base.OnClick();
		if (base.enabled && (bool)m_oMenuEntryPoint)
		{
			if (base.gameObject.name == "ButtonPrev")
			{
				m_oMenuEntryPoint.PlayBackSound();
			}
			else if (base.gameObject.name == "ButtonGo")
			{
				m_oMenuEntryPoint.PlayGoSound();
			}
			else if (base.gameObject.name.Contains("ButtonArrow"))
			{
				m_oMenuEntryPoint.PlaySwitchPageSound();
			}
			else
			{
				m_oMenuEntryPoint.PlayValidSound();
			}
		}
	}
}

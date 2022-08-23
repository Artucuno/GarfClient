using UnityEngine;

public class IconCarac : MonoBehaviour
{
	public string m_sSpriteName;

	public string m_TitleTextId;

	public string m_InfoTextId;

	public string spriteName
	{
		get
		{
			return m_sSpriteName;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				if (!string.IsNullOrEmpty(m_sSpriteName))
				{
					m_sSpriteName = string.Empty;
				}
			}
			else if (m_sSpriteName != value)
			{
				m_sSpriteName = value;
			}
		}
	}

	public static int CompareNameDefault(IconCarac oItem1, IconCarac oItem2)
	{
		bool flag = oItem2.name.Contains("_Def");
		if (oItem1.name.Contains("_Def"))
		{
			if (flag)
			{
				return CompareName(oItem1, oItem2);
			}
			return -1;
		}
		if (flag)
		{
			return 1;
		}
		return 0;
	}

	public static int CompareName(IconCarac oItem1, IconCarac oItem2)
	{
		return oItem1.name.CompareTo(oItem2.name);
	}

	public static E_UnlockableItemSate SuppressNewState(E_UnlockableItemSate eState)
	{
		switch (eState)
		{
		case E_UnlockableItemSate.NewLocked:
			return E_UnlockableItemSate.Locked;
		case E_UnlockableItemSate.NewUnlocked:
			return E_UnlockableItemSate.Unlocked;
		default:
			return eState;
		}
	}
}

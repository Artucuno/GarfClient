public class KartBonus
{
	public static Tuple<EITEM, bool>[] BonusBehind = new Tuple<EITEM, bool>[10]
	{
		new Tuple<EITEM, bool>(EITEM.ITEM_NONE, false),
		new Tuple<EITEM, bool>(EITEM.ITEM_PIE, true),
		new Tuple<EITEM, bool>(EITEM.ITEM_AUTOLOCK_PIE, true),
		new Tuple<EITEM, bool>(EITEM.ITEM_SPRING, true),
		new Tuple<EITEM, bool>(EITEM.ITEM_LASAGNA, false),
		new Tuple<EITEM, bool>(EITEM.ITEM_DIAMOND, true),
		new Tuple<EITEM, bool>(EITEM.ITEM_UFO, false),
		new Tuple<EITEM, bool>(EITEM.ITEM_NAP, false),
		new Tuple<EITEM, bool>(EITEM.ITEM_PARFUME, false),
		new Tuple<EITEM, bool>(EITEM.ITEM_MAGIC, false)
	};

	public EITEM m_eItem;

	public int m_iQuantity;

	public bool m_bAnimated;

	public BonusAnimation m_pSprite;

	public float m_fSecureAnimationTimer;

	public KartBonus()
	{
		m_eItem = EITEM.ITEM_NONE;
		m_iQuantity = 0;
		m_bAnimated = false;
		m_pSprite = null;
		m_fSecureAnimationTimer = 0f;
	}

	public void Reset()
	{
		if (m_eItem == EITEM.ITEM_UFO)
		{
			Singleton<BonusMgr>.Instance.UfoLaunched = false;
		}
		m_eItem = EITEM.ITEM_NONE;
		m_iQuantity = 0;
		m_bAnimated = false;
		m_pSprite = null;
		m_fSecureAnimationTimer = 0f;
	}

	public void Affect(KartBonus kb)
	{
		m_eItem = kb.m_eItem;
		m_iQuantity = kb.m_iQuantity;
		m_bAnimated = kb.m_bAnimated;
		m_pSprite = kb.m_pSprite;
		m_fSecureAnimationTimer = kb.m_fSecureAnimationTimer;
	}

	public static bool IsBehind(EITEM _Item)
	{
		Tuple<EITEM, bool>[] bonusBehind = BonusBehind;
		foreach (Tuple<EITEM, bool> tuple in bonusBehind)
		{
			if (tuple.Item1 == _Item)
			{
				return tuple.Item2;
			}
		}
		return false;
	}
}

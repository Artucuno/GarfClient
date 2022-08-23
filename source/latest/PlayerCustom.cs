using UnityEngine;

public class PlayerCustom : MonoBehaviour
{
	private BonusCustom m_pBonusCustom;

	private KartCustom m_pKartCustom;

	private PlayerCarac m_pPlayerCarac;

	public KartCustom KartCustom
	{
		get
		{
			return m_pKartCustom;
		}
		set
		{
			m_pKartCustom = value;
		}
	}

	public BonusCustom BonusCustom
	{
		get
		{
			return m_pBonusCustom;
		}
		set
		{
			m_pBonusCustom = value;
		}
	}

	private void Awake()
	{
		m_pBonusCustom = null;
		m_pKartCustom = null;
		m_pPlayerCarac = null;
	}

	private void Start()
	{
		m_pPlayerCarac = base.transform.parent.FindChild("Tunning").GetComponent<PlayerCarac>();
	}

	public float GetBonusValue(EITEM _Bonus, EBonusCustomEffect _Effect)
	{
		if (m_pBonusCustom != null && _Bonus == m_pBonusCustom.Category && _Effect == m_pBonusCustom.Effect && m_pPlayerCarac != null && m_pPlayerCarac.CharacterCarac != null)
		{
			float num = 0f;
			if (m_pPlayerCarac.CharacterCarac.Owner == m_pBonusCustom.Character)
			{
				num = ((_Effect != 0 && _Effect != EBonusCustomEffect.STICK) ? (m_pBonusCustom.MegaValue * m_pBonusCustom.Value / 100f) : m_pBonusCustom.MegaValue);
			}
			return m_pBonusCustom.Value + num;
		}
		return 0f;
	}
}

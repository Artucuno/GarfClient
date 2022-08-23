using UnityEngine;

public class BonusEffectMgr
{
	private BonusEffect[] m_pEffect = new BonusEffect[9];

	private Kart m_pTarget;

	public Kart Target
	{
		get
		{
			return m_pTarget;
		}
		set
		{
			m_pTarget = value;
		}
	}

	public BonusEffectMgr(Kart _Target)
	{
		Target = _Target;
		for (int i = 0; i < 9; i++)
		{
			m_pEffect[i] = null;
		}
	}

	public void Start()
	{
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("BonusEffect/BoostBonusEffect"));
		m_pEffect[0] = gameObject.GetComponent<BonusEffect>();
		gameObject = (GameObject)Object.Instantiate(Resources.Load("BonusEffect/UpsideDownBonusEffect"));
		m_pEffect[1] = gameObject.GetComponent<UpsideDownBonusEffect>();
		gameObject = (GameObject)Object.Instantiate(Resources.Load("BonusEffect/SpinBonusEffect"));
		m_pEffect[2] = gameObject.GetComponent<SpinBonusEffect>();
		gameObject = (GameObject)Object.Instantiate(Resources.Load("BonusEffect/JumpBonusEffect"));
		m_pEffect[8] = gameObject.GetComponent<JumpBonusEffect>();
		gameObject = (GameObject)Object.Instantiate(Resources.Load("BonusEffect/NapBonusEffect"));
		m_pEffect[3] = gameObject.GetComponent<NapBonusEffect>();
		gameObject = (GameObject)Object.Instantiate(Resources.Load("BonusEffect/MagicBonusEffect"));
		m_pEffect[5] = gameObject.GetComponent<MagicBonusEffect>();
		gameObject = (GameObject)Object.Instantiate(Resources.Load("BonusEffect/LevitateBonusEffect"));
		m_pEffect[7] = gameObject.GetComponent<LevitateBonusEffect>();
		gameObject = (GameObject)Object.Instantiate(Resources.Load("BonusEffect/ParfumeBonusEffect"));
		m_pEffect[4] = gameObject.GetComponent<ParfumeBonusEffect>();
		for (int i = 0; i < 9; i++)
		{
			if (m_pEffect[i] != null)
			{
				m_pEffect[i].BonusEffectMgr = this;
			}
		}
	}

	public void Dispose()
	{
		for (int i = 0; i < m_pEffect.Length; i++)
		{
			if (m_pEffect[i] != null)
			{
				Object.Destroy(m_pEffect[i].gameObject);
			}
		}
	}

	public void Update()
	{
	}

	public void Reset()
	{
		for (int i = 0; i < 9; i++)
		{
			if (m_pEffect[i] != null && m_pEffect[i].Activated)
			{
				m_pEffect[i].Deactivate();
			}
		}
	}

	public bool ActivateBonusEffect(EBonusEffect _BonusEffect)
	{
		if (_BonusEffect != EBonusEffect.BONUSEFFECT_COUNT && m_pEffect[(int)_BonusEffect] != null)
		{
			return m_pEffect[(int)_BonusEffect].Activate();
		}
		return false;
	}

	public void DeactivateBonusEffect(EBonusEffect _BonusEffect)
	{
		if (_BonusEffect != EBonusEffect.BONUSEFFECT_COUNT && m_pEffect[(int)_BonusEffect] != null)
		{
			m_pEffect[(int)_BonusEffect].Deactivate();
		}
	}

	public bool IsBonusEffectActivated(EBonusEffect _BonusEffect)
	{
		if (_BonusEffect != EBonusEffect.BONUSEFFECT_COUNT && m_pEffect[(int)_BonusEffect] != null)
		{
			return m_pEffect[(int)_BonusEffect].Activated;
		}
		return false;
	}

	public BonusEffect GetBonusEffect(EBonusEffect _BonusEffect)
	{
		if (_BonusEffect != EBonusEffect.BONUSEFFECT_COUNT && m_pEffect[(int)_BonusEffect] != null)
		{
			return m_pEffect[(int)_BonusEffect];
		}
		return null;
	}
}

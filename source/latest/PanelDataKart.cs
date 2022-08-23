using UnityEngine;

public class PanelDataKart : MonoBehaviour
{
	public GameObject m_oGaugeAcceleration;

	public GameObject m_oGaugeSpeed;

	public GameObject m_oGaugeSteering;

	public GameObject[] m_oSpriteAdv = new GameObject[4];

	public GameObject m_oIconAdv;

	private float m_fAcceleration;

	private float m_fAccelerationTarget;

	private float m_fSpeed;

	private float m_fSpeedTarget;

	private float m_fManiability;

	private float m_fManiabilityTarget;

	public float m_fGaugeInertia = 0.15f;

	private float m_fGaugeScaleMin = 56f;

	private float m_fGaugeScaleMax = 350f;

	private MenuSelectKart m_pMenuSelectKart;

	private void Start()
	{
		m_fAcceleration = (m_fAccelerationTarget = 0f);
		m_fSpeed = (m_fSpeedTarget = 0f);
		m_fManiability = (m_fManiabilityTarget = 0f);
		Transform parent = base.transform.parent;
		m_pMenuSelectKart = parent.GetComponent<MenuSelectKart>();
		while (m_pMenuSelectKart == null)
		{
			parent = parent.parent;
			if (!parent)
			{
				break;
			}
			m_pMenuSelectKart = parent.GetComponent<MenuSelectKart>();
		}
	}

	private void Update()
	{
		Vector3 vector = new Vector3(1f, 1f, 1f);
		if ((bool)m_oGaugeAcceleration)
		{
			vector = m_oGaugeAcceleration.transform.localScale;
		}
		if (m_fAcceleration != m_fAccelerationTarget)
		{
			m_fAcceleration = Tricks.ComputeInertia(m_fAcceleration, m_fAccelerationTarget, m_fGaugeInertia, Time.deltaTime);
			if ((bool)m_oGaugeAcceleration)
			{
				m_oGaugeAcceleration.transform.localScale = new Vector3(m_fGaugeScaleMin + (m_fGaugeScaleMax - m_fGaugeScaleMin) * m_fAcceleration / 100f, vector.y, vector.z);
			}
		}
		if (m_fSpeed != m_fSpeedTarget)
		{
			m_fSpeed = Tricks.ComputeInertia(m_fSpeed, m_fSpeedTarget, m_fGaugeInertia, Time.deltaTime);
			if ((bool)m_oGaugeSpeed)
			{
				m_oGaugeSpeed.transform.localScale = new Vector3(m_fGaugeScaleMin + (m_fGaugeScaleMax - m_fGaugeScaleMin) * m_fSpeed / 100f, vector.y, vector.z);
			}
		}
		if (m_fManiability != m_fManiabilityTarget)
		{
			m_fManiability = Tricks.ComputeInertia(m_fManiability, m_fManiabilityTarget, m_fGaugeInertia, Time.deltaTime);
			if ((bool)m_oGaugeSteering)
			{
				m_oGaugeSteering.transform.localScale = new Vector3(m_fGaugeScaleMin + (m_fGaugeScaleMax - m_fGaugeScaleMin) * m_fManiability / 100f, vector.y, vector.z);
			}
		}
	}

	public void OnUpdatePanel()
	{
		int num = 0;
		PlayerConfig playerConfig = Singleton<GameConfigurator>.Instance.PlayerConfig;
		KartCarac kartCarac = (KartCarac)Resources.Load("Kart/" + playerConfig.KartPrefab[(int)playerConfig.m_eKart], typeof(KartCarac));
		CharacterCarac characterCarac = (CharacterCarac)Resources.Load("Character/" + playerConfig.CharacterPrefab[(int)playerConfig.m_eCharacter], typeof(CharacterCarac));
		m_fAccelerationTarget = kartCarac.Acceleration + characterCarac.Acceleration + playerConfig.m_oKartCustom.Acceleration;
		m_fSpeedTarget = kartCarac.Speed + characterCarac.Speed + playerConfig.m_oKartCustom.Speed;
		m_fManiabilityTarget = kartCarac.Maniability + characterCarac.Maniability + playerConfig.m_oKartCustom.Maniability;
		if (kartCarac.Owner == playerConfig.m_eCharacter)
		{
			switch (kartCarac.BonusCaracteristic)
			{
			case DrivingCaracteristics.ACCELERATION:
				m_fAccelerationTarget += kartCarac.Bonus;
				break;
			case DrivingCaracteristics.SPEED:
				m_fSpeedTarget += kartCarac.Bonus;
				break;
			case DrivingCaracteristics.MANIABILITY:
				m_fManiabilityTarget += kartCarac.Bonus;
				break;
			}
		}
		if (playerConfig.m_oKartCustom.Owner == playerConfig.m_eKart)
		{
			switch (playerConfig.m_oKartCustom.BonusCaracteristic)
			{
			case DrivingCaracteristics.ACCELERATION:
				m_fAccelerationTarget += playerConfig.m_oKartCustom.Bonus;
				break;
			case DrivingCaracteristics.SPEED:
				m_fSpeedTarget += playerConfig.m_oKartCustom.Bonus;
				break;
			case DrivingCaracteristics.MANIABILITY:
				m_fManiabilityTarget += playerConfig.m_oKartCustom.Bonus;
				break;
			}
		}
		num = kartCarac.NbSlots + characterCarac.NbSlots + playerConfig.m_oHat.NbSlots + playerConfig.m_oKartCustom.NbSlots;
		if ((bool)m_pMenuSelectKart && m_pMenuSelectKart.GetAdvantageRestrictions() == MenuSelectKart.EAdvantageRestrictions.TIMETRIAL_RESTRICTION)
		{
			num = 0;
		}
		Singleton<GameConfigurator>.Instance.NbSlots = num;
		for (int i = 0; i < 4; i++)
		{
			m_oSpriteAdv[i].SetActive((i < num) ? true : false);
		}
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TIME_TRIAL)
		{
			m_oIconAdv.SetActive(false);
		}
		else
		{
			m_oIconAdv.SetActive(true);
		}
	}
}

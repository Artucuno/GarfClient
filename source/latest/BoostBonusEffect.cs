using UnityEngine;

public class BoostBonusEffect : BonusEffect
{
	[HideInInspector]
	[SerializeField]
	public float SpeedUp;

	[HideInInspector]
	[SerializeField]
	public float Acceleration;

	public BoostBonusEffect()
	{
		SpeedUp = 0f;
		Acceleration = 0f;
		InertiaVehicle = false;
		m_bStoppedByAnim = false;
	}

	public bool boostToggle = true;

	protected override void OnDestroy()
	{
	}

	public override void Start()
	{
		base.Start();
	}

	public override void Update()
	{
		base.Update();
	}

	public override bool Activate()
	{
		base.Activate();
		Kart target = m_pBonusEffectMgr.Target;
		float boostDelay = EffectDuration + m_pBonusEffectMgr.Target.GetBonusMgr().GetBonusValue(EITEM.ITEM_LASAGNA, EBonusCustomEffect.DURATION) * EffectDuration / 100f;
		bool flag = target.GetControlType() == RcVehicle.ControlType.AI;
		int iQuantity = 1;
		if (!flag)
		{
			if (!boostToggle)
			{
				target.Boost(10000, boostDelay, Acceleration, true);
			}
			else
            {
				target.Boost(SpeedUp, boostDelay, Acceleration, true);
			}
		}
		else
			target.Boost(SpeedUp, boostDelay, Acceleration, true);
		target.KartSound.PlayVoice(KartSound.EVoices.Good);
		if (target.OnBoost != null)
		{
			target.OnBoost();
		}
		return true;
	}
}

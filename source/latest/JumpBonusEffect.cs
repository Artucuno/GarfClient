using UnityEngine;

public class JumpBonusEffect : BonusEffect
{
	[HideInInspector]
	[SerializeField]
	public float JumpHeight;

	[HideInInspector]
	[SerializeField]
	public float JumpForward;

	private bool m_bBackwardJump;

	public float RepulseJump = 20f;

	public bool BackwardJump
	{
		set
		{
			m_bBackwardJump = value;
		}
	}

	public JumpBonusEffect()
	{
		JumpHeight = 0f;
		JumpForward = 0f;
		InertiaVehicle = false;
		m_bStoppedByAnim = false;
		BackwardJump = false;
		m_iAnimState = Animator.StringToHash("UpsideDown.UpsideDown_Front");
		m_iAnimParameter = Animator.StringToHash("UpsideDown_Front");
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
		if (m_pBonusEffectMgr.GetBonusEffect(EBonusEffect.BONUSEFFECT_LEVITATE).Activated)
		{
			return true;
		}
		base.Activate();
		Kart target = m_pBonusEffectMgr.Target;
		float num = JumpForward + m_pBonusEffectMgr.Target.GetBonusMgr().GetBonusValue(EITEM.ITEM_SPRING, EBonusCustomEffect.HEIGHT) * JumpForward / 100f;
		if (m_bBackwardJump)
		{
			num += RepulseJump * target.GetWheelSpeedMS() / target.GetMaxSpeed();
		}
		if (target.SpringJump(JumpHeight, num, m_bBackwardJump))
		{
			target.FxMgr.PlayKartFx(eKartFx.Jump);
		}
		target.KartSound.PlaySound(16);
		target.KartSound.PlayVoice(KartSound.EVoices.Good);
		if (m_bBackwardJump)
		{
			target.Anim.LaunchBonusAnimAll(m_iAnimParameter, m_iAnimState, false);
			target.Anim.StopBonusAnimAll(m_iAnimState);
		}
		m_bBackwardJump = false;
		return true;
	}
}

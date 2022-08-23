using System;
using UnityEngine;

public class UpsideDownBonusEffect : BonusEffect
{
	private int[] m_AnimState = new int[Enum.GetValues(typeof(EBonusEffectDirection)).Length];

	private int[] m_AnimParameter = new int[Enum.GetValues(typeof(EBonusEffectDirection)).Length];

	public static Action<int> OnLaunch;

	public UpsideDownBonusEffect()
	{
		m_AnimState[0] = Animator.StringToHash("UpsideDown.UpsideDown_Right");
		m_AnimParameter[0] = Animator.StringToHash("UpsideDown_Right");
		m_AnimState[1] = Animator.StringToHash("UpsideDown.UpsideDown_Left");
		m_AnimParameter[1] = Animator.StringToHash("UpsideDown_Left");
		m_AnimState[2] = Animator.StringToHash("UpsideDown.UpsideDown_Front");
		m_AnimParameter[2] = Animator.StringToHash("UpsideDown_Front");
		m_AnimState[3] = Animator.StringToHash("UpsideDown.UpsideDown_Back");
		m_AnimParameter[3] = Animator.StringToHash("UpsideDown_Back");
		m_iAnimParameter = m_AnimParameter[0];
		m_iAnimState = m_AnimState[0];
	}

	public override void Start()
	{
		m_bStoppedByAnim = false;
		base.Start();
	}

	public override void Update()
	{
		base.Update();
	}

	public override bool Activate()
	{
		if (Activated || m_pBonusEffectMgr.GetBonusEffect(EBonusEffect.BONUSEFFECT_SPIN).Activated || m_pBonusEffectMgr.GetBonusEffect(EBonusEffect.BONUSEFFECT_JUMP).Activated)
		{
			return false;
		}
		if (m_pBonusEffectMgr.GetBonusEffect(EBonusEffect.BONUSEFFECT_LEVITATE).Activated)
		{
			return true;
		}
		m_iAnimState = m_AnimState[(int)m_eEffectDirection];
		m_iAnimParameter = m_AnimParameter[(int)m_eEffectDirection];
		base.Activate();
		Kart target = m_pBonusEffectMgr.Target;
		target.CancelDrift();
		target.Anim.LaunchBonusAnimOnKart(m_iAnimParameter, m_iAnimState, true);
		target.KartSound.PlaySound(11);
		target.KartSound.PlayVoice(KartSound.EVoices.Bad);
		if (OnLaunch != null)
		{
			OnLaunch(target.Index);
			if (target.OnHit != null)
			{
				target.OnHit();
			}
		}
		return true;
	}
}

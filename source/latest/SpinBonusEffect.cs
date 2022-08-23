using System;
using UnityEngine;

public class SpinBonusEffect : BonusEffect
{
	public static Action<int> OnLaunch;

	public SpinBonusEffect()
	{
		m_iAnimParameter = Animator.StringToHash("Slide");
		m_iAnimState = Animator.StringToHash("Base Layer.Slide");
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
		if (Activated || m_pBonusEffectMgr.GetBonusEffect(EBonusEffect.BONUSEFFECT_UPSIDE_DOWN).Activated || m_pBonusEffectMgr.GetBonusEffect(EBonusEffect.BONUSEFFECT_LEVITATE).Activated || m_pBonusEffectMgr.GetBonusEffect(EBonusEffect.BONUSEFFECT_JUMP).Activated)
		{
			return false;
		}
		base.Activate();
		Kart target = m_pBonusEffectMgr.Target;
		target.CancelDrift();
		target.KartSound.PlaySound(12);
		target.KartSound.PlayVoice(KartSound.EVoices.Bad);
		if (OnLaunch != null)
		{
			OnLaunch(target.Index);
		}
		return true;
	}
}

using UnityEngine;

public class LevitateBonusEffect : BonusEffect
{
	private UFO m_pOwner;

	private float m_fPreviousGroundGravity;

	private float m_fPreviousAirGravity;

	private bool m_bStopLevitate;

	public UFO Owner
	{
		set
		{
			m_pOwner = value;
		}
	}

	public LevitateBonusEffect()
	{
		m_pOwner = null;
	}

	public override void Start()
	{
		m_iAnimParameter = Animator.StringToHash("Ufo");
		m_iAnimState = Animator.StringToHash("Base Layer.Ufo");
		m_bStoppedByAnim = false;
		base.Start();
	}

	public override void Update()
	{
		base.Update();
		if (Activated && !m_bStopLevitate && m_pBonusEffectMgr.Target.Transform.position.y > m_pOwner.Transform.position.y - m_pOwner.LevitateOffset && m_pOwner.Parent.GetState() == BonusEntity.BonusState.BONUS_ONGROUND)
		{
			Kart target = m_pBonusEffectMgr.Target;
			RcKinematicPhysic rcKinematicPhysic = (RcKinematicPhysic)target.GetVehiclePhysic();
			rcKinematicPhysic.SetLinearVelocity(new Vector3(0f, 0f, 0f));
			m_bStopLevitate = true;
		}
	}

	public override void SetDuration()
	{
		m_fCurrentDuration = EffectDuration + m_pOwner.Parent.BonusDuration * EffectDuration / 100f;
	}

	public override bool Activate()
	{
		Kart target = m_pBonusEffectMgr.Target;
		RcKinematicPhysic rcKinematicPhysic = (RcKinematicPhysic)target.GetVehiclePhysic();
		float num = m_pOwner.Parent.UfoHeight - m_pOwner.LevitateOffset;
		Vector3 impulseForce = new Vector3(0f, num / 1f, 0f);
		ImpulseForce = impulseForce;
		rcKinematicPhysic.SetLinearVelocity(new Vector3(0f, 0f, 0f));
		base.Activate();
		m_bStopLevitate = false;
		target.CancelDrift();
		if (rcKinematicPhysic != null)
		{
			m_fPreviousAirGravity = rcKinematicPhysic.m_fAirAdditionnalGravity;
			m_fPreviousGroundGravity = rcKinematicPhysic.m_fGroundAdditionnalGravity;
			rcKinematicPhysic.m_fAirAdditionnalGravity = Physics.gravity.y;
			rcKinematicPhysic.m_fGroundAdditionnalGravity = Physics.gravity.y;
		}
		target.KartSound.PlaySound(15);
		return true;
	}

	public override void Deactivate()
	{
		base.Deactivate();
		Kart target = m_pBonusEffectMgr.Target;
		RcKinematicPhysic rcKinematicPhysic = (RcKinematicPhysic)target.GetVehiclePhysic();
		if (rcKinematicPhysic != null)
		{
			rcKinematicPhysic.m_fAirAdditionnalGravity = m_fPreviousAirGravity;
			rcKinematicPhysic.m_fGroundAdditionnalGravity = m_fPreviousGroundGravity;
		}
		if (m_pOwner != null && (Network.peerType == NetworkPeerType.Disconnected || Network.isServer))
		{
			m_pOwner.RemoveUFO(false);
		}
		target.KartSound.StopSound(15);
	}
}

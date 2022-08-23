using UnityEngine;

public class MalusBonusEntity : BonusEntity
{
	protected int m_iLayerVehicle;

	protected int m_iLayerBonus;

	public LayerMask LayerStick;

	protected LayerMask m_pLayerBonus;

	protected LayerMask m_pLayerIgnoreRaycast;

	protected EBonusEffectDirection m_eImpactDirection;

	public MalusBonusEntity()
	{
		ReactivationDelay = 0f;
		m_bBehind = false;
	}

	public override void Awake()
	{
		base.Awake();
		m_iLayerVehicle = 1 << LayerMask.NameToLayer("Vehicle");
		m_iLayerBonus = 1 << LayerMask.NameToLayer("Bonus");
		m_pLayerBonus = LayerMask.NameToLayer("Bonus");
		m_pLayerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
		ActivateGameObject(false);
	}

	public override void Update()
	{
		base.Update();
		if (m_eState == BonusState.BONUS_LAUNCHREQUEST)
		{
			SetActive(true);
			m_eState = BonusState.BONUS_LAUNCHED;
		}
		else if (m_eState == BonusState.BONUS_LAUNCHED)
		{
			if (m_bBehind)
			{
				LaunchAnimFinished();
				m_eState = BonusState.BONUS_ANIMLAUNCHED;
			}
		}
		else if (m_eState == BonusState.BONUS_ANIMLAUNCHED && m_bBehind && IsOnGround())
		{
			m_eState = BonusState.BONUS_ONGROUND;
		}
	}

	public virtual void LaunchAnimFinished()
	{
	}

	public override void OnEnable()
	{
		base.OnEnable();
	}

	public virtual void OnCollisionEnter(Collision collision)
	{
		if (Network.peerType == NetworkPeerType.Disconnected || Network.isServer)
		{
			int num = 1 << collision.gameObject.layer;
			if (((num & m_iLayerVehicle) != 0 || (num & m_iLayerBonus) != 0) && collision.gameObject != null)
			{
				OnTriggerEnter(collision.collider);
			}
		}
	}

	public override void DoOnTriggerEnter(GameObject other, int otherlayer)
	{
		base.DoOnTriggerEnter(other, otherlayer);
		PerformBonusReaction(other, otherlayer);
	}

	public void PerformBonusReaction(GameObject other, int otherlayer)
	{
		int num = 1 << otherlayer;
		if ((num & m_iLayerVehicle) != 0)
		{
			if (!(other != null))
			{
				return;
			}
			Kart componentInChildren = other.GetComponentInChildren<Kart>();
			if (m_eState < BonusState.BONUS_LAUNCHED)
			{
				return;
			}
			if (componentInChildren != null)
			{
				m_eImpactDirection = CheckImpactDirection(other);
				m_eState = BonusState.BONUS_TRIGGERED;
				ParfumeBonusEffect parfumeBonusEffect = (ParfumeBonusEffect)componentInChildren.GetBonusMgr().GetBonusEffectMgr().GetBonusEffect(EBonusEffect.BONUSEFFECT_ATTRACTED);
				if (!parfumeBonusEffect.Activated || parfumeBonusEffect.StinkParfume)
				{
					ActivateBonusEffect(componentInChildren);
				}
			}
			NetDestroy();
		}
		else if ((num & m_iLayerBonus) != 0 && other != null)
		{
			BonusEntity componentInChildren2 = other.GetComponentInChildren<BonusEntity>();
			if (m_eState >= BonusState.BONUS_LAUNCHED)
			{
				PerformBonusCollision(componentInChildren2);
			}
		}
	}

	public virtual void PerformBonusCollision(BonusEntity _Bonus)
	{
		if (_Bonus != null)
		{
			m_eState = BonusState.BONUS_TRIGGERED;
			_Bonus.NetDestroy();
		}
		NetDestroy();
	}

	public virtual void ActivateBonusEffect(Kart _Kart)
	{
	}

	public override void SetActive(bool _Active)
	{
		base.SetActive(_Active);
	}

	protected override void OnDestroy()
	{
	}

	public override void DoDestroy()
	{
		base.DoDestroy();
		SetActive(false);
		ActivateGameObject(false);
	}

	public override void Launch()
	{
		base.Launch();
		ActivateGameObject(true);
	}

	public virtual bool IsOnGround()
	{
		return false;
	}

	public EBonusEffectDirection CheckImpactDirection(GameObject _object)
	{
		Vector3 vector = _object.transform.InverseTransformPoint(m_pTransform.position);
		if (vector.z > 0f)
		{
			if (vector.z > Mathf.Abs(vector.x))
			{
				return EBonusEffectDirection.FRONT;
			}
			if (vector.x > 0f)
			{
				return EBonusEffectDirection.RIGHT;
			}
			return EBonusEffectDirection.LEFT;
		}
		if (vector.z < Mathf.Abs(vector.x))
		{
			return EBonusEffectDirection.BACK;
		}
		if (vector.x > 0f)
		{
			return EBonusEffectDirection.RIGHT;
		}
		return EBonusEffectDirection.LEFT;
	}
}

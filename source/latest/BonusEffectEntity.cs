using UnityEngine;

public class BonusEffectEntity : BonusEntity
{
	public LayerMask layer;

	public EBonusEffect BonusEffect;

	public override void Awake()
	{
		base.Awake();
		if (base.renderer != null)
		{
			base.renderer.enabled = true;
		}
		m_pCollider.enabled = true;
		m_bActive = true;
	}

	public override void Update()
	{
		base.Update();
	}

	public override void DoOnTriggerEnter(GameObject other, int otherlayer)
	{
		if (m_bActive)
		{
			int num = 1 << otherlayer;
			if ((num & (int)layer) != 0)
			{
				OnGoodCollision(other);
			}
		}
	}

	public virtual void OnGoodCollision(GameObject other)
	{
		Kart componentInChildren = other.GetComponentInChildren<Kart>();
		if ((bool)componentInChildren)
		{
			componentInChildren.GetBonusMgr().GetBonusEffectMgr().ActivateBonusEffect(BonusEffect);
		}
		if (ReactivationDelay > 0f)
		{
			SetActive(false);
		}
	}

	public override void DoDestroy()
	{
		base.DoDestroy();
	}

	public override void Launch()
	{
		base.Launch();
	}
}

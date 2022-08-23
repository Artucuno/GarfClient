using UnityEngine;

public class SpringBonusEntity : BonusEffectEntity
{
	private float m_fTimerNoCollision;

	private Collider LauncherCollider;

	public LayerMask RoadLayer;

	private bool m_bOnGround;

	public float SpeedBackward = 50f;

	public ParticleSystem SpringUsed;

	public AudioSource SoundDropped;

	public SpringBonusEntity()
	{
		ReactivationDelay = 0f;
		m_eItem = EITEM.ITEM_SPRING;
	}

	public override void Awake()
	{
		base.Awake();
		m_bSynchronizePosition = true;
		SetActive(false);
		ActivateGameObject(false);
	}

	public override void Launch()
	{
		m_bBehind = true;
		base.Launch();
		SetActive(true);
		ActivateGameObject(true);
		if (Network.peerType == NetworkPeerType.Disconnected || Network.isServer)
		{
			m_pTransform.position = base.Launcher.Transform.position + base.Launcher.Transform.parent.rotation * new Vector3(0f, 1f, -1.5f);
		}
		LauncherCollider = base.Launcher.Transform.parent.collider;
		Physics.IgnoreCollision(m_pCollider, LauncherCollider, true);
		m_fTimerNoCollision = 0f;
		m_bOnGround = false;
		if ((bool)SoundDropped)
		{
			SoundDropped.Play();
		}
	}

	public override void Update()
	{
		base.Update();
		if (base.Launcher != null)
		{
			float deltaTime = Time.deltaTime;
			m_fTimerNoCollision += deltaTime;
			if (m_fTimerNoCollision > 1f)
			{
				Physics.IgnoreCollision(m_pCollider, LauncherCollider, false);
				base.Launcher = null;
			}
			if ((Network.peerType == NetworkPeerType.Disconnected || Network.isServer) && !m_bOnGround)
			{
				float num = SpeedBackward / 3.6f;
				base.transform.position += deltaTime * num * Vector3.down;
			}
		}
	}

	public override void OnGoodCollision(GameObject other)
	{
		Kart componentInChildren = other.GetComponentInChildren<Kart>();
		if ((bool)componentInChildren)
		{
			ParfumeBonusEffect parfumeBonusEffect = (ParfumeBonusEffect)componentInChildren.GetBonusMgr().GetBonusEffectMgr().GetBonusEffect(EBonusEffect.BONUSEFFECT_ATTRACTED);
			if (!parfumeBonusEffect.Activated || parfumeBonusEffect.StinkParfume)
			{
				JumpBonusEffect jumpBonusEffect = (JumpBonusEffect)componentInChildren.GetBonusMgr().GetBonusEffectMgr().GetBonusEffect(BonusEffect);
				jumpBonusEffect.BackwardJump = true;
				base.OnGoodCollision(other);
			}
		}
		SetActive(false);
		ActivateGameObject(false);
	}

	public override void DoDestroy()
	{
		base.DoDestroy();
		if ((bool)SpringUsed)
		{
			SpringUsed.transform.position = m_pTransform.position;
			SpringUsed.Play(true);
		}
		SetActive(false);
		ActivateGameObject(false);
	}

	public override void DoOnTriggerEnter(GameObject other, int otherlayer)
	{
		base.DoOnTriggerEnter(other, otherlayer);
		if ((Network.peerType != 0 && !Network.isServer) || !m_bActive)
		{
			return;
		}
		int num = 1 << otherlayer;
		if ((num & (int)RoadLayer) != 0)
		{
			Ray ray = new Ray(m_pTransform.position, Vector3.down);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 5f, RoadLayer))
			{
				m_pTransform.position = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
				Quaternion quaternion = default(Quaternion);
				quaternion = Quaternion.FromToRotation(m_pTransform.rotation * Vector3.up, hitInfo.normal);
				m_pTransform.rotation = quaternion;
				m_bOnGround = true;
			}
		}
	}
}

using UnityEngine;

public class ParfumeBonusEffect : BonusEffect
{
	public float NewMass = 10000f;

	[HideInInspector]
	public GameObject AttackEffect;

	private GameObject _attackEffect;

	public GameObject BadAttackEffect;

	private GameObject _badAttackEffect;

	public GameObject UsedEffect;

	private GameObject _usedEffect;

	public GameObject BadUsedEffect;

	private GameObject _badUsedEffect;

	private bool m_bStinkParfume;

	private Transform m_pTransform;

	public Vector3 ParfumeOffset;

	[SerializeField]
	[HideInInspector]
	public float SpeedUp;

	[HideInInspector]
	[SerializeField]
	public float Acceleration;

	private Collider m_pCollider;

	public AudioSource ActivateParfumeSound;

	public AudioSource GoodParfumeSound;

	public AudioSource BadParfumeSound;

	public AudioSource BadParfumeCollisionSound;

	public bool StinkParfume
	{
		get
		{
			return m_bStinkParfume;
		}
	}

	public override void Start()
	{
		base.Start();
		InertiaVehicle = false;
		m_bStoppedByAnim = false;
		m_pCollider = base.collider;
		m_pTransform = base.transform;
		m_pCollider.enabled = false;
		_attackEffect = (GameObject)Object.Instantiate(AttackEffect);
		_badAttackEffect = (GameObject)Object.Instantiate(BadAttackEffect);
		_usedEffect = (GameObject)Object.Instantiate(UsedEffect);
		_badUsedEffect = (GameObject)Object.Instantiate(BadUsedEffect);
		_usedEffect.transform.position = m_pBonusEffectMgr.Target.Transform.position;
		_badUsedEffect.transform.position = m_pBonusEffectMgr.Target.Transform.position;
		_usedEffect.transform.parent = m_pBonusEffectMgr.Target.Transform;
		_badUsedEffect.transform.parent = m_pBonusEffectMgr.Target.Transform;
	}

	protected override void OnDestroy()
	{
		if (_attackEffect != null)
		{
			Object.Destroy(_attackEffect);
		}
		if (_badAttackEffect != null)
		{
			Object.Destroy(_badAttackEffect);
		}
		if (_usedEffect != null)
		{
			Object.Destroy(_usedEffect);
		}
		if (_badUsedEffect != null)
		{
			Object.Destroy(_badUsedEffect);
		}
	}

	public override void Update()
	{
		base.Update();
	}

	public override void SetDuration()
	{
		m_fCurrentDuration = EffectDuration + m_pBonusEffectMgr.Target.GetBonusMgr().GetBonusValue(EITEM.ITEM_PARFUME, EBonusCustomEffect.ATTRACT) * EffectDuration / 100f;
	}

	public override bool Activate()
	{
		base.Activate();
		Kart target = m_pBonusEffectMgr.Target;
		m_pTransform.parent = target.Transform;
		m_pTransform.localPosition = Vector3.zero;
		m_pCollider.enabled = true;
		m_bStinkParfume = m_pBonusEffectMgr.Target.GetBonusMgr().GetBonusValue(EITEM.ITEM_PARFUME, EBonusCustomEffect.REPULSE) != 0f;
		if (m_bStinkParfume)
		{
			_badAttackEffect.transform.position = target.Transform.position;
			_badAttackEffect.transform.parent = target.Transform;
			_badAttackEffect.transform.localPosition = target.Transform.rotation * ParfumeOffset;
			_badAttackEffect.particleSystem.Play();
			_badUsedEffect.particleSystem.Play();
		}
		else
		{
			_attackEffect.transform.position = target.Transform.position;
			_attackEffect.transform.parent = target.Transform;
			_attackEffect.transform.localPosition = target.Transform.rotation * ParfumeOffset;
			_attackEffect.particleSystem.Play();
			_usedEffect.particleSystem.Play();
			NapBonusEffect napBonusEffect = (NapBonusEffect)target.BonusMgr.GetBonusEffectMgr().GetBonusEffect(EBonusEffect.BONUSEFFECT_SLEPT);
			if ((bool)napBonusEffect && napBonusEffect.Activated)
			{
				napBonusEffect.Deactivate();
			}
		}
		if (target.GetControlType() == RcVehicle.ControlType.Human)
		{
			Camera.mainCamera.GetComponent<CamStateFollow>().bBoost = true;
			ParticleSystem componentInChildren = Camera.mainCamera.GetComponentInChildren<ParticleSystem>();
			if ((bool)componentInChildren)
			{
				componentInChildren.Play();
			}
		}
		float speedUpMs = SpeedUp + m_pBonusEffectMgr.Target.GetBonusMgr().GetBonusValue(EITEM.ITEM_PARFUME, EBonusCustomEffect.REPULSE) * SpeedUp / 100f;
		bool flag = target.GetControlType() == RcVehicle.ControlType.AI;
		int iQuantity = 1;
		if (!flag)
		{
			target.ParfumeBoost(10000, EffectDuration);
		}
		else
		{
			target.ParfumeBoost(speedUpMs, EffectDuration);
		}
		target.KartSound.PlayVoice(KartSound.EVoices.Good);
		if (target.OnBoost != null)
		{
			target.OnBoost();
		}
		target.Anim.LaunchSuccessAnim(true);
		ActivateParfumeSound.Play();
		if (!m_bStinkParfume)
		{
			GoodParfumeSound.Play();
		}
		else
		{
			BadParfumeSound.Play();
		}
		RcKinematicPhysic rcKinematicPhysic = (RcKinematicPhysic)target.GetVehiclePhysic();
		rcKinematicPhysic.m_fMass = NewMass;
		return true;
	}

	public override void Deactivate()
	{
		base.Deactivate();
		m_pTransform.parent = null;
		m_pCollider.enabled = false;
		if (m_bStinkParfume)
		{
			_badAttackEffect.particleSystem.Stop();
			_badAttackEffect.particleSystem.Clear();
			_badUsedEffect.particleSystem.Stop();
			_badUsedEffect.particleSystem.Clear();
		}
		else
		{
			_attackEffect.particleSystem.Stop();
			_attackEffect.particleSystem.Clear();
			_usedEffect.particleSystem.Stop();
			_usedEffect.particleSystem.Clear();
		}
		_badUsedEffect.particleSystem.Stop();
		if (!m_bStinkParfume)
		{
			GoodParfumeSound.Stop();
		}
		else
		{
			BadParfumeSound.Stop();
		}
		Kart target = m_pBonusEffectMgr.Target;
		if (target.GetControlType() == RcVehicle.ControlType.Human)
		{
			ParticleSystem componentInChildren = Camera.mainCamera.GetComponentInChildren<ParticleSystem>();
			if ((bool)componentInChildren)
			{
				componentInChildren.Stop();
			}
			Camera.mainCamera.GetComponent<CamStateFollow>().bBoost = false;
		}
		target.GetComponent<PlayerCarac>().ApplyWeight();
	}

	public void OnTriggerEnter(Collider other)
	{
		if (!Activated || !StinkParfume || ((Network.peerType == NetworkPeerType.Disconnected || !Network.isServer) && Network.peerType != 0))
		{
			return;
		}
		Kart componentInChildren = other.gameObject.GetComponentInChildren<Kart>();
		if (!(componentInChildren == null))
		{
			if (Network.isServer)
			{
				NetworkViewID viewID = other.gameObject.networkView.viewID;
				m_pBonusEffectMgr.Target.BonusMgr.networkView.RPC("OnStinkParfumeTriggerred", RPCMode.All, viewID);
			}
			else if (Network.peerType == NetworkPeerType.Disconnected)
			{
				m_pBonusEffectMgr.Target.BonusMgr.DoStinkParfumeTriggerred(componentInChildren);
			}
		}
	}
}

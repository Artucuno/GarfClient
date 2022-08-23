using UnityEngine;

public class MagicBonusEntity : BonusEntity
{
	private Vector3 _direction;

	public float Speed;

	public float GravityForward;

	protected float m_fCurrentSpeed;

	private GameObject _attackEffect;

	private GameObject _impactEffect;

	private GameObject _secondImpactEffect;

	private GameObject _wallImpactEffect;

	public GameObject AttackEffect;

	public GameObject ImpactEffect;

	public GameObject WallImpactEffect;

	private Kart _secondKart;

	private float _hideTimer;

	public float HideDelay = 0.8f;

	private bool _isHidden;

	public float DurationDelay = 5f;

	private float _durationTimer;

	protected Rigidbody m_pRigidBody;

	private Vector3 m_vVelocity;

	private float m_TrailTimer;

	private bool m_IgnoreCol;

	public AudioSource SoundLaunched;

	public AudioSource SoundTouchPlayer;

	public AudioSource SoundTouchWall;

	public AudioSource SoundTravel;

	public LayerMask VehicleLayer;

	public LayerMask ColLayer;

	public MagicBonusEntity()
	{
		m_eItem = EITEM.ITEM_MAGIC;
	}

	public override void Awake()
	{
		_attackEffect = (GameObject)Object.Instantiate(AttackEffect);
		_impactEffect = (GameObject)Object.Instantiate(ImpactEffect);
		_secondImpactEffect = (GameObject)Object.Instantiate(ImpactEffect);
		_wallImpactEffect = (GameObject)Object.Instantiate(WallImpactEffect);
		_attackEffect.transform.position = Vector3.zero;
		_attackEffect.transform.parent = base.transform;
		_impactEffect.transform.position = Vector3.zero;
		_impactEffect.transform.parent = base.transform.transform.parent;
		_secondImpactEffect.transform.position = Vector3.zero;
		_secondImpactEffect.transform.parent = base.transform.transform.parent;
		_wallImpactEffect.transform.position = Vector3.zero;
		_wallImpactEffect.transform.parent = base.transform.transform.parent;
		m_pRigidBody = GetComponent<Rigidbody>();
		m_fCurrentSpeed = 0f;
		m_bSynchronizePosition = true;
		base.Awake();
		ActivateGameObject(false);
	}

	protected override void OnDestroy()
	{
		if (_attackEffect != null)
		{
			Object.Destroy(_attackEffect.gameObject);
		}
		if (_impactEffect != null)
		{
			Object.Destroy(_impactEffect.gameObject);
		}
		if (_secondImpactEffect != null)
		{
			Object.Destroy(_secondImpactEffect.gameObject);
		}
		if (_wallImpactEffect != null)
		{
			Object.Destroy(_wallImpactEffect.gameObject);
		}
	}

	public virtual void FixedUpdate()
	{
		if ((Network.peerType == NetworkPeerType.Disconnected || Network.isServer) && m_pRigidBody.velocity.z != 0f)
		{
			Vector3 vVelocity = m_vVelocity;
			vVelocity.y = m_pRigidBody.velocity.y;
			m_pRigidBody.velocity = vVelocity;
			m_pRigidBody.velocity = m_fCurrentSpeed * m_pRigidBody.velocity.normalized;
			m_pRigidBody.AddForce(new Vector3(0f, 0f - GravityForward, 0f), ForceMode.Impulse);
		}
	}

	public override void Update()
	{
		base.Update();
		if (!m_bActive)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if ((bool)_attackEffect && !_attackEffect.particleSystem.isPlaying && m_eState == BonusState.BONUS_LAUNCHED)
		{
			m_TrailTimer += deltaTime;
			if (m_TrailTimer > 0.05f)
			{
				m_TrailTimer = 0f;
				_attackEffect.particleSystem.Play();
			}
		}
		if (_attackEffect.activeSelf)
		{
			_durationTimer += deltaTime;
			if (_durationTimer > DurationDelay)
			{
				DeactivateGameObject();
			}
		}
		else if (!_isHidden && _impactEffect.particleSystem.isStopped)
		{
			DeactivateGameObject();
		}
		if (_isHidden)
		{
			_hideTimer += deltaTime;
			if (_hideTimer > HideDelay)
			{
				ActivateVehicle(base.Launcher, true);
				ActivateVehicle(_secondKart, true);
				_hideTimer = 0f;
				_isHidden = false;
			}
		}
	}

	public new void Launch()
	{
		base.Launch();
		m_vVelocity = Vector3.zero;
		SetActive(true);
		if (Network.peerType == NetworkPeerType.Disconnected || Network.isServer)
		{
			m_pTransform.position = base.Launcher.Transform.position;
			m_pTransform.position += base.Launcher.Transform.rotation * new Vector3(0f, 0.5f, 3f);
			m_pTransform.rotation = base.Launcher.Transform.rotation;
		}
		m_pRigidBody.isKinematic = false;
		m_pCollider.isTrigger = false;
		_direction = base.Launcher.transform.forward;
		_direction.Normalize();
		_hideTimer = 0f;
		_isHidden = false;
		_durationTimer = 0f;
		if ((bool)SoundLaunched && (bool)SoundTravel)
		{
			SoundLaunched.Play();
			SoundTravel.Play();
		}
	}

	public void DeactivateGameObject()
	{
		if (m_IgnoreCol)
		{
			if (m_pCollider.enabled && m_pLauncher.transform.parent.collider.enabled)
			{
				Physics.IgnoreCollision(m_pCollider, m_pLauncher.transform.parent.collider, false);
			}
			m_IgnoreCol = false;
		}
		ActivateGameObject(false);
		m_bActive = false;
	}

	public override void SetActive(bool pActive)
	{
		if (!pActive)
		{
			_attackEffect.particleSystem.Stop();
			_attackEffect.SetActive(false);
			if (m_IgnoreCol && m_pCollider.gameObject.activeSelf && m_pLauncher.transform.parent.collider.gameObject.activeSelf)
			{
				Physics.IgnoreCollision(m_pCollider, m_pLauncher.transform.parent.collider, false);
				m_IgnoreCol = false;
			}
			m_pCollider.enabled = pActive;
			return;
		}
		_attackEffect.SetActive(true);
		ActivateGameObject(true);
		_impactEffect.particleSystem.Stop();
		_secondImpactEffect.particleSystem.Stop();
		_wallImpactEffect.particleSystem.Stop();
		ComputeBaseSpeed();
		m_vVelocity = base.LaunchHorizontalDirection * m_fCurrentSpeed;
		m_pRigidBody.velocity = m_vVelocity;
		m_bActive = true;
		m_IgnoreCol = true;
		m_pCollider.enabled = pActive;
		if (m_pCollider.gameObject.activeSelf && m_pLauncher.transform.parent.collider.gameObject.activeSelf)
		{
			Physics.IgnoreCollision(m_pCollider, m_pLauncher.transform.parent.collider, true);
		}
	}

	public override void DoOnTriggerEnter(GameObject pOther, int otherlayer)
	{
		int num = 1 << otherlayer;
		if ((num & (int)VehicleLayer) != 0)
		{
			_secondKart = pOther.GetComponentInChildren<Kart>();
			if (_secondKart != null)
			{
				ParfumeBonusEffect parfumeBonusEffect = (ParfumeBonusEffect)_secondKart.GetBonusMgr().GetBonusEffectMgr().GetBonusEffect(EBonusEffect.BONUSEFFECT_ATTRACTED);
				if (!parfumeBonusEffect.Activated || parfumeBonusEffect.StinkParfume)
				{
					ActivateBonusEffect();
				}
				_secondImpactEffect.transform.position = _secondKart.transform.position;
				_impactEffect.transform.position = base.Launcher.transform.position;
				_impactEffect.particleSystem.Play();
				_secondImpactEffect.transform.position = _secondKart.transform.position;
				_secondImpactEffect.particleSystem.Play();
				if ((bool)SoundTouchPlayer && (bool)SoundTravel)
				{
					SoundTouchPlayer.Play();
					SoundTravel.Stop();
				}
				SetActive(false);
			}
		}
		else if ((num & (int)ColLayer) != 0)
		{
			_wallImpactEffect.transform.position = m_pTransform.position;
			_wallImpactEffect.particleSystem.Play();
			if ((bool)SoundTouchWall && (bool)SoundTravel)
			{
				SoundTouchWall.Play();
				SoundTravel.Stop();
			}
			SetActive(false);
		}
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (Network.peerType != 0 && !Network.isServer)
		{
			return;
		}
		int num = 1 << collision.gameObject.layer;
		if ((num & (int)VehicleLayer) != 0)
		{
			if (collision.gameObject != null)
			{
				OnTriggerEnter(collision.collider);
			}
		}
		else if ((num & (int)ColLayer) != 0)
		{
			PerformCollision(collision);
		}
	}

	public void OnCollisionStay(Collision collision)
	{
		if (Network.peerType == NetworkPeerType.Disconnected || Network.isServer)
		{
			int num = 1 << collision.gameObject.layer;
			if ((num & (int)ColLayer) != 0)
			{
				PerformCollision(collision);
			}
		}
	}

	public void PerformCollision(Collision collision)
	{
		if (collision.collider != null && collision.collider.isTrigger)
		{
			return;
		}
		for (int i = 0; i < collision.contacts.Length; i++)
		{
			ContactPoint contactPoint = collision.contacts[i];
			if (contactPoint.normal.y < 0.5f)
			{
				DestroyByWall();
				break;
			}
		}
	}

	public void DestroyByWall()
	{
		if (m_bActive)
		{
			if (Network.isServer)
			{
				m_pNetworkView.RPC("OnNetworkDestroy", RPCMode.All);
			}
			else if (Network.peerType == NetworkPeerType.Disconnected)
			{
				DoDestroyByWall();
			}
		}
	}

	public void DoDestroyByWall()
	{
		_wallImpactEffect.transform.position = m_pTransform.position;
		_wallImpactEffect.particleSystem.Play();
		if ((bool)SoundTouchWall && (bool)SoundTravel)
		{
			SoundTouchWall.Play();
			SoundTravel.Stop();
		}
		SetActive(false);
	}

	public void ActivateBonusEffect()
	{
		MagicBonusEffect magicBonusEffect = (MagicBonusEffect)base.Launcher.GetBonusMgr().GetBonusEffectMgr().GetBonusEffect(EBonusEffect.BONUSEFFECT_TELEPORTED);
		magicBonusEffect.FirstKart = base.Launcher;
		magicBonusEffect.SecondKart = _secondKart;
		base.Launcher.GetBonusMgr().GetBonusEffectMgr().ActivateBonusEffect(EBonusEffect.BONUSEFFECT_TELEPORTED);
		_isHidden = true;
		_hideTimer = 0f;
		ActivateVehicle(base.Launcher, false);
		ActivateVehicle(_secondKart, false);
	}

	public void ComputeBaseSpeed()
	{
		float num = Speed / 3.6f;
		m_fCurrentSpeed = num + m_pLauncher.GetBonusMgr().GetBonusValue(EITEM.ITEM_MAGIC, EBonusCustomEffect.SPEED) * num / 100f;
	}

	public void ActivateVehicle(Kart pKart, bool bActivate)
	{
		pKart.GetVehiclePhysic().Enable = bActivate;
		pKart.SetLocked(!bActivate);
		pKart.GetComponent<RcKinematicPhysic>().m_pVehicleMesh.gameObject.SetActive(bActivate);
		if (bActivate)
		{
			pKart.KartSound.PlaySound(0);
		}
	}
}

using UnityEngine;

public class PieBonusEntity : MalusBonusEntity
{
	public float SpeedForward;

	public float SpeedBackward;

	public Mesh State0;

	public Mesh State1;

	public Material MatState0;

	public Material MatState1;

	public float GravityForward;

	public float GravityBackward;

	public float TimeNoPhysic;

	protected Rigidbody m_pRigidBody;

	private bool m_bIsOnGround;

	public MeshFilter cMeshFilter;

	public MeshRenderer cMeshRenderer;

	private float m_fTimerNoPhysic;

	private float m_fTimerSecure;

	private Vector3 m_vVelocity;

	protected float m_fCurrentSpeed;

	protected Vector3 m_Direction;

	public GameObject DestructionParticle;

	protected GameObject m_pDestructionParticle;

	protected Kart m_pHumanPlayer;

	public RcMultiPath IdealPath;

	protected MultiPathPosition m_PathPosition;

	protected int _index;

	public Animation Anim;

	public ParticleSystem TrailParticle;

	protected float m_TrailTimer;

	public AudioSource SoundLaunch;

	public AudioSource SoundTravel;

	protected AudioSource SoundSplatch;

	public int Index
	{
		get
		{
			return _index;
		}
		set
		{
			_index = value;
		}
	}

	public PieBonusEntity()
	{
		m_bIsOnGround = false;
		m_fTimerNoPhysic = 0f;
		m_fTimerSecure = 0f;
		m_fCurrentSpeed = 0f;
		m_Direction = Vector3.zero;
		DestructionParticle = null;
		m_pDestructionParticle = null;
		m_eItem = EITEM.ITEM_PIE;
		IdealPath = null;
		m_PathPosition = MultiPathPosition.UNDEFINED_MP_POS;
	}

	public Vector2 GetFlatVelocity()
	{
		return new Vector2(m_vVelocity.x, m_vVelocity.z);
	}

	public override void Awake()
	{
		base.Awake();
		m_pRigidBody = GetComponent<Rigidbody>();
		m_pDestructionParticle = (GameObject)Object.Instantiate(DestructionParticle);
		m_pDestructionParticle.transform.parent = m_pTransform.transform.parent;
		m_bSynchronizePosition = true;
	}

	public override void Start()
	{
		SoundSplatch = m_pDestructionParticle.GetComponent<AudioSource>();
		m_pHumanPlayer = Singleton<GameManager>.Instance.GameMode.GetHumanKart();
	}

	public override void Update()
	{
		base.Update();
		float deltaTime = Time.deltaTime;
		if (m_eState > BonusState.BONUS_LAUNCHED && m_fTimerNoPhysic < TimeNoPhysic)
		{
			m_fTimerNoPhysic += deltaTime;
			if (m_fTimerNoPhysic > TimeNoPhysic)
			{
				Physics.IgnoreCollision(m_pCollider, m_pLauncher.Transform.parent.collider, false);
			}
		}
		if (m_eState == BonusState.BONUS_ANIMLAUNCHED && m_bBehind)
		{
			m_fTimerSecure += deltaTime;
			if (m_fTimerSecure > 5f)
			{
				NetDestroy();
			}
		}
		if ((bool)TrailParticle && !TrailParticle.isPlaying && m_eState == BonusState.BONUS_LAUNCHED && !m_bBehind)
		{
			m_TrailTimer += deltaTime;
			if (m_TrailTimer > 0.05f)
			{
				m_TrailTimer = 0f;
				TrailParticle.Clear();
				TrailParticle.Play();
			}
		}
		if (CheckHUDRadar())
		{
			IdealPath.UpdateMPPosition(ref m_PathPosition, m_pTransform.position, 0, 0, false);
			MultiPathPosition _mpPosition = MultiPathPosition.UNDEFINED_MP_POS;
			IdealPath.UpdateMPPosition(ref _mpPosition, m_pHumanPlayer.Transform.position, 0, 0, false);
			float distToEndLine = IdealPath.GetDistToEndLine(m_PathPosition);
			float distToEndLine2 = IdealPath.GetDistToEndLine(_mpPosition);
			Vector3 vector = m_pTransform.position - m_pHumanPlayer.Transform.position;
			float num = distToEndLine - distToEndLine2;
			if (num >= 0f && num <= 50f)
			{
				float num2 = Vector3.Dot(vector, m_pHumanPlayer.Transform.forward);
				Vector3 vector2 = m_pHumanPlayer.Transform.position + m_pHumanPlayer.Transform.forward * num2;
				float magnitude = (m_pTransform.position - vector2).magnitude;
				magnitude *= AngleDir(m_pHumanPlayer.Transform.forward, vector, m_pHumanPlayer.Transform.up);
				UpdateHudRadar(magnitude, num);
			}
			else
			{
				DisableHudRadar();
			}
		}
		else
		{
			DisableHudRadar();
		}
	}

	protected virtual void DisableHudRadar()
	{
		if (Singleton<GameManager>.Instance.GameMode != null && (bool)Singleton<GameManager>.Instance.GameMode.Hud && (bool)Singleton<GameManager>.Instance.GameMode.Hud.HudRadarComp)
		{
			Singleton<GameManager>.Instance.GameMode.Hud.HudRadarComp.Pies[_index].enabled = false;
		}
	}

	protected virtual void UpdateHudRadar(float pHorizontalDist, float pDistance)
	{
		if (Singleton<GameManager>.Instance.GameMode != null)
		{
			Singleton<GameManager>.Instance.GameMode.Hud.HudRadarComp.UpdatePie(_index, pHorizontalDist, pDistance);
		}
	}

	protected virtual bool CheckHUDRadar()
	{
		return m_bActive && !m_bBehind && IdealPath != null && !cMeshRenderer.isVisible;
	}

	public override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);
		if (Network.peerType != 0 && !Network.isServer)
		{
			return;
		}
		if (!m_bBehind)
		{
			PerformCollision(collision);
			return;
		}
		int num = 1 << collision.gameObject.layer;
		if (m_eState < BonusState.BONUS_ONGROUND && (num & (int)LayerStick) != 0)
		{
			StickOnGround(collision.contacts[0].normal);
		}
	}

	public void OnCollisionStay(Collision collision)
	{
		if ((Network.peerType == NetworkPeerType.Disconnected || Network.isServer) && !m_bBehind)
		{
			PerformCollision(collision);
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
				DestroyByCollision();
				break;
			}
		}
	}

	public void DestroyByCollision()
	{
		if (m_bActive)
		{
			NetDestroy();
		}
	}

	public override void DoOnTriggerEnter(GameObject other, int otherlayer)
	{
		base.DoOnTriggerEnter(other, otherlayer);
	}

	public virtual void FixedUpdate()
	{
		if (Network.peerType != 0 && !Network.isServer)
		{
			return;
		}
		if (!m_bBehind)
		{
			if (m_pRigidBody.velocity.z != 0f)
			{
				Vector3 vVelocity = m_vVelocity;
				vVelocity.y = m_pRigidBody.velocity.y;
				m_pRigidBody.velocity = vVelocity;
				m_pRigidBody.velocity = m_fCurrentSpeed * m_pRigidBody.velocity.normalized;
				m_pRigidBody.AddForce(new Vector3(0f, 0f - GravityForward, 0f), ForceMode.Impulse);
			}
		}
		else
		{
			m_pRigidBody.AddForce(new Vector3(0f, 0f - GravityBackward, 0f), ForceMode.Impulse);
		}
	}

	private float AngleDir(Vector3 pForward, Vector3 pTargetDir, Vector3 pUp)
	{
		Vector3 lhs = Vector3.Cross(pForward, pTargetDir);
		float num = Vector3.Dot(lhs, pUp);
		if ((double)num > 0.0)
		{
			return 1f;
		}
		if ((double)num < 0.0)
		{
			return -1f;
		}
		return 0f;
	}

	public override void SetActive(bool _Active)
	{
		if (_Active)
		{
			if (Network.peerType == NetworkPeerType.Disconnected || Network.isServer)
			{
				if (!m_bBehind)
				{
					ComputeBaseSpeed();
					m_vVelocity = base.LaunchHorizontalDirection * m_fCurrentSpeed;
					m_pRigidBody.velocity = m_vVelocity;
				}
				else
				{
					float num = SpeedBackward / 3.6f;
					m_pRigidBody.AddForce((-m_pLauncher.Transform.parent.forward + m_pLauncher.Transform.parent.up).normalized * num, ForceMode.VelocityChange);
				}
			}
		}
		else
		{
			DisableHudRadar();
		}
		base.SetActive(_Active);
		if (_Active)
		{
			Physics.IgnoreCollision(m_pCollider, m_pLauncher.Transform.parent.collider, true);
		}
	}

	public override void DoDestroy()
	{
		base.DoDestroy();
		m_pDestructionParticle.transform.position = m_pTransform.position;
		m_pDestructionParticle.particleSystem.Play();
		if ((bool)SoundTravel && (bool)SoundSplatch)
		{
			SoundTravel.Stop();
			SoundSplatch.Play();
		}
		if ((bool)TrailParticle)
		{
			TrailParticle.Stop();
		}
		SetActive(false);
	}

	public override void ActivateBonusEffect(Kart _Kart)
	{
		base.ActivateBonusEffect(_Kart);
		if (!m_bBehind)
		{
			_Kart.GetBonusMgr().GetBonusEffectMgr().GetBonusEffect(EBonusEffect.BONUSEFFECT_UPSIDE_DOWN)
				.BonusEffectDirection = m_eImpactDirection;
			_Kart.GetBonusMgr().GetBonusEffectMgr().ActivateBonusEffect(EBonusEffect.BONUSEFFECT_UPSIDE_DOWN);
		}
		else
		{
			_Kart.GetBonusMgr().GetBonusEffectMgr().ActivateBonusEffect(EBonusEffect.BONUSEFFECT_SPIN);
		}
		if (_Kart != m_pLauncher)
		{
			m_pLauncher.KartSound.PlayVoice(KartSound.EVoices.Good);
			m_pLauncher.Anim.LaunchSuccessAnim(true);
		}
	}

	public override void Launch()
	{
		base.Launch();
	}

	public void Launch(bool _Behind)
	{
		Launch(m_pLauncher.Transform.position, m_pLauncher.Transform.parent.rotation, _Behind);
	}

	public void NetLaunch(NetworkViewID launcherViewID, bool _Behind)
	{
		m_pNetworkView.RPC("Launch", RPCMode.All, launcherViewID, _Behind);
	}

	public void Launch(Vector3 position, Quaternion rotation, bool _Behind)
	{
		m_bBehind = _Behind;
		m_bIsOnGround = false;
		cMeshFilter.mesh = State0;
		cMeshRenderer.material = MatState0;
		m_pCollider.isTrigger = false;
		base.gameObject.layer = m_pLayerBonus;
		m_fTimerNoPhysic = 0f;
		m_fTimerSecure = 0f;
		Launch();
		m_pTransform.parent.rotation = Quaternion.identity;
		m_pTransform.rotation = Quaternion.identity;
		m_pRigidBody.isKinematic = false;
		m_pRigidBody.velocity = Vector3.zero;
		Vector3 zero = Vector3.zero;
		if (Network.peerType == NetworkPeerType.Disconnected || Network.isServer)
		{
			if (m_bBehind)
			{
				zero = position + rotation * new Vector3(0f, 1f, -0.5f);
			}
			else
			{
				m_Direction = position + rotation * new Vector3(0f, 0.5f, 3f);
				zero = m_Direction;
				m_vVelocity = Vector3.zero;
			}
			m_pTransform.parent.position = zero;
			m_pRigidBody.position = zero;
		}
		if ((bool)SoundLaunch && (bool)SoundTravel)
		{
			SoundLaunch.Play();
			SoundTravel.Play();
		}
		if ((bool)Anim && !_Behind)
		{
			Anim.Play("PieRun");
		}
		else if ((bool)Anim)
		{
			Anim.Stop();
		}
		if ((bool)TrailParticle)
		{
			TrailParticle.Stop();
			TrailParticle.Clear();
		}
	}

	public override bool IsOnGround()
	{
		return m_bIsOnGround;
	}

	public override void LaunchAnimFinished()
	{
		base.LaunchAnimFinished();
	}

	public virtual void StickOnGround(Vector3 normal)
	{
		if (m_bActive)
		{
			if (Network.peerType != 0 && m_pNetworkView != null)
			{
				m_pNetworkView.RPC("OnStickOnGround", RPCMode.All, normal);
			}
			else if (Network.peerType == NetworkPeerType.Disconnected || m_pNetworkView == null)
			{
				DoStickOnGround(normal);
			}
		}
	}

	public virtual void DoStickOnGround(Vector3 normal)
	{
		m_pTransform.rotation = Quaternion.identity;
		Vector3 right = m_pTransform.right;
		Vector3 up = m_pTransform.up;
		Vector3 forward = m_pTransform.forward;
		up = normal;
		right = Vector3.Cross(up, forward);
		right.Normalize();
		forward = Vector3.Cross(right, up);
		forward.Normalize();
		m_pTransform.right = right;
		m_pTransform.up = up;
		m_pTransform.forward = forward;
		cMeshFilter.mesh = State1;
		cMeshRenderer.material = MatState1;
		m_bIsOnGround = true;
		m_pRigidBody.isKinematic = true;
		base.collider.isTrigger = true;
		base.gameObject.layer = m_pLayerBonus;
		m_pDestructionParticle.transform.position = m_pTransform.position;
		m_pDestructionParticle.particleSystem.Play();
		if ((bool)SoundTravel && (bool)SoundSplatch)
		{
			SoundTravel.Stop();
			SoundSplatch.Play();
		}
	}

	public void ComputeBaseSpeed()
	{
		float num = SpeedForward / 3.6f;
		m_fCurrentSpeed = num + m_pLauncher.GetBonusMgr().GetBonusValue(EITEM.ITEM_PIE, EBonusCustomEffect.SPEED) * num / 100f;
	}

	public override void PerformBonusCollision(BonusEntity _Bonus)
	{
		if (_Bonus != null && _Bonus is SpringBonusEntity)
		{
			m_eState = BonusState.BONUS_TRIGGERED;
			_Bonus.NetDestroy();
		}
		else
		{
			base.PerformBonusCollision(_Bonus);
		}
	}
}

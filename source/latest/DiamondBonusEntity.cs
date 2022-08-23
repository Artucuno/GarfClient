using System;
using UnityEngine;

public class DiamondBonusEntity : BonusEntity
{
	public enum E_STATE
	{
		NONE,
		LAUNCH,
		ATTACHED,
		IDLE,
		EXPLODE,
		END
	}

	public float AngularVelocity = 2f;

	public float MaxScale = 2f;

	public float ScaleFactor = 1f;

	public float Angle = 15f;

	public float Range = 30f;

	private Vector3 _initialSpeed = Vector3.zero;

	private float _initialTime;

	public float ProjectileSpeedFactor = 4f;

	private float _currentAngle;

	private Vector3 _currentDir;

	private float _currentRange;

	private float _currentSpeedFactor;

	private float m_fTrailTimer;

	public float MinDistance;

	public float SpeedFactor = 1f;

	private float m_fTimerNoPhysic;

	public float TimerNoPhysic;

	private float _currentSpeed;

	private Vector3 _direction = Vector3.zero;

	private Kart _target;

	private bool _isAttached;

	private E_STATE _state;

	public LayerMask LayerVehicle;

	public LayerMask LayerRoad;

	public LayerMask LayerBonus;

	public LayerMask LayerWall;

	public int JumpNumber = 4;

	private int m_iKartJumpNumber;

	private int _jumpCounter;

	private Collider _launcherCollider;

	public float LifeTime = 10f;

	private float LifeTimeWithHat;

	private float _lifeTimeTimer;

	private Rigidbody _rigidBody;

	private GameObject _explosionEffect;

	private GameObject _attackEffect;

	private Transform _parent;

	public GameObject DiamondGO;

	private Transform _diamond;

	private Collider _explosionCollider;

	public GameObject AttackEffect;

	public GameObject ExplosionEffect;

	public GameObject ExplosionAnimationGO;

	private Animation _explosionAnimation;

	public GameObject ParticleTrail;

	private bool m_bVictorySound;

	private float m_fColliderDiameter;

	public AudioSource SoundLaunch;

	public AudioSource SoundExplode;

	public AudioSource SoundGlued;

	public AudioSource SoundUnglued;

	public AudioSource SoundLand;

	public AudioSource SoundCountdown;

	public DiamondBonusEntity()
	{
		m_eItem = EITEM.ITEM_DIAMOND;
	}

	public override void Awake()
	{
		_rigidBody = GetComponent<Rigidbody>();
		_attackEffect = (GameObject)UnityEngine.Object.Instantiate(AttackEffect);
		_attackEffect.transform.parent = base.transform;
		_attackEffect.transform.position = Vector3.zero;
		_explosionEffect = (GameObject)UnityEngine.Object.Instantiate(ExplosionEffect);
		_explosionEffect.transform.parent = base.transform;
		_explosionEffect.transform.position = Vector3.zero;
		_explosionCollider = ExplosionAnimationGO.collider;
		_explosionAnimation = ExplosionAnimationGO.animation;
		_diamond = DiamondGO.transform;
		m_bSynchronizePosition = true;
		m_fColliderDiameter = ((SphereCollider)base.collider).radius * 2f;
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
		_parent = base.transform.parent.parent;
	}

	public void NetLaunch(NetworkViewID launcherViewID, bool pBehind)
	{
		m_pNetworkView.RPC("Launch", RPCMode.All, launcherViewID, pBehind);
	}

	public void Launch(bool pBehind)
	{
		m_bBehind = pBehind;
		_lifeTimeTimer = 0f;
		ActivateGameObject(true);
		SetActive(true);
		Launch();
	}

	public override void Launch()
	{
		base.Launch();
		m_bSynchronizePosition = true;
		ParticleTrail.gameObject.SetActive(false);
		ParticleTrail.particleSystem.Stop();
		ParticleTrail.particleSystem.Clear();
		_explosionEffect.particleSystem.Stop();
		_explosionEffect.particleSystem.Clear();
		_diamond.renderer.enabled = true;
		if (!_attackEffect.particleSystem.isPlaying)
		{
			_attackEffect.particleSystem.Play();
		}
		if (Network.peerType == NetworkPeerType.Disconnected || Network.isServer)
		{
			m_pTransform.position = base.Launcher.Transform.position + base.Launcher.Transform.up * 0.2f;
			m_pTransform.position = new Vector3(m_pTransform.position.x, m_pTransform.position.y + 0.54f, m_pTransform.position.z);
		}
		_launcherCollider = base.Launcher.Transform.parent.collider;
		m_pCollider.enabled = true;
		_launcherCollider.enabled = true;
		_explosionCollider.enabled = false;
		if (m_pCollider.gameObject.activeSelf && _launcherCollider.gameObject.activeSelf)
		{
			Physics.IgnoreCollision(m_pCollider, _launcherCollider, true);
		}
		_jumpCounter = 0;
		_isAttached = false;
		_currentDir = Vector3.zero;
		_currentSpeedFactor = ProjectileSpeedFactor;
		if (_target != null)
		{
			UnregisterEvents();
			_target = null;
		}
		_state = E_STATE.LAUNCH;
		if (Network.peerType == NetworkPeerType.Disconnected || Network.isServer)
		{
			if (m_bBehind)
			{
				Vector3 pDir = -base.LaunchHorizontalDirection + base.Launcher.transform.up;
				CalculateInitialSpeed(pDir, 7.5f, 0.08f);
			}
			else
			{
				Vector3 pDir2 = base.LaunchHorizontalDirection + base.Launcher.transform.up;
				CalculateInitialSpeed(pDir2, Angle, Range);
			}
		}
		Vector3 vector = new Vector3(base.Launcher.transform.forward.x, 0f, base.Launcher.transform.forward.z);
		_rigidBody.angularVelocity = vector * AngularVelocity;
		_rigidBody.velocity = Vector3.zero;
		if ((bool)SoundLaunch && (bool)SoundCountdown)
		{
			SoundLaunch.Play();
			SoundCountdown.Play();
		}
		m_bVictorySound = false;
		LifeTimeWithHat = LifeTime + m_pLauncher.GetBonusMgr().GetBonusValue(EITEM.ITEM_DIAMOND, EBonusCustomEffect.EXPLOSION_RADIUS) * LifeTime / 100f;
	}

	public void CalculateInitialSpeed(Vector3 pDir, float pAngle, float pRange)
	{
		_currentAngle = pAngle;
		_currentRange = pRange;
		_initialTime = Time.time;
		pDir.Normalize();
		float num = (0f - _currentRange) * Physics.gravity.y;
		float num2 = Mathf.Abs(2f * Mathf.Cos(pAngle * ((float)Math.PI / 180f)) * Mathf.Sin(pAngle * ((float)Math.PI / 180f)));
		_initialSpeed = Mathf.Sqrt(num / num2) * pDir;
	}

	private void PerformLaunch(float pSpeedFactor, float vDeltaTime)
	{
		float num = Time.time - _initialTime;
		float x = _initialSpeed.x * Mathf.Cos(_currentAngle * ((float)Math.PI / 180f));
		float y = _initialSpeed.y * Mathf.Sin(_currentAngle * ((float)Math.PI / 180f)) + Physics.gravity.y * num * num / 2f;
		float z = _initialSpeed.z * Mathf.Cos(_currentAngle * ((float)Math.PI / 180f));
		_currentDir = new Vector3(x, y, z) * vDeltaTime * pSpeedFactor;
		float magnitude = _currentDir.magnitude;
		if (magnitude > m_fColliderDiameter)
		{
			Vector3 position = base.transform.position;
			RaycastHit hitInfo;
			if (Physics.Raycast(position, _currentDir, out hitInfo, magnitude, ~(int)LayerRoad))
			{
				_currentDir = _currentDir.normalized * hitInfo.distance;
			}
		}
		base.transform.position += _currentDir;
	}

	public void FixedUpdate()
	{
		if (!m_bActive)
		{
			return;
		}
		float fixedDeltaTime = Time.fixedDeltaTime;
		if (_state == E_STATE.LAUNCH && (Network.peerType == NetworkPeerType.Disconnected || Network.isServer))
		{
			if (m_bBehind)
			{
				PerformLaunch(5f, fixedDeltaTime);
			}
			else
			{
				PerformLaunch(_currentSpeedFactor, fixedDeltaTime);
			}
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
		if (_state == E_STATE.LAUNCH)
		{
			if (!m_bBehind && (bool)ParticleTrail && !ParticleTrail.particleSystem.isPlaying)
			{
				m_fTrailTimer += deltaTime;
				if (m_fTrailTimer > 0.1f)
				{
					m_fTrailTimer = 0f;
					ParticleTrail.gameObject.SetActive(true);
					ParticleTrail.particleSystem.Play();
				}
			}
		}
		else if (_state == E_STATE.EXPLODE && !_explosionAnimation.isPlaying)
		{
			_explosionCollider.enabled = false;
			_state = E_STATE.END;
		}
		else if (_state == E_STATE.END)
		{
			if (!_explosionEffect.particleSystem.isPlaying)
			{
				SetActive(false);
			}
		}
		else if (_state == E_STATE.IDLE && m_fTimerNoPhysic < TimerNoPhysic)
		{
			m_fTimerNoPhysic += deltaTime;
			if (m_fTimerNoPhysic > TimerNoPhysic && _explosionCollider.gameObject.activeSelf && _launcherCollider.gameObject.activeSelf)
			{
				Physics.IgnoreCollision(_explosionCollider, _launcherCollider, false);
			}
		}
		if ((Network.peerType == NetworkPeerType.Disconnected || Network.isServer) && _state != E_STATE.EXPLODE && _state != E_STATE.END)
		{
			_lifeTimeTimer += deltaTime;
			if (_lifeTimeTimer > LifeTimeWithHat && !_explosionAnimation.isPlaying)
			{
				if (Network.isServer)
				{
					m_pNetworkView.RPC("Explode", RPCMode.All);
				}
				else
				{
					Explode();
				}
			}
		}
		if (_target == null)
		{
			_diamond.localScale += Vector3.one * ScaleFactor * deltaTime;
			if (_diamond.localScale.x > MaxScale)
			{
				_diamond.localScale = Vector3.one * MaxScale;
			}
		}
		if (!_isAttached && _target != null)
		{
			if (Network.peerType != 0 && !Network.isServer)
			{
				return;
			}
			_direction = _target.transform.position + Vector3.up * 0.25f - m_pTransform.position;
			if (!(_direction != Vector3.zero))
			{
				return;
			}
			if (_direction.sqrMagnitude < MinDistance)
			{
				AttachToTarget();
			}
			else if (_rigidBody.gameObject.activeInHierarchy)
			{
				_direction.Normalize();
				_rigidBody.velocity = _direction * _currentSpeed;
				_diamond.localScale -= Vector3.one * ScaleFactor * deltaTime;
				if (_diamond.localScale.x < 1f)
				{
					_diamond.localScale = Vector3.one;
				}
			}
		}
		else if (_target != null && _jumpCounter >= m_iKartJumpNumber)
		{
			LoseTargetByJump();
		}
	}

	public void AttachToTarget()
	{
		if (m_bActive)
		{
			if (Network.peerType != 0 && m_pNetworkView != null)
			{
				m_pNetworkView.RPC("OnAttachToTarget", RPCMode.All);
			}
			else if (Network.peerType == NetworkPeerType.Disconnected || m_pNetworkView == null)
			{
				DoAttachToTarget();
			}
		}
	}

	public void DoAttachToTarget()
	{
		m_bSynchronizePosition = false;
		GameObject gameObject = _target.transform.parent.gameObject;
		KartCustom componentInChildren = gameObject.GetComponentInChildren<KartCustom>();
		if (componentInChildren != null)
		{
			Transform parent = componentInChildren.transform.GetChild(0).GetChild(0).transform;
			if (_parent != null)
			{
				_parent.parent = parent;
				base.transform.localPosition = Vector3.zero;
				_parent.localPosition = new Vector3(-0.2f, 0.25f, 0f);
				_rigidBody.velocity = Vector3.zero;
			}
			_isAttached = true;
			m_iKartJumpNumber = (int)((float)JumpNumber + _target.GetBonusMgr().GetBonusValue(EITEM.ITEM_DIAMOND, EBonusCustomEffect.STICK));
			_diamond.localScale = Vector3.one;
			_rigidBody.transform.rotation = Quaternion.identity;
			base.transform.rotation = Quaternion.identity;
			base.transform.Rotate(Vector3.left * 90f);
			_rigidBody.angularVelocity = Vector3.up * AngularVelocity;
			_jumpCounter = 0;
		}
	}

	private void RegisterEvents()
	{
		Kart target = _target;
		target.OnJump = (Action)Delegate.Combine(target.OnJump, new Action(KartJumped));
		Kart target2 = _target;
		target2.OnBeSwaped = (Action<Kart>)Delegate.Combine(target2.OnBeSwaped, new Action<Kart>(KartBeSwaped));
		Kart target3 = _target;
		target3.OnTeleported = (Action)Delegate.Combine(target3.OnTeleported, new Action(LoseTarget));
		Kart target4 = _target;
		target4.OnSpringJump = (Action)Delegate.Combine(target4.OnSpringJump, new Action(LoseTarget));
		KartBonusMgr bonusMgr = _target.GetBonusMgr();
		bonusMgr.OnLaunchBonus = (Action<EITEM, bool>)Delegate.Combine(bonusMgr.OnLaunchBonus, new Action<EITEM, bool>(KartLaunchBonus));
		Kart target5 = _target;
		target5.OnBoost = (Action)Delegate.Combine(target5.OnBoost, new Action(LoseTarget));
		Kart target6 = _target;
		target6.OnRespawn = (Action)Delegate.Combine(target6.OnRespawn, new Action(LoseTarget));
	}

	private void UnregisterEvents()
	{
		Kart target = _target;
		target.OnJump = (Action)Delegate.Remove(target.OnJump, new Action(KartJumped));
		Kart target2 = _target;
		target2.OnBeSwaped = (Action<Kart>)Delegate.Remove(target2.OnBeSwaped, new Action<Kart>(KartBeSwaped));
		Kart target3 = _target;
		target3.OnTeleported = (Action)Delegate.Remove(target3.OnTeleported, new Action(LoseTarget));
		Kart target4 = _target;
		target4.OnSpringJump = (Action)Delegate.Remove(target4.OnSpringJump, new Action(LoseTarget));
		KartBonusMgr bonusMgr = _target.GetBonusMgr();
		bonusMgr.OnLaunchBonus = (Action<EITEM, bool>)Delegate.Remove(bonusMgr.OnLaunchBonus, new Action<EITEM, bool>(KartLaunchBonus));
		Kart target5 = _target;
		target5.OnBoost = (Action)Delegate.Remove(target5.OnBoost, new Action(LoseTarget));
		Kart target6 = _target;
		target6.OnRespawn = (Action)Delegate.Remove(target6.OnRespawn, new Action(LoseTarget));
	}

	private void KartLaunchBonus(EITEM pItem, bool pBehind)
	{
		if (pItem == EITEM.ITEM_LASAGNA || (pItem == EITEM.ITEM_SPRING && !pBehind) || (pItem == EITEM.ITEM_PARFUME && !((ParfumeBonusEffect)_target.GetBonusMgr().GetBonusEffectMgr().GetBonusEffect(EBonusEffect.BONUSEFFECT_ATTRACTED)).StinkParfume))
		{
			LoseTarget();
		}
	}

	private void KartBeSwaped(Kart pOtherKart)
	{
		LoseTarget();
	}

	public void DoLoseTarget()
	{
		if (m_bActive && _target != null)
		{
			m_bBehind = true;
			base.transform.Rotate(new Vector3(-354f, 0f, 0f));
			_target.HasDiamondAttached = false;
			base.Launcher = _target;
			m_bSynchronizePosition = true;
			if (_parent != null)
			{
				_parent.parent = null;
			}
			Launch();
			if ((bool)SoundUnglued)
			{
				SoundUnglued.Play();
			}
		}
	}

	public void LoseTargetByJump()
	{
		if (m_bActive && _target != null)
		{
			if (Network.peerType != 0 && m_pNetworkView != null)
			{
				m_pNetworkView.RPC("OnLoseTarget", RPCMode.All);
			}
			else if (Network.peerType == NetworkPeerType.Disconnected || m_pNetworkView == null)
			{
				DoLoseTarget();
			}
		}
	}

	public void LoseTarget()
	{
		if (m_bActive && _target != null)
		{
			if (Network.isServer && m_pNetworkView != null)
			{
				m_pNetworkView.RPC("OnLoseTarget", RPCMode.All);
			}
			else if (Network.peerType == NetworkPeerType.Disconnected || m_pNetworkView == null)
			{
				DoLoseTarget();
			}
		}
	}

	public void Explode()
	{
		if (_state == E_STATE.EXPLODE)
		{
			return;
		}
		ParticleTrail.particleSystem.Stop();
		ParticleTrail.gameObject.SetActive(false);
		_diamond.renderer.enabled = false;
		_attackEffect.particleSystem.Stop();
		_explosionEffect.particleSystem.Play();
		m_pCollider.enabled = false;
		_explosionCollider.enabled = true;
		_explosionAnimation.Play();
		if (_explosionCollider.gameObject.activeSelf && (bool)_launcherCollider && _launcherCollider.gameObject.activeSelf)
		{
			Physics.IgnoreCollision(_explosionCollider, _launcherCollider, false);
		}
		_state = E_STATE.EXPLODE;
		if (_target != null)
		{
			if (!_target.GetBonusMgr().GetBonusEffectMgr().GetBonusEffect(EBonusEffect.BONUSEFFECT_UPSIDE_DOWN)
				.Activated)
			{
				_target.GetBonusMgr().GetBonusEffectMgr().GetBonusEffect(EBonusEffect.BONUSEFFECT_UPSIDE_DOWN)
					.BonusEffectDirection = EBonusEffectDirection.BACK;
				_target.GetBonusMgr().GetBonusEffectMgr().ActivateBonusEffect(EBonusEffect.BONUSEFFECT_UPSIDE_DOWN);
				PlayVictorySound(_target);
			}
			_target.HasDiamondAttached = false;
			UnregisterEvents();
			_target = null;
		}
		if ((bool)SoundExplode && (bool)SoundCountdown)
		{
			SoundExplode.Play();
			SoundCountdown.Stop();
		}
	}

	public void PlayVictorySound(Kart pKart)
	{
		if (pKart != m_pLauncher && !m_bVictorySound)
		{
			m_bVictorySound = true;
			m_pLauncher.KartSound.PlayVoice(KartSound.EVoices.Good);
			m_pLauncher.Anim.LaunchSuccessAnim(true);
		}
	}

	private void KartJumped()
	{
		_jumpCounter++;
	}

	public override void SetActive(bool pActive)
	{
		if (!pActive)
		{
			if (_parent != null)
			{
				_parent.parent = null;
			}
			base.transform.localPosition = Vector3.zero;
		}
		base.SetActive(pActive);
		ActivateGameObject(pActive);
	}

	public override void DoOnTriggerEnter(GameObject pOther, int otherlayer)
	{
		int num = 1 << otherlayer;
		if (pOther != null && (num & (int)LayerBonus) != 0)
		{
			BonusEntity componentInChildren = pOther.GetComponentInChildren<BonusEntity>();
			if (componentInChildren is DiamondBonusEntity && _state != E_STATE.END)
			{
				((DiamondBonusEntity)componentInChildren).Explode();
				Explode();
			}
		}
		if (_state != E_STATE.EXPLODE && _state != E_STATE.END)
		{
			if ((num & (int)LayerVehicle) != 0 && _state != E_STATE.ATTACHED)
			{
				if (Network.peerType != 0 && !Network.isServer)
				{
					return;
				}
				Kart componentInChildren2 = pOther.GetComponentInChildren<Kart>();
				if ((bool)componentInChildren2)
				{
					ParfumeBonusEffect parfumeBonusEffect = (ParfumeBonusEffect)componentInChildren2.GetBonusMgr().GetBonusEffectMgr().GetBonusEffect(EBonusEffect.BONUSEFFECT_ATTRACTED);
					if (!parfumeBonusEffect.Activated || parfumeBonusEffect.StinkParfume)
					{
						AcquireTarget(pOther);
					}
				}
			}
			else
			{
				if (_state == E_STATE.IDLE || _state == E_STATE.ATTACHED)
				{
					return;
				}
				if ((num & (int)LayerWall) != 0)
				{
					if (Network.peerType != 0 && !Network.isServer)
					{
						return;
					}
					Ray ray = new Ray(base.transform.position, _currentDir);
					RaycastHit hitInfo;
					if (Physics.Raycast(ray, out hitInfo, 5f, LayerWall))
					{
						Vector3 normal = hitInfo.normal;
						normal.Normalize();
						float num2 = Vector3.Dot(_currentDir, normal);
						Vector3 pDir = _currentDir - 2f * (num2 * normal);
						_currentAngle = num2 * 57.29578f;
						if (_currentAngle < 0f)
						{
							_currentAngle += 360f;
						}
						CalculateNewRange();
						CalculateInitialSpeed(pDir, _currentAngle, _currentRange);
						CollideWall();
					}
				}
				else if ((num & (int)LayerRoad) != 0 && (Network.peerType == NetworkPeerType.Disconnected || Network.isServer))
				{
					Ray ray2 = new Ray(base.transform.position, Vector3.down);
					RaycastHit hitInfo2;
					if (Physics.Raycast(ray2, out hitInfo2, 5f, LayerRoad))
					{
						CollideRoad(hitInfo2.point);
					}
				}
			}
		}
		else
		{
			if (_state != E_STATE.EXPLODE || !_explosionAnimation.isPlaying || (num & (int)LayerVehicle) == 0)
			{
				return;
			}
			Kart componentInChildren3 = pOther.GetComponentInChildren<Kart>();
			if ((bool)componentInChildren3 && !componentInChildren3.GetBonusMgr().GetBonusEffectMgr().GetBonusEffect(EBonusEffect.BONUSEFFECT_UPSIDE_DOWN)
				.Activated)
			{
				ParfumeBonusEffect parfumeBonusEffect2 = (ParfumeBonusEffect)componentInChildren3.GetBonusMgr().GetBonusEffectMgr().GetBonusEffect(EBonusEffect.BONUSEFFECT_ATTRACTED);
				if (!parfumeBonusEffect2.Activated || parfumeBonusEffect2.StinkParfume)
				{
					componentInChildren3.GetBonusMgr().GetBonusEffectMgr().GetBonusEffect(EBonusEffect.BONUSEFFECT_UPSIDE_DOWN)
						.BonusEffectDirection = EBonusEffectDirection.BACK;
					componentInChildren3.GetBonusMgr().GetBonusEffectMgr().ActivateBonusEffect(EBonusEffect.BONUSEFFECT_UPSIDE_DOWN);
					PlayVictorySound(componentInChildren3);
				}
			}
		}
	}

	public void CollideWall()
	{
		if (m_bActive)
		{
			if (Network.peerType != 0 && m_pNetworkView != null)
			{
				m_pNetworkView.RPC("OnCollideWall", RPCMode.All);
			}
			else if (Network.peerType == NetworkPeerType.Disconnected || m_pNetworkView == null)
			{
				DoCollideWall();
			}
		}
	}

	public void DoCollideWall()
	{
		if ((bool)SoundLand)
		{
			SoundLand.Play();
		}
	}

	public void CollideRoad(Vector3 hit)
	{
		if (m_bActive)
		{
			if (Network.peerType != 0 && m_pNetworkView != null)
			{
				m_pNetworkView.RPC("OnCollideRoad", RPCMode.All, hit);
			}
			else if (Network.peerType == NetworkPeerType.Disconnected || m_pNetworkView == null)
			{
				DoCollideRoad(hit);
			}
		}
	}

	public void DoCollideRoad(Vector3 hit)
	{
		_state = E_STATE.IDLE;
		_explosionCollider.enabled = true;
		if (_explosionCollider.gameObject.activeSelf && (bool)_launcherCollider && (bool)_launcherCollider.gameObject && _launcherCollider.gameObject.activeSelf)
		{
			Physics.IgnoreCollision(_explosionCollider, _launcherCollider, true);
		}
		m_fTimerNoPhysic = 0f;
		_rigidBody.angularVelocity = Vector3.zero;
		base.transform.position = new Vector3(hit.x, hit.y + 0.5f, hit.z);
		if ((bool)SoundLand)
		{
			SoundLand.Play();
		}
	}

	private void CalculateNewRange()
	{
		float num = Time.time - _initialTime;
		float num2 = num * _initialSpeed.magnitude * Mathf.Cos(_currentAngle * ((float)Math.PI / 180f));
		_currentRange -= num2;
		if (_currentRange < 0f)
		{
			_currentRange = 0f;
		}
	}

	public void DoAcquireTarget(GameObject pParent)
	{
		_target = pParent.GetComponentInChildren<Kart>();
		if (!(_target == null))
		{
			_target.KartSound.PlayVoice(KartSound.EVoices.Bad);
			_target.HasDiamondAttached = true;
			RegisterEvents();
			base.transform.Rotate(new Vector3(354f, 0f, 0f));
			_currentSpeed = ((KartArcadeGearBox)_target.GetGearBox()).GetBaseMaxSpeed() + SpeedFactor / 3.6f;
			_state = E_STATE.ATTACHED;
			if ((bool)SoundGlued)
			{
				SoundGlued.Play();
			}
		}
	}

	public void AcquireTarget(GameObject pParent)
	{
		if (!m_bActive)
		{
			return;
		}
		if (Network.peerType != 0 && m_pNetworkView != null)
		{
			NetworkViewID networkViewID = default(NetworkViewID);
			if ((bool)pParent.GetComponent<NetworkView>())
			{
				networkViewID = pParent.GetComponent<NetworkView>().viewID;
			}
			m_pNetworkView.RPC("OnAcquireTarget", RPCMode.All, networkViewID);
		}
		else if (Network.peerType == NetworkPeerType.Disconnected || m_pNetworkView == null)
		{
			DoAcquireTarget(pParent);
		}
	}
}

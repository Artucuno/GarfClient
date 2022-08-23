using System;
using System.Collections;
using UnityEngine;

public class Kart : RcVehicle
{
	public float DriftRatio = 0.5f;

	public float m_fJumpHeight = 0.5f;

	private float m_fInvincibleTimer;

	private KartAnim m_pKartAnim;

	private KartBonusMgr m_pKartBonusMgr;

	public int Index;

	private HUDBonus m_pHudBonus;

	private HUDPosition m_pHudPosition;

	private float _napEffectTime;

	private float _napEffectFactor;

	private KartFxMgr m_pKartFxMgr;

	private float m_fMiniBoost;

	public float m_fMiniBoostFillRateCounterSteer = 20f;

	public float m_fMiniBoostFillRateMed = 35f;

	public float m_fMiniBoostFillRateMaxDrift = 50f;

	public float ThresholdMiniBoost = 80f;

	public float SpeedUpMiniBoost = 20f;

	public float DurationMiniBoost = 1f;

	public float AccelerationMiniBoost = 2f;

	public float PercentMiniBoostThreshold = 60f;

	private bool m_bMiniboostThresholdOk;

	private KartSound m_pKartSound;

	private EAdvantage m_eAdvantage;

	private bool hasSpringActivated;

	private float tmpJumpHeight;

	private float tmpJumpForward;

	public Action OnHit;

	public Action<Kart> OnUfoCatchMe;

	public float WrongWayTimer = 1.5f;

	private float m_fWrongWayTimer;

	private NetworkViewID m_oNetworkViewID = NetworkViewID.unassigned;

	public float DraftMinimalSpeed = 30f;

	public float DraftDistance = 5f;

	public float DraftCapsuleRadius = 1f;

	public float DraftTimer = 1.5f;

	private float m_fDraftTimer;

	private float m_fDeltaDraft;

	private bool m_bIsFollowed;

	private int m_iRaycastTurn;

	private Kart m_oDraftingKart;

	public Action<Kart> OnBeSwaped;

	public Action OnBoost;

	public Action OnJump;

	public Action OnSpringJump;

	public bool IsInShortCut { get; set; }

	public bool HasDiamondAttached { get; set; }

	public KartSound KartSound
	{
		get
		{
			return m_pKartSound;
		}
	}

	public NetworkViewID networkViewID
	{
		get
		{
			if (m_oNetworkViewID.Equals(NetworkViewID.unassigned))
			{
				m_oNetworkViewID = Transform.parent.gameObject.networkView.viewID;
			}
			return m_oNetworkViewID;
		}
	}

	public Transform Transform
	{
		get
		{
			return m_pTransform;
		}
		set
		{
			m_pTransform = value;
		}
	}

	public KartAnim Anim
	{
		get
		{
			return m_pKartAnim;
		}
	}

	public KartBonusMgr BonusMgr
	{
		get
		{
			return m_pKartBonusMgr;
		}
	}

	public HUDBonus HUDBonus
	{
		get
		{
			return m_pHudBonus;
		}
		set
		{
			m_pHudBonus = value;
			if (m_pKartBonusMgr != null)
			{
				m_pKartBonusMgr.HUDBonus = m_pHudBonus;
			}
		}
	}

	public HUDPosition HUDPosition
	{
		get
		{
			return m_pHudPosition;
		}
		set
		{
			m_pHudPosition = value;
		}
	}

	public KartFxMgr FxMgr
	{
		get
		{
			return m_pKartFxMgr;
		}
	}

	public EAdvantage SelectedAdvantage
	{
		get
		{
			return m_eAdvantage;
		}
		set
		{
			m_eAdvantage = value;
		}
	}

	public float MiniBoost
	{
		get
		{
			return m_fMiniBoost;
		}
	}

	public Vector3 LaunchDirection
	{
		get
		{
			Transform pVehicleMesh = (m_pVehiclePhysic as RcKinematicPhysic).m_pVehicleMesh;
			return Quaternion.Slerp(pVehicleMesh.rotation, m_pTransform.rotation, DriftRatio) * Vector3.forward;
		}
	}

	public Vector3 LaunchHorizontalDirection
	{
		get
		{
			Transform pVehicleMesh = (m_pVehiclePhysic as RcKinematicPhysic).m_pVehicleMesh;
			Vector3 vector = Quaternion.Slerp(pVehicleMesh.rotation, m_pTransform.rotation, DriftRatio) * Vector3.forward;
			vector.y = 0f;
			return vector.normalized;
		}
	}

	public bool IsSleeping()
	{
		return _napEffectTime > 0f;
	}

	public override void Awake()
	{
		base.Awake();
		m_pTransform = base.gameObject.transform;
		m_pKartAnim = m_pTransform.parent.FindChild("Base").GetComponent<KartAnim>();
		m_pKartBonusMgr = m_pTransform.parent.FindChild("Base").GetComponent<KartBonusMgr>();
		m_pKartFxMgr = m_pTransform.parent.FindChild("Base").GetComponent<KartFxMgr>();
		m_pKartSound = m_pTransform.parent.FindChild("Sound").GetComponent<KartSound>();
	}

	public override void Start()
	{
		base.Start();
		SetDefaultValues();
		if (DebugMgr.Instance != null && DebugMgr.Instance.dbgData.DisplayCustom)
		{
			KartArcadeGearBox kartArcadeGearBox = (KartArcadeGearBox)m_pGearBox;
			string text = "Vehicule " + Index;
			for (int i = 0; i < Enum.GetValues(typeof(DrivingCaracteristics)).Length; i++)
			{
				string text2 = text;
				text = text2 + " : " + ((DrivingCaracteristics)i).ToString() + " : " + kartArcadeGearBox.GetPercentAdvantages((DrivingCaracteristics)i);
			}
		}
		m_fMiniBoost = 0f;
		m_eAdvantage = EAdvantage.None;
		m_bMiniboostThresholdOk = false;
		if (m_eControlType == ControlType.Human && Singleton<GameManager>.Instance.GameMode.Hud != null && Singleton<GameManager>.Instance.GameMode.Hud.Position.WrongWay != null)
		{
			Singleton<GameManager>.Instance.GameMode.Hud.Position.WrongWay.SetActive(false);
		}
	}

	public void CancelDrift()
	{
		SetArcadeDriftFactor(0f);
		m_pKartFxMgr.StopDriftFx();
		m_pKartFxMgr.KartFxs[0].ParticleSystem.Stop();
		m_pKartFxMgr.KartFxs[8].ParticleSystem.Stop();
		if (GetControlType() == ControlType.Human)
		{
			Camera mainCamera = Camera.mainCamera;
			mainCamera.GetComponent<CamStateFollow>().bBoost = false;
			ParticleSystem componentInChildren = mainCamera.GetComponentInChildren<ParticleSystem>();
			if ((bool)componentInChildren)
			{
				componentInChildren.Stop();
			}
		}
	}

	public void Draft(float deltaTime)
	{
		m_fDeltaDraft = deltaTime;
	}

	public void ResetDraft()
	{
		m_fDraftTimer = 0f;
	}

	public override void Update()
	{
		base.Update();
		float deltaTime = Time.deltaTime;
		if (_napEffectTime > 0f)
		{
			_napEffectTime -= deltaTime;
			if (_napEffectTime <= 0f)
			{
				_napEffectTime = 0f;
				GetCarac().m_fDriftTimeToMaxSteering -= _napEffectFactor;
				GetCarac().m_fResetSteeringNoInput -= _napEffectFactor;
				GetCarac().m_fDriftResetSteeringNoInput -= _napEffectFactor;
				GetCarac().m_fTimeToMaxSteering -= _napEffectFactor;
				GetCarac().m_fResetSteeringOppositeInput -= _napEffectFactor / 2f;
				_napEffectFactor = 0f;
			}
			else
			{
				SetArcadeDriftFactor(0f);
			}
		}
		else
		{
			float num = ComputeMiniBoostFillRate();
			if (m_fMiniBoost < 100f)
			{
				m_fMiniBoost += deltaTime * num;
				Mathf.Clamp(m_fMiniBoost, 0f, 100f);
				if (m_fMiniBoost >= ThresholdMiniBoost)
				{
					m_pKartFxMgr.BoostDrift(m_fMiniBoost, ThresholdMiniBoost);
					if ((bool)m_pKartSound && !m_bMiniboostThresholdOk)
					{
						m_pKartSound.PlaySound(7);
						m_bMiniboostThresholdOk = true;
					}
				}
			}
		}
		if (base.RaceStats != null && m_pHudPosition != null)
		{
			m_pHudPosition.DisplayRaceStats(base.RaceStats);
		}
		if (m_eControlType == ControlType.Human && Singleton<GameManager>.Instance.GameMode.State == E_GameState.Race)
		{
			if (m_pRaceStats.IsReverse())
			{
				m_fWrongWayTimer += Time.deltaTime;
				if (m_fWrongWayTimer > WrongWayTimer)
				{
					Singleton<GameManager>.Instance.SoundManager.PlaySound(ERaceSounds.WrongWay);
					Singleton<GameManager>.Instance.GameMode.Hud.Position.WrongWay.SetActive(true);
				}
			}
			else if (m_fWrongWayTimer > 0f)
			{
				Singleton<GameManager>.Instance.SoundManager.StopSound(ERaceSounds.WrongWay);
				Singleton<GameManager>.Instance.GameMode.Hud.Position.WrongWay.SetActive(false);
				m_fWrongWayTimer = 0f;
			}
		}
		if (hasSpringActivated && IsOnGround())
		{
			StartCoroutine(DoSpringJump());
			hasSpringActivated = false;
		}
	}

	public void LateUpdate()
	{
		if (m_fDeltaDraft > 0f)
		{
			m_fDraftTimer += m_fDeltaDraft;
			m_fDeltaDraft = 0f;
			if (m_fDraftTimer >= DraftTimer)
			{
				ResetDraft();
				LaunchMiniBoost();
			}
		}
		else
		{
			ResetDraft();
		}
	}

	public IEnumerator DoSpringJump()
	{
		yield return new WaitForSeconds(0.05f);
		Jump(tmpJumpHeight, tmpJumpForward);
	}

	public void OnGUI()
	{
		if (GetControlType() == ControlType.Human && DebugMgr.Instance != null && DebugMgr.Instance.dbgData.DisplaySpeed)
		{
			GUI.contentColor = Color.red;
			GUI.Label(new Rect(400f, 20f, 200f, 50f), "Speed : " + GetWheelSpeedMS() * 3.6f + "(" + GetWheelSpeedMS() * 3.6f * (1f - GetHandicap()) + ")");
		}
	}

	public void StartSleepMode(float pIntertiaFactor, float pDuration)
	{
		if (_napEffectTime == 0f && _napEffectFactor == 0f)
		{
			_napEffectFactor = GetCarac().m_fDriftTimeToMaxSteering * pIntertiaFactor;
			GetCarac().m_fDriftTimeToMaxSteering += _napEffectFactor;
			GetCarac().m_fResetSteeringNoInput += _napEffectFactor;
			GetCarac().m_fDriftResetSteeringNoInput += _napEffectFactor;
			GetCarac().m_fTimeToMaxSteering += _napEffectFactor;
			GetCarac().m_fResetSteeringOppositeInput += _napEffectFactor / 2f;
			m_fMiniBoost = 0f;
			m_bMiniboostThresholdOk = false;
			SetArcadeDriftFactor(0f);
			FxMgr.StopDriftFx();
		}
		_napEffectTime = pDuration;
	}

	public void SetDefaultValues()
	{
		KartArcadeGearBox kartArcadeGearBox = (KartArcadeGearBox)m_pGearBox;
		kartArcadeGearBox.SetDefaultValues();
		m_fInvincibleTimer = 0f;
	}

	public KartBonusMgr GetBonusMgr()
	{
		return m_pKartBonusMgr;
	}

	public override void Respawn()
	{
		KartArcadeGearBox kartArcadeGearBox = (KartArcadeGearBox)m_pGearBox;
		kartArcadeGearBox.SetDefaultValues();
		m_pKartBonusMgr.Respawn();
		m_pKartAnim.ForceStopBonusAnimAll();
		base.Respawn();
		if (Singleton<ChallengeManager>.Instance.IsActive && GetControlType() == ControlType.Human)
		{
			Singleton<ChallengeManager>.Instance.Notify(EChallengeSingleRaceObjective.NoFall);
		}
	}

	public override bool IsBoosting()
	{
		KartArcadeGearBox kartArcadeGearBox = (KartArcadeGearBox)m_pGearBox;
		return kartArcadeGearBox.IsBoosting();
	}

	public override void ManageRunState()
	{
		base.ManageRunState();
		float deltaTime = Time.deltaTime;
		if (m_fInvincibleTimer > 0f)
		{
			m_fInvincibleTimer -= deltaTime;
		}
		else if ((double)m_fInvincibleTimer != -1.0)
		{
			m_fInvincibleTimer = 0f;
		}
	}

	public void ActivateInvincibility(float _invincibleDelay)
	{
		m_fInvincibleTimer = _invincibleDelay;
	}

	public void DeactivateInvincibility()
	{
		m_fInvincibleTimer = 0f;
	}

	public bool IsInvincible()
	{
		return m_fInvincibleTimer > 0f || m_fInvincibleTimer == -1f;
	}

	public void Boost(float _speedUpMs, float _boostDelay, float _BoostAcceleration, bool bWithEffect)
	{
		KartArcadeGearBox kartArcadeGearBox = (KartArcadeGearBox)m_pGearBox;
		kartArcadeGearBox.Boost(_speedUpMs, _boostDelay, _BoostAcceleration, bWithEffect);
		KartSound.PlaySound(6);
	}

	public void ParfumeBoost(float _speedUpMs, float _boostDelay)
	{
		KartArcadeGearBox kartArcadeGearBox = (KartArcadeGearBox)m_pGearBox;
		kartArcadeGearBox.ParfumeBoost(_speedUpMs, _boostDelay);
	}

	public void SlowDown(float _SlowedDownMaxSpeed, float _SlowedDownDelay, float _SlowedDownDownMs)
	{
		KartArcadeGearBox kartArcadeGearBox = (KartArcadeGearBox)m_pGearBox;
		kartArcadeGearBox.SlowDown(_SlowedDownMaxSpeed, _SlowedDownDelay, _SlowedDownDownMs);
	}

	public bool SpringJump(float _JumpHeight, float _JumpForward, bool _Backward)
	{
		if (Jump(_JumpHeight, (!_Backward) ? _JumpForward : (0f - _JumpForward)))
		{
			if (OnSpringJump != null)
			{
				OnSpringJump();
			}
			return true;
		}
		return false;
	}

	public bool Jump(float _JumpHeight, float _JumpForward)
	{
		RcKinematicPhysic rcKinematicPhysic = (RcKinematicPhysic)m_pVehiclePhysic;
		if (!(_JumpForward >= 0f) || IsOnGround())
		{
			float num = ((_JumpHeight != 0f) ? _JumpHeight : m_fJumpHeight);
			float num2 = rcKinematicPhysic.m_fAirAdditionnalGravity + Vector3.Dot(Physics.gravity, Vector3.down);
			float num3 = Mathf.Sqrt(2f * num * num2);
			float num4 = 2f * num3 / num2;
			Vector3 impulse = Vector3.up * num3 + Transform.forward * _JumpForward;
			rcKinematicPhysic.SwitchToInertiaMode(num4 * 0.5f, impulse, true, true);
			if (OnJump != null)
			{
				OnJump();
			}
			return true;
		}
		hasSpringActivated = true;
		tmpJumpHeight = _JumpHeight;
		tmpJumpForward = _JumpForward;
		return false;
	}

	public override void ComputeWheelSpeed()
	{
		KartArcadeGearBox kartArcadeGearBox = (KartArcadeGearBox)m_pGearBox;
		KartKinematicPhysic kartKinematicPhysic = (KartKinematicPhysic)m_pVehiclePhysic;
		kartKinematicPhysic.SetBoosting(kartArcadeGearBox.IsBoosting());
		kartKinematicPhysic.SetParfume(kartArcadeGearBox.IsParfume());
		base.ComputeWheelSpeed();
		if (kartArcadeGearBox.IsBoosting())
		{
			float deltaTime = Time.deltaTime;
			float maxSpeed = GetMaxSpeed();
			VehicleHandling _handlingOut;
			m_pVehicleCarac.ComputeHandling(Mathf.Abs(m_fWheelSpeedMS * (1f - GetHandicap())), out _handlingOut);
			m_fWheelSpeedMS += m_pGearBox.ComputeAcceleration(Mathf.Abs(m_fWheelSpeedMS)) * deltaTime;
			if (m_fWheelSpeedMS > maxSpeed)
			{
				m_fWheelSpeedMS = maxSpeed;
			}
		}
	}

	public override void SetArcadeDriftFactor(float factor)
	{
		base.SetArcadeDriftFactor(factor);
		if (factor == 0f)
		{
			float action = Singleton<InputManager>.Instance.GetAction(EAction.Drift);
			if (action == 0f && m_fMiniBoost >= ThresholdMiniBoost)
			{
				LaunchMiniBoost();
			}
			m_fMiniBoost = 0f;
			m_bMiniboostThresholdOk = false;
			m_pKartFxMgr.BoostDrift(m_fMiniBoost, ThresholdMiniBoost);
		}
	}

	public float ComputeMiniBoostFillRate()
	{
		if (m_fArcadeDriftFactor == 0f || !IsOnGround())
		{
			FxMgr.StopDriftFx();
			return 0f;
		}
		if (Singleton<ChallengeManager>.Instance.IsActive && GetControlType() == ControlType.Human && m_fArcadeDriftFactor != 0f)
		{
			Singleton<ChallengeManager>.Instance.Notify(EChallengeSingleRaceObjective.NoDrift);
		}
		if (m_fMiniBoost < ThresholdMiniBoost)
		{
			FxMgr.PlayKartFx(eKartFx.DriftLeft);
			FxMgr.PlayKartFx(eKartFx.DriftRight);
		}
		else
		{
			FxMgr.PlayKartFx(eKartFx.DriftLeft2);
			FxMgr.PlayKartFx(eKartFx.DriftRight2);
		}
		if (m_fArcadeDriftFactor * m_fSteeringFactor > 0f)
		{
			return RcUtils.LinearInterpolation(0f, m_fMiniBoostFillRateMed, 1f, m_fMiniBoostFillRateCounterSteer, Mathf.Abs(m_fArcadeDriftFactor * m_fSteeringFactor), true);
		}
		return RcUtils.LinearInterpolation(0f, m_fMiniBoostFillRateMed, 1f, m_fMiniBoostFillRateMaxDrift, Mathf.Abs(m_fArcadeDriftFactor * m_fSteeringFactor), true);
	}

	public void LaunchMiniBoost()
	{
		float num = SpeedUpMiniBoost;
		float num2 = DurationMiniBoost;
		float num3 = AccelerationMiniBoost;
		if (m_fMiniBoost < 100f)
		{
			float num4 = RcUtils.LinearInterpolation(ThresholdMiniBoost, PercentMiniBoostThreshold, 100f, 100f, m_fMiniBoost, true);
			num = num4 * num / 100f;
			num2 = num4 * num2 / 100f;
			num3 = num4 * num3 / 100f;
		}
		int num5 = Singleton<RandomManager>.Instance.Next(0, 4);
		if (num5 < 3)
		{
			KartSound.PlayVoice(KartSound.EVoices.Good);
		}
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TUTORIAL)
		{
			if (m_fMiniBoost < 100f)
			{
				(Singleton<GameManager>.Instance.GameMode as TutorialGameMode).BlueDrift();
			}
			else
			{
				(Singleton<GameManager>.Instance.GameMode as TutorialGameMode).RedDrift();
			}
		}
		Boost(num, num2, num3, true);
	}

	public void StartRace()
	{
		if (m_pKartBonusMgr != null)
		{
			m_pKartBonusMgr.StartRace();
		}
	}

	public void TakePuzzlePiece(int iIndex)
	{
		if (m_pHudPosition != null)
		{
			m_pHudPosition.TakePuzzlePiece(iIndex);
		}
	}

	public bool HasNapEffect()
	{
		if (_napEffectTime > 0f)
		{
			return true;
		}
		return false;
	}
}

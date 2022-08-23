using System;
using UnityEngine;

public class NapBonusEffect : BonusEffect
{
	[SerializeField]
	[HideInInspector]
	public float SlowDownFactor = 50f;

	[SerializeField]
	[HideInInspector]
	public float DecelerationSpeed = 0.5f;

	[HideInInspector]
	[SerializeField]
	public float InertiaFactor = 0.5f;

	public GameObject AttackEffect;

	private GameObject _attackEffect;

	public static Action OnLaunched;

	public AudioSource Music;

	private Kart m_pLauncher;

	public Vector3 NapOffset;

	public Kart Launcher
	{
		set
		{
			m_pLauncher = value;
		}
	}

	public override void Start()
	{
		m_iAnimParameter = Animator.StringToHash("NapAttack_Impact");
		m_iAnimState = Animator.StringToHash("NapAttack.NapAttack_Loop");
		InertiaVehicle = false;
		m_bStoppedByAnim = false;
		_attackEffect = (GameObject)UnityEngine.Object.Instantiate(AttackEffect);
		base.Start();
	}

	protected override void OnDestroy()
	{
		if (_attackEffect != null)
		{
			UnityEngine.Object.Destroy(_attackEffect);
		}
	}

	public override void Update()
	{
		base.Update();
	}

	public override void SetDuration()
	{
		m_fCurrentDuration = EffectDuration + m_pBonusEffectMgr.Target.GetBonusMgr().GetBonusValue(EITEM.ITEM_NAP, EBonusCustomEffect.DURATION) * EffectDuration / 100f;
	}

	public override bool Activate()
	{
		base.Activate();
		Kart target = m_pBonusEffectMgr.Target;
		target.CancelDrift();
		if (OnLaunched != null && target.GetControlType() == RcVehicle.ControlType.Human)
		{
			OnLaunched();
		}
		float pIntertiaFactor = InertiaFactor + m_pLauncher.GetBonusMgr().GetBonusValue(EITEM.ITEM_NAP, EBonusCustomEffect.INERTIA) * InertiaFactor / 100f;
		target.SlowDown(SlowDownFactor, m_fCurrentDuration, DecelerationSpeed);
		target.StartSleepMode(pIntertiaFactor, m_fCurrentDuration);
		target.Anim.LaunchBonusAnimOnCharacter(m_iAnimParameter, m_iAnimState, false);
		target.Anim.CanQueueAnim = false;
		target.KartSound.CanSpeak = false;
		target.KartSound.PlaySound(13);
		target.KartSound.PlayVoice(KartSound.EVoices.Snore);
		_attackEffect.transform.position = target.Transform.position;
		_attackEffect.transform.parent = target.Transform;
		_attackEffect.transform.localPosition = target.Transform.rotation * NapOffset;
		_attackEffect.particleSystem.Play();
		if (target.GetControlType() == RcVehicle.ControlType.Human)
		{
			Music.Play();
			Singleton<GameManager>.Instance.GameMode.MainMusic.volume = 0.3f;
		}
		return true;
	}

	public override void Deactivate()
	{
		base.Deactivate();
		Kart target = m_pBonusEffectMgr.Target;
		target.Anim.ForceStopBonusAnim(m_iAnimParameter, false, true);
		target.KartSound.CanSpeak = true;
		target.Anim.CanQueueAnim = true;
		if (_attackEffect != null)
		{
			_attackEffect.particleSystem.Stop();
		}
		target.KartSound.StopSound(13);
		if (target.GetControlType() == RcVehicle.ControlType.Human)
		{
			Music.Stop();
			Singleton<GameManager>.Instance.GameMode.MainMusic.volume = 0.8f;
		}
		target.KartSound.StopVoice();
		target.KartSound.PlayVoice(KartSound.EVoices.Awake);
	}
}

using UnityEngine;

public class BonusEffect : MonoBehaviour
{
	public bool Activated;

	[SerializeField]
	[HideInInspector]
	public float EffectDuration;

	protected float m_fCurrentDuration;

	protected BonusEffectMgr m_pBonusEffectMgr;

	protected bool InertiaVehicle;

	protected int m_iAnimParameter;

	protected int m_iAnimState;

	protected bool m_bStoppedByAnim;

	public AnimationClip m_Animation;

	protected EBonusEffectDirection m_eEffectDirection;

	public Vector3 ImpulseForce;

	public Vector3 InertiaDamping = Vector3.one;

	public BonusEffectMgr BonusEffectMgr
	{
		get
		{
			return m_pBonusEffectMgr;
		}
		set
		{
			m_pBonusEffectMgr = value;
		}
	}

	public EBonusEffectDirection BonusEffectDirection
	{
		get
		{
			return m_eEffectDirection;
		}
		set
		{
			m_eEffectDirection = value;
		}
	}

	public BonusEffect()
	{
		Activated = false;
		m_fCurrentDuration = 0f;
		EffectDuration = 0f;
		m_pBonusEffectMgr = null;
		InertiaVehicle = true;
		m_iAnimParameter = -1;
		m_iAnimState = -1;
		m_bStoppedByAnim = true;
		m_eEffectDirection = EBonusEffectDirection.LEFT;
	}

	public virtual void Start()
	{
		Activated = false;
		if (m_bStoppedByAnim && m_Animation != null)
		{
			EffectDuration = m_Animation.length;
		}
	}

	protected virtual void OnDestroy()
	{
	}

	public virtual void Update()
	{
		if (Activated && EffectDuration > 0f && m_fCurrentDuration > 0f)
		{
			m_fCurrentDuration -= Time.deltaTime;
			if (m_fCurrentDuration < 0f)
			{
				Deactivate();
				Kart target = m_pBonusEffectMgr.Target;
				target.Anim.StopBonusAnimAll(m_iAnimState);
			}
		}
	}

	public virtual void SetDuration()
	{
		m_fCurrentDuration = EffectDuration;
	}

	public virtual bool Activate()
	{
		Activated = true;
		SetDuration();
		if (InertiaVehicle)
		{
			Kart target = m_pBonusEffectMgr.Target;
			RcKinematicPhysic rcKinematicPhysic = (RcKinematicPhysic)target.GetVehiclePhysic();
			if (m_iAnimParameter != -1)
			{
				target.Anim.LaunchBonusAnimAll(m_iAnimParameter, m_iAnimState, m_bStoppedByAnim);
			}
			if (InertiaDamping != Vector3.one)
			{
				rcKinematicPhysic.SwitchToInertiaMode(m_fCurrentDuration, ImpulseForce, false, true, InertiaDamping);
			}
			else
			{
				rcKinematicPhysic.SwitchToInertiaMode(m_fCurrentDuration, ImpulseForce, false, true);
			}
		}
		return true;
	}

	public virtual void Deactivate()
	{
		Activated = false;
	}
}

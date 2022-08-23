using System;
using UnityEngine;

public class KartAnim : MonoBehaviour
{
	public Transform m_pKartAnimatorOwner;

	public Transform m_pCharacterAnimatorOwner;

	public Vector3 m_vMaxWheelSqueeze = Vector3.one;

	private Kart m_pKart;

	private GkAnimController _kartAnimController;

	private GkAnimController _characterAnimController;

	private float steeringFactor;

	private float steeringMassFactor;

	private float accelerationMassFactor;

	private int m_iSuccesAnim;

	private bool m_bCanQueueAnim = true;

	public bool CanQueueAnim
	{
		get
		{
			return m_bCanQueueAnim;
		}
		set
		{
			m_bCanQueueAnim = value;
		}
	}

	public KartAnim()
	{
		m_pKart = null;
		_kartAnimController = null;
		_characterAnimController = null;
		steeringFactor = 0f;
		steeringMassFactor = 0f;
		accelerationMassFactor = 0f;
	}

	public void Awake()
	{
	}

	public void Start()
	{
		m_pKart = base.transform.parent.GetComponentInChildren<Kart>();
		_kartAnimController = new GkAnimController(m_pKartAnimatorOwner.GetComponent<Animator>());
		_characterAnimController = new GkAnimController(m_pCharacterAnimatorOwner.GetComponent<Animator>());
		RcPhysicWheel[] wheels = m_pKart.GetVehiclePhysic().GetWheels();
		int nbWheels = m_pKart.GetNbWheels();
		for (int i = 0; i < nbWheels; i++)
		{
			RcPhysicWheel rcPhysicWheel = wheels[i];
			rcPhysicWheel.VMaxSqueeze = m_vMaxWheelSqueeze;
		}
		m_iSuccesAnim = Animator.StringToHash("Base Layer.Succes_1");
	}

	public void LaunchDefeatAnim(bool pLaunch)
	{
		if (m_bCanQueueAnim)
		{
			_characterAnimController.Animator.SetBool("Defeat", pLaunch);
		}
	}

	public void LaunchVictoryAnim(bool pLaunch)
	{
		if (m_bCanQueueAnim)
		{
			_characterAnimController.Animator.SetBool("Victory", pLaunch);
		}
	}

	public void LaunchSuccessAnim(bool pLaunch)
	{
		if (m_bCanQueueAnim)
		{
			_characterAnimController.Animator.SetBool("Projectile_Succes", pLaunch);
		}
	}

	public void Update()
	{
		float deltaTime = Time.deltaTime;
		float v = RcUtils.LinearInterpolation(-20f, -1f, 20f, 0.5f, m_pKart.GetWheelAccelMSS(), true);
		SteeringAnim(deltaTime);
		if (m_pKart.IsBoosting())
		{
			v = 1f;
		}
		float v2 = 0.5f * (1f + Mathf.Abs(m_pKart.GetArcadeDriftFactor())) * RcUtils.LinearInterpolation(-10f, -1f, 10f, 1f, Mathf.Abs(m_pKart.GetWheelSpeedMS()) * Mathf.Abs(m_pKart.GetWheelSpeedMS()) * m_pKart.GetSteeringAngle(), true);
		RcUtils.COMPUTE_INERTIA(ref steeringMassFactor, v2, 0.995f, deltaTime * 1000f);
		RcUtils.COMPUTE_INERTIA(ref accelerationMassFactor, v, 0.995f, deltaTime * 1000f);
		_kartAnimController.Animator.SetFloat("Speed", Mathf.Abs(m_pKart.GetWheelSpeedMS()));
		_kartAnimController.Animator.SetFloat("Direction", steeringMassFactor);
		_kartAnimController.Animator.SetFloat("Acceleration", 0f - accelerationMassFactor);
		_characterAnimController.Animator.SetFloat("Speed", m_pKart.GetWheelSpeedMS());
		_characterAnimController.Animator.SetBool("Boost", m_pKart.IsBoosting());
		_characterAnimController.Animator.SetFloat("Direction", 0f - steeringFactor);
		_kartAnimController.Update();
		_characterAnimController.Update();
		if (_characterAnimController.Animator.GetCurrentAnimatorStateInfo(0).nameHash == m_iSuccesAnim)
		{
			LaunchSuccessAnim(false);
		}
	}

	public void SteeringAnim(float dt)
	{
		if (dt == 0f)
		{
			return;
		}
		float v = m_pKart.GetSteeringFactor();
		steeringFactor = Tricks.ComputeInertia(steeringFactor, v, 200f, dt * 1000f);
		RcPhysicWheel[] wheels = m_pKart.GetVehiclePhysic().GetWheels();
		int nbWheels = m_pKart.GetNbWheels();
		for (int i = 0; i < nbWheels; i++)
		{
			RcPhysicWheel rcPhysicWheel = wheels[i];
			if (rcPhysicWheel.BSteeringOn)
			{
				rcPhysicWheel.FSteeringAnimationAngle = (0f - steeringFactor) * (float)Math.PI / 6f;
			}
		}
	}

	public void LaunchBonusAnim(int pAnimParameter, int pAnimState, bool pCanStop, bool pOnKart, bool pOnCharacter)
	{
		if (pOnKart)
		{
			_kartAnimController.Play(pAnimParameter, pAnimState, pCanStop);
		}
		if (pOnCharacter)
		{
			_characterAnimController.Play(pAnimParameter, pAnimState, pCanStop);
		}
	}

	public void StopBonusAnim(int pAnimState, bool pOnKart, bool pOnCharacter)
	{
		if (pOnKart)
		{
			_kartAnimController.Stop(pAnimState);
		}
		if (pOnCharacter)
		{
			_characterAnimController.Stop(pAnimState);
		}
	}

	public void ForceStopBonusAnim(int pAnimParameter, bool pOnKart, bool pOnCharacter)
	{
		if (pOnKart)
		{
			_kartAnimController.ForceStop(pAnimParameter);
		}
		if (pOnCharacter)
		{
			_characterAnimController.ForceStop(pAnimParameter);
		}
	}

	public void LaunchBonusAnimAll(int pAnimParameter, int pAnimState, bool pCanStop)
	{
		LaunchBonusAnim(pAnimParameter, pAnimState, pCanStop, true, true);
	}

	public void StopBonusAnimAll(int pAnimState)
	{
		StopBonusAnim(pAnimState, true, true);
	}

	public void ForceStopBonusAnimAll()
	{
		_kartAnimController.StopAll();
		_characterAnimController.StopAll();
	}

	public void LaunchBonusAnimOnKart(int pAnimParameter, int pAnimState, bool pCanStop)
	{
		LaunchBonusAnim(pAnimParameter, pAnimState, pCanStop, true, false);
	}

	public void StopBonusAnimOnKart(int pAnimState)
	{
		if (_kartAnimController.Animator.GetCurrentAnimatorStateInfo(0).nameHash == pAnimState)
		{
			StopBonusAnim(pAnimState, true, false);
		}
	}

	public void LaunchBonusAnimOnCharacter(int pAnimParameter, int pAnimState, bool pCanStop)
	{
		LaunchBonusAnim(pAnimParameter, pAnimState, pCanStop, false, true);
	}

	public void StopBonusAnimOnCharacter(int pAnimState)
	{
		if (_characterAnimController.Animator.GetCurrentAnimatorStateInfo(0).nameHash == pAnimState)
		{
			StopBonusAnim(pAnimState, false, true);
		}
	}
}

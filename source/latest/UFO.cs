using UnityEngine;

public class UFO : MonoBehaviour
{
	public enum ParticleName
	{
		NORMAL_RAY,
		GOOD_RAY,
		BAD_RAY
	}

	private BoxCollider m_pCollider;

	private ParticleSystem[] m_pRayParticles = new ParticleSystem[3];

	public GameObject ApparitionParticle;

	private GameObject ApparitionParticleGO;

	private Vector3 m_InitialParticlePos;

	private Vector3 m_InitialBoxCenter;

	private Vector3 m_InitialPos;

	private Transform m_pTransform;

	private Vector3 m_pFinalPosition;

	private float m_Speed;

	private Vector3 m_Direction;

	private float m_fScale;

	private UFOBonusEntity m_pParent;

	private string m_iUfoIndex;

	private int m_iLayerVehicle;

	private Kart m_pTarget;

	private float m_Timer;

	public bool GoodRay;

	public Animation m_pAnimator;

	private bool m_bLeaveIsPlaying;

	public AudioSource SoundGood;

	public AudioSource SoundBad;

	public AudioSource SoundDisappear;

	public float LevitateOffset;

	public UFOBonusEntity Parent
	{
		get
		{
			return m_pParent;
		}
	}

	public Transform Transform
	{
		get
		{
			return m_pTransform;
		}
	}

	public UFO()
	{
		m_pCollider = null;
		for (int i = 0; i < 3; i++)
		{
			m_pRayParticles[i] = null;
		}
		ApparitionParticle = null;
		ApparitionParticleGO = null;
		m_InitialParticlePos = Vector3.zero;
		m_InitialBoxCenter = Vector3.zero;
		m_pTransform = null;
		m_pFinalPosition = Vector3.zero;
		m_InitialPos = Vector3.zero;
		m_Direction = Vector3.zero;
		m_Speed = 0f;
		m_fScale = 1f;
		m_pParent = null;
		m_bLeaveIsPlaying = false;
	}

	public void Awake()
	{
		m_pParent = base.gameObject.transform.parent.GetComponent<UFOBonusEntity>();
		m_pCollider = GetComponent<BoxCollider>();
		m_InitialPos = base.transform.localPosition;
		m_pRayParticles[0] = base.transform.Find("fx_alien_beam_activated").particleSystem;
		m_pRayParticles[1] = base.transform.Find("fx_alien_good").particleSystem;
		m_pRayParticles[2] = base.transform.Find("fx_alien_bad").particleSystem;
		ApparitionParticleGO = (GameObject)Object.Instantiate(ApparitionParticle);
		ApparitionParticleGO.transform.parent = m_pParent.transform.parent;
		m_InitialParticlePos = m_pRayParticles[0].gameObject.transform.localPosition;
		m_InitialBoxCenter = m_pCollider.center;
		m_pTransform = base.transform;
		m_iLayerVehicle = 1 << LayerMask.NameToLayer("Vehicle");
		string text = base.gameObject.name;
		m_iUfoIndex = text.Substring(text.Length - 1, 1);
	}

	public void Launch(Vector3 _FinalPosition, float _Timer, float Dist)
	{
		m_pFinalPosition = _FinalPosition;
		float magnitude = (m_pFinalPosition - m_pTransform.position).magnitude;
		m_Speed = magnitude / _Timer;
		m_Direction = (m_pFinalPosition - m_pTransform.position).normalized;
		m_fScale = Dist;
		if ((bool)m_pAnimator)
		{
			m_pAnimator.Play("UFO_Run" + m_iUfoIndex);
		}
	}

	public void MoveToTarget(Kart _Target, float _Timer)
	{
		m_pTarget = _Target;
		m_Timer = _Timer;
	}

	public void Reset()
	{
		m_pTransform.localPosition = m_InitialPos;
		GoodRay = false;
		m_pTarget = null;
		DeactivateComponents();
		m_bLeaveIsPlaying = false;
		base.gameObject.SetActive(false);
	}

	public void DeactivateComponents()
	{
		DeactivateCollider();
		DeactivateParticle();
	}

	public void DeactivateCollider()
	{
		m_pCollider.center = m_InitialBoxCenter;
		m_pCollider.enabled = false;
	}

	public void DeactivateParticle()
	{
		for (int i = 0; i < m_pRayParticles.Length; i++)
		{
			m_pRayParticles[i].gameObject.transform.localPosition = m_InitialParticlePos;
			m_pRayParticles[i].Stop();
			m_pRayParticles[i].gameObject.SetActive(false);
		}
	}

	public void InPlace(bool _ActiveComponents)
	{
		m_pTransform.position = m_pFinalPosition;
		if (_ActiveComponents)
		{
			m_pCollider.center -= Vector3.up * ((m_pParent.UfoHeight + 2f) / 2f);
			Vector3 size = m_pCollider.size;
			size.x = m_fScale;
			size.y = m_pParent.UfoHeight + 2f;
			size.z = m_fScale;
			m_pCollider.size = size;
			m_pCollider.enabled = true;
			if ((bool)m_pAnimator)
			{
				m_pAnimator.Play("UFO_Launch");
			}
			for (int i = 0; i < m_pRayParticles.Length; i++)
			{
				m_pRayParticles[i].gameObject.SetActive(true);
				m_pRayParticles[i].gameObject.transform.position -= Vector3.up * (m_pParent.UfoHeight + 1f);
				m_pRayParticles[i].startSize = m_fScale * 2f;
				m_pRayParticles[i].Play();
			}
			m_pRayParticles[1].renderer.enabled = false;
			m_pRayParticles[2].renderer.enabled = false;
		}
	}

	public void Move(float _deltaTime)
	{
		if (m_pTarget != null)
		{
			m_Timer -= _deltaTime;
			m_pFinalPosition = m_pTarget.GetPosition() + Vector3.up * m_pParent.UfoHeight;
			float magnitude = (m_pFinalPosition - m_pTransform.position).magnitude;
			m_Speed = magnitude / m_Timer;
			m_Direction = (m_pFinalPosition - m_pTransform.position).normalized;
		}
		m_pTransform.position += m_Direction * m_Speed * _deltaTime;
	}

	public virtual void OnTriggerEnter(Collider other)
	{
		if (Network.isServer)
		{
			if (other.gameObject.networkView != null)
			{
				NetworkViewID viewID = other.gameObject.networkView.viewID;
				base.networkView.RPC("OnNetworkViewTriggerEnter", RPCMode.All, viewID);
			}
		}
		else if (Network.peerType == NetworkPeerType.Disconnected)
		{
			DoOnTriggerEnter(other.gameObject);
		}
	}

	[RPC]
	public void OnNetworkViewTriggerEnter(NetworkViewID viewId)
	{
		NetworkView networkView = NetworkView.Find(viewId);
		if (networkView != null)
		{
			DoOnTriggerEnter(networkView.gameObject);
		}
	}

	public void DoOnTriggerEnter(GameObject other)
	{
		if (!m_pCollider.enabled)
		{
			return;
		}
		int num = 1 << other.layer;
		if ((num & m_iLayerVehicle) == 0)
		{
			return;
		}
		Kart componentInChildren = other.GetComponentInChildren<Kart>();
		if (!(componentInChildren != null) || !(m_pParent != null))
		{
			return;
		}
		if (componentInChildren.OnUfoCatchMe != null)
		{
			componentInChildren.OnUfoCatchMe(componentInChildren);
		}
		ParfumeBonusEffect parfumeBonusEffect = (ParfumeBonusEffect)componentInChildren.GetBonusMgr().GetBonusEffectMgr().GetBonusEffect(EBonusEffect.BONUSEFFECT_ATTRACTED);
		if (parfumeBonusEffect.Activated && !parfumeBonusEffect.StinkParfume)
		{
			return;
		}
		if (!GoodRay)
		{
			m_pRayParticles[0].gameObject.SetActive(false);
			m_pRayParticles[2].renderer.enabled = true;
			m_pParent.MatchToTarget(this, componentInChildren);
			LevitateBonusEffect levitateBonusEffect = componentInChildren.GetBonusMgr().GetBonusEffectMgr().GetBonusEffect(EBonusEffect.BONUSEFFECT_LEVITATE) as LevitateBonusEffect;
			levitateBonusEffect.Owner = this;
			componentInChildren.GetBonusMgr().GetBonusEffectMgr().ActivateBonusEffect(EBonusEffect.BONUSEFFECT_LEVITATE);
			DeactivateCollider();
			if ((bool)m_pAnimator)
			{
				m_pAnimator.Play("UFO_Suck");
			}
			if (Singleton<GameManager>.Instance.GameMode != null && Singleton<GameManager>.Instance.GameMode.Hud != null)
			{
				Singleton<GameManager>.Instance.GameMode.Hud.HudRadarComp.SetUfoToBad();
			}
			if ((bool)SoundBad)
			{
				SoundBad.Play();
			}
			componentInChildren.KartSound.PlayVoice(KartSound.EVoices.Bad);
			if ((bool)m_pParent && (bool)m_pParent.Launcher && m_pParent.Launcher != componentInChildren)
			{
				m_pParent.Launcher.KartSound.PlayVoice(KartSound.EVoices.Good);
				m_pParent.Launcher.Anim.LaunchSuccessAnim(true);
			}
		}
		else
		{
			m_pRayParticles[0].gameObject.SetActive(false);
			m_pRayParticles[1].renderer.enabled = true;
			if (Network.peerType == NetworkPeerType.Disconnected || Network.isServer)
			{
				m_pParent.RemoveUFO(true);
			}
			if (Singleton<GameManager>.Instance.GameMode != null && Singleton<GameManager>.Instance.GameMode.Hud != null)
			{
				Singleton<GameManager>.Instance.GameMode.Hud.HudRadarComp.SetUfoToGood();
			}
			if ((bool)SoundGood)
			{
				SoundGood.Play();
			}
			if ((bool)m_pParent && (bool)m_pParent.Launcher)
			{
				m_pParent.Launcher.KartSound.PlayVoice(KartSound.EVoices.Bad);
			}
			componentInChildren.KartSound.PlayVoice(KartSound.EVoices.Good);
			componentInChildren.Anim.LaunchSuccessAnim(true);
		}
	}

	public Vector3 GetPosition()
	{
		return m_pTransform.position;
	}

	public void PlayLeaveAnim()
	{
		ApparitionParticleGO.transform.position = m_pTransform.position;
		ApparitionParticleGO.particleSystem.Play();
		DeactivateComponents();
		if ((bool)m_pAnimator)
		{
			m_pAnimator.Play("UFO_Leave");
		}
		m_bLeaveIsPlaying = true;
		if ((bool)SoundDisappear && (bool)SoundBad)
		{
			SoundDisappear.Play();
			SoundBad.Stop();
		}
	}

	public void Leave()
	{
		m_bLeaveIsPlaying = false;
		base.gameObject.SetActive(false);
	}

	public bool AnimLeaveIsPlaying()
	{
		return m_bLeaveIsPlaying;
	}

	public void Appear()
	{
		base.gameObject.SetActive(true);
		ApparitionParticleGO.transform.position = m_pTransform.position;
		ApparitionParticleGO.particleSystem.Play();
	}

	public void Idle()
	{
		if ((bool)m_pAnimator)
		{
			m_pAnimator.Play("UFO_Idle" + m_iUfoIndex);
		}
	}

	public void RemoveUFO(bool _All)
	{
		m_pParent.RemoveUFO(_All);
	}
}

using UnityEngine;

public class BonusEntity : MonoBehaviour
{
	public enum BonusState
	{
		BONUS_NONE,
		BONUS_LAUNCHREQUEST,
		BONUS_LAUNCHED,
		BONUS_ANIMLAUNCHED,
		BONUS_ONGROUND,
		BONUS_TRIGGERED,
		BONUS_DESTROYED
	}

	[SerializeField]
	[HideInInspector]
	public float ReactivationDelay;

	protected float m_fTimeBeforeReactivation;

	protected bool m_bActive;

	protected Collider m_pCollider;

	private float m_fTimer;

	protected Kart m_pLauncher;

	protected BonusState m_eState;

	protected Transform m_pTransform;

	private int m_iAnimParameterBehind;

	private int m_iAnimParameterFront;

	private int m_iAnimStateBehind;

	private int m_iAnimStateFront;

	protected bool m_bBehind;

	protected EITEM m_eItem;

	protected NetworkBonusEntity m_pNetworkBonusEntity;

	protected NetworkView m_pNetworkView;

	protected bool m_bSynchronizePosition;

	protected bool m_bSynchronizeRotation;

	public bool BSynchronizePosition
	{
		get
		{
			return m_bSynchronizePosition;
		}
	}

	public bool BSynchronizeRotation
	{
		get
		{
			return m_bSynchronizeRotation;
		}
	}

	public bool Activate
	{
		get
		{
			return m_bActive;
		}
		set
		{
			m_bActive = value;
		}
	}

	public float Timer
	{
		get
		{
			return m_fTimer;
		}
		set
		{
			m_fTimer = value;
		}
	}

	public Kart Launcher
	{
		get
		{
			return m_pLauncher;
		}
		set
		{
			m_pLauncher = value;
		}
	}

	public Vector3 LaunchDirection
	{
		get
		{
			return m_pLauncher.LaunchDirection;
		}
	}

	public Vector3 LaunchHorizontalDirection
	{
		get
		{
			return m_pLauncher.LaunchHorizontalDirection;
		}
	}

	public BonusEntity()
	{
		ReactivationDelay = 1f;
		m_fTimeBeforeReactivation = 0f;
		m_bActive = false;
		m_pCollider = null;
		m_fTimer = 0f;
		m_eState = BonusState.BONUS_NONE;
		m_bBehind = false;
		m_eItem = EITEM.ITEM_NONE;
		m_bSynchronizePosition = false;
	}

	public BonusState GetState()
	{
		return m_eState;
	}

	public Vector3 GetPosition()
	{
		return m_pTransform.position;
	}

	public Vector2 GetFlatPosition()
	{
		return new Vector2(m_pTransform.position.x, m_pTransform.position.z);
	}

	public bool IsStatic()
	{
		return m_bBehind;
	}

	public virtual void Awake()
	{
		Singleton<BonusMgr>.Instance.AddBonus(base.transform.parent.gameObject, m_eItem);
		m_pCollider = GetComponent<Collider>();
		m_pTransform = base.gameObject.transform;
		m_iAnimParameterBehind = Animator.StringToHash("Projectile_LaunchBack");
		m_iAnimParameterFront = Animator.StringToHash("Projectile_LaunchFront");
		m_iAnimStateBehind = Animator.StringToHash("Projectile_Launch.Projectile_LaunchBack");
		m_iAnimStateFront = Animator.StringToHash("Projectile_Launch.Projectile_LaunchFront");
		m_pNetworkBonusEntity = base.transform.parent.GetComponentInChildren<NetworkBonusEntity>();
		if (m_pNetworkBonusEntity != null)
		{
			m_pNetworkView = m_pNetworkBonusEntity.GetComponent<NetworkView>();
		}
		else
		{
			m_pNetworkView = base.transform.parent.GetComponentInChildren<NetworkView>();
		}
		SetActive(false);
	}

	public virtual void Start()
	{
	}

	public virtual void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
	}

	protected virtual void OnDestroy()
	{
	}

	public virtual void Update()
	{
		if (ReactivationDelay > 0f && m_fTimeBeforeReactivation > 0f)
		{
			m_fTimeBeforeReactivation -= Time.deltaTime;
			if (m_fTimeBeforeReactivation <= 0f)
			{
				SetActive(true);
			}
		}
	}

	public virtual void OnEnable()
	{
		m_fTimer = Time.time;
	}

	protected void ActivateGameObject(bool _Active)
	{
		base.gameObject.SetActive(_Active);
		if (m_pNetworkBonusEntity != null)
		{
			m_pNetworkBonusEntity.enabled = _Active;
		}
	}

	public virtual void SetActive(bool _Active)
	{
		m_bActive = _Active;
		m_fTimeBeforeReactivation = ((!_Active) ? ReactivationDelay : 0f);
		if ((bool)base.renderer)
		{
			base.renderer.enabled = _Active;
		}
		if ((bool)m_pCollider)
		{
			m_pCollider.enabled = _Active;
		}
	}

	public virtual void Launch()
	{
		m_eState = BonusState.BONUS_LAUNCHREQUEST;
		if (m_pLauncher != null)
		{
			m_pLauncher.Anim.LaunchBonusAnimOnCharacter((!m_bBehind) ? m_iAnimParameterFront : m_iAnimParameterBehind, (!m_bBehind) ? m_iAnimStateFront : m_iAnimStateBehind, true);
		}
	}

	public virtual void DoDestroy()
	{
		m_eState = BonusState.BONUS_DESTROYED;
	}

	public virtual void OnTriggerEnter(Collider other)
	{
		if (!m_bActive)
		{
			return;
		}
		if (Network.isServer && m_pNetworkView != null)
		{
			Transform parent = other.gameObject.transform.parent;
			Transform transform = null;
			GameObject gameObject = null;
			if ((bool)parent)
			{
				transform = parent.Find("NetworkDelegate");
				if ((bool)transform)
				{
					gameObject = transform.gameObject;
				}
			}
			if (other.gameObject.networkView != null || ((bool)gameObject && gameObject.networkView != null))
			{
				NetworkViewID networkViewID = ((!(other.gameObject.networkView != null)) ? gameObject.networkView.viewID : other.gameObject.networkView.viewID);
				m_pNetworkView.RPC("OnNetworkViewTriggerEnter", RPCMode.All, networkViewID);
			}
			else
			{
				m_pNetworkView.RPC("OnLayerTriggerEnter", RPCMode.All, other.gameObject.layer);
			}
		}
		else if (Network.peerType == NetworkPeerType.Disconnected || m_pNetworkView == null)
		{
			DoOnTriggerEnter(other.gameObject, other.gameObject.layer);
		}
	}

	public virtual void DoOnTriggerEnter(GameObject other, int otherlayer)
	{
	}

	public virtual void DoOnTriggerExit(GameObject other, int otherlayer)
	{
	}

	public void NetDestroy()
	{
		if (Network.isServer)
		{
			m_pNetworkView.RPC("OnNetworkDestroy", RPCMode.All);
		}
		else if (Network.peerType == NetworkPeerType.Disconnected)
		{
			DoDestroy();
		}
	}

	public void NetworkInitialize(NetworkViewID LauncherViewID)
	{
		if (Network.peerType != 0)
		{
			m_pNetworkView.RPC("NetworkInitialize", RPCMode.Others, LauncherViewID);
		}
	}

	public void DoNetworkInitialize(NetworkViewID LauncherViewID)
	{
		if (Activate)
		{
			SetActive(false);
		}
		Launcher = Singleton<BonusMgr>.Instance.Karts[LauncherViewID];
	}

	public void NetLaunch(NetworkViewID launcherViewID)
	{
		m_pNetworkView.RPC("Launch", RPCMode.All, launcherViewID);
	}
}

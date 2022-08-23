using UnityEngine;

public abstract class RcVehiclePhysic : MonoBehaviour
{
	private const int MAX_VEHICLE_COLLISION_LISTENER = 8;

	protected RcVehicle m_pVehicle;

	protected Rigidbody m_pVehicleRigidBody;

	protected int m_iGroundSurface;

	protected Vector3 m_fGroundNormal = Vector3.up;

	protected int m_iNbWheels;

	protected float m_fWVelocity;

	protected bool m_bTouchedDeath;

	protected RcCollisionListener[] m_pCollisionListeners;

	protected Transform m_pVehicleRigidBodyTransform;

	protected Transform m_pTransform;

	protected float m_fDeathDelay;

	protected bool m_bEnabled;

	public float FDeathDelay
	{
		get
		{
			return m_fDeathDelay;
		}
	}

	public bool BTouchedDeath
	{
		get
		{
			return m_bTouchedDeath;
		}
	}

	public bool Enable
	{
		get
		{
			return m_bEnabled;
		}
		set
		{
			m_bEnabled = value;
		}
	}

	public RcVehiclePhysic()
	{
		m_pVehicle = null;
		m_pVehicleRigidBody = null;
		m_iNbWheels = 0;
		m_fWVelocity = 0f;
		m_bTouchedDeath = false;
		m_fDeathDelay = 1.5f;
		m_pCollisionListeners = new RcCollisionListener[8];
		m_bEnabled = true;
	}

	public void ResetTouchedDeath()
	{
		m_bTouchedDeath = false;
	}

	public Transform GetTransform()
	{
		return m_pTransform;
	}

	public int GetNbWheels()
	{
		return m_iNbWheels;
	}

	public virtual void Start()
	{
		m_pVehicleRigidBody = base.transform.parent.GetComponentInChildren<Rigidbody>();
		m_pVehicleRigidBodyTransform = m_pVehicleRigidBody.transform;
		m_pVehicle = base.transform.parent.GetComponentInChildren<RcVehicle>();
		m_pTransform = base.transform;
	}

	public virtual void Stop()
	{
		for (int i = 0; i < 8; i++)
		{
			m_pCollisionListeners[i] = null;
		}
	}

	public virtual void Update()
	{
		m_fWVelocity = m_pVehicle.GetWheelSpeedMS() * (1f - m_pVehicle.GetHandicap());
	}

	public virtual bool IsGoingTooFast()
	{
		return false;
	}

	public void AddCollisionListener(RcCollisionListener _Listener)
	{
		for (int i = 0; i < 8; i++)
		{
			if (m_pCollisionListeners[i] == null)
			{
				m_pCollisionListeners[i] = _Listener;
				break;
			}
		}
	}

	public void RemoveCollisionListener(RcCollisionListener _Listener)
	{
		for (int i = 0; i < 8; i++)
		{
			if (m_pCollisionListeners[i] == _Listener)
			{
				m_pCollisionListeners[i] = null;
				break;
			}
		}
	}

	public virtual void FireOnCollision(CollisionData collisionInfos)
	{
		for (int i = 0; i < 8; i++)
		{
			if (m_pCollisionListeners[i] != null)
			{
				m_pCollisionListeners[i].OnCollision(collisionInfos);
			}
		}
	}

	public virtual float GetDriftRatio()
	{
		return 0f;
	}

	public bool HasVehicleBody()
	{
		return m_pVehicleRigidBody != null;
	}

	public Rigidbody GetVehicleBody()
	{
		return m_pVehicleRigidBody;
	}

	public Transform GetVehicleBodyTransform()
	{
		return m_pVehicleRigidBodyTransform;
	}

	private GameObject GetVehicleNode()
	{
		return base.gameObject;
	}

	public RcVehicle GetVehicle()
	{
		return m_pVehicle;
	}

	public int GetGroundSurface()
	{
		return m_iGroundSurface;
	}

	public abstract Vector3 GetLinearVelocity();

	public abstract Vector3 ForecastPosition(float dtSec);

	public abstract void SetGripFactor(float _factor);

	public abstract float GetGripFactor();

	public abstract float GetWheelSpeed();

	public abstract float GetFrontToRearWheelLength();

	public void Teleport(Vector3 vTeleportPos, Quaternion qTeleportOrientation)
	{
		Teleport(vTeleportPos, qTeleportOrientation, Vector3.zero);
	}

	public void Teleport(Vector3 vTeleportPos, Quaternion qTeleportOrientation, Vector3 linearVelocity)
	{
		Teleport(vTeleportPos, qTeleportOrientation, linearVelocity, Vector3.zero);
	}

	public abstract void Teleport(Vector3 vTeleportPos, Quaternion qTeleportOrientation, Vector3 linearVelocity, Vector3 angularVelocity);

	public abstract void NetMove(Vector3 vTeleportPos, Quaternion qTeleportOrientation, Vector3 linearVelocity, Vector3 angularVelocity);

	public abstract RcPhysicWheel[] GetWheels();

	public abstract void Reset();

	public abstract Vector3 GetAngularVelocity();

	public abstract void SetLinearVelocity(Vector3 velocity);

	public abstract void SetAngularVelocity(Vector3 velocity);
}

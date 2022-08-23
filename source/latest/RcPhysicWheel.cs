using System;
using System.Collections.Generic;
using UnityEngine;

public class RcPhysicWheel : MonoBehaviour
{
	public enum WheelSide
	{
		Left,
		Right,
		Center
	}

	public enum WheelAxle
	{
		Front,
		Rear,
		Center
	}

	public Transform m_pVehicleTransform;

	public Transform m_pWheelMeshTransform;

	public WheelSide m_eSide = WheelSide.Center;

	public WheelAxle m_eAxle = WheelAxle.Center;

	public string materialSuffix = "Wheel";

	public bool m_bAnimateRollByRotation;

	public bool m_bAnimateRollByUV = true;

	public bool m_bAnimateSteering = true;

	public bool m_bAnimateSqueeze = true;

	public float m_fUVAnimScale = -1f;

	protected Vector3 m_vMaxSqueeze = Vector3.one;

	public bool m_bMotorOn = true;

	public bool m_bSteeringOn = true;

	public bool m_bMirrorMesh;

	public Vector3 m_vOffset = Vector3.zero;

	public Vector3 m_vModelToMesh = Vector3.zero;

	public float m_fSpring = 30f;

	public float m_fDamping = 3f;

	public float m_fMaxCompression = 0.3f;

	public Vector2 m_vCollisionGrip = Vector2.one;

	public Vector2 m_vDriftGrip = Vector2.one;

	public Vector2 m_vNormalGrip = Vector2.one;

	public float m_fRadius = 0.3f;

	protected float m_fStretch;

	protected float m_fMovement;

	protected GroundCharac m_oGroundCharac;

	protected Vector3 m_vRestContactPoint = Vector3.zero;

	protected bool m_bOnGround;

	protected float m_fDifferentialFactor;

	protected float m_fGripFactor;

	protected float m_fForwardGrip;

	protected float m_fSideGrip;

	protected float m_fSideSlip;

	protected float m_fContactDepth;

	protected float m_fSuspensionFactor = 1f;

	protected float m_fMassRep;

	protected float m_fSteeringAngle;

	protected float m_fSteeringAnimationAngle;

	protected float m_fGripMaxNormalRatio;

	protected float m_fAnimDistance;

	protected float m_fHandBrake;

	protected float m_fStabilityControl;

	protected Vector3 m_vVelocity = Vector3.zero;

	protected float m_fSurfaceNoise;

	protected Transform m_pVehicleRoot;

	protected List<Material> m_pWheelMaterial = new List<Material>();

	protected Quaternion hack;

	public Vector3 VMaxSqueeze
	{
		set
		{
			m_vMaxSqueeze = value;
		}
	}

	public bool BOnGround
	{
		get
		{
			return m_bOnGround;
		}
	}

	public float FGripFactor
	{
		get
		{
			return m_fGripFactor;
		}
		set
		{
			m_fGripFactor = value;
		}
	}

	public float FForwardGrip
	{
		get
		{
			return m_fForwardGrip;
		}
		set
		{
			m_fForwardGrip = value;
		}
	}

	public float FHandBrake
	{
		get
		{
			return m_fHandBrake;
		}
		set
		{
			m_fHandBrake = value;
		}
	}

	public float FSideGrip
	{
		get
		{
			return m_fSideGrip;
		}
		set
		{
			m_fSideGrip = value;
		}
	}

	public bool BMotorOn
	{
		get
		{
			return m_bMotorOn;
		}
	}

	public Vector2 VCollisionGrip
	{
		get
		{
			return m_vCollisionGrip;
		}
	}

	public Vector2 VDriftGrip
	{
		get
		{
			return m_vDriftGrip;
		}
	}

	public Vector2 VNormalGrip
	{
		get
		{
			return m_vNormalGrip;
		}
	}

	public float FSideSlip
	{
		get
		{
			return m_fSideSlip;
		}
		set
		{
			m_fSideSlip = value;
		}
	}

	public float FSurfaceNoise
	{
		get
		{
			return m_fSurfaceNoise;
		}
		set
		{
			m_fSurfaceNoise = value;
		}
	}

	public float FStretch
	{
		get
		{
			return m_fStretch;
		}
	}

	public GroundCharac OGroundCharac
	{
		get
		{
			return m_oGroundCharac;
		}
	}

	public bool BSteeringOn
	{
		get
		{
			return m_bSteeringOn;
		}
		set
		{
			m_bSteeringOn = value;
		}
	}

	public float FSteeringAngle
	{
		get
		{
			return m_fSteeringAngle;
		}
		set
		{
			m_fSteeringAngle = value;
		}
	}

	public float FSteeringAnimationAngle
	{
		get
		{
			return m_fSteeringAnimationAngle;
		}
		set
		{
			m_fSteeringAnimationAngle = value;
		}
	}

	public float FDifferentialFactor
	{
		get
		{
			return m_fDifferentialFactor;
		}
		set
		{
			m_fDifferentialFactor = value;
		}
	}

	public Vector3 VRestContactPoint
	{
		get
		{
			return m_vRestContactPoint;
		}
	}

	public Vector3 VOffset
	{
		get
		{
			return m_vOffset;
		}
	}

	public WheelAxle EAxle
	{
		get
		{
			return m_eAxle;
		}
	}

	public WheelSide ESide
	{
		get
		{
			return m_eSide;
		}
	}

	public float FGripMaxNormalRatio
	{
		get
		{
			return m_fGripMaxNormalRatio;
		}
		set
		{
			m_fGripMaxNormalRatio = value;
		}
	}

	public float FMassRep
	{
		get
		{
			return m_fMassRep;
		}
		set
		{
			m_fMassRep = value;
		}
	}

	public float FStabilityControl
	{
		get
		{
			return m_fStabilityControl;
		}
		set
		{
			m_fStabilityControl = value;
		}
	}

	public Transform VehicleRoot
	{
		get
		{
			return m_pVehicleRoot;
		}
		set
		{
			m_pVehicleRoot = value;
			hack = Quaternion.Inverse(m_pVehicleRoot.localRotation);
		}
	}

	public void Awake()
	{
		if (m_pWheelMeshTransform == null)
		{
			m_pWheelMeshTransform = base.transform;
		}
	}

	public virtual void Start()
	{
		m_fAnimDistance = (float)Math.PI * 2f * m_fRadius;
		Renderer[] componentsInChildren = m_pVehicleTransform.GetComponentsInChildren<Renderer>();
		m_pWheelMaterial.Clear();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			Material[] materials = renderer.materials;
			foreach (Material material in materials)
			{
				if (material.name.Contains(materialSuffix))
				{
					m_pWheelMaterial.Add(material);
					break;
				}
			}
		}
	}

	public void ResetMotion()
	{
		m_fMovement = 0f;
		m_vVelocity = Vector3.zero;
	}

	public void UpdateMotion(float fWheelVelocity, Vector3 vVehicleVelocity)
	{
		m_fMovement += fWheelVelocity * Time.fixedDeltaTime;
		m_vVelocity = vVehicleVelocity;
	}

	public float GetRestStretch()
	{
		return m_fMassRep * Vector3.Dot(Physics.gravity, Vector3.down) / m_fSpring;
	}

	public virtual Vector3 GetWorldPos()
	{
		Quaternion rotation = m_pVehicleRoot.rotation;
		Vector3 position = m_pVehicleRoot.position;
		return position + rotation * hack * (m_vModelToMesh + m_vOffset + m_fStretch * Vector3.up);
	}

	public virtual void UpdateNode()
	{
		if (m_pVehicleRoot == null)
		{
			return;
		}
		Quaternion quaternion = m_pVehicleRoot.rotation * hack;
		m_pWheelMeshTransform.position = GetWorldPos();
		Vector3 axis = quaternion * Vector3.up;
		if (m_bAnimateSqueeze)
		{
			float num = (1f - Vector3.Dot(m_vMaxSqueeze, Vector3.up)) * m_fRadius;
			float num2 = 0f;
			if (m_fStretch > m_fMaxCompression - 2f * num)
			{
				num2 = RcUtils.LinearInterpolation(m_fMaxCompression - 2f * num, 0f, m_fMaxCompression, 1f, m_fStretch, true);
				m_pWheelMeshTransform.position -= quaternion * (0.5f * num2 * num * Vector3.up);
			}
			Vector3 one = Vector3.one;
			one.x = RcUtils.LinearInterpolation(0f, 1f, 1f, m_vMaxSqueeze.x, num2, true);
			one.y = RcUtils.LinearInterpolation(0f, 1f, 1f, m_vMaxSqueeze.y, num2, true);
			one.z = RcUtils.LinearInterpolation(0f, 1f, 1f, m_vMaxSqueeze.z, num2, true);
			m_pWheelMeshTransform.localScale = one;
		}
		Quaternion quaternion2 = quaternion;
		if (m_bMirrorMesh)
		{
			Quaternion quaternion3 = Quaternion.AngleAxis(180f, Vector3.up);
			quaternion2 *= quaternion3;
		}
		if (m_bSteeringOn && m_bAnimateSteering)
		{
			float angle = m_fSteeringAnimationAngle * 180f / (float)Math.PI;
			Quaternion quaternion4 = Quaternion.AngleAxis(angle, axis);
			quaternion2 = quaternion4 * quaternion2;
		}
		if (m_fAnimDistance > 0f)
		{
			int num3 = (int)(m_fMovement / m_fAnimDistance);
			float num4 = m_fMovement / m_fAnimDistance - (float)num3;
			if (m_fHandBrake > 0f)
			{
				num4 *= 1f - m_fHandBrake;
			}
			if (m_bMirrorMesh)
			{
				num4 *= -1f;
			}
			if (num4 != 0f)
			{
				if (m_bAnimateRollByUV && m_pWheelMaterial.Count > 0)
				{
					foreach (Material item in m_pWheelMaterial)
					{
						Vector2 mainTextureOffset = item.mainTextureOffset;
						mainTextureOffset.x = num4 * m_fUVAnimScale;
						item.mainTextureOffset = mainTextureOffset;
					}
				}
				else if (m_bAnimateRollByRotation)
				{
					Quaternion quaternion5 = Quaternion.AngleAxis((0f - num4) * 360f, Vector3.left);
					quaternion2 *= quaternion5;
				}
			}
		}
		m_pWheelMeshTransform.rotation = quaternion2;
	}

	public virtual void DebugDrawInfo(int xOffset, int yOffset)
	{
		string text = ((!m_bOnGround) ? "air" : "ground");
		GUI.contentColor = Color.green;
		GUI.Label(new Rect(xOffset, yOffset, 200f, 200f), text);
		Quaternion rotation = m_pVehicleTransform.rotation;
		Vector3 lhs = rotation * Vector3.forward;
		Vector3 lhs2 = rotation * Vector3.left;
		float num = Vector3.Dot(lhs, m_vVelocity) * 3.6f;
		float num2 = Vector3.Dot(lhs2, m_vVelocity) * 3.6f;
		text = string.Format("fV : {0:000.00}", num);
		GUI.Label(new Rect(xOffset, yOffset + 20, 200f, 200f), text);
		text = string.Format("sV : {0:000.00}", num2);
		GUI.Label(new Rect(xOffset, yOffset + 40, 200f, 200f), text);
		text = string.Format("strech : {0:0.00}", m_fStretch);
		GUI.Label(new Rect(xOffset, yOffset + 60, 200f, 200f), text);
	}
}

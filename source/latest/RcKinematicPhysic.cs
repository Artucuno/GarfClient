using System;
using System.Collections.Generic;
using UnityEngine;

public class RcKinematicPhysic : RcVehiclePhysic
{
	[Serializable]
	public class SurfaceResponse
	{
		public LayerMask layers;

		public float maxSpeedKph;

		public float bounce;

		public float bounceMinSpeedKph;

		public bool collidesWithChassis;

		public bool collidesWithWheels;

		public bool gripsWithWheels;

		public bool kills;

		public float deathDelay;
	}

	public enum eMoveCheckResult
	{
		R_NO_COLL,
		R_SLIDE_COLL,
		R_BOUNCE_COLL,
		RESULT_COUNT
	}

	protected Vector3 m_vImpulse = Vector3.zero;

	public Vector3 m_fAirDamping = Vector3.one;

	public Vector3 m_fInertiaModeDamping = Vector3.one;

	protected Vector3 m_fCurrentInertiaModeDamping = Vector3.one;

	public float m_fAirAdditionnalGravity = 10f;

	public float m_fGroundAdditionnalGravity;

	public float m_fCollisionReorientationInertia = 0.98f;

	public bool m_bDebugInfo;

	public float m_fMass = 1f;

	protected LayerMask m_DeathMask;

	protected LayerMask m_SphereCollidesWith;

	protected LayerMask m_WheelsCollideWith;

	protected LayerMask m_WheelsGripWith;

	protected Vector3 m_vCombineTrickMv = Vector3.zero;

	protected Vector3 m_vCollisionMove = Vector3.zero;

	public float m_fSkinWidth = 0.02f;

	public Transform m_pVehicleMesh;

	protected bool m_bAllowedAirControl;

	protected bool m_flying;

	protected bool m_prevFlying;

	protected bool m_landing;

	protected float mCollisionDotProdInertia;

	protected Vector3 mPreviousPosition;

	protected Quaternion mPreviousOrientation;

	protected Vector3 mLinearVelocity;

	protected Vector3 mAngularVelocity;

	protected Vector3 m_vTransSum;

	protected Quaternion mRotationQuaternion;

	protected RcKinematicWheel[] m_pWheels;

	protected float m_fFrontToRearWheel;

	protected SphereCollider sphere;

	protected bool m_bInertiaMode;

	protected float m_fDriftAngle;

	public float m_fMaxDriftAngle = 35f;

	public float m_fTimeToMaxDriftAngle = 0.3f;

	public float m_fGripRecoveryPrc = 0.75f;

	public float m_fMaxIncline = 40f;

	protected float m_fAvgFriction;

	protected bool bounced;

	public float m_fVehicleCollSideImpulseIntensity = 6f;

	public float m_fVehicleCollUpImpulseIntensity = 1.3f;

	public float m_fVehicleCollBackPrc = 0.1f;

	public float m_fVehicleCollInertiaDelay = 0.2f;

	public Vector3 m_fVehicleCollInertiaDamping = Vector3.zero;

	public float m_fWallReorientateSpeed = 0.5f;

	protected float m_fInertiaTimer;

	public List<SurfaceResponse> m_vSurfaceResponses;

	protected CollisionData collData;

	public bool m_bSimplifiedWheels;

	public int m_iCountFixedUpdate;

	private static int gsInitCountFixedUpdate;

	public int DeathMaskValue
	{
		get
		{
			return m_DeathMask.value;
		}
	}

	public bool Flying
	{
		get
		{
			return m_flying;
		}
	}

	public Vector3 LinearVelocity
	{
		get
		{
			return mLinearVelocity;
		}
	}

	public Vector3 CombineTrickMv
	{
		get
		{
			return m_vCombineTrickMv;
		}
	}

	public bool BInertiaMode
	{
		get
		{
			return m_bInertiaMode;
		}
	}

	public RcKinematicPhysic()
	{
		m_vImpulse = Vector3.zero;
		mPreviousPosition = Vector3.zero;
		mLinearVelocity = Vector3.zero;
		mAngularVelocity = Vector3.zero;
		mPreviousOrientation = Quaternion.identity;
		m_vTransSum = Vector3.zero;
		mRotationQuaternion = Quaternion.identity;
		mCollisionDotProdInertia = 1f;
		m_flying = false;
		m_prevFlying = false;
		m_landing = false;
		m_fInertiaTimer = 0f;
		m_bInertiaMode = false;
		m_vCombineTrickMv = Vector3.zero;
		m_vCollisionMove = Vector3.zero;
		m_bAllowedAirControl = true;
		m_fDriftAngle = 0f;
		m_fAvgFriction = 1f;
		m_SphereCollidesWith.value = 0;
		m_WheelsCollideWith.value = 0;
		m_WheelsGripWith.value = 0;
		m_vSurfaceResponses = new List<SurfaceResponse>();
		collData = default(CollisionData);
		m_iCountFixedUpdate = gsInitCountFixedUpdate++;
	}

	public void Awake()
	{
		sphere = base.transform.parent.GetComponentInChildren<SphereCollider>();
		foreach (SurfaceResponse vSurfaceResponse in m_vSurfaceResponses)
		{
			if (vSurfaceResponse.collidesWithChassis)
			{
				m_SphereCollidesWith.value |= vSurfaceResponse.layers.value;
			}
			if (vSurfaceResponse.collidesWithWheels)
			{
				m_WheelsCollideWith.value |= vSurfaceResponse.layers.value;
			}
			if (vSurfaceResponse.gripsWithWheels)
			{
				m_WheelsGripWith.value |= vSurfaceResponse.layers.value;
			}
			if (vSurfaceResponse.kills)
			{
				m_DeathMask.value |= vSurfaceResponse.layers.value;
			}
		}
		ConfigureWheels();
	}

	public override bool IsGoingTooFast()
	{
		foreach (SurfaceResponse vSurfaceResponse in m_vSurfaceResponses)
		{
			float maxSpeedKph = vSurfaceResponse.maxSpeedKph;
			for (int i = 0; i < m_iNbWheels; i++)
			{
				int surface = m_pWheels[i].OGroundCharac.surface;
				if (m_pWheels[i].BOnGround && ((1 << surface) & vSurfaceResponse.layers.value) != 0)
				{
					if (m_fWVelocity * 3.6f > maxSpeedKph)
					{
						return true;
					}
					return false;
				}
			}
		}
		return false;
	}

	public bool IsLocked()
	{
		return m_pVehicle != null && m_pVehicle.IsLocked();
	}

	public override void Start()
	{
		base.Start();
		for (int i = 0; i < m_iNbWheels; i++)
		{
			m_pWheels[i].SetMasks(m_WheelsCollideWith, m_WheelsGripWith, m_DeathMask);
		}
	}

	public void ConfigureWheels()
	{
		m_pWheels = base.transform.parent.GetComponentsInChildren<RcKinematicWheel>();
		m_iNbWheels = m_pWheels.Length;
		RcPhysicWheel rcPhysicWheel = null;
		RcPhysicWheel rcPhysicWheel2 = null;
		for (int i = 0; i < m_iNbWheels; i++)
		{
			m_pWheels[i].FMassRep = 1f / (float)m_iNbWheels;
			if (m_pWheels[i].EAxle == RcPhysicWheel.WheelAxle.Rear)
			{
				rcPhysicWheel2 = m_pWheels[i];
				continue;
			}
			if (m_pWheels[i].EAxle == RcPhysicWheel.WheelAxle.Front)
			{
				rcPhysicWheel = m_pWheels[i];
				continue;
			}
			if (!rcPhysicWheel)
			{
				rcPhysicWheel = m_pWheels[i];
			}
			if (!rcPhysicWheel2)
			{
				rcPhysicWheel2 = m_pWheels[i];
			}
		}
		if (rcPhysicWheel == null || rcPhysicWheel2 == null)
		{
			m_fFrontToRearWheel = 1f;
		}
		else
		{
			m_fFrontToRearWheel = Mathf.Abs(Vector3.Dot(rcPhysicWheel.VOffset - rcPhysicWheel2.VOffset, Vector3.forward));
		}
	}

	public void SwitchToInertiaMode(float duration, Vector3 impulse, bool allowAirControl, bool cumulative)
	{
		m_fCurrentInertiaModeDamping = m_fInertiaModeDamping;
		if (!cumulative)
		{
			m_vImpulse = impulse;
			if (m_fInertiaTimer < duration)
			{
				m_fInertiaTimer = duration;
			}
		}
		else
		{
			m_fInertiaTimer += duration;
			m_vImpulse += impulse;
		}
		m_bInertiaMode = true;
		m_landing = false;
		m_bAllowedAirControl &= allowAirControl;
	}

	public void SwitchToInertiaMode(float duration, Vector3 impulse, bool allowAirControl, bool cumulative, Vector3 CustomDamping)
	{
		SwitchToInertiaMode(duration, impulse, allowAirControl, cumulative);
		m_fCurrentInertiaModeDamping = CustomDamping;
	}

	public override Vector3 GetLinearVelocity()
	{
		return mLinearVelocity;
	}

	public override Vector3 ForecastPosition(float dtSec)
	{
		return m_pVehicleRigidBodyTransform.position + dtSec * mLinearVelocity;
	}

	public override void SetGripFactor(float _factor)
	{
	}

	public override float GetGripFactor()
	{
		return 1f;
	}

	public override float GetWheelSpeed()
	{
		return m_fWVelocity;
	}

	public override float GetFrontToRearWheelLength()
	{
		return m_fFrontToRearWheel;
	}

	public override void Teleport(Vector3 vTeleportPos, Quaternion qTeleportOrientation, Vector3 linearVelocity, Vector3 angularVelocity)
	{
		if (vTeleportPos != m_pVehicleRigidBodyTransform.position)
		{
			m_pVehicleRigidBodyTransform.position = vTeleportPos;
		}
		SetOrientation(qTeleportOrientation);
		Reset();
		mPreviousPosition = vTeleportPos - linearVelocity * Time.fixedDeltaTime;
		if (angularVelocity != Vector3.zero)
		{
			float angle = Time.fixedDeltaTime * angularVelocity.magnitude * 57.29578f;
			Quaternion quaternion = Quaternion.AngleAxis(angle, -angularVelocity);
			mPreviousOrientation = quaternion * qTeleportOrientation;
		}
		else
		{
			mPreviousOrientation = qTeleportOrientation;
		}
		m_fInertiaTimer = 0f;
		mLinearVelocity = linearVelocity;
		mAngularVelocity = angularVelocity;
		m_vTransSum = Vector3.zero;
		mRotationQuaternion = Quaternion.identity;
		m_vCombineTrickMv = Vector3.zero;
		m_vCollisionMove = Vector3.zero;
		for (int i = 0; i < m_iNbWheels; i++)
		{
			m_pWheels[i].ResetMotion();
		}
		RetroApplyWheelSpeed();
		UpdateWheelGroundState();
	}

	public override void NetMove(Vector3 vTeleportPos, Quaternion qTeleportOrientation, Vector3 linearVelocity, Vector3 angularVelocity)
	{
		if (vTeleportPos != m_pVehicleRigidBodyTransform.position)
		{
			m_pVehicleRigidBodyTransform.position = vTeleportPos;
		}
		SetOrientation(qTeleportOrientation);
		mPreviousPosition = vTeleportPos - linearVelocity * Time.fixedDeltaTime;
		if (angularVelocity != Vector3.zero)
		{
			float angle = Time.fixedDeltaTime * angularVelocity.magnitude * 57.29578f;
			Quaternion quaternion = Quaternion.AngleAxis(angle, -angularVelocity);
			mPreviousOrientation = quaternion * qTeleportOrientation;
		}
		else
		{
			mPreviousOrientation = qTeleportOrientation;
		}
		m_fInertiaTimer = 0f;
		mLinearVelocity = linearVelocity;
		mAngularVelocity = angularVelocity;
		m_vTransSum = Vector3.zero;
		mRotationQuaternion = Quaternion.identity;
		m_vCombineTrickMv = Vector3.zero;
		m_vCollisionMove = Vector3.zero;
		RetroApplyWheelSpeed();
	}

	public override RcPhysicWheel[] GetWheels()
	{
		return m_pWheels;
	}

	public override void Reset()
	{
		m_iGroundSurface = 0;
		m_fGroundNormal = Vector3.up;
		mPreviousPosition = Vector3.zero;
		mPreviousOrientation = Quaternion.identity;
		mLinearVelocity = Vector3.zero;
		mAngularVelocity = Vector3.zero;
		m_fInertiaTimer = 0f;
		m_vImpulse = Vector3.zero;
		m_bAllowedAirControl = true;
		mCollisionDotProdInertia = 1f;
		m_flying = false;
		m_prevFlying = false;
		m_landing = false;
		m_fWVelocity = 0f;
		m_fAvgFriction = 1f;
		for (int i = 0; i < m_iNbWheels; i++)
		{
			m_pWheels[i].Reset();
		}
	}

	public override void Update()
	{
		m_fWVelocity = m_pVehicle.GetWheelSpeedMS() * (1f - m_pVehicle.GetHandicap());
		for (int i = 0; i < m_iNbWheels; i++)
		{
			m_pWheels[i].UpdateNode();
		}
	}

	public void FixedUpdate()
	{
		if (!m_bEnabled)
		{
			return;
		}
		float arcadeDriftFactor = m_pVehicle.GetArcadeDriftFactor();
		float fixedDeltaTime = Time.fixedDeltaTime;
		float num = m_fMaxDriftAngle * (float)Math.PI / 180f;
		float num2 = arcadeDriftFactor * num;
		float num3 = num2 - m_fDriftAngle;
		float num4 = num / m_fTimeToMaxDriftAngle;
		if (num3 > 0f)
		{
			m_fDriftAngle += num4 * fixedDeltaTime;
			if (m_fDriftAngle > num2)
			{
				m_fDriftAngle = num2;
			}
		}
		else if (num3 < 0f)
		{
			m_fDriftAngle -= num4 * fixedDeltaTime;
			if (m_fDriftAngle < num2)
			{
				m_fDriftAngle = num2;
			}
		}
		Vector3 position = m_pVehicleRigidBodyTransform.position;
		Quaternion rotation = m_pVehicleRigidBodyTransform.rotation;
		float angle;
		(Quaternion.Inverse(mPreviousOrientation) * rotation).ToAngleAxis(out angle, out mAngularVelocity);
		mAngularVelocity.Normalize();
		mAngularVelocity *= angle * (float)Math.PI / 180f / fixedDeltaTime;
		mPreviousPosition = position;
		mPreviousOrientation = rotation;
		if (!m_flying && (m_prevFlying || m_bInertiaMode))
		{
			Vector3 lhs = rotation * Vector3.forward;
			Vector3 rhs = Vector3.Cross(lhs, m_fGroundNormal);
			lhs = Vector3.Cross(m_fGroundNormal, rhs);
			lhs.Normalize();
			float fWVelocity = Vector3.Dot(mLinearVelocity, lhs);
			m_fWVelocity = fWVelocity;
			m_pVehicle.SetWheelSpeedMS(m_fWVelocity / (1f - m_pVehicle.GetHandicap()));
		}
		if (m_flying && m_bAllowedAirControl)
		{
			Vector3 rhs2 = mLinearVelocity;
			rhs2.y = 0f;
			Vector3 lhs2 = rotation * Vector3.forward;
			lhs2.y = 0f;
			lhs2.Normalize();
			float num5 = Vector3.Dot(lhs2, rhs2);
			float num6 = m_pVehicle.GetSteeringFactor();
			if (arcadeDriftFactor != 0f)
			{
				num6 = RcUtils.LinearInterpolation(1f, 0f, -1f, num6, num6 * arcadeDriftFactor, true);
			}
			if (num6 != 0f)
			{
				float num7 = (0f - num6) * fixedDeltaTime;
				if (num5 < -0.1f)
				{
					num7 *= -1f;
				}
				Quaternion quaternion = Quaternion.AngleAxis(num7 * 180f / (float)Math.PI, Vector3.up);
				Rotate(quaternion);
				mLinearVelocity = quaternion * mLinearVelocity;
			}
		}
		m_vTransSum = Vector3.zero;
		mRotationQuaternion = Quaternion.identity;
		if (!m_flying && !m_bInertiaMode && m_fAvgFriction != 0f)
		{
			if (m_fWVelocity != 0f)
			{
				float steeringAngle = m_pVehicle.GetSteeringAngle();
				Vector3 rhs3 = rotation * Vector3.forward;
				Vector3 vector = ((!m_landing) ? (rotation * Vector3.up) : m_fGroundNormal);
				Vector3 vector2 = Vector3.Cross(vector, rhs3);
				vector2.Normalize();
				rhs3 = Vector3.Cross(vector2, vector);
				rhs3.Normalize();
				Vector3 vTransSum = rhs3;
				vTransSum *= m_fWVelocity * fixedDeltaTime * m_fAvgFriction;
				if (Mathf.Abs(steeringAngle) > 0.0001f)
				{
					float num8 = Mathf.Tan(steeringAngle);
					float num9 = -1f * m_fFrontToRearWheel / num8;
					Vector3 vector3 = position;
					RcPhysicWheel rcPhysicWheel = null;
					for (int i = 0; i < m_iNbWheels; i++)
					{
						if (m_pWheels[i].EAxle == RcPhysicWheel.WheelAxle.Rear)
						{
							rcPhysicWheel = m_pWheels[i];
							break;
						}
					}
					Vector3 vector4 = vector3;
					if ((bool)rcPhysicWheel)
					{
						vector4 += rhs3 * rcPhysicWheel.VOffset.z;
					}
					Vector3 vector5 = vector4 + vector2 * num9;
					float num10 = m_fWVelocity * fixedDeltaTime / num9;
					Quaternion quaternion2 = Quaternion.AngleAxis(num10 * 180f / (float)Math.PI, vector);
					vector3 -= vector5;
					vector3 = quaternion2 * vector3;
					vector3 += vector5;
					m_vTransSum = vector3 - position;
					vector4 -= vector5;
					vector4 = quaternion2 * vector4;
					vector4 += vector5;
					Vector3 vector6 = vector3 - vector4;
					vector6.Normalize();
					Vector3 lhs3 = rotation * Vector3.up;
					Vector3 rhs4 = Vector3.Cross(lhs3, vector6);
					rhs4.Normalize();
					lhs3 = Vector3.Cross(vector6, rhs4);
					Quaternion quaternion3 = Quaternion.LookRotation(vector6, lhs3);
					mRotationQuaternion = Quaternion.Inverse(rotation) * quaternion3;
				}
				else
				{
					m_vTransSum = vTransSum;
				}
			}
		}
		else
		{
			Vector3 zero = Vector3.zero;
			if (!bounced)
			{
				zero = mLinearVelocity;
				zero.y -= m_vCollisionMove.y / fixedDeltaTime;
			}
			bounced = false;
			zero += m_vImpulse;
			m_vCollisionMove = Vector3.zero;
			m_vImpulse = Vector3.zero;
			m_vTransSum = zero * fixedDeltaTime;
		}
		if (m_bInertiaMode)
		{
			m_fInertiaTimer -= fixedDeltaTime;
			if (m_fInertiaTimer < 0f)
			{
				m_fInertiaTimer = 0f;
				m_bInertiaMode = false;
				m_bAllowedAirControl = true;
			}
		}
		if (m_fWVelocity != 0f && m_pVehicleMesh != null)
		{
			Quaternion quaternion4 = Quaternion.AngleAxis(m_fDriftAngle * 180f / (float)Math.PI, Vector3.up);
			Quaternion quaternion5 = Quaternion.Inverse(m_pVehicleMesh.localRotation) * quaternion4;
			m_pVehicleMesh.localRotation = quaternion4;
			if (arcadeDriftFactor == 0f)
			{
				Vector3 axis;
				float angle2;
				quaternion5.ToAngleAxis(out angle2, out axis);
				angle2 *= 0f - m_fGripRecoveryPrc;
				mRotationQuaternion *= Quaternion.AngleAxis(angle2, axis);
			}
		}
		Vector3 zero2 = Vector3.zero;
		if (!m_bInertiaMode || m_flying)
		{
			zero2 += Physics.gravity;
		}
		if (m_bInertiaMode || m_fAvgFriction == 0f)
		{
			if (m_fCurrentInertiaModeDamping != Vector3.zero)
			{
				Vector3 vector7 = rotation * Vector3.right;
				Vector3 vector8 = rotation * Vector3.up;
				Vector3 vector9 = rotation * Vector3.forward;
				float num11 = Vector3.Dot(mLinearVelocity, vector7);
				float num12 = Vector3.Dot(mLinearVelocity, vector8);
				float num13 = Vector3.Dot(mLinearVelocity, vector9);
				zero2 -= num11 * m_fCurrentInertiaModeDamping.x * vector7;
				if (num12 < 0f)
				{
					zero2 -= num12 * m_fCurrentInertiaModeDamping.y * vector8;
				}
				zero2 -= num13 * m_fCurrentInertiaModeDamping.z * vector9;
			}
		}
		else if (m_flying && m_fAirDamping != Vector3.zero)
		{
			Vector3 vector10 = rotation * Vector3.right;
			Vector3 vector11 = rotation * Vector3.up;
			Vector3 vector12 = rotation * Vector3.forward;
			float num14 = Vector3.Dot(mLinearVelocity, vector10);
			float num15 = Vector3.Dot(mLinearVelocity, vector11);
			float num16 = Vector3.Dot(mLinearVelocity, vector12);
			zero2 -= num14 * m_fAirDamping.x * vector10;
			zero2 -= num15 * m_fAirDamping.y * vector11;
			zero2 -= num16 * m_fAirDamping.z * vector12;
		}
		if (!m_flying)
		{
			zero2 += m_fGroundAdditionnalGravity * Vector3.down;
		}
		else
		{
			zero2 += m_fAirAdditionnalGravity * Vector3.down;
		}
		m_vTransSum += zero2 * fixedDeltaTime * fixedDeltaTime;
		m_bSimplifiedWheels = m_pVehicle.GetControlType() != RcVehicle.ControlType.Human;
		Quaternion quaternion6 = rotation * mRotationQuaternion;
		Vector3 vector13 = quaternion6 * Vector3.up;
		if (m_bSimplifiedWheels && m_iNbWheels > 1)
		{
			int num17 = 1;
			float massOnAnchor = 1f / (float)m_iNbWheels;
			Vector3 zero3 = Vector3.zero;
			for (int j = 0; j < m_iNbWheels; j++)
			{
				zero3 += m_pWheels[j].VOffset;
			}
			zero3 *= 1f / (float)m_iNbWheels;
			Vector3 anchorPoint = position + m_vTransSum + quaternion6 * zero3;
			m_pWheels[num17].updateAnchor(anchorPoint, vector13, massOnAnchor);
			m_pWheels[num17].Manage(fixedDeltaTime);
			if (m_pWheels[num17].BOnGround)
			{
				Vector3 lhs4 = quaternion6 * Vector3.forward;
				Vector3 normal = m_pWheels[num17].OGroundCharac.normal;
				Vector3 rhs5 = Vector3.Cross(lhs4, normal);
				rhs5.Normalize();
				lhs4 = Vector3.Cross(normal, rhs5);
				quaternion6 = Quaternion.LookRotation(lhs4, normal);
				vector13 = quaternion6 * Vector3.up;
			}
			for (int k = 0; k < m_iNbWheels; k++)
			{
				if (k != num17)
				{
					m_pWheels[k].AdaptWithOffset(m_pWheels[num17], quaternion6 * (m_pWheels[k].VOffset - zero3), fixedDeltaTime);
				}
			}
			m_pWheels[num17].AdaptWithOffset(m_pWheels[num17], quaternion6 * (m_pWheels[num17].VOffset - zero3), fixedDeltaTime);
			for (int l = 0; l < m_iNbWheels; l++)
			{
				if (m_pWheels[l].BSteeringOn)
				{
					m_pWheels[l].FSteeringAngle = m_pVehicle.GetSteeringAngle();
				}
				m_pWheels[l].UpdateMotion(m_fWVelocity, mLinearVelocity);
			}
		}
		else
		{
			float massOnAnchor2 = 1f / (float)m_iNbWheels;
			for (int m = 0; m < m_iNbWheels; m++)
			{
				Vector3 anchorPoint2 = position + m_vTransSum + quaternion6 * m_pWheels[m].VOffset;
				m_pWheels[m].updateAnchor(anchorPoint2, vector13, massOnAnchor2);
				m_pWheels[m].Manage(fixedDeltaTime);
				if (m_pWheels[m].BSteeringOn)
				{
					m_pWheels[m].FSteeringAngle = m_pVehicle.GetSteeringAngle();
				}
				m_pWheels[m].UpdateMotion(m_fWVelocity, mLinearVelocity);
			}
		}
		UpdateWheelGroundState();
		if (!m_flying)
		{
			Vector3 zero4 = Vector3.zero;
			Vector3 zero5 = Vector3.zero;
			Vector3 zero6 = Vector3.zero;
			Vector3 zero7 = Vector3.zero;
			Vector3 zero8 = Vector3.zero;
			int num18 = 0;
			int num19 = 0;
			int num20 = 0;
			int num21 = 0;
			for (int n = 0; n < m_iNbWheels; n++)
			{
				if (m_pWheels[n].EAxle == RcPhysicWheel.WheelAxle.Front)
				{
					num18++;
					zero5 += m_pWheels[n].VRestContactPoint - vector13 * m_pWheels[n].VOffset.y;
				}
				else if (m_pWheels[n].EAxle == RcPhysicWheel.WheelAxle.Rear)
				{
					num19++;
					zero6 += m_pWheels[n].VRestContactPoint - vector13 * m_pWheels[n].VOffset.y;
				}
				if (m_pWheels[n].ESide == RcPhysicWheel.WheelSide.Left)
				{
					num21++;
					zero7 += m_pWheels[n].VRestContactPoint - vector13 * m_pWheels[n].VOffset.y;
				}
				else if (m_pWheels[n].ESide == RcPhysicWheel.WheelSide.Right)
				{
					num20++;
					zero8 += m_pWheels[n].VRestContactPoint - vector13 * m_pWheels[n].VOffset.y;
				}
			}
			if (num18 > 0)
			{
				zero5 /= (float)num18;
			}
			if (num19 > 0)
			{
				zero6 /= (float)num19;
			}
			if (num21 > 0)
			{
				zero7 /= (float)num21;
			}
			if (num20 > 0)
			{
				zero8 /= (float)num20;
			}
			Vector3 vector14 = zero5 - zero6;
			vector14.Normalize();
			Vector3 lhs5 = zero7 - zero8;
			lhs5.Normalize();
			Vector3 vector15 = Vector3.Cross(lhs5, vector14);
			vector15.Normalize();
			Quaternion quaternion7 = Quaternion.LookRotation(vector14, vector15);
			SetOrientation(quaternion7);
			float num22 = 0f;
			for (int num23 = 0; num23 < m_iNbWheels; num23++)
			{
				zero4 += m_pWheels[num23].VRestContactPoint - quaternion6 * m_pWheels[num23].VOffset;
				num22 += 1f;
			}
			zero4 /= num22;
			mRotationQuaternion = Quaternion.Inverse(quaternion6) * quaternion7;
			Vector3 lhs6 = zero4 - position - m_vTransSum;
			float num24 = Vector3.Dot(lhs6, vector15);
			if (!m_bInertiaMode || m_landing || num24 > 0f)
			{
				m_vCombineTrickMv = vector15 * num24;
				m_vTransSum += m_vCombineTrickMv;
			}
			else
			{
				m_vCombineTrickMv = Vector3.zero;
			}
		}
		else
		{
			m_vCombineTrickMv = Vector3.zero;
		}
		collData.Reset();
		Vector3 colNrm = Vector3.zero;
		float bounce = 0f;
		eMoveCheckResult eMoveCheckResult = SolveKinematic(ref position, m_vTransSum, 0, ref colNrm, ref bounce);
		m_pVehicleRigidBodyTransform.position = position;
		if (eMoveCheckResult == eMoveCheckResult.R_BOUNCE_COLL)
		{
			colNrm -= Vector3.Dot(colNrm, Vector3.up) * Vector3.up;
			Vector3 lhs7 = Vector3.Cross(mLinearVelocity, colNrm);
			Vector3 vector16 = Vector3.Cross(lhs7, colNrm);
			vector16.Normalize();
			float num25 = Vector3.Dot(mLinearVelocity, vector16);
			Vector3 impulse = -mLinearVelocity + 2f * vector16 * Vector3.Dot(vector16, mLinearVelocity) * ((!(num25 > 0f)) ? 1f : (-1f));
			impulse.Normalize();
			float num26 = bounce * mLinearVelocity.magnitude;
			impulse *= num26;
			impulse += Vector3.up * 2f;
			impulse.Normalize();
			impulse *= num26;
			SwitchToInertiaMode(0.3f, impulse, true, false);
			bounced = true;
		}
		if (eMoveCheckResult != 0)
		{
			m_vCollisionMove = position - mPreviousPosition - m_vTransSum;
		}
		if (eMoveCheckResult != 0)
		{
			Vector3 vector17 = m_pVehicleRigidBodyTransform.rotation * Vector3.forward;
			float f = Vector3.Dot(mLinearVelocity, vector17);
			RetroApplyWheelSpeed();
			if (Mathf.Abs(f) > 0.01f)
			{
				Vector3 vector18 = vector17;
				vector18.y = 0f;
				vector18.Normalize();
				Vector3 vector19 = mLinearVelocity;
				vector19.y = 0f;
				vector19.Normalize();
				if (vector19 != vector18 && vector19 != -vector18)
				{
					Vector3 axis2 = Vector3.Cross(vector18, vector19);
					float v = Vector3.Dot(vector18, vector19);
					RcUtils.COMPUTE_INERTIA(ref mCollisionDotProdInertia, v, m_fCollisionReorientationInertia, fixedDeltaTime);
					float num27 = Mathf.Acos(mCollisionDotProdInertia);
					float num28 = m_fWallReorientateSpeed * fixedDeltaTime;
					if (num27 > num28)
					{
						num27 = num28;
					}
					else if (num27 < 0f - num28)
					{
						num27 = 0f - num28;
					}
					Quaternion rotation2 = Quaternion.AngleAxis(num27 * 180f / (float)Math.PI, axis2);
					Rotate(rotation2);
				}
			}
			FireOnCollision(collData);
		}
		else
		{
			mCollisionDotProdInertia = 1f;
		}
		mLinearVelocity = (position - mPreviousPosition) / fixedDeltaTime;
		if (m_landing)
		{
			float num29 = Vector3.Dot(mLinearVelocity, m_fGroundNormal);
			if (num29 > 0f)
			{
				mLinearVelocity -= num29 * m_fGroundNormal;
			}
		}
	}

	protected void UpdateWheelGroundState()
	{
		int num = 0;
		m_prevFlying = m_flying;
		m_iGroundSurface = 0;
		m_fAvgFriction = 0f;
		m_fGroundNormal = Vector3.zero;
		for (int i = 0; i < m_iNbWheels; i++)
		{
			if (m_pWheels[i].BOnGround)
			{
				int surface = m_pWheels[i].OGroundCharac.surface;
				if (((1 << surface) & m_DeathMask.value) != 0)
				{
					DieMotherFucker(surface);
				}
				if (surface > m_iGroundSurface)
				{
					m_iGroundSurface = surface;
				}
				m_fGroundNormal += m_pWheels[i].OGroundCharac.normal;
				num++;
			}
			m_fAvgFriction += m_pWheels[i].FForwardGrip;
		}
		m_fAvgFriction /= m_iNbWheels;
		if (num > 0)
		{
			m_fGroundNormal.Normalize();
			m_flying = false;
		}
		else
		{
			m_fGroundNormal = Vector3.up;
			m_flying = true;
		}
		if (!m_flying)
		{
			if (m_prevFlying)
			{
				m_landing = true;
			}
			else if (num == m_iNbWheels && !m_bInertiaMode)
			{
				m_landing = false;
			}
		}
		else
		{
			m_landing = false;
		}
	}

	protected void SetOrientation(Quaternion newOrientation)
	{
		Quaternion rotation = Quaternion.Inverse(m_pVehicleRigidBodyTransform.rotation) * newOrientation;
		Rotate(rotation);
	}

	protected Quaternion GetOrientation()
	{
		return m_pVehicleRigidBodyTransform.rotation;
	}

	protected void Rotate(Quaternion rotation)
	{
		Rotate(rotation, false);
	}

	protected void Rotate(Quaternion rotation, bool byPass)
	{
		Quaternion rotation2 = m_pVehicleRigidBodyTransform.rotation;
		Quaternion quaternion = rotation2 * rotation;
		Vector3 fromDirection = quaternion * Vector3.up;
		Vector3 vector = quaternion * Vector3.forward;
		Vector3 rhs = quaternion * Vector3.left;
		float angle;
		Vector3 axis;
		Quaternion.FromToRotation(fromDirection, m_fGroundNormal).ToAngleAxis(out angle, out axis);
		float num = Vector3.Dot(axis, rhs) * angle;
		if (Mathf.Abs(num) > m_fMaxIncline && !byPass)
		{
			Quaternion quaternion2 = ((!(num > 0f)) ? Quaternion.AngleAxis(0f - num - m_fMaxIncline, -Vector3.left) : Quaternion.AngleAxis(num - m_fMaxIncline, Vector3.left));
			quaternion *= quaternion2;
		}
		fromDirection = quaternion * Vector3.up;
		vector = quaternion * Vector3.forward;
		rhs = quaternion * Vector3.left;
		Quaternion.FromToRotation(fromDirection, m_fGroundNormal).ToAngleAxis(out angle, out axis);
		float num2 = Vector3.Dot(axis, vector) * angle;
		if (Mathf.Abs(num2) > m_fMaxIncline && !byPass)
		{
			Quaternion quaternion3 = ((!(num2 > 0f)) ? Quaternion.AngleAxis(0f - num2 - m_fMaxIncline, -Vector3.forward) : Quaternion.AngleAxis(num2 - m_fMaxIncline, Vector3.forward));
			quaternion *= quaternion3;
		}
		if (rotation2 != quaternion)
		{
			m_pVehicleRigidBodyTransform.rotation = quaternion;
		}
	}

	protected void RetroApplyWheelSpeed()
	{
		if (!m_pVehicle.IsLocked())
		{
			Quaternion rotation = m_pVehicleRigidBodyTransform.rotation;
			Vector3 rhs = rotation * Vector3.forward;
			float num = Vector3.Dot(mLinearVelocity, rhs);
			if (Mathf.Abs(num) > 5f)
			{
				RcUtils.COMPUTE_INERTIA(ref m_fWVelocity, num, 0.999f, Time.deltaTime * 1000f);
			}
			else if (m_pVehicle.GetMoveFactor() > 0f)
			{
				m_fWVelocity = 5f;
			}
			else if (m_pVehicle.GetMoveFactor() < 0f)
			{
				m_fWVelocity = -5f;
			}
			else
			{
				m_fWVelocity = 0f;
			}
			m_pVehicle.SetWheelSpeedMS(m_fWVelocity / (1f - m_pVehicle.GetHandicap()));
		}
	}

	protected eMoveCheckResult SolveKinematic(ref Vector3 position, Vector3 displ, int recursiveCnt, ref Vector3 colNrm, ref float bounce)
	{
		float minDst = displ.magnitude;
		Vector3 remainingMove = Vector3.zero;
		eMoveCheckResult eMoveCheckResult = MvTest(position, ref minDst, ref remainingMove, sphere.radius, displ, ref colNrm, ref bounce);
		if (minDst > 0f)
		{
			Vector3 vector = displ;
			vector.Normalize();
			position += minDst * vector;
		}
		if (remainingMove != Vector3.zero && recursiveCnt < 4)
		{
			eMoveCheckResult eMoveCheckResult2 = SolveKinematic(ref position, remainingMove, recursiveCnt + 1, ref colNrm, ref bounce);
			if (eMoveCheckResult2 > eMoveCheckResult)
			{
				eMoveCheckResult = eMoveCheckResult2;
			}
		}
		return eMoveCheckResult;
	}

	protected eMoveCheckResult MvTest(Vector3 position, ref float minDst, ref Vector3 remainingMove, float radius, Vector3 translation, ref Vector3 colNrm, ref float bounce)
	{
		Vector3 direction = translation;
		direction.Normalize();
		float num = minDst;
		Ray ray = default(Ray);
		ray.direction = direction;
		ray.origin = position + sphere.center;
		float magnitude = translation.magnitude;
		int num2 = m_SphereCollidesWith.value;
		if (!m_bTouchedDeath)
		{
			num2 |= m_DeathMask.value;
		}
		RaycastHit hitInfo;
		bool flag = Physics.SphereCast(ray, radius, out hitInfo, magnitude + m_fSkinWidth + 1f, num2);
		eMoveCheckResult eMoveCheckResult = eMoveCheckResult.R_NO_COLL;
		bool flag2 = false;
		int layer = 0;
		if (flag)
		{
			Vector3 normal = hitInfo.normal;
			Vector3 vector = hitInfo.point + hitInfo.normal * radius;
			float magnitude2 = (vector - ray.origin).magnitude;
			if (magnitude2 < 0f)
			{
				Debug.DrawLine(hitInfo.point, hitInfo.point + hitInfo.normal, Color.green, Time.deltaTime, false);
			}
			if (magnitude2 - m_fSkinWidth < minDst)
			{
				float num3 = 0.1f;
				eMoveCheckResult = eMoveCheckResult.R_SLIDE_COLL;
				int layer2 = hitInfo.collider.gameObject.layer;
				flag2 = ((1 << layer2) & m_DeathMask.value) != 0;
				if (flag2)
				{
					layer = layer2;
				}
				minDst = magnitude2 - m_fSkinWidth;
				if (minDst < 0f)
				{
					minDst = 0f;
				}
				if (((1 << layer2) & m_SphereCollidesWith.value) == 0)
				{
					eMoveCheckResult = eMoveCheckResult.R_NO_COLL;
					remainingMove = translation;
				}
				else
				{
					if (hitInfo.collider != null && hitInfo.collider.gameObject != null)
					{
						float num4 = Mathf.Abs(Vector3.Dot(mLinearVelocity, hitInfo.normal));
						foreach (SurfaceResponse vSurfaceResponse in m_vSurfaceResponses)
						{
							int layer3 = hitInfo.collider.gameObject.layer;
							if (((1 << layer3) & vSurfaceResponse.layers.value) != 0 && num4 * 3.6f > vSurfaceResponse.bounceMinSpeedKph && bounce < vSurfaceResponse.bounce)
							{
								num3 = vSurfaceResponse.bounce;
								eMoveCheckResult = eMoveCheckResult.R_BOUNCE_COLL;
								if (colNrm == Vector3.zero)
								{
									colNrm = hitInfo.normal;
									bounce = num3;
								}
							}
						}
					}
					Vector3 vector2 = ray.origin + translation - vector;
					remainingMove = vector2 - (1f + num3) * Vector3.Dot(vector2, normal) * normal;
					if (collData.position == Vector3.zero)
					{
						collData.normal = hitInfo.normal;
						collData.solid = m_pVehicleRigidBody;
						collData.other = hitInfo.rigidbody;
						collData.surface = layer2;
						collData.position = hitInfo.point;
						collData.depth = RcUtils.FastSqrtApprox(vector2.sqrMagnitude);
					}
				}
			}
		}
		if (flag2)
		{
			DieMotherFucker(layer);
		}
		if (eMoveCheckResult == eMoveCheckResult.R_NO_COLL)
		{
			minDst = num;
			remainingMove = Vector3.zero;
		}
		return eMoveCheckResult;
	}

	public void DieMotherFucker(int layer)
	{
		if (m_bTouchedDeath)
		{
			return;
		}
		m_bTouchedDeath = true;
		foreach (SurfaceResponse vSurfaceResponse in m_vSurfaceResponses)
		{
			if ((vSurfaceResponse.layers.value & (1 << layer)) != 0)
			{
				m_fDeathDelay = vSurfaceResponse.deathDelay;
				break;
			}
		}
	}

	public override float GetDriftRatio()
	{
		return 0.5f * Mathf.Abs(m_pVehicle.GetArcadeDriftFactor());
	}

	public override Vector3 GetAngularVelocity()
	{
		return mAngularVelocity;
	}

	public override void SetLinearVelocity(Vector3 velocity)
	{
		mLinearVelocity = velocity;
	}

	public override void SetAngularVelocity(Vector3 velocity)
	{
		mAngularVelocity = velocity;
	}
}

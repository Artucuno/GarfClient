using UnityEngine;

public class RcKinematicWheel : RcPhysicWheel
{
	private const float m_fLandAnchor = 3f;

	private const float m_fLandAnchorSpeed = 10f;

	public bool m_bSphereCast;

	public float m_fNaturalSlope = 0.05f;

	private Vector3 mAxis;

	private float m_fAnchorSpeed;

	private Vector3 m_vAnchorPoint;

	private float m_massOnAnchor;

	private float m_fOldStretch;

	private float m_fStretchSpeed;

	private float m_fStretchTolerance;

	private LayerMask m_WheelsCollideWith;

	private LayerMask m_WheelsGripWith;

	private LayerMask m_WheelsDeathMask;

	private Ray m_pRay;

	private float m_fLastHeight;

	private bool m_bLastHeightValid;

	public float AnchorSpeed
	{
		get
		{
			return m_fAnchorSpeed;
		}
	}

	public RcKinematicWheel()
	{
		m_massOnAnchor = 1f;
		m_fStretchSpeed = 0f;
		Reset();
	}

	public void updateAnchor(Vector3 _anchorPoint, Vector3 _axis, float _massOnAnchor)
	{
		Vector3 lhs = _anchorPoint - m_vAnchorPoint;
		lhs -= Vector3.Dot(lhs, _axis) * _axis;
		m_fStretchTolerance = m_fNaturalSlope * lhs.magnitude;
		m_vAnchorPoint = _anchorPoint;
		mAxis = _axis;
		m_massOnAnchor = _massOnAnchor;
	}

	public void SetMasks(LayerMask collideWith, LayerMask gripWith, LayerMask deathMask)
	{
		m_WheelsCollideWith = collideWith;
		m_WheelsGripWith = gripWith;
		m_WheelsDeathMask = deathMask;
	}

	public override void Start()
	{
		base.Start();
		m_pRay = default(Ray);
	}

	public void Reset()
	{
		mAxis = Vector3.up;
		m_vRestContactPoint = Vector3.zero;
		m_vAnchorPoint = Vector3.zero;
		m_fAnchorSpeed = 0f;
		m_fOldStretch = 0f;
		m_fStretch = 0f;
		m_bOnGround = false;
		m_oGroundCharac.normal = Vector3.up;
		m_oGroundCharac.surface = 0;
	}

	private bool TestCollision(Vector3 src, float maxStep)
	{
		int layerMask = m_WheelsCollideWith.value | m_WheelsDeathMask.value;
		src += (m_fMaxCompression + maxStep) * mAxis;
		m_pRay.origin = src;
		m_pRay.direction = -mAxis;
		bool flag = false;
		RaycastHit hitInfo;
		flag = ((!m_bSphereCast) ? Physics.Raycast(m_pRay, out hitInfo, maxStep + m_fMaxCompression + m_fRadius, layerMask) : Physics.SphereCast(m_pRay, m_fRadius, out hitInfo, maxStep + m_fMaxCompression + m_fRadius, layerMask));
		if (Mathf.Abs(Vector3.Dot(hitInfo.normal, mAxis)) < 0.5f)
		{
			flag = false;
		}
		m_oGroundCharac.point = hitInfo.point;
		m_oGroundCharac.normal = hitInfo.normal;
		m_oGroundCharac.surface = 0;
		if (hitInfo.collider != null && hitInfo.collider.gameObject != null)
		{
			m_oGroundCharac.surface = hitInfo.collider.gameObject.layer;
		}
		return flag;
	}

	public void Manage(float _deltaTime)
	{
		bool hit = TestCollision(m_vAnchorPoint, _deltaTime * 60f);
		if (m_oGroundCharac.surface != 0)
		{
			if (((1 << m_oGroundCharac.surface) & m_WheelsCollideWith.value) == 0)
			{
				hit = false;
			}
			if (((1 << m_oGroundCharac.surface) & m_WheelsGripWith.value) != 0)
			{
				m_fForwardGrip = 1f;
				m_fSideGrip = 1f;
			}
			else
			{
				m_fForwardGrip = 0f;
				m_fSideGrip = 0f;
			}
		}
		float num = 0f;
		if (m_bOnGround)
		{
			float num2 = Vector3.Dot(Physics.gravity, mAxis);
			num2 += (m_fDamping * m_fStretchSpeed + m_fSpring * m_fStretch) / m_massOnAnchor;
			num = num2 * _deltaTime;
		}
		else if (m_fAnchorSpeed > -3f)
		{
			num = -10f * _deltaTime;
		}
		m_fAnchorSpeed = Mathf.Clamp(m_fAnchorSpeed + num, -3f, 3f);
		m_vRestContactPoint = m_vAnchorPoint + mAxis * m_fAnchorSpeed * _deltaTime;
		m_fOldStretch = m_fStretch;
		UpdateStretch(hit, _deltaTime);
	}

	public void AdaptWithOffset(RcKinematicWheel wheel, Vector3 offset, float _deltaTime)
	{
		m_vAnchorPoint = wheel.m_vAnchorPoint + offset;
		m_vRestContactPoint = wheel.m_vRestContactPoint + offset;
		m_oGroundCharac = wheel.m_oGroundCharac;
		m_oGroundCharac.point += offset;
		m_fAnchorSpeed = wheel.m_fAnchorSpeed;
		m_bOnGround = wheel.m_bOnGround;
		mAxis = wheel.mAxis;
		m_massOnAnchor = wheel.m_massOnAnchor;
		m_fForwardGrip = wheel.m_fForwardGrip;
		m_fSideGrip = wheel.m_fSideGrip;
		if (wheel != this)
		{
			m_fOldStretch = m_fStretch;
		}
		UpdateStretch(wheel.m_bOnGround, _deltaTime);
	}

	protected void UpdateStretch(bool _hit, float _deltaTime)
	{
		float num = m_fRadius - Vector3.Dot(m_vRestContactPoint - m_oGroundCharac.point, mAxis);
		if (_hit && num >= 0f)
		{
			float num2 = num - m_fStretch;
			if (num2 > 0f)
			{
				if (num2 > m_fStretchTolerance)
				{
					num2 = m_fStretchTolerance;
				}
				m_vRestContactPoint += mAxis * num2;
				num -= num2;
			}
			if (num > m_fMaxCompression)
			{
				m_vRestContactPoint = m_oGroundCharac.point + (m_fRadius - m_fMaxCompression) * mAxis;
				num = m_fMaxCompression;
			}
			m_fStretch = num;
			m_bOnGround = true;
		}
		else
		{
			m_fStretch = 0f;
			m_bOnGround = false;
		}
		m_fStretchSpeed = Mathf.Clamp((m_fStretch - m_fOldStretch) / _deltaTime, -10f, 10f);
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class KartFxMgr : MonoBehaviour, RcCollisionListener
{
	[Serializable]
	public class SurfaceFx
	{
		public LayerMask SurfaceLayer;

		public GameObject AssociateFx;

		public SurfaceFx()
		{
			AssociateFx = null;
		}
	}

	[Serializable]
	public class KartFx
	{
		public GameObject AssociateFx;

		public bool FollowKart;

		private Transform m_pTransform;

		private GameObject m_pObject;

		private ParticleSystem m_pParticles;

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

		public GameObject Object
		{
			get
			{
				return m_pObject;
			}
			set
			{
				m_pObject = value;
				Transform = m_pObject.transform;
				m_pParticles = m_pObject.particleSystem;
			}
		}

		public ParticleSystem ParticleSystem
		{
			get
			{
				return m_pParticles;
			}
		}

		public KartFx()
		{
			AssociateFx = null;
			FollowKart = true;
			m_pTransform = null;
			m_pObject = null;
			m_pParticles = null;
		}
	}

	private Kart m_pKart;

	public float SpeedToProduceSurfaceFx = 7f;

	public List<SurfaceFx> SurfacesFx;

	private List<GameObject> SurfaceFxGameObject = new List<GameObject>();

	[SerializeField]
	[HideInInspector]
	public KartFx[] KartFxs = new KartFx[Enum.GetValues(typeof(eKartFx)).Length];

	public LayerMask WaterLayer;

	private Color m_pDriftBaseColor;

	private Color m_pDriftColor;

	private bool m_bDriftColor;

	public KartFxMgr()
	{
		m_pKart = null;
		SurfacesFx = new List<SurfaceFx>();
	}

	private void OnDestroy()
	{
		Stop();
		if (KartFxs == null)
		{
			return;
		}
		for (int i = 0; i < KartFxs.Length; i++)
		{
			if (KartFxs[i] != null && !KartFxs[i].FollowKart)
			{
				UnityEngine.Object.Destroy(KartFxs[i].Object);
			}
		}
	}

	private void Start()
	{
		m_pKart = base.transform.parent.FindChild("Tunning").GetComponent<Kart>();
		if ((bool)m_pKart)
		{
			m_pKart.AddCollisionListener(this);
		}
		m_bDriftColor = false;
	}

	public void Stop()
	{
		if ((bool)m_pKart)
		{
			m_pKart.RemoveCollisionListener(this);
			m_pKart = null;
		}
	}

	public void InstantiateAndAttach(Transform _Parent)
	{
		for (int i = 0; i < SurfacesFx.Count; i++)
		{
			if (SurfacesFx[i].AssociateFx != null)
			{
				SurfaceFxGameObject.Add((GameObject)UnityEngine.Object.Instantiate(SurfacesFx[i].AssociateFx));
				SurfaceFxGameObject[i].transform.position = _Parent.position;
				SurfaceFxGameObject[i].transform.parent = _Parent;
			}
		}
		for (int j = 0; j < KartFxs.Length; j++)
		{
			if (KartFxs[j].AssociateFx != null)
			{
				KartFxs[j].Object = (GameObject)UnityEngine.Object.Instantiate(KartFxs[j].AssociateFx);
				if (KartFxs[j].FollowKart)
				{
					KartFxs[j].Transform.parent = _Parent;
				}
				if (j == 3)
				{
					m_pDriftBaseColor = KartFxs[j].ParticleSystem.startColor;
				}
			}
		}
	}

	private void Update()
	{
		if (m_pKart == null)
		{
			return;
		}
		float wheelSpeed = m_pKart.GetVehiclePhysic().GetWheelSpeed();
		bool flag = wheelSpeed >= SpeedToProduceSurfaceFx || wheelSpeed <= 0f - SpeedToProduceSurfaceFx;
		for (int i = 0; i < SurfacesFx.Count; i++)
		{
			if (!flag)
			{
				if (SurfaceFxGameObject[i].particleSystem.isPlaying)
				{
					SurfaceFxGameObject[i].particleSystem.Stop();
				}
				continue;
			}
			bool flag2 = false;
			for (int j = 0; j < m_pKart.GetVehiclePhysic().GetNbWheels(); j++)
			{
				int surface = m_pKart.GetGroundCharac(j).surface;
				if (m_pKart.IsOnGround(j) && ((1 << surface) & SurfacesFx[i].SurfaceLayer.value) != 0)
				{
					flag2 = true;
					break;
				}
			}
			if (flag2 && !SurfaceFxGameObject[i].particleSystem.isPlaying)
			{
				SurfaceFxGameObject[i].particleSystem.startSpeed = Mathf.Abs(wheelSpeed);
				SurfaceFxGameObject[i].particleSystem.Play();
				StopAllOtherSurfaceFx(SurfaceFxGameObject[i]);
				break;
			}
			if (flag2 && SurfaceFxGameObject[i].particleSystem.isPlaying)
			{
				SurfaceFxGameObject[i].particleSystem.startSpeed = Mathf.Abs(wheelSpeed);
			}
			else if (!flag2 && SurfaceFxGameObject[i].particleSystem.isPlaying)
			{
				SurfaceFxGameObject[i].particleSystem.Stop();
			}
		}
		for (int k = 0; k < m_pKart.GetVehiclePhysic().GetNbWheels(); k++)
		{
			int surface2 = m_pKart.GetGroundCharac(k).surface;
			if (((1 << surface2) & WaterLayer.value) != 0)
			{
				PlayKartFx(eKartFx.Dive);
			}
		}
	}

	public void StopAllOtherSurfaceFx(GameObject _ExeptMe)
	{
		foreach (GameObject item in SurfaceFxGameObject)
		{
			if (item != _ExeptMe && item.particleSystem.isPlaying)
			{
				item.particleSystem.Stop();
			}
		}
	}

	public void PlayKartFx(eKartFx _Fx)
	{
		if ((KartFxs[(int)_Fx].ParticleSystem.loop && !KartFxs[(int)_Fx].ParticleSystem.isPlaying) || !KartFxs[(int)_Fx].ParticleSystem.loop)
		{
			if (!KartFxs[(int)_Fx].FollowKart)
			{
				KartFxs[(int)_Fx].Transform.position = m_pKart.Transform.position;
			}
			KartFxs[(int)_Fx].ParticleSystem.Play();
		}
	}

	public void PlayKartFx(eKartFx _Fx, Vector3 _pos)
	{
		if ((KartFxs[(int)_Fx].ParticleSystem.loop && !KartFxs[(int)_Fx].ParticleSystem.isPlaying) || !KartFxs[(int)_Fx].ParticleSystem.loop)
		{
			KartFxs[(int)_Fx].Transform.position = _pos;
			KartFxs[(int)_Fx].ParticleSystem.Play();
		}
	}

	public void StopKartFx(eKartFx _Fx)
	{
		if (KartFxs[(int)_Fx] != null && KartFxs[(int)_Fx].ParticleSystem != null && KartFxs[(int)_Fx].ParticleSystem.isPlaying)
		{
			if (!KartFxs[(int)_Fx].FollowKart)
			{
				KartFxs[(int)_Fx].Transform.position = m_pKart.Transform.position;
			}
			KartFxs[(int)_Fx].ParticleSystem.Stop();
			KartFxs[(int)_Fx].ParticleSystem.Clear();
		}
	}

	public void StopDriftFx()
	{
		StopKartFx(eKartFx.DriftLeft);
		StopKartFx(eKartFx.DriftLeft2);
		StopKartFx(eKartFx.DriftRight);
		StopKartFx(eKartFx.DriftRight2);
		m_bDriftColor = false;
	}

	public void OnCollision(CollisionData collisionInfo)
	{
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		if ((bool)m_pKart)
		{
			vector = m_pKart.GetVehiclePhysic().GetLinearVelocity();
		}
		if (collisionInfo.other != null)
		{
			vector2 = collisionInfo.other.velocity;
		}
		float f = Vector3.Dot(vector - vector2, collisionInfo.normal);
		if (Mathf.Abs(f) > 2f && !m_pKart.IsLocked() && m_pKart.GetWheelSpeedMS() > 13.9f)
		{
			PlayKartFx(eKartFx.Impact, collisionInfo.position);
		}
	}

	public void BoostDrift(float _BoostLevel, float _Threshold)
	{
		if (_BoostLevel == 0f)
		{
			KartFxs[3].ParticleSystem.startColor = m_pDriftBaseColor;
			KartFxs[4].ParticleSystem.startColor = m_pDriftBaseColor;
		}
		else if (_BoostLevel >= _Threshold)
		{
			Color color = new Color((_BoostLevel - _Threshold) / (100f - _Threshold), 0.5f, 0.4f);
			KartFxs[3].ParticleSystem.startColor = color;
			KartFxs[4].ParticleSystem.startColor = color;
			m_pDriftColor = color;
			m_bDriftColor = true;
		}
	}

	public void Boost()
	{
		Color startColor = (m_bDriftColor ? m_pDriftColor : m_pDriftBaseColor);
		KartFxs[0].ParticleSystem.startColor = startColor;
		KartFxs[8].ParticleSystem.startColor = startColor;
		KartFxs[0].ParticleSystem.Play();
		KartFxs[8].ParticleSystem.Play();
		m_bDriftColor = false;
	}
}

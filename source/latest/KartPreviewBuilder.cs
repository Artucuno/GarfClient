using System.Collections.Generic;
using UnityEngine;

public class KartPreviewBuilder : MonoBehaviour
{
	private GameObject m_oKartPreview;

	private GameObject m_oCharacterPreview;

	private GameObject m_oHatPreview;

	private GameObject m_oKartCustomPreview;

	private ECharacter m_eCharacter;

	private ECharacter m_eKart;

	private string m_sCustomName;

	private string m_sHatName;

	public float m_fSpeedRotation;

	public float m_fRotationInertia = 0.3f;

	private float m_fSpeed;

	private bool m_bDrag;

	private Animator m_pCharacterAnimator;

	private int m_iSuccesAnim;

	[HideInInspector]
	public List<AudioSource> SelectionVoices = new List<AudioSource>();

	public void Init()
	{
		DestroyKartPreview();
		m_eKart = ECharacter.NONE;
		m_sCustomName = string.Empty;
		m_sHatName = string.Empty;
		m_eKart = ECharacter.NONE;
		m_iSuccesAnim = Animator.StringToHash("End_Of_Race.Victory");
	}

	public void DestroyKartPreview()
	{
		if ((bool)m_oKartPreview)
		{
			Object.Destroy(m_oKartPreview);
			m_oKartPreview = null;
		}
		if ((bool)m_oKartCustomPreview)
		{
			Object.Destroy(m_oKartCustomPreview);
			m_oKartCustomPreview = null;
		}
		if ((bool)m_oCharacterPreview)
		{
			Object.Destroy(m_oCharacterPreview);
			m_oCharacterPreview = null;
		}
		if ((bool)m_oHatPreview)
		{
			Object.Destroy(m_oHatPreview);
			m_oHatPreview = null;
		}
		Resources.UnloadUnusedAssets();
	}

	public void Build(ECharacter eCharacter, ECharacter eKart, string sCustomName, string sHatName)
	{
		PlayerConfig playerConfig = Singleton<GameConfigurator>.Instance.PlayerConfig;
		if ((bool)m_oKartPreview && m_eKart != eKart)
		{
			Object.Destroy(m_oKartPreview);
			m_oKartPreview = null;
		}
		if (!m_oKartPreview)
		{
			m_oKartPreview = (GameObject)Object.Instantiate(Resources.Load("Kart/" + playerConfig.KartPrefab[(int)eKart]));
			ForceLod0(m_oKartPreview);
		}
		m_eKart = eKart;
		if ((bool)m_oCharacterPreview && m_eCharacter != eCharacter)
		{
			Object.Destroy(m_oCharacterPreview);
			m_oCharacterPreview = null;
		}
		if (!m_oCharacterPreview)
		{
			m_oCharacterPreview = (GameObject)Object.Instantiate(Resources.Load("Character/" + playerConfig.CharacterPrefab[(int)eCharacter]));
			ForceLod0(m_oCharacterPreview);
			if ((bool)SelectionVoices[(int)eCharacter])
			{
				SelectionVoices[(int)eCharacter].Play();
			}
			if ((bool)m_oCharacterPreview)
			{
				m_pCharacterAnimator = m_oCharacterPreview.GetComponent<Animator>();
				if ((bool)m_pCharacterAnimator)
				{
					m_pCharacterAnimator.SetBool("Victory", true);
				}
			}
		}
		m_eCharacter = eCharacter;
		if ((bool)m_oKartCustomPreview && m_sCustomName != sCustomName)
		{
			Object.Destroy(m_oKartCustomPreview);
			m_oKartCustomPreview = null;
		}
		if (!m_oKartCustomPreview)
		{
			m_oKartCustomPreview = (GameObject)Object.Instantiate(Resources.Load("Kart/" + sCustomName));
			ForceLod0(m_oKartCustomPreview);
		}
		m_sCustomName = sCustomName;
		if ((bool)m_oHatPreview && m_sHatName != sHatName)
		{
			Object.Destroy(m_oHatPreview);
			m_oHatPreview = null;
		}
		if (!m_oHatPreview)
		{
			m_oHatPreview = (GameObject)Object.Instantiate(Resources.Load("Hat/" + sHatName));
			ForceLod0(m_oHatPreview);
		}
		Resources.UnloadUnusedAssets();
		m_sHatName = sHatName;
		m_oHatPreview.transform.parent = null;
		m_oCharacterPreview.transform.parent = null;
		m_oKartPreview.transform.parent = null;
		m_oKartCustomPreview.transform.parent = null;
		m_oKartPreview.transform.localPosition = Vector3.zero;
		m_oKartPreview.transform.localRotation = Quaternion.identity;
		m_oCharacterPreview.transform.localPosition = Vector3.zero;
		m_oCharacterPreview.transform.localRotation = Quaternion.identity;
		m_oHatPreview.transform.localPosition = Vector3.zero;
		m_oHatPreview.transform.localRotation = Quaternion.identity;
		m_oHatPreview.transform.localScale = Vector3.one;
		m_oKartCustomPreview.transform.localPosition = Vector3.zero;
		m_oKartCustomPreview.transform.localRotation = Quaternion.identity;
		Transform child = m_oCharacterPreview.transform.GetChild(0);
		m_oHatPreview.transform.parent = child.transform.Find("b_cat Attachment/b_cat Root/b_cat Pelvis/b_cat Spine/b_cat Neck/b_cat Head");
		if (m_oHatPreview.transform.parent == null)
		{
			m_oHatPreview.transform.parent = m_oCharacterPreview.transform.Find("b_cat Attachment/b_cat Root/b_cat Pelvis/b_cat Spine/b_cat Neck/b_cat Head");
		}
		m_oHatPreview.transform.localPosition = Vector3.zero;
		BonusCustom component = m_oHatPreview.GetComponent<BonusCustom>();
		if ((bool)component)
		{
			Template template = component.GetTemplate(eCharacter);
			if (template != null)
			{
				m_oHatPreview.transform.localPosition = template.Position;
				m_oHatPreview.transform.localScale = template.Scale;
			}
			m_oHatPreview.transform.localRotation = Quaternion.Euler(270f, 90f, 0f);
		}
		m_oCharacterPreview.transform.parent = m_oKartPreview.transform.Find("Kart(Clone)/b_kart_root/b_kart_body");
		m_oKartCustomPreview.transform.parent = m_oCharacterPreview.transform.parent;
		m_oKartCustomPreview.transform.localRotation = Quaternion.Euler(90f, 270f, 0f);
		m_oKartPreview.transform.parent = base.transform;
		m_oKartPreview.transform.localPosition = Vector3.zero;
		m_oKartPreview.transform.localRotation = Quaternion.identity;
		LodRemote component2 = m_oKartPreview.GetComponent<LodRemote>();
		if ((bool)component2)
		{
			component2.m_pLodGroupReceiver[0] = m_oCharacterPreview.GetComponent<LODGroup>();
			component2.m_pLodGroupReceiver[1] = m_oHatPreview.GetComponent<LODGroup>();
			component2.m_pLodGroupReceiver[2] = m_oKartCustomPreview.GetComponent<LODGroup>();
		}
		m_fSpeed = m_fSpeedRotation;
	}

	public void Update()
	{
		if (m_bDrag)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if (deltaTime > 0f)
		{
			m_fSpeed = Tricks.ComputeInertia(m_fSpeed, m_fSpeedRotation, m_fRotationInertia, deltaTime);
			base.transform.Rotate(new Vector3(0f, m_fSpeed * deltaTime, 0f));
			if ((bool)m_pCharacterAnimator && m_pCharacterAnimator.GetCurrentAnimatorStateInfo(0).nameHash == m_iSuccesAnim)
			{
				m_pCharacterAnimator.SetBool("Victory", false);
			}
		}
	}

	private void OnDrag(Vector2 oDelta)
	{
		m_fSpeed = -0.3f * oDelta.x / Time.deltaTime;
		base.transform.Rotate(new Vector3(0f, m_fSpeed * Time.deltaTime, 0f));
	}

	private void OnPress(bool bIsPressed)
	{
		m_bDrag = bIsPressed;
	}

	private void ForceLod0(GameObject pObj)
	{
		if ((bool)pObj)
		{
			LODGroup component = pObj.GetComponent<LODGroup>();
			if ((bool)component)
			{
				component.ForceLOD(0);
			}
		}
	}
}

using UnityEngine;

public class UICopySprite : MonoBehaviour
{
	public UISprite m_pCopiedSprite;

	private GameObject m_pSpriteInstance;

	private UISprite m_pUISprite;

	private void Start()
	{
		if ((bool)m_pCopiedSprite)
		{
			m_pSpriteInstance = (GameObject)Object.Instantiate(m_pCopiedSprite.gameObject);
		}
		if ((bool)m_pSpriteInstance)
		{
			m_pSpriteInstance.transform.parent = base.transform.parent;
			m_pSpriteInstance.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, base.transform.localPosition.z - 1f);
			m_pSpriteInstance.transform.localScale = base.transform.localScale;
			m_pSpriteInstance.SetActive(false);
			m_pUISprite = m_pSpriteInstance.GetComponent<UISprite>();
		}
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
		if ((bool)m_pSpriteInstance && (bool)m_pCopiedSprite && (bool)m_pUISprite)
		{
			m_pSpriteInstance.SetActive(m_pCopiedSprite.gameObject.activeSelf);
			if (!m_pCopiedSprite.atlas.Equals(m_pUISprite.atlas))
			{
				m_pUISprite.atlas = m_pCopiedSprite.atlas;
			}
			if (!m_pCopiedSprite.spriteName.Equals(m_pUISprite.spriteName))
			{
				m_pUISprite.spriteName = m_pCopiedSprite.spriteName;
			}
		}
	}

	private void OnDestroy()
	{
		if ((bool)m_pSpriteInstance)
		{
			Object.Destroy(m_pSpriteInstance);
		}
	}

	private void OnDisable()
	{
		if ((bool)m_pSpriteInstance)
		{
			m_pSpriteInstance.SetActive(false);
		}
	}
}

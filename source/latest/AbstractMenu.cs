using UnityEngine;

public class AbstractMenu : MonoBehaviour
{
	protected MenuEntryPoint m_pMenuEntryPoint;

	public Camera m_oCamera;

	private UICamera m_oMenuCamera;

	protected LayerMask m_pLayerHud;

	public virtual void Awake()
	{
		m_pMenuEntryPoint = GameObject.Find("MenuEntryPoint").GetComponent<MenuEntryPoint>();
		m_oMenuCamera = base.transform.GetComponentInChildren<UICamera>();
		if ((bool)m_oMenuCamera)
		{
			m_pLayerHud = m_oMenuCamera.eventReceiverMask;
		}
		base.transform.gameObject.SetActive(false);
	}

	public virtual void Start()
	{
	}

	public virtual void Update()
	{
		if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
		{
			ActSwapMenu(EMenus.MENU_WELCOME);
		}
	}

	public virtual void OnEnter()
	{
		base.gameObject.SetActive(true);
		if ((bool)m_pMenuEntryPoint)
		{
			m_pMenuEntryPoint.SetCamera(m_oCamera);
		}
		PlayMusic();
	}

	public virtual void OnEnter(int iEntryPoint)
	{
		OnEnter();
	}

	public virtual void OnExit()
	{
		base.gameObject.SetActive(false);
	}

	public virtual void OnPopupShow()
	{
		if ((bool)m_oMenuCamera)
		{
			m_oMenuCamera.eventReceiverMask = 0;
		}
	}

	public virtual void OnPopupQuit()
	{
		if ((bool)m_oMenuCamera)
		{
			m_oMenuCamera.eventReceiverMask = m_pLayerHud;
		}
	}

	public GameObject FindMenu(string GameObjectName)
	{
		GameObject gameObject = GameObject.Find(GameObjectName);
		if (!gameObject)
		{
		}
		return gameObject;
	}

	public void ActSwapMenu(EMenus NextMenu)
	{
		if ((bool)m_pMenuEntryPoint)
		{
			m_pMenuEntryPoint.SetState(NextMenu);
		}
	}

	public void ActShowPopup(EPopUps Popup)
	{
		if ((bool)m_pMenuEntryPoint)
		{
			m_pMenuEntryPoint.ShowPopup(Popup, null, true);
		}
	}

	public virtual void PlayMusic()
	{
		if ((bool)m_pMenuEntryPoint)
		{
			m_pMenuEntryPoint.PlayMenuMusic();
		}
	}
}

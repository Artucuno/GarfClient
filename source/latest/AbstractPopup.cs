using UnityEngine;

public class AbstractPopup : MonoBehaviour
{
	protected MenuEntryPoint m_pMenuEntryPoint;

	public UILocalize Text;

	public virtual void Awake()
	{
		m_pMenuEntryPoint = GameObject.Find("MenuEntryPoint").GetComponent<MenuEntryPoint>();
		base.transform.gameObject.SetActive(false);
	}

	public virtual void Show(string _TextId)
	{
		if (Text != null && _TextId != null)
		{
			Text.key = _TextId;
		}
		base.gameObject.SetActive(true);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public void OnQuit()
	{
		m_pMenuEntryPoint.QuitPopup();
	}
}

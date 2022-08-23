using UnityEngine;

public class HoldMessageButton : MonoBehaviour
{
	public GameObject Target;

	private HUDControls m_pHudControls;

	public EInputAction Action;

	public UICamera CurrentCamera;

	private RaycastHit m_pRaycastHit;

	private bool _Input;

	private void Start()
	{
		m_pHudControls = Target.GetComponent<HUDControls>();
	}

	private void Update()
	{
		UICamera.current = CurrentCamera;
		Touch[] touches = Input.touches;
		foreach (Touch touch in touches)
		{
			if (CurrentCamera != null && UICamera.Raycast(touch.position, ref m_pRaycastHit) && m_pRaycastHit.collider.gameObject == base.gameObject)
			{
				_Input = true;
				m_pHudControls.OnAction(Action);
				break;
			}
		}
		if (Input.touches.Length == 0)
		{
			_Input = false;
		}
	}

	public void OnEnter()
	{
	}

	public bool HasInput()
	{
		return _Input;
	}
}

using UnityEngine;

public class SteerButton : MonoBehaviour
{
	public bool Left;

	private Rect m_pBounds;

	private void Start()
	{
		BoxCollider boxCollider = base.collider as BoxCollider;
		m_pBounds.center = boxCollider.center;
		m_pBounds.width = boxCollider.size.x;
		m_pBounds.height = boxCollider.size.y;
	}

	private void Update()
	{
		Touch[] touches = Input.touches;
		foreach (Touch touch in touches)
		{
			if (m_pBounds.Contains(touch.position))
			{
				if (Left)
				{
					Singleton<InputManager>.Instance.NullSafeAct(Singleton<InputManager>.Instance.SetAction, EAction.Steer, -1f);
				}
				else
				{
					Singleton<InputManager>.Instance.NullSafeAct(Singleton<InputManager>.Instance.SetAction, EAction.Steer, 1f);
				}
				break;
			}
		}
	}

	public void OnEnter()
	{
	}
}

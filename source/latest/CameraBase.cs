using UnityEngine;

public class CameraBase : MonoBehaviour
{
	public ECamState m_StartState;

	private ECamState m_PrevState;

	private Transform m_pTransform;

	private ECamState m_CurrentState = ECamState.None;

	protected CamState[] CamStates = new CamState[9];

	public ECamState CurrentState
	{
		get
		{
			return m_CurrentState;
		}
		protected set
		{
			if (value == m_CurrentState)
			{
				return;
			}
			if (m_CurrentState != ECamState.None)
			{
				GetCurrentStateClass().Exit();
			}
			m_PrevState = m_CurrentState;
			m_CurrentState = value;
			if (m_CurrentState > ECamState.None)
			{
				if (m_PrevState != ECamState.None)
				{
					GetCurrentStateClass().Enter(CamStates[(int)m_PrevState].m_Transform, CamStates[(int)m_PrevState].m_Target);
				}
				else
				{
					GetCurrentStateClass().Enter(base.transform, null);
				}
			}
		}
	}

	public void Start()
	{
		CamState[] components = GetComponents<CamState>();
		CamState[] array = components;
		foreach (CamState camState in array)
		{
			if (CamStates[(int)camState.state] == null)
			{
				CamStates[(int)camState.state] = camState;
			}
		}
		m_pTransform = base.transform;
		m_PrevState = ECamState.None;
		CurrentState = m_StartState;
	}

	private void FixedUpdate()
	{
		if (CurrentState != ECamState.None)
		{
			ECamState currentState = CurrentState;
			ECamState currentState2 = GetCurrentStateClass().Manage(Time.fixedDeltaTime);
			m_pTransform.position = GetCurrentStateClass().m_Transform.position;
			m_pTransform.rotation = GetCurrentStateClass().m_Transform.rotation;
			if (currentState == CurrentState)
			{
				CurrentState = currentState2;
			}
		}
	}

	protected CamState GetCurrentStateClass()
	{
		return CamStates[(int)m_CurrentState];
	}

	public void SwitchCamera(ECamState _newCam, ECamState _transition)
	{
		if (_transition < ECamState.TransCut)
		{
		}
		((CamStateTransition)CamStates[(int)_transition]).Setup(GetCurrentStateClass(), CamStates[(int)_newCam], true);
		CurrentState = _transition;
	}
}

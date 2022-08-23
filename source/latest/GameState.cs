using System;
using UnityEngine;

public abstract class GameState : MonoBehaviour
{
	public Action<E_GameState> OnStateChanged;

	protected GameMode m_pGameMode;

	public GameMode gameMode
	{
		get
		{
			return m_pGameMode;
		}
		set
		{
			m_pGameMode = value;
		}
	}

	public GameState()
	{
	}

	private void OnDestroy()
	{
		OnStateChanged = null;
	}

	public abstract void Enter();

	public abstract void Exit();

	protected abstract void Update();
}

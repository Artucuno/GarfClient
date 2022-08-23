using UnityEngine;

public class TutorialGameState : GameState
{
	public float MinDisplayTime = 2f;

	private bool m_bShowNextTime;

	public void Awake()
	{
	}

	public override void Enter()
	{
		Camera.main.GetComponent<CameraBase>().SwitchCamera(ECamState.Follow, ECamState.TransCut);
		base.gameMode.Hud.EnterTutorial(Next);
	}

	public override void Exit()
	{
		Singleton<GameSaveManager>.Instance.SetShowTutorial(m_bShowNextTime, true);
		base.gameMode.Hud.ExitTutorial();
		base.gameMode.Hud.StartRace();
	}

	protected override void Update()
	{
	}

	public void Next(bool ShowNextTime)
	{
		m_bShowNextTime = ShowNextTime;
		OnStateChanged(E_GameState.Start);
	}
}

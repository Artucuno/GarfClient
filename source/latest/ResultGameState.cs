public class ResultGameState : GameState
{
	public override void Enter()
	{
		((InGameGameMode)m_pGameMode).UpdateScores();
		((InGameGameMode)m_pGameMode).FillResults();
	}

	public override void Exit()
	{
	}

	protected override void Update()
	{
		if (!Singleton<GameManager>.Instance.SoundManager.SoundsList[3].isPlaying && !Singleton<GameManager>.Instance.SoundManager.SoundsList[4].isPlaying)
		{
			Singleton<GameManager>.Instance.SoundManager.PlayMusic(ERaceMusicLoops.InterRace);
		}
	}
}

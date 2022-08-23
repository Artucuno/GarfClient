using UnityEngine;

public class EndRaceGameState : GameState
{
	public override void Enter()
	{
		m_pGameMode.Hud.HUDFinish.EndState = this;
		m_pGameMode.Hud.EnterFinishRace();
	}

	public override void Exit()
	{
		m_pGameMode.Hud.ExitFinishRace();
	}

	public void Next()
	{
		OnStateChanged(E_GameState.Result);
		EntryPoint component = GameObject.Find("EntryPoint").GetComponent<EntryPoint>();
		component.AskForRating = true;
		component.AskForSharing = true;
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			component.ShowInterstitial = true;
		}
	}

	protected override void Update()
	{
		if (!Singleton<GameManager>.Instance.SoundManager.SoundsList[3].isPlaying && !Singleton<GameManager>.Instance.SoundManager.SoundsList[4].isPlaying)
		{
			Singleton<GameManager>.Instance.SoundManager.SetMusic(ERaceMusicLoops.InterRace);
			Singleton<GameManager>.Instance.SoundManager.PlayMusic();
		}
	}
}

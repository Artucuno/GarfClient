using UnityEngine;

public class ChampionShipLaunchButton : MonoBehaviour
{
	public GameObject ChampionShipTracks;

	private void OnClick()
	{
		ChampionShipData component = ChampionShipTracks.GetComponent<ChampionShipData>();
		Singleton<GameConfigurator>.Instance.GameModeType = E_GameModeType.CHAMPIONSHIP;
		Singleton<GameConfigurator>.Instance.SetChampionshipData(component, false);
		Singleton<GameConfigurator>.Instance.StartScene = component.Tracks[0];
		Singleton<GameConfigurator>.Instance.CurrentTrackIndex = 0;
		Object.Destroy(GameObject.Find("MenuEntryPoint"));
		LoadingManager.LoadLevel(Singleton<GameConfigurator>.Instance.StartScene);
	}
}

using UnityEngine;

public class TrackSelectionButton : MonoBehaviour
{
	public string TrackToLaunch;

	public E_GameModeType GameModeType;

	private void OnClick()
	{
		if (TrackToLaunch.Equals("MenuRoot"))
		{
			LoadingManager.LoadLevel("MenuRoot");
			return;
		}
		Singleton<GameConfigurator>.Instance.StartScene = TrackToLaunch;
		Singleton<GameConfigurator>.Instance.GameModeType = GameModeType;
		Object.Destroy(GameObject.Find("MenuEntryPoint"));
		LoadingManager.LoadLevel(Singleton<GameConfigurator>.Instance.StartScene);
	}
}

using UnityEngine;

public class MenuTutoHub : AbstractMenu
{
	public void OnClickDriving()
	{
		Singleton<GameSaveManager>.Instance.SetShowTutorial(false, true);
		Singleton<GameConfigurator>.Instance.GameModeType = E_GameModeType.TUTORIAL;
		ChampionShipData data = (ChampionShipData)Resources.Load("ChampionShip/Champion_Ship_1", typeof(ChampionShipData));
		Singleton<GameConfigurator>.Instance.SetChampionshipData(data, false);
		Singleton<GameConfigurator>.Instance.StartScene = Singleton<GameConfigurator>.Instance.ChampionShipData.Tracks[0];
		Singleton<GameConfigurator>.Instance.CurrentTrackIndex = 0;
		LoadingManager.LoadLevel(Singleton<GameConfigurator>.Instance.StartScene);
	}

	public void OnClickBonus()
	{
		if ((bool)m_pMenuEntryPoint)
		{
			m_pMenuEntryPoint.SetState(EMenus.MENU_TUTORIAL, 0);
		}
	}

	public void OnClickCustom()
	{
		if ((bool)m_pMenuEntryPoint)
		{
			m_pMenuEntryPoint.SetState(EMenus.MENU_TUTORIAL, 2);
		}
	}
}

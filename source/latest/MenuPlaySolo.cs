using UnityEngine;

public class MenuPlaySolo : AbstractMenu
{
	public UIInput Input;

	private bool isEntered;

	public override void OnEnter()
	{
		isEntered = true;
		base.OnEnter();
		if (Input != null)
		{
			Input.text = Singleton<GameSaveManager>.Instance.GetPseudo();
			Input.defaultText = Localization.instance.Get("MENU_PLAYER");
			Input.maxChars = 8;
			Input.selected = false;
		}
		if (Network.isServer)
		{
			Network.Disconnect();
		}
	}

	public override void OnExit()
	{
		if (isEntered)
		{
			OnSubmit();
		}
		base.OnExit();
	}

	public void OnButtonSingleRace()
	{
		OnSubmit();
		ActSwapMenu(EMenus.MENU_CHAMPIONSHIP);
		Singleton<GameConfigurator>.Instance.GameModeType = E_GameModeType.SINGLE;
	}

	public void OnButtonChampionship()
	{
		OnSubmit();
		ActSwapMenu(EMenus.MENU_CHAMPIONSHIP);
		Singleton<GameConfigurator>.Instance.GameModeType = E_GameModeType.CHAMPIONSHIP;
	}

	public void OnButtonTimeTrial()
	{
		OnSubmit();
		ActSwapMenu(EMenus.MENU_CHAMPIONSHIP);
		Singleton<GameConfigurator>.Instance.GameModeType = E_GameModeType.TIME_TRIAL;
	}

	private void OnSubmit()
	{
		if (Input != null)
		{
			string text = NGUITools.StripSymbols(Input.text);
			string text2 = text.Trim();
			if (!string.IsNullOrEmpty(text2) && text2 != Localization.instance.Get("MENU_PLAYER"))
			{
				Singleton<GameSaveManager>.Instance.SetPseudo(text2, true);
			}
			else if (Input.text.Equals(string.Empty))
			{
				Singleton<GameSaveManager>.Instance.SetPseudo(string.Empty, true);
			}
			Input.selected = false;
		}
	}
}

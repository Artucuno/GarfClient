using System.Collections.Generic;
using UnityEngine;

public class MenuChampionship : AbstractMenu
{
	public List<GameObject> ChampionshipData = new List<GameObject>();

	public GameObject DifficultyButton50;

	public GameObject DifficultyButton100;

	public GameObject DifficultyButton150;

	private List<ChampionShipData> ChampionshipDataComp = new List<ChampionShipData>();

	private bool m_bNeedToInit;

	public List<ChampionshipButton> Championship = new List<ChampionshipButton>();

	public int Price;

	public override void OnEnter()
	{
		base.OnEnter();
		m_bNeedToInit = true;
		ChampionshipDataComp.Clear();
		foreach (GameObject championshipDatum in ChampionshipData)
		{
			ChampionshipDataComp.Add(championshipDatum.GetComponent<ChampionShipData>());
		}
	}

	public void LateUpdate()
	{
		if (!m_bNeedToInit)
		{
			return;
		}
		m_bNeedToInit = false;
		foreach (ChampionshipButton item in Championship)
		{
			item.Reward.gameObject.SetActive(false);
		}
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TIME_TRIAL)
		{
			if ((bool)DifficultyButton50)
			{
				DifficultyButton50.SetActive(false);
			}
			if ((bool)DifficultyButton100)
			{
				DifficultyButton100.SetActive(false);
			}
			if ((bool)DifficultyButton150)
			{
				DifficultyButton150.GetComponentInChildren<UICheckbox>().isChecked = true;
			}
			Singleton<GameConfigurator>.Instance.Difficulty = EDifficulty.HARD;
		}
		else
		{
			if ((bool)DifficultyButton50)
			{
				DifficultyButton50.SetActive(true);
			}
			if ((bool)DifficultyButton100)
			{
				DifficultyButton100.SetActive(true);
			}
			if (Singleton<GameConfigurator>.Instance != null)
			{
				switch (Singleton<GameConfigurator>.Instance.Difficulty)
				{
				case EDifficulty.EASY:
					if ((bool)DifficultyButton50)
					{
						DifficultyButton50.GetComponentInChildren<UICheckbox>().isChecked = true;
					}
					break;
				case EDifficulty.NORMAL:
					if ((bool)DifficultyButton100)
					{
						DifficultyButton100.GetComponentInChildren<UICheckbox>().isChecked = true;
					}
					break;
				case EDifficulty.HARD:
					if ((bool)DifficultyButton150)
					{
						DifficultyButton150.GetComponentInChildren<UICheckbox>().isChecked = true;
					}
					break;
				}
			}
			else
			{
				if ((bool)DifficultyButton100)
				{
					DifficultyButton100.GetComponentInChildren<UICheckbox>().isChecked = true;
				}
				Singleton<GameConfigurator>.Instance.Difficulty = EDifficulty.NORMAL;
			}
		}
		RefreshChampionship();
	}

	public override void Update()
	{
		if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
		{
			ActSwapMenu(EMenus.MENU_SOLO);
		}
	}

	public void RefreshChampionship()
	{
		int num = 0;
		foreach (ChampionshipButton item in Championship)
		{
			if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.CHAMPIONSHIP)
			{
				int rank = Singleton<GameSaveManager>.Instance.GetRank(ChampionshipData[num].name, Singleton<GameConfigurator>.Instance.Difficulty);
				if (rank >= 0 && rank < 3)
				{
					item.Reward.gameObject.SetActive(true);
					item.Reward.ChangeTexture(rank);
				}
				else
				{
					item.Reward.gameObject.SetActive(false);
				}
			}
			E_UnlockableItemSate e_UnlockableItemSate;
			if (Singleton<GameConfigurator>.Instance.GameModeType != E_GameModeType.TIME_TRIAL)
			{
				e_UnlockableItemSate = ((Singleton<GameConfigurator>.Instance.ChampionshipPass == null || !(Singleton<GameConfigurator>.Instance.ChampionshipPass.ChampionshipSelectionned == ChampionshipDataComp[num]) || Singleton<GameConfigurator>.Instance.ChampionshipPass.State == EChampionshipPassState.None || Singleton<GameConfigurator>.Instance.ChampionshipPass.Difficulty != Singleton<GameConfigurator>.Instance.Difficulty) ? Singleton<GameSaveManager>.Instance.GetChampionShipState(ChampionshipData[num].name, Singleton<GameConfigurator>.Instance.Difficulty) : E_UnlockableItemSate.Unlocked);
			}
			else if (Singleton<GameConfigurator>.Instance.ChampionshipPass != null && Singleton<GameConfigurator>.Instance.ChampionshipPass.ChampionshipSelectionned == ChampionshipDataComp[num] && Singleton<GameConfigurator>.Instance.ChampionshipPass.State != 0)
			{
				e_UnlockableItemSate = E_UnlockableItemSate.Unlocked;
			}
			else
			{
				E_UnlockableItemSate championShipState = Singleton<GameSaveManager>.Instance.GetChampionShipState(ChampionshipData[num].name, EDifficulty.EASY);
				E_UnlockableItemSate championShipState2 = Singleton<GameSaveManager>.Instance.GetChampionShipState(ChampionshipData[num].name, EDifficulty.NORMAL);
				E_UnlockableItemSate championShipState3 = Singleton<GameSaveManager>.Instance.GetChampionShipState(ChampionshipData[num].name, EDifficulty.HARD);
				e_UnlockableItemSate = ((championShipState2 <= championShipState3) ? championShipState3 : championShipState2);
				e_UnlockableItemSate = ((championShipState <= e_UnlockableItemSate) ? e_UnlockableItemSate : championShipState);
			}
			switch (e_UnlockableItemSate)
			{
			case E_UnlockableItemSate.NewUnlocked:
			case E_UnlockableItemSate.Unlocked:
				item.ButtonChampionship.gameObject.SetActive(true);
				item.ButtonPass.gameObject.SetActive(false);
				item.Collider.enabled = true;
				if ((bool)item.Hidden)
				{
					item.Hidden.SetActive(false);
				}
				break;
			case E_UnlockableItemSate.NewLocked:
			case E_UnlockableItemSate.Locked:
				item.ButtonChampionship.gameObject.SetActive(true);
				item.ButtonPass.gameObject.SetActive(true);
				item.ButtonPassLabel.text = string.Format(Localization.instance.Get("MENU_BT_PASS"), Price);
				item.Collider.enabled = false;
				if ((bool)item.Hidden)
				{
					item.Hidden.SetActive(false);
				}
				break;
			default:
				item.ButtonChampionship.gameObject.SetActive(false);
				item.ButtonPass.gameObject.SetActive(false);
				if ((bool)item.Hidden)
				{
					item.Hidden.SetActive(true);
				}
				break;
			}
			num++;
		}
	}

	public void OnSelectChampionship(int iId)
	{
		Singleton<GameConfigurator>.Instance.SetChampionshipData(ChampionshipDataComp[iId], false);
		Singleton<GameConfigurator>.Instance.CurrentTrackIndex = 0;
		ActSwapMenu(EMenus.MENU_SELECT_TRACK);
	}

	public void OnSelectRandomChampionship()
	{
		OnSelectChampionship(Random.Range(0, AvailableChampionships()));
		if (Singleton<GameConfigurator>.Instance.GameModeType != E_GameModeType.CHAMPIONSHIP)
		{
			Singleton<GameConfigurator>.Instance.CurrentTrackIndex = Random.Range(0, 4);
		}
	}

	public void OnSelectDifficulty(int iDifficulty)
	{
		switch (iDifficulty)
		{
		case 0:
			Singleton<GameConfigurator>.Instance.Difficulty = EDifficulty.EASY;
			break;
		case 1:
			Singleton<GameConfigurator>.Instance.Difficulty = EDifficulty.NORMAL;
			break;
		case 2:
			Singleton<GameConfigurator>.Instance.Difficulty = EDifficulty.HARD;
			break;
		}
		RefreshChampionship();
	}

	public int AvailableChampionships()
	{
		int num = 0;
		foreach (ChampionShipData item in ChampionshipDataComp)
		{
			if (Singleton<GameSaveManager>.Instance.GetChampionShipState(item.name, Singleton<GameConfigurator>.Instance.Difficulty) == E_UnlockableItemSate.NewUnlocked || Singleton<GameSaveManager>.Instance.GetChampionShipState(item.name, Singleton<GameConfigurator>.Instance.Difficulty) == E_UnlockableItemSate.Unlocked)
			{
				num++;
			}
		}
		return num;
	}

	public void OnPass(int iId)
	{
		m_pMenuEntryPoint.ShowPurchasePopup(string.Format(Localization.instance.Get("MENU_POPUP_BUY_ITEM_CONFIRMATION_CHAMPIONSHIP"), Price), Price, PurchaseItem, null, iId);
	}

	public void PurchaseItem(object oParam)
	{
		Singleton<GameConfigurator>.Instance.SetChampionshipData(ChampionshipDataComp[(int)oParam], true);
		ActSwapMenu(EMenus.MENU_SELECT_TRACK);
	}

	public void OnShop()
	{
		m_pMenuEntryPoint.SetState(EMenus.MENU_SELECT_KART, 1);
	}

	public void OnUnlock()
	{
		GkUtils.UnlockAll();
		RefreshChampionship();
	}
}

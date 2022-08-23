using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuSelectKart : AbstractMenu
{
	public enum EAdvantageRestrictions
	{
		NO_RESTRICTION,
		SINGLE_RESTRICTION,
		CHAMPIONSHIP_RESTRICTION,
		TIMETRIAL_RESTRICTION,
		MULTI_SINGLE_RESTRICTION,
		MULTI_CHAMP_RESTRICTION
	}

	public GameObject m_oScrollPanel;

	public GameObject m_oButtonItemTemplate;

	public GameObject m_oKartPreview;

	public UILabel m_oItemTitle;

	public UILabel m_oItemDescription;

	public GameObject m_oBuyItemButton;

	private int m_iCurrentTab;

	private Transform m_oGrid;

	private bool m_bNeedSelectTabAtUpdate;

	private bool m_bNeedGoBuyCoins;

	private bool m_bBackToPreviousState;

	private IconCarac m_pIconSelected;

	private NetworkMgr m_oNetworkMgr;

	private bool goNet;

	private bool wasServer;

	private Transform m_oBtnItemSelected;

	public GameObject m_oPanelAdvantages;

	public GameObject m_oPanelDataKart;

	public GameObject m_oPanelDataIcon;

	public GameObject m_oButtonCoins;

	public GameObject m_oButtonNext;

	public GameObject m_oButtonAdvantages;

	public GameObject m_oButtonPersonage;

	public GameObject NetworkReady;

	public UIPanel m_oPanelText;

	public GameObject m_oPanelInApp;

	public UILabel m_oInAppTitle;

	public UILabel m_oInAppDescription;

	public UILabel m_oInAppPrice;

	public UIAtlas m_oAdvAtlas;

	private ECharacter m_eLastValidCharacter;

	private ECharacter m_eLastValidKart;

	private BonusCustom m_oLastValidHat;

	private KartCustom m_oLastValidKartCustom;

	public int m_iTimeLimit = 25;

	public int m_iStartDisplaying = 10;

	public UILabel m_oTimerLabel;

	public float m_elapsedTime;

	private bool m_bLimitTime = true;

	private float m_fElapsedTime;

	private bool m_bTrackInfoLoadingDone;

	public AudioSource CustomSound;

	public AudioSource HatSound;

	public AudioSource AdvantageSound;

	public AudioSource CoinsSound;

	public UILabel TrackName;

	public UITexturePattern ChampionshipIcon;

	public UITexturePattern GameModeIcon;

	public UITexturePattern DifficultyIcon;

	public List<UITexturePattern> ClientState;

	public GameObject ClientStateGO;

	public GameObject AnchorTopLeft;

	public Camera m_oCameraShop;

	public bool ShowShop;

	private EntryPoint m_pEntryPoint;

	private EAdvantageRestrictions m_eAdvantageRestrictions;

	private List<CharacterCarac> m_oCharacterList = new List<CharacterCarac>();

	private List<BonusCustom> m_oHatList = new List<BonusCustom>();

	private List<KartCarac> m_oKartList = new List<KartCarac>();

	private List<KartCustom> m_oKartCustomList = new List<KartCustom>();

	private List<InAppCarac> m_oCoinsCaracList = new List<InAppCarac>();

	private List<AdvantageData> m_oAdvantagesList = new List<AdvantageData>();

	public GameObject ButtonPersonage
	{
		get
		{
			if (!m_oButtonPersonage)
			{
				m_oButtonPersonage = GameObject.Find("ButtonPersonage");
			}
			return m_oButtonPersonage;
		}
	}

	public override void Awake()
	{
		base.Awake();
		m_oNetworkMgr = (NetworkMgr)UnityEngine.Object.FindObjectOfType(typeof(NetworkMgr));
		UnityEngine.Object[] array = Resources.LoadAll("Character", typeof(CharacterCarac));
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object @object in array2)
		{
			m_oCharacterList.Add((CharacterCarac)@object);
		}
		UnityEngine.Object[] array3 = Resources.LoadAll("Hat", typeof(BonusCustom));
		UnityEngine.Object[] array4 = array3;
		foreach (UnityEngine.Object object2 in array4)
		{
			m_oHatList.Add((BonusCustom)object2);
		}
		UnityEngine.Object[] array5 = Resources.LoadAll("Kart", typeof(KartCarac));
		UnityEngine.Object[] array6 = array5;
		foreach (UnityEngine.Object object3 in array6)
		{
			m_oKartList.Add((KartCarac)object3);
		}
		UnityEngine.Object[] array7 = Resources.LoadAll("Kart", typeof(KartCustom));
		UnityEngine.Object[] array8 = array7;
		foreach (UnityEngine.Object object4 in array8)
		{
			if (object4.name.Contains("_Def"))
			{
				m_oKartCustomList.Insert(0, (KartCustom)object4);
			}
			else
			{
				m_oKartCustomList.Add((KartCustom)object4);
			}
		}
		UnityEngine.Object[] array9 = Resources.LoadAll("InApp", typeof(InAppCarac));
		UnityEngine.Object[] array10 = array9;
		foreach (UnityEngine.Object object5 in array10)
		{
			m_oCoinsCaracList.Add((InAppCarac)object5);
		}
		UnityEngine.Object[] array11 = Resources.LoadAll("Advantages", typeof(AdvantageData));
		UnityEngine.Object[] array12 = array11;
		foreach (UnityEngine.Object object6 in array12)
		{
			m_oAdvantagesList.Add((AdvantageData)object6);
		}
		SortLists();
		InAppManager instance = Singleton<InAppManager>.Instance;
		instance.OnPurchaseSucceed = (AInAppService.PurchaseDelegate)Delegate.Combine(instance.OnPurchaseSucceed, new AInAppService.PurchaseDelegate(PurchaseSucceed));
		InAppManager instance2 = Singleton<InAppManager>.Instance;
		instance2.OnPurchaseFailed = (AInAppService.PurchaseDelegate)Delegate.Combine(instance2.OnPurchaseFailed, new AInAppService.PurchaseDelegate(PurchaseFailed));
		InAppManager instance3 = Singleton<InAppManager>.Instance;
		instance3.OnPurchaseCancelled = (AInAppService.PurchaseDelegate)Delegate.Combine(instance3.OnPurchaseCancelled, new AInAppService.PurchaseDelegate(PurchaseFailed));
		m_oGrid = m_oScrollPanel.transform.GetChild(0);
		m_bNeedSelectTabAtUpdate = true;
		m_bNeedGoBuyCoins = false;
		string rpCustom = string.Empty;
		string rpHat = string.Empty;
		Singleton<GameSaveManager>.Instance.GetPlayerConfig(ref Singleton<GameConfigurator>.Instance.PlayerConfig.m_eCharacter, ref Singleton<GameConfigurator>.Instance.PlayerConfig.m_eKart, ref rpCustom, ref rpHat);
		if (Singleton<GameConfigurator>.Instance.PlayerConfig.m_eCharacter == ECharacter.NONE)
		{
			Singleton<GameConfigurator>.Instance.PlayerConfig.m_eCharacter = ECharacter.GARFIELD;
		}
		else
		{
			E_UnlockableItemSate characterState = Singleton<GameSaveManager>.Instance.GetCharacterState(Singleton<GameConfigurator>.Instance.PlayerConfig.m_eCharacter);
			if (characterState == E_UnlockableItemSate.NewUnlocked || characterState == E_UnlockableItemSate.Unlocked)
			{
				m_eLastValidCharacter = Singleton<GameConfigurator>.Instance.PlayerConfig.m_eCharacter;
			}
			else
			{
				m_eLastValidCharacter = ECharacter.GARFIELD;
			}
		}
		if (Singleton<GameConfigurator>.Instance.PlayerConfig.m_eKart == ECharacter.NONE)
		{
			Singleton<GameConfigurator>.Instance.PlayerConfig.m_eKart = ECharacter.JON;
		}
		else
		{
			E_UnlockableItemSate characterState = Singleton<GameSaveManager>.Instance.GetKartState(Singleton<GameConfigurator>.Instance.PlayerConfig.m_eKart);
			if (characterState == E_UnlockableItemSate.NewUnlocked || characterState == E_UnlockableItemSate.Unlocked)
			{
				m_eLastValidKart = Singleton<GameConfigurator>.Instance.PlayerConfig.m_eKart;
			}
			else
			{
				m_eLastValidKart = ECharacter.JON;
			}
		}
		if (rpHat == "None")
		{
			rpHat = Singleton<GameConfigurator>.Instance.PlayerConfig.m_eCharacter.ToString()[0] + "_DefaultHat";
		}
		else
		{
			E_UnlockableItemSate characterState = Singleton<GameSaveManager>.Instance.GetHatState(rpHat);
			if (characterState != E_UnlockableItemSate.NewUnlocked && characterState != E_UnlockableItemSate.Unlocked)
			{
				rpHat = Singleton<GameConfigurator>.Instance.PlayerConfig.m_eCharacter.ToString()[0] + "_DefaultHat";
			}
		}
		foreach (BonusCustom oHat in m_oHatList)
		{
			if (oHat.name == rpHat)
			{
				m_oLastValidHat = oHat;
				Singleton<GameConfigurator>.Instance.PlayerConfig.m_oHat = oHat;
			}
		}
		if (rpCustom == "None")
		{
			rpCustom = "K" + Singleton<GameConfigurator>.Instance.PlayerConfig.m_eKart.ToString()[0] + "C_Default";
		}
		else
		{
			E_UnlockableItemSate characterState = Singleton<GameSaveManager>.Instance.GetCustomState(rpCustom);
			if (characterState != E_UnlockableItemSate.NewUnlocked && characterState != E_UnlockableItemSate.Unlocked)
			{
				rpCustom = "K" + Singleton<GameConfigurator>.Instance.PlayerConfig.m_eKart.ToString()[0] + "C_Default";
			}
		}
		foreach (KartCustom oKartCustom in m_oKartCustomList)
		{
			if (oKartCustom.name == rpCustom)
			{
				m_oLastValidKartCustom = oKartCustom;
				Singleton<GameConfigurator>.Instance.PlayerConfig.m_oKartCustom = oKartCustom;
			}
		}
		m_pEntryPoint = GameObject.Find("EntryPoint").GetComponent<EntryPoint>();
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			ShowShop = true;
		}
	}

	private void OnDestroy()
	{
		InAppManager instance = Singleton<InAppManager>.Instance;
		instance.OnPurchaseSucceed = (AInAppService.PurchaseDelegate)Delegate.Remove(instance.OnPurchaseSucceed, new AInAppService.PurchaseDelegate(PurchaseSucceed));
		InAppManager instance2 = Singleton<InAppManager>.Instance;
		instance2.OnPurchaseFailed = (AInAppService.PurchaseDelegate)Delegate.Remove(instance2.OnPurchaseFailed, new AInAppService.PurchaseDelegate(PurchaseFailed));
		InAppManager instance3 = Singleton<InAppManager>.Instance;
		instance3.OnPurchaseCancelled = (AInAppService.PurchaseDelegate)Delegate.Remove(instance3.OnPurchaseCancelled, new AInAppService.PurchaseDelegate(PurchaseFailed));
		m_oCharacterList.Clear();
		m_oHatList.Clear();
		m_oKartList.Clear();
		m_oKartCustomList.Clear();
		m_oCoinsCaracList.Clear();
		m_oAdvantagesList.Clear();
		m_oNetworkMgr.ResetReadyStates();
	}

	public override void Start()
	{
		base.Start();
	}

	public override void OnEnter()
	{
		OnEnter(0);
		wasServer = Network.isServer;
		SortLists();
	}

	public override void OnEnter(int iEntryPoint)
	{
		base.OnEnter();
		goNet = false;
		NetworkReady.SetActive(false);
		if (!ShowShop)
		{
			m_oButtonCoins.SetActive(false);
		}
		else
		{
			m_oButtonCoins.SetActive(true);
		}
		if (Network.isServer)
		{
			m_oNetworkMgr.networkView.RPC("SetMenu", RPCMode.Others, 4);
			m_oNetworkMgr.networkView.RPC("ShareTrackChoice", RPCMode.All, (int)Singleton<GameConfigurator>.Instance.GameModeType, Singleton<GameConfigurator>.Instance.ChampionShipData.Index, Singleton<GameConfigurator>.Instance.CurrentTrackIndex, (int)Singleton<GameConfigurator>.Instance.Difficulty);
		}
		m_eAdvantageRestrictions = EAdvantageRestrictions.TIMETRIAL_RESTRICTION;
		if (iEntryPoint == 1)
		{
			m_eAdvantageRestrictions = EAdvantageRestrictions.NO_RESTRICTION;
			if ((bool)m_oButtonAdvantages && (bool)m_oButtonAdvantages.collider)
			{
				m_oButtonAdvantages.collider.enabled = true;
			}
			m_oPanelText.gameObject.SetActive(false);
			m_oPanelInApp.SetActive(true);
			m_oBuyItemButton.SetActive(false);
			AnchorTopLeft.SetActive(false);
			ClientStateGO.SetActive(false);
		}
		else
		{
			m_oPanelText.gameObject.SetActive(true);
			m_oPanelInApp.SetActive(false);
			AnchorTopLeft.SetActive(true);
			DifficultyIcon.ChangeTexture((int)Singleton<GameConfigurator>.Instance.Difficulty);
			if ((Network.peerType == NetworkPeerType.Disconnected || m_oNetworkMgr.TrackChoiceReceived) && Singleton<GameConfigurator>.Instance.ChampionShipData != null)
			{
				UpdateTrackInfo();
			}
			else
			{
				m_bTrackInfoLoadingDone = false;
				m_eAdvantageRestrictions = EAdvantageRestrictions.MULTI_CHAMP_RESTRICTION;
			}
			if ((bool)m_oButtonAdvantages && (bool)m_oButtonAdvantages.collider)
			{
				if (m_eAdvantageRestrictions == EAdvantageRestrictions.TIMETRIAL_RESTRICTION)
				{
					m_oButtonAdvantages.collider.enabled = false;
					m_oButtonAdvantages.SetActive(false);
					if (m_iCurrentTab == 4)
					{
						OpenBuyCoinsTab(true);
					}
				}
				else
				{
					m_oButtonAdvantages.SetActive(true);
					m_oButtonAdvantages.collider.enabled = true;
				}
			}
			if (Network.peerType != 0)
			{
				ClientStateGO.SetActive(true);
			}
			else
			{
				ClientStateGO.SetActive(false);
			}
		}
		if ((bool)m_oButtonNext)
		{
			m_oButtonNext.SetActive(true);
		}
		if ((bool)m_oKartPreview)
		{
			m_oKartPreview.SendMessage("Init");
		}
		if (!m_bNeedSelectTabAtUpdate)
		{
			int iCurrentTab = m_iCurrentTab;
			m_iCurrentTab = -1;
			OnSelectTab(iCurrentTab);
			UpdatePreviewKart();
		}
		if (iEntryPoint == 1)
		{
			if (!m_bNeedSelectTabAtUpdate)
			{
				OpenBuyCoinsTab(false);
			}
			else
			{
				m_bNeedGoBuyCoins = true;
			}
		}
		m_bBackToPreviousState = ((iEntryPoint != 0) ? true : false);
		if ((bool)m_oPanelAdvantages)
		{
			PanelAdvantages component = m_oPanelAdvantages.GetComponent<PanelAdvantages>();
			component.Initialize();
		}
		if (Network.peerType == NetworkPeerType.Disconnected || m_oTimerLabel == null)
		{
			m_bLimitTime = false;
		}
		else
		{
			m_fElapsedTime = 0f;
			m_bLimitTime = true;
		}
		if (m_oTimerLabel != null)
		{
			m_oTimerLabel.enabled = false;
		}
		m_elapsedTime = 0f;
		if (Network.peerType != 0)
		{
			for (int i = 0; i < ClientState.Count; i++)
			{
				ClientState[i].ChangeTexture(m_oNetworkMgr.SelectedColors.Count);
				ClientState[i].gameObject.SetActive(i < m_oNetworkMgr.ReadyToGo.Count);
			}
		}
	}

	private void UpdateTrackInfo()
	{
		ChampionshipIcon.ChangeTexture(Singleton<GameConfigurator>.Instance.ChampionShipData.Index);
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.CHAMPIONSHIP)
		{
			if (Network.isServer || Network.isClient)
			{
				m_eAdvantageRestrictions = EAdvantageRestrictions.MULTI_CHAMP_RESTRICTION;
			}
			else
			{
				m_eAdvantageRestrictions = EAdvantageRestrictions.CHAMPIONSHIP_RESTRICTION;
			}
			TrackName.text = Singleton<GameConfigurator>.Instance.ChampionShipData.ChampionShipName;
			GameModeIcon.ChangeTexture(0);
		}
		else if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.SINGLE)
		{
			GameModeIcon.ChangeTexture(1);
			TrackName.text = Singleton<GameConfigurator>.Instance.ChampionShipData.TracksName[Singleton<GameConfigurator>.Instance.CurrentTrackIndex];
			if (Network.isServer || Network.isClient)
			{
				m_eAdvantageRestrictions = EAdvantageRestrictions.MULTI_SINGLE_RESTRICTION;
			}
			else
			{
				m_eAdvantageRestrictions = EAdvantageRestrictions.SINGLE_RESTRICTION;
			}
		}
		else if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TIME_TRIAL)
		{
			GameModeIcon.ChangeTexture(2);
			TrackName.text = Singleton<GameConfigurator>.Instance.ChampionShipData.TracksName[Singleton<GameConfigurator>.Instance.CurrentTrackIndex];
		}
		m_bTrackInfoLoadingDone = true;
	}

	public override void OnExit()
	{
		if ((bool)m_oKartPreview)
		{
			m_oKartPreview.SendMessage("DestroyKartPreview");
		}
		base.OnExit();
	}

	private void LateUpdate()
	{
		if (m_bNeedSelectTabAtUpdate)
		{
			int iCurrentTab = m_iCurrentTab;
			m_iCurrentTab = -1;
			OnSelectTab(iCurrentTab);
			UpdatePreviewKart();
			if (m_bNeedGoBuyCoins)
			{
				OpenBuyCoinsTab(false);
				m_bNeedGoBuyCoins = false;
			}
			m_bNeedSelectTabAtUpdate = false;
		}
		if (m_bLimitTime)
		{
			m_fElapsedTime += Time.deltaTime;
			if (m_fElapsedTime > (float)(m_iTimeLimit - m_iStartDisplaying))
			{
				m_oTimerLabel.enabled = true;
				m_oTimerLabel.text = ((int)((float)m_iTimeLimit - m_fElapsedTime)).ToString();
				if (m_fElapsedTime >= (float)m_iTimeLimit)
				{
					m_oTimerLabel.enabled = false;
					m_bLimitTime = false;
					OnTimeOut();
				}
			}
		}
		m_elapsedTime += Time.deltaTime;
	}

	public override void Update()
	{
		if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
		{
			OnButtonBack();
		}
		if (Network.isServer && goNet && !m_oNetworkMgr.WaitingSynchronization)
		{
			m_oNetworkMgr.DispatchIds();
			m_oNetworkMgr.networkView.RPC("Go", RPCMode.All, Singleton<GameConfigurator>.Instance.StartScene);
		}
		if (Network.peerType != 0)
		{
			Dictionary<NetworkPlayer, bool> readyToGo = m_oNetworkMgr.ReadyToGo;
			int num = 0;
			foreach (KeyValuePair<NetworkPlayer, bool> item in readyToGo)
			{
				if (item.Value)
				{
					ClientState[num].ChangeTexture(m_oNetworkMgr.SelectedColors.IndexOf(m_oNetworkMgr.PlayersColor[item.Key]));
				}
				num++;
			}
		}
		else if (goNet && wasServer)
		{
			Go(null);
		}
		if (!m_bTrackInfoLoadingDone && m_oNetworkMgr.TrackChoiceReceived && Singleton<GameConfigurator>.Instance.ChampionShipData != null)
		{
			UpdateTrackInfo();
		}
	}

	public void OnButtonGo()
	{
		string sItemTypeInvalidTextId;
		if (IsNextButtonValid(out sItemTypeInvalidTextId))
		{
			Singleton<GameConfigurator>.Instance.PlayerConfig.GetAdvantages().Clear();
			PanelAdvantages component = m_oPanelAdvantages.GetComponent<PanelAdvantages>();
			if ((bool)component)
			{
				component.ValidSlots();
			}
			if (Singleton<GameConfigurator>.Instance.PlayerConfig.GetAdvantages().Count == 0 && Singleton<GameConfigurator>.Instance.GameModeType != E_GameModeType.TIME_TRIAL && !Singleton<ChallengeManager>.Instance.IsActive)
			{
				OnSelectTab(4);
				m_oButtonAdvantages.GetComponent<UICheckbox>().isChecked = true;
				Popup2Choices popup2Choices = (Popup2Choices)m_pMenuEntryPoint.ShowPopup(EPopUps.POPUP_DIALOG_2CHOICES, false);
				if ((bool)popup2Choices)
				{
					Popup2Choices.Callback oCbRight = Go;
					popup2Choices.Show("MENU_POPUP_NO_ADVANTAGE", null, oCbRight);
				}
			}
			else
			{
				Go(null);
			}
		}
		else
		{
			PopupDialog popupDialog = (PopupDialog)m_pMenuEntryPoint.ShowPopup(EPopUps.POPUP_DIALOG, true);
			popupDialog.gameObject.SetActive(true);
			UILabel component2 = popupDialog.Text.gameObject.GetComponent<UILabel>();
			if ((bool)component2)
			{
				component2.text = string.Format(Localization.instance.Get("MENU_SHOP_OBJECT_UNAVAILABLE"), Localization.instance.Get(sItemTypeInvalidTextId));
			}
		}
	}

	private void RevertToLastValidState()
	{
		GameSaveManager instance = Singleton<GameSaveManager>.Instance;
		PlayerConfig playerConfig = Singleton<GameConfigurator>.Instance.PlayerConfig;
		E_UnlockableItemSate characterState = instance.GetCharacterState(playerConfig.m_eCharacter);
		if (characterState != E_UnlockableItemSate.NewUnlocked && characterState != E_UnlockableItemSate.Unlocked)
		{
			playerConfig.m_eCharacter = m_eLastValidCharacter;
			if (playerConfig.m_oHat.Owner != ECharacter.NONE && playerConfig.m_oHat.Owner != m_eLastValidCharacter)
			{
				if (m_oLastValidHat.Owner == ECharacter.NONE || m_oLastValidHat.Owner == m_eLastValidCharacter)
				{
					playerConfig.m_oHat = m_oLastValidHat;
				}
				else
				{
					foreach (BonusCustom oHat in m_oHatList)
					{
						if (oHat.Owner == m_eLastValidCharacter)
						{
							playerConfig.m_oHat = oHat;
							break;
						}
					}
				}
			}
		}
		characterState = instance.GetKartState(playerConfig.m_eKart);
		if (characterState != E_UnlockableItemSate.NewUnlocked && characterState != E_UnlockableItemSate.Unlocked)
		{
			playerConfig.m_eKart = m_eLastValidKart;
			if (playerConfig.m_oKartCustom.Character != ECharacter.NONE && playerConfig.m_oKartCustom.Character != m_eLastValidKart)
			{
				if (m_oLastValidKartCustom.Character == ECharacter.NONE || m_oLastValidKartCustom.Character == m_eLastValidKart)
				{
					playerConfig.m_oKartCustom = m_oLastValidKartCustom;
				}
				else
				{
					foreach (KartCustom oKartCustom in m_oKartCustomList)
					{
						if (oKartCustom.Owner == m_eLastValidKart)
						{
							playerConfig.m_oKartCustom = oKartCustom;
							break;
						}
					}
				}
			}
		}
		characterState = instance.GetHatState(playerConfig.m_oHat.name);
		if (characterState != E_UnlockableItemSate.NewUnlocked && characterState != E_UnlockableItemSate.Unlocked)
		{
			playerConfig.m_oHat = m_oLastValidHat;
		}
		characterState = instance.GetCustomState(playerConfig.m_oKartCustom.name);
		if (characterState != E_UnlockableItemSate.NewUnlocked && characterState != E_UnlockableItemSate.Unlocked)
		{
			playerConfig.m_oKartCustom = m_oLastValidKartCustom;
		}
		KartCarac kartCarac = (KartCarac)Resources.Load("Kart/" + playerConfig.KartPrefab[(int)playerConfig.m_eKart], typeof(KartCarac));
		CharacterCarac characterCarac = (CharacterCarac)Resources.Load("Character/" + playerConfig.CharacterPrefab[(int)playerConfig.m_eCharacter], typeof(CharacterCarac));
		Singleton<GameConfigurator>.Instance.NbSlots = kartCarac.NbSlots + characterCarac.NbSlots + playerConfig.m_oHat.NbSlots + playerConfig.m_oKartCustom.NbSlots;
	}

	private void OnTimeOut()
	{
		RevertToLastValidState();
		Singleton<GameConfigurator>.Instance.PlayerConfig.GetAdvantages().Clear();
		PanelAdvantages component = m_oPanelAdvantages.GetComponent<PanelAdvantages>();
		if ((bool)component)
		{
			component.ValidSlots();
		}
		Go(null);
	}

	public void OnButtonBack()
	{
		RevertToLastValidState();
		Singleton<GameSaveManager>.Instance.SetPlayerConfig(Singleton<GameConfigurator>.Instance.PlayerConfig.m_eCharacter, Singleton<GameConfigurator>.Instance.PlayerConfig.m_eKart, Singleton<GameConfigurator>.Instance.PlayerConfig.m_oKartCustom.name, Singleton<GameConfigurator>.Instance.PlayerConfig.m_oHat.name, true);
		if (LogManager.Instance != null)
		{
		}
		if (m_bBackToPreviousState)
		{
			m_pMenuEntryPoint.SetPreviousState();
			if (ASE_Tools.Available)
			{
				ASE_ChartBoost.ShowInterstitial("Default");
			}
		}
		else if (Network.isClient || Network.isServer)
		{
			ActSwapMenu(EMenus.MENU_MULTI_JOIN);
			Network.Disconnect();
		}
		else
		{
			ActSwapMenu(EMenus.MENU_SELECT_TRACK);
		}
	}

	public void Go(object oParam = null)
	{
		Singleton<GameSaveManager>.Instance.SetPlayerConfig(Singleton<GameConfigurator>.Instance.PlayerConfig.m_eCharacter, Singleton<GameConfigurator>.Instance.PlayerConfig.m_eKart, Singleton<GameConfigurator>.Instance.PlayerConfig.m_oKartCustom.name, Singleton<GameConfigurator>.Instance.PlayerConfig.m_oHat.name, true);
		if (LogManager.Instance != null)
		{
			PlayerConfig playerConfig = Singleton<GameConfigurator>.Instance.PlayerConfig;
			foreach (EAdvantage advantage in playerConfig.GetAdvantages())
			{
			}
			if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) && Singleton<GameOptionManager>.Instance.GetInputType() != E_InputType.Gyroscopic)
			{
			}
		}
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			LoadingManager.LoadLevel(Singleton<GameConfigurator>.Instance.StartScene);
		}
		else if (!goNet)
		{
			goNet = true;
			m_oButtonNext.SetActive(false);
			NetworkReady.SetActive(true);
			if (!m_oNetworkMgr.WaitingSynchronization)
			{
				m_oNetworkMgr.networkView.RPC("SetReadyToGo", RPCMode.All, Network.player, true);
				m_oNetworkMgr.StartSynchronization();
			}
		}
	}

	public void RemoveAllItems()
	{
		if ((bool)m_oScrollPanel && (bool)m_oGrid)
		{
			for (int num = m_oGrid.GetChildCount(); num > 0; num--)
			{
				UnityEngine.Object.Destroy(m_oGrid.GetChild(num - 1).gameObject);
			}
		}
	}

	public void OnSelectTab(int iTab)
	{
		if (iTab == m_iCurrentTab)
		{
			return;
		}
		bool flag = true;
		m_iCurrentTab = iTab;
		RemoveAllItems();
		if ((bool)m_oPanelAdvantages)
		{
			PanelAdvantages component = m_oPanelAdvantages.GetComponent<PanelAdvantages>();
			if ((bool)component && component.enabled)
			{
				if (m_iCurrentTab == 5)
				{
					component.HiddenExit();
				}
				else
				{
					component.OnExit();
				}
			}
			if ((bool)m_oKartPreview && (bool)m_oKartPreview.collider)
			{
				m_oKartPreview.collider.enabled = true;
			}
		}
		Camera camera = m_oCamera;
		m_oPanelText.gameObject.SetActive(true);
		m_oPanelInApp.SetActive(false);
		switch (m_iCurrentTab)
		{
		case 0:
			InitCharacters();
			break;
		case 1:
			InitKarts();
			break;
		case 2:
			InitHats();
			break;
		case 3:
			InitKartCustoms();
			break;
		case 4:
			flag = false;
			InitAdvantages();
			camera = m_oCameraShop;
			break;
		case 5:
			m_oPanelText.gameObject.SetActive(false);
			m_oPanelInApp.SetActive(true);
			m_oBuyItemButton.SetActive(false);
			flag = false;
			InitCoins();
			camera = m_oCameraShop;
			break;
		}
		if ((bool)m_pMenuEntryPoint && (bool)m_oCamera && (bool)m_oCameraShop)
		{
			m_pMenuEntryPoint.SetCamera(camera);
		}
		if ((bool)m_oPanelDataKart)
		{
			m_oPanelDataKart.SetActive(flag);
		}
		if ((bool)m_oPanelDataIcon)
		{
			m_oPanelDataIcon.SetActive(flag);
		}
	}

	private BtnItem AddItem()
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(m_oButtonItemTemplate);
		if (!gameObject)
		{
			return null;
		}
		Vector3 position = gameObject.transform.position;
		position.z = 0f;
		gameObject.transform.position = position;
		gameObject.transform.parent = m_oGrid.transform;
		return gameObject.GetComponent<BtnItem>();
	}

	public void InitCharacters()
	{
		if (!m_oScrollPanel || !m_oButtonItemTemplate || !m_oGrid)
		{
			return;
		}
		bool flag = false;
		Transform oBtn = null;
		GameSaveManager instance = Singleton<GameSaveManager>.Instance;
		ECharacter eCharacter = Singleton<GameConfigurator>.Instance.PlayerConfig.m_eCharacter;
		foreach (CharacterCarac oCharacter in m_oCharacterList)
		{
			E_UnlockableItemSate characterState = instance.GetCharacterState(oCharacter.Owner);
			BtnItem btnItem = AddItem();
			flag = ((oCharacter.Owner == eCharacter) ? true : false);
			btnItem.Init(base.gameObject, m_oScrollPanel, oCharacter.spriteName, ERarity.Base, 0, oCharacter, characterState, flag);
			if (flag)
			{
				oBtn = btnItem.gameObject.transform;
				UpdateTextPanel(oCharacter, characterState);
			}
		}
		UpdatePanel(oBtn);
	}

	public void InitKarts()
	{
		if (!m_oScrollPanel || !m_oButtonItemTemplate || !m_oGrid)
		{
			return;
		}
		bool flag = true;
		Transform oBtn = null;
		GameSaveManager instance = Singleton<GameSaveManager>.Instance;
		ECharacter eKart = Singleton<GameConfigurator>.Instance.PlayerConfig.m_eKart;
		foreach (KartCarac oKart in m_oKartList)
		{
			E_UnlockableItemSate kartState = instance.GetKartState(oKart.Owner);
			BtnItem btnItem = AddItem();
			flag = ((oKart.Owner == eKart) ? true : false);
			btnItem.Init(base.gameObject, m_oScrollPanel, oKart.spriteName, ERarity.Base, Singleton<GameConfigurator>.Instance.PriceConfig.GetKartPrice(), oKart, kartState, flag);
			if (flag)
			{
				oBtn = btnItem.gameObject.transform;
				UpdateTextPanel(oKart, kartState);
			}
		}
		UpdatePanel(oBtn);
	}

	public void InitHats()
	{
		if (!m_oScrollPanel || !m_oButtonItemTemplate || !m_oGrid)
		{
			return;
		}
		bool flag = true;
		Transform oBtn = null;
		GameSaveManager instance = Singleton<GameSaveManager>.Instance;
		ECharacter eCharacter = Singleton<GameConfigurator>.Instance.PlayerConfig.m_eCharacter;
		string text = Singleton<GameConfigurator>.Instance.PlayerConfig.m_oHat.name;
		foreach (BonusCustom oHat in m_oHatList)
		{
			if (oHat.Owner != ECharacter.NONE && oHat.Owner != eCharacter)
			{
				continue;
			}
			E_UnlockableItemSate hatState = instance.GetHatState(oHat.name);
			if (hatState != 0 || (hatState == E_UnlockableItemSate.Hidden && oHat.Rarity == ERarity.Base))
			{
				BtnItem btnItem = AddItem();
				flag = ((oHat.name == text) ? true : false);
				btnItem.Init(base.gameObject, m_oScrollPanel, oHat.spriteName, oHat.Rarity, Singleton<GameConfigurator>.Instance.PriceConfig.GetHatPrice(oHat.Rarity, oHat.Owner == ECharacter.NONE), oHat, hatState, flag);
				if (flag)
				{
					oBtn = btnItem.gameObject.transform;
					UpdateTextPanel(oHat, hatState);
				}
			}
		}
		UpdatePanel(oBtn);
	}

	public void InitKartCustoms()
	{
		if (!m_oScrollPanel || !m_oButtonItemTemplate)
		{
			return;
		}
		bool flag = true;
		Transform oBtn = null;
		GameSaveManager instance = Singleton<GameSaveManager>.Instance;
		ECharacter eKart = Singleton<GameConfigurator>.Instance.PlayerConfig.m_eKart;
		string text = Singleton<GameConfigurator>.Instance.PlayerConfig.m_oKartCustom.name;
		foreach (KartCustom oKartCustom in m_oKartCustomList)
		{
			if (oKartCustom.Character != ECharacter.NONE && oKartCustom.Character != eKart)
			{
				continue;
			}
			E_UnlockableItemSate customState = instance.GetCustomState(oKartCustom.name);
			if (customState != 0 || (customState == E_UnlockableItemSate.Hidden && oKartCustom.Rarity == ERarity.Base))
			{
				BtnItem btnItem = AddItem();
				flag = ((oKartCustom.name == text) ? true : false);
				btnItem.Init(base.gameObject, m_oScrollPanel, oKartCustom.spriteName, oKartCustom.Rarity, Singleton<GameConfigurator>.Instance.PriceConfig.GetCustoPrice(oKartCustom.Rarity), oKartCustom, customState, flag);
				if (flag)
				{
					oBtn = btnItem.gameObject.transform;
					UpdateTextPanel(oKartCustom, customState);
				}
			}
		}
		UpdatePanel(oBtn);
	}

	public bool IsAdvantageAvailable(EAdvantage eAdvantage)
	{
		foreach (AdvantageData oAdvantages in m_oAdvantagesList)
		{
			if (oAdvantages.AdvantageType == eAdvantage)
			{
				return IsAdvantageAvailable(oAdvantages);
			}
		}
		return false;
	}

	private bool IsAdvantageAvailable(AdvantageData oAdvantage)
	{
		switch (m_eAdvantageRestrictions)
		{
		case EAdvantageRestrictions.SINGLE_RESTRICTION:
			return oAdvantage.bIsAvailableInSingle;
		case EAdvantageRestrictions.CHAMPIONSHIP_RESTRICTION:
			return oAdvantage.bIsAvailableInChampionship;
		case EAdvantageRestrictions.MULTI_SINGLE_RESTRICTION:
			return (oAdvantage.bIsAvailableInSingle && oAdvantage.bIsAvailableInMulti) ? true : false;
		case EAdvantageRestrictions.MULTI_CHAMP_RESTRICTION:
			return (oAdvantage.bIsAvailableInChampionship && oAdvantage.bIsAvailableInMulti) ? true : false;
		default:
			return true;
		}
	}

	public void InitAdvantages()
	{
		if (!m_oPanelAdvantages)
		{
			return;
		}
		PanelAdvantages component = m_oPanelAdvantages.GetComponent<PanelAdvantages>();
		if ((bool)component)
		{
			component.OnEnter();
		}
		if ((bool)m_oKartPreview && (bool)m_oKartPreview.collider)
		{
			m_oKartPreview.collider.enabled = false;
		}
		if (!m_oScrollPanel || !m_oButtonItemTemplate || !m_oGrid)
		{
			return;
		}
		bool flag = true;
		Transform oBtn = null;
		GameSaveManager instance = Singleton<GameSaveManager>.Instance;
		foreach (AdvantageData oAdvantages in m_oAdvantagesList)
		{
			E_UnlockableItemSate advantageState = instance.GetAdvantageState(oAdvantages.AdvantageType);
			if (advantageState != 0 && IsAdvantageAvailable(oAdvantages))
			{
				BtnItem btnItem = AddItem();
				btnItem.m_oAtlas = m_oAdvAtlas;
				btnItem.Init(base.gameObject, m_oScrollPanel, oAdvantages.spriteName, ERarity.Base, oAdvantages.Price, oAdvantages, advantageState, flag);
				if (flag)
				{
					flag = false;
					oBtn = btnItem.gameObject.transform;
					UpdateTextPanel(oAdvantages, advantageState);
				}
			}
		}
		foreach (AdvantageData oAdvantages2 in m_oAdvantagesList)
		{
			E_UnlockableItemSate advantageState2 = instance.GetAdvantageState(oAdvantages2.AdvantageType);
			if (advantageState2 == E_UnlockableItemSate.Hidden && IsAdvantageAvailable(oAdvantages2))
			{
				BtnItem btnItem2 = AddItem();
				btnItem2.m_oAtlas = m_oAdvAtlas;
				btnItem2.Init(base.gameObject, m_oScrollPanel, oAdvantages2.spriteName, ERarity.Base, oAdvantages2.Price, oAdvantages2, advantageState2, flag);
				if (flag)
				{
					flag = false;
					oBtn = btnItem2.gameObject.transform;
					UpdateTextPanel(oAdvantages2, advantageState2);
				}
			}
		}
		UpdatePanel(oBtn);
	}

	public void InitCoins()
	{
		if (!m_oScrollPanel || !m_oButtonItemTemplate)
		{
			return;
		}
		bool flag = true;
		Transform oBtn = null;
		foreach (InAppCarac oCoinsCarac in m_oCoinsCaracList)
		{
			BtnItem btnItem = AddItem();
			btnItem.Init(base.gameObject, m_oScrollPanel, oCoinsCarac.spriteName, ERarity.Base, 0, oCoinsCarac, E_UnlockableItemSate.Locked, flag);
			if (flag)
			{
				flag = false;
				oBtn = btnItem.gameObject.transform;
				UpdateInAppPanel(oCoinsCarac);
			}
		}
		UpdatePanel(oBtn);
	}

	public void OnClickItem(UnityEngine.Object oData)
	{
		E_UnlockableItemSate e_UnlockableItemSate = E_UnlockableItemSate.Hidden;
		GameSaveManager instance = Singleton<GameSaveManager>.Instance;
		switch (m_iCurrentTab)
		{
		case 0:
			Singleton<GameConfigurator>.Instance.PlayerConfig.m_eCharacter = ((CharacterCarac)oData).Owner;
			if (Singleton<GameConfigurator>.Instance.PlayerConfig.m_oHat.Owner != ECharacter.NONE && Singleton<GameConfigurator>.Instance.PlayerConfig.m_oHat.Owner != Singleton<GameConfigurator>.Instance.PlayerConfig.m_eCharacter)
			{
				foreach (BonusCustom oHat in m_oHatList)
				{
					if (oHat.Owner == Singleton<GameConfigurator>.Instance.PlayerConfig.m_eCharacter)
					{
						Singleton<GameConfigurator>.Instance.PlayerConfig.m_oHat = oHat;
						break;
					}
				}
			}
			e_UnlockableItemSate = instance.GetCharacterState(((CharacterCarac)oData).Owner);
			if (e_UnlockableItemSate == E_UnlockableItemSate.NewUnlocked || e_UnlockableItemSate == E_UnlockableItemSate.Unlocked)
			{
				m_eLastValidCharacter = ((CharacterCarac)oData).Owner;
				E_UnlockableItemSate hatState = instance.GetHatState(Singleton<GameConfigurator>.Instance.PlayerConfig.m_oHat.name);
				if (hatState == E_UnlockableItemSate.NewUnlocked || hatState == E_UnlockableItemSate.Unlocked)
				{
					m_oLastValidHat = Singleton<GameConfigurator>.Instance.PlayerConfig.m_oHat;
				}
			}
			if (e_UnlockableItemSate == E_UnlockableItemSate.NewLocked)
			{
				e_UnlockableItemSate = E_UnlockableItemSate.Locked;
				instance.SetCharacterState(((CharacterCarac)oData).Owner, E_UnlockableItemSate.Locked, true);
			}
			if (e_UnlockableItemSate == E_UnlockableItemSate.NewUnlocked)
			{
				e_UnlockableItemSate = E_UnlockableItemSate.Unlocked;
				instance.SetCharacterState(((CharacterCarac)oData).Owner, E_UnlockableItemSate.Unlocked, true);
			}
			break;
		case 1:
			Singleton<GameConfigurator>.Instance.PlayerConfig.m_eKart = ((KartCarac)oData).Owner;
			if (Singleton<GameConfigurator>.Instance.PlayerConfig.m_oKartCustom.Character != ECharacter.NONE && Singleton<GameConfigurator>.Instance.PlayerConfig.m_oKartCustom.Character != Singleton<GameConfigurator>.Instance.PlayerConfig.m_eKart)
			{
				foreach (KartCustom oKartCustom in m_oKartCustomList)
				{
					if (oKartCustom.Character == Singleton<GameConfigurator>.Instance.PlayerConfig.m_eKart)
					{
						Singleton<GameConfigurator>.Instance.PlayerConfig.m_oKartCustom = oKartCustom;
						break;
					}
				}
			}
			e_UnlockableItemSate = instance.GetKartState(((KartCarac)oData).Owner);
			if (e_UnlockableItemSate == E_UnlockableItemSate.NewLocked)
			{
				e_UnlockableItemSate = E_UnlockableItemSate.Locked;
				instance.SetKartState(((KartCarac)oData).Owner, E_UnlockableItemSate.Locked, true);
			}
			if (e_UnlockableItemSate == E_UnlockableItemSate.NewUnlocked)
			{
				e_UnlockableItemSate = E_UnlockableItemSate.Unlocked;
				instance.SetKartState(((KartCarac)oData).Owner, E_UnlockableItemSate.Unlocked, true);
			}
			if (e_UnlockableItemSate == E_UnlockableItemSate.Unlocked)
			{
				m_eLastValidKart = ((KartCarac)oData).Owner;
				E_UnlockableItemSate customState = instance.GetCustomState(Singleton<GameConfigurator>.Instance.PlayerConfig.m_oKartCustom.name);
				if (customState == E_UnlockableItemSate.NewUnlocked || customState == E_UnlockableItemSate.Unlocked)
				{
					m_oLastValidKartCustom = Singleton<GameConfigurator>.Instance.PlayerConfig.m_oKartCustom;
				}
			}
			break;
		case 2:
			Singleton<GameConfigurator>.Instance.PlayerConfig.m_oHat = (BonusCustom)oData;
			e_UnlockableItemSate = instance.GetHatState(((BonusCustom)oData).name);
			if (e_UnlockableItemSate == E_UnlockableItemSate.NewLocked)
			{
				e_UnlockableItemSate = E_UnlockableItemSate.Locked;
				instance.SetHatState(((BonusCustom)oData).name, E_UnlockableItemSate.Locked, true);
			}
			if (e_UnlockableItemSate == E_UnlockableItemSate.NewUnlocked)
			{
				e_UnlockableItemSate = E_UnlockableItemSate.Unlocked;
				instance.SetHatState(((BonusCustom)oData).name, E_UnlockableItemSate.Unlocked, true);
			}
			if (e_UnlockableItemSate == E_UnlockableItemSate.Unlocked)
			{
				m_oLastValidHat = (BonusCustom)oData;
			}
			if ((bool)HatSound)
			{
				HatSound.Play();
			}
			break;
		case 3:
			Singleton<GameConfigurator>.Instance.PlayerConfig.m_oKartCustom = (KartCustom)oData;
			e_UnlockableItemSate = instance.GetCustomState(((KartCustom)oData).name);
			if (e_UnlockableItemSate == E_UnlockableItemSate.NewLocked)
			{
				e_UnlockableItemSate = E_UnlockableItemSate.Locked;
				instance.SetCustomState(((KartCustom)oData).name, E_UnlockableItemSate.Locked, true);
			}
			if (e_UnlockableItemSate == E_UnlockableItemSate.NewUnlocked)
			{
				e_UnlockableItemSate = E_UnlockableItemSate.Unlocked;
				instance.SetCustomState(((KartCustom)oData).name, E_UnlockableItemSate.Unlocked, true);
			}
			if ((bool)CustomSound)
			{
				CustomSound.Play();
			}
			if (e_UnlockableItemSate == E_UnlockableItemSate.Unlocked)
			{
				m_oLastValidKartCustom = (KartCustom)oData;
			}
			break;
		case 4:
			e_UnlockableItemSate = instance.GetAdvantageState(((AdvantageData)oData).AdvantageType);
			if (e_UnlockableItemSate == E_UnlockableItemSate.NewLocked)
			{
				e_UnlockableItemSate = E_UnlockableItemSate.Locked;
				instance.SetAdvantageState(((AdvantageData)oData).AdvantageType, E_UnlockableItemSate.Locked, true);
			}
			if (e_UnlockableItemSate == E_UnlockableItemSate.NewUnlocked)
			{
				e_UnlockableItemSate = E_UnlockableItemSate.Unlocked;
				instance.SetAdvantageState(((AdvantageData)oData).AdvantageType, E_UnlockableItemSate.Unlocked, true);
			}
			if ((bool)AdvantageSound)
			{
				AdvantageSound.Play();
			}
			break;
		case 5:
			e_UnlockableItemSate = E_UnlockableItemSate.Locked;
			if ((bool)CoinsSound)
			{
				CoinsSound.Play();
			}
			break;
		}
		RefreshItem((IconCarac)oData, e_UnlockableItemSate);
		UpdatePreviewKart();
		if (m_iCurrentTab == 5)
		{
			UpdateInAppPanel((IconCarac)oData);
		}
		else
		{
			UpdateTextPanel((IconCarac)oData, e_UnlockableItemSate);
		}
	}

	private void UpdatePanel(Transform oBtn)
	{
		m_oGrid.gameObject.SendMessage("Reposition");
		if (!oBtn)
		{
			return;
		}
		UIDraggablePanel component = m_oScrollPanel.GetComponent<UIDraggablePanel>();
		if ((bool)component)
		{
			UIPanel component2 = component.GetComponent<UIPanel>();
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(component.transform, oBtn);
			Vector3 relative = new Vector3(component2.clipRange.x - (bounds.max.x + bounds.min.x) * 0.5f, 0f, 0f);
			SpringPanel component3 = component2.gameObject.GetComponent<SpringPanel>();
			if ((bool)component3)
			{
				component3.enabled = false;
			}
			component.MoveRelative(relative);
		}
	}

	private void UpdateTextPanel(IconCarac oCarac, E_UnlockableItemSate eState)
	{
		BroadcastMessage("OnUpdatePanel", null, SendMessageOptions.DontRequireReceiver);
		if (!oCarac || !m_oItemTitle || !m_oItemDescription)
		{
			return;
		}
		m_oItemTitle.text = Localization.instance.Get(oCarac.m_TitleTextId);
		if (m_oItemTitle.text.Length == 0)
		{
			m_oItemTitle.text = oCarac.name;
		}
		m_oItemDescription.text = Localization.instance.Get(oCarac.m_InfoTextId);
		if ((bool)m_oBuyItemButton)
		{
			if (eState == E_UnlockableItemSate.Locked || eState == E_UnlockableItemSate.NewLocked)
			{
				m_oBuyItemButton.SetActive(true);
			}
			else
			{
				m_oBuyItemButton.SetActive(false);
			}
			m_pIconSelected = oCarac;
		}
	}

	private void UpdateInAppPanel(IconCarac oCarac)
	{
		BroadcastMessage("OnUpdatePanel", null, SendMessageOptions.DontRequireReceiver);
		if ((bool)oCarac && (bool)m_oItemTitle && (bool)m_oItemDescription && (bool)m_pEntryPoint)
		{
			InAppProductData inAppData = m_pEntryPoint.GetInAppData(((InAppCarac)oCarac).ProdutId);
			if (inAppData != null)
			{
				m_oInAppTitle.text = Localization.instance.Get(oCarac.m_TitleTextId);
				m_oInAppDescription.text = string.Format("{0:# ### ### ###}", ((InAppCarac)oCarac).CoinsEarn);
				m_oInAppPrice.text = inAppData.Price;
				m_oBuyItemButton.SetActive(false);
				m_pIconSelected = oCarac;
			}
		}
	}

	private bool IsNextButtonValid(out string sItemTypeInvalidTextId)
	{
		GameSaveManager instance = Singleton<GameSaveManager>.Instance;
		sItemTypeInvalidTextId = string.Empty;
		E_UnlockableItemSate characterState = instance.GetCharacterState(Singleton<GameConfigurator>.Instance.PlayerConfig.m_eCharacter);
		if (characterState != E_UnlockableItemSate.NewUnlocked && characterState != E_UnlockableItemSate.Unlocked)
		{
			return false;
		}
		sItemTypeInvalidTextId = "MENU_SHOP_OBJECT_KART";
		characterState = instance.GetKartState(Singleton<GameConfigurator>.Instance.PlayerConfig.m_eKart);
		if (characterState != E_UnlockableItemSate.NewUnlocked && characterState != E_UnlockableItemSate.Unlocked)
		{
			return false;
		}
		sItemTypeInvalidTextId = "MENU_SHOP_OBJECT_HAT";
		characterState = instance.GetHatState(Singleton<GameConfigurator>.Instance.PlayerConfig.m_oHat.name);
		if (characterState != E_UnlockableItemSate.NewUnlocked && characterState != E_UnlockableItemSate.Unlocked)
		{
			return false;
		}
		sItemTypeInvalidTextId = "MENU_SHOP_OBJECT_CUSTO";
		characterState = instance.GetCustomState(Singleton<GameConfigurator>.Instance.PlayerConfig.m_oKartCustom.name);
		if (characterState != E_UnlockableItemSate.NewUnlocked && characterState != E_UnlockableItemSate.Unlocked)
		{
			return false;
		}
		return true;
	}

	private void UpdatePreviewKart()
	{
		if ((bool)m_oKartPreview)
		{
			KartPreviewBuilder component = m_oKartPreview.GetComponent<KartPreviewBuilder>();
			component.Build(Singleton<GameConfigurator>.Instance.PlayerConfig.m_eCharacter, Singleton<GameConfigurator>.Instance.PlayerConfig.m_eKart, Singleton<GameConfigurator>.Instance.PlayerConfig.m_oKartCustom.name, Singleton<GameConfigurator>.Instance.PlayerConfig.m_oHat.name);
		}
	}

	public void OnPurchase()
	{
		int num = 0;
		switch (m_iCurrentTab)
		{
		case 0:
			return;
		case 1:
			num = Singleton<GameConfigurator>.Instance.PriceConfig.GetKartPrice();
			break;
		case 2:
			num = Singleton<GameConfigurator>.Instance.PriceConfig.GetHatPrice(((BonusCustom)m_pIconSelected).Rarity, ((BonusCustom)m_pIconSelected).Owner == ECharacter.NONE);
			break;
		case 3:
			num = Singleton<GameConfigurator>.Instance.PriceConfig.GetCustoPrice(((KartCustom)m_pIconSelected).Rarity);
			break;
		case 4:
			num = ((AdvantageData)m_pIconSelected).Price;
			break;
		case 5:
			OnPopupShow();
			Singleton<InAppManager>.Instance.PurchaseProduct(((InAppCarac)m_pIconSelected).ProdutId);
			return;
		}
		m_pMenuEntryPoint.ShowPurchasePopup(string.Format(Localization.instance.Get("MENU_POPUP_BUY_ITEM_CONFIRMATION"), Localization.instance.Get(m_pIconSelected.m_TitleTextId), num), num, PurchaseItem, OpenBuyCoinsTab, true);
	}

	public void PurchaseItem(object oParam)
	{
		switch (m_iCurrentTab)
		{
		case 0:
			return;
		case 1:
			Singleton<GameSaveManager>.Instance.SetKartState(((KartCarac)m_pIconSelected).Owner, E_UnlockableItemSate.Unlocked, true);
			m_oKartList.Sort(KartCarac.Compare);
			break;
		case 2:
			Singleton<GameSaveManager>.Instance.SetHatState(((BonusCustom)m_pIconSelected).name, E_UnlockableItemSate.Unlocked, true);
			m_oHatList.Sort(BonusCustom.Compare);
			break;
		case 3:
			Singleton<GameSaveManager>.Instance.SetCustomState(((KartCustom)m_pIconSelected).name, E_UnlockableItemSate.Unlocked, true);
			m_oKartCustomList.Sort(KartCustom.Compare);
			break;
		case 4:
			Singleton<GameSaveManager>.Instance.EarnAdvantage(((AdvantageData)m_pIconSelected).AdvantageType, 1, true);
			UpdateTextPanel(m_pIconSelected, E_UnlockableItemSate.Locked);
			return;
		case 5:
			return;
		}
		int iCurrentTab = m_iCurrentTab;
		m_iCurrentTab = -1;
		OnSelectTab(iCurrentTab);
	}

	private bool SpendCoins(int iPrice)
	{
		bool flag = false;
		flag = ((Singleton<GameSaveManager>.Instance.GetCoins() >= iPrice) ? true : false);
		if (flag)
		{
			Singleton<GameSaveManager>.Instance.SpendCoins(iPrice, true);
		}
		return flag;
	}

	private void PurchaseSucceed(string pProductId)
	{
		m_pMenuEntryPoint.ShowPopup(EPopUps.POPUP_DIALOG, "MENU_POPUP_BUY_OK", true);
		foreach (InAppCarac oCoinsCarac in m_oCoinsCaracList)
		{
			if (oCoinsCarac.ProdutId == pProductId)
			{
				Singleton<GameSaveManager>.Instance.EarnCoins(oCoinsCarac.CoinsEarn, false, true);
				break;
			}
		}
	}

	private void PurchaseFailed(string pError)
	{
		m_pMenuEntryPoint.ShowPopup(EPopUps.POPUP_DIALOG, "MENU_POPUP_BUY_FAILED", true);
	}

	private void PurchaseCancelled(string pError)
	{
		m_pMenuEntryPoint.ShowPopup(EPopUps.POPUP_DIALOG, "MENU_POPUP_BUY_CANCELLED", true);
	}

	private void RefreshItem(UnityEngine.Object oItemData, E_UnlockableItemSate eState)
	{
		BtnItem[] componentsInChildren = m_oGrid.GetComponentsInChildren<BtnItem>();
		foreach (BtnItem btnItem in componentsInChildren)
		{
			if (btnItem.IsDataEqual(oItemData))
			{
				btnItem.RefreshState(eState);
				break;
			}
		}
	}

	private void OpenBuyCoinsTab(object oActiveNextButton)
	{
		bool flag = (bool)oActiveNextButton;
		if ((bool)m_oButtonCoins && m_oButtonCoins.activeSelf)
		{
			m_oButtonCoins.SendMessage("OnClick");
		}
		else
		{
			ButtonPersonage.SendMessage("OnClick");
		}
		if ((bool)m_oButtonNext)
		{
			m_oButtonNext.SetActive(flag);
		}
	}

	public EAdvantageRestrictions GetAdvantageRestrictions()
	{
		return m_eAdvantageRestrictions;
	}

	public void SetPanelTextAlpha(float fAlpha)
	{
		if ((bool)m_oPanelText)
		{
			m_oPanelText.alpha = Tricks.ComputeInertia(m_oPanelText.alpha, fAlpha, 0.15f, Time.deltaTime);
		}
	}

	public void OnDisconnectedFromServer()
	{
		if (Network.isClient)
		{
			RevertToLastValidState();
			ActSwapMenu(EMenus.MENU_MULTI_JOIN);
		}
	}

	public void SortLists()
	{
		m_oCharacterList.Sort(CharacterCarac.Compare);
		m_oHatList.Sort(BonusCustom.Compare);
		m_oKartList.Sort(KartCarac.Compare);
		m_oKartCustomList.Sort(KartCustom.Compare);
		m_oAdvantagesList.Sort(AdvantageData.Compare);
	}
}

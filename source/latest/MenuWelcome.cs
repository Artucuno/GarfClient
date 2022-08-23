using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Net;

public class MenuWelcome : AbstractMenu
{
	private List<InAppCarac> pCoinsCarac = new List<InAppCarac>();

	public UISprite ChallengeType;

	public UITexturePattern ChallengeState;

	private bool m_bIntroMusic;

	public GameObject[] Stars = new GameObject[5];

	public float MenuMusicFadeInDuration = 0.2f;

	public float MenuMusicDelay = -0.2f;

	public GameObject ButtonQuit;

	public GameObject ButtonSharing;

	public GameObject ButtonMoreApps;

	public GameObject HighlightTutorialPanel;

	private EMenus m_eOnHighlightTutorialExit = EMenus.MENU_WELCOME;

	string garfver = "1.0";

	public override void Awake()
	{
		base.Awake();
		Object[] array = Resources.LoadAll("InApp", typeof(InAppCarac));
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] is InAppCarac)
			{
				InAppCarac item = (InAppCarac)array[i];
				pCoinsCarac.Add(item);
			}
		}
		m_bIntroMusic = false;
		if ((bool)ButtonQuit)
		{
			if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer)
			{
				ButtonQuit.SetActive(true);
			}
			else
			{
				ButtonQuit.SetActive(false);
			}
		}
		MenuMusicDelay += m_pMenuEntryPoint.IntroMusic.clip.length - (float)(m_pMenuEntryPoint.IntroMusic.timeSamples / m_pMenuEntryPoint.IntroMusic.clip.frequency);
	}

	public new void Start()
	{
		EntryPoint component = GameObject.Find("EntryPoint").GetComponent<EntryPoint>();
		if (component.DisplayHighlightTutorial)
		{
			HighlightTutorial(null);
		}
		else if (component.AskForRating)
		{
			OnAskForRating();
		}
		else if (component.ShowInterstitial)
		{
			ShowInterstitial();
		}
	}

	public override void OnEnter()
	{
		m_bIntroMusic = false;
		base.OnEnter();
		if (Singleton<ChallengeManager>.Instance.AlreadyPlayed)
		{
			ChallengeState.gameObject.SetActive(true);
			ChallengeState.ChangeTexture((!Singleton<ChallengeManager>.Instance.Success) ? 1 : 0);
		}
		else
		{
			ChallengeState.gameObject.SetActive(false);
		}
		Singleton<ChallengeManager>.Instance.DeActivate();
		if (ChallengeType != null)
		{
			if (Singleton<ChallengeManager>.Instance.IsMonday)
			{
				ChallengeType.spriteName = "icon_chalenge_monday";
			}
			else
			{
				ChallengeType.spriteName = "icon_chalenge_day";
			}
		}
		int num = Singleton<GameConfigurator>.Instance.PlayerConfig.NbStars - 1;
		for (int i = 0; i < Stars.Length; i++)
		{
			if ((bool)Stars[i])
			{
				Stars[i].SetActive(i <= num);
			}
		}
		ButtonSharing.SetActive(false);
		ButtonMoreApps.SetActive(false);
		//m_pMenuEntryPoint.ShowPopup(EPopUps.POPUP_DIALOG, "What the fuck did you just fucking say about me, you little bitch? I'll have you know I graduated top of my class in the Navy Seals, and I've been involved in numerous secret raids on Al-Quaeda, and I have over 300 confirmed kills. I am trained in gorilla warfare and I'm the top sniper in the entire US armed forces. You are nothing to me but just another target. I will wipe you the fuck out with precision the likes of which has never been seen before on this Earth, mark my fucking words. You think you can get away with saying that shit to me over the Internet? Think again, fucker. As we speak I am contacting my secret network of spies across the USA and your IP is being traced right now so you better prepare for the storm, maggot. The storm that wipes out the pathetic little thing you call your life. You're fucking dead, kid. I can be anywhere, anytime, and I can kill you in over seven hundred ways, and that's just with my bare hands. Not only am I extensively trained in unarmed combat, but I have access to the entire arsenal of the United States Marine Corps and I will use it to its full extent to wipe your miserable ass off the face of the continent, you little shit. If only you could have known what unholy retribution your little 'clever' comment was about to bring down upon you, maybe you would have held your fucking tongue. But you couldn't, you didn't, and now you're paying the price, you goddamn idiot. I will shit fury all over you and you will drown in it. You're fucking dead, kiddo.", true);
		//StreamWriter sw = new StreamWriter("C:\\Test.txt");
		//sw.Write('amogus');
		try
		{
			const string url = "http://garfclient.artucuno.dev/version";
			var request = WebRequest.Create(url);
			request.Method = "GET";

			using var webResponse = request.GetResponse();
			using var webStream = webResponse.GetResponseStream();

			using var reader = new StreamReader(webStream);
			var data = reader.ReadToEnd();
			string rdata = data.Trim();

			const string surl = "http://garfclient.artucuno.dev/updatedesc";
			var srequest = WebRequest.Create(surl);
			request.Method = "GET";

			using var swebResponse = srequest.GetResponse();
			using var swebStream = swebResponse.GetResponseStream();

			using var sreader = new StreamReader(swebStream);
			var sdata = sreader.ReadToEnd();
			string srdata = sdata.Trim();

			if (rdata != garfver)
			{
				string e = System.String.Format("There is an update! v{0} - {1}", rdata, srdata);
				m_pMenuEntryPoint.ShowPopup(EPopUps.POPUP_DIALOG, e, true);
			}
			else
			{
				m_pMenuEntryPoint.ShowPopup(EPopUps.POPUP_DIALOG, "Up to date!", true);
			}

		}
		catch (System.Exception e)
		{
			m_pMenuEntryPoint.ShowPopup(EPopUps.POPUP_DIALOG, e.Message, true);
		}
	}

	public override void OnExit()
	{
		base.OnExit();
		m_bIntroMusic = false;
		if (!m_pMenuEntryPoint.MenuMusic.isPlaying)
		{
			m_pMenuEntryPoint.StartFadingInMenuMusic(MenuMusicFadeInDuration);
		}
	}

	public override void Update()
	{
		if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
		{
			OnQuit();
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Singleton<GameSaveManager>.Instance.EarnCoins(10000, false, true);
			m_pMenuEntryPoint.ShowPopup(EPopUps.POPUP_DIALOG, "Added 10000 Garf Coins", true);
		}
		if (!m_pMenuEntryPoint.MenuMusic.isPlaying)
		{
			MenuMusicDelay -= Time.deltaTime;
			if (MenuMusicDelay <= 0f)
			{
				m_pMenuEntryPoint.StartFadingInMenuMusic(MenuMusicFadeInDuration);
			}
		}
		if ((bool)HighlightTutorialPanel && HighlightTutorialPanel.activeSelf && Input.anyKeyDown)
		{
			OnHighlightTutorialExit();
		}
		if (ButtonSharing.activeSelf && !Singleton<GameSaveManager>.Instance.GetAskSharing())
		{
			ButtonSharing.SetActive(false);
		}
	}

	public void OnShop()
	{
		m_pMenuEntryPoint.SetState(EMenus.MENU_SELECT_KART, 1);
	}

	public override void PlayMusic()
	{
		if (!m_pMenuEntryPoint.MenuMusic.isPlaying)
		{
			m_pMenuEntryPoint.PlayIntroMusic();
		}
		m_bIntroMusic = true;
	}

	public void OnQuit()
	{
		if (Application.platform != RuntimePlatform.WindowsWebPlayer && Application.platform != RuntimePlatform.OSXWebPlayer)
		{
			Application.Quit();
		}
	}

	public void OnPlay(object param = null)
	{
		if (Singleton<GameSaveManager>.Instance.GetFirstTime())
		{
			Popup2Choices popup2Choices = (Popup2Choices)m_pMenuEntryPoint.ShowPopup(EPopUps.POPUP_DIALOG_2CHOICES, false);
			if ((bool)popup2Choices)
			{
				popup2Choices.Show("MENU_POPUP_FIRSTTIME", HighlightTutorial, LaunchIGTutorial, EMenus.MENU_SOLO);
			}
		}
		else
		{
			ActSwapMenu(EMenus.MENU_SOLO);
		}
	}

	public void OnChallenge(object param = null)
	{
		if (Singleton<GameSaveManager>.Instance.GetFirstTime())
		{
			Popup2Choices popup2Choices = (Popup2Choices)m_pMenuEntryPoint.ShowPopup(EPopUps.POPUP_DIALOG_2CHOICES, false);
			if ((bool)popup2Choices)
			{
				popup2Choices.Show("MENU_POPUP_FIRSTTIME", HighlightTutorial, LaunchIGTutorial, EMenus.MENU_CHALLENGE);
			}
		}
		else
		{
			ActSwapMenu(EMenus.MENU_CHALLENGE);
		}
	}

	public void HighlightTutorial(object param = null)
	{
		if (!HighlightTutorialPanel)
		{
			if (param != null)
			{
				ActSwapMenu((EMenus)(int)param);
			}
			return;
		}
		HighlightTutorialPanel.SetActive(true);
		GameObject.Find("EntryPoint").GetComponent<EntryPoint>().DisplayHighlightTutorial = false;
		if (param != null)
		{
			m_eOnHighlightTutorialExit = (EMenus)(int)param;
		}
		else
		{
			m_eOnHighlightTutorialExit = EMenus.MENU_WELCOME;
		}
	}

	public void OnHighlightTutorialExit()
	{
		HighlightTutorialPanel.SetActive(false);
		if (m_eOnHighlightTutorialExit != EMenus.MENU_WELCOME)
		{
			m_eOnHighlightTutorialExit = EMenus.MENU_WELCOME;
		}
	}

	public void LaunchIGTutorial(object param = null)
	{
		Singleton<GameSaveManager>.Instance.SetShowTutorial(false, true);
		GameObject.Find("EntryPoint").GetComponent<EntryPoint>().DisplayHighlightTutorial = true;
		Singleton<GameConfigurator>.Instance.GameModeType = E_GameModeType.TUTORIAL;
		ChampionShipData data = (ChampionShipData)Resources.Load("ChampionShip/Champion_Ship_1", typeof(ChampionShipData));
		Singleton<GameConfigurator>.Instance.SetChampionshipData(data, false);
		Singleton<GameConfigurator>.Instance.StartScene = Singleton<GameConfigurator>.Instance.ChampionShipData.Tracks[0];
		Singleton<GameConfigurator>.Instance.CurrentTrackIndex = 0;
		LoadingManager.LoadLevel(Singleton<GameConfigurator>.Instance.StartScene);
	}

	public void OnAskForRating()
	{
		Popup3Choices popup3Choices = (Popup3Choices)m_pMenuEntryPoint.ShowPopup(EPopUps.POPUP_DIALOG_3CHOICES, false);
		if ((bool)popup3Choices)
		{
			popup3Choices.Show("MENU_POPUP_RATING", OnNeverRate, OnRate, OnRemindRatingLater, null, "MENU_POPUP_NEVER_RATE", "MENU_POPUP_RATE_LATER", "MENU_POPUP_RATE");
		}
		EntryPoint component = GameObject.Find("EntryPoint").GetComponent<EntryPoint>();
		component.AskForRating = false;
	}

	public void OnNeverRate(object param)
	{
		Singleton<GameSaveManager>.Instance.SetAskRating(-1, true);
	}

	public void OnRemindRatingLater(object param)
	{
	}

	public void OnRate(object param)
	{
		Singleton<GameSaveManager>.Instance.SetAskRating(-1, true);
	}

	public void OnAskForSharing()
	{
		Popup2Choices popup2Choices = (Popup2Choices)m_pMenuEntryPoint.ShowPopup(EPopUps.POPUP_FACEBOOK, false);
		if ((bool)popup2Choices)
		{
			popup2Choices.Show(null, NoShare, YesShare, null, null, null);
		}
		EntryPoint component = GameObject.Find("EntryPoint").GetComponent<EntryPoint>();
		component.AskForSharing = false;
	}

	public void NoShare(object param)
	{
	}

	public void YesShare(object param)
	{
		EntryPoint component = GameObject.Find("EntryPoint").GetComponent<EntryPoint>();
		component.DoSharing();
	}

	public void ShowInterstitial()
	{
		if (ASE_Tools.Available)
		{
			ASE_ChartBoost.ShowInterstitial("Default");
		}
	}

	public void OnMoreApps()
	{
		if (ASE_Tools.Available)
		{
			ASE_Facebook.Logout();
			ASE_ChartBoost.ShowMoreApps();
		}
	}
}

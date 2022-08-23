using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuEntryPoint : MonoBehaviour
{
	private class PurchasePopupData
	{
		public int m_iPrice;

		public Callback m_oCbSucceed;

		public Callback m_oCbNoMoney;

		public object m_oParamCb;
	}

	public delegate void Callback(object param);

	private List<ESounds> soundToPlay = new List<ESounds>();

	private EMenus m_eState;

	private EMenus m_ePreviousState;

	private EPopUps m_ePopupState;

	public Camera m_oCurrentCamera;

	public Camera m_oDefaultCamera;

	[SerializeField]
	[HideInInspector]
	public List<AbstractMenu> MenuRefList = new List<AbstractMenu>();

	public List<AbstractPopup> PopupRefList = new List<AbstractPopup>();

	public AudioSource IntroMusic;

	public AudioSource MenuMusic;

	public AudioSource HoverSound;

	public AudioSource ValidSound;

	public AudioSource BackSound;

	public AudioSource GoSound;

	public AudioSource SwitchPageSound;

	public AudioSource NotEnoughCoinsSound;

	public AudioSource BuyCoinsSound;

	public AudioSource PopupSound;

	private float m_fMenuMusicFadeInDuration = 0.2f;

	private bool m_bMenuMusicFadingIn;

	private float m_fMenuMusicFadeIn;

	private GameObject mainMenuBackGround;

	private GameObject menuBackGroundUFO;

	private GameObject menuBackGroundPIE;

	private List<string> menuBackGroundAnims = new List<string>();

	private List<GameObject> menuBackGroundChars = new List<GameObject>();

	private int lastAnimPlayed = -1;

	private int lastCharChosen = -1;

	private void Awake()
	{
		mainMenuBackGround = GameObject.Find("GARFIELD");
		menuBackGroundUFO = GameObject.Find("UFO");
		menuBackGroundPIE = GameObject.Find("PIE");
		menuBackGroundChars.Add(GameObject.Find("ARLENE"));
		menuBackGroundChars.Add(GameObject.Find("HARRY"));
		menuBackGroundChars.Add(GameObject.Find("JON"));
		menuBackGroundChars.Add(GameObject.Find("LIZE"));
		menuBackGroundChars.Add(GameObject.Find("NERMAL"));
		menuBackGroundChars.Add(GameObject.Find("ODIE"));
		menuBackGroundChars.Add(GameObject.Find("SQUEAK"));
		menuBackGroundAnims.Add("Menu_Kart_Anim_CATCHED");
		menuBackGroundAnims.Add("Menu_Kart_Anim_JUMP");
		menuBackGroundAnims.Add("Menu_Kart_Anim_OVERTAKE");
		menuBackGroundAnims.Add("Menu_Kart_Anim_SLIDE");
		if (mainMenuBackGround != null)
		{
			mainMenuBackGround.GetComponent<Animation>().Play();
		}
		else
		{
			Debug.Log("Menu Background Anim problem");
		}
		menuBackGroundUFO.SetActive(false);
		menuBackGroundPIE.SetActive(false);
		foreach (GameObject menuBackGroundChar in menuBackGroundChars)
		{
			menuBackGroundChar.SetActive(false);
		}
	}

	public void SelectRandomMenuAnim()
	{
		if (lastCharChosen >= 0 && lastAnimPlayed >= 0)
		{
			if (menuBackGroundChars[lastCharChosen] != null)
			{
				stopAnim(menuBackGroundChars[lastCharChosen]);
			}
			else
			{
				Debug.Log("Menu Background Anim problem");
			}
		}
		if (Random.value >= 0.33f)
		{
			int num = -1;
			do
			{
				num = Random.Range(0, menuBackGroundChars.Count);
			}
			while (num == lastCharChosen && menuBackGroundChars.Count > 1);
			lastCharChosen = num;
			if (menuBackGroundChars[lastCharChosen] != null)
			{
				startAnim(menuBackGroundChars[lastCharChosen]);
			}
			else
			{
				Debug.Log("Menu Background Anim problem");
			}
		}
		else
		{
			lastCharChosen = -1;
		}
	}

	public void startAnim(GameObject character)
	{
		character.SetActive(true);
		Animation component = character.GetComponent<Animation>();
		int num = -1;
		do
		{
			num = Random.Range(0, menuBackGroundAnims.Count);
		}
		while (num == lastAnimPlayed && menuBackGroundAnims.Count > 1);
		lastAnimPlayed = num;
		component.Play(menuBackGroundAnims[lastAnimPlayed]);
		if (menuBackGroundAnims[lastAnimPlayed].Equals("Menu_Kart_Anim_CATCHED") && menuBackGroundUFO != null)
		{
			menuBackGroundUFO.SetActive(true);
			menuBackGroundUFO.GetComponent<Animation>().Play("Menu_Kart_Anim_UFO");
		}
		if (menuBackGroundAnims[lastAnimPlayed].Equals("Menu_Kart_Anim_SLIDE") && menuBackGroundPIE != null)
		{
			menuBackGroundPIE.SetActive(true);
			menuBackGroundPIE.GetComponent<Animation>().Play("Menu_Kart_Anim_PIE");
		}
	}

	public void stopAnim(GameObject character)
	{
		Animation component = character.GetComponent<Animation>();
		component.Stop(menuBackGroundAnims[lastAnimPlayed]);
		if (menuBackGroundAnims[lastAnimPlayed].Equals("Menu_Kart_Anim_CATCHED") && menuBackGroundUFO != null)
		{
			menuBackGroundUFO.GetComponent<Animation>().Stop("Menu_Kart_Anim_UFO");
			menuBackGroundUFO.SetActive(false);
		}
		if (menuBackGroundAnims[lastAnimPlayed].Equals("Menu_Kart_Anim_SLIDE") && menuBackGroundPIE != null)
		{
			menuBackGroundPIE.GetComponent<Animation>().Stop("Menu_Kart_Anim_UPIE");
			menuBackGroundPIE.SetActive(false);
		}
		character.SetActive(false);
	}

	private void Start()
	{
		Singleton<GameConfigurator>.Instance.ResetChampionShip();
		if (Singleton<ChallengeManager>.Instance.IsActive)
		{
			Singleton<ChallengeManager>.Instance.DeActivate();
		}
		if ((bool)Singleton<GameConfigurator>.Instance.PlayerConfig)
		{
			Singleton<GameConfigurator>.Instance.PlayerConfig.GetAdvantages().Clear();
			Singleton<GameConfigurator>.Instance.PlayerConfig.ResetAdvantages();
		}
		m_oCurrentCamera.enabled = true;
		if (Singleton<RewardManager>.Instance._comicStrips != null)
		{
			SetState(Singleton<RewardManager>.Instance.GetState());
		}
		else
		{
			SetState(Singleton<GameConfigurator>.Instance.MenuToLaunch);
		}
		if (Singleton<GameConfigurator>.Instance.ChampionshipPass != null)
		{
			Singleton<GameConfigurator>.Instance.ChampionshipPass = null;
		}
	}

	private void Update()
	{
		if (soundToPlay.Count > 0 && LoadingManager.loadingFinished)
		{
			switch (soundToPlay[0])
			{
			case ESounds.INTRO_MUSIC:
				if (IntroMusic != null && !IntroMusic.isPlaying)
				{
					if (MenuMusic != null && MenuMusic.isPlaying)
					{
						MenuMusic.Stop();
					}
					IntroMusic.Play();
				}
				break;
			case ESounds.MENU_MUSIC:
				if (MenuMusic != null && !MenuMusic.isPlaying && (!(IntroMusic != null) || !IntroMusic.isPlaying))
				{
					MenuMusic.Play();
				}
				break;
			case ESounds.HOVER_SOUND:
				if ((bool)HoverSound)
				{
					HoverSound.Play();
				}
				break;
			case ESounds.VALID_SOUND:
				if ((bool)ValidSound)
				{
					ValidSound.Play();
				}
				break;
			case ESounds.BACK_SOUND:
				if ((bool)BackSound)
				{
					BackSound.Play();
				}
				break;
			case ESounds.GO_SOUND:
				if ((bool)GoSound)
				{
					GoSound.Play();
				}
				break;
			case ESounds.SWITCH_PAGE_SOUND:
				if ((bool)SwitchPageSound)
				{
					SwitchPageSound.Play();
				}
				break;
			case ESounds.NOT_ENOUGH_COINS_SOUND:
				if ((bool)BuyCoinsSound)
				{
					BuyCoinsSound.Play();
				}
				break;
			case ESounds.BUY_COINS_SOUND:
				if ((bool)NotEnoughCoinsSound)
				{
					NotEnoughCoinsSound.Play();
				}
				break;
			case ESounds.POPUP_SOUND:
				if ((bool)PopupSound)
				{
					PopupSound.Play();
				}
				break;
			default:
				Debug.Log(soundToPlay[0]);
				break;
			}
			soundToPlay.RemoveAt(0);
		}
		if (m_bMenuMusicFadingIn)
		{
			m_fMenuMusicFadeIn += Time.deltaTime;
			if (m_fMenuMusicFadeIn < m_fMenuMusicFadeInDuration)
			{
				MenuMusic.volume = m_fMenuMusicFadeIn / m_fMenuMusicFadeInDuration;
				return;
			}
			MenuMusic.volume = 1f;
			m_bMenuMusicFadingIn = false;
		}
	}

	public void StartFadingInMenuMusic(float FadeInDuration)
	{
		m_bMenuMusicFadingIn = true;
		MenuMusic.volume = 0f;
		MenuMusic.Play();
		m_fMenuMusicFadeInDuration = FadeInDuration;
	}

	public void SetState(EMenus eState)
	{
		MenuRefList[(int)m_eState].OnExit();
		m_ePreviousState = m_eState;
		m_eState = eState;
		MenuRefList[(int)m_eState].OnEnter();
	}

	public void SetStateDelay(EMenus eState, float Delay)
	{
		StartCoroutine(SetStateDelayCoroutine(eState, 0.2f));
	}

	private IEnumerator SetStateDelayCoroutine(EMenus eState, float Delay)
	{
		MenuRefList[(int)m_eState].OnExit();
		m_ePreviousState = m_eState;
		yield return new WaitForSeconds(Delay);
		m_eState = eState;
		MenuRefList[(int)m_eState].OnEnter();
	}

	public void SetState(EMenus eState, int iSubEntryPoint)
	{
		MenuRefList[(int)m_eState].OnExit();
		m_ePreviousState = m_eState;
		m_eState = eState;
		MenuRefList[(int)m_eState].OnEnter(iSubEntryPoint);
	}

	public void SetPreviousState()
	{
		SetState(m_ePreviousState);
	}

	public void SetCamera(Camera oCamera)
	{
		if ((bool)oCamera)
		{
			m_oCurrentCamera.enabled = false;
			m_oCurrentCamera = oCamera;
			m_oCurrentCamera.enabled = true;
		}
		else
		{
			m_oCurrentCamera.enabled = false;
			m_oCurrentCamera = m_oDefaultCamera;
			m_oCurrentCamera.enabled = true;
		}
	}

	private void DoShowPopup(EPopUps Popup, bool bSound)
	{
		MenuRefList[(int)m_eState].OnPopupShow();
		m_ePopupState = Popup;
		if (bSound)
		{
			soundToPlay.Add(ESounds.POPUP_SOUND);
		}
	}

	public void ShowPopup(EPopUps Popup, string _TextId, bool bSound)
	{
		DoShowPopup(Popup, bSound);
		PopupRefList[(int)m_ePopupState].Show(_TextId);
	}

	public AbstractPopup ShowPopup(EPopUps Popup, bool bSound)
	{
		DoShowPopup(Popup, bSound);
		return PopupRefList[(int)m_ePopupState];
	}

	public void QuitPopup()
	{
		MenuRefList[(int)m_eState].OnPopupQuit();
		PopupRefList[(int)m_ePopupState].Hide();
	}

	public void ShowPurchasePopup(string sText, int iPrice, Callback oCbSucceed, Callback oCbNoMoney = null, object oParamCb = null)
	{
		if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer && Singleton<GameSaveManager>.Instance.GetCoins() < iPrice)
		{
			PopupDialog popupDialog = (PopupDialog)ShowPopup(EPopUps.POPUP_DIALOG, false);
			soundToPlay.Add(ESounds.NOT_ENOUGH_COINS_SOUND);
			popupDialog.Show(string.Format(Localization.instance.Get("MENU_POUP_NOT_ENOUGH_MONEY_PC"), iPrice - Singleton<GameSaveManager>.Instance.GetCoins()));
			return;
		}
		Popup2Choices popup2Choices = (Popup2Choices)ShowPopup(EPopUps.POPUP_DIALOG_2CHOICES, false);
		if ((bool)popup2Choices)
		{
			PurchasePopupData purchasePopupData = new PurchasePopupData();
			purchasePopupData.m_oCbSucceed = oCbSucceed;
			purchasePopupData.m_oCbNoMoney = oCbNoMoney;
			purchasePopupData.m_iPrice = iPrice;
			purchasePopupData.m_oParamCb = oParamCb;
			if (Singleton<GameSaveManager>.Instance.GetCoins() >= iPrice)
			{
				Popup2Choices.Callback oCbRight = PurchaseOk;
				popup2Choices.ShowText(sText, null, oCbRight, purchasePopupData);
				soundToPlay.Add(ESounds.BUY_COINS_SOUND);
			}
			else
			{
				soundToPlay.Add(ESounds.NOT_ENOUGH_COINS_SOUND);
				Popup2Choices.Callback oCbRight = PurchaseNoMoney;
				popup2Choices.ShowText(string.Format(Localization.instance.Get("MENU_POUP_NOT_ENOUGH_MONEY"), iPrice - Singleton<GameSaveManager>.Instance.GetCoins()), null, oCbRight, purchasePopupData);
			}
		}
	}

	private void PurchaseOk(object oParam)
	{
		PurchasePopupData purchasePopupData = (PurchasePopupData)oParam;
		Singleton<GameSaveManager>.Instance.SpendCoins(purchasePopupData.m_iPrice, true);
		purchasePopupData.m_oCbSucceed(purchasePopupData.m_oParamCb);
	}

	private void PurchaseNoMoney(object oParam)
	{
		PurchasePopupData purchasePopupData = (PurchasePopupData)oParam;
		if (purchasePopupData.m_oCbNoMoney == null)
		{
			SetState(EMenus.MENU_SELECT_KART, 1);
		}
		else
		{
			purchasePopupData.m_oCbNoMoney(purchasePopupData.m_oParamCb);
		}
	}

	public void PlayIntroMusic()
	{
		int num = soundToPlay.Count - 1;
		if (num < 0 || (num >= 0 && soundToPlay[num] != ESounds.INTRO_MUSIC))
		{
			soundToPlay.Add(ESounds.INTRO_MUSIC);
		}
	}

	public void PlayMenuMusic()
	{
		int num = soundToPlay.Count - 1;
		if (num < 0 || (num >= 0 && soundToPlay[num] != ESounds.MENU_MUSIC))
		{
			soundToPlay.Add(ESounds.MENU_MUSIC);
		}
	}

	public void PlayHoverSound()
	{
		soundToPlay.Add(ESounds.HOVER_SOUND);
	}

	public void PlayValidSound()
	{
		soundToPlay.Add(ESounds.VALID_SOUND);
	}

	public void PlayBackSound()
	{
		soundToPlay.Add(ESounds.BACK_SOUND);
	}

	public void PlayGoSound()
	{
		soundToPlay.Add(ESounds.GO_SOUND);
	}

	public void PlaySwitchPageSound()
	{
		soundToPlay.Add(ESounds.SWITCH_PAGE_SOUND);
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
	public enum eFacebookState
	{
		None,
		Login,
		Logged,
		SharingAsked,
		PublishAsked
	}

	public string StartScene = "MenuRoot";

	private List<InAppProductData> m_pInAppProduct = new List<InAppProductData>();

	public string ChartBoostAppIDIOS = "YOUR_APP_ID";

	public string ChartBoostAppSignatureIOS = "YOUR_APP_SIGNATURE";

	public string ChartBoostAppIDAndroid = "YOUR_APP_ID";

	public string ChartBoostAppSignatureAndroid = "YOUR_APP_SIGNATURE";

	public string ChartBoostAppIDAmazon = "YOUR_APP_ID";

	public string ChartBoostAppSignatureAmazon = "YOUR_APP_SIGNATURE";

	public string FacebookAppID = "YOUR_FACEBOOK_ID";

	private bool m_bDisplayHighlightTutorial;

	private bool m_bAskForRating;

	private bool m_bAskForSharing;

	private bool m_bShowInterstitial;

	private bool m_bSharingOn;

	private eFacebookState m_eFacebookState;

	private string m_sFacebookTitle;

	private string m_sFacebookDescription;

	public bool DisplayHighlightTutorial
	{
		get
		{
			return m_bDisplayHighlightTutorial;
		}
		set
		{
			m_bDisplayHighlightTutorial = value;
		}
	}

	public eFacebookState FacebookState
	{
		get
		{
			return m_eFacebookState;
		}
		set
		{
			m_eFacebookState = value;
		}
	}

	public bool AskForRating
	{
		get
		{
			return false;
		}
		set
		{
			m_bAskForRating = value;
			if (value)
			{
				Singleton<GameSaveManager>.Instance.AddAskRating(true);
			}
		}
	}

	public bool AskForSharing
	{
		get
		{
			return false;
		}
		set
		{
			m_bAskForSharing = value;
		}
	}

	public bool ShowInterstitial
	{
		get
		{
			return false;
		}
		set
		{
			m_bShowInterstitial = value;
		}
	}

	private void Awake()
	{
		if (LogManager.Instance != null)
		{
		}
		if (!Debug.isDebugBuild)
		{
			StartScene = "MenuRoot";
		}
		InAppManager instance = Singleton<InAppManager>.Instance;
		instance.OnProductDataReceived = (Action<List<InAppProductData>>)Delegate.Combine(instance.OnProductDataReceived, new Action<List<InAppProductData>>(InAppProductDataReceived));
		UnityEngine.Object.DontDestroyOnLoad(this);
		ASE_ChartBoost.SetGameObjectName(base.name);
		ASE_Facebook.SetGameObjectName(base.name);
	}

	private void DoDestroy()
	{
		InAppManager instance = Singleton<InAppManager>.Instance;
		instance.OnProductDataReceived = (Action<List<InAppProductData>>)Delegate.Remove(instance.OnProductDataReceived, new Action<List<InAppProductData>>(InAppProductDataReceived));
	}

	private void Start()
	{
		UnityEngine.Object[] array = Resources.LoadAll("InApp", typeof(InAppCarac));
		Singleton<GameManager>.Instance.Init();
		Singleton<GameConfigurator>.Instance.Init();
		Singleton<GameConfigurator>.Instance.FacebookAppID = FacebookAppID;
		Singleton<InputManager>.Instance.Init();
		Singleton<RandomManager>.Instance.Init();
		Singleton<GameOptionManager>.Instance.Init();
		Singleton<GameSaveManager>.Instance.Init();
		Singleton<ChallengeManager>.Instance.Init();
		Singleton<RewardManager>.Instance.Reset();
		string[] array2 = new string[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] is InAppCarac)
			{
				InAppCarac inAppCarac = (InAppCarac)array[i];
				array2[i] = inAppCarac.ProdutId;
			}
		}
		Singleton<InAppManager>.Instance.CollectStoreInfo(array2);
		Singleton<GameConfigurator>.Instance.StartScene = StartScene;
		LoadingManager.LoadLevel(StartScene);
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("AISettings"));
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		AISettings component = gameObject.GetComponent<AISettings>();
		Singleton<GameConfigurator>.Instance.AISettings = component;
		GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("GameSettings"));
		UnityEngine.Object.DontDestroyOnLoad(gameObject2);
		GameSettings component2 = gameObject2.GetComponent<GameSettings>();
		Singleton<GameConfigurator>.Instance.GameSettings = component2;
		if (ASE_Tools.Available)
		{
			ASE_ChartBoost.Init(Singleton<GameConfigurator>.Instance.ChartBoostAppID, Singleton<GameConfigurator>.Instance.ChartBoostAppSignature);
			ASE_ChartBoost.CacheInterstitial("Default");
			ASE_ChartBoost.CacheMoreApps();
			ASE_Facebook.Connect(Singleton<GameConfigurator>.Instance.FacebookAppID);
		}
		m_eFacebookState = eFacebookState.None;
	}

	private void Update()
	{
		DebugMgr.Instance.NullSafe(delegate
		{
		});
		Singleton<ChallengeManager>.Instance.NullSafe(delegate(ChallengeManager i)
		{
			i.Update();
		});
	}

	private void LateUpdate()
	{
		Singleton<InputManager>.Instance.Update();
	}

	public void InAppProductDataReceived(List<InAppProductData> pList)
	{
		m_pInAppProduct = pList;
	}

	public InAppProductData GetInAppData(string sProductID)
	{
		if (m_pInAppProduct.Count == 0)
		{
			return null;
		}
		foreach (InAppProductData item in m_pInAppProduct)
		{
			if (item.ProductID == sProductID)
			{
				return item;
			}
		}
		return null;
	}

	private void AutoChooseQualityLevel()
	{
		if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
		{
			return;
		}
		float[] array = new float[6] { 0.4f, 0.5f, 0.6f, 0.7f, 1.5f, 2f };
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			QualitySettings.lodBias = array[4];
			return;
		}
		int graphicsShaderLevel = SystemInfo.graphicsShaderLevel;
		int num = SystemInfo.graphicsPixelFillrate;
		int graphicsMemorySize = SystemInfo.graphicsMemorySize;
		int processorCount = SystemInfo.processorCount;
		if (num < 0)
		{
			num = ((graphicsShaderLevel < 10) ? 1000 : ((graphicsShaderLevel < 20) ? 1300 : ((graphicsShaderLevel >= 30) ? 3000 : 2000)));
			if (processorCount >= 6)
			{
				num *= 3;
			}
			else if (processorCount >= 3)
			{
				num *= 2;
			}
			if (graphicsMemorySize >= 512)
			{
				num *= 2;
			}
			else if (graphicsMemorySize <= 128)
			{
				num /= 2;
			}
		}
		int width = Screen.width;
		int height = Screen.height;
		float num2 = (float)(width * height + 120000) * 3E-05f;
		float[] array2 = new float[6] { 5f, 30f, 80f, 130f, 200f, 320f };
		int i = 0;
		for (string[] names = QualitySettings.names; i < names.Length && (float)num > num2 * array2[i + 1]; i++)
		{
		}
		QualitySettings.lodBias = array[i];
	}

	private void FacebookEvent(string sData)
	{
		string text = ((ASE_Facebook.FacebookEvent)ASE_Tools.GetDataEvent(sData)).ToString();
		if (sData.Length > 1)
		{
			text = text + " : " + ASE_Tools.GetDataMessage(sData);
		}
		if (text.Contains("FBEVENT_PUBLICATION_DID_SHARED"))
		{
			if (m_eFacebookState == eFacebookState.SharingAsked)
			{
				Singleton<RewardManager>.Instance.GiveSharingReward();
				Singleton<GameSaveManager>.Instance.SetAskSharing(false, true);
			}
		}
		else if (!text.Contains("FBEVENT_PUBLICATION_DID_CANCELED"))
		{
		}
	}

	private void ChartBoostEvent(string sData)
	{
		string text = ((ASE_ChartBoost.ChartBoostEvent)ASE_Tools.GetDataEvent(sData)).ToString();
		if (sData.Length > 1)
		{
			text = text + " : " + ASE_Tools.GetDataMessage(sData);
		}
	}

	public void OnFacebook(string sTitle, string sDescription)
	{
		if (ASE_Tools.Available)
		{
			m_sFacebookTitle = sTitle;
			m_sFacebookDescription = sDescription;
			if (!ASE_Facebook.IsConnected())
			{
				ASE_Facebook.Connect(Singleton<GameConfigurator>.Instance.FacebookAppID);
			}
			if (ASE_Facebook.IsConnected())
			{
				FacebookOperations();
			}
		}
	}

	public void DoSharing()
	{
		m_bSharingOn = true;
		OnFacebook(string.Empty, string.Empty);
	}

	public void FacebookLogin()
	{
		if (ASE_Facebook.IsConnected() && string.IsNullOrEmpty(ASE_Facebook.GetUserId()))
		{
			ASE_Facebook.Login();
			Debug.Log("login facebook");
			m_eFacebookState = eFacebookState.Login;
		}
	}

	public void FacebookOperations()
	{
		if (m_bSharingOn)
		{
			ASE_Facebook.Publish("GarfieldKart", string.Empty, Localization.instance.Get("FB_SHARE_TEXT"), "https://itunes.apple.com/fr/app/id656101044?mt=8", "http://iphone.anuman.fr/FACEBOOK/Microidsgamesforall/GarfielKart/garfieldkart_320.jpg");
			m_eFacebookState = eFacebookState.SharingAsked;
			m_bSharingOn = false;
			return;
		}
		string icon = string.Empty;
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.CHAMPIONSHIP)
		{
			icon = "http://iphone.anuman.fr/FACEBOOK/Microidsgamesforall/GarfielKart/GK12_championship.png";
		}
		else if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.SINGLE)
		{
			icon = "http://iphone.anuman.fr/FACEBOOK/Microidsgamesforall/GarfielKart/GK12_single_race.png";
		}
		else if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TIME_TRIAL)
		{
			icon = "http://iphone.anuman.fr/FACEBOOK/Microidsgamesforall/GarfielKart/GK12_time_trial.png";
		}
		ASE_Facebook.Publish(m_sFacebookTitle, string.Empty, m_sFacebookDescription, "https://itunes.apple.com/fr/app/id656101044?mt=8", icon);
		m_eFacebookState = eFacebookState.PublishAsked;
	}
}

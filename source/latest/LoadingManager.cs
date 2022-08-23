using System;
using System.Collections;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
	private const float ms_fLoadingMinimumDuration = 1f;

	private float m_fElapsedTime;

	private float m_fEndTime;

	private AsyncOperation m_pAO;

	private static bool m_bLoadingInProgress;

	public static bool loadingFinished;

	private static string m_sLevelToLoad = string.Empty;

	public static int LevelIndex;

	private static NetworkMgr m_oNetworkMgr;

	public UILabel Label;

	public UISprite[] Sprites;

	public static string SLevelToLoad
	{
		get
		{
			return m_sLevelToLoad;
		}
	}

	private void Awake()
	{
		Texture2D mainTexture = Resources.Load("ANIM", typeof(Texture2D)) as Texture2D;
		UISprite[] sprites = Sprites;
		foreach (UISprite uISprite in sprites)
		{
			uISprite.atlas.spriteMaterial.mainTexture = mainTexture;
		}
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		m_bLoadingInProgress = true;
		m_fElapsedTime = 0f;
		m_fEndTime = float.MinValue;
		m_pAO = null;
		Label.text = string.Empty;
		if ((bool)Label && m_sLevelToLoad != "MenuRoot" && Singleton<GameConfigurator>.Instance.ChampionShipData != null)
		{
			Label.text = Singleton<GameConfigurator>.Instance.ChampionShipData.TracksName[Singleton<GameConfigurator>.Instance.CurrentTrackIndex];
		}
		m_oNetworkMgr = (NetworkMgr)UnityEngine.Object.FindObjectOfType(typeof(NetworkMgr));
		if (Network.peerType != 0)
		{
			NetworkPlayer[] connections = Network.connections;
			foreach (NetworkPlayer player in connections)
			{
				if (Network.isServer)
				{
					Network.SetReceivingEnabled(player, 0, true);
				}
				Network.isMessageQueueRunning = true;
				Network.SetSendingEnabled(0, true);
			}
		}
		StartCoroutine(StartLoading());
	}

	private IEnumerator StartLoading()
	{
		m_pAO = Application.LoadLevelAsync(m_sLevelToLoad);
		yield return m_pAO;
	}

	public void OnDestroy()
	{
		UISprite[] sprites = Sprites;
		foreach (UISprite uISprite in sprites)
		{
			Resources.UnloadAsset(uISprite.atlas.spriteMaterial.mainTexture);
		}
	}

	private void Update()
	{
		if (m_bLoadingInProgress)
		{
			m_fElapsedTime += Time.deltaTime;
			if (m_pAO != null && !Application.isLoadingLevel && m_fElapsedTime >= 1f)
			{
				m_fEndTime = m_fElapsedTime;
				m_bLoadingInProgress = false;
			}
		}
		else
		{
			m_fEndTime += Time.deltaTime;
		}
		if (m_fEndTime > m_fElapsedTime + 0.2f)
		{
			UnityEngine.Object.DestroyObject(base.gameObject);
			loadingFinished = true;
		}
	}

	public static bool IsLoading()
	{
		return m_bLoadingInProgress;
	}

	public static void LoadLevel(string _levelName)
	{
		if (Network.peerType != 0)
		{
			if (Network.isServer)
			{
				LevelIndex++;
				m_oNetworkMgr.networkView.RPC("SetLevelIndex", RPCMode.Others, LevelIndex);
			}
			NetworkPlayer[] connections = Network.connections;
			foreach (NetworkPlayer player in connections)
			{
				if (Network.isServer)
				{
					Network.SetReceivingEnabled(player, 0, false);
				}
			}
			Network.SetSendingEnabled(0, false);
			Network.isMessageQueueRunning = false;
			Network.SetLevelPrefix(LevelIndex);
		}
		loadingFinished = false;
		Singleton<GameManager>.Instance.Reset();
		m_sLevelToLoad = _levelName;
		Application.LoadLevel("LoadingScreen");
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}
}

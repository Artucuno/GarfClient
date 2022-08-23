using System;
using System.Collections;
using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
	public enum ECreationState
	{
		None,
		SceneLoaded,
		PlayersCreated,
		SceneStarted
	}

	private ECreationState m_eState;

	private int m_iStep;

	private NetworkMgr networkMgr;

	private GameObject m_pSounds;

	public static Action OnVehicleCreated;

	public ECreationState State
	{
		get
		{
			return m_eState;
		}
	}

	private void Awake()
	{
		m_eState = ECreationState.None;
		networkMgr = (NetworkMgr)UnityEngine.Object.FindObjectOfType(typeof(NetworkMgr));
		m_pSounds = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Sounds"));
		if ((bool)m_pSounds)
		{
			SoundManager component = m_pSounds.GetComponent<SoundManager>();
			Singleton<GameManager>.Instance.SoundManager = component;
		}
		Screen.sleepTimeout = -1;
	}

	private IEnumerator Start()
	{
		if (DebugMgr.Instance != null && Singleton<GameConfigurator>.Instance.StartScene.Equals("MenuRoot"))
		{
			Singleton<GameConfigurator>.Instance.StartScene = DebugMgr.Instance.dbgData.SceneToLaunch;
		}
		while (LoadingManager.IsLoading())
		{
			yield return null;
		}
		Singleton<GameManager>.Instance.LaunchGame();
		if ((bool)Singleton<GameConfigurator>.Instance.PlayerConfig)
		{
			Singleton<GameConfigurator>.Instance.PlayerConfig.ResetAdvantages();
		}
		if (Network.peerType != 0)
		{
			networkMgr.StartSynchronization();
		}
		m_eState = ECreationState.SceneLoaded;
		m_iStep = 0;
	}

	private void OnDestroy()
	{
		Screen.sleepTimeout = -2;
		Singleton<BonusMgr>.DestroyInstance();
		Network.RemoveRPCs(Network.player, 0);
	}

	public void Update()
	{
		if (m_eState == ECreationState.SceneLoaded)
		{
			UnityEngine.Object[] array = UnityEngine.Object.FindSceneObjectsOfType(typeof(RcPortalTrigger));
			for (int j = 0; j < array.Length; j++)
			{
				RcPortalTrigger rcPortalTrigger = (RcPortalTrigger)array[j];
				rcPortalTrigger.enabled = false;
			}
			if (Network.peerType == NetworkPeerType.Disconnected || !networkMgr.WaitingSynchronization)
			{
				Singleton<BonusMgr>.Instance.Init();
				Singleton<GameManager>.Instance.CreatePlayers();
				m_eState = ECreationState.PlayersCreated;
				if (Network.peerType != 0)
				{
					networkMgr.StartSynchronization();
				}
			}
		}
		else if (m_eState == ECreationState.PlayersCreated && m_iStep++ > 5 && (Network.peerType == NetworkPeerType.Disconnected || !networkMgr.WaitingSynchronization))
		{
			if (OnVehicleCreated != null)
			{
				OnVehicleCreated();
			}
			Singleton<GameManager>.Instance.GameMode.StartScene();
			Singleton<BonusMgr>.Instance.NullSafe(delegate(BonusMgr i)
			{
				i.StartScene();
			});
			m_eState = ECreationState.SceneStarted;
		}
	}
}

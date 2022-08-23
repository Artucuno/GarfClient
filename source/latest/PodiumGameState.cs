using UnityEngine;

public class PodiumGameState : GameState
{
	public Camera m_Camera;

	private GameObject m_pPodium;

	private float m_fFadeTimer = -1f;

	private Animation m_pCameraAnimation;

	public override void Enter()
	{
		GameObject gameObject = GameObject.Find("Podium");
		m_pGameMode.Hud.HUDFade.ForceIn();
		m_fFadeTimer = 0f;
		m_pPodium = gameObject.transform.GetChild(0).gameObject;
		if (m_pGameMode.MainMusic != null && m_pGameMode.MainMusic.isPlaying)
		{
			m_pGameMode.MainMusic.Stop();
		}
		Singleton<GameManager>.Instance.SoundManager.StopMusic();
		m_pPodium.SetActive(true);
		m_pCameraAnimation = m_pPodium.GetComponentInChildren<Animation>();
		DesactivatePhysic();
		if (m_Camera == null)
		{
			m_Camera = m_pPodium.transform.Find("CameraPodium").camera;
		}
		foreach (GameObject namePlate in Singleton<GameManager>.Instance.GameMode.Hud.HUDPause.NamePlates)
		{
			namePlate.GetComponent<Billboard>().SetCamera(m_Camera);
		}
		BonusEntity[] array = (BonusEntity[])Object.FindObjectsOfType(typeof(BonusEntity));
		BonusEntity[] array2 = array;
		foreach (BonusEntity bonusEntity in array2)
		{
			bonusEntity.gameObject.SetActive(false);
		}
	}

	private void DesactivatePhysic()
	{
		for (int i = 0; i < Singleton<GameConfigurator>.Instance.RankingManager.RaceScoreCount(); i++)
		{
			ChampionShipScoreData championshipPos = Singleton<GameConfigurator>.Instance.RankingManager.GetChampionshipPos(i);
			GameObject playerWithVehicleId = m_pGameMode.GetPlayerWithVehicleId(championshipPos.KartIndex);
			Kart kartWithVehicleId = m_pGameMode.GetKartWithVehicleId(championshipPos.KartIndex);
			kartWithVehicleId.FxMgr.Stop();
			kartWithVehicleId.GetBonusMgr().GetBonusEffectMgr().Reset();
			kartWithVehicleId.GetBonusMgr().GetBonusEffectMgr().Dispose();
			kartWithVehicleId.SetArcadeDriftFactor(0f);
			playerWithVehicleId.GetComponentInChildren<RcNetworkController>().enabled = false;
			kartWithVehicleId.GetVehiclePhysic().Enable = false;
			kartWithVehicleId.SetLocked(true);
			playerWithVehicleId.GetComponentInChildren<RcVirtualController>().SetDrivingEnabled(false);
			playerWithVehicleId.SetActive(i < 3 || kartWithVehicleId.GetControlType() == RcVehicle.ControlType.Human);
			if (i < 3)
			{
				GameObject gameObject = GameObject.Find(m_pPodium.name + "/Kart" + (i + 1));
				playerWithVehicleId.transform.position = gameObject.transform.position;
				playerWithVehicleId.transform.rotation = gameObject.transform.rotation;
				KartAnim componentInChildren = playerWithVehicleId.GetComponentInChildren<KartAnim>();
				componentInChildren.LaunchDefeatAnim(false);
				componentInChildren.LaunchVictoryAnim(true);
				if (kartWithVehicleId.GetControlType() == RcVehicle.ControlType.Human)
				{
					PlayMusic(true);
				}
			}
			else if (kartWithVehicleId.GetControlType() == RcVehicle.ControlType.Human)
			{
				GameObject gameObject2 = GameObject.Find(m_pPodium.name + "/Kart4");
				playerWithVehicleId.transform.position = gameObject2.transform.position;
				playerWithVehicleId.transform.rotation = gameObject2.transform.rotation;
				KartAnim anim = kartWithVehicleId.Anim;
				anim.LaunchVictoryAnim(false);
				anim.LaunchDefeatAnim(true);
				PlayMusic(false);
			}
		}
	}

	public override void Exit()
	{
		m_fFadeTimer = -1f;
	}

	public void Next()
	{
		OnStateChanged(E_GameState.Result);
	}

	protected override void Update()
	{
		if (m_fFadeTimer >= 0f)
		{
			m_fFadeTimer += Time.deltaTime;
			if (m_fFadeTimer > 0.5f)
			{
				m_pGameMode.Hud.HUDFade.DoFadeOut(2.5f);
				m_fFadeTimer = -1f;
			}
		}
		if ((bool)m_pCameraAnimation && !m_pCameraAnimation.isPlaying && (bool)m_pGameMode.Hud && !m_pGameMode.Hud.HUDPodium.activeSelf)
		{
			m_pGameMode.Hud.ShowHudPodium();
		}
	}

	public void PlayMusic(bool _Victory)
	{
		if (_Victory)
		{
			Singleton<GameManager>.Instance.SoundManager.PlayMusic(ERaceMusicLoops.PodiumVictory);
		}
		else
		{
			Singleton<GameManager>.Instance.SoundManager.PlayMusic(ERaceMusicLoops.PodiumDefeat);
		}
	}
}

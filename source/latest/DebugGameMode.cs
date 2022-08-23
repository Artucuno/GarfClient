using UnityEngine;

public class DebugGameMode : GameMode
{
	private bool PlaceVehicles;

	private Transform StartPosition;

	public override void Awake()
	{
		base.Awake();
		StartPosition = null;
		PlaceVehicles = false;
	}

	public override void CreatePlayers()
	{
		if (!DebugMgr.Instance.dbgData.RandomPlayer)
		{
			DebugMgr.Instance.LoadDefaultPlayer(0, this);
			return;
		}
		ECharacter character = Singleton<GameConfigurator>.Instance.PlayerConfig.Character;
		ECharacter kart = Singleton<GameConfigurator>.Instance.PlayerConfig.Kart;
		GameObject gameObject = (GameObject)Resources.Load("Hat/" + Singleton<GameConfigurator>.Instance.PlayerConfig.m_oHat.name);
		GameObject gameObject2 = (GameObject)Resources.Load("Kart/" + Singleton<GameConfigurator>.Instance.PlayerConfig.m_oKartCustom.name);
		CreatePlayer(character, kart, gameObject2.name, gameObject.name, 0, 0, false, false);
	}

	public void FixedUpdate()
	{
		if (PlaceVehicles)
		{
			RcKinematicPhysic rcKinematicPhysic = (RcKinematicPhysic)m_pPlayers[0].Item2.GetVehiclePhysic();
			rcKinematicPhysic.Teleport(StartPosition.position, StartPosition.rotation);
			m_pPlayers[0].Item2.Enable();
			PlaceVehicles = false;
			m_bReadyToStart = true;
			Object[] array = Object.FindSceneObjectsOfType(typeof(RcPortalTrigger));
			for (int i = 0; i < array.Length; i++)
			{
				RcPortalTrigger rcPortalTrigger = (RcPortalTrigger)array[i];
				rcPortalTrigger.enabled = true;
			}
		}
	}

	public override void StartScene()
	{
		ComputePlayerAdvantages();
		GameObject gameObject = GameObject.Find("Start");
		if (gameObject != null)
		{
			StartPosition = gameObject.transform.GetChild(0);
			PlaceVehicles = true;
		}
	}
}

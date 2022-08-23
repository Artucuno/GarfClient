using UnityEngine;

public class PlayerBuilder : MonoBehaviour
{
	private string[] m_sKartWheel = new string[4] { "b_kart_l_wheel_ar", "b_kart_l_wheel_av", "b_kart_r_wheel_ar", "b_kart_r_wheel_av" };

	private string[] m_sPlayerWheel = new string[4] { "WheelLAr", "WheelLAv", "WheelRAr", "WheelRAv" };

	[RPC]
	public void Build(int characterIndex, int kartIndex, string customName, string hatName, int iNbStars, int pIndex, bool pLock, bool vIsAI, string pseudo, Vector3 vColor)
	{
		PlayerConfig playerConfig = Singleton<GameConfigurator>.Instance.PlayerConfig;
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		base.transform.localPosition = Vector3.zero;
		if (characterIndex != -1 && kartIndex != -1)
		{
			GameObject gameObject3 = (GameObject)Object.Instantiate(Resources.Load("Kart/" + playerConfig.KartPrefab[kartIndex]));
			Transform transform = gameObject3.transform.Find("Kart(Clone)/b_kart_root");
			gameObject3.transform.parent = base.transform;
			gameObject3.transform.localPosition = Vector3.zero;
			for (int i = 0; i < m_sKartWheel.Length; i++)
			{
				AssignWheel(i, transform);
			}
			KartFxMgr componentInChildren = GetComponentInChildren<KartFxMgr>();
			if (componentInChildren != null)
			{
				componentInChildren.InstantiateAndAttach(gameObject3.transform);
				if (componentInChildren.enabled)
				{
				}
				componentInChildren.enabled = true;
			}
			GameObject gameObject4 = (GameObject)Object.Instantiate(Resources.Load("Character/" + playerConfig.CharacterPrefab[characterIndex]));
			gameObject4.transform.parent = transform.Find("b_kart_body");
			KartAnim componentInChildren2 = GetComponentInChildren<KartAnim>();
			if (componentInChildren2.enabled)
			{
			}
			componentInChildren2.enabled = true;
			componentInChildren2.m_pKartAnimatorOwner = gameObject3.transform;
			componentInChildren2.m_pCharacterAnimatorOwner = gameObject4.transform;
			Component[] componentsInChildren = GetComponentsInChildren<RcKinematicWheel>();
			Component[] array = componentsInChildren;
			for (int j = 0; j < array.Length; j++)
			{
				RcKinematicWheel rcKinematicWheel = (RcKinematicWheel)array[j];
				rcKinematicWheel.m_pVehicleTransform = base.transform;
				rcKinematicWheel.VehicleRoot = transform;
				if (rcKinematicWheel.enabled)
				{
				}
				rcKinematicWheel.enabled = true;
			}
			RcKinematicPhysic componentInChildren3 = GetComponentInChildren<RcKinematicPhysic>();
			componentInChildren3.m_pVehicleMesh = gameObject3.transform;
			componentInChildren3.ConfigureWheels();
			PlayerCarac componentInChildren4 = GetComponentInChildren<PlayerCarac>();
			componentInChildren4.KartCarac = gameObject3.GetComponent<KartCarac>();
			componentInChildren4.CharacterCarac = gameObject4.GetComponent<CharacterCarac>();
			if (customName != null)
			{
				Object @object = Resources.Load("Kart/" + customName);
				if (@object != null)
				{
					gameObject = (GameObject)Object.Instantiate(@object);
					gameObject.transform.parent = transform.Find("b_kart_body");
					PlayerCustom componentInChildren5 = GetComponentInChildren<PlayerCustom>();
					componentInChildren5.KartCustom = gameObject.GetComponent<KartCustom>();
				}
			}
			if (hatName != null)
			{
				Object object2 = Resources.Load("Hat/" + hatName);
				if (object2 != null)
				{
					gameObject2 = (GameObject)Object.Instantiate(object2);
					Transform child = gameObject4.transform.GetChild(0);
					gameObject2.transform.parent = child.transform.Find("b_cat Attachment/b_cat Root/b_cat Pelvis/b_cat Spine/b_cat Neck/b_cat Head");
					if (gameObject2.transform.parent == null)
					{
						gameObject2.transform.parent = gameObject4.transform.Find("b_cat Attachment/b_cat Root/b_cat Pelvis/b_cat Spine/b_cat Neck/b_cat Head");
					}
					PlayerCustom componentInChildren6 = GetComponentInChildren<PlayerCustom>();
					gameObject2.transform.localPosition = Vector3.zero;
					componentInChildren6.BonusCustom = gameObject2.GetComponent<BonusCustom>();
					if (componentInChildren6.BonusCustom != null)
					{
						Template template = componentInChildren6.BonusCustom.GetTemplate((ECharacter)characterIndex);
						if (template != null)
						{
							gameObject2.transform.localPosition = template.Position;
							gameObject2.transform.localScale = template.Scale;
						}
					}
					int childCount = gameObject2.transform.GetChildCount();
					for (int num = childCount - 1; num >= 0; num--)
					{
						Transform child2 = gameObject2.transform.GetChild(num);
						if (child2.name.Contains("LOD"))
						{
							child2.parent = gameObject4.transform;
						}
					}
				}
			}
			LodRemote component = gameObject3.GetComponent<LodRemote>();
			if ((bool)component)
			{
				component.m_pLodGroupReceiver[0] = gameObject4.GetComponent<LODGroup>();
				component.m_pLodGroupReceiver[1] = gameObject2.GetComponent<LODGroup>();
				component.m_pLodGroupReceiver[2] = gameObject.GetComponent<LODGroup>();
			}
			LODGroup component2 = gameObject3.GetComponent<LODGroup>();
			if (!vIsAI)
			{
				component2.ForceLOD(0);
			}
		}
		Kart componentInChildren7 = GetComponentInChildren<Kart>();
		componentInChildren7.SetLocked(pLock);
		componentInChildren7.Index = pIndex;
		if (base.networkView.isMine || Network.peerType == NetworkPeerType.Disconnected)
		{
			if (vIsAI)
			{
				componentInChildren7.SetControlType(RcVehicle.ControlType.AI);
				Object.DestroyImmediate(GetComponentInChildren<RcHumanController>());
			}
			else
			{
				componentInChildren7.SetControlType(RcVehicle.ControlType.Human);
				HUDInGame hud = Singleton<GameManager>.Instance.GameMode.Hud;
				if (hud != null)
				{
					componentInChildren7.HUDBonus = hud.Bonus;
					componentInChildren7.HUDPosition = hud.Position;
				}
				base.gameObject.tag = "Player";
			}
		}
		else
		{
			componentInChildren7.SetControlType(RcVehicle.ControlType.Net);
			Object.DestroyImmediate(GetComponentInChildren<RcHumanController>());
			Color cColor = new Color(vColor.x, vColor.y, vColor.z, 1f);
			PlayerData pPlayerData = new PlayerData((ECharacter)characterIndex, (ECharacter)kartIndex, customName, hatName, iNbStars, (!vIsAI) ? pseudo : string.Empty, cColor);
			((InGameGameMode)Singleton<GameManager>.Instance.GameMode).AddPlayerData(pPlayerData, pIndex);
		}
		if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
		{
			RcSkidMarks[] componentsInChildren2 = GetComponentsInChildren<RcSkidMarks>();
			foreach (RcSkidMarks rcSkidMarks in componentsInChildren2)
			{
				rcSkidMarks.enabled = true;
			}
		}
		Singleton<GameManager>.Instance.GameMode.AddPlayer(base.gameObject, componentInChildren7);
		Singleton<GameConfigurator>.Instance.RankingManager.InitPlayer(pIndex, vIsAI);
		componentInChildren7.SetVehicleId(pIndex);
		base.transform.position = new Vector3(0f, -1000f, 0f);
	}

	public void AssignWheel(int _Index, Transform _Kart)
	{
		GameObject gameObject = base.transform.FindChild(m_sPlayerWheel[_Index]).gameObject;
		if (gameObject != null)
		{
			RcKinematicWheel component = gameObject.GetComponent<RcKinematicWheel>();
			component.m_pVehicleTransform = base.transform;
			component.m_pWheelMeshTransform = _Kart.Find(m_sKartWheel[_Index]);
		}
	}
}

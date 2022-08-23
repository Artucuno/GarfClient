using System;
using System.Collections.Generic;
using UnityEngine;

public class HUDRadar : MonoBehaviour
{
	public GameObject[] Results = new GameObject[3];

	public List<UISprite> CharacterSprites = new List<UISprite>();

	public List<UISprite> Pies = new List<UISprite>();

	public List<UISprite> AutolockPies = new List<UISprite>();

	public List<UISprite> CharacterMiniMap = new List<UISprite>();

	public List<UILabel> PositionsMiniMap = new List<UILabel>();

	public List<UISprite> SelectorMiniMap = new List<UISprite>();

	public List<Color> SelectorColors = new List<Color>();

	public UISprite Ufo;

	public UISprite EndLine;

	public UISprite ProgressionBar;

	public Color BadUfoColor;

	public Color GoodUfoColor;

	private GameObject _player;

	private int _playerIndex = -1;

	private RcVehicleRaceStats _playerStats;

	private List<Tuple<GameObject, int, Renderer, RcVehicleRaceStats>> _vehicles;

	private RcMultiPath _idealPath;

	private bool _hasStart;

	private float _raceLength;

	private int[] _characters = new int[6];

	public float MaxLimit;

	private float m_fPosXLimit;

	public int RefreshRate = 5;

	private Transform vPlayerTransform;

	private Transform vProgressionBarTranform;

	private Transform[] vCharacterMiniMapTransform = new Transform[6];

	private Transform[] vVehicleTransforms = new Transform[6];

	private int m_bUpdate;

	private void Start()
	{
		foreach (UISprite characterSprite in CharacterSprites)
		{
			characterSprite.enabled = false;
		}
		foreach (UISprite py in Pies)
		{
			py.enabled = false;
		}
		foreach (UISprite autolockPy in AutolockPies)
		{
			autolockPy.enabled = false;
		}
		GameObject[] results = Results;
		foreach (GameObject gameObject in results)
		{
			gameObject.SetActive(false);
		}
		Ufo.enabled = false;
		UpsideDownBonusEffect.OnLaunch = (Action<int>)Delegate.Combine(UpsideDownBonusEffect.OnLaunch, new Action<int>(LaunchAnim));
		SpinBonusEffect.OnLaunch = (Action<int>)Delegate.Combine(SpinBonusEffect.OnLaunch, new Action<int>(LaunchAnim));
		GameObject gameObject2 = GameObject.Find("RadarRef");
		m_fPosXLimit = gameObject2.transform.parent.localPosition.x + gameObject2.transform.localPosition.x;
		vProgressionBarTranform = ProgressionBar.transform;
	}

	private void OnDestroy()
	{
		UpsideDownBonusEffect.OnLaunch = (Action<int>)Delegate.Remove(UpsideDownBonusEffect.OnLaunch, new Action<int>(LaunchAnim));
		SpinBonusEffect.OnLaunch = (Action<int>)Delegate.Remove(SpinBonusEffect.OnLaunch, new Action<int>(LaunchAnim));
	}

	private void LaunchAnim(int pIndex)
	{
		CharacterMiniMap[pIndex].animation.Play();
		CharacterSprites[pIndex].animation.Play();
	}

	public void StartRace()
	{
		GameObject gameObject = GameObject.Find("SplineRespawn");
		_idealPath = null;
		if (gameObject != null)
		{
			_idealPath = gameObject.GetComponent<RcMultiPath>();
			_raceLength = _idealPath.TotalLength;
		}
		_vehicles = new List<Tuple<GameObject, int, Renderer, RcVehicleRaceStats>>();
		_playerIndex = Singleton<GameManager>.Instance.GameMode.GetHumanPlayerVehicleId();
		PlayerData[] playerDataList = Singleton<GameConfigurator>.Instance.PlayerDataList;
		for (int i = 0; i < 6; i++)
		{
			GameObject playerWithVehicleId = Singleton<GameManager>.Instance.GameMode.GetPlayerWithVehicleId(i);
			Kart kartWithVehicleId = Singleton<GameManager>.Instance.GameMode.GetKartWithVehicleId(i);
			if (!(playerWithVehicleId != null) || !(kartWithVehicleId != null))
			{
				continue;
			}
			int vehicleId = kartWithVehicleId.GetVehicleId();
			kartWithVehicleId.OnRaceEnded = (Action<RcVehicle>)Delegate.Combine(kartWithVehicleId.OnRaceEnded, new Action<RcVehicle>(RaceEnd));
			ECharacter owner = playerWithVehicleId.GetComponentInChildren<CharacterCarac>().Owner;
			_vehicles.Add(new Tuple<GameObject, int, Renderer, RcVehicleRaceStats>(playerWithVehicleId, vehicleId, playerWithVehicleId.GetComponentInChildren<SkinnedMeshRenderer>(), playerWithVehicleId.GetComponentInChildren<RcVehicleRaceStats>()));
			vVehicleTransforms[vehicleId] = playerWithVehicleId.transform;
			CharacterMiniMap[vehicleId].enabled = true;
			CharacterMiniMap[vehicleId].GetComponent<UITexturePattern>().ChangeTexture((int)owner);
			CharacterSprites[vehicleId].GetComponent<UITexturePattern>().ChangeTexture((int)owner);
			_characters[vehicleId] = (int)owner;
			if (Network.peerType != 0)
			{
				if (vehicleId == _playerIndex)
				{
					SelectorMiniMap[_playerIndex].enabled = true;
				}
				else
				{
					RaceScoreData scoreData = Singleton<GameConfigurator>.Instance.RankingManager.GetScoreData(vehicleId);
					if (scoreData != null)
					{
						SelectorMiniMap[vehicleId].enabled = !scoreData.IsAI;
					}
					else
					{
						SelectorMiniMap[vehicleId].enabled = false;
					}
				}
				if (playerDataList[vehicleId] != null)
				{
					SelectorMiniMap[vehicleId].color = playerDataList[vehicleId].CharacColor;
				}
			}
			else
			{
				SelectorMiniMap[vehicleId].enabled = false;
			}
			if (vehicleId == _playerIndex)
			{
				CharacterMiniMap[_playerIndex].depth = 30;
				_player = playerWithVehicleId;
				_playerStats = playerWithVehicleId.GetComponentInChildren<RcVehicleRaceStats>();
				vPlayerTransform = _player.transform;
			}
			vCharacterMiniMapTransform[vehicleId] = CharacterMiniMap[vehicleId].transform.parent.transform;
		}
		_hasStart = true;
	}

	private void Update()
	{
		if (!_hasStart || ++m_bUpdate % RefreshRate != 0)
		{
			return;
		}
		float num = _playerStats.GetDistToEndOfLap();
		float y = vProgressionBarTranform.localScale.y;
		float num2 = (0f - y) * (num / _raceLength);
		num2 -= vProgressionBarTranform.localPosition.y;
		Transform transform = vCharacterMiniMapTransform[_playerIndex];
		vCharacterMiniMapTransform[_playerIndex].localPosition = new Vector3(transform.localPosition.x, num2, transform.localPosition.z);
		int rank = _playerStats.GetRank();
		if (rank < 3)
		{
			PositionsMiniMap[_playerIndex].text = (rank + 1).ToString();
		}
		else
		{
			PositionsMiniMap[_playerIndex].text = string.Empty;
		}
		foreach (Tuple<GameObject, int, Renderer, RcVehicleRaceStats> vehicle in _vehicles)
		{
			GameObject item = vehicle.Item1;
			int item2 = vehicle.Item2;
			Renderer item3 = vehicle.Item3;
			RcVehicleRaceStats item4 = vehicle.Item4;
			if (item2 == _playerIndex)
			{
				continue;
			}
			int rank2 = item4.GetRank();
			if (rank2 < 3)
			{
				PositionsMiniMap[item2].text = (rank2 + 1).ToString();
			}
			else
			{
				PositionsMiniMap[item2].text = string.Empty;
			}
			if (item == null)
			{
				continue;
			}
			Transform transform2 = vVehicleTransforms[item2];
			float distToEndOfLap = item4.GetDistToEndOfLap();
			if (!item3.isVisible)
			{
				float num3 = 0f;
				if (item4.GetLogicNbLap() != _playerStats.GetLogicNbLap())
				{
					num = _raceLength - num;
					num3 = num + distToEndOfLap;
				}
				else
				{
					num3 = distToEndOfLap - num;
				}
				Vector3 vector = transform2.position - vPlayerTransform.position;
				if (num3 >= 0f && num3 <= 50f)
				{
					float num4 = Vector3.Dot(vector, vPlayerTransform.forward);
					Vector3 vector2 = vPlayerTransform.position + vPlayerTransform.forward * num4;
					float magnitude = (transform2.position - vector2).magnitude;
					magnitude *= AngleDir(vPlayerTransform.forward, vector, vPlayerTransform.up);
					UpdateSprite(item2, magnitude, num3, CharacterSprites, 0.1f, true);
				}
				else
				{
					CharacterSprites[item2].enabled = false;
				}
			}
			else
			{
				CharacterSprites[item2].enabled = false;
			}
			float num5 = (0f - y) * (distToEndOfLap / _raceLength);
			num5 -= vProgressionBarTranform.localPosition.y;
			Transform transform3 = vCharacterMiniMapTransform[item2];
			vCharacterMiniMapTransform[item2].localPosition = new Vector3(transform3.localPosition.x, num5, transform3.localPosition.z);
			int depth = (int)item4.GetVehicle().m_eControlType * 10 - rank2;
			CharacterMiniMap[item2].depth = depth;
			PositionsMiniMap[item2].depth = depth;
		}
	}

	private void RaceEnd(RcVehicle pVehicle)
	{
		if (!(pVehicle is Kart))
		{
			return;
		}
		int vehicleId = pVehicle.GetVehicleId();
		int num = 3;
		bool flag = SelectorMiniMap[vehicleId].enabled;
		Color color = SelectorMiniMap[vehicleId].color;
		PositionsMiniMap[vehicleId].alpha = 0.25f;
		SelectorMiniMap[vehicleId].alpha = 0.25f;
		CharacterMiniMap[vehicleId].alpha = 0.25f;
		if (vehicleId != _playerIndex)
		{
			num = _vehicles[vehicleId].Item4.GetRank();
		}
		else
		{
			num = _playerStats.GetRank();
			CharacterSprites[_playerIndex].transform.parent.gameObject.SetActive(false);
		}
		if (num < 3)
		{
			Results[num].SetActive(true);
			UITexturePattern componentInChildren = Results[num].GetComponentInChildren<UITexturePattern>();
			if ((bool)componentInChildren)
			{
				componentInChildren.ChangeTexture(_characters[vehicleId]);
			}
			UISprite component = Results[num].transform.GetChild(2).GetComponent<UISprite>();
			component.enabled = flag;
			component.color = color;
		}
	}

	private float AngleDir(Vector3 pForward, Vector3 pTargetDir, Vector3 pUp)
	{
		Vector3 lhs = Vector3.Cross(pForward, pTargetDir);
		float num = Vector3.Dot(lhs, pUp);
		if ((double)num > 0.0)
		{
			return 1f;
		}
		if ((double)num < 0.0)
		{
			return -1f;
		}
		return 0f;
	}

	public void UpdateSprite(int pIndex, float pPosX, float pDistance, List<UISprite> vSprites, float pPriority, bool pTransformParent)
	{
		UISprite uISprite = vSprites[pIndex];
		uISprite.enabled = true;
		float num = 1f - pDistance / 50f;
		float num2 = Mathf.Abs(m_fPosXLimit);
		float value = pPosX * num2 / (MaxLimit * MaxLimit);
		value = Mathf.Clamp(value, 0f - num2, num2);
		uISprite.alpha = Mathf.Lerp(0.2f, 1f, num);
		if (pTransformParent)
		{
			uISprite.transform.parent.localPosition = new Vector3(value, uISprite.transform.parent.localPosition.y, 0f - num - pPriority);
		}
		else
		{
			uISprite.transform.localPosition = new Vector3(value, uISprite.transform.localPosition.y, 0f - num - pPriority);
		}
	}

	public void UpdatePie(int pIndex, float pPosX, float pDistance)
	{
		UpdateSprite(pIndex, pPosX, pDistance, Pies, 0.2f, false);
	}

	public void UpdateAutolockPie(int pIndex, float pPosX, float pDistance)
	{
		UpdateSprite(pIndex, pPosX, pDistance, AutolockPies, 0.3f, false);
	}

	public void UpdateUFO(float pDistance)
	{
		float y = vProgressionBarTranform.localScale.y;
		float num = (0f - y) * (pDistance / _raceLength);
		num -= vProgressionBarTranform.localPosition.y;
		Transform transform = Ufo.transform;
		Ufo.transform.localPosition = new Vector3(transform.localPosition.x, num - 38f, transform.localPosition.z);
	}

	public void SetUfoToGood()
	{
		Ufo.color = GoodUfoColor;
	}

	public void SetUfoToBad()
	{
		Ufo.color = BadUfoColor;
	}

	public void SetUfoToNormal()
	{
		Ufo.color = Color.white;
	}
}

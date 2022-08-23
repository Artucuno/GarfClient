using System;

[Serializable]
public class GkAIManager : RacingAIManager
{
	protected PathSettings _pathSettings;

	protected BehaviourSettings _behaviourSettings;

	protected EDifficulty _difficulty;

	public override void Init(AIPathHandler pPathModule)
	{
		base.Init(pPathModule);
		_pathSettings = Singleton<GameConfigurator>.Instance.AISettings.PathSettings;
		_behaviourSettings = Singleton<GameConfigurator>.Instance.AISettings.BehaviourSettings;
		_difficulty = Singleton<GameConfigurator>.Instance.Difficulty;
	}

	public override RacingAI CreateAI()
	{
		GkRacingAI gkRacingAI = new GkRacingAI();
		gkRacingAI.OnNeedPath = (Action<GkRacingAI>)Delegate.Combine(gkRacingAI.OnNeedPath, new Action<GkRacingAI>(GivePath));
		return gkRacingAI;
	}

	protected override void CheckPositionOnPath(RacingAI pAi)
	{
		if (!(pAi.CurrentPath is RcFastValuePath))
		{
			return;
		}
		RcFastValuePath rcFastValuePath = (RcFastValuePath)pAi.CurrentPath;
		RcFastValuePathComp point = rcFastValuePath.GetPoint(pAi.CurrentPathIndex);
		if (point is GkFastPathValueComp)
		{
			GkFastPathValueComp gkFastPathValueComp = (GkFastPathValueComp)point;
			float value = gkFastPathValueComp.GetValue();
			Kart kart = (Kart)pAi.Vehicle;
			EITEM item = kart.GetBonusMgr().GetItem(0);
			bool flag = gkFastPathValueComp.ChangePath;
			if (value != 0f)
			{
				if (!gkFastPathValueComp.UseBonus)
				{
					flag = ((item == (EITEM)(int)value) ? true : false);
				}
				else if (item == (EITEM)(int)value)
				{
					if (kart.IsInShortCut || IsMore(_behaviourSettings.ShootChance.GetChance(pAi.Level)))
					{
						((GkRacingAI)pAi).ActivateBonus(kart, gkFastPathValueComp.Behind);
					}
				}
				else
				{
					flag = gkFastPathValueComp.ChangePathIfNoCondition;
				}
			}
			if (!flag)
			{
				return;
			}
			kart.IsInShortCut = false;
			if ((int)pAi.Vehicle.GetArcadeDriftFactor() != 0)
			{
				return;
			}
			if (gkFastPathValueComp.Path != null)
			{
				if (TakeShortcut(pAi.Level))
				{
					_pathModule.SetPathAvailable((RcFastValuePath)pAi.CurrentPath);
					pAi.IdealPath = _pathModule.GetShortcut(gkFastPathValueComp.Path.GetInstanceID());
					pAi.PathPosition = PathPosition.UNDEFINED_POSITION;
					kart.IsInShortCut = true;
				}
			}
			else
			{
				GivePath((GkRacingAI)pAi);
			}
		}
		else
		{
			if (!(point is GkFastPathDriftValueComp))
			{
				return;
			}
			GkFastPathDriftValueComp gkFastPathDriftValueComp = (GkFastPathDriftValueComp)point;
			E_DriftOrientation orientation = gkFastPathDriftValueComp.Orientation;
			if (orientation == E_DriftOrientation.None || IsMore(_behaviourSettings.ShootChance.GetChance(pAi.Level)))
			{
				int num = (int)orientation;
				pAi.Vehicle.SetArcadeDriftFactor(num);
				if (orientation != 0)
				{
					((Kart)pAi.Vehicle).Jump(0f, 0f);
				}
			}
		}
	}

	public void GivePath(GkRacingAI pAi)
	{
		E_PathType pathType = GetPathType(pAi.Level);
		_pathModule.SetPathAvailable((RcFastValuePath)pAi.CurrentPath);
		pAi.IdealPath = _pathModule.GetPath(pathType, pAi.Level);
		pAi.PathPosition = PathPosition.UNDEFINED_POSITION;
	}

	public bool IsMore(int pValue)
	{
		int num = Singleton<RandomManager>.Instance.Next(1, 100);
		return pValue >= num;
	}

	public bool TakeShortcut(E_AILevel pLevel)
	{
		if (Singleton<GameManager>.Instance.GameMode.State != E_GameState.Race)
		{
			return false;
		}
		switch (pLevel)
		{
		case E_AILevel.GOOD:
			if (_difficulty == EDifficulty.EASY)
			{
				return IsMore(_pathSettings.EasyShortcutPathChance.Good);
			}
			if (_difficulty == EDifficulty.NORMAL)
			{
				return IsMore(_pathSettings.NormalShortcutPathChance.Good);
			}
			if (_difficulty == EDifficulty.HARD)
			{
				return IsMore(_pathSettings.HardShortcutPathChance.Good);
			}
			return false;
		case E_AILevel.AVERAGE:
			if (_difficulty == EDifficulty.EASY)
			{
				return IsMore(_pathSettings.EasyShortcutPathChance.Average);
			}
			if (_difficulty == EDifficulty.NORMAL)
			{
				return IsMore(_pathSettings.NormalShortcutPathChance.Average);
			}
			if (_difficulty == EDifficulty.HARD)
			{
				return IsMore(_pathSettings.HardShortcutPathChance.Average);
			}
			return false;
		case E_AILevel.BAD:
			if (_difficulty == EDifficulty.EASY)
			{
				return IsMore(_pathSettings.EasyShortcutPathChance.Bad);
			}
			if (_difficulty == EDifficulty.NORMAL)
			{
				return IsMore(_pathSettings.NormalShortcutPathChance.Bad);
			}
			if (_difficulty == EDifficulty.HARD)
			{
				return IsMore(_pathSettings.HardShortcutPathChance.Bad);
			}
			return false;
		default:
			return false;
		}
	}

	public E_PathType GetPathType(E_AILevel pLevel)
	{
		switch (pLevel)
		{
		case E_AILevel.GOOD:
			if (_difficulty == EDifficulty.EASY)
			{
				return GetPathType(_pathSettings.EasyGoodPathChance);
			}
			if (_difficulty == EDifficulty.NORMAL)
			{
				return GetPathType(_pathSettings.NormalGoodPathChance);
			}
			if (_difficulty == EDifficulty.HARD)
			{
				return GetPathType(_pathSettings.HardGoodPathChance);
			}
			return E_PathType.NONE;
		case E_AILevel.AVERAGE:
			if (_difficulty == EDifficulty.EASY)
			{
				return GetPathType(_pathSettings.EasyAveragePathChance);
			}
			if (_difficulty == EDifficulty.NORMAL)
			{
				return GetPathType(_pathSettings.NormalAveragePathChance);
			}
			if (_difficulty == EDifficulty.HARD)
			{
				return GetPathType(_pathSettings.HardAveragePathChance);
			}
			return E_PathType.NONE;
		case E_AILevel.BAD:
			if (_difficulty == EDifficulty.EASY)
			{
				return GetPathType(_pathSettings.EasyBadPathChance);
			}
			if (_difficulty == EDifficulty.NORMAL)
			{
				return GetPathType(_pathSettings.NormalBadPathChance);
			}
			if (_difficulty == EDifficulty.HARD)
			{
				return GetPathType(_pathSettings.HardBadPathChance);
			}
			return E_PathType.NONE;
		default:
			return E_PathType.NONE;
		}
	}

	private E_PathType GetPathType(Chance pChance)
	{
		int num = Singleton<RandomManager>.Instance.Next(0, 99);
		int good = pChance.Good;
		int average = pChance.Average;
		if (num >= 0 && num < good)
		{
			return E_PathType.GOOD;
		}
		if (num >= good && num < good + average)
		{
			return E_PathType.AVERAGE;
		}
		if (num >= good + average && num < 100)
		{
			return E_PathType.BAD;
		}
		return E_PathType.NONE;
	}

	public void SetBoostStart(GameMode oGameMode)
	{
		Chance chance = _behaviourSettings.BoostStart100ccChance;
		switch (Singleton<GameConfigurator>.Instance.Difficulty)
		{
		case EDifficulty.EASY:
			chance = _behaviourSettings.BoostStart50ccChance;
			break;
		case EDifficulty.NORMAL:
			chance = _behaviourSettings.BoostStart100ccChance;
			break;
		case EDifficulty.HARD:
			chance = _behaviourSettings.BoostStart150ccChance;
			break;
		}
		for (int i = 0; i < 6; i++)
		{
			if (AIs[i] != null && AIs[i].Vehicle.GetControlType() == RcVehicle.ControlType.AI && IsMore(chance.GetChance(AIs[i].Level)))
			{
				oGameMode.SetAdvantage(AIs[i].Vehicle.GetVehicleId(), EAdvantage.BoostStart);
			}
		}
	}
}

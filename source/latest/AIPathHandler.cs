using System.Collections.Generic;
using UnityEngine;

public class AIPathHandler : MonoBehaviour
{
	protected List<RcFastValuePath> _idealGoodPaths = new List<RcFastValuePath>();

	protected List<int> _availableGoodPaths = new List<int>();

	protected List<RcFastValuePath> _idealAveragePaths = new List<RcFastValuePath>();

	protected List<int> _availableAveragePaths = new List<int>();

	protected List<RcFastValuePath> _idealBadPaths = new List<RcFastValuePath>();

	protected List<int> _availableBadPaths = new List<int>();

	protected Dictionary<int, RcFastValuePath> _shortcutPaths = new Dictionary<int, RcFastValuePath>();

	public bool DebugDraw;

	public int AIDebugIndex = 1;

	protected GkAIManager _AIManager
	{
		get
		{
			return ((InGameGameMode)Singleton<GameManager>.Instance.GameMode).AIManager;
		}
	}

	public RcFastValuePath GetLeftBorder(int index)
	{
		return null;
	}

	public RcFastValuePath GetRightBorder(int index)
	{
		return null;
	}

	private void CreateIdealPath(GameObject pGameObject, E_PathType pPathType)
	{
		RcFastValuePath rcFastValuePath = new RcFastValuePath(pGameObject);
		rcFastValuePath.PathType = pPathType;
		rcFastValuePath.SetLoop(true);
		switch (pPathType)
		{
		case E_PathType.GOOD:
			_availableGoodPaths.Add(_idealGoodPaths.Count);
			_idealGoodPaths.Add(rcFastValuePath);
			break;
		case E_PathType.AVERAGE:
			_availableAveragePaths.Add(_idealAveragePaths.Count);
			_idealAveragePaths.Add(rcFastValuePath);
			break;
		case E_PathType.BAD:
			_availableBadPaths.Add(_idealBadPaths.Count);
			_idealBadPaths.Add(rcFastValuePath);
			break;
		case E_PathType.SHORTCUT:
			_shortcutPaths.Add(pGameObject.GetInstanceID(), rcFastValuePath);
			break;
		}
	}

	public RcFastPath GetShortcut(int pId)
	{
		return _shortcutPaths[pId];
	}

	public int GetPathIndex(RcFastValuePath pPath)
	{
		switch (pPath.PathType)
		{
		case E_PathType.GOOD:
			if (pPath != null)
			{
				return _idealGoodPaths.IndexOf(pPath);
			}
			return -1;
		case E_PathType.AVERAGE:
			if (pPath != null)
			{
				return _idealAveragePaths.IndexOf(pPath);
			}
			return -1;
		case E_PathType.BAD:
			if (pPath != null)
			{
				return _idealBadPaths.IndexOf(pPath);
			}
			return -1;
		default:
			return -1;
		}
	}

	public void SetPathAvailable(RcFastValuePath pPreviousPath)
	{
		switch (pPreviousPath.PathType)
		{
		case E_PathType.GOOD:
			if (pPreviousPath != null)
			{
				int num3 = _idealGoodPaths.IndexOf(pPreviousPath);
				if (num3 >= 0)
				{
					_availableGoodPaths.Add(num3);
				}
			}
			break;
		case E_PathType.AVERAGE:
			if (pPreviousPath != null)
			{
				int num2 = _idealAveragePaths.IndexOf(pPreviousPath);
				if (num2 >= 0)
				{
					_availableAveragePaths.Add(num2);
				}
			}
			break;
		case E_PathType.BAD:
			if (pPreviousPath != null)
			{
				int num = _idealBadPaths.IndexOf(pPreviousPath);
				if (num >= 0)
				{
					_availableBadPaths.Add(num);
				}
			}
			break;
		}
	}

	public RcFastPath GetPath(E_PathType pPathType, E_AILevel pAILevel)
	{
		switch (pPathType)
		{
		case E_PathType.GOOD:
			if (_availableGoodPaths.Count > 0)
			{
				int num3 = _availableGoodPaths[Singleton<RandomManager>.Instance.Next(_availableGoodPaths.Count - 1)];
				_availableGoodPaths.Remove(num3);
				return _idealGoodPaths[num3];
			}
			return GetPath(E_PathType.AVERAGE, pAILevel);
		case E_PathType.AVERAGE:
			if (_availableAveragePaths.Count > 0)
			{
				int num2 = _availableAveragePaths[Singleton<RandomManager>.Instance.Next(_availableAveragePaths.Count - 1)];
				_availableAveragePaths.Remove(num2);
				return _idealAveragePaths[num2];
			}
			switch (pAILevel)
			{
			case E_AILevel.GOOD:
				return GetPath(E_PathType.GOOD, pAILevel);
			case E_AILevel.BAD:
				return GetPath(E_PathType.BAD, pAILevel);
			case E_AILevel.AVERAGE:
				switch (Singleton<GameConfigurator>.Instance.Difficulty)
				{
				case EDifficulty.EASY:
					return GetPath(E_PathType.BAD, pAILevel);
				case EDifficulty.NORMAL:
					return GetPath(E_PathType.BAD, pAILevel);
				case EDifficulty.HARD:
					return GetPath(E_PathType.GOOD, pAILevel);
				}
				break;
			}
			return null;
		case E_PathType.BAD:
			if (_availableBadPaths.Count > 0)
			{
				int num = _availableBadPaths[Singleton<RandomManager>.Instance.Next(_availableBadPaths.Count - 1)];
				_availableBadPaths.Remove(num);
				return _idealBadPaths[num];
			}
			return GetPath(E_PathType.AVERAGE, pAILevel);
		default:
			return null;
		}
	}

	public RcFastPath GetFirstPath(E_PathType pPathType)
	{
		switch (pPathType)
		{
		case E_PathType.GOOD:
			if (_idealGoodPaths.Count > 0)
			{
				return _idealGoodPaths[0];
			}
			break;
		case E_PathType.AVERAGE:
			if (_idealAveragePaths.Count > 0)
			{
				return _idealAveragePaths[0];
			}
			break;
		case E_PathType.BAD:
			if (_idealBadPaths.Count > 0)
			{
				return _idealBadPaths[0];
			}
			break;
		}
		return null;
	}

	protected virtual void StartIdealPath()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (!(child != null))
			{
				continue;
			}
			GameObject gameObject = child.gameObject;
			if (gameObject != null)
			{
				PathType component = gameObject.GetComponent<PathType>();
				if (component != null)
				{
					CreateIdealPath(gameObject, component.Type);
				}
			}
		}
	}

	public virtual void InitIdealPaths(RacingAI pAI, int pIndex)
	{
		E_PathType pathType = _AIManager.GetPathType(pAI.Level);
		pAI.IdealPath = GetPath(pathType, pAI.Level);
	}

	protected virtual void Start()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		StartIdealPath();
		_AIManager.Init(this);
		for (int i = 0; i < 6; i++)
		{
			GameObject player = Singleton<GameManager>.Instance.GameMode.GetPlayer(i);
			if (player != null)
			{
				E_AILevel pLevel = E_AILevel.GOOD;
				if (i == 5)
				{
					pLevel = E_AILevel.BAD;
				}
				else if (i > 2)
				{
					pLevel = E_AILevel.AVERAGE;
				}
				RegisterController(player, pLevel);
			}
		}
	}

	public void RegisterController(GameObject pPlayer, E_AILevel pLevel)
	{
		RcVirtualController componentInChildren = pPlayer.GetComponentInChildren<RcVirtualController>();
		_AIManager.RegisterVirtualController(componentInChildren, pLevel);
	}

	public bool UpdatePositionOnPath(RacingAI pAI)
	{
		RcFastPath currentPath = pAI.CurrentPath;
		Vector3 position = pAI.Vehicle.GetPosition();
		PathPosition _pathPosition = pAI.PathPosition;
		currentPath.UpdatePathPosition(ref _pathPosition, position, 10, 3, false, currentPath.IsLooping());
		bool result = false;
		if (pAI.PathPosition.index != _pathPosition.index)
		{
			result = true;
		}
		pAI.PathPosition = _pathPosition;
		return result;
	}

	protected virtual void OnDrawGizmos()
	{
		if (Application.isPlaying && Application.isEditor)
		{
			_AIManager.DebugDrawGizmos(DebugDraw, AIDebugIndex);
		}
	}
}

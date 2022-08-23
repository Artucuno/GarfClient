using System.Collections.Generic;
using UnityEngine;

public class DebugAiGameMode : InGameGameMode
{
	public override void CreatePlayers()
	{
		DebugMgr.Instance.LoadDefaultPlayer(0, this);
	}

	private int GetRandomStartPos(List<int> pAvailableStartPos)
	{
		int num = pAvailableStartPos[Random.Range(0, pAvailableStartPos.Count)];
		pAvailableStartPos.Remove(num);
		return num;
	}
}

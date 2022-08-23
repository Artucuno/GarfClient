using System.Collections.Generic;
using UnityEngine;

internal class RankComparer : IComparer<Tuple<int, GameObject>>
{
	private RacingAIManager _aiManager;

	public RankComparer(RacingAIManager pAIManager)
	{
		_aiManager = pAIManager;
	}

	public int Compare(GameObject pFirstGO, GameObject pSecondGO)
	{
		int aIIndex = pFirstGO.GetComponentInChildren<RcVirtualController>().AIIndex;
		int aIIndex2 = pSecondGO.GetComponentInChildren<RcVirtualController>().AIIndex;
		RacingAI racingAI = _aiManager.AIs[aIIndex];
		RacingAI racingAI2 = _aiManager.AIs[aIIndex2];
		if (racingAI.Level == racingAI2.Level)
		{
			RcVehicleRaceStats componentInChildren = pFirstGO.GetComponentInChildren<RcVehicleRaceStats>();
			RcVehicleRaceStats componentInChildren2 = pSecondGO.GetComponentInChildren<RcVehicleRaceStats>();
			if (componentInChildren.GetRank() < componentInChildren2.GetRank())
			{
				return -1;
			}
			return 1;
		}
		RcVehicleRaceStats componentInChildren3 = pFirstGO.GetComponentInChildren<RcVehicleRaceStats>();
		RcVehicleRaceStats componentInChildren4 = pFirstGO.GetComponentInChildren<RcVehicleRaceStats>();
		if ((float)racingAI.Level * componentInChildren3.GetDistToEndOfRace() < (float)racingAI2.Level * componentInChildren4.GetDistToEndOfRace())
		{
			return -1;
		}
		return 1;
	}

	public int Compare(Tuple<int, GameObject> x, Tuple<int, GameObject> y)
	{
		return Compare(x.Item2, y.Item2);
	}
}

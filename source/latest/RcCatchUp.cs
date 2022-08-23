using System;
using System.Collections.Generic;
using UnityEngine;

public class RcCatchUp : MonoBehaviour
{
	[Serializable]
	public class CatchUpPair
	{
		public float distance;

		public float factor;

		public float aiFactor;

		public CatchUpPair(float dist, float factor, float aiFactor)
		{
			distance = dist;
			this.factor = factor;
			this.aiFactor = aiFactor;
		}
	}

	public bool m_bIsActivate = true;

	public List<CatchUpPair> m_oTabCatchUp;

	public RcCatchUp()
	{
		m_oTabCatchUp = new List<CatchUpPair>();
	}

	public void AddFactor(float dist, float factor, float aiFactor)
	{
		m_oTabCatchUp.Add(new CatchUpPair(dist, factor, aiFactor));
	}

	public void ComputeCatchUp(RcVehicle _pVehicle, float _distToEndOfRace, float _refDistToEndOfRace)
	{
		float num = 0f;
		if (m_bIsActivate && m_oTabCatchUp.Count > 0)
		{
			float num2 = _refDistToEndOfRace - _distToEndOfRace;
			CatchUpPair catchUpPair = m_oTabCatchUp[0];
			int i;
			for (i = 0; i < m_oTabCatchUp.Count; i++)
			{
				catchUpPair = m_oTabCatchUp[i];
				if (num2 < catchUpPair.distance)
				{
					break;
				}
			}
			if (i == 0 || i == m_oTabCatchUp.Count)
			{
				num = ((!_pVehicle.IsAutoPilot()) ? catchUpPair.factor : catchUpPair.aiFactor);
			}
			else
			{
				CatchUpPair catchUpPair2 = m_oTabCatchUp[i - 1];
				num = ((!_pVehicle.IsAutoPilot()) ? RcUtils.LinearInterpolation(catchUpPair2.distance, catchUpPair2.factor, catchUpPair.distance, catchUpPair.factor, num2) : RcUtils.LinearInterpolation(catchUpPair2.distance, catchUpPair2.aiFactor, catchUpPair.distance, catchUpPair.aiFactor, num2));
			}
		}
		_pVehicle.SetTempHandicap(0f - num);
	}
}

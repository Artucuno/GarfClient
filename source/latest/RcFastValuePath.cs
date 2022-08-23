using UnityEngine;

public class RcFastValuePath : RcFastPath
{
	protected E_PathType _pathType;

	protected RcFastValuePathComp[] m_pPointValues;

	public E_PathType PathType
	{
		get
		{
			return _pathType;
		}
		set
		{
			_pathType = value;
		}
	}

	public RcFastValuePath(int _arraySize)
		: base(_arraySize)
	{
		m_pPointValues = new RcFastValuePathComp[_arraySize];
		for (int i = 0; i < _arraySize; i++)
		{
			m_pPointValues[i] = null;
		}
	}

	public RcFastValuePath(GameObject slowPath)
		: base(slowPath)
	{
		m_pPointValues = new RcFastValuePathComp[m_iNbPoints];
		if (m_iNbPoints <= 0)
		{
			return;
		}
		for (int i = 0; i < m_iNbPoints; i++)
		{
			RcFastValuePathComp component = slowPath.transform.GetChild(i).gameObject.GetComponent<RcFastValuePathComp>();
			if ((bool)component)
			{
				m_pPointValues[i] = component;
			}
		}
	}

	public RcFastValuePathComp GetPoint(int pInd)
	{
		if (m_iNbPoints <= 0)
		{
			return null;
		}
		return m_pPointValues[pInd];
	}

	public float GetPointValue(int pInd)
	{
		if (m_iNbPoints <= 0)
		{
			return 0f;
		}
		return m_pPointValues[pInd].GetValue();
	}

	public int GetIntPointValue(int pInd)
	{
		if (m_iNbPoints <= 0)
		{
			return 0;
		}
		return m_pPointValues[pInd].GetIntValue();
	}

	public override void SetSize(int _newSize, bool _updateLengthsAndDistances)
	{
		for (int i = m_iNbPoints; i < _newSize; i++)
		{
			m_pPointValues[i] = null;
		}
		base.SetSize(_newSize, _updateLengthsAndDistances);
	}
}

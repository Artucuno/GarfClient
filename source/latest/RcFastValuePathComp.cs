using UnityEngine;

public class RcFastValuePathComp : MonoBehaviour
{
	public float m_fValue;

	public RcFastValuePathComp()
	{
		m_fValue = 0f;
	}

	public float GetValue()
	{
		return m_fValue;
	}

	public int GetIntValue()
	{
		return (int)m_fValue;
	}
}

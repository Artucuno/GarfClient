using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
	public Kart Player;

	private Transform m_pTransform;

	public float DistThreshold = 30f;

	private AudioSource m_pAudio;

	private void Start()
	{
		Player = null;
		m_pTransform = base.transform;
		m_pAudio = GetComponent<AudioSource>();
	}

	private void Update()
	{
		if (!(Player != null))
		{
			return;
		}
		float sqrMagnitude = (m_pTransform.position - Player.Transform.position).sqrMagnitude;
		if (sqrMagnitude < DistThreshold)
		{
			if (!m_pAudio.enabled)
			{
				m_pAudio.enabled = true;
			}
		}
		else if (m_pAudio.enabled)
		{
			m_pAudio.enabled = false;
		}
	}
}

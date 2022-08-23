public class KartKinematicPhysic : RcKinematicPhysic
{
	public bool m_bNoSurfaceResponseInBoost = true;

	private bool m_bBoosting;

	private bool m_bParfume;

	public void SetBoosting(bool bBoosting)
	{
		m_bBoosting = bBoosting;
	}

	public void SetParfume(bool bParfume)
	{
		m_bParfume = bParfume;
	}

	public override bool IsGoingTooFast()
	{
		if (m_bNoSurfaceResponseInBoost && (m_bBoosting || m_bParfume))
		{
			return false;
		}
		return base.IsGoingTooFast();
	}
}

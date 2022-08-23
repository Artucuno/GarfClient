public class RaceScoreData
{
	private int _raceScore;

	private int _previousPosition;

	private int _previousPositionForReset;

	private int _racePosition;

	private bool _isAI;

	private int _kartIndex;

	private int _previousRaceScore;

	public int RaceScore
	{
		get
		{
			return _raceScore;
		}
	}

	public bool IsAI
	{
		get
		{
			return _isAI;
		}
		set
		{
			_isAI = value;
		}
	}

	public int RacePosition
	{
		get
		{
			return _racePosition;
		}
		set
		{
			_racePosition = value;
		}
	}

	public int KartIndex
	{
		get
		{
			return _kartIndex;
		}
	}

	public int PreviousRaceScore
	{
		get
		{
			return _previousRaceScore;
		}
	}

	public int PreviousRacePosition
	{
		get
		{
			return _previousPosition;
		}
	}

	public RaceScoreData(int pKartIndex, bool pIsAI)
	{
		_kartIndex = pKartIndex;
		_raceScore = 0;
		_isAI = pIsAI;
		_previousPosition = 0;
		_racePosition = 0;
		_previousRaceScore = 0;
		_previousPositionForReset = 0;
	}

	public virtual void RestartRace()
	{
		_raceScore = _previousRaceScore;
		_racePosition = _previousPosition;
		_previousPosition = _previousPositionForReset;
	}

	public void ResetRace()
	{
		_raceScore = 0;
		_previousRaceScore = 0;
	}

	public virtual bool SetRaceScore(int iScore)
	{
		if (_raceScore <= 0)
		{
			_previousRaceScore = _raceScore;
			_raceScore += iScore;
			return true;
		}
		return false;
	}

	public void SetRacePosition(int pPosition)
	{
		_previousPositionForReset = _previousPosition;
		_previousPosition = _racePosition;
		_racePosition = pPosition;
	}
}

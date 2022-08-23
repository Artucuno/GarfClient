public class ChampionShipScoreData : RaceScoreData
{
	private int _championshipScore;

	private int _previousChampionshipPosition;

	private int _previousChampionshipPositionForReset;

	private int _championshipPosition;

	private int _previousChampionshipScore;

	public int ChampionshipScore
	{
		get
		{
			return _championshipScore;
		}
	}

	public int ChampionshipPosition
	{
		get
		{
			return _championshipPosition;
		}
		set
		{
			_championshipPosition = value;
		}
	}

	public int PreviousChampionshipScore
	{
		get
		{
			return _previousChampionshipScore;
		}
		set
		{
			_previousChampionshipScore = value;
		}
	}

	public ChampionShipScoreData(int pKartIndex, bool pIsAI)
		: base(pKartIndex, pIsAI)
	{
		_championshipScore = 0;
		_previousChampionshipPosition = 0;
		_championshipPosition = 0;
		_previousChampionshipPositionForReset = 0;
	}

	public override void RestartRace()
	{
		base.RestartRace();
		_championshipScore = _previousChampionshipScore;
		_championshipPosition = _previousChampionshipPosition;
		_previousChampionshipPosition = _previousChampionshipPositionForReset;
	}

	public override bool SetRaceScore(int iScore)
	{
		if (base.SetRaceScore(iScore))
		{
			PreviousChampionshipScore = _championshipScore;
			_championshipScore += iScore;
			return true;
		}
		return false;
	}

	public void SetChampionShipPosition(int pPosition)
	{
		_previousChampionshipPositionForReset = _previousChampionshipPosition;
		_previousChampionshipPosition = _championshipPosition;
		_championshipPosition = pPosition;
	}
}

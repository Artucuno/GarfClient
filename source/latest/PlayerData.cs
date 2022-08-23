using UnityEngine;

public class PlayerData
{
	private ECharacter _character;

	private ECharacter _kart;

	private string _custom;

	private string _hat;

	private int _kartIndex;

	private int _nbStars;

	private string _pseudo;

	private Color _characColor = Color.yellow;

	public ECharacter Character
	{
		get
		{
			return _character;
		}
		set
		{
			_character = value;
		}
	}

	public ECharacter Kart
	{
		get
		{
			return _kart;
		}
		set
		{
			_kart = value;
		}
	}

	public string Custom
	{
		get
		{
			return _custom;
		}
		set
		{
			_custom = value;
		}
	}

	public string Hat
	{
		get
		{
			return _hat;
		}
		set
		{
			_hat = value;
		}
	}

	public int KartIndex
	{
		get
		{
			return _kartIndex;
		}
		set
		{
			_kartIndex = value;
		}
	}

	public int NbStars
	{
		get
		{
			return _nbStars;
		}
		set
		{
			_nbStars = value;
		}
	}

	public string Pseudo
	{
		get
		{
			return _pseudo;
		}
		set
		{
			_pseudo = value;
		}
	}

	public Color CharacColor
	{
		get
		{
			return _characColor;
		}
		set
		{
			_characColor = value;
		}
	}

	public PlayerData(ECharacter pCharacter, ECharacter pKart, string pCustom, string pHat, int iNbStars, string sPseudo, Color cColor)
	{
		_character = pCharacter;
		_kart = pKart;
		_custom = pCustom;
		_hat = pHat;
		_nbStars = iNbStars;
		_pseudo = sPseudo;
		_characColor = cColor;
	}
}

using UnityEngine;
using System.Collections;
using System;
using CodeStage.AntiCheat.ObscuredTypes;

[Serializable]
public class UserData
{
	private static readonly string UserDataFile = "user.data";

	#region Singleton

	private static UserData _instance;
	
	public static UserData Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new UserData();

				if (Helper.Load<UserData>(UserDataFile, ref _instance))
				{
					_instance.OnLoaded();
				}
				else
				{
					_instance.Reset();
				}
			}
			
			return _instance;
		}
	}

	#endregion

	private int _version = 1;

	// The current coin
	private ObscuredInt _coin;

	// The current mana
	private int _mana;

	// The current level
	private int _level = Settings.InitMap;

	// The current map
	private int _map = Settings.InitMap;

	// The current undo
	private int _undo = -1;
	
	// The current hint
	private int _hint = -1;
	
	// The current hammer
	private int _hammer = -1;
	
	// The current deploy
	private int _deploy = -1;

	// True if BGM on
	private bool _isBGMOn;
	
	// True if SFX on
	private bool _isSFXOn;

	// The mana time when start countdown
	private long _manaTime;

	// True if just unlock new level
	private bool _isNewLevel;

	// True if play next level
	private bool _isNextLevel;

	// Positive if just unlock new booster
	private int _newBooster = -1;

	// The ramining undo tutorial
	private int _remainingUndoTutorial = Settings.UndoTutorialTotal;
	
	// True if played hint tutorial
	private bool _isPlayedHintTutorial;
	
	// True if played hammer tutorial
	private bool _isPlayedHammerTutorial;
	
	// True if played deploy tutorial
	private bool _isPlayedDeployTutorial;

	//
	private bool _isPlayedCutscene1;
	private bool _isPlayedCutscene2;
	private bool _isPlayedCutscene3;
	private bool _isPlayedCutscene4;

	// True if winned a level
	private bool[] _isWinned = new bool[Settings.MapCount];

	private int _showRateGameTimes = 0;
	private int	_showLoginFBTimes = 0;

	// True if dirty
	[NonSerialized]
	private bool _isDirty;


	public ObscuredInt Coin
	{
		get { return _coin; }
		set { _coin = value; _isDirty = true; }
	}

	public int Mana
	{
		get { return _mana; }
		set { _mana = Mathf.Clamp(value, 0, Settings.MaxMana); _isDirty = true; }
	}

	public int ShowRateGameTime
	{
		get {return _showRateGameTimes;}
		set {_showRateGameTimes = value; _isDirty = true;}
	}

	public int ShowLoginFBTimes
	{
		get { return _showLoginFBTimes;}
		set { _showLoginFBTimes = value; _isDirty = true;}
	}

	/// <summary>
	/// Gets or sets the current level (one-based). (Level cao nhat unlocked)
	/// </summary>
	public int Level
	{
		get { return _level; }
		set { _level = Mathf.Clamp(value, 1, Settings.MapCount); _isDirty = true; }
	}

	/// <summary>
	/// Gets or sets the current map (one-based). (Level dang choi)
	/// </summary>
	public int Map
	{
		get { return _map; }
		set { _map = Mathf.Clamp(value, 1, _level); _isDirty = true; }
	}
	
	public int Undo
	{
		get { return _undo; }
		set { _undo = value; _isDirty = true; }
	}
	
	public int Hint
	{
		get { return _hint; }
		set { _hint = value; _isDirty = true; }
	}

	public int Hammer
	{
		get { return _hammer; }
		set { _hammer = value; _isDirty = true; }
	}

	public int Deploy
	{
		get { return _deploy; }
		set { _deploy = value; _isDirty = true; }
	}

	public bool BGMOn
	{
		get { return _isBGMOn; }
		set { _isBGMOn = value; _isDirty = true; }
	}

	public bool SFXOn
	{
		get { return _isSFXOn; }
		set { _isSFXOn = value; _isDirty = true; }
	}

	public long ManaTime
	{
		get { return _manaTime; }
		set { _manaTime = value; _isDirty = true; }
	}

	public bool NewLevel
	{
		get { return _isNewLevel; }
		set { _isNewLevel = value; _isDirty = true; }
	}
	
	public bool NextLevel
	{
		get { return _isNextLevel; }
		set { _isNextLevel = value; _isDirty = true; }
	}

	public int NewBooster
	{
		get { return _newBooster; }
		set { _newBooster = value; _isDirty = true; }
	}

	public int RemainingUndoTutorial
	{
		get { return _remainingUndoTutorial; }
		set { _remainingUndoTutorial = value; _isDirty = true; }
	}
	
	public bool PlayedHintTutorial
	{
		get { return _isPlayedHintTutorial; }
		set { _isPlayedHintTutorial = value; _isDirty = true; }
	}
	
	public bool PlayedHammerTutorial
	{
		get { return _isPlayedHammerTutorial; }
		set { _isPlayedHammerTutorial = value; _isDirty = true; }
	}
	
	public bool PlayedDeployTutorial
	{
		get { return _isPlayedDeployTutorial; }
		set { _isPlayedDeployTutorial = value; _isDirty = true; }
	}

	public bool PlayedCutscene1
	{
		get { return _isPlayedCutscene1; }
		set { _isPlayedCutscene1 = value; _isDirty = true; }
	}

	public bool PlayedCutscene2
	{
		get { return _isPlayedCutscene2; }
		set { _isPlayedCutscene2 = value; _isDirty = true; }
	}

	public bool PlayedCutscene3
	{
		get { return _isPlayedCutscene3; }
		set { _isPlayedCutscene3 = value; _isDirty = true; }
	}

	public bool PlayedCutscene4
	{
		get { return _isPlayedCutscene4; }
		set { _isPlayedCutscene4 = value; _isDirty = true; }
	}

	public int GetBoosterQuantity(BoosterType type)
	{
		if (type == BoosterType.Undo)
		{
			return _undo;
		}

		if (type == BoosterType.Hint)
		{
			return _hint;
		}
		
		if (type == BoosterType.Hammer)
		{
			return _hammer;
		}
		
		if (type == BoosterType.Deploy)
		{
			return _deploy;
		}

		return 0;
	}

	public void SetBoosterQuantity(BoosterType type, int quantity)
	{
		if (type == BoosterType.Undo)
		{
			_undo = quantity;
		}
		else if (type == BoosterType.Hint)
		{
			_hint = quantity;
		}
		else if (type == BoosterType.Hammer)
		{
			_hammer = quantity;
		}
		else if (type == BoosterType.Deploy)
		{
			_deploy = quantity;
		}

		_isDirty = true;
	}

	public void UnlockBooster(BoosterType type)
	{
		if (type == BoosterType.Undo)
		{
			_undo = Settings.unlockBoosterQuantities[type.ToInt()];
		}
		else if (type == BoosterType.Hint)
		{
			_hint = Settings.unlockBoosterQuantities[type.ToInt()];
		}
		else if (type == BoosterType.Hammer)
		{
			_hammer = Settings.unlockBoosterQuantities[type.ToInt()];
		}
		else if (type == BoosterType.Deploy)
		{
			_deploy = Settings.unlockBoosterQuantities[type.ToInt()];
		}

		_isDirty = true;
	}
	
	public bool IsBoosterUnlocked(BoosterType type)
	{
		if (type == BoosterType.Undo)
		{
			return _undo >= 0;
		}
		
		if (type == BoosterType.Hint)
		{
			return _hint >= 0;
		}
		
		if (type == BoosterType.Hammer)
		{
			return _hammer >= 0;
		}

		if (type == BoosterType.Deploy)
		{
			return _deploy >= 0;
		}

		return false;
	}

	/// <summary>
	/// Check if winned the specified level (one-based).
	/// </summary>
	public bool IsWinned(int level)
	{
		return _isWinned[level - 1];
	}

	public void SetWinned(int level)
	{
		_isWinned[level - 1] = true;

		_isDirty = true;
	}

	public void SetWinned(int from, int to)
	{
		for (int i = from - 1; i < to; i++)
		{
			_isWinned[i] = true;
		}

		_isDirty = true;
	}

	void OnLoaded()
	{
		if (_mana >= Settings.MaxMana)
		{
			_manaTime = 0;
		}
	}

	public void Reset()
	{
		// Reset coin
		_coin = Settings.InitCoin;

		// Reset mana
		_mana = Settings.InitMana;

		// Reset level
		_level = Settings.InitMap;

		// Reset map
		_map = Settings.InitMap;

		// Reset undo
		_undo = -1;

		// Reset hint
		_hint = -1;
		
		// Reset hammer
		_hammer = -1;
		
		// Reset deploy
		_deploy = -1;

		// Reset BGM
		_isBGMOn = true;

		// Reset SFX
		_isSFXOn = true;

		// Reset mana time
		_manaTime = 0;

		// Reset new level
		_isNewLevel = false;

		// Reset next level
		_isNextLevel = false;

		// Reset new booster
		_newBooster = -1;

		// Reset remaining undo tutorial
		_remainingUndoTutorial = Settings.UndoTutorialTotal;
		
		// Reset hint tutorial
		_isPlayedHintTutorial = false;

		// Reset hammer tutorial
		_isPlayedHammerTutorial = false;

		// Reset deploy tutorial
		_isPlayedDeployTutorial = false;

		//
		_isPlayedCutscene1 = false;
		_isPlayedCutscene2 = false;
		_isPlayedCutscene3 = false;
		_isPlayedCutscene4 = false;

		// Reset winned
		for (int i = 0; i < _isWinned.Length; i++)
		{
			_isWinned[i] = false;
		}

		// Set dirty
		_isDirty = true;
	}
	
	public bool Save()
	{
		if (Helper.Save<UserData>(this, UserDataFile))
		{
			_isDirty = false;

			return true;
		}

		return false;
	}

	public void Update()
	{
		if (_isDirty)
		{
			if (Helper.Save<UserData>(this, UserDataFile))
			{
				_isDirty = false;
			}
		}
	}

	public override string ToString()
	{
		return string.Format("[UserData: Version={0}, Coin={1}, Mana={2}, Level={3}, Map={4}, Undo={5}, Hint={6}, Hammer={7}, Deploy={8}, BGM={9}, SFX={10}, ManaTime={11}]",
			_version, Coin, Mana, Level, Map, Undo, Hint, Hammer, Deploy, BGMOn, SFXOn, DateTime.FromFileTime(_manaTime));
	}
}

using UnityEngine;
using System.Collections;

public static class Settings
{
	public static readonly string MainMenuScene     = "MainMenuScene";
	public static readonly string MapSelectionScene = "MapSelectionScene";
	public static readonly string MainGameScene     = "MainGameScene";
	
	public static readonly string NoInternetConnection = "No internet connection!";
	
	public static readonly string InviteTitle   = "Epic Puzzle";
	public static readonly string InviteMessage = "Play with me!";
	public static readonly string InviteFailed  = "Invite failed!";
	
	public static readonly string AskManaTitle    = "Request a mana from your friend";
	public static readonly string AskManaMessage  = "Give me a mana";
	public static readonly string AskManaFailed   = "Request mana failed!";
	public static readonly string SendManaTitle   = "Send a mana to your friend";
	public static readonly string SendManaMessage = "Take this mana";

	public static readonly string SendCoinByInviteTitle   = "Bonus coin";
	public static readonly string SendCoinByInviteMessage = "{0} coins!";

	public static readonly string SendManaByInviteTitle   = "Bonus mana";
	public static readonly string SendManaByInviteMessage = "{0} manas!";

	public static readonly string CannotJumpText = "There're no tile to jump.\nIt's end!";
	public static readonly string CannotJumpBackwardText = "Buck can't jump backwards, across or diagonal";

	public static readonly string LongJumpTutorialText = "Buck can make a long jump.\nTry to jump to all tiles";
	public static readonly string JumpTutorialText = "Buck can jump ahead, left and right\nbut can not jump backward or diagonal.";

	public static readonly string LeaderboardFailed  = "Leaderboard failed!";

	public static readonly string QuitConfirm   = "Are you sure to quit?";
	public static readonly string ReplayConfirm = "Are you sure to replay?";
	public static readonly string QuitGameConfirm = "Are you want to quit game?";
	public static readonly string CannotUndo    = "You can't undo in this case!";
	public static readonly string CannotHint    = "We can't hint in this case!";

	public static readonly int InitMap  = 1;
	public static readonly int MapCount = 99;

	public static readonly int InitMana = 17;
	public static readonly int MaxMana 	= 17;
	public static readonly float ManaDelayTime = 15 * 60f;

	public static readonly int InitCoin      = 1000;
	public static readonly int CoinByInvite  = 80;
	public static readonly int ManaByInvite  = 2;
	private static readonly int CoinToBuy1Mana = 40;
	public static readonly int CoinToWatchVideo = 40;
	public static readonly int ManaToWatchVideo = 1;
	
	public static readonly int UndoTutorialTotal   = 2;
	public static readonly float HintTutorialDelay = 20.0f;
	public static readonly int HammerTutorialLevel = 16;
	public static readonly int DeployTutorialLevel = 34;

	public static int CloneNumber = 1;

	public static int CoinToBuyFullMana
	{
		get
		{
			return (MaxMana - UserData.Instance.Mana) * CoinToBuy1Mana;
		}
	}

	/* Boosters */

	public static readonly int[] unlockBoosterLevels =
	{
		5,	// Undo
		8,	// Hint
		15,	// Hammer
		33,	// Deploy
	};

	public static readonly string[] unlockBoosterMessages =
	{
		"You unlocked Undo booster",
		"You unlocked Hint booster",
		"You unlocked Hammer booster",
		"You unlocked Deploy booster"
	};

	public static readonly int[] unlockBoosterQuantities =
	{
		10,	// Undo
		5,	// Hint
		3,	// Hammer
		3,	// Deploy
	};

	public static readonly string[] boosterTitles =
	{
		"Undo",
		"Hint",
		"Hammer",
		"Deploy",
	};

	public static readonly string[] boosterDescriptions =
	{
		"Use this to undo one step!",
		"Use this to see next right step!",
		"Use this to destroy any Foothold!",
		"Use this to deploy one Foothold!"
	};
	
	public static readonly int[] buyBoosterCoins =
	{
		200,	// Undo
		200,	// Hint
		200,	// Hammer
		200,	// Deploy
	};

	public static readonly int[] buyBoosterQuantities =
	{
		10,	// Undo
		3,	// Hint
		1,	// Hammer
		1,	// Deploy
	};

	/// <summary>
	/// Get bonus coins of the specified level (one-based).
	/// </summary>
	public static int GetBonusCoins(int level)
	{
		return 100;
	}
}

#if UNITY_EDITOR
#define SHOW_DEBUG
#endif

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using Facebook.Unity;

public class GameManager : SimpleSingleton<GameManager>, IBoardEventListener, IBoosterEventListener
{
	private static readonly int giuIm = Animator.StringToHash("GiuIm");

	private static readonly Vector3 defaultScale       = Vector3.one;
	private static readonly Vector3 boosterHandOffset  = new Vector3(-1.0f, 1.0f, 0);
	private static readonly Vector3 boosterHandOffset2 = new Vector3(-0.7f, -0.6f, 0);

	private static readonly float dummyMoveSpeed 	   = 30.0f; // Units per second
	private static readonly float minDummyMoveDuration = 0.1f;
	private static readonly float maxDummyMoveDuration = 0.2f;

	private static readonly float circleDiameter = 2.0f;
	private static readonly float squareSize     = 2.0f;

	private static readonly float transitionDuration  = 0.5f;

	private static readonly float highlightDuration   = 0.25f;
	private static readonly float unhighlightDuration = 0.25f;
	private static readonly float handFadeDuration    = 0.25f;
	private static readonly float handDelay		      = 0.25f;
	private static readonly float handMoveDuration    = 1.0f;

	private static readonly float boosterHighlightDelay    = 0.2f;
	private static readonly float boosterHighlightDuration = 0.25f;

	/// <summary>
	/// The canvas.
	/// </summary>
	public Canvas canvas;

	/// <summary>
	/// The zones.
	/// </summary>
	public ZoneSetting[] zones = new ZoneSetting[3];

	/// <summary>
	/// The animal prefab.
	/// </summary>
	public GameObject animalPrefab;

	/// <summary>
	/// The hammer prefab.
	/// </summary>
	public GameObject hammerPrefab;

	/// <summary>
	/// The unlock booster prefab.
	/// </summary>
	public GameObject unlockBoosterPrefab;

	/// <summary>
	/// The hand prefab.
	/// </summary>
	public GameObject handPrefab;

	/// <summary>
	/// The help prefab.
	/// </summary>
	public GameObject helpPrefab;

	/// <summary>
	/// The no more mana popup prefab.
	/// </summary>
	public GameObject noMoreManaPopupPrefab;

	/// <summary>
	/// The cannot jump backward.
	/// </summary>
	public Sprite cannotJumpBackward;

	/// <summary>
	/// The map.
	/// </summary>
	public GameObject map;

	/// <summary>
	/// The map overlay.
	/// </summary>
	public MapOverlay mapOverlay;

	/// <summary>
	/// The start transition.
	/// </summary>
	public InOutSprite startTransition;

	/// <summary>
	/// The tutorial highlight.
	/// </summary>
	public InOutSprite tutorialHighlight;

	/// <summary>
	/// The level info.
	/// </summary>
	public GameObject levelInfo;

	/// <summary>
	/// The left controls.
	/// </summary>
	public GameObject leftControls;

	/// <summary>
	/// The right controls.
	/// </summary>
	public GameObject rightControls;

	/// <summary>
	/// The boosters.
	/// </summary>
	public GameObject boosters;
	
	/// <summary>
	/// The dummy deploy.
	/// </summary>
	public GameObject dummyDeploy;

	/// <summary>
	/// The dummy hammer.
	/// </summary>
	public GameObject dummyHammer;

	/// <summary>
	/// The pause popup.
	/// </summary>
	public PausePopupScript pausePopup;
	
	/// <summary>
	/// The win popup.
	/// </summary>
	public WinPopupScript winPopup;
	
	/// <summary>
	/// The lose popup.
	/// </summary>
	public LosePopupScript losePopup;

	/// <summary>
	/// The buy booster popup.
	/// </summary>
	public BuyBoosterPopupScript buyBoosterPopup;
	
	/// <summary>
	/// The unlock booster popup.
	/// </summary>
	public UnlockBoosterPopupScript unlockBoosterPopup;

	/// <summary>
	/// The leaderboard.
	/// </summary>
	public LeaderboardScript leaderboard;

	/// <summary>
	/// Tutotrial text
	/// </summary>
	public GameObject tutorialTextBG;
	public Text tutorialText;

	/// <summary>
	/// The level.
	/// </summary>
	public AtlasImage level;

	// The user data (cache)
	private UserData _userData;

	// The map scale
	private float _mapScale = 1.0f;

	// The current zone
	private ZoneSetting _zone;

	// The zone type
	private ZoneType _zoneType;

	// The foothold counter
	private int _footholdCount;

	// The board
	private Board _board;

	// The number of rows
	private int _gridHeight;

	// The number of columns
	private int _gridWidth;

	// The array of footholds
	private Foothold[,] _footholds;

	// The animal
	private Animal _animal;

	// The array of boosters
	private Booster[] _boosters;

	// The dummy booster
	private GameObject _dummyBooster;
	
	// The dummy delta y
	private float _dummyDeltaY;

	// The dummy booster AABB (relative)
	private float _dummyBoosterLeft;
	private float _dummyBoosterTop;
	private float _dummyBoosterRight;
	private float _dummyBoosterBottom;
	
	// The camera size
	private Vector2 _cameraSize;

	// The highlight "out" size
	private float _highlightOutSize;

	// The booster moved delegate
	private Action<Vector3> _onBoosterMoved;

	// The "undo" stack
	private Stack<UndoData> _undoStack = new Stack<UndoData>(30);

	// The "undo" direction
	private Direction _undoDirection;

	// The hint helper
	private HintHelper _hintHelper;

	// The direction stack
	private List<KeyDirection> _directionStack = new List<KeyDirection>(10);

	// The input queue
	private List<Cell> _inputQueue = new List<Cell>(10);

	// The time foothold durations
	private Dictionary<int, int> _timeFootholdDurations = new Dictionary<int, int>(10);

	// True if game ended
	private bool _isGameEnded;

	/* Tutorial */

	private GameObject _hand;

	private bool _isControlTutorial;

	private bool _isUndoTutorial;
	private bool _isPlayedUndoTutorial;

	private bool _isHintTutorial;
	private float _hintTime = -1;

	private bool _isHammerTutorial;
	private int _hammerRow    = -1;
	private int _hammerColumn = -1;
	
	private bool _isDeployTutorial;
	private int _deployRow    = -1;
	private int _deployColumn = -1;

	private Vector3 _handStartPos;
	private Vector3 _handEndPos;

	// Get zone setting
	public ZoneSetting Zone
	{
		get
		{
			return _zone;
		}
	}

	void OnEnable()
	{
		KeyManager.AddBackEventHandler(OnKeyBack);
	}

	void OnDisable()
	{
		KeyManager.RemoveBackEventHandler(OnKeyBack);
	}

	void OnKeyBack()
	{
		ShowPausePopup();
	}

	public void PushDirection(int row, int column, Direction direction)
	{
		_directionStack.Add(new KeyDirection(GetCellIndex(row, column), direction));
	}

	public Direction PopDirection(int row, int column)
	{
		int key = GetCellIndex(row, column);

		for (int i = _directionStack.Count - 1; i >= 0; i--)
		{
			KeyDirection keyDirection = _directionStack[i];

			if (keyDirection.key == key)
			{
				_directionStack.RemoveAt(i);

				return keyDirection.direction;
			}
		}

		return Direction.None;
	}

	void Awake()
	{
		// Set user data
		_userData = UserData.Instance;

		Transform boostersTransform = boosters.transform;

		// Set boosters position
		boostersTransform.SetPositionY(-Camera.main.orthographicSize - 2.0f);

		// Get all boosters
		int boosterCount = boostersTransform.childCount;

		_boosters = new Booster[boosterCount];

		for (int i = 0; i < boosterCount; i++)
		{
			_boosters[i] = boostersTransform.GetChild(i).GetComponent<Booster>();
			_boosters[i].EventListener = this;

			if (_userData.NewBooster != _boosters[i].Type.ToInt())
			{
				_boosters[i].Quantity = _userData.GetBoosterQuantity(_boosters[i].Type);
			}
		}
	}

	void Start()
	{
		// Disable touch
		TouchManager.Instance.Enabled = false;
		
		// Get current map
		int mapId = _userData.Map;

		Manager.Instance.analytics.LogScreen("Playing Game - Level "+ mapId.ToString("000"));
		
		// Get zone type
		_zoneType = ZoneTypeHelper.GetZoneType(mapId);

		// Play BGM
		SoundManager.Instance.PlayRandomMusic(SoundID.MainGame1, SoundID.MainGame2, SoundID.MainGame3);

		// Get map scale
		_mapScale = ResolutionHelper.GetScale();

		// Set scale
		map.transform.SetScale(_mapScale);
		dummyDeploy.transform.SetScale(_mapScale);
		dummyHammer.transform.SetScale(_mapScale);

		// Set zone
		_zone = zones[_zoneType.ToInt()];

		// Set deploy icon
		dummyDeploy.SetSprite(_zone.footholdIcon);

		// Create background
		GameObject background = Instantiate(_zone.backgroundPrefab) as GameObject;
		background.transform.SetParent(map.transform);
		background.transform.localScale = defaultScale;

		// Set level
		level.Text = string.Format("L {0:00}", mapId);

		// Get board
		_board = background.GetComponentInChildren<Board>();
		_board.EventListener = this;

		// Set dummy booster
		_dummyBooster = dummyHammer;

		// Load map
		MapData mapData = MapData.Load(mapId);

		// Set map
		SetMap(mapData);
		
		// Set game not ended
		_isGameEnded = false;

		// Set camera size
		_cameraSize = Camera.main.GetSize();

		// Set highlight "out" size
		_highlightOutSize = _cameraSize.y * 2.0f;

		// Get animal position
		Vector3 animalPosition = _animal.transform.position;

		startTransition.outerSize = _cameraSize;
		startTransition.innerPosition = new Vector2(animalPosition.x, animalPosition.y + 0.2f);

		_animal.Greet();

		var show = LerpFloatAction.Create(0, circleDiameter, transitionDuration, (size) => { startTransition.InnerSize = size; });
//		var greet = CallFuncAction.Create(() => { _animal.Greet(); });
		var delay = DelayAction.Create(1.0f);
		var disappear = LerpFloatAction.Create(circleDiameter, _cameraSize.y * 2.0f, transitionDuration, (size) => { startTransition.InnerSize = size; });

		gameObject.Play(SequenceAction.Create(show, /*greet,*/ delay, disappear), () => {
			// Destroy transition
			Destroy(startTransition.gameObject);

			// Show level info
			levelInfo.Show();

			// Show left controls
			leftControls.Show();

			// Show right controls
			rightControls.Show();

			// Show boosters
			boosters.Show();

			bool hasNewBooster = (_userData.NewBooster >= 0);

			boosters.Play(VMoveAction.MoveTo(-Camera.main.orthographicSize + 0.7f, 0.5f, Ease.SineOut), () => {
				for (int i = 0; i < _boosters.Length; i++)
				{
					_boosters[i].UpdateBoundary();
				}

				// Check unlock new booster
				if (_userData.NewBooster >= 0)
				{
					// Play sound
					SoundManager.Instance.PlaySound(SoundID.UnlockBooster);

					// Get new booster
					Booster booster = _boosters.Get(_userData.NewBooster);
					booster.Quantity = _userData.GetBoosterQuantity(booster.Type);

					// Create effect
					if (unlockBoosterPrefab != null)
					{
						unlockBoosterPrefab.Create(booster.transform, booster.transform.position);
					}

					// Clear new booster
					_userData.NewBooster = -1;
				}
			});

			// Play
			_animal.Play();

//			Manager.Instance.SetUpdateSendRequests(true);

			// Control tutorial
			if (IsControlTutorial())
			{
				ShowHelpPopup();

				_isControlTutorial = true;

				// Set outer size
				tutorialHighlight.outerSize = _cameraSize;

				// Set inner size
				tutorialHighlight.InnerSize = _highlightOutSize;

				// Create hand
				_hand = handPrefab.Create(_board.transform, Vector3.zero);
				_hand.Hide();

				if (PlayControlTutorial(hasNewBooster ? 2.0f : 1.0f))
				{
					// Disable boosters
					SetBoostersInteractable(false);
				}
				else
				{
					_isControlTutorial = false;

					// Enable touch
					TouchManager.Instance.Enabled = true;
				}

//				tutorialTextBG.SetActive(true);
//				tutorialText.gameObject.SetActive(true);
//				tutorialText.text = Settings.JumpTutorialText;
//
//				var fadeOut = FadeAction.FadeTo(0, 1f);
//				var delayAction = DelayAction.Create(1.5f);
//				var fadeIn = FadeAction.FadeTo(1, 1);
//				var squence = SequenceAction.Create(fadeOut, fadeIn, delayAction);
//				tutorialText.gameObject.Play(RepeatAction.RepeatForever(squence));
			}
			else
			{
				Manager.Instance.SetUpdateSendRequests(true);

				// Enable touch
				TouchManager.Instance.Enabled = true;

				// Hammer tutorial
				if (_isHammerTutorial)
				{
					PlayHammerTutorial();
				}
				// Deploy tutorial
				else if (_isDeployTutorial)
				{
					PlayDeployTutorial();
				}
				else if (IsLongJumpTurotial())
				{
//					tutorialTextBG.SetActive(true);
//					tutorialText.gameObject.SetActive(true);
//					tutorialText.text = Settings.LongJumpTutorialText;
//
//					var fadeOut = FadeAction.FadeTo(0, 1);
//					var delayAction = DelayAction.Create(1.5f);
//					var fadeIn = FadeAction.FadeTo(1, 1);
//					var squence = SequenceAction.Create(fadeOut, fadeIn, delayAction);
//					tutorialText.gameObject.Play(RepeatAction.RepeatForever(squence));
				}
			}
		});

		MyAdmob.Instance.ShowBanner();
		//MyAdmob.Instance.HideBanner();
	}

	void OnDestroy()
	{
		if (Manager.IsActive)
		{
			Manager.Instance.SetUpdateSendRequests(false);
		}
	}

	void SetMap(MapData mapData)
	{
		if (mapData == null) return;

		// Get footholds
		int[,] footholds = mapData.footholds;

		// Get number of rows
		_gridHeight = footholds.GetRow();

		// Get number of columns
		_gridWidth = footholds.GetColumn();

		// Construct map overlay
		mapOverlay.Construct(_gridWidth, _gridHeight, 1.85f);

		// Create array of footholds
		_footholds = new Foothold[_gridHeight, _gridWidth];

		// Reset foothold counter
		_footholdCount = 0;

		for (int i = 0; i < _gridHeight; i++)
		{
			int row = _gridHeight - 1 - i;

			for (int j = 0; j < _gridWidth; j++)
			{
				// Get type of foothold
				FootholdType type = footholds[i, j].ToFootholdType();

				if (type != FootholdType.None)
				{
					AddFoothold(row, j, type);
				}
			}
		}

		// Set TimeFoothold duration
		mapData.DeserializeTimeFootholds((row, column, duration) => {
			row = _gridHeight - 1 - row;

			TimeFoothold timeFoothold = (TimeFoothold)_footholds[row, column];
			
			if (timeFoothold != null)
			{
				timeFoothold.Duration = duration;
			}
			else
			{
				//Log.Debug("TimeFoothold is null!");
			}

			_timeFootholdDurations.Add(row * _gridWidth + column, duration);
		});

		// Create animal
		GameObject animal = Instantiate(animalPrefab) as GameObject;
		animal.transform.SetParent(_board.transform);
		animal.transform.localScale = defaultScale;

		int startRow    = _gridHeight - 1 - mapData.startRow;
		int startColumn = mapData.startColumn;
		Direction direction = mapData.direction;

		// Set animal
		_animal = animal.GetComponent<Animal>();
		_animal.Init(_footholds[startRow, startColumn], direction);

		// Hammer tutorial
		if (IsPlayHammerTutorial())
		{
			// Add foothold
			if (direction.IsLeft())
			{
				_hammerRow    = startRow;
				_hammerColumn = startColumn + 1;
			}
			else if (direction.IsRight())
			{
				_hammerRow    = startRow;
				_hammerColumn = startColumn - 1;
			}
			else if (direction.IsUp())
			{
				_hammerRow    = startRow - 1;
				_hammerColumn = startColumn;
			}
			else if (direction.IsDown())
			{
				_hammerRow    = startRow + 1;
				_hammerColumn = startColumn;
			}

			if (GetFoothold(_hammerRow, _hammerColumn) == null)
			{
				AddFoothold(_hammerRow, _hammerColumn, FootholdType.Normal);

				_isHammerTutorial = true;
			}
			else
			{
				//Log.Debug("Hammer tutorial map invalid!");
			}
		}
		// Deploy tutorial
		else if (IsPlayDeployTutorial())
		{
			// Remove foothold
			if (direction.IsLeft())
			{
				_deployRow    = startRow;
				_deployColumn = startColumn - 1;
			}
			else if (direction.IsRight())
			{
				_deployRow    = startRow;
				_deployColumn = startColumn + 1;
			}
			else if (direction.IsUp())
			{
				_deployRow    = startRow + 1;
				_deployColumn = startColumn;
			}
			else if (direction.IsDown())
			{
				_deployRow    = startRow - 1;
				_deployColumn = startColumn;
			}

			Foothold foothold = GetFoothold(_deployRow, _deployColumn);

			if (foothold != null)
			{
				Destroy(foothold.gameObject);

				_footholds[_deployRow, _deployColumn] = null;

				// Decrease foothold
				_footholdCount--;

				_isDeployTutorial = true;
			}
			else
			{
				//Log.Debug("Deploy tutorial map invalid!");
			}
		}

		// Set skin
		if (!_zoneType.IsDefault())
		{
			_animal.SetClothes(_zone.clothesLeft, _zone.clothesUp, _zone.clothesDown);
			_animal.SetCap(_zone.capLeft, _zone.capUp, _zone.capDown);
		}
	}

	void AddFoothold(int row, int column, FootholdType type, bool undo = false)
	{
		// Get foothold prefab
		GameObject footholdPrefab = _zone.GetFootholdPrefab(type);

		// Create foothold
		GameObject footholdObj = footholdPrefab.Create(_board.transform, _board.GetPosition(row, column));
		
		// Get foothold
		Foothold foothold = footholdObj.GetComponent<Foothold>();
		foothold.Construct(row, column, undo);
		
		// Set foothold
		_footholds[row, column] = foothold;
		
		// Increase foothold
		_footholdCount++;
	}

	public int GetTimeFootholdDuration(int row, int column)
	{
		return _timeFootholdDurations[row * _gridWidth + column];
	}

	public void OnAnimalExitFoothold(Foothold foothold)
	{
		// Stop update hint time
		_hintTime = -1;

		if (foothold.IsAtomic())
		{
			_footholds[foothold.Row, foothold.Column] = null;

			// Decrease foothold
			_footholdCount--;
		}
	}
	
	public void OnFootholdDestroyed(Foothold foothold)
	{
		_footholds[foothold.Row, foothold.Column] = null;
		
		// Decrease foothold
		_footholdCount--;

		// Check end game
		CheckEndGame();
	}
	
	public void OnFootholdExpired(Foothold foothold)
	{
		LoseGame();
	}

	public void OnAnimalDidFinishJump(Animal animal)
	{
		// Check input queue
		if (_inputQueue.Count > 0)
		{
			// Get first cell
			Cell first = _inputQueue[0];

			// Remove first cell
			_inputQueue.RemoveAt(0);

			// Update animal speed
//			_animal.SetSpeed(GetAnimalSpeed());

			// Continue to jump
			if (!JumpTo(first.row, first.column))
			{
				//Log.Debug("Input queue failed!");

				_inputQueue.Clear();
				_animal.ResetSpeed();
			}
		}
		else
		{
			_animal.ResetSpeed();

			// Control tutorial
			if (_isControlTutorial)
			{
				if (IsFinished())
				{
					WinGame();
				}
				else
				{
					if (!PlayControlTutorial())
					{
						// Unlock cells
						_board.ResetEnabledCell();

						// Unlock directions
						_board.ResetEnabledDirection();

						// Enable boosters
						SetBoostersInteractable(true);

						_isControlTutorial = false;

						CheckEndGame();
					}
				}
			}
			// Hint tutorial
			else if (IsPlayHintTutorial())
			{
				_hintTime = 0;
			}
			else
			{
				CheckEndGame();
			}
		}
	}

	public void OnAnimalDidFinishUnjump(Animal animal)
	{
		// Set direction
		animal.Direction = _undoDirection;

		// Hint tutorial
		if (IsPlayHintTutorial())
		{
			_hintTime = 0;
		}
	}
	
	Foothold GetLeftFoothold(int row, int column)
	{
		for (int i = column - 1; i >= 0; i--)
		{
			if (_footholds[row, i] != null)
			{
				return _footholds[row, i];
			}
		}

		return null;
	}
	
	Foothold GetRightFoothold(int row, int column)
	{
		for (int i = column + 1; i < _gridWidth; i++)
		{
			if (_footholds[row, i] != null)
			{
				return _footholds[row, i];
			}
		}
		
		return null;
	}
	
	Foothold GetAboveFoothold(int row, int column)
	{
		for (int i = row + 1; i < _gridHeight; i++)
		{
			if (_footholds[i, column] != null)
			{
				return _footholds[i, column];
			}
		}
		
		return null;
	}
	
	Foothold GetBelowFoothold(int row, int column)
	{
		for (int i = row - 1; i >= 0; i--)
		{
			if (_footholds[i, column] != null)
			{
				return _footholds[i, column];
			}
		}
		
		return null;
	}

	bool IsFinished()
	{
		return _footholdCount == 1;

//		int count = 0;
//
//		for (int i = 0; i < _gridHeight; i++)
//		{
//			for (int j = 0; j < _gridWidth; j++)
//			{
//				if (_footholds[i, j] != null)
//				{
//					count++;
//
//					if (count > 1)
//					{
//						return false;
//					}
//				}
//			}
//		}
//
//		return true;
	}

	bool CanJump()
	{
		// Get row
		int row = _animal.Row;

		// Get column
		int column = _animal.Column;

		// Left
		if (_animal.IsLeft())
		{
			return GetLeftFoothold(row, column) != null || GetAboveFoothold(row, column) != null || GetBelowFoothold(row, column) != null;
		}
		
		// Right
		if (_animal.IsRight())
		{
			return GetRightFoothold(row, column) != null || GetAboveFoothold(row, column) != null || GetBelowFoothold(row, column) != null;
		}
		
		// Up
		if (_animal.IsUp())
		{
			return GetAboveFoothold(row, column) != null || GetLeftFoothold(row, column) != null || GetRightFoothold(row, column) != null;
		}
		
		// Down
		if (_animal.IsDown())
		{
			return GetBelowFoothold(row, column) != null || GetLeftFoothold(row, column) != null || GetRightFoothold(row, column) != null;
		}

		return false;
	}

	void JumpLeft(Foothold foothold)
	{
		// Check if this foothold is locking
		if (foothold.Locking)
		{
			//Log.Debug("Foothold is locking!");
			return;
		}

		Assert.IsFalse(_animal.Jumping);

		// Trace
		_undoStack.Push(_animal.Trace());

		// Jump left
		_animal.JumpLeft(foothold);
	}
	
	void JumpRight(Foothold foothold)
	{
		// Check if this foothold is locking
		if (foothold.Locking)
		{
			//Log.Debug("Foothold is locking!");
			return;
		}
		
		Assert.IsFalse(_animal.Jumping);

		// Trace
		_undoStack.Push(_animal.Trace());

		// Jump right
		_animal.JumpRight(foothold);
	}
	
	void JumpUp(Foothold foothold)
	{
		// Check if this foothold is locking
		if (foothold.Locking)
		{
			//Log.Debug("Foothold is locking!");
			return;
		}
		
		Assert.IsFalse(_animal.Jumping);

		// Trace
		_undoStack.Push(_animal.Trace());

		// Jump up
		_animal.JumpUp(foothold);
	}
	
	void JumpDown(Foothold foothold)
	{
		// Check if this foothold is locking
		if (foothold.Locking)
		{
			//Log.Debug("Foothold is locking!");
			return;
		}
		
		Assert.IsFalse(_animal.Jumping);

		// Trace
		_undoStack.Push(_animal.Trace());

		// Jump down
		_animal.JumpDown(foothold);
	}

	bool JumpTo(int row, int column)
	{
		// Get foothold
		Foothold foothold = _footholds[row, column];
		
		if (foothold == null)
		{
			Wrong();
			return false;
		}

		// Get animal row
		int animalRow = _animal.Row;
		
		// Get animal column
		int animalColumn = _animal.Column;
		
		// Horizontal
		if (row == animalRow && column != animalColumn)
		{
			// Left
			if (column < animalColumn)
			{
				if (_animal.IsRight())
				{
					Wrong(foothold, true);
					return false;
				}
				else
				{
					for (int i = column + 1; i < animalColumn; i++)
					{
						if (_footholds[row, i] != null)
						{
							Wrong(foothold);
							return false;
						}
					}
					
					JumpLeft(foothold);
				}
			}
			// Right
			else
			{
				if (_animal.IsLeft())
				{
					Wrong(foothold, true);
					return false;
				}
				else
				{
					for (int i = animalColumn + 1; i < column; i++)
					{
						if (_footholds[row, i] != null)
						{
							Wrong(foothold);
							return false;
						}
					}
					
					JumpRight(foothold);
				}
			}
		}
		// Vertical
		else if (row != animalRow && column == animalColumn)
		{
			// Up
			if (row > animalRow)
			{
				if (_animal.IsDown())
				{
					Wrong(foothold, true);
					return false;
				}
				else
				{
					for (int i = animalRow + 1; i < row; i++)
					{
						if (_footholds[i, column] != null)
						{
							Wrong(foothold);
							return false;
						}
					}
					
					JumpUp(foothold);
				}
			}
			// Down
			else
			{
				if (_animal.IsUp())
				{
					Wrong(foothold, true);
					return false;
				}
				else
				{
					for (int i = row + 1; i < animalRow; i++)
					{
						if (_footholds[i, column] != null)
						{
							Wrong(foothold);
							return false;
						}
					}
					
					JumpDown(foothold);
				}
			}
		}
		else
		{
			Wrong(foothold);
			return false;
		}

		return true;
	}

	void OnStartJump()
	{
		// Control tutorial
		if (_isControlTutorial)
		{
			// Disable touch
			SetTouchEnabled(false);
			
			// Unhighlight tutorial
			UnhighlightTutorial();
		}
	}

	Direction GetDirection(int row, int column, Direction direction, int nextRow, int nextColumn)
	{
		// Horizontal
		if (nextRow == row && nextColumn != column)
		{
			// Left
			if (nextColumn < column)
			{
				if (direction.IsRight())
				{
					return Direction.None;
				}
				else
				{
					for (int i = nextColumn + 1; i < column; i++)
					{
						if (_footholds[nextRow, i] != null)
						{
							return Direction.None;
						}
					}
					
					return Direction.Left;
				}
			}
			// Right
			else
			{
				if (direction.IsLeft())
				{
					return Direction.None;
				}
				else
				{
					for (int i = column + 1; i < nextColumn; i++)
					{
						if (_footholds[nextRow, i] != null)
						{
							return Direction.None;
						}
					}
					
					return Direction.Right;
				}
			}
		}
		// Vertical
		else if (nextRow != row && nextColumn == column)
		{
			// Up
			if (nextRow > row)
			{
				if (direction.IsDown())
				{
					return Direction.None;
				}
				else
				{
					for (int i = row + 1; i < nextRow; i++)
					{
						if (_footholds[i, nextColumn] != null)
						{
							return Direction.None;
						}
					}
					
					return Direction.Up;
				}
			}
			// Down
			else
			{
				if (direction.IsUp())
				{
					return Direction.None;
				}
				else
				{
					for (int i = nextRow + 1; i < row; i++)
					{
						if (_footholds[i, nextColumn] != null)
						{
							return Direction.None;
						}
					}
					
					return Direction.Down;
				}
			}
		}

		return Direction.None;
	}
	
	Direction GetDirection(int row, int column, int nextRow, int nextColumn)
	{
		// Horizontal
		if (nextRow == row && nextColumn != column)
		{
			// Left
			if (nextColumn < column)
			{
				return Direction.Left;
			}
			// Right
			else
			{
				return Direction.Right;
			}
		}
		// Vertical
		else if (nextRow != row && nextColumn == column)
		{
			// Up
			if (nextRow > row)
			{
				return Direction.Up;
			}
			// Down
			else
			{
				return Direction.Down;
			}
		}
		
		return Direction.None;
	}

	void AddInput(int row, int column)
	{
//		Log.Debug(string.Format("Add: ({0},{1})", row, column));

		// Get foothold
		Foothold foothold = _footholds[row, column];

		if (foothold == null)
		{
			Wrong();
			return;
		}

		foothold.OnHighlight();

		int count = _inputQueue.Count;

		if (count == 0)
		{
			Direction direction = GetDirection(_animal.Row, _animal.Column, _animal.Direction, row, column);
			
			if (direction.IsNone())
			{
				Wrong();
			}
			else
			{
				if (foothold.Direction.IsOpposite(direction))
				{
					Wrong();
				}
				else
				{
					AddInputCallback(row, column);
				}
			}
		}
		else
		{
			for (int i = 0; i < count; i++)
			{
				if (_inputQueue[i].Is(row, column))
				{
					Wrong();
					return;
				}
			}

			// Get last cell
			Cell lastCell = _inputQueue[count - 1];

			// Get last foothold
			Foothold lastFoothold = _footholds[lastCell.row, lastCell.column];

			Direction lastDirection = lastFoothold.Direction;

			if (lastDirection.IsNone())
			{
				if (count == 1)
				{
					lastDirection = GetDirection(_animal.Row, _animal.Column, lastCell.row, lastCell.column);
				}
				else
				{
					Cell prevLastCell = _inputQueue[count - 2];
					lastDirection = GetDirection(prevLastCell.row, prevLastCell.column, lastCell.row, lastCell.column);
				}
			}

			if (lastDirection.IsNone())
			{
				Wrong();
			}
			else
			{
				Direction direction = GetDirection(lastCell.row, lastCell.column, lastDirection, row, column);
				
				if (direction.IsNone())
				{
					Wrong();
				}
				else
				{
					AddInputCallback(row, column);
				}
			}
		}
	}

	void AddInputCallback(int row, int column)
	{
//		Log.Debug(string.Format("Push: ({0},{1})", row, column));

		_inputQueue.Add(new Cell(row, column));

		// Update animal speed
		_animal.SetSpeed(GetAnimalSpeed());
	}

	float GetAnimalSpeed()
	{
		return 1.5f + (_inputQueue.Count - 1) * 0.5f;
	}

	void AddInput(Func<int, int, Foothold> func)
	{
		int count = _inputQueue.Count;

		if (count == 0)
		{
			Foothold foothold = func(_animal.Row, _animal.Column);
			
			if (foothold != null)
			{
				AddInput(foothold.Row, foothold.Column);
			}
			else
			{
				Wrong();
			}
		}
		else
		{
			Cell last = _inputQueue[count - 1];
			
			Foothold foothold = func(last.row, last.column);
			
			if (foothold != null)
			{
				AddInput(foothold.Row, foothold.Column);
			}
			else
			{
				Wrong();
			}
		}
	}

	void Wrong(Foothold foothold = null, bool backward = false)
	{
//		Log.Debug("Wrong");
		
		SoundManager.Instance.PlaySound (SoundID.CyrusWrongInput);

		if (!_animal.Jumping)
		{
			_animal.OnWrong();
		}

		if (foothold != null)
		{
			foothold.OnWrong();

			// Only show message form level 1 to 5
			if (UserData.Instance.Map <= 5) {

				// Pause game
				SetPaused(true);

				Manager.Instance.ShowMessage(Settings.CannotJumpBackwardText, cannotJumpBackward, () => {
					// Resume game
					SetPaused(false);
				});
			}

		}
	}

	void SetTouchEnabled(bool enabled)
	{
		TouchManager.Instance.Enabled = enabled;
		_board.Interactable = enabled; // Key input
	}
	
	void SetUIEnabled(bool enabled)
	{
		canvas.SetInteractable(enabled);
	}

	void SetInteractable(bool interactable)
	{
		SetTouchEnabled(interactable);
		SetUIEnabled(interactable);
	}

	void SetPaused(bool paused)
	{
		SetTouchEnabled(!paused);

		for (int i = 0; i < _gridHeight; i++)
		{
			for (int j = 0; j < _gridWidth; j++)
			{
				if (_footholds[i, j] != null)
				{
					_footholds[i, j].SetPaused(paused);
				}
			}
		}

		_animal.SetPaused(paused);
	}

	void CheckEndGame()
	{
		// Check win game
		if (IsFinished())
		{
			WinGame();
		}
		// Check lose game
		else if (!CanJump())
		{
			if (IsPlayUndoTutorial())
			{
				// Play tutorial
				PlayUndoTutorial();
			}
			else
			{
				// Pause game
				SetPaused(true);

				// Play sound
				SoundManager.Instance.PlaySound(SoundID.CyrusWrongInput);
				Manager.Instance.ShowMessage("Opps!!!", Settings.CannotJumpText, () => {
					LoseGame();
				});
			}
		}
	}

	void WinGame()
	{
		// Stop BGM
		SoundManager.Instance.StopMusic();

		// Play SFX
		SoundManager.Instance.PlaySound(SoundID.WinGame);

		_isGameEnded = true;

		// Stop foothold
		_animal.Foothold.Stop();

		_animal.OnWinGame();

		// Disable interaction
		SetInteractable(false);

		// Check if unlock next map
		if (_userData.Map == _userData.Level)
		{
			// Increase level
			_userData.Level++;

			// Update Facebook score
			FBManager.Instance.UpdateScore(() => {
				// Set new level
				_userData.NewLevel = (_userData.Map == _userData.Level - 1);

				Invoke("WinGameCallback", 1.0f);
			});
		}
		else
		{
			Invoke("WinGameCallback", 1.0f);
		}
	}

	void WinGameCallback()
	{
		if (_userData.Mana < Settings.MaxMana)
		{
			_userData.Mana++;

			NotificationManager.ManaChanged(_userData.Mana);
		}
		
		// Show Win popup
		winPopup.Show(() => {
			BoosterType newType = BoosterType.None;

			for (int i = 0; i < BoosterTypeHelper.boosterCount; i++)
			{
				BoosterType type = BoosterTypeHelper.boosterTypes[i];

				if (!_userData.IsBoosterUnlocked(type) && _userData.Map >= type.GetUnlockLevel())
				{
					// Unlock booster
					_userData.UnlockBooster(type);

					newType = type;
				}
			}

			if (newType != BoosterType.None)
			{
				// Set new booster
				_userData.NewBooster = newType.ToInt();
				
				if (unlockBoosterPopup != null)
				{
					// Disable UI
					SetUIEnabled(false);

					gameObject.Play(SequenceAction.Create(DelayAction.Create(0.25f), CallFuncAction.Create(() => {
						// Enable UI
						SetUIEnabled(true);

						// Show popup
						unlockBoosterPopup.Show(newType);

						SoundManager.Instance.PlaySound(SoundID.GotUnlock);
					})));
				}
			}
		});
	}

	void LoseGame()
	{
		// Stop BGM
		SoundManager.Instance.StopMusic();
		
		// Play SFX
		SoundManager.Instance.PlaySound(SoundID.LoseGame);

		_isGameEnded = true;
		
		_animal.OnLoseGame();

		// Disable interaction
		SetInteractable(false);

		Invoke("LoseGameCallback", 0.5f);
	}

	void LoseGameCallback()
	{
		// Show Lose popup
		losePopup.Show();
	}

	void ShowFootholdOverlays()
	{
		if (_isHammerTutorial)
		{
			for (int i = 0; i < _gridHeight; i++)
			{
				for (int j = 0; j < _gridWidth; j++)
				{
					mapOverlay.SetShowCell(i, j, false);
				}
			}

			mapOverlay.SetShowCell(_hammerRow, _hammerColumn, true);
		}
		else
		{
			for (int i = 0; i < _gridHeight; i++)
			{
				for (int j = 0; j < _gridWidth; j++)
				{
					mapOverlay.SetShowCell(i, j, _footholds[i, j] != null);
				}
			}
			
			mapOverlay.SetShowCell(_animal.Row, _animal.Column, false);
		}

		mapOverlay.Show();
	}

	void HideFootholdOverlays()
	{
		mapOverlay.Hide();
	}
	
	void ShowEmptyOverlays()
	{
		if (_isDeployTutorial)
		{
			for (int i = 0; i < _gridHeight; i++)
			{
				for (int j = 0; j < _gridWidth; j++)
				{
					mapOverlay.SetShowCell(i, j, false);
				}
			}

			mapOverlay.SetShowCell(_deployRow, _deployColumn, true);
		}
		else
		{
			for (int i = 0; i < _gridHeight; i++)
			{
				for (int j = 0; j < _gridWidth; j++)
				{
					mapOverlay.SetShowCell(i, j, _footholds[i, j] == null);
				}
			}
		}

		mapOverlay.Show();
	}
	
	void HideEmptyOverlays()
	{
		mapOverlay.Hide();
	}

	bool Hint()
	{
		if (_isHintTutorial)
		{
			_isHintTutorial = false;
			
			EndBoosterTutorial();
		}

		if (!IsIdle())
		{
			return false;
		}
		
		Foothold foothold = GetNextFoothold();

		if (foothold != null)
		{
			foothold.OnHint();
			SoundManager.Instance.PlaySound(SoundID.Ring);

			return true;
		}

		SoundManager.Instance.PlaySound(SoundID.CyrusWrongInput);
		Manager.Instance.ShowMessage("Sorry", Settings.CannotHint);

		return false;
	}

	Foothold GetNextFoothold()
	{
		if (_hintHelper == null)
		{
			_hintHelper = new HintHelper(_gridHeight, _gridWidth);
		}
		
		int startRow    = _animal.Row;
		int startColumn = _animal.Column;
		
		Direction direction = _hintHelper.Hint(_footholds, startRow, startColumn, _animal.Direction);
		
		if (direction.IsLeft())
		{
			return GetLeftFoothold(startRow, startColumn);
		}

		if (direction.IsRight())
		{
			return GetRightFoothold(startRow, startColumn);
		}

		if (direction.IsUp())
		{
			return GetAboveFoothold(startRow, startColumn);
		}

		if (direction.IsDown())
		{
			return GetBelowFoothold(startRow, startColumn);
		}

		return null;
	}

	bool Undo()
	{
		if (_isUndoTutorial)
		{
			_isUndoTutorial = false;

			EndBoosterTutorial();
		}

		if (_undoStack.Count < 1)
		{
			Manager.Instance.ShowMessage("Sorry", Settings.CannotUndo);
			return false;
		}

		if (!IsIdle())
		{
			return false;
		}

//		if (_isControlTutorial)
//		{
//			Log.Debug("Playing control tutorial!");
//			return false;
//		}

		// Pop data
		UndoData data = _undoStack.Peek();

		// Add foothold
		if (_footholds[data.row, data.column] == null)
		{
			AddFoothold(data.row, data.column, data.foothold, true);
		}

		// Save direction
		_undoDirection = data.direction;

		// Unjump
		_animal.Unjump(_footholds[data.row, data.column]);

		// Delete data
		_undoStack.Pop();

		return true;
	}

	bool IsIdle()
	{
		if (_animal.Jumping)
		{
			//Log.Debug("Animal is jumping!");
			return false;
		}
		
		if (_inputQueue.Count > 0)
		{
			//Log.Debug("Jumping in queue!");
			return false;
		}

		return true;
	}

	void UnlockBooster(BoosterType type)
	{
		_userData.UnlockBooster(type);
		UpdateBooster(type);
	}

	void DecreaseDeploy()
	{
		_userData.Deploy--;
		UpdateBooster(BoosterType.Deploy);

		Manager.Instance.analytics.LogEvent("Main Game", "Use Booster", "Deploy", 1);
	}
	
	void DecreaseHammer()
	{
		_userData.Hammer--;
		UpdateBooster(BoosterType.Hammer);

		Manager.Instance.analytics.LogEvent("Main Game", "Use Booster", "Hammer", 1);
	}
	
	void DecreaseHint()
	{
		_userData.Hint--;
		UpdateBooster(BoosterType.Hint);

		Manager.Instance.analytics.LogEvent("Main Game", "Use Booster", "Hint", 1);
	}
	
	void DecreaseUndo()
	{
		_userData.Undo--;
		UpdateBooster(BoosterType.Undo);

		Manager.Instance.analytics.LogEvent("Main Game", "Use Booster", "Undo", 1);
	}

	void AddDeploy(int quantity)
	{
		_userData.Deploy += quantity;
		UpdateBooster(BoosterType.Deploy);
	}
	
	void AddHammer(int quantity)
	{
		_userData.Hammer += quantity;
		UpdateBooster(BoosterType.Hammer);
	}
	
	void AddHint(int quantity)
	{
		_userData.Hint += quantity;
		UpdateBooster(BoosterType.Hint);
	}
	
	void AddUndo(int quantity)
	{
		_userData.Undo += quantity;
		UpdateBooster(BoosterType.Undo);
	}

	void UpdateBooster(BoosterType type)
	{
		Booster booster = _boosters[type.ToInt()];
		
		if (type.IsDeploy())
		{
			booster.Quantity = _userData.Deploy;
		}
		else if (type.IsHammer())
		{
			booster.Quantity = _userData.Hammer;
		}
		else if (type.IsHint())
		{
			booster.Quantity = _userData.Hint;
		}
		else if (type.IsUndo())
		{
			booster.Quantity = _userData.Undo;
		}
	}

	int GetCellIndex(int row, int column)
	{
		return row * _gridWidth + column;
	}

	#region UI

	public void ShowHelpPopup()
	{
		if (helpPrefab != null)
		{
			Manager.Instance.SetUpdateSendRequests(false);

			// Play sound
			SoundManager.PlayButtonOpen();

			// Pause game
			SetPaused(true);

			GameObject help = helpPrefab.CreateUI(canvas.transform, Vector2.zero);

			HelpPopupScript script = help.GetComponent<HelpPopupScript>();
			script.CloseCallback = () => {
				// Resume game
				SetPaused(false);

				Manager.Instance.SetUpdateSendRequests(true);
			};
		}
		else
		{
			// Play sound
			SoundManager.PlayButtonClick();
		}

		Manager.Instance.analytics.LogEvent("Main Game", "Button Click", "Help", 1);
	}

	public void ShowPausePopup()
	{
		// Play sound
		SoundManager.PlayButtonOpen();

		if (_isUndoTutorial || _isHintTutorial || _isHammerTutorial || _isDeployTutorial)
		{
			if (_hand != null && _hand.activeSelf)
			{
				_hand.PauseAction();
			}
		}
		else
		{
			// Pause game
			SetPaused(true);
		}

		// Show Pause popup
		pausePopup.Show();

		Manager.Instance.analytics.LogEvent("Main Game", "Button Click", "Pause", 1);
	}

	public void Resume()
	{
		if (_isUndoTutorial || _isHintTutorial || _isHammerTutorial || _isDeployTutorial)
		{
			if (_hand != null && _hand.activeSelf)
			{
				_hand.ResumeAction();
			}
		}
		else
		{
			SetPaused(false);
		}
	}

	public void PlayButtonSound()
	{
		SoundManager.PlayButtonClick();
	}

	public void OnButtonReplay()
	{
		Manager.Instance.ShowConfirm("Wait!", Settings.ReplayConfirm, (yes) => {
			if (yes)
			{
				Replay();
			}
		});
	}
	
	public void Replay()
	{
		// Play sound
		//SoundManager.PlayButtonClick();
		// Dont check mana
		if (true)
		{
			_userData.Mana--;

			NotificationManager.ManaChanged(_userData.Mana);

			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
		else
		{
			if (noMoreManaPopupPrefab != null)
			{
				// Create popup
				GameObject noMoreManaPopup = noMoreManaPopupPrefab.CreateUI(canvas.transform);
				NoMoreManaPopupScript script = noMoreManaPopup.GetComponent<NoMoreManaPopupScript>();

				if (script != null)
				{
					NotificationManager.AddManaEventHandler(OnManaChanged);

					script.Show(null);
				}
			}
			else
			{
				//Log.Debug("No more mana!");
			}
		}

		Manager.Instance.analytics.LogEvent("Main Game", "Button Click", "Replay", 1);
	}

	void OnManaChanged(int mana)
	{
		NotificationManager.RemoveManaEventHandler(OnManaChanged);

		if (mana > 0)
		{
			NoMoreManaPopupScript script = FindObjectOfType<NoMoreManaPopupScript>();

			if (script != null)
			{
				script.ForceClose();
			}
		}
	}

	public void ShowLeaderboard()
	{
		if (!Helper.IsOnline())
		{
			// Play sound
			SoundManager.PlayButtonClick();

			Manager.Instance.ShowMessage(Settings.NoInternetConnection);
			return;
		}

		// Play sound
		SoundManager.PlayButtonOpen();

		if (FB.IsLoggedIn)
		{
			leaderboard.Show();
		}
		else
		{
			FBManager.LogIn((error) => {
				if (string.IsNullOrEmpty(error))
				{
					// Update Facebook score
					FBManager.Instance.UpdateScore(() => {
						leaderboard.Show();
					});
				}
			});
		}

		Manager.Instance.analytics.LogEvent("Main Game", "Button Click", "Leader Board", 1);
	}
	
	void Update()
	{
// #if UNITY_EDITOR
// 		if (Input.GetKeyDown(KeyCode.Return))
// 		{
// 			Replay();
// 		}
// #endif

		if (_hintTime >= 0)
		{
			if (!_animal.Paused)
			{
				if (!Manager.Instance.Dialog)
				{
					_hintTime += Time.deltaTime;

					if (_hintTime >= Settings.HintTutorialDelay)
					{
						// Play tutorial
						PlayHintTutorial();
					}
				}
			}
		}
	}

	#endregion // UI

	#region IBoardEventListener

	public void OnCellPressed(int row, int column)
	{

	}
	
	public void OnCellReleased(int row, int column)
	{
		if (_animal.Jumping)
		{
			AddInput(row, column);
		}
		else
		{
			OnStartJump();

			JumpTo(row, column);
		}
	}
	
	public void OnCellCancelled()
	{
		
	}
	
	public void OnDraggedLeft()
	{
		if (_animal.Jumping)
		{
			AddInput(GetLeftFoothold);
			return;
		}

		// Right
		if (_animal.IsRight())
		{
			Wrong(null, GetLeftFoothold(_animal.Row, _animal.Column) != null);
		}
		else
		{
			// Get foothold
			Foothold foothold = GetLeftFoothold(_animal.Row, _animal.Column);
			
			if (foothold != null)
			{
				OnStartJump();

				JumpLeft(foothold);
			}
			else
			{
				Wrong();
			}
		}
	}
	
	public void OnDraggedRight()
	{
		if (_animal.Jumping)
		{
			AddInput(GetRightFoothold);
			return;
		}

		// Left
		if (_animal.IsLeft())
		{
			Wrong(null, GetRightFoothold(_animal.Row, _animal.Column) != null);
		}
		else
		{
			// Get foothold
			Foothold foothold = GetRightFoothold(_animal.Row, _animal.Column);
			
			if (foothold != null)
			{
				OnStartJump();

				JumpRight(foothold);
			}
			else
			{
				Wrong();
			}
		}
	}
	
	public void OnDraggedUp()
	{
		if (_animal.Jumping)
		{
			AddInput(GetAboveFoothold);
			return;
		}

		// Down
		if (_animal.IsDown())
		{
			Wrong(null, GetAboveFoothold(_animal.Row, _animal.Column) != null);
		}
		else
		{
			// Get foothold
			Foothold foothold = GetAboveFoothold(_animal.Row, _animal.Column);
			
			if (foothold != null)
			{
				OnStartJump();

				JumpUp(foothold);
			}
			else
			{
				Wrong();
			}
		}
	}
	
	public void OnDraggedDown()
	{
		if (_animal.Jumping)
		{
			AddInput(GetBelowFoothold);
			return;
		}

		// Up
		if (_animal.IsUp())
		{
			Wrong(null, GetBelowFoothold(_animal.Row, _animal.Column) != null);
		}
		else
		{
			// Get foothold
			Foothold foothold = GetBelowFoothold(_animal.Row, _animal.Column);
			
			if (foothold != null)
			{
				OnStartJump();

				JumpDown(foothold);
			}
			else
			{
				Wrong();
			}
		}
	}
	
	#endregion // IBoardEventListener

	#region IBoosterEventListener

	public void OnBoosterPressed(Booster booster)
	{
		if (booster.Locked)
		{
			// Play sound
			SoundManager.PlayButtonClick();

			return;
		}

		if (booster.Empty)
		{
			// Play sound
			SoundManager.Instance.PlaySound(SoundID.CyrusWrongInput);

			if (buyBoosterPopup != null)
			{
				// Pause game
				SetPaused(true);

				// Show popup
				buyBoosterPopup.Show(booster.Type, () => {
					// Update booster
					UpdateBooster(booster.Type);

					// Resume game
					SetPaused(false);
				});
			}
			else
			{
				//Log.Debug("Buy more booster!");
			}

			return;
		}

		// Play sound
		SoundManager.PlayButtonClick();

		// Get type
		BoosterType type = booster.Type;

		// Deploy
		if (type.IsDeploy())
		{
			ShowEmptyOverlays();
		}
		// Hammer
		else if (type.IsHammer())
		{
			ShowFootholdOverlays();
		}
	}

	public void OnBoosterBuyMore(Booster booster)
	{
		if (buyBoosterPopup != null)
		{
			// Pause game
			SetPaused(true);

			// Show popup
			buyBoosterPopup.Show(booster.Type, () => {
				// Update booster
				UpdateBooster(booster.Type);

				// Resume game
				SetPaused(false);
			});
		}
	}
	
	public void OnBoosterReleased(Booster booster)
	{
		// Get type
		BoosterType type = booster.Type;

		// Deploy
		if (type.IsDeploy())
		{
			HideEmptyOverlays();
		}

		// Hammer
		else if (type.IsHammer())
		{
			HideFootholdOverlays();
		}

		// Hint
		else if (type.IsHint())
		{
			if (_userData.Hint > 0)
			{
				if (Hint())
				{
					DecreaseHint();
				}
			}
			else
			{
				//Log.Debug("No more Hint!");
			}
		}
		
		// Undo
		else if (type.IsUndo())
		{
			if (_userData.Undo > 0)
			{
				if (Undo())
				{
					DecreaseUndo();
				}
			}
			else
			{
				//Log.Debug("No more Undo!");
			}
		}
	}

	public void OnBoosterTouchBegan(Booster booster, Vector3 position)
	{
		// Get type
		BoosterType type = booster.Type;

		if (_dummyBooster.activeSelf)
		{
			_dummyBooster.StopAction(true);
		}

		if (type.IsDeploy())
		{
			_dummyBooster   = dummyDeploy;
			_onBoosterMoved = OnDeployMoved;

			// Deploy tutorial
			if (_isDeployTutorial)
			{
				// Hide hand
				HideHand();
			}
		}
		else if (type.IsHammer())
		{
			_dummyBooster   = dummyHammer;
			_onBoosterMoved = OnHammerMoved;

			// Hammer tutorial
			if (_isHammerTutorial)
			{
				// Hide hand
				HideHand();
			}
		}
		else
		{
			_onBoosterMoved = OnNullMoved;
		}

		// Set dummy delta y
		_dummyDeltaY = type.GetDummyDeltaY();

		// Set dummy AABB
		type.GetDummyAABB(_mapScale, out _dummyBoosterLeft, out _dummyBoosterTop, out _dummyBoosterRight, out _dummyBoosterBottom);

		position.y += _dummyDeltaY;
		
		_dummyBooster.transform.position = position;
		_dummyBooster.Show();
	}
	
	public void OnBoosterTouchMoved(Vector3 position)
	{
		_onBoosterMoved(position);
	}

	public void OnBoosterTouchEnded(Booster booster, Vector3 position)
	{
		position.y += _dummyDeltaY;
		
		_dummyBooster.transform.position = position;

		float left   = position.x + _dummyBoosterLeft;
		float right  = position.x + _dummyBoosterRight;
		float bottom = position.y + _dummyBoosterBottom;
		float top    = position.y + _dummyBoosterTop;
		
		int row = -1;
		int column = -1;

		bool isValid = false;

		// Get type
		BoosterType type = booster.Type;
 		
		// Hide selected cell
		mapOverlay.SetSelectedCell(-1, -1);

		// Deploy
		if (type.IsDeploy())
		{
			HideEmptyOverlays();

			if (_board.GetCell(left, top, right, bottom, CanBuild, ref row, ref column))
			{
				AddFoothold(row, column, FootholdType.Normal);

				DecreaseDeploy();
				
				isValid = true;

				if (_isDeployTutorial)
				{
					_isDeployTutorial = false;
					
					// Resume game
					SetPaused(false);
					
					// Enable boosters
					SetBoostersInteractable(true);
				}
			}
		}
		// Hammer
		else if (type.IsHammer())
		{
			HideFootholdOverlays();

			if (_board.GetCell(left, top, right, bottom, CanDestroy, ref row, ref column))
			{
				Foothold foothold = _footholds[row, column];
				foothold.OnHammerStart();

				if (hammerPrefab != null)
				{
					GameObject hammer = Instantiate(hammerPrefab);
					hammer.transform.SetParent(_board.transform);
					hammer.transform.localScale = defaultScale;

					HammerScript script = hammer.GetComponentInChildren<HammerScript>();
					if (script != null)
					{
						script.Foothold = foothold;
					}

					script.SetHammerPosition(foothold.transform.localPosition);
				}
				else
				{
					foothold.OnHammerEnd();
				}

				DecreaseHammer();
				
				isValid = true;

				if (_isHammerTutorial)
				{
					_isHammerTutorial = false;

					// Resume game
					SetPaused(false);

					// Enable boosters
					SetBoostersInteractable(true);
				}
			}
		}

		if (isValid)
		{
			// Hide dummy booster
			_dummyBooster.Hide();
		}
		else
		{
			Vector3 endPosition = booster.transform.position;
			float duration = Helper.GetDuration(position, endPosition, dummyMoveSpeed);
//			Log.Debug("Duration: " + duration);
			
			var move = MoveAction.MoveTo(endPosition, Mathf.Clamp(duration, minDummyMoveDuration, maxDummyMoveDuration));
			var hide = HideAction.Create();
			var action = SequenceAction.Create(move, hide);
			
			_dummyBooster.Play(action, () => {
				if (_isHammerTutorial)
				{
					PlayHammerTutorialCallback();
				}
				else if (_isDeployTutorial)
				{
					PlayDeployTutorialCallback();
				}
			});
		}
	}

	void OnDeployMoved(Vector3 position)
	{
		position.y += _dummyDeltaY;

		_dummyBooster.transform.position = position;

		float left   = position.x + _dummyBoosterLeft;
		float right  = position.x + _dummyBoosterRight;
		float bottom = position.y + _dummyBoosterBottom;
		float top    = position.y + _dummyBoosterTop;

		int row = -1;
		int column = -1;

		_board.GetCell(left, top, right, bottom, CanBuild, ref row, ref column);

		mapOverlay.SetSelectedCell(row, column);
	}
	
	void OnHammerMoved(Vector3 position)
	{
		position.y += _dummyDeltaY;

		_dummyBooster.transform.position = position;

		float left   = position.x + _dummyBoosterLeft;
		float right  = position.x + _dummyBoosterRight;
		float bottom = position.y + _dummyBoosterBottom;
		float top    = position.y + _dummyBoosterTop;
		
		int row = -1;
		int column = -1;
		
		_board.GetCell(left, top, right, bottom, CanDestroy, ref row, ref column);
		
		mapOverlay.SetSelectedCell(row, column);
	}
	
	void OnNullMoved(Vector3 position)
	{

	}

	bool CanBuild(int row, int column)
	{
		if (!IsIdle()) return false;

		if (_isDeployTutorial)
		{
			return (row == _deployRow && column == _deployColumn);
		}

		return _footholds[row, column] == null;
	}
	
	bool CanDestroy(int row, int column)
	{
		if (!IsIdle()) return false;

		if (_isHammerTutorial)
		{
			return (row == _hammerRow && column == _hammerColumn);
		}

		return (_footholds[row, column] != null && !_footholds[row, column].Locking && !_animal.IsAt(row, column));
	}

	#endregion // IBoosterEventListener

	#region Tutorial

	bool IsControlTutorial()
	{
		return _userData.Map == 1;
	}

	bool IsLongJumpTurotial()
	{
		return _userData.Map == 2;
	}

	bool PlayControlTutorial(float delay = 0)
	{
		// Get next foothold
		Foothold next = GetNextFoothold();
		
		if (next != null)
		{
			// Disable interaction
			SetInteractable(false);

			// Highlight foothold
			HighlightFoothold(delay, next, () => {
				// Lock cells
				_board.SetEnabledCell(next.Row, next.Column);

				// Get direction
				Direction direction = GetDirection(_animal.Row, _animal.Column, next.Row, next.Column);

				// Lock direction
				_board.SetEnabledDirection(direction);

				// Show hand
				Show(_hand);

				_hand.transform.position = _board.GetPosition(next.Row - 1, next.Column - 1);
				_hand.SetAlpha(0, true);
				_hand.Play(FadeAction.RecursiveFadeIn(handFadeDuration));

				// Enable interaction
				SetInteractable(true);
			});

			return true;
		}

		return false;
	}

	bool IsPlayUndoTutorial()
	{
		// no need to play this tutorial
		return false;
		//return (!_isPlayedUndoTutorial && _userData.RemainingUndoTutorial > 0 && _userData.Undo > 0 && _undoStack.Count > 0);
	}

	void PlayUndoTutorial()
	{
		_isUndoTutorial = true;
		
		// Set played
		_isPlayedUndoTutorial = true;
		
		// Decrease tutorial
		_userData.RemainingUndoTutorial--;

		StartBoosterTutorial(BoosterType.Undo);
	}
	
	bool IsPlayHintTutorial()
	{
		return (!_userData.PlayedHintTutorial && _userData.Hint > 0 && CanJump());
	}
	
	void PlayHintTutorial()
	{
		_isHintTutorial = true;

		_hintTime = -1;
		
		// Set played tutorial
		_userData.PlayedHintTutorial = true;

		StartBoosterTutorial(BoosterType.Hint);
	}

	bool IsPlayHammerTutorial()
	{
		return (!_userData.PlayedHammerTutorial &&  _userData.Hammer > 0 && _userData.Map == Settings.HammerTutorialLevel);
	}

	void PlayHammerTutorial()
	{
		// Set played tutorial
		_userData.PlayedHammerTutorial = true;

		// Pause game
		SetPaused(true);

		// Set touch enabled
		SetTouchEnabled(true);
		
		// Disable board
		_board.Interactable = false;
		
		// Disable boosters
		SetBoostersInteractable(false);

		// Get booster
		Booster hammer = GetBooster(BoosterType.Hammer);
		hammer.Interactable = true;

		// Set end position
		_handEndPos = _board.GetPosition(_hammerRow, _hammerColumn) + boosterHandOffset2;

		// Create hand
		_hand = CreateHand2(hammer);

		PlayHammerTutorialCallback();
	}

	void PlayHammerTutorialCallback()
	{
		// Show hand
		Show(_hand);

		_hand.transform.position = _handStartPos;
		_hand.SetAlpha(0, true);

		var delay1 = DelayAction.Create(handDelay);
		var fadeIn = FadeAction.RecursiveFadeIn(handFadeDuration);
		var delay2 = DelayAction.Create(handDelay);
		var move = QuadBezierAction.BezierTo(new Vector3(_handStartPos.x, _handEndPos.y + 1.0f, 0), _handEndPos, handMoveDuration);
		var delay3 = DelayAction.Create(handDelay);
		var fadeOut = FadeAction.RecursiveFadeOut(handFadeDuration);
		var pose = SetPositionAction.Create(_handStartPos);

		var action = SequenceAction.Create(delay1, fadeIn, delay2, move, delay3, fadeOut, pose);

//		_hand.Play(RepeatAction.RepeatForever(action, false));
		_hand.Play(action, PlayHammerTutorialCallback);
	}
	
	bool IsPlayDeployTutorial()
	{
		return (!_userData.PlayedDeployTutorial &&  _userData.Deploy > 0 && _userData.Map == Settings.DeployTutorialLevel);
	}
	
	void PlayDeployTutorial()
	{
		// Set played tutorial
		_userData.PlayedDeployTutorial = true;
		
		// Pause game
		SetPaused(true);
		
		// Set touch enabled
		SetTouchEnabled(true);
		
		// Disable board
		_board.Interactable = false;
		
		// Disable boosters
		SetBoostersInteractable(false);
		
		// Get booster
		Booster deploy = GetBooster(BoosterType.Deploy);
		deploy.Interactable = true;
		
		// Set end position
		_handEndPos = _board.GetPosition(_deployRow, _deployColumn) + boosterHandOffset2;
		
		// Create hand
		_hand = CreateHand2(deploy);
		
		PlayDeployTutorialCallback();
	}
	
	void PlayDeployTutorialCallback()
	{
		// Show hand
		Show(_hand);
		
		_hand.transform.position = _handStartPos;
		_hand.SetAlpha(0, true);
		
		var delay1 = DelayAction.Create(handDelay);
		var fadeIn = FadeAction.RecursiveFadeIn(handFadeDuration);
		var delay2 = DelayAction.Create(handDelay);
		var move = QuadBezierAction.BezierTo(new Vector3(_handStartPos.x, _handEndPos.y + 1.0f, 0), _handEndPos, handMoveDuration);
		var delay3 = DelayAction.Create(handDelay);
		var fadeOut = FadeAction.RecursiveFadeOut(handFadeDuration);
		var pose = SetPositionAction.Create(_handStartPos);
		
		var action = SequenceAction.Create(delay1, fadeIn, delay2, move, delay3, fadeOut, pose);
		
//		_hand.Play(RepeatAction.RepeatForever(action, false));
		_hand.Play(action, PlayDeployTutorialCallback);
	}

	GameObject CreateHand(Booster booster)
	{
		GameObject hand = handPrefab.Create(_board.transform, booster.transform.position + boosterHandOffset);
		hand.transform.localRotation = Quaternion.Euler(0, 0, -90.0f);
		hand.SetAlpha(0, true);
		
		hand.Play(FadeAction.RecursiveFadeIn(handFadeDuration));

		return hand;
	}
	
	GameObject CreateHand2(Booster booster)
	{
		_handStartPos = booster.transform.position + boosterHandOffset2;
		_handStartPos.y += 2.0f;

		GameObject hand = handPrefab.Create(_board.transform, _handStartPos);
		hand.GetComponentInChildren<Animator>().SetTrigger(giuIm);

		return hand;
	}

	void Show(GameObject go)
	{
		if (go.activeSelf)
		{
			go.StopAction();
		}
		else
		{
			go.Show();
		}
	}

	void Hide(GameObject go)
	{
		if (go.activeSelf)
		{
			go.StopAction();
			go.Hide();
		}
	}

	void HideHand()
	{
		if (_hand.activeSelf)
		{
			// Stop action
			_hand.StopAction();
			
			// Play action
			_hand.Play(FadeAction.RecursiveFadeOut(handFadeDuration), () => {
				// Hide hand
				_hand.Hide();
			});
		}
	}

	void HighlightFoothold(float delay, Foothold foothold, Action callback)
	{
		// Set inner position
		tutorialHighlight.innerPosition = foothold.transform.position;
		
		GameObject highlight = tutorialHighlight.gameObject;

		// Show highlight
		Show(highlight);
		
		var action = LerpFloatAction.Create(tutorialHighlight.InnerSize, squareSize, highlightDuration, (size) => {
			tutorialHighlight.InnerSize = size;
		});

		// Play action
		if (delay > 0)
		{
			highlight.Play(SequenceAction.Create(DelayAction.Create(delay), action), callback);
		}
		else
		{
			highlight.Play(action, callback);
		}
	}
	
	void HighlightBooster(Booster booster, Action callback)
	{
		// Set outer size
		tutorialHighlight.outerSize = _cameraSize;
		
		// Set inner size
		tutorialHighlight.InnerSize = _highlightOutSize;

		// Set inner position
		tutorialHighlight.innerPosition = booster.transform.position;

		GameObject highlight = tutorialHighlight.gameObject;
		
		// Show highlight
		Show(highlight);
		
		var action = LerpFloatAction.Create(tutorialHighlight.InnerSize, squareSize, boosterHighlightDuration, (size) => {
			tutorialHighlight.InnerSize = size;
		});
		
		// Play action
		highlight.Play(SequenceAction.Create(DelayAction.Create(boosterHighlightDelay), action), callback);
	}

	void UnhighlightTutorial()
	{
		GameObject highlight = tutorialHighlight.gameObject;
		highlight.StopAction();
		
		// Play action
		highlight.Play(LerpFloatAction.Create(tutorialHighlight.InnerSize, _highlightOutSize, unhighlightDuration, (size) => {
			tutorialHighlight.InnerSize = size;
		}), () => {
			// Hide highlight
			highlight.Hide();
		});

		// Hide hand
		HideHand();
	}

	void StartBoosterTutorial(BoosterType type)
	{
		// Disable interaction
		SetInteractable(false);

		// Pause game
		SetPaused(true);
		
		// Get booster
		Booster booster = GetBooster(type);
		
		// Highlight booster
		HighlightBooster(booster, () => {
			// Create hand
			_hand = CreateHand(booster);
			
			// Enable interaction
			SetInteractable(true);
			
			// Disable board
			_board.Interactable = false;
			
			// Disable boosters
			SetBoostersInteractable(false);

			// Enable tutorial booster
			booster.Interactable = true;
		});
	}

	void EndBoosterTutorial()
	{
		// Unhighlight tutorial
		UnhighlightTutorial();
		
		// Resume game
		SetPaused(false);

		// Enable board
		_board.Interactable = true;

		// Enable boosters
		SetBoostersInteractable(true);
	}

	Foothold GetFoothold(int row, int column)
	{
		Assert.IsInRange(row, 0, _gridHeight - 1, "E1234343212: Row out of range!");
		Assert.IsInRange(column, 0, _gridWidth - 1, "E8787676321: Column out of range!");

		return _footholds[row, column];
	}

	Booster GetBooster(BoosterType type)
	{
		for (int i = 0; i < _boosters.Length; i++)
		{
			if (_boosters[i].Type == type)
			{
				return _boosters[i];
			}
		}

		return null;
	}

	void SetBoostersInteractable(bool interactable)
	{
		for (int i = 0; i < _boosters.Length; i++)
		{
			_boosters[i].Interactable = interactable;
		}
	}

	#endregion // Tutorial

#if SHOW_DEBUG
	private double _lastTime = 0.0;
	private bool _showDebug = false;
	
	void OnGUI()
	{
		if (!_showDebug)
		{
			if (GUI.Button(new Rect(0, 40, 100, 100), "", GUIStyle.none))
			{
				double time = System.DateTime.Now.TimeOfDay.TotalSeconds;
				
				if (time - _lastTime < 0.35)
				{
					_lastTime = 0;
					
					_showDebug = !_showDebug;
				}
				else
				{
					_lastTime = time;
				}
			}
		}
		
		if (_showDebug)
		{
			GUILayout.BeginArea(new Rect(10, 50, Screen.width, Screen.height));
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();

			GUI.enabled = (_userData.Mana < Settings.MaxMana);
			if (GUILayout.Button("Get Mana"))
			{
				_userData.Mana++;
			}
			GUI.enabled = true;

			GUI.enabled = (_userData.Mana > 0);
			if (GUILayout.Button("Clear Mana"))
			{
				NotificationManager.ManaChanged(0);
			}
			GUI.enabled = true;

#if UNITY_EDITOR
			GUI.enabled = (_userData.Mana < Settings.MaxMana);
			if (GUILayout.Button("Cheat Mana"))
			{
				TimeManager.CheatManaCountdown();
			}
			GUI.enabled = true;
#endif
			
			if (GUILayout.Button("Get Coin"))
			{
				_userData.Coin += 10;
			}
			
			if (_userData.Undo < 0)
			{
				if (GUILayout.Button("Unlock Undo"))
				{
					UnlockBooster(BoosterType.Undo);
				}
			}
			else if (GUILayout.Button("Get Undo"))
			{
				AddUndo(1);
			}
			
			if (_userData.Hint < 0)
			{
				if (GUILayout.Button("Unlock Hint"))
				{
					UnlockBooster(BoosterType.Hint);
				}
			}
			else if (GUILayout.Button("Get Hint"))
			{
				AddHint(1);

				if (_hintTime < 0 && !_animal.Jumping && IsPlayHintTutorial())
				{
					_hintTime = 0;
				}
			}

			if (_userData.Hammer < 0)
			{
				if (GUILayout.Button("Unlock Hammer"))
				{
					UnlockBooster(BoosterType.Hammer);
				}
			}
			else if (GUILayout.Button("Get Hammer"))
			{
				AddHammer(1);
			}
			
			if (_userData.Deploy < 0)
			{
				if (GUILayout.Button("Unlock Deploy"))
				{
					UnlockBooster(BoosterType.Deploy);
				}
			}
			else if (GUILayout.Button("Get Deploy"))
			{
				AddDeploy(1);
			}

			if (GUILayout.Button("Reset Undo Tutorial"))
			{
				UserData.Instance.RemainingUndoTutorial = Settings.UndoTutorialTotal;
			}
			
			if (GUILayout.Button("Reset Hint Tutorial"))
			{
				UserData.Instance.PlayedHintTutorial = false;

				if (_hintTime < 0 && !_animal.Jumping && IsPlayHintTutorial())
				{
					_hintTime = 0;
				}
			}
			
			if (GUILayout.Button("Reset Hammer Tutorial"))
			{
				UserData.Instance.PlayedHammerTutorial = false;
			}
			
			if (GUILayout.Button("Reset Deploy Tutorial"))
			{
				UserData.Instance.PlayedDeployTutorial = false;
			}

			if (!_isGameEnded)
			{
				if (GUILayout.Button("Replay"))
				{
					Replay();
				}

				if (GUILayout.Button("Win"))
				{
					WinGame();
				}

				if (GUILayout.Button("Lose"))
				{
					LoseGame();
				}
			}
			
			if (GUILayout.Button("Close"))
			{
				_showDebug = false;
			}
			
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
	}
#endif

//	void OnDrawGizmos()
//	{
//		if (_dummyBooster != null && _dummyBooster.activeSelf)
//		{
//			Vector3 position = _dummyBooster.transform.position;
//			GizmosHelper.DrawRect(position.x + _dummyBoosterLeft, position.y + _dummyBoosterTop, position.x + _dummyBoosterRight, position.y + _dummyBoosterBottom, Color.blue);
//		}
//	}
}

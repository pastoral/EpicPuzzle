using UnityEngine;

public class Foothold : MonoBehaviour
{
	private static readonly Vector3 hintScale = new Vector3(1.5f, 1.5f, 1.5f);

	private static readonly float[] sinkTimes  = new float[] { 0.0f, 0.2f, 0.3f, 0.4f };
	private static readonly float[] sinkValues = new float[] { 0.0f, -0.1f, 0.05f, 0.0f };

	private static readonly float shakeSpeed = 0.015f;
	private static readonly float shakeLeft  = 0.01f;
	private static readonly float shakeRight = 0.01f;
	private static readonly float shakeVar   = 0.001f;

	private static readonly float heaveSpeed = 0.03f;
	private static readonly float heaveUp    = 0.05f;
	private static readonly float heaveDown  = 0.02f;
	private static readonly float heaveVar   = 0.005f;

	private static readonly float sinkDelta  = 0.1f;

	/// <summary>
	/// The wave prefab.
	/// </summary>
	public GameObject wavePrefab;

	// The foothold type
	protected FootholdType _type = FootholdType.Normal;

	// The row
	protected int _row;

	// The column
	protected int _column;

	// The animal
	protected Animal _animal;

	// The current position
	protected Vector3 _position;
	
	// The start x
	protected float _startX;
	
	// The next x
	protected float _nextX;
	
	// The current x
	protected float _x;

	// The start y
	protected float _startY;
	
	// The next y
	protected float _nextY;

	// The current y
	protected float _y;

	// The sink helper
	protected TimelineFloatHelper _sinkHelper = new TimelineFloatHelper(sinkTimes, sinkValues);

	protected bool _isLocking;
	protected bool _swingEnabled;

	// Get type
	public FootholdType Type
	{
		get { return _type; }
	}
	
	// Get "normalize" type (for AI)
	public FootholdType NormalizeType
	{
		get
		{
			if (_type.IsDouble())
			{
				if (GetCount() == 1)
				{
					return FootholdType.Normal;
				}
			}
			else if (_type.IsTime())
			{
				return FootholdType.Normal;
			}

			return _type;
		}
	}

	// Get row
	public int Row
	{
		get { return _row; }
	}

	// Get column
	public int Column
	{
		get { return _column; }
	}

	// Get cell
	public Cell Cell
	{
		get { return new Cell(_row, _column); }
	}

	// Get direction
	public virtual Direction Direction
	{
		get
		{
			return Direction.None;
		}
	}

	// Check if locking
	public bool Locking
	{
		get { return _isLocking; }
	}

	void Start()
	{
		// Get position
		_position = transform.position;

		// Set start x
		_startX = _position.x;
		
		// Set next x
		_nextX = _startX - shakeLeft + shakeVar.GetRandom();
		
		// Set current x
		_x = _startX;

		// Set start y
		_startY = _position.y;

		// Set next y
		_nextY = _startY + heaveUp + heaveVar.GetRandom();

		// Set current y
		_y = _startY;
	}

	virtual public void Construct(int row, int column, bool undo)
	{
		// Set row
		_row = row;

		// Set column
		_column = column;

		if (undo)
		{
			// Unsink
			Unsink();
		}
		else
		{
			// Enable swing
			_swingEnabled = true;
		}
	}

	public void OnHint()
	{
		gameObject.StopAction("hint", true);

		var zoom = ScaleAction.ScaleTo(hintScale, 0.1f, Ease.Linear, LerpDirection.PingPong);
		var action = RepeatAction.Create(zoom, 3, false);

		gameObject.Play(action).name = "hint";
	}

	public void OnWrong()
	{
		// Add wave
		AddWave();

		// Sink
		_y = _startY;
		_sinkHelper.Play();
	}

	public void OnHighlight()
	{
		// Add wave
		AddWave();

		// Sink
		_y = _startY;
		_sinkHelper.Play();
	}

	public void OnHammerStart()
	{
		// Set locking
		_isLocking = true;
	}
	
	public void OnHammerEnd()
	{
		// Disable swing
		_swingEnabled = false;

		GameManager.Instance.OnFootholdDestroyed(this);

		gameObject.Play(FadeAction.FadeOut(0.5f), () => {
			SelfDestroy();
		});
	}

	virtual public void OnAnimalEnter(Animal animal)
	{
		// Set animal
		_animal = animal;

		// Set animal position
		_animal.Position = _position;

		// Add wave
		AddWave();

		// Sink
		_sinkHelper.Play();
	}
	
	virtual public void OnAnimalUnenter(Animal animal)
	{
		// Set animal to null
		_animal = null;
	}

	virtual public void OnAnimalExit(Animal animal)
	{
		// Set animal to null
		_animal = null;

		GameManager.Instance.OnAnimalExitFoothold(this);

		// Sink
		Sink();
	}
	
	virtual public void OnAnimalStartUnexit(Animal animal)
	{

	}

	virtual public void OnAnimalDidFinishUnexit(Animal animal)
	{
		// Set animal
		_animal = animal;
		
		// Set animal position
		_animal.Position = _position;
	}

	virtual public bool IsAtomic()
	{
		return true;
	}
	
	virtual public int GetCount()
	{
		return 1;
	}
	
	virtual public void SetPaused(bool paused)
	{
		
	}

	virtual public void Stop()
	{

	}

	virtual protected void OnUpdate(float deltaTime)
	{
		if (!_swingEnabled) return;

		// Shake
		{
			// Move left
			if (_nextX < _startX)
			{
				_x -= shakeSpeed * deltaTime;

				if (_x <= _nextX)
				{
					_x = _nextX;
					_nextX = _startX + shakeRight + shakeVar.GetRandom();
				}
			}
			// Move right
			else
			{
				_x += shakeSpeed * deltaTime;
				
				if (_x >= _nextX)
				{
					_x = _nextX;
					_nextX = _startX - shakeLeft + shakeVar.GetRandom();
				}
			}

			_position.x = _x;
		}

		// Heave
		{
			if (_sinkHelper.IsFinished())
			{
				// Move up
				if (_nextY > _startY)
				{
					_y += heaveSpeed * deltaTime;

					if (_y >= _nextY)
					{
						_y = _nextY;
						_nextY = _startY - heaveDown + heaveVar.GetRandom();
					}
				}
				// Move down
				else
				{
					_y -= heaveSpeed * deltaTime;

					if (_y <= _nextY)
					{
						_y = _nextY;
						_nextY = _startY + heaveUp + heaveVar.GetRandom();
					}
				}
				
				_position.y = _y;
			}
			else
			{
				_position.y = _y + _sinkHelper.Update(deltaTime);
			}
		}

		// Set position
		transform.position = _position;

		// Update animal position
		if (_animal != null)
		{
			_animal.Position = _position;
		}
	}
	
	void Update()
	{
		OnUpdate(Time.deltaTime);
	}

	void AddWave()
	{
		if (wavePrefab != null)
		{
			GameObject wave = Instantiate(wavePrefab);
			wave.transform.SetParent(transform.parent);
			wave.transform.position = transform.position;
		}
	}

	void Sink()
	{
		var delay = DelayAction.Create(0.5f);
		var disableSwing = CallFuncAction.Create(() => { _swingEnabled = false; });
		var move = MoveAction.MoveBy(new Vector3(0, -sinkDelta, 0), 0.5f, Ease.SineIn);
		var fadeOut = FadeAction.RecursiveFadeOut(0.5f);
		
		gameObject.Play(SequenceAction.Create(delay, disableSwing, ParallelAction.ParallelAll(move, fadeOut)), () => {
			SelfDestroy();
		});
	}

	void Unsink()
	{
		gameObject.SetAlpha(0, true);

		Vector3 position = transform.position;
		position.y -= sinkDelta;

		var delay = DelayAction.Create(0.05f);
		var setPosition = SetPositionAction.Create(position);
		var move = MoveAction.MoveBy(new Vector3(0, sinkDelta, 0), 0.5f, Ease.SineOut);
		var fadeIn = FadeAction.RecursiveFadeIn(0.5f);

		gameObject.Play(SequenceAction.Create(delay, setPosition, ParallelAction.ParallelAll(move, fadeIn)), () => { _swingEnabled = true; });
	}

	void SelfDestroy()
	{
		Destroy(gameObject);
	}
}

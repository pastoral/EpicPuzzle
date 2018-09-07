using UnityEngine;
using System;

public class Animal : MonoBehaviour
{
	private static readonly int dungChao  = Animator.StringToHash("DungChao");
	private static readonly int nhayLen   = Animator.StringToHash("Len_NhayTile");
	private static readonly int nhayNgang = Animator.StringToHash("Ngang_NhayTile");
	private static readonly int nhayXuong = Animator.StringToHash("Xuong_NhayTile");
	private static readonly int nhayXong  = Animator.StringToHash("Xong_NhayTile");
	private static readonly int nghiLen   = Animator.StringToHash("Len_Nghi");
	private static readonly int nghiNgang = Animator.StringToHash("Ngang_Nghi");
	private static readonly int nghiXuong = Animator.StringToHash("Xuong_Nghi");
	private static readonly int winXuong  = Animator.StringToHash("Xuong_Win");
	private static readonly int loseXuong = Animator.StringToHash("Xuong_Lose");

	private static readonly int nhayTaiChoLen   = Animator.StringToHash("Len_NhayTaiCho");
	private static readonly int nhayTaiChoNgang = Animator.StringToHash("Ngang_NhayTaiCho");
	private static readonly int nhayTaiChoXuong = Animator.StringToHash("Xuong_NhayTaiCho");

	private static readonly int[] nhayVuiLen = new int[]
	{ 
		Animator.StringToHash("Len_NhayVui_1"),
		Animator.StringToHash("Len_NhayVui_2")
	};

	private static readonly int[] nhayVuiNgang = new int[]
	{
		Animator.StringToHash("Ngang_NhayVui_1"),
		Animator.StringToHash("Ngang_NhayVui_2")
	};

	private static readonly int[] nhayVuiXuong = new int[]
	{
		Animator.StringToHash("Xuong_NhayVui_1"),
		Animator.StringToHash("Xuong_NhayVui_2"),
		Animator.StringToHash("Xuong_NhayVui_3")
	};

	private static readonly float controlHeightFactor = 1.5f;
	private static readonly float controlDeltaY	      = 1.5f;

	/// <summary>
	/// The clothes.
	/// </summary>
	public GameObject clothesLeft;
	public GameObject clothesUp;
	public GameObject clothesDown;

	/// <summary>
	/// The cap.
	/// </summary>
	public GameObject capLeft;
	public GameObject capUp;
	public GameObject capDown;

	/// <summary>
	/// The animator.
	/// </summary>
	public Animator animator;

	/// <summary>
	/// The jump speed (units per second).
	/// </summary>
	public float jumpSpeed = 10.0f;
	
	/// <summary>
	/// The jump delay.
	/// </summary>
	public float jumpDelay = 0.35f;
	
	/// <summary>
	/// The minimum jump duration.
	/// </summary>
	public float minJumpDuration = 0.35f;
	
	/// <summary>
	/// The maximum jump duration.
	/// </summary>
	public float maxJumpDuration = 1.0f;

	/// <summary>
	/// The idle duration.
	/// </summary>
	public float idleDuration = 3.0f;

	// The direction
	private Direction _direction = Direction.None;

	// The row
	private int _row;

	// The column
	private int _column;

	// The current foothold
	private Foothold _foothold;

	// The jump helper
	private QuadBezierHelper _jumpHelper = new QuadBezierHelper();

	// True if jumping
	private bool _isJumping;

	// Is undo?
	private bool _isUndo;

	// The idle time
	private float _idleTime;

	// Is paused?
	private bool _isPaused;

	// The current speed
	private float _speed = 1.0f;

	// The next speed
	private float _nextSpeed = 1.0f;

	// Get/Set direction
	public Direction Direction
	{
		get
		{
			return _direction;
		}
		set
		{
			if (_direction != value)
			{
				_direction = value;

				if (_direction.IsRight())
				{
					animator.SetTrigger(nghiNgang);

					transform.SetHorizontalFlip(true);
				}
				else
				{
					if (_direction.IsLeft())
					{
						animator.SetTrigger(nghiNgang);
					}
					else if (_direction.IsUp())
					{
						animator.SetTrigger(nghiLen);
					}
					else if (_direction.IsDown())
					{
						animator.SetTrigger(nghiXuong);
					}

					transform.SetHorizontalFlip(false);
				}
			}
		}
	}

	public Vector3 Position
	{
		get
		{
			return transform.position;
		}
		set
		{
			transform.position = value;
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

	// Get foothold
	public Foothold Foothold
	{
		get { return _foothold; }
	}

	// Check if animal is jumping
	public bool Jumping
	{
		get { return _isJumping; }
	}
	
	public bool Paused
	{
		get
		{
			return _isPaused;
		}
	}

	public void SetSpeed(float speed)
	{
		_nextSpeed = speed;
	}

	public void ResetSpeed()
	{
		_speed = _nextSpeed = 1.0f;

		animator.speed = _speed;
	}

	public void SetClothes(Sprite left, Sprite up, Sprite down)
	{
		clothesLeft.SetSprite(left);
		clothesUp.SetSprite(up);
		clothesDown.SetSprite(down);
	}
	
	public void SetCap(Sprite left, Sprite up, Sprite down)
	{
		capLeft.SetSprite(left);
		capUp.SetSprite(up);
		capDown.SetSprite(down);
	}

	public bool IsAt(int row, int column)
	{
		return (_row == row && _column == column);
	}

	public void Init(Foothold foothold, Direction direction)
	{
		// Set current foothold
		_foothold = foothold;

		//
		_row    = _foothold.Row;
		_column = _foothold.Column;

		// Set direction
//		this.Direction = direction;
//
//		_foothold.OnAnimalEnter(this);

		_direction = direction;

		// Set start position
		transform.position = foothold.transform.position;
	}

	public bool IsLeft()
	{
		return _direction.IsLeft();
	}
	
	public bool IsRight()
	{
		return _direction.IsRight();
	}
	
	public bool IsUp()
	{
		return _direction.IsUp();
	}
	
	public bool IsDown()
	{
		return _direction.IsDown();
	}

	public void Greet()
	{
		animator.SetTrigger(dungChao);
	}

	public void Play()
	{
		// Play random sound
		SoundManager.Instance.PlayRandomSound(SoundID.Cyrus1, SoundID.Cyrus2, SoundID.Cyrus3, SoundID.Cyrus4);

		Direction direction = _direction;
		_direction = Direction.None;

		// Set direction
		this.Direction = direction;
		
		_foothold.OnAnimalEnter(this);
	}

	public void JumpLeft(Foothold foothold)
	{
		// Set direction
		_direction = Direction.Left;

		animator.SetTrigger(nhayNgang);
		transform.SetHorizontalFlip(false);

		// Set not undo
		_isUndo = false;

		// Jump
		Jump(foothold);
	}
	
	public void JumpRight(Foothold foothold)
	{
		// Set direction
		_direction = Direction.Right;

		animator.SetTrigger(nhayNgang);
		transform.SetHorizontalFlip(true);

		// Set not undo
		_isUndo = false;

		// Jump
		Jump(foothold);
	}
	
	public void JumpUp(Foothold foothold)
	{
		// Set direction
		_direction = Direction.Up;

		animator.SetTrigger(nhayLen);
		transform.SetHorizontalFlip(false);

		// Set not undo
		_isUndo = false;

		// Jump
		Jump(foothold);
	}
	
	public void JumpDown(Foothold foothold)
	{
		// Set direction
		_direction = Direction.Down;

		animator.SetTrigger(nhayXuong);
		transform.SetHorizontalFlip(false);

		// Set not undo
		_isUndo = false;

		// Jump
		Jump(foothold);
	}
	
	public void Unjump(Foothold foothold)
	{
		// Set undo
		_isUndo = true;

		// Jump
		Jump(foothold);
	}

//	public void ForceJump()
//	{
//		// Check if jumping
//		if (_isJumping)
//		{
//			// Fix position
//			transform.position = _foothold.transform.position;
//
//			// Set not jumping
//			_isJumping = false;
//
//			if (_isUndo)
//			{
//				_foothold.OnAnimalDidFinishUnexit(this);
//
//				GameManager.Instance.OnAnimalDidFinishUnjump(this);
//			}
//			else
//			{
//				_foothold.OnAnimalEnter(this);
//
//				GameManager.Instance.OnAnimalDidFinishJump(this);
//			}
//		}
//	}

	public void SetPaused(bool paused)
	{
		_isPaused = paused;

		if (_isJumping)
		{
			animator.enabled = !paused;
		}
	}

	public UndoData Trace()
	{
		return new UndoData(_row, _column, _foothold.Type, _direction);
	}

	public void OnEnterIdle()
	{
		// Set idle time
		_idleTime = idleDuration;
	}

	public void OnWrong()
	{
		if (_direction.IsHorizontal())
		{
			animator.SetTrigger(nhayTaiChoNgang);
		}
		else if (_direction.IsUp())
		{
			animator.SetTrigger(nhayTaiChoLen);
		}
		else if (_direction.IsDown())
		{
			animator.SetTrigger(nhayTaiChoXuong);
		}
	}

	public void OnWinGame()
	{
		animator.SetTrigger(winXuong);
	}
	
	public void OnLoseGame()
	{
		animator.SetTrigger(loseXuong);
	}

	public void JumpToMap(float delay1, Vector3 position, float delay2, Action callback)
	{
		Vector2 start = transform.position;
		Vector2 end   = position;
		Vector2 control;
		Direction direction = DirectionHelper.GetDirection(start, end);

		// Up
		if (direction.IsUp())
		{
			// Left
			if (direction.IsLeft())
			{
				animator.SetTrigger(nhayNgang);
				transform.SetHorizontalFlip(false);

				control = new Vector2((start.x + end.x) * 0.5f, start.y + Mathf.Sqrt(Mathf.Abs(end.x - start.x)) * controlHeightFactor);
			}
			// Right
			else if (direction.IsRight())
			{
				animator.SetTrigger(nhayNgang);
				transform.SetHorizontalFlip(true);

				control = new Vector2((start.x + end.x) * 0.5f, start.y + Mathf.Sqrt(Mathf.Abs(end.x - start.x)) * controlHeightFactor);
			}
			else
			{
				animator.SetTrigger(nhayLen);
				transform.SetHorizontalFlip(false);

				control = new Vector2(start.x, end.y + controlDeltaY);
			}
		}
		else
		{
			// Left
			if (direction.IsLeft())
			{
				animator.SetTrigger(nhayNgang);
				transform.SetHorizontalFlip(false);

				control = new Vector2((start.x + end.x) * 0.5f, start.y + Mathf.Sqrt(Mathf.Abs(end.x - start.x)) * controlHeightFactor);
			}
			// Right
			else if (direction.IsRight())
			{
				animator.SetTrigger(nhayNgang);
				transform.SetHorizontalFlip(true);

				control = new Vector2((start.x + end.x) * 0.5f, start.y + Mathf.Sqrt(Mathf.Abs(end.x - start.x)) * controlHeightFactor);
			}
			else
			{
				animator.SetTrigger(nhayXuong);
				transform.SetHorizontalFlip(false);

				control = new Vector2(start.x, start.y + controlDeltaY);
			}
		}

		var jump = SequenceAction.Create(QuadBezierAction.BezierTo(control, end, 0.5f), CallFuncAction.Create(() => { animator.SetTrigger(nghiXuong); }));
		BaseAction action = null;

		if (delay1 > 0)
		{
			if (delay2 > 0)
			{
				action = SequenceAction.Create(DelayAction.Create(delay1), jump, DelayAction.Create(delay2));
			}
			else
			{
				action = SequenceAction.Create(DelayAction.Create(delay1), jump);
			}
		}
		else
		{
			if (delay2 > 0)
			{
				action = SequenceAction.Create(jump, DelayAction.Create(delay2));
			}
			else
			{
				action = jump;
			}
		}

		// Jump
		gameObject.Play(action, callback);
	}

	void Jump(Foothold foothold)
	{
		SoundManager.Instance.PlaySound(SoundID.CyrusStartJump);

		if (_isUndo)
		{
			_foothold.OnAnimalUnenter(this);

			foothold.OnAnimalStartUnexit(this);
		}
		else
		{
			_foothold.OnAnimalExit(this);
		}

		// Set current foothold
		_foothold = foothold;

		bool isHorizontal = (_row == _foothold.Row);

		//
		_row    = _foothold.Row;
		_column = _foothold.Column;

		// Construct jump helper
		Vector2 start = transform.position;
		Vector2 end   = _foothold.transform.position;
		Vector2 control;
		float length;

		if (isHorizontal)
		{
			control = new Vector2((start.x + end.x) * 0.5f, start.y + Mathf.Sqrt(Mathf.Abs(end.x - start.x)) * controlHeightFactor);
			length  = Helper.GetQuadBezierLength(start, control, end);
		}
		else
		{
			if (_direction.IsUp())
			{
				if (_isUndo)
				{
					control = new Vector2(start.x, start.y + controlDeltaY);
					length  = start.y - end.y + controlDeltaY;
				}
				else
				{
					control = new Vector2(start.x, end.y + controlDeltaY);
					length  = end.y - start.y + controlDeltaY;
				}
			}
			else
			{
				if (_isUndo)
				{
					control = new Vector2(start.x, end.y + controlDeltaY);
					length  = end.y - start.y + controlDeltaY;
				}
				else
				{
					control = new Vector2(start.x, start.y + controlDeltaY);
					length  = start.y - end.y + controlDeltaY;
				}
			}
		}

		float duration = Mathf.Clamp(length / jumpSpeed, minJumpDuration, maxJumpDuration);
		
		_jumpHelper.Construct(start, control, end, duration, jumpDelay);

		// Set jumping
		_isJumping = true;
	}

	void Update()
	{
		if (_isPaused) return;

		if (_isJumping)
		{
			float deltaTime = Time.deltaTime;

			if (_speed != _nextSpeed)
			{
				_speed = Mathf.Lerp(_speed, _nextSpeed, deltaTime * 5.0f);

				if (Mathf.Abs(_nextSpeed - _speed) < 0.001f)
				{
					_speed = _nextSpeed;
				}

				animator.speed = _speed;
			}

			// Update position
			transform.position = _jumpHelper.Update(deltaTime * _speed);

			// Check if finished
			if (_jumpHelper.IsFinished())
			{
				SoundManager.Instance.PlaySound(SoundID.CyrusFinishJump);

				animator.SetTrigger(nhayXong);

				_isJumping = false;

				if (_isUndo)
				{
					_foothold.OnAnimalDidFinishUnexit(this);

					GameManager.Instance.OnAnimalDidFinishUnjump(this);
				}
				else
				{
					_foothold.OnAnimalEnter(this);

					if (!_foothold.Type.IsRedirect())
					{
						GameManager.Instance.OnAnimalDidFinishJump(this);
					}
				}
			}
		}
		else if (_idleTime > 0)
		{
			_idleTime -= Time.deltaTime;

			if (_idleTime <= 0)
			{
				if (_direction.IsUp())
				{
					animator.SetTrigger(nhayVuiLen.Any());
				}
				else if (_direction.IsHorizontal())
				{
					animator.SetTrigger(nhayVuiNgang.Any());
				}
				else
				{
					animator.SetTrigger(nhayVuiXuong.Any());
				}
			}
		}
	}
}

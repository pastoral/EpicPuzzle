using UnityEngine;

public class TimeFoothold : Foothold
{
	private static readonly Vector3 alarmScale = new Vector3(2.0f, 2.0f, 1.0f);
	private static readonly Color alarmColor = new Color(1.0f, 0.0f, 0.0f);
	private static readonly float alarmDuration = 0.5f;

	/// <summary>
	/// The duration.
	/// </summary>
	[SerializeField]
	private int _duration = 5;

	/// <summary>
	/// The frame.
	/// </summary>
	public GameObject frame;

	/// <summary>
	/// The number.
	/// </summary>
	public AtlasNumber number;

	/// <summary>
	/// The number effect.
	/// </summary>
	public AtlasNumber numberEffect;

	// The remaining time
	private float _time;

	// The current time
	private int _currentTime;

	// Is countdown enabled?
	private bool _countdownEnabled;

	// Is paused?
	private bool _isPaused;

	public int Duration
	{
		get
		{
			return _duration;
		}
		set
		{
			_duration = value;

			// Set time
			_time = _duration;
			
			// Reset current time
			_currentTime = 0;
			
			// Update time
			UpdateTime();
		}
	}

	void OnValidate()
	{
		if (number != null)
		{
			number.Number = _duration;
		}
	}

	public override void Construct(int row, int column, bool undo)
	{
		// Set type
		_type = FootholdType.Time;

		if (undo)
		{
			_duration = GameManager.Instance.GetTimeFootholdDuration(row, column);
		}

		// Set time
		_time = _duration;

		// Update time
		UpdateTime();

		base.Construct(row, column, undo);
	}
	
	public override void OnAnimalEnter(Animal animal)
	{
		// Play sound
		SoundManager.Instance.PlaySound(SoundID.Tick, SoundType.Loop);

		// Start countdown
		_countdownEnabled = true;

		base.OnAnimalEnter(animal);
	}
	
	public override void OnAnimalUnenter(Animal animal)
	{
		// Stop sound
		SoundManager.Instance.StopSound(SoundID.Tick);

		// Stop countdown
		_countdownEnabled = false;

		// Hide frame
		if (frame.activeSelf)
		{
			frame.StopAction();
			frame.Hide();
		}

		// Hide effect
		GameObject effect = numberEffect.gameObject;

		if (effect.activeSelf)
		{
			effect.StopAction();
			effect.Hide();
		}

		// Reset time
		_time = _duration;
		
		// Update time
		UpdateTime();

		base.OnAnimalUnenter(animal);
	}
	
	public override void OnAnimalExit(Animal animal)
	{
		// Stop sound
		SoundManager.Instance.StopSound(SoundID.Tick);

		// Stop countdown
		_countdownEnabled = false;

		base.OnAnimalExit(animal);
	}
	
	public override void OnAnimalDidFinishUnexit(Animal animal)
	{
		base.OnAnimalDidFinishUnexit(animal);

		// Play sound
		SoundManager.Instance.PlaySound(SoundID.Tick, SoundType.Loop);

		// Start countdown
		_countdownEnabled = true;
	}
	
	public override void SetPaused(bool paused)
	{
		base.SetPaused(paused);

		if (_countdownEnabled)
		{
			if (paused)
			{
				// Stop sound
				SoundManager.Instance.StopSound(SoundID.Tick);
			}
			else
			{
				// Play sound
				SoundManager.Instance.PlaySound(SoundID.Tick, SoundType.Loop);
			}
		}

		_isPaused = paused;
	}

	public override void Stop()
	{
		// Stop sound
		SoundManager.Instance.StopSound(SoundID.Tick);

		// Stop countdown
		_countdownEnabled = false;

		base.Stop();
	}

	override protected void OnUpdate(float deltaTime)
	{
		base.OnUpdate(deltaTime);

		if (_isPaused) return;

		if (_countdownEnabled)
		{
			if (_time > 0)
			{
				_time -= Time.deltaTime;

				// Update time
				UpdateTime();

				if (_time <= 0)
				{
					// Stop sound
					SoundManager.Instance.StopSound(SoundID.Tick);

					// Stop countdown
					_countdownEnabled = false;

					// Play sound
					SoundManager.Instance.PlaySound(SoundID.TimeOut);

					GameManager.Instance.OnFootholdExpired(this);
				}
			}
		}
	}

	void UpdateTime()
	{
		// Get current time
		int time = Mathf.CeilToInt(_time);

		if (time != _currentTime)
		{
			if (_currentTime > 0)
			{
				// Frame
				frame.StopAction();
				frame.Show();
				frame.Play(BlinkAction.Create(2, 0.3f, false, false), () => { frame.Hide(); });

				// Play effect
				GameObject effect = numberEffect.gameObject;
				effect.StopAction(true);
				effect.Show();
				effect.transform.localScale = Vector3.one;
				effect.SetColor(_currentTime > 3 ? Color.white : alarmColor, true);

				numberEffect.Number = _currentTime;

				var zoomOut = ScaleAction.ScaleTo(alarmScale, alarmDuration);
				var fadeOut = FadeAction.RecursiveFadeOut(alarmDuration);
				var hide = HideAction.Create();

				effect.Play(SequenceAction.Create(ParallelAction.ParallelAll(zoomOut, fadeOut), hide));
			}

			// Set current time
			_currentTime = time;

			// Update number
			number.Number = time;
		}
	}
}

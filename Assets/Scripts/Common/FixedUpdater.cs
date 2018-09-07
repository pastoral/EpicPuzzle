using UnityEngine;
using System;

public class FixedUpdater
{
	// The update callback
	private Action _updateCallback;

	// The fixed time
	private float _fixedTime = 0.02f;

	// True if playing
	private bool _isPlaying;

	// The time counter
	private float _time;

	public bool Playing
	{
		get
		{
			return _isPlaying;
		}
	}

	/// <summary>
	/// Gets or sets the update callback.
	/// </summary>
	public Action UpdateCallback
	{
		get
		{
			return _updateCallback == OnUpdate ? null : _updateCallback;
		}
		set
		{
			_updateCallback = (value == null) ? OnUpdate : value;
		}
	}

	/// <summary>
	/// Gets or sets the fixed time.
	/// </summary>
	public float FixedTime
	{
		get
		{
			return _fixedTime;
		}
		set
		{
			_fixedTime = value;
		}
	}

	public FixedUpdater(float fixedTime = 0.02f)
	{
		// Set update callback
		_updateCallback = OnUpdate;

		// Set fixed time
		_fixedTime = fixedTime;
	}
	
	public FixedUpdater(Action updateCallback, float fixedTime = 0.02f)
	{
		// Set update callback
		this.UpdateCallback = updateCallback;
		
		// Set fixed time
		_fixedTime = fixedTime;
	}

	public void Play()
	{
		_isPlaying = true;
	}

	public void Play(Action updateCallback)
	{
		// Set update callback
		this.UpdateCallback = updateCallback;

		_isPlaying = true;
	}

	public void Stop()
	{
		_isPlaying = false;
	}

	public void Update()
	{
		if (_isPlaying)
		{
			_time += Time.deltaTime;

			if (_time >= _fixedTime)
			{
				_time = 0;

				_updateCallback();
			}
		}
	}

	void OnUpdate()
	{

	}
}

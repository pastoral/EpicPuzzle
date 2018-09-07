///////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System;

public class ActionScript : MonoBehaviour
{
	// The current action
	private BaseAction _action = NullAction.Instance;

	// The callback
	private Action _callback;

	// Self-destroy when action finished or not
	private bool _isSelfDestroy;

	private bool _isPaused;

	private bool _isFinished = true;

	public bool Paused
	{
		get
		{
			return _isPaused;
		}
		set
		{
			_isPaused = value;
		}
	}

	// Play the specified action
	public void Play(BaseAction action, Action callback = null, bool selfDestroy = true)
	{
		// Set current action
		_action = action;

		// Set callback
		_callback = callback;

		// Set self-destroy
		_isSelfDestroy = selfDestroy;

		// Play action
		_action.Play(gameObject);

		// Set not paused
		_isPaused = false;

		_isFinished = _action.IsFinished();

		// Check if action finished
		if (_isFinished)
		{
			if (_callback != null)
			{
				_callback();
			}

			if (selfDestroy)
			{
				Destroy(this);
			}
		}
	}
	
	// Replay action
	public void Replay(bool reset = true)
	{
		_action.Replay(gameObject, reset);

		// Set not paused
		_isPaused = false;

		_isFinished = _action.IsFinished();
	}

	// Stop action
	public void Stop(bool forceEnd = false)
	{
		if (!_isFinished)
		{
			_action.Stop(forceEnd);

			_isFinished = true;
			
//			if (_callback != null)
//			{
//				_callback();
//			}
			
			if (_isSelfDestroy)
			{
				Destroy(this);
			}
		}
	}

	void Update()
	{
		if (_isPaused || _isFinished)
		{
			return;
		}

		if (_action.Update(Time.deltaTime))
		{
			_isFinished = true;

			if (_callback != null)
			{
				_callback();
			}
			
			if (_isSelfDestroy)
			{
				Destroy(this);
			}
		}
	}
}

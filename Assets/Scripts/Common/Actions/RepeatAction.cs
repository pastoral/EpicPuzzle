using UnityEngine;

public class RepeatAction : BaseAction
{
	// The action
	private BaseAction _action;
	
	// The number of repeat (-1=infinite)
	private int _repeatCount = -1;

	// True if reset when replay
	private bool _isReset = true;

	// The target
	private GameObject _target;

	// The remaining repeat
	private int _remaining = 0;
	
	// True if action finished
	private bool _isFinished;

	public RepeatAction(BaseAction action, int count, bool reset)
	{
		Debug.Assert(count != 0, "E123456789: Count must be not zero!");
		
		// Set action
		_action = action;
		
		// Set repeat count
		_repeatCount = count;

		// Set reset
		_isReset = reset;
	}

	public static RepeatAction Create(BaseAction action, int count, bool reset = true)
	{
		return new RepeatAction(action, count, reset);
	}

	public static RepeatAction RepeatForever(BaseAction action, bool reset = true)
	{
		return Create(action, -1, reset);
	}

	public override void Play(GameObject target)
	{
		// Set target
		_target = target;

		// Set remaining repeat
		_remaining = _repeatCount;

		// Set not finished
		_isFinished = false;

		// Play action
		_action.Play(target);
		
		// Check if action finished
		if (_action.IsFinished())
		{
			if (_repeatCount > 0)
			{
				for (int i = 1; i < _repeatCount; i++)
				{
					// Replay action
					_action.Replay(_target, _isReset);
				}
				
				// Set remaining to zero
				_remaining = 0;
				
				// Set finished
				_isFinished = true;
			}
		}
	}

	public override void Reset()
	{
		// Reset action
		_action.Reset();
	}
	
	public override void Stop(bool forceEnd = false)
	{
		if (!_isFinished)
		{
			// Stop action
			_action.Stop(forceEnd);
			
			// Set remaining to zero
			_remaining = 0;
			
			// Set finished
			_isFinished = true;
		}
	}
	
	public override bool IsFinished()
	{
		return _isFinished;
	}
	
	public override bool Update(float deltaTime)
	{
		// Check if action finished
		if (_isFinished)
		{
			return true;
		}
		
		// Update action
		if (_action.Update(deltaTime))
		{
			// Check if repeat forever
			if (_repeatCount < 0)
			{
				// Replay action
				_action.Replay(_target, _isReset);
			}
			else
			{
				// Decrease remaining
				_remaining--;
				
				if (_remaining > 0)
				{
					// Replay action
					_action.Replay(_target, _isReset);
				}
				else
				{
					// Set finished
					_isFinished = true;
				}
			}
		}
		
		return _isFinished;
	}
}

using UnityEngine;
using System;

public class LerpIntAction : BaseAction
{
	// The helper
	private LerpIntHelper _helper = new LerpIntHelper();

	// The callback
	private Action<int> _callback;

	public LerpIntAction(int start, int end, float duration, Easer easer, LerpDirection direction, Action<int> callback)
	{
		// Construct helper
		_helper.Construct(end, duration, easer, direction);

		// Set start
		_helper.Start = start;

		// Set callback
		_callback = callback;
	}

	public static LerpIntAction Create(int start, int end, float duration, Action<int> callback)
	{
		return new LerpIntAction(start, end, duration, null, LerpDirection.Forward, callback);
	}
	
	public static LerpIntAction Create(int start, int end, float duration, Easer easer, Action<int> callback)
	{
		return new LerpIntAction(start, end, duration, easer, LerpDirection.Forward, callback);
	}

	public static LerpIntAction Create(int start, int end, float duration, Easer easer, LerpDirection direction, Action<int> callback)
	{
		return new LerpIntAction(start, end, duration, easer, direction, callback);
	}

	public override void Play(GameObject target)
	{
		_helper.Play();

		_callback(_helper.Start);
	}

	public override void Stop(bool forceEnd = false)
	{
		if (!_helper.IsFinished())
		{
			_helper.Stop();
			
			if (forceEnd)
			{
				_callback(_helper.End);
			}
		}
	}

	public override bool IsFinished()
	{
		return _helper.IsFinished();
	}

	public override bool Update(float deltaTime)
	{
		if (!_helper.IsFinished())
		{
			_callback(_helper.Update(deltaTime));
		}
		
		return _helper.IsFinished();
	}
}

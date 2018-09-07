using UnityEngine;
using System;

public class LerpFloatAction : BaseAction
{
	// The helper
	private LerpFloatHelper _helper = new LerpFloatHelper();

	// The callback
	private Action<float> _callback;

	public LerpFloatAction(float start, float end, float duration, Easer easer, LerpDirection direction, Action<float> callback)
	{
		// Construct helper
		_helper.Construct(end, duration, easer, direction);

		// Set start
		_helper.Start = start;

		// Set callback
		_callback = callback;
	}

	public static LerpFloatAction Create(float start, float end, float duration, Action<float> callback)
	{
		return new LerpFloatAction(start, end, duration, null, LerpDirection.Forward, callback);
	}
	
	public static LerpFloatAction Create(float start, float end, float duration, Easer easer, Action<float> callback)
	{
		return new LerpFloatAction(start, end, duration, easer, LerpDirection.Forward, callback);
	}

	public static LerpFloatAction Create(float start, float end, float duration, Easer easer, LerpDirection direction, Action<float> callback)
	{
		return new LerpFloatAction(start, end, duration, easer, direction, callback);
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

using UnityEngine;

public class DelayAction : BaseAction
{
	// The duration
	private float _duration;
	
	// The variance
	private float _variance;

	// The accumulative time
	private float _time;

	public DelayAction(float duration, float variance)
	{
		// Set duration
		_duration = duration;
		
		// Set variance
		_variance = variance;
	}

	public static DelayAction Create(float duration, float variance = 0)
	{
		return new DelayAction(duration, variance);
	}

	public override void Play(GameObject target)
	{
		// Set time
		_time = _duration.Variance(_variance);
	}

	public override void Stop(bool forceEnd = false)
	{
		// Set finished
		_time = 0;
	}

	public override bool IsFinished()
	{
		return _time <= 0;
	}

	public override bool Update(float deltaTime)
	{
		if (_time > 0)
		{
			// Decrease time
			_time -= deltaTime;
		}
		
		return _time <= 0;
	}
}

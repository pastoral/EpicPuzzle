using UnityEngine;

public class BlinkAction : BaseAction
{
	// The number of blinks
	private int _blinkCount;

	// The duration
	private float _duration;

	// Is loop?
	private bool _isLoop;

	// Is recursive?
	private bool _isRecursive;
	
	// The color adapter
	private ColorAdapter _colorAdapter;

	// The time
	private float _time;

	// Is visible?
	private bool _isVisible;

	// Is finished?
	private bool _isFinished;

	public BlinkAction(int blinkCount, float duration, bool isLoop, bool isRecursive)
	{
		// Set number of blinks
		_blinkCount = blinkCount;

		// Set duration
		_duration = duration;

		// Set loop
		_isLoop = isLoop;

		// Set recursive
		_isRecursive = isRecursive;
	}

	public static BlinkAction Create(int blinkCount, float duration, bool isLoop = false, bool isRecursive = true)
	{
		return new BlinkAction(blinkCount, duration, isLoop, isRecursive);
	}

	public override void Play(GameObject target)
	{
		// Set color adapter
		_colorAdapter = ColorAdapter.Get(target);

		// Set time
		_time = 0;

		// Set visible
		_isVisible = true;

		// Set not finished
		_isFinished = false;

		// Show
		_colorAdapter.SetAlpha(1.0f, _isRecursive);
	}

	public override void Stop(bool forceEnd = false)
	{
		if (!_isFinished)
		{
			if (!_isVisible)
			{
				_colorAdapter.SetAlpha(1.0f, _isRecursive);

				_isVisible = true;
			}

			_isFinished = true;
		}
	}

	public override bool IsFinished()
	{
		return _isFinished;
	}

	public override bool Update(float deltaTime)
	{
		if (_isFinished) return true;

		// Increase time
		_time += deltaTime;

		if (_isLoop)
		{
			while (_time >= _duration)
			{
				_time -= _duration;
			}
		}

		if (_time < _duration)
		{
			float t = _time / _duration;
			float slice = 1.0f / _blinkCount;

			while (t >= slice)
			{
				t -= slice;
			}

			bool isVisible = (t > slice * 0.5f);

			if (isVisible != _isVisible)
			{
				_colorAdapter.SetAlpha(isVisible ? 1.0f : 0.0f, _isRecursive);

				_isVisible = isVisible;
			}
		}
		else
		{
			if (!_isVisible)
			{
				_colorAdapter.SetAlpha(1.0f, _isRecursive);

				_isVisible = true;
			}

			// Set finished
			_isFinished = true;
		}

		return _isFinished;
	}
}

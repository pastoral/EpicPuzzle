using UnityEngine;

public class QuadBezierHelper
{
	// The start point
	private Vector2 _start;
	
	// The control point
	private Vector2 _control;

	// The end point
	private Vector2 _end;

	// The duration
	private float _duration;

	// The delay time
	private float _delay;

	// The easer
	private Easer _easer;

	// The current value
	private Vector2 _value;

	// The accumulative time
	private float _time;
	
	// True if finished
	private bool _isFinished = true;

	public Vector2 End
	{
		get
		{
			return _end;
		}
	}

	public void Construct(Vector2 start, Vector2 control, Vector2 end, float duration, float delay = 0.0f, Easer easer = null)
	{
		// Set start point
		_start = start;
		
		// Set control point
		_control = control;

		// Set end point
		_end = end;

		// Set duration
		_duration = duration;

		// Set delay time
		_delay = delay;

		// Set easer
		_easer = easer ?? Ease.Linear;

		// Set current value
		_value = start;
		
		// Set time
		_time = 0;
		
		// Not finished
		_isFinished = false;
	}
	
	public Vector2 Get()
	{
		return _value;
	}

	public void Stop(bool isEnd = false)
	{
		if (!_isFinished)
		{
			if (isEnd)
			{
				_value = _end;
			}

			_isFinished = true;
		}
	}

	public bool IsFinished()
	{
		return _isFinished;
	}
	
	public Vector2 Update(float deltaTime)
	{
		if (_isFinished)
		{
			return _value;
		}

		if (_delay > 0)
		{
			_delay -= deltaTime;
			
			if (_delay < 0)
			{
				deltaTime = -_delay;
			}
			else
			{
				return _value;
			}
		}

		// Increase time
		_time += deltaTime;
		
		if (_time < _duration)
		{
			float t = _easer(_time / _duration);

			float c = t * t;
			float a = 1 - 2 * t + c;
			float b = 2 * (t - c);

			_value.x = a * _start.x + b * _control.x + c * _end.x;
			_value.y = a * _start.y + b * _control.y + c * _end.y;
		}
		else
		{
			// Set value to end
			_value = _end;
			
			// Set finished
			_isFinished = true;
		}
		
		return _value;
	}

	public void Draw(Color color)
	{
		Gizmos.color = color;
		
		Vector3 from = _start;
		Vector3 to   = Vector3.zero;
		
		float step = 0.01f;
		float t = step;
		float a, b, c;
		
		for (; t < 1.0f; t += step)
		{
			c = t * t;
			a = 1 - 2 * t + c;
			b = 2 * (t - c);
			
			to.x = a * _start.x + b * _control.x + c * _end.x;
			to.y = a * _start.y + b * _control.y + c * _end.y;
			
			Gizmos.DrawLine(from, to);
			
			from = to;
		}
		
		Gizmos.DrawLine(from, _end);
	}
}

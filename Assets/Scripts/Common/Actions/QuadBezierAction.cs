using UnityEngine;

public class QuadBezierAction : BaseAction
{
	// The start position
	private Vector3 _start;
	
	// The control position
	private Vector3 _control;
	
	// The end position
	private Vector3 _end;

	// Relative or absolute
	private bool _isRelative;

	// Local or world
	private bool _isLocal;

	// The duration
	private float _duration;

	// The accumulative time
	private float _time;

	// The transform
	private Transform _transform;

	public QuadBezierAction(Vector3 control, Vector3 end, bool isRelative, float duration, bool isLocal)
	{
		_control = control;
		_end     = end;

		_isRelative = isRelative;
		_isLocal    = isLocal;
		_duration   = duration;
	}

	public static QuadBezierAction BezierTo(Vector3 control, Vector3 end, float duration, bool isLocal = false)
	{
		return new QuadBezierAction(control, end, false, duration, isLocal);
	}
	
	public static QuadBezierAction BezierBy(Vector3 control, Vector3 end, float duration, bool isLocal = false)
	{
		return new QuadBezierAction(control, end, true, duration, isLocal);
	}

	public override void Play(GameObject target)
	{
		// Get transform
		_transform = target.transform;

		// Set start
		_start = _isLocal ? _transform.localPosition : _transform.position;

		if (_isRelative)
		{
			_control += _start;
			_end     += _start;
		}
	}
	
	public override void Reset()
	{
		if (_isLocal)
		{
			_transform.localPosition = _start;
		}
		else
		{
			_transform.position = _start;
		}
	}
	
	public override void Stop(bool forceEnd = false)
	{
		if (_time < _duration)
		{
			_time = _duration;

			if (forceEnd)
			{
				if (_isLocal)
				{
					_transform.localPosition = _end;
				}
				else
				{
					_transform.position = _end;
				}
			}
		}
	}
	
	public override bool IsFinished()
	{
		return _time >= _duration;
	}
	
	public override bool Update(float deltaTime)
	{
		if (_time < _duration)
		{
			_time += deltaTime;

			float t = (_time < _duration) ? _time / _duration : 1.0f;

			float C = t * t;
			float A = 1 - 2 * t + C;
			float B = 2 * (t - C);
			
			float x = A * _start.x + B * _control.x + C * _end.x;
			float y = A * _start.y + B * _control.y + C * _end.y;
			
			if (_isLocal)
			{
				_transform.localPosition = new Vector3(x, y, 0);
			}
			else
			{
				_transform.position = new Vector3(x, y, 0);
			}
		}

		return _time >= _duration;
	}
}

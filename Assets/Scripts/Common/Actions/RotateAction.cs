using UnityEngine;

public class RotateAction : BaseAction
{
	// The helper
	private LerpFloatHelper _helper = new LerpFloatHelper();

	// Relative or absolute
	private bool _isRelative;

	// The transform
	private Transform _transform;

	public RotateAction(float end, bool isRelative, float duration, Easer easer, LerpDirection direction)
	{
		_helper.Construct(end, duration, easer, direction);

		_isRelative = isRelative;
	}

	public static RotateAction Create(float end, bool isRelative, float duration, Easer easer = null, LerpDirection direction = LerpDirection.Forward)
	{
		return new RotateAction(end, isRelative, duration, easer, direction);
	}

	public static RotateAction RotateTo(float end, float duration, Easer easer = null, LerpDirection direction = LerpDirection.Forward)
	{
		return Create(end, false, duration, easer, direction);
	}
	
	public static RotateAction RotateBy(float end, float duration, Easer easer = null, LerpDirection direction = LerpDirection.Forward)
	{
		return Create(end, true, duration, easer, direction);
	}

	public override void Play(GameObject target)
	{
		// Get transform
		_transform = target.transform;

		// Set start
		_helper.Start = _transform.localRotation.eulerAngles.z;

		if (_isRelative)
		{
			_helper.AddEndByStart();
		}

		_helper.Play();
	}
	
	public override void Reset()
	{
		_transform.localRotation = Quaternion.Euler(0, 0, _helper.Start);
	}
	
	public override void Stop(bool forceEnd = false)
	{
		if (!_helper.IsFinished())
		{
			_helper.Stop();

			if (forceEnd)
			{
				_transform.localRotation = Quaternion.Euler(0, 0, _helper.End);
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
			_transform.localRotation = Quaternion.Euler(0, 0, _helper.Update(deltaTime));
		}

		return _helper.IsFinished();
	}
}

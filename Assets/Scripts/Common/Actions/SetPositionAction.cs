using UnityEngine;

public class SetPositionAction : BaseAction
{
	// The position
	private Vector3 _position;

	// The variance
	private Vector3 _variance;

	// Is relative
	private bool _isRelative;

	// Local or world
	private bool _isLocal;

	// The start position
	private Vector3? _startPosition;

	public SetPositionAction(Vector3 position, Vector3 variance, bool isRelative, bool isLocal)
	{
		// Set position
		_position = position;

		// Set variance
		_variance = variance;

		// Set relative
		_isRelative = isRelative;

		// Set local
		_isLocal = isLocal;
	}
	
	public static SetPositionAction Create(Vector3 position, Vector3? variance = null, bool isRelative = false, bool isLocal = false)
	{
		return new SetPositionAction(position, variance ?? Vector3.zero, isRelative, isLocal);
	}

	public override void Play(GameObject target)
	{
		if (_isRelative)
		{
			if (_isLocal)
			{
				if (!_startPosition.HasValue)
				{
					_startPosition = target.transform.localPosition;
				}

				target.transform.localPosition = _startPosition.Value + _position.Variance(_variance);
			}
			else
			{
				if (!_startPosition.HasValue)
				{
					_startPosition = target.transform.position;
				}
				
				target.transform.position = _startPosition.Value + _position.Variance(_variance);
			}
		}
		else
		{
			if (_isLocal)
			{
				target.transform.localPosition = _position.Variance(_variance);
			}
			else
			{
				target.transform.position = _position.Variance(_variance);
			}
		}
	}
}

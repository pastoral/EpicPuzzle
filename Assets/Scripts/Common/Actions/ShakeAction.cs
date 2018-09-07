using UnityEngine;

public class ShakeAction : BaseAction
{
	private float _minX;
	private float _maxX;
	private float _varX;
	private float _speedX;

	private float _minY;
	private float _maxY;
	private float _varY;
	private float _speedY;

	// Local or world
	private bool _isLocal;

	// The transform
	private Transform _transform;

	// The start position
	private Vector3 _startPosition;

	// The current position
	private Vector3 _position;

	// The start x
	protected float _startX;
	
	// The next x
	protected float _nextX;

	// The start y
	protected float _startY;
	
	// The next y
	protected float _nextY;

	// True if action finished
	private bool _isFinished;

	public ShakeAction(float minX, float maxX, float varX, float speedX, float minY, float maxY, float varY, float speedY, bool isLocal)
	{
		_minX = minX;
		_maxX = maxX;
		_varX = varX;
		_speedX = speedX;

		_minY = minY;
		_maxY = maxY;
		_varY = varY;
		_speedY = speedY;

		// Set local
		_isLocal = isLocal;
	}

	public static ShakeAction Create(float minX, float maxX, float varX, float speedX, float minY, float maxY, float varY, float speedY, bool isLocal = false)
	{
		return new ShakeAction(minX, maxX, varX, speedX, minY, maxY, varY, speedY, isLocal);
	}

	public override void Play(GameObject target)
	{
		// Get transform
		_transform = target.transform;

		// Set start position
		_startPosition = _isLocal ? _transform.localPosition : _transform.position;

		// Set current position
		_position = _startPosition;

		// Set start x
		_startX = _position.x;
		
		// Set next x
		_nextX = _startX + _minX + _varX.GetRandom();

		// Set start y
		_startY = _position.y;
		
		// Set next y
		_nextY = _startY + _minY + _varY.GetRandom();

		// Not finished
		_isFinished = false;
	}

	public override void Reset()
	{
		if (_isLocal)
		{
			_transform.localPosition = _startPosition;
		}
		else
		{
			_transform.position = _startPosition;
		}
	}

	public override void Stop(bool forceEnd = false)
	{
		_isFinished = true;
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
		
		// Move left
		if (_nextX < _startX)
		{
			_position.x -= _speedX * deltaTime;
			
			if (_position.x <= _nextX)
			{
				_position.x = _nextX;
				_nextX = _startX + _maxX + _varX.GetRandom();
			}
		}
		// Move right
		else
		{
			_position.x += _speedX * deltaTime;
			
			if (_position.x >= _nextX)
			{
				_position.x = _nextX;
				_nextX = _startX + _minX + _varX.GetRandom();
			}
		}

		// Move up
		if (_nextY > _startY)
		{
			_position.y += _speedY * deltaTime;
			
			if (_position.y >= _nextY)
			{
				_position.y = _nextY;
				_nextY = _startY + _minY + _varY.GetRandom();
			}
		}
		// Move down
		else
		{
			_position.y -= _speedY * deltaTime;
			
			if (_position.y <= _nextY)
			{
				_position.y = _nextY;
				_nextY = _startY + _maxY + _varY.GetRandom();
			}
		}

		// Set position
		if (_isLocal)
		{
			_transform.localPosition = _position;
		}
		else
		{
			_transform.position = _position;
		}
		
		return false;
	}
}

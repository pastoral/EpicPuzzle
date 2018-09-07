using UnityEngine;

public class LerpVector3Helper : LerpHelper<Vector3>
{
	public LerpVector3Helper()
	{
		
	}
	
	public LerpVector3Helper(Vector3 value)
	{
		_value = value;
	}

	protected override Vector3 Add(Vector3 a, Vector3 b)
	{
		return a + b;
	}

	protected override Vector3 Subtract(Vector3 a, Vector3 b)
	{
		return a - b;
	}

	protected override void Lerp(float t)
	{
		_value.x = _start.x + _delta.x * t;
		_value.y = _start.y + _delta.y * t;
		_value.z = _start.z + _delta.z * t;
	}
}

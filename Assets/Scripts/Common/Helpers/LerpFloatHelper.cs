using UnityEngine;

public class LerpFloatHelper : LerpHelper<float>
{
	public LerpFloatHelper()
	{

	}

	public LerpFloatHelper(float value)
	{
		_value = value;
	}

	protected override float Add(float a, float b)
	{
		return a + b;
	}

	protected override float Subtract(float a, float b)
	{
		return a - b;
	}

	protected override void Lerp(float t)
	{
		_value = _start + _delta * t;
	}
}

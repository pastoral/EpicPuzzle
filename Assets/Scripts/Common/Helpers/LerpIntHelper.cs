using UnityEngine;

public class LerpIntHelper : LerpHelper<int>
{
	public LerpIntHelper()
	{
		
	}
	
	public LerpIntHelper(int value)
	{
		_value = value;
	}

	protected override int Add(int a, int b)
	{
		return a + b;
	}

	protected override int Subtract(int a, int b)
	{
		return a - b;
	}

	protected override void Lerp(float t)
	{
		_value = (int)(_start + _delta * t);
	}
}

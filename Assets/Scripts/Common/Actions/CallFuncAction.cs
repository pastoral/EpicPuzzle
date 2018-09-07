using UnityEngine;
using System;

public class CallFuncAction : BaseAction
{
	// The callback
	private Action _callback;
	
	public CallFuncAction(Action callback)
	{
		// Set callback
		_callback = callback;
	}
	
	public static CallFuncAction Create(Action callback)
	{
		return new CallFuncAction(callback);
	}

	public override void Play(GameObject target)
	{
		if (_callback != null)
		{
			_callback();
		}
	}
}

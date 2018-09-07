#if !DEBUG
#define DEBUG
#endif

using System;
using System.Diagnostics;

public static class Log
{
	[Conditional("DEBUG")]
	public static void Debug(object message)
	{
		UnityEngine.Debug.Log(message);
	}

	[Conditional("DEBUG")]
	public static void Debug(string message, params object[] args)
	{
		UnityEngine.Debug.Log(string.Format(message, args));
	}
	
	[Conditional("DEBUG")]
	public static void Warning(object message)
	{
		UnityEngine.Debug.LogWarning(message);
	}
	
	[Conditional("DEBUG")]
	public static void Warning(string message, params object[] args)
	{
		UnityEngine.Debug.LogWarning(string.Format(message, args));
	}

	[Conditional("DEBUG")]
	public static void Error(object message)
	{
		UnityEngine.Debug.LogError(message);
	}
	
	[Conditional("DEBUG")]
	public static void Error(string message, params object[] args)
	{
		UnityEngine.Debug.LogError(string.Format(message, args));
	}
	
	[Conditional("DEBUG")]
	public static void Exception(Exception exception)
	{
		UnityEngine.Debug.LogException(exception);
	}
}

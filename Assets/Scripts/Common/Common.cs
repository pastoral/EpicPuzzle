#if UNITY_EDITOR
	#define USE_LOG
	#define ASSERT
#endif
using UnityEngine;
using System.Collections;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public delegate void VoidDelegate();

public class Common
{
	//-----------------------------------
	//--------------------- Log , warning, 
	
	[Conditional("USE_LOG")]
	public static void Log(object message)
	{
		//Debug.Log(message);
	}
	
	[Conditional("USE_LOG")]
	public static void Log(string format, params object[] args)
	{
		Debug.Log(string.Format(format, args));
	}
	
	[Conditional("USE_LOG")]
	public static void LogWarning(object message, Object context)
	{
		Debug.LogWarning(message, context);
	}
	
	[Conditional("USE_LOG")]
	public static void LogWarning(Object context, string format, params object[] args)
	{
		Debug.LogWarning(string.Format(format, args), context);
	}
	
	
	
	[Conditional("USE_LOG")]
	public static void Warning(bool condition, object message)
	{
		if ( ! condition) Debug.LogWarning(message);
	}
	
	[Conditional("USE_LOG")]
	public static void Warning(bool condition, object message, Object context)
	{
		if ( ! condition) Debug.LogWarning(message, context);
	}
	
	
	
	//---------------------------------------------
	//------------- Assert ------------------------
	
	/// Thown an exception if condition = false
	[Conditional("ASSERT")]
	public static void Assert(bool condition)
	{
		if (! condition) throw new UnityException();
	}
	
	/// Thown an exception if condition = false, show message on console's log
	[Conditional("ASSERT")]
	public static void Assert(bool condition, string message)
	{
		if (! condition) throw new UnityException(message);
	}
	
	/// Thown an exception if condition = false, show message on console's log
	[Conditional("ASSERT")]
	public static void Assert(bool condition, string format, params object[] args)
	{
		if (! condition) throw new UnityException(string.Format(format, args));
	}
	
	/// Throw exception and also hightlight object in Hierarchy windows
	[Conditional("ASSERT")]
	public static void Assert(bool condition, Object context = null)
	{
		if (! condition)
		{
			// use LogWarning instead of LogError : dont show too many "red" color debug console text
			if (context != null) Debug.LogWarning("Error from this object !!!", context);
			throw new UnityException();
		}
	}

	/// Throw exception and also hightlight object in Hierarchy windows
	[Conditional("ASSERT")]
	public static void Assert(bool condition, string message, Object context)
	{
		if (! condition)
		{
			// use LogWarning instead of LogError : dont show too many "red" color debug console text
			if (context != null) Debug.LogWarning("Error from this object !!! : " + message, context);
			throw new UnityException(message);
		}
	}
}
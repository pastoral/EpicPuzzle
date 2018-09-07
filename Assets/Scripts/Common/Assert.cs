using System;
using System.Diagnostics;

public static class Assert
{
	[Conditional("DEBUG")]
	public static void Fail(string message = "")
	{
		throw new Exception(message);
	}
	
	[Conditional("DEBUG")]
	public static void IsTrue(bool condition, string message = "")
	{
		if (!condition)
			throw new Exception(message);
	}
	
	[Conditional("DEBUG")]
	public static void IsFalse(bool condition, string message = "")
	{
		if (condition)
			throw new Exception(message);
	}
	
	[Conditional("DEBUG")]
	public static void IsNull(object obj, string message = "")
	{
		if (obj != null)
			throw new Exception(message);
	}
	
	[Conditional("DEBUG")]
	public static void IsNotNull(object obj, string message = "")
	{
		if (obj == null)
			throw new Exception(message);
	}
	
	[Conditional("DEBUG")]
	public static void IsZero(int n, string message = "")
	{
		if (n != 0)
			throw new Exception(message);
	}
	
	[Conditional("DEBUG")]
	public static void IsNotZero(int n, string message = "")
	{
		if (n == 0)
			throw new Exception(message);
	}
	
	[Conditional("DEBUG")]
	public static void IsPositive(int n, string message = "")
	{
		if (n <= 0)
			throw new Exception(message);
	}
	
	[Conditional("DEBUG")]
	public static void IsNotPositive(int n, string message = "")
	{
		if (n > 0)
			throw new Exception(message);
	}

	[Conditional("DEBUG")]
	public static void IsNegative(int n, string message = "")
	{
		if (n >= 0)
			throw new Exception(message);
	}
	
	[Conditional("DEBUG")]
	public static void IsNotNegative(int n, string message = "")
	{
		if (n < 0)
			throw new Exception(message);
	}

	[Conditional("DEBUG")]
	public static void IsInRange(int n, int start, int end, string message = "")
	{
		if (n < start || n > end)
			throw new Exception(message);
	}
	
	[Conditional("DEBUG")]
	public static void IsNotInRange(int n, int start, int end, string message = "")
	{
		if (n >= start && n <= end)
			throw new Exception(message);
	}
	
	[Conditional("DEBUG")]
	public static void Todo(string message = "")
	{
		throw new Exception("TODO: " + message);
	}
	
	[Conditional("DEBUG")]
	public static void NotSupported(string message = "")
	{
		throw new Exception("NOT SUPPORTED: " + message);
	}
}

using UnityEngine;
using System.Collections;

public enum FootholdType
{
	None = -1,
	Normal,
	Double,
	Time,
	RedirectLeft,
	RedirectUp,
	RedirectRight,
	RedirectDown
}

public static class FootholdTypeHelper
{
	public static bool IsNone(this FootholdType type)
	{
		return type == FootholdType.None;
	}

	public static bool IsNormal(this FootholdType type)
	{
		return type == FootholdType.Normal;
	}

	public static bool IsDouble(this FootholdType type)
	{
		return type == FootholdType.Double;
	}
	
	public static bool IsTime(this FootholdType type)
	{
		return type == FootholdType.Time;
	}
	
	public static bool IsRedirect(this FootholdType type)
	{
		return 	type == FootholdType.RedirectLeft 	||
				type == FootholdType.RedirectUp 	||
				type == FootholdType.RedirectRight 	||
				type == FootholdType.RedirectDown;
	}

	public static int ToInt(this FootholdType type)
	{
		return (int)type;
	}

	public static string ToStringExt(this FootholdType type)
	{
		if (type == FootholdType.Normal)
		{
			return "O";
		}

		if (type == FootholdType.Double)
		{
			return "2";
		}

		if (type == FootholdType.Time)
		{
			return "T";
		}
		
		if (type == FootholdType.RedirectLeft)
		{
			return "L";
		}
		
		if (type == FootholdType.RedirectUp)
		{
			return "U";
		}
		
		if (type == FootholdType.RedirectRight)
		{
			return "R";
		}
		
		if (type == FootholdType.RedirectDown)
		{
			return "D";
		}

		return "X";
	}
	
	public static ItemType ToItemType(this FootholdType type)
	{
		return (ItemType)type;
	}

	public static FootholdType ToFootholdType(this int type)
	{
		return (FootholdType)type;
	}

	public static Direction GetDirection(this FootholdType type)
	{
		if (type == FootholdType.RedirectLeft)
		{
			return Direction.Left;
		}
		
		if (type == FootholdType.RedirectUp)
		{
			return Direction.Up;
		}
		
		if (type == FootholdType.RedirectRight)
		{
			return Direction.Right;
		}
		
		if (type == FootholdType.RedirectDown)
		{
			return Direction.Down;
		}
		
		return Direction.None;
	}
}

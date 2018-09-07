using UnityEngine;
using System.Collections;

public enum Direction
{
	None = -1,
	Left,
	Up,
	Right,
	Down
}

public static class DirectionHelper
{
	public static bool IsNone(this Direction dir)
	{
		return dir == Direction.None;
	}

	public static bool IsLeft(this Direction dir)
	{
		return dir == Direction.Left;
	}
	
	public static bool IsRight(this Direction dir)
	{
		return dir == Direction.Right;
	}
	
	public static bool IsUp(this Direction dir)
	{
		return dir == Direction.Up;
	}
	
	public static bool IsDown(this Direction dir)
	{
		return dir == Direction.Down;
	}
	
	public static bool IsHorizontal(this Direction dir)
	{
		return dir == Direction.Left || dir == Direction.Right;
	}
	
	public static bool IsVertical(this Direction dir)
	{
		return dir == Direction.Up || dir == Direction.Down;
	}
	
	public static bool IsOpposite(this Direction dir, Direction other)
	{
		if (dir == Direction.Left)
		{
			return other == Direction.Right;
		}
		
		if (dir == Direction.Right)
		{
			return other == Direction.Left;
		}
		
		if (dir == Direction.Up)
		{
			return other == Direction.Down;
		}
		
		if (dir == Direction.Down)
		{
			return other == Direction.Up;
		}
		
		return false;
	}
	
	public static bool IsSameRow(this Vector3 pos1, Vector3 pos2)
	{
		return (pos1.y == pos2.y);
	}
	
	public static bool IsSameColumn(this Vector3 pos1, Vector3 pos2)
	{
		return (pos1.x == pos2.x);
	}
	public static Direction Reverse(this Direction dir)
	{
		if (dir == Direction.Left)
		{
			return Direction.Right;
		}
		
		if (dir == Direction.Right)
		{
			return Direction.Left;
		}
		
		if (dir == Direction.Up)
		{
			return Direction.Down;
		}
		
		if (dir == Direction.Down)
		{
			return Direction.Up;
		}
		
		return dir;
	}
	
	public static Direction RotateLeft(this Direction dir)
	{
		if (dir == Direction.Left)
		{
			return Direction.Down;
		}
		
		if (dir == Direction.Down)
		{
			return Direction.Right;
		}
		
		if (dir == Direction.Right)
		{
			return Direction.Up;
		}
		
		if (dir == Direction.Up)
		{
			return Direction.Left;
		}
		
		return dir;
	}
	
	public static Direction RotateRight(this Direction dir)
	{
		if (dir == Direction.Left)
		{
			return Direction.Up;
		}
		
		if (dir == Direction.Up)
		{
			return Direction.Right;
		}
		
		if (dir == Direction.Right)
		{
			return Direction.Down;
		}
		
		if (dir == Direction.Down)
		{
			return Direction.Left;
		}
		
		return dir;
	}
	
	public static int ToInt(this Direction dir)
	{
		return (int)dir;
	}
	
	public static Direction GetRandom()
	{
		switch (Random.Range(0, 4))
		{
		case 0: return Direction.Left;
		case 1: return Direction.Up;
		case 2: return Direction.Right;
		case 3: return Direction.Down;
		}
		
		return Direction.Left;
	}
	
	public static Direction GetDirection(Vector3 point)
	{
		// Horizontal
		if (Mathf.Abs(point.x) >= Mathf.Abs(point.y))
		{
			return point.x < 0 ? Direction.Left : (point.x > 0 ? Direction.Right : Direction.None);
		}
		
		// Vertical
		return point.y < 0 ? Direction.Down : Direction.Up;
	}
	
	public static Direction GetDirection(Vector3 start, Vector3 end)
	{
		return GetDirection(end - start);
	}
}

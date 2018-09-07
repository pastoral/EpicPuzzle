using UnityEngine;
using System.Collections;

public enum ItemType
{
	None = -1,
	
	Foothold,
	FootholdDouble,
	FootholdTime,
	FootholdRedirectLeft,
	FootholdRedirectUp,
	FootholdRedirectRight,
	FootholdRedirectDown,
	
	FrogLeft,
	FrogUp,
	FrogRight,
	FrogDown,

	Count
}

public static class ItemTypeHelper
{
	public static bool IsFrog(this ItemType itemType)
	{
		return 	itemType == ItemType.FrogLeft 	||
				itemType == ItemType.FrogUp 	||
				itemType == ItemType.FrogRight 	||
				itemType == ItemType.FrogDown;
	}

	public static Direction GetDirection(this ItemType itemType)
	{
		if (itemType == ItemType.FrogLeft)
		{
			return Direction.Left;
		}

		if (itemType == ItemType.FrogUp)
		{
			return Direction.Up;
		}
		
		if (itemType == ItemType.FrogRight)
		{
			return Direction.Right;
		}
		
		if (itemType == ItemType.FrogDown)
		{
			return Direction.Down;
		}

		return Direction.None;
	}

	public static int ToInt(this ItemType itemType)
	{
		return (int)itemType;
	}

	public static ItemType GetFrog(Direction direction)
	{
		if (direction == Direction.Left)
		{
			return ItemType.FrogLeft;
		}
		
		if (direction == Direction.Up)
		{
			return ItemType.FrogUp;
		}
		
		if (direction == Direction.Right)
		{
			return ItemType.FrogRight;
		}
		
		if (direction == Direction.Down)
		{
			return ItemType.FrogDown;
		}
		
		return ItemType.FrogUp;
	}
}

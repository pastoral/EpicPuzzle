///////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2015 AsNet Co., Ltd.
// All Rights Reserved. These instructions, statements, computer
// programs, and/or related material (collectively, the "Source")
// contain unpublished information proprietary to AsNet Co., Ltd
// which is protected by US federal copyright law and by
// international treaties. This Source may NOT be disclosed to
// third parties, or be copied or duplicated, in whole or in
// part, without the written consent of AsNet Co., Ltd.
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;

public enum Pivot
{
	Center,
	TopLeft,
	Top,
	TopRight,
	Left,
	Right,
	BottomLeft,
	Bottom,
	BottomRight
}

public static class PivotHelper
{
	/// <summary>
	/// Gets the bottom-left position.
	/// </summary>
	public static Vector2 GetPosition(this Pivot pivot, float width, float height)
	{
		// Center
		if (pivot == Pivot.Center)
		{
			return new Vector2(-width * 0.5f, -height * 0.5f);
		}

		// Top-Left
		if (pivot == Pivot.TopLeft)
		{
			return new Vector2(0, -height);
		}
		
		// Top
		if (pivot == Pivot.Top)
		{
			return new Vector2(-width * 0.5f, -height);
		}
		
		// Top-Right
		if (pivot == Pivot.TopRight)
		{
			return new Vector2(-width, -height);
		}
		
		// Left
		if (pivot == Pivot.Left)
		{
			return new Vector2(0, -height * 0.5f);
		}

		// Right
		if (pivot == Pivot.Right)
		{
			return new Vector2(-width, -height * 0.5f);
		}
		
		// Bottom-Left
		if (pivot == Pivot.BottomLeft)
		{
			return new Vector2(0, 0);
		}
		
		// Bottom
		if (pivot == Pivot.Bottom)
		{
			return new Vector2(-width * 0.5f, 0);
		}
		
		// Bottom-Right
		if (pivot == Pivot.BottomRight)
		{
			return new Vector2(-width, 0);
		}
		
		return new Vector2(-width * 0.5f, -height * 0.5f);
	}

	// Get bottom-left position
	public static Vector2 GetPosition(this Pivot pivot, float width, float height, float skewX)
	{
		// Center
		if (pivot == Pivot.Center)
		{
			return new Vector2(-width * 0.5f - skewX * 0.5f, -height * 0.5f);
		}
		
		// Top-Left
		if (pivot == Pivot.TopLeft)
		{
			return new Vector2(-skewX, -height);
		}
		
		// Top
		if (pivot == Pivot.Top)
		{
			return new Vector2(-width * 0.5f - skewX, -height);
		}
		
		// Top-Right
		if (pivot == Pivot.TopRight)
		{
			return new Vector2(-width - skewX, -height);
		}
		
		// Left
		if (pivot == Pivot.Left)
		{
			return new Vector2(-skewX * 0.5f, -height * 0.5f);
		}
		
		// Right
		if (pivot == Pivot.Right)
		{
			return new Vector2(-width - skewX * 0.5f, -height * 0.5f);
		}
		
		// Bottom-Left
		if (pivot == Pivot.BottomLeft)
		{
			return new Vector2(0, 0);
		}
		
		// Bottom
		if (pivot == Pivot.Bottom)
		{
			return new Vector2(-width * 0.5f, 0);
		}
		
		// Bottom-Right
		if (pivot == Pivot.BottomRight)
		{
			return new Vector2(-width, 0);
		}
		
		return new Vector2(-width * 0.5f, -height * 0.5f);
	}
}

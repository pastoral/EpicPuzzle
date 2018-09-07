using UnityEngine;

public enum BoosterType
{
	None = -1,
	Undo,
	Hint,
	Hammer,
	Deploy
}

public static class BoosterTypeHelper
{
	public static readonly BoosterType[] boosterTypes =
	{
		BoosterType.Undo,
		BoosterType.Hint,
		BoosterType.Hammer,
		BoosterType.Deploy
	};

	public static readonly int boosterCount = boosterTypes.Length;

	public static bool IsUndo(this BoosterType type)
	{
		return type == BoosterType.Undo;
	}
	
	public static bool IsHint(this BoosterType type)
	{
		return type == BoosterType.Hint;
	}
	
	public static bool IsHammer(this BoosterType type)
	{
		return type == BoosterType.Hammer;
	}
	
	public static bool IsDeploy(this BoosterType type)
	{
		return type == BoosterType.Deploy;
	}

	public static bool IsInstant(this BoosterType type)
	{
		return type == BoosterType.Undo || type == BoosterType.Hint;
	}

	public static int ToInt(this BoosterType type)
	{
		return (int)type;
	}
	
	public static float GetDummyDeltaY(this BoosterType type)
	{
		// Deploy
		if (type == BoosterType.Deploy)
		{
			return 0.75f;
		}

		return 0.5f;
	}
	
	public static void GetDummyAABB(this BoosterType type, float scale, out float left, out float top, out float right, out float bottom)
	{
		// Deploy
		if (type == BoosterType.Deploy)
		{
			float halfWidth  = 0.15f * scale;
			float halfHeight = halfWidth;

			left   = -halfWidth;
			right  = halfWidth;
			bottom = -halfHeight;
			top    = halfHeight;
		}
		// Hammer
		else if (type == BoosterType.Hammer)
		{
			left   = -0.7f * scale;
			right  = -0.3f * scale;
			bottom =  0.4f * scale;
			top    =  0.9f * scale;
		}
		else
		{
			left = top = right = bottom = 0;
		}
	}

	public static int GetUnlockLevel(this BoosterType type)
	{
		return Settings.unlockBoosterLevels[type.ToInt()];
	}

	public static string GetUnlockMessage(this BoosterType type)
	{
		return Settings.unlockBoosterMessages[type.ToInt()];
	}
	
	public static int GetUnlockQuantity(this BoosterType type)
	{
		return Settings.unlockBoosterQuantities[type.ToInt()];
	}
	
	public static string GetTitle(this BoosterType type)
	{
		return Settings.boosterTitles[type.ToInt()];
	}

	public static string GetDescription(this BoosterType type)
	{
		return Settings.boosterDescriptions[type.ToInt()];
	}
	
	public static int GetBuyCoin(this BoosterType type)
	{
		return Settings.buyBoosterCoins[type.ToInt()];
	}
	
	public static int GetBuyQuantity(this BoosterType type)
	{
		return Settings.buyBoosterQuantities[type.ToInt()];
	}
}

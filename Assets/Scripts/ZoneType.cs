using UnityEngine;

public enum ZoneType
{
	Forest,
	Desert,
	Volcano
}

public static class ZoneTypeHelper
{
	public static bool IsDefault(this ZoneType zoneType)
	{
		return zoneType == ZoneType.Forest;
	}

	public static int ToInt(this ZoneType zoneType)
	{
		return (int)zoneType;
	}

	public static ZoneType ToZoneType(this int zone)
	{
		return (ZoneType)zone;
	}
	
	public static int MapCount(this ZoneType zoneType)
	{
		if (zoneType == ZoneType.Forest)
		{
			return 33;
		}

		if (zoneType == ZoneType.Desert)
		{
			return 66;
		}

		return Settings.MapCount;
	}
	
//	public static SoundID GetBGM(this ZoneType zoneType)
//	{
//		if (zoneType == ZoneType.Forest)
//		{
//			return SoundID.MainGameForest;
//		}
//		
//		if (zoneType == ZoneType.Desert)
//		{
//			return SoundID.MainGameDesert;
//		}
//		
//		return SoundID.MainGameVolcano;
//	}

	public static ZoneType GetZoneType(int map)
	{
		if (map <= 33)
		{
			return ZoneType.Forest;
		}
		
		if (map <= 66)
		{
			return ZoneType.Desert;
		}
		
		return ZoneType.Volcano;
	}
}

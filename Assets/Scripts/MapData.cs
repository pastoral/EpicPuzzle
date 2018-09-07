using UnityEngine;
using System.Collections;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

[System.Serializable]
public class MapData
{
	// The footholds
	public int[,] footholds;

	// The start cell
	public int startRow, startColumn;

	// The direction
	public Direction direction;

	// The time foothold durations
	public string timeFootholdDurations;

	// Load from file
	public static MapData Load(int mapID)
	{
		return Load(string.Format("Maps/Map{0}", mapID));
	}

	// Load from file
	public static MapData Load(string path)
	{
//		MapData mapData = null;
//		Helper.Load<MapData>(fileName, ref mapData);
//
//		return mapData;

		TextAsset map = Resources.Load<TextAsset>(path);
		
		if (map != null)
		{
			BinaryFormatter bf = new BinaryFormatter();

			return (MapData)bf.Deserialize(new MemoryStream(map.bytes));
		}

		return null;
	}

	public void DeserializeTimeFootholds(Action<int, int, int> callback)
	{
		if (string.IsNullOrEmpty(timeFootholdDurations)) return;
		
		int columnCount = footholds.GetColumn();

		string[] idDurations = timeFootholdDurations.Split(' ');
		int count = idDurations.Length;
		
		for (int i = 0; i < count; i++)
		{
			string idDuration = idDurations[i];
			int index = idDuration.IndexOf(':');
			
			if (index > 0)
			{
				int id = idDuration.Substring(0, index).ToInt();
				int duration = idDuration.Substring(index + 1).ToInt();
				
				int row = (int)(id / columnCount);
				int column = id % columnCount;
				
				callback(row, column, duration);
			}
			else
			{
				Debug.Log("Format invalid: " + idDuration);
			}
		}
	}

	public void Log()
	{
		string[] lines = this.ToString().Split(new char[] {'\n'}, System.StringSplitOptions.RemoveEmptyEntries);

		for (int i = 0; i < lines.Length; i++)
		{
			Debug.Log(lines[i]);
		}
	}

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();

		int rows    = footholds.GetRow();
		int columns = footholds.GetColumn();

		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				sb.Append(string.Format(j == startColumn && i == startRow ? " [{0}] " : "  {0}  ", footholds[i, j].ToFootholdType().ToStringExt()));
			}

			sb.Append("\n");
		}

		sb.Append(string.Format("Direction: {0}\n", direction.ToString()));

		if (!string.IsNullOrEmpty(timeFootholdDurations))
		{
			sb.Append(string.Format("Durations: {0}\n", timeFootholdDurations));
		}

		return sb.ToString();
	}
}

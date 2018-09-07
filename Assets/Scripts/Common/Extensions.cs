using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using Random = UnityEngine.Random;

public static class Extensions
{
	#region Dictionary

	public static string ToStringExt(this Dictionary<string, string> dict)
	{
		StringBuilder sb = new StringBuilder("{");
		bool first = true;
		
		foreach (var key in dict.Keys)
		{
			var value = dict[key];
			
			if (first)
			{
				sb.Append(string.Format(" {0} = {1}", key, value));
				first = false;
			}
			else
			{
				sb.Append(string.Format(", {0} = {1}", key, value));
			}
		}
		
		sb.Append(" }");
		
		return sb.ToString();
	}

	public static string ToStringExt(this Dictionary<string, object> dict)
	{
		StringBuilder sb = new StringBuilder("{");
		bool first = true;
		
		foreach (var key in dict.Keys)
		{
			var value = dict[key];
			
			if (value.GetType() == typeof(Dictionary<string, object>))
			{
				if (first)
				{
					sb.Append(string.Format(" {0} = {1}", key, ((Dictionary<string, object>)value).ToStringExt()));
					first = false;
				}
				else
				{
					sb.Append(string.Format(", {0} = {1}", key, ((Dictionary<string, object>)value).ToStringExt()));
				}
			}
			else
			{
				if (first)
				{
					sb.Append(string.Format(" {0} = {1}", key, value));
					first = false;
				}
				else
				{
					sb.Append(string.Format(", {0} = {1}", key, value));
				}
			}
		}
		
		sb.Append(" }");
		
		return sb.ToString();
	}
	
	#endregion

	#region String

	public static int ToInt(this string s)
	{
		int value = 0;
		int.TryParse(s, out value);
		
		return value;
	}
	
	public static bool ToBool(this string s)
	{
		bool value = false;
		bool.TryParse(s, out value);
		
		return value;
	}

	public static float ToFloat(this string s)
	{
		float value = 0f;
		float.TryParse(s, out value);
		
		return value;
	}

	#endregion
	
	#region Color
	
	// Get RGB component
	public static Vector3 RGB(this Color color)
	{
		return new Vector3(color.r, color.g, color.b);
	}

	public static Color Reverse(this Color color)
	{
		return new Color(1.0f - color.r, 1.0f - color.g, 1.0f - color.b, color.a);
	}
	
	public static Color Copy(this Color color, float a)
	{
		return new Color(color.r, color.g, color.b, a);
	}

	#endregion

	public static string ToResult(this bool success)
	{
		return success ? "Success" : "Failure";
	}

	public static float GetRandom(this float variance)
	{
		return Random.Range(-variance, variance);
	}
	
	public static float Variance(this float f, float variance)
	{
		return variance != 0 ? f + Random.Range(-variance, variance) : f;
	}

	public static Vector3 Variance(this Vector3 v3, Vector3 variance)
	{
		return new Vector3(variance.x != 0 ? v3.x + Random.Range(-variance.x, variance.x) : v3.x,
		                   variance.y != 0 ? v3.y + Random.Range(-variance.y, variance.y) : v3.y,
		                   variance.z != 0 ? v3.z + Random.Range(-variance.z, variance.z) : v3.z);
	}

	public static string ToTimeString(this float time)
	{
		int minutes = Mathf.FloorToInt(time / 60.0f);
		int seconds = Mathf.FloorToInt(time - minutes * 60);

		return string.Format("{0:00}:{1:00}", minutes, seconds);
	}

	public static string ToUniqueNames(this List<string> names)
	{
		int count = names.Count;

		if (count == 1)
		{
			return names[0];
		}

		if (count == 2)
		{
			return names[1] != names[0] ? string.Format("{0} and {1}", names[0], names[1]) : names[0];
		}

		List<string> uniqueNames = new List<string>(count);
		uniqueNames.Add(names[0]);

		for (int i = 1; i < count; i++)
		{
			string name = names[i];
			bool existed = false;

			for (int j = 0; j < uniqueNames.Count; j++)
			{
				if (uniqueNames[j] == name)
				{
					existed = true;
					break;
				}
			}

			if (!existed) uniqueNames.Add(name);
		}

		count = uniqueNames.Count;

		if (count == 1)
		{
			return uniqueNames[0];
		}

		if (count == 2)
		{
			return uniqueNames[1] != uniqueNames[0] ? string.Format("{0} and {1}", uniqueNames[0], uniqueNames[1]) : uniqueNames[0];
		}

		return string.Format("{0} and {1} others", uniqueNames[0], count - 1);
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class Helper
{
	public static float GetDegreeAngle(float deltaX, float deltaY)
	{
		float angle = Mathf.Atan2(deltaY, deltaX) * Mathf.Rad2Deg;
		
		return angle >= 0 ? angle : angle + 360f;
	}

	// Get path of the specified file name
	public static string GetFilePath(string fileName)
	{
#if UNITY_EDITOR
		return string.Format("{0}/Resources/{1}", Application.dataPath, fileName);
#elif UNITY_STANDALONE
		return string.Format("{0}/{1}", Application.dataPath, fileName);
#else
		return string.Format("{0}/{1}", Application.persistentDataPath, fileName);
#endif
	}

	// Save data to the specified file
	public static bool Save<T>(T data, string fileName)
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(GetFilePath(fileName));
		bf.Serialize(file, data);
		file.Close();

		return true;
	}

	// Load data from the specified file
	public static bool Load<T>(string fileName, ref T data)
	{
		string path = GetFilePath(fileName);

		if (File.Exists(path))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(path, FileMode.Open);
			data = (T)bf.Deserialize(file);
			file.Close();

			return true;
		}

		return false;
	}

	// Check if online
	public static bool IsOnline()
	{
		return Application.internetReachability != NetworkReachability.NotReachable;
	}
	
	public static float GetDuration(Vector3 start, Vector3 end, float speed)
	{
		float deltaX = end.x - start.x;
		float deltaY = end.y - start.y;
		float length = Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY);

		return length / speed;
	}

	public static float GetQuadBezierLength(Vector2 start, Vector2 control, Vector2 end, int stepCount = 10)
	{
		Vector2 prev = start;
		Vector2 current;
		float length = 0f;
		float t;
		float A, B, C;
		
		for (int i = 1; i < stepCount; i++)
		{
			// Calculate uniformed time
			t = (float)i / stepCount;
			
			//
			C = t * t;
			A = 1 - 2 * t + C;
			B = 2 * (t - C);
			
			// Calculate current position
			current.x = A * start.x + B * control.x + C * end.x;
			current.y = A * start.y + B * control.y + C * end.y;
			
			// Increase length
			length += (current - prev).magnitude;
			
			// Save current position
			prev = current;
		}
		
		//
		length += (end - prev).magnitude;
		
		return length;
	}

	#region Button

	public static void SetText(this Button button, string text)
	{
		Transform textTransform = button.transform.FindInChildren("Text");

		if (textTransform != null)
		{
			textTransform.GetComponent<Text>().text = text;
		}
	}

	#endregion
}

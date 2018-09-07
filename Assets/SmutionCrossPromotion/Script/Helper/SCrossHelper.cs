using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SCrossHelper
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

	#if UNITY_EDITOR
	public static UnityEditor.EditorWindow GetMainGameView()
	{
	System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
	System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
	System.Object Res = GetMainGameView.Invoke(null, null);

	return (UnityEditor.EditorWindow)Res;
	}

	public static Vector2 GetMainGameViewSize()
	{
	System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
	System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
	System.Object Res = GetSizeOfMainGameView.Invoke(null, null);

	return (Vector2)Res;
	}

	public static Vector2 GetMainGameViewPosition()
	{
	Rect rect = GetMainGameView().position;
	Vector2 size = GetMainGameViewSize();

	return new Vector2((rect.size.x - size.x) * 0.5f, (rect.size.y - 12 - size.y) * 0.5f);
	}
	#endif

}

using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public static class CustomMenu
{
	[MenuItem("Project/Layout %l")]	// CTRL+L
	static void Layout()
	{
		// Get all selected objects
		GameObject[] objects = Selection.gameObjects;

		if (objects.Length == 0)
		{
			// Get all objects
			objects = GameObject.FindObjectsOfType<GameObject>();
		}

		foreach (GameObject go in objects)
		{
			if (go != null && go.activeInHierarchy)
			{
				go.SendMessage("OnLayout", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	[MenuItem("Project/Unlayout %#l")]	// CTRL+SHIFT+L
	static void Unlayout()
	{
		// Get all selected objects
		GameObject[] objects = Selection.gameObjects;
		
		if (objects.Length == 0)
		{
			// Get all objects
			objects = GameObject.FindObjectsOfType<GameObject>();
		}
		
		foreach (GameObject go in objects)
		{
			if (go != null && go.activeInHierarchy)
			{
				go.SendMessage("OnUnlayout", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	[MenuItem("Project/Reload")]
	static void Reload()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	[MenuItem("Speed/0.5x")]
	static void Speed05x()
	{
		Time.timeScale = 0.5f;
	}
	
	[MenuItem("Speed/1x")]
	static void Speed5x()
	{
		Time.timeScale = 1f;
	}
	
	[MenuItem("Speed/2x")]
	static void Speed2x()
	{
		Time.timeScale = 2f;
	}
	
	[MenuItem("Speed/4x")]
	static void Speed4x()
	{
		Time.timeScale = 4f;
	}
}

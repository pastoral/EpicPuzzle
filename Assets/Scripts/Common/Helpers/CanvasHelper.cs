using UnityEngine;

public static class CanvasHelper
{
	public static Transform FindInChildren(this Canvas canvas, string name)
	{
		return canvas.transform.FindInChildren(name);
	}
	
	public static T GetComponentInChildren<T>(this Canvas canvas, string name) where T : Component
	{
		return canvas.transform.GetComponentInChildren<T>(name);
	}

	public static void SetInteractable(this Canvas canvas, bool interactable)
	{
		IgnoreRaycast ignoreRaycast = canvas.GetComponent<IgnoreRaycast>();
		
		if (ignoreRaycast != null)
		{
			ignoreRaycast.interactable = interactable;
		}
	}
}

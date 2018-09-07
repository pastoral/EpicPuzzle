using UnityEngine;
using System.Collections;

public class IgnoreRaycast : MonoBehaviour, ICanvasRaycastFilter
{
	/// <summary>
	/// The interactable.
	/// </summary>
	public bool interactable = true;

    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
		return interactable;
    }
}

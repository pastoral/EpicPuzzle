using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SortingLayerHelper : MonoBehaviour {

	Renderer _renderer;
	public string layerID;
	public int orderID;

	void OnValidate()
	{
		// Find if renderer = null
		if (_renderer == null) FindRenderer();

		_renderer.sortingLayerName = layerID;
		_renderer.sortingOrder = orderID;
	}

	void FindRenderer()
	{
		_renderer = GetComponent<Renderer> ();
	}
}

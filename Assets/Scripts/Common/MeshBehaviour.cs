using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshBehaviour : MonoBehaviour
{
	/// <summary>
	/// The sprite.
	/// </summary>
	[SerializeField]
	protected Sprite _sprite;
	
	/// <summary>
	/// The color.
	/// </summary>
	[SerializeField]
	protected Color _color = Color.white;

	/// <summary>
	/// The sorting layer index.
	/// </summary>
	[SerializeField]
	protected int _sortingLayerIndex;

	/// <summary>
	/// The order in layer.
	/// </summary>
	[SerializeField]
	protected int _orderInLayer;

	public Sprite Sprite
	{
		get
		{
			return _sprite;
		}
		set
		{
			_sprite = value;

			// Get renderer
			Renderer renderer = GetComponent<Renderer>();
			
			if (renderer != null)
			{
				renderer.SetTexture(_sprite != null ? _sprite.texture : null);
			}
		}
	}

	public Color Color
	{
		get
		{
			return _color;
		}
		set
		{
			_color = value;

			// Get renderer
			Renderer renderer = GetComponent<Renderer>();
			
			if (renderer != null)
			{
				renderer.SetColor(_color);
			}
		}
	}

	public int SortingLayerIndex
	{
		get
		{
			return _sortingLayerIndex;
		}
		set
		{
			_sortingLayerIndex = value;

			// Get renderer
			Renderer renderer = GetComponent<Renderer>();
			
			if (renderer != null)
			{
				renderer.sortingLayerID = SortingLayer.layers[_sortingLayerIndex].id;
			}
		}
	}
	
	public int SortingOrder
	{
		get
		{
			return _orderInLayer;
		}
		set
		{
			_orderInLayer = value;
			
			// Get renderer
			Renderer renderer = GetComponent<Renderer>();
			
			if (renderer != null)
			{
				renderer.sortingOrder = _orderInLayer;
			}
		}
	}
	
	public void Validate()
	{
		OnValidate();
	}

	protected virtual void OnValidate()
	{
		// Get renderer
		Renderer renderer = GetComponent<Renderer>();

		if (renderer != null)
		{
			// Set texture
			renderer.SetTexture(_sprite != null ? _sprite.texture : null);

			// Set color
			renderer.SetColor(_color);
			
			// Set sorting layer
			renderer.sortingLayerID = SortingLayer.layers[_sortingLayerIndex].id;

			// Set order in layer
			renderer.sortingOrder = _orderInLayer;
		}
	}

	protected virtual Mesh GetMesh()
	{
		// Get mesh filter
		MeshFilter meshFilter = GetComponent<MeshFilter>();

		if (meshFilter != null)
		{
			// Get mesh
			Mesh mesh = meshFilter.sharedMesh;
			
			if (mesh == null)
			{
				Mesh newMesh = new Mesh();
				System.Type type = GetType();
				newMesh.name = type.Name;
				
				meshFilter.mesh = newMesh;
				mesh = meshFilter.sharedMesh;
			}

			return mesh;
		}

		return null;
	}

	void OnDrawGizmos()
	{
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		
		if (meshFilter != null)
		{
			Mesh mesh = meshFilter.sharedMesh;
			
			if (mesh != null)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawWireMesh(mesh, transform.position, transform.localRotation, transform.localScale);
			}
		}
	}
}

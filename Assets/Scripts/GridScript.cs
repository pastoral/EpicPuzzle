using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GridScript : MonoBehaviour
{
	/// <summary>
	/// The row.
	/// </summary>
	public int row = 7;

	/// <summary>
	/// The column.
	/// </summary>
	public int column = 5;

	/// <summary>
	/// The size of the cell.
	/// </summary>
	public float cellSize = 100.0f;

	/// <summary>
	/// The color.
	/// </summary>
	public Color color = Color.blue;

	void OnDrawGizmos()
	{
		// Set color
		Gizmos.color = color;
		
		// Bottom-left position
		float bottom = -row    * cellSize * 0.5f;
		float left   = -column * cellSize * 0.5f;
		
		// Top-right position
		float top   = -bottom;
		float right = -left;
		
		Vector3 from = Vector3.zero;
		Vector3 to   = Vector3.zero;
		
		//
		float x = left;
		float y = bottom;

		from.y = bottom;
		to.y   = top;
		
		// Draw vertical lines
		for (int i = 0; i <= column; i++)
		{
			from.x = to.x = x;
			
			Gizmos.DrawLine(from, to);
			
			x += cellSize;
		}
		
		//
		from.x = left;
		to.x   = right;
		
		// Draw horizontal lines
		for (int i = 0; i <= row; i++)
		{
			from.y = to.y = y;
			
			Gizmos.DrawLine(from, to);
			
			y += cellSize;
		}
	}
	
	// Get cell at the specified world position
	public bool GetCell(Vector3 position, ref int r, ref int c)
	{
		float width  = column * cellSize;
		float height = row * cellSize;
		
		float x = position.x - transform.position.x + width * 0.5f;
		float y = position.y - transform.position.y + height * 0.5f;
		
		if (x >= 0 && x < width && y >= 0 && y < height)
		{
			r = Mathf.FloorToInt((y - Mathf.Epsilon) / cellSize);
			c = Mathf.FloorToInt((x - Mathf.Epsilon) / cellSize);
			
//			r = row - 1 - r;
			
			return true;
		}
		
		return false;
	}

	// Get world position at the specified cell
	public Vector3 GetPosition(int r, int c)
	{
//		r = row - 1 - r;

		float width  = column * cellSize;
		float height = row * cellSize;

		float x = transform.position.x - width  * 0.5f + (c + 0.5f) * cellSize;
		float y = transform.position.y - height * 0.5f + (r + 0.5f) * cellSize;

		return new Vector3(x, y, 0);
	}
	
//	void Update()
//	{
//		if (Input.touchCount > 0 || Input.GetMouseButton(0))
//		{
//			// Get input position
//			Vector3 inputPosition = GetInputPositionInWorld();
//			
//			int r = -1;
//			int c = -1;
//			
//			if (GetCell(inputPosition, ref r, ref c))
//			{
//				Debug.Log(string.Format("({0},{1})", r, c));
//			}
//		}
//	}
//
//	// Get input position (screen space)
//	Vector3 GetInputPosition()
//	{
//		if (Input.touchCount > 0)
//		{
//			return Input.GetTouch(0).position;
//		}
//		
//		return Input.mousePosition;
//	}
//
//	// Get input position (world space)
//	Vector3 GetInputPositionInWorld()
//	{
//		return Camera.main.ScreenToWorldPoint(GetInputPosition());
//	}
}

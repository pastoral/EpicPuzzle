using UnityEngine;

public class InOutSprite : MeshBehaviour
{
	/// <summary>
	/// The outer size.
	/// </summary>
	public Vector2 outerSize = new Vector2(10.0f, 10.0f);

	/// <summary>
	/// The inner size.
	/// </summary>
	public Vector2 innerSize = new Vector2(5.0f, 5.0f);
	
	/// <summary>
	/// The inner position (relative to center).
	/// </summary>
	public Vector2 innerPosition = Vector2.zero;

	/// <summary>
	/// The pivot.
	/// </summary>
	public Pivot pivot;

	// The vertices
	private Vector3[] _vertices = new Vector3[16];

	// The triangles
	private int[] _triangles =
	{
		0, 4, 1,
		1, 4, 5,
		1, 5, 2,
		2, 5, 6,
		2, 6, 3,
		3, 6, 7,

		4, 8, 5,
		5, 8, 9,
		5, 9, 6,
		6, 9, 10,
		6, 10, 7,
		7, 10, 11,

		8, 12, 9,
		9, 12, 13,
		9, 13, 10,
		10, 13, 14,
		10, 14, 11,
		11, 14, 15
	};

	// The uvs
	private Vector2[] _uvs = new Vector2[16];

	public float InnerSize
	{
		get
		{
			return (innerSize.x + innerSize.y) * 0.5f;
		}
		set
		{
			innerSize = new Vector2(value, value);

			OnValidate();
		}
	}

	/*
	 * 		middle3  middle4
	 * 		   ^		^
	 * 		   |		|
	 *   12    | 13	    | 14   15
	 *    +----+--------+----+
	 *    |    |        |    |
	 *  8 |    | 9      | 10 | 11
	 *    +----+--------+----+---> middle2
	 *    |    |        |    |
	 *    |    |        |    |
	 *  4 |    | 5      | 6  | 7
	 *    +----+--------+----+---> middle1
	 *    |    |        |    |
	 *    |    |        |    |
	 *    +----+--------+----+
	 *    0    1        2    3
	 */
	protected override void OnValidate()
	{
		base.OnValidate();

		if (_sprite == null) return;

		Vector3 position = pivot.GetPosition(outerSize.x, outerSize.y);

		float left   = position.x;
		float right  = left + outerSize.x;
		float bottom = position.y;
		float top    = bottom + outerSize.y;

		float middle1 = bottom + innerPosition.y + (outerSize.y - innerSize.y) * 0.5f;
		float middle2 = middle1 + innerSize.y;
		float middle3 = left + innerPosition.x + (outerSize.x - innerSize.x) * 0.5f;
		float middle4 = middle3 + innerSize.x;

		float extraLeft   = left - middle3;
		float extraRight  = middle4 - right;
		float extraBottom = bottom - middle1;
		float extraTop    = middle2 - top;

		middle1 = Mathf.Clamp(middle1, bottom, top);
		middle2 = Mathf.Clamp(middle2, bottom, top);
		middle3 = Mathf.Clamp(middle3, left, right);
		middle4 = Mathf.Clamp(middle4, left, right);

		// Set vertices
		_vertices[0].x = left;
		_vertices[0].y = bottom;
		_vertices[1].x = middle3;
		_vertices[1].y = bottom;
		_vertices[2].x = middle4;
		_vertices[2].y = bottom;
		_vertices[3].x = right;
		_vertices[3].y = bottom;

		_vertices[4].x = left;
		_vertices[4].y = middle1;
		_vertices[5].x = middle3;
		_vertices[5].y = middle1;
		_vertices[6].x = middle4;
		_vertices[6].y = middle1;
		_vertices[7].x = right;
		_vertices[7].y = middle1;
		
		_vertices[8].x = left;
		_vertices[8].y = middle2;
		_vertices[9].x = middle3;
		_vertices[9].y = middle2;
		_vertices[10].x = middle4;
		_vertices[10].y = middle2;
		_vertices[11].x = right;
		_vertices[11].y = middle2;
		
		_vertices[12].x = left;
		_vertices[12].y = top;
		_vertices[13].x = middle3;
		_vertices[13].y = top;
		_vertices[14].x = middle4;
		_vertices[14].y = top;
		_vertices[15].x = right;
		_vertices[15].y = top;
		
		float marginLeft   = extraLeft > 0 ? extraLeft / innerSize.x : 0;
		float marginRight  = extraRight > 0 ? extraRight / innerSize.x : 0;
		float marginBottom = extraBottom > 0 ? extraBottom / innerSize.y : 0;
		float marginTop    = extraTop > 0 ? extraTop / innerSize.y : 0;

		float uvLeft   = 0;
		float uvTop    = 0;
		float uvRight  = 0;
		float uvBottom = 0;

		float uvMiddle1 = 0;
		float uvMiddle2 = 0;
		float uvMiddle3 = 0;
		float uvMiddle4 = 0;

		_sprite.GetUVs(marginLeft, marginTop, marginRight, marginBottom, ref uvLeft, ref uvTop, ref uvRight, ref uvBottom, ref uvMiddle1, ref uvMiddle2, ref uvMiddle3, ref uvMiddle4);
		
		// Set uvs
		_uvs[0].x = uvLeft;
		_uvs[0].y = uvBottom;
		_uvs[1].x = uvMiddle3;
		_uvs[1].y = uvBottom;
		_uvs[2].x = uvMiddle4;
		_uvs[2].y = uvBottom;
		_uvs[3].x = uvRight;
		_uvs[3].y = uvBottom;

		_uvs[4].x = uvLeft;
		_uvs[4].y = uvMiddle1;
		_uvs[5].x = uvMiddle3;
		_uvs[5].y = uvMiddle1;
		_uvs[6].x = uvMiddle4;
		_uvs[6].y = uvMiddle1;
		_uvs[7].x = uvRight;
		_uvs[7].y = uvMiddle1;
		
		_uvs[8].x = uvLeft;
		_uvs[8].y = uvMiddle2;
		_uvs[9].x = uvMiddle3;
		_uvs[9].y = uvMiddle2;
		_uvs[10].x = uvMiddle4;
		_uvs[10].y = uvMiddle2;
		_uvs[11].x = uvRight;
		_uvs[11].y = uvMiddle2;
		
		_uvs[12].x = uvLeft;
		_uvs[12].y = uvTop;
		_uvs[13].x = uvMiddle3;
		_uvs[13].y = uvTop;
		_uvs[14].x = uvMiddle4;
		_uvs[14].y = uvTop;
		_uvs[15].x = uvRight;
		_uvs[15].y = uvTop;

		// Get mesh
		Mesh mesh = GetMesh();
		mesh.Clear();
		mesh.vertices = _vertices;
		mesh.triangles = _triangles;
		mesh.uv = _uvs;
		mesh.RecalculateNormals();
		;
	}
}

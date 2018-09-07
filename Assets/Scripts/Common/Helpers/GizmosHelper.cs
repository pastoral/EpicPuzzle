using UnityEngine;

public static class GizmosHelper
{
	private static readonly float DoublePI = Mathf.PI * 2.0f;

	public static void DrawRect(float left, float top, float right, float bottom, Color color)
	{
		Gizmos.color = color;
		
		Vector3 from = Vector3.zero;
		Vector3 to   = Vector3.zero;
		
		from.x = to.x = left;
		from.y = bottom;
		to.y   = top;
		Gizmos.DrawLine(from, to);
		
		from.x = to.x = right;
		Gizmos.DrawLine(from, to);
		
		from.x = left;
		from.y = top;
		Gizmos.DrawLine(from, to);
		
		from.y = to.y = bottom;
		Gizmos.DrawLine(from, to);
	}

	public static void DrawRect(Rect rect, Color color)
	{
		DrawRect(rect.xMin, rect.yMin, rect.xMax, rect.yMax, color);
	}
	
	public static void DrawRect(Vector3 center, float size, Color color)
	{
		float halfSize = size * 0.5f;

		DrawRect(center.x - halfSize, center.y + halfSize, center.x + halfSize, center.y - halfSize, color);
	}

	public static void DrawCircle(Vector3 center, float radius, float angleStep = 3.0f, Color? color = null)
	{
		// Convert angle to radian
		angleStep *= Mathf.Deg2Rad;

		Gizmos.color = color ?? Color.blue;
		
		Vector3 from = Vector3.zero;
		Vector3 to   = Vector3.zero;
		
		from.x = center.x + radius;
		from.y = center.y;

		float angle = angleStep;

		while (angle < DoublePI)
		{
			to.x = center.x + Mathf.Cos(angle) * radius;
			to.y = center.y + Mathf.Sin(angle) * radius;
			Gizmos.DrawLine(from, to);

			from = to;

			angle += angleStep;
		}

		to.x = center.x + radius;
		to.y = center.y;
		Gizmos.DrawLine(from, to);
	}
}

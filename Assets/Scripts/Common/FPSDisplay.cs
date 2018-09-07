using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
	public enum Anchor
	{
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}

	/// <summary>
	/// The anchor.
	/// </summary>
	public Anchor anchor = Anchor.TopLeft;

	/// <summary>
	/// The font size.
	/// </summary>
	public int fontSize = 18;

	/// <summary>
	/// The color.
	/// </summary>
	public Color color = Color.red;

	// The delta time
	private float _deltaTime = 0.0f;

	// The style
	GUIStyle _style;

	void Update()
	{
		// Update delta time
		_deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
	}

	void OnGUI()
	{
		if (_style == null)
		{
			_style = new GUIStyle(GUI.skin.label);
			_style.fontSize = fontSize;
			_style.normal.textColor = color;
			_style.fixedWidth = 100;

			if (anchor == Anchor.TopLeft)
			{
				_style.alignment = TextAnchor.UpperLeft;
			}
			else if (anchor == Anchor.TopRight)
			{
				_style.alignment = TextAnchor.UpperRight;
			}
			else if (anchor == Anchor.BottomLeft)
			{
				_style.alignment = TextAnchor.LowerLeft;
			}
			else if (anchor == Anchor.BottomRight)
			{
				_style.alignment = TextAnchor.LowerRight;
			}
		}

		string text = string.Format("FPS: {0}", Mathf.RoundToInt(1.0f / _deltaTime));

		GUILayout.BeginArea(new Rect(10, 5, Screen.width - 20, Screen.height - 10));

		if (anchor == Anchor.TopLeft)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(text, _style);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		else if (anchor == Anchor.TopRight)
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(text, _style);
			GUILayout.EndHorizontal();
		}
		else if (anchor == Anchor.BottomLeft)
		{
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.Label(text, _style);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}
		else if (anchor == Anchor.BottomRight)
		{
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(text, _style);
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}

		GUILayout.EndArea();
	}
}

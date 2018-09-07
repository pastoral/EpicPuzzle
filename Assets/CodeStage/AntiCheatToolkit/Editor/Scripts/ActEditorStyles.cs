using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.Editor
{
	public class ActEditorStyles
	{
		private static GUIStyle boldWrappedLabel;
		public static GUIStyle BoldWrappedLabel
		{
			get
			{
				if (boldWrappedLabel == null)
				{
					boldWrappedLabel = new GUIStyle(EditorStyles.wordWrappedLabel);
					boldWrappedLabel.fontStyle = FontStyle.Bold;
				}

				return boldWrappedLabel;
			}
		}

		private static GUIStyle largeBoldWrappedLabel;
		public static GUIStyle LargeBoldWrappedLabel
		{
			get
			{
				if (largeBoldWrappedLabel == null)
				{
					largeBoldWrappedLabel = new GUIStyle(EditorStyles.largeLabel);
					largeBoldWrappedLabel.wordWrap = true;
					largeBoldWrappedLabel.fontStyle = FontStyle.Bold;
				}

				return largeBoldWrappedLabel;
			}
		}
	}
}
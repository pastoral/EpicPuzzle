namespace CodeStage.AntiCheat.Editor.Editors
{
	using Detectors;
	using UnityEditor;

	[CustomEditor(typeof (SpeedHackDetector))]
	internal class SpeedHackDetectorEditor : ActDetectorEditor
	{
		private SerializedProperty interval;
		private SerializedProperty maxFalsePositives;
		private SerializedProperty coolDown;

		protected override void FindUniqueDetectorProperties()
		{
			interval = serializedObject.FindProperty("interval");
			maxFalsePositives = serializedObject.FindProperty("maxFalsePositives");
			coolDown = serializedObject.FindProperty("coolDown");
		}

		protected override void DrawUniqueDetectorProperties()
		{
			EditorGUILayout.PropertyField(interval);
			EditorGUILayout.PropertyField(maxFalsePositives);
			EditorGUILayout.PropertyField(coolDown);
		}
	}
}

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshBehaviour), true)]
public class MeshBehaviourEditor : Editor
{
	SerializedObject meshBehaviour;

	void OnEnable()
	{
		meshBehaviour = serializedObject;
	}

	public override void OnInspectorGUI()
	{
		meshBehaviour.Update();

		SerializedProperty property = meshBehaviour.GetIterator();

		// Script
		if (property.NextVisible(true))
		{
			EditorGUILayout.PropertyField(property);
		}

		while (property.NextVisible(false))
		{
			if (property.name == "_sortingLayerIndex")
			{
				property.intValue = EditorGUILayout.Popup("Sorting Layer", property.intValue, GetSortingLayerNames());
			}
			else
			{
				EditorGUILayout.PropertyField(property);
			}
		}

		if (GUI.changed)
		{
			meshBehaviour.ApplyModifiedProperties();
		}
	}

	string[] GetSortingLayerNames()
	{
		SortingLayer[] layers = SortingLayer.layers;
		int layerCount = layers.Length;

		string[] names = new string[layerCount];

		for (int i = 0; i < layerCount; i++)
		{
			names[i] = layers[i].name;
		}

		return names;
	}
}

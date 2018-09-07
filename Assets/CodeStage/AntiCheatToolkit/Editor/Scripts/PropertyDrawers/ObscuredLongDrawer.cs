using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.Editor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ObscuredLong))]
	internal class ObscuredLongDrawer : ObscuredPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			EditorGUI.LabelField(position, label.text + " [works in Unity 5+]");
#else
			SerializedProperty hiddenValue = prop.FindPropertyRelative("hiddenValue");
			SetBoldIfValueOverridePrefab(prop, hiddenValue);

			SerializedProperty cryptoKey = prop.FindPropertyRelative("currentCryptoKey");
			SerializedProperty fakeValue = prop.FindPropertyRelative("fakeValue");
			SerializedProperty inited = prop.FindPropertyRelative("inited");

			long currentCryptoKey = cryptoKey.longValue;
			long val = 0;

			if (!inited.boolValue)
			{
				if (currentCryptoKey == 0)
				{
					currentCryptoKey = cryptoKey.longValue = ObscuredLong.cryptoKeyEditor;
				}
				hiddenValue.longValue = ObscuredLong.Encrypt(0, currentCryptoKey);
				inited.boolValue = true;
			}
			else
			{
				val = ObscuredLong.Decrypt(hiddenValue.longValue, currentCryptoKey);
			}

			EditorGUI.BeginChangeCheck();
			val = EditorGUI.LongField(position, label, val);
			if (EditorGUI.EndChangeCheck())
				hiddenValue.longValue = ObscuredLong.Encrypt(val, currentCryptoKey);

			fakeValue.longValue = val;
			ResetBoldFont();
#endif
		}
	}
}
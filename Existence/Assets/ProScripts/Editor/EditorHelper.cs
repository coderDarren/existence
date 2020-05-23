
using UnityEngine;
using UnityEditor;

namespace ProScripts {

	namespace EditorScripts {

		public class EditorHelper : MonoBehaviour {

			public static void CopyPropertyValue(SerializedProperty fromProperty, SerializedProperty toProperty)
			{
				if (!fromProperty.type.Equals(toProperty.type))
				{
					Debug.LogWarning("Attempting to copy [PROPERTY="+fromProperty.name+"] into [PROPERTY="+toProperty.name+"] but there is a type mismatch [TYPE1="+fromProperty.type+",TYPE2="+toProperty.type+"] respectively.");
					return;
				}

				switch (fromProperty.type)
				{
					case "bool": toProperty.boolValue = fromProperty.boolValue; break;
					case "double": toProperty.doubleValue = fromProperty.doubleValue; break;
					case "float": toProperty.floatValue = fromProperty.floatValue; break;
					case "int": toProperty.intValue = fromProperty.intValue; break;
					case "long": toProperty.longValue = fromProperty.longValue; break;
					case "string": toProperty.stringValue = fromProperty.stringValue; break;
					default:
						if (fromProperty.type.Contains("PPtr"))
						{
							toProperty.objectReferenceValue = fromProperty.objectReferenceValue;
						}

						else
						{
							Debug.LogWarning("Attempting to copy [PROPERTY="+fromProperty.name+"] into [PROPERTY="+toProperty.name+"] but [TYPE="+fromProperty.type+"] was not recognized.");
						}
					break;
				}

				//Debug.Log("Attempt to copy property "+fromProperty.name+" into new property "+toProperty.name+" has finished.");
			}

			public static void CopyPropertyList(SerializedProperty fromPropertyList, SerializedProperty toPropertyList)
			{
				toPropertyList.ClearArray();
						
				for (int i = 0; i < fromPropertyList.arraySize; i++)
				{
					SerializedProperty fromProperty = fromPropertyList.GetArrayElementAtIndex(i);
					toPropertyList.InsertArrayElementAtIndex(i);
					SerializedProperty toProperty = toPropertyList.GetArrayElementAtIndex(i);

					int fromPropertyNum = 0;
					SerializedProperty fromPropertyEnd = fromProperty.GetEndProperty();
					//Debug.Log("property copy end: "+fromPropertyEnd.name);
					while (fromProperty.Next(true))
					{
						if (fromProperty.name == fromPropertyEnd.name) break;

						int toPropertyNum = 0;
						SerializedProperty toPropertyEnd = toProperty.GetEndProperty();
						//Debug.Log("new property end: "+toPropertyEnd.name);
						while (toProperty.Next(true))
						{
							if (toProperty.name == toPropertyEnd.name) break;

							if (toPropertyNum == fromPropertyNum)
							{
								CopyPropertyValue(fromProperty, toProperty);
							}
							toPropertyNum++;
						}

						toProperty = toPropertyList.GetArrayElementAtIndex(i);

						fromPropertyNum++;
					}
				}
			}

			public static void IndexPropertyField(SerializedProperty property, string label="", float index=0, int labelWidth=48, int propertyWidth=0)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(20 * index);
				if (label.Equals(string.Empty))
				{
					EditorGUILayout.PropertyField(property);
				}
				else
				{
					GUILayout.Label(label, GUILayout.Width(labelWidth));
					if (propertyWidth == 0)
					{
						EditorGUILayout.PropertyField(property, GUIContent.none);
					}
					else
					{
						EditorGUILayout.PropertyField(property, GUIContent.none, GUILayout.Width(propertyWidth));
					}
				}
				EditorGUILayout.EndHorizontal();
			}

			public static void IndexBoldLabelField(string label, int index, GUISkin skin)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(20 * index);

				if (EditorGUIUtility.isProSkin)
				{
					GUILayout.Label(label, skin.GetStyle("BoldLabelPro"));
				}

				else
				{
					GUILayout.Label(label, skin.GetStyle("BoldLabel"));
				}

				EditorGUILayout.EndHorizontal();
			}

			public static void IndexLabelField(string label, int index)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(20 * index);

				GUILayout.Label(label);

				EditorGUILayout.EndHorizontal();
			}

			public static void DrawFullHorizontalLine(float viewWidth)
			{
				GUILayout.Space(2);
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(18);
				GUILayout.Box("",GUILayout.Width(viewWidth - 32), GUILayout.Height(1));
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(2);
			}
		}
	}
}

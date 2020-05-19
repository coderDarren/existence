using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace ProScripts {

	namespace EditorScripts {

		[CustomEditor(typeof(ProButton), true)]
		public class ProButtonEditor : Editor {

			private void OnEnable() 
			{
				m_serializedObject = new SerializedObject(target);
			}

			private void OnDisable() {}

			public override void OnInspectorGUI()
			{
				GUILayout.Space(8);

				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button("Enhanced Edit Mode", EditorStyles.toolbarButton))
				{
					ProButtonWindow.Open(m_serializedObject, (ProButton)target);
				}

				Color c = GUI.backgroundColor;
				GUI.backgroundColor = Color.grey;

				if (GUILayout.Button(m_showDefaultInspector ? "Hide Default Inspector" : "Show Default Inspector", EditorStyles.toolbarButton))
				{
					m_showDefaultInspector = !m_showDefaultInspector;
				}

				GUI.backgroundColor = c;

				EditorGUILayout.EndHorizontal();
				
				if (m_showDefaultInspector)
				{
					base.OnInspectorGUI();
				}
			}

			private SerializedObject m_serializedObject;
			private bool m_showDefaultInspector;

		}
	}
}


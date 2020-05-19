using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace ProScripts {
		
	namespace EditorScripts {

		public class ProButtonWindow : EditorWindow {

			private void OnEnable() 
			{
				m_skin = Resources.Load("ProButtonSkin") as GUISkin;
				
				if (!EditorUpdateManager.isUpdating)
				{
					EditorUpdateManager.BeginUpdating();
				}

				RegisterUpdateRoutines();
			}

			private void OnDisable()
			{
				UnregisterUpdateRoutines();
				
				if (EditorUpdateManager.isUpdating)
				{
					EditorUpdateManager.StopUpdating();
				}
			}

			public static void Open(SerializedObject so, ProButton proButton)
			{
				m_serializedObject = so;
				m_window = (ProButtonWindow)GetWindow(typeof(ProButtonWindow));
				m_window.title = "Pro Button";
				m_window.minSize = new Vector2(340, 300);
				m_proButton = proButton;
			}

			private void OnGUI()
			{

				if (m_serializedObject == null)
				{
					EditorGUILayout.HelpBox("Editing session was interrupted. Please reopen this window.", MessageType.Error);
					return;
				}

				m_serializedObject.Update();

				m_viewWidth = position.width - 6;

				m_menuSelection = GUILayout.Toolbar(m_menuSelection, m_menuOptions, EditorStyles.toolbarButton);

				m_currMenuOption = (MenuOptions)Enum.Parse(typeof(MenuOptions), m_menuOptions[m_menuSelection]);

				EditorHelper.DrawFullHorizontalLine(m_viewWidth);

				switch (m_currMenuOption)
				{
					case MenuOptions.Events: DrawEventSettings(); break;
					case MenuOptions.Animations: DrawAnimationSettings(); break;
					case MenuOptions.Misc: DrawMiscSettings(); break;
				}
				
				m_serializedObject.ApplyModifiedProperties();
			}

			private void DrawAnimationSettings()
			{
				m_settingSelection = GUILayout.Toolbar(m_settingSelection, m_styleOptions, EditorStyles.toolbarButton);
				m_buttonStateSelection = GUILayout.Toolbar(m_buttonStateSelection, m_buttonStateOptions, EditorStyles.toolbarButton);

				m_currStyleOption = (StyleOptions)Enum.Parse(typeof(StyleOptions), m_styleOptions[m_settingSelection]);
				m_currStateOption = (StateOptions)Enum.Parse(typeof(StateOptions), m_buttonStateOptions[m_buttonStateSelection]);

				PerformCopyInstruction();

				switch (m_currStyleOption)
				{
					case StyleOptions.Canvas:	DetermineListDrawerForButtonElement(BUTTON_ELEMENT_CANVAS_SETTINGS); break;
					case StyleOptions.Color:	DetermineListDrawerForButtonElement(BUTTON_ELEMENT_COLOR_SETTINGS); break; 
					case StyleOptions.Fill:		DetermineListDrawerForButtonElement(BUTTON_ELEMENT_FILL_SETTINGS); break; 
					case StyleOptions.Scale:	DetermineListDrawerForButtonElement(BUTTON_ELEMENT_SIZE_SETTINGS); break; 
					case StyleOptions.Rotation:	DetermineListDrawerForButtonElement(BUTTON_ELEMENT_ROTATION_SETTINGS); break; 
					case StyleOptions.Position:	DetermineListDrawerForButtonElement(BUTTON_ELEMENT_POSITION_SETTINGS); break;
				}
				
			}

			private void DrawEventSettings()
			{
				m_buttonStateSelection = GUILayout.Toolbar(m_buttonStateSelection, m_buttonStateOptions, EditorStyles.toolbarButton);
				m_currStateOption = (StateOptions)Enum.Parse(typeof(StateOptions), m_buttonStateOptions[m_buttonStateSelection]);

				SerializedProperty stateProperty = null;

				switch (m_currStateOption)
				{
					case StateOptions.Rest:
					
					stateProperty = m_serializedObject.FindProperty("restSettings");
					EditorHelper.IndexBoldLabelField("Event on Rest", 1, m_skin);
					EditorHelper.IndexPropertyField(stateProperty.FindPropertyRelative("eventAction"), index:1);

					break;

					case StateOptions.Hover:

					stateProperty = m_serializedObject.FindProperty("hoverSettings");
					EditorHelper.IndexBoldLabelField("Event on Hover", 1, m_skin);
					EditorHelper.IndexPropertyField(stateProperty.FindPropertyRelative("eventAction"), index:1);

					break;

					case StateOptions.Up:

					stateProperty = m_serializedObject.FindProperty("upSettings");
					EditorHelper.IndexBoldLabelField("Event on Up", 1, m_skin);
					EditorHelper.IndexPropertyField(stateProperty.FindPropertyRelative("eventAction"), index:1);

					break;

					case StateOptions.Down:

					stateProperty = m_serializedObject.FindProperty("downSettings");
					EditorHelper.IndexBoldLabelField("Event on Down", 1, m_skin);
					EditorHelper.IndexPropertyField(stateProperty.FindPropertyRelative("eventAction"), index:1);

					break;

					case StateOptions.Hold:

					GUILayout.Space(8);
					EditorHelper.IndexLabelField("Down events are used during the hold event.", index:1);

					break;
				}
			}

			private void DrawMiscSettings()
			{
				GUILayout.Space(8);

				EditorHelper.IndexPropertyField(m_serializedObject.FindProperty("parentButton"), index:1, label:"Parent Button", labelWidth:100);
				EditorHelper.IndexPropertyField(m_serializedObject.FindProperty("debugThisButton"), index:1, label:"Debug This Button", labelWidth:120);
			}

			private void DetermineListDrawerForButtonElement(string element)
			{
				string buttonElement = m_currStyleOption.ToString();

				switch (m_currStateOption)
				{
					case StateOptions.Rest:
					
					EditorHelper.IndexBoldLabelField(buttonElement + " On Rest", 1, m_skin);
					GUILayout.Space(4);
					ShowListForStateAndElement("restSettings", element);

					break;

					case StateOptions.Hover:

					EditorHelper.IndexBoldLabelField(buttonElement + " On Hover", 1, m_skin);
					GUILayout.Space(4);
					ShowListForStateAndElement("hoverSettings", element);

					break;

					case StateOptions.Up:

					EditorHelper.IndexBoldLabelField(buttonElement + " On Up", 1, m_skin);
					GUILayout.Space(4);
					ShowListForStateAndElement("upSettings", element);

					break;

					case StateOptions.Down:

					EditorHelper.IndexBoldLabelField(buttonElement + " On Down", 1, m_skin);
					GUILayout.Space(4);
					ShowListForStateAndElement("downSettings", element);

					break;

					case StateOptions.Hold:

					GUILayout.Space(8);
					SerializedProperty holdSettings = m_serializedObject.FindProperty("holdSettings");
					EditorHelper.IndexPropertyField(holdSettings.FindPropertyRelative("enabled"), label:"Enable Hold Action", labelWidth:150, index:1);

					if (holdSettings.FindPropertyRelative("enabled").boolValue)
					{
						EditorHelper.IndexPropertyField(holdSettings.FindPropertyRelative("enableDelay"), label:"Delay Before Hold", labelWidth:150, index:1);
						EditorHelper.IndexPropertyField(holdSettings.FindPropertyRelative("executionInterval"), label:"Time Between Actions", labelWidth:150, index:1);
					}

					break;
				}
			}

			private void PerformCopyInstruction()
			{
				if (m_currStateOption == StateOptions.Hold) return;

				GUILayout.Space(8);
				EditorGUILayout.BeginHorizontal(GUILayout.Width(m_viewWidth - 20));
				GUILayout.Space(24);
				
				SerializedProperty fromPropertyList = null;
				SerializedProperty toPropertyList = m_serializedObject.FindProperty(GetSettingNameForSelectedState()).FindPropertyRelative(GetSettingNameForSelectedStyle());

				if (m_currStateOption != StateOptions.Rest)
				{
					if (GUILayout.Button("From Rest", EditorStyles.toolbarButton))
					{
						fromPropertyList = m_serializedObject.FindProperty("restSettings").FindPropertyRelative(GetSettingNameForSelectedStyle());
						EditorHelper.CopyPropertyList(fromPropertyList, toPropertyList);
					}
				}

				if (m_currStateOption != StateOptions.Hover)
				{
					if (GUILayout.Button("From Hover", EditorStyles.toolbarButton))
					{
						fromPropertyList = m_serializedObject.FindProperty("hoverSettings").FindPropertyRelative(GetSettingNameForSelectedStyle());
						EditorHelper.CopyPropertyList(fromPropertyList, toPropertyList);
					}
				}

				if (m_currStateOption != StateOptions.Down)
				{
					if (GUILayout.Button("From Down", EditorStyles.toolbarButton))
					{
						fromPropertyList = m_serializedObject.FindProperty("downSettings").FindPropertyRelative(GetSettingNameForSelectedStyle());
						EditorHelper.CopyPropertyList(fromPropertyList, toPropertyList);
					}
				}

				if (m_currStateOption != StateOptions.Up)
				{
					if (GUILayout.Button("From Up", EditorStyles.toolbarButton))
					{
						fromPropertyList = m_serializedObject.FindProperty("upSettings").FindPropertyRelative(GetSettingNameForSelectedStyle());
						EditorHelper.CopyPropertyList(fromPropertyList, toPropertyList);
					}
				}

				EditorGUILayout.EndHorizontal();
				GUILayout.Space(4);

			}

			private void DeterminePropertyDrawerForButtonElement(SerializedProperty element, SerializedProperty elementState, string elementName)
			{
				switch (elementName)
				{
					case BUTTON_ELEMENT_CANVAS_SETTINGS:

					EditorHelper.IndexBoldLabelField("Target State", 1, m_skin);
				
					DetermineObjectDrawerForButtonElement(element, GetObjectNameForButtonElement(elementName));
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("alpha"), "Alpha", 1);
					EditorGUILayout.BeginHorizontal();
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("delay"), "Delay", 1);
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("timeToAlpha"), "Time", 1);
					EditorGUILayout.EndHorizontal();
					EditorHelper.IndexPropertyField(element.FindPropertyRelative("shouldBeActive"), "Should Be Active", 1, labelWidth:105);
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("usePostValues"), "Use Post Values", 1, labelWidth:95);

					if (elementState.FindPropertyRelative("usePostValues").boolValue)
					{
						EditorHelper.IndexBoldLabelField("Post State", 1, m_skin);
						
						EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("postAlpha"), "Alpha", 1);
						EditorGUILayout.BeginHorizontal();
						EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("postDelay"), "Delay", 1);
						EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("postTimeToAlpha"), "Time", 1);
						EditorGUILayout.EndHorizontal();
					}

					break;

					case BUTTON_ELEMENT_COLOR_SETTINGS:

					EditorHelper.IndexBoldLabelField("Target State", 1, m_skin);

					DetermineObjectDrawerForButtonElement(element, "img");
					DetermineObjectDrawerForButtonElement(element, "txt");
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("color"), "Color", 1);
					EditorGUILayout.BeginHorizontal();
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("delay"), "Delay", 1);
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("timeToColor"), "Time", 1);
					EditorGUILayout.EndHorizontal();
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("usePostValues"), "Use Post Values", 1, labelWidth:95);

					if (elementState.FindPropertyRelative("usePostValues").boolValue)
					{
						EditorHelper.IndexBoldLabelField("Post State", 1, m_skin);
						
						EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("postColor"), "Color", 1);
						EditorGUILayout.BeginHorizontal();
						EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("postDelay"), "Delay", 1);
						EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("postTimeToColor"), "Time", 1);
						EditorGUILayout.EndHorizontal();
					}

					break;

					case BUTTON_ELEMENT_FILL_SETTINGS:

					EditorHelper.IndexBoldLabelField("Target State", 1, m_skin);

					DetermineObjectDrawerForButtonElement(element, GetObjectNameForButtonElement(elementName));
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("fill"), "Fill", 1);
					EditorGUILayout.BeginHorizontal();
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("delay"), "Delay", 1);
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("timeToFill"), "Time", 1);
					EditorGUILayout.EndHorizontal();
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("usePostValues"), "Use Post Values", 1, labelWidth:95);

					if (elementState.FindPropertyRelative("usePostValues").boolValue)
					{
						EditorHelper.IndexBoldLabelField("Post State", 1, m_skin);
						
						EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("postFill"), "Fill", 1);
						EditorGUILayout.BeginHorizontal();
						EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("postDelay"), "Delay", 1);
						EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("postTimeToFill"), "Time", 1);
						EditorGUILayout.EndHorizontal();
					}

					break;

					case BUTTON_ELEMENT_SIZE_SETTINGS:

					EditorHelper.IndexBoldLabelField("Target State", 1, m_skin);

					DetermineObjectDrawerForButtonElement(element, GetObjectNameForButtonElement(elementName));
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("vec"), "Size", 1);
					EditorGUILayout.BeginHorizontal();
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("delay"), "Delay", 1);
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("timeToVec"), "Time", 1);
					EditorGUILayout.EndHorizontal();
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("usePostValues"), "Use Post Values", 1, labelWidth:95);

					if (elementState.FindPropertyRelative("usePostValues").boolValue)
					{
						EditorHelper.IndexBoldLabelField("Post State", 1, m_skin);
						
						EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("postVec"), "Size", 1);
						EditorGUILayout.BeginHorizontal();
						EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("postDelay"), "Delay", 1);
						EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("postTimeToVec"), "Time", 1);
						EditorGUILayout.EndHorizontal();
					}

					break;

					case BUTTON_ELEMENT_ROTATION_SETTINGS:

					EditorHelper.IndexBoldLabelField("Target State", 1, m_skin);

					DetermineObjectDrawerForButtonElement(element, GetObjectNameForButtonElement(elementName));
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("vec"), "Euler", 1);
					EditorGUILayout.BeginHorizontal();
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("delay"), "Delay", 1);
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("timeToVec"), "Time", 1);
					EditorGUILayout.EndHorizontal();
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("usePostValues"), "Use Post Values", 1, labelWidth:95);

					if (elementState.FindPropertyRelative("usePostValues").boolValue)
					{
						EditorHelper.IndexBoldLabelField("Post State", 1, m_skin);
						
						EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("postVec"), "Euler", 1);
						EditorGUILayout.BeginHorizontal();
						EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("postDelay"), "Delay", 1);
						EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("postTimeToVec"), "Time", 1);
						EditorGUILayout.EndHorizontal();
					}

					break;

					case BUTTON_ELEMENT_POSITION_SETTINGS:

					EditorHelper.IndexBoldLabelField("Target State", 1, m_skin);

					DetermineObjectDrawerForButtonElement(element, GetObjectNameForButtonElement(elementName));
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("vec"), "Pos", 1);
					EditorGUILayout.BeginHorizontal();
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("delay"), "Delay", 1);
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("timeToVec"), "Time", 1);
					EditorGUILayout.EndHorizontal();
					EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("usePostValues"), "Use Post Values", 1, labelWidth:95);

					if (elementState.FindPropertyRelative("usePostValues").boolValue)
					{
						EditorHelper.IndexBoldLabelField("Post State", 1, m_skin);
						
						EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("postVec"), "Pos", 1);
						EditorGUILayout.BeginHorizontal();
						EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("postDelay"), "Delay", 1);
						EditorHelper.IndexPropertyField(elementState.FindPropertyRelative("postTimeToVec"), "Time", 1);
						EditorGUILayout.EndHorizontal();
					}

					break;
				}
			}

			private void DetermineObjectDrawerForButtonElement(SerializedProperty element, string elementName)
			{
				SerializedProperty objProperty = element.FindPropertyRelative(elementName);
				if (objProperty == null) return;
				EditorHelper.IndexPropertyField(objProperty, elementName, 1);
			}

			private UnityEngine.Object GetObjectForButtonElement(SerializedProperty element, string elementName)
			{
				SerializedProperty objProperty = element.FindPropertyRelative(elementName);
				if (objProperty == null) 
				{
					return null;
				}
				UnityEngine.Object o = objProperty.objectReferenceValue;
				if (o == null)
				{
					objProperty = element.FindPropertyRelative("txt");
					if (objProperty != null)
						o = objProperty.objectReferenceValue;
				}
				return o;
			}

			private string GetObjectNameForButtonElement(string elementName)
			{
				switch (elementName)
				{
					case BUTTON_ELEMENT_CANVAS_SETTINGS: return "canvas";
					case BUTTON_ELEMENT_COLOR_SETTINGS:
					case BUTTON_ELEMENT_FILL_SETTINGS: return "img";
					case BUTTON_ELEMENT_SIZE_SETTINGS:
					case BUTTON_ELEMENT_ROTATION_SETTINGS:
					case BUTTON_ELEMENT_POSITION_SETTINGS: return "obj";
					default: return string.Empty;
				}
			}

			private string GetSettingNameForSelectedState()
			{
				switch (m_currStateOption)
				{
					case StateOptions.Rest: return "restSettings";
					case StateOptions.Hover: return "hoverSettings";
					case StateOptions.Down: return "downSettings";
					case StateOptions.Up: return "upSettings";
					default: return string.Empty;
				}
			}

			private string GetSettingNameForSelectedStyle()
			{
				switch (m_currStyleOption)
				{
					case StyleOptions.Canvas: return "canvasSettings";
					case StyleOptions.Color: return "colorSettings";
					case StyleOptions.Fill: return "fillSettings";
					case StyleOptions.Scale: return "sizeSettings";
					case StyleOptions.Rotation: return "rotationSettings";
					case StyleOptions.Position: return "positionSettings";
					default: return string.Empty;
				}
			}

			Vector2 scrollPos;
			bool fadeGroup;
			List<AnimBool> expandAnims;
			private void ShowListForStateAndElement(string state, string element)
			{
				Color tempGUIcolor= GUI.color;

				if (expandAnims == null)
				{
					expandAnims = new List<AnimBool>();
				}

				SerializedProperty stateElement = m_serializedObject.FindProperty(state);
				SerializedProperty elementList = stateElement.FindPropertyRelative(element);
				SerializedProperty item = null;

				EditorGUILayout.BeginHorizontal(GUILayout.Width(m_viewWidth - 8));
				GUILayout.Space(12);

				tempGUIcolor = GUI.color;
				GUI.color = Color.green;
				if (GUILayout.Button("Add", EditorStyles.toolbarButton))
				{
					AnimBool expandAnim = new AnimBool(true);
					expandAnim.valueChanged.AddListener(Repaint);
					expandAnims.Add(expandAnim);
					elementList.InsertArrayElementAtIndex(elementList.arraySize);
				}
				GUI.color = tempGUIcolor;

				EditorGUILayout.EndHorizontal();

				EditorHelper.DrawFullHorizontalLine(m_viewWidth);

				scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

				for (int i = 0; i < elementList.arraySize; i++)
				{	
					item = elementList.GetArrayElementAtIndex(i);
					SerializedProperty itemState = item.FindPropertyRelative("state");
					UnityEngine.Object o = GetObjectForButtonElement(item, GetObjectNameForButtonElement(element));

					EditorGUILayout.BeginHorizontal(GUILayout.Width(m_viewWidth - 8));
					GUILayout.Space(12);

					if (expandAnims.Count <= i)
					{
						AnimBool expandAnim = new AnimBool(true);
						expandAnim.valueChanged.AddListener(Repaint);
						expandAnims.Insert(i, expandAnim);
					}

					tempGUIcolor = GUI.color;
					GUI.color = Color.cyan;
					if (GUILayout.Button(o == null ? "null "+i+"" : o.name, EditorStyles.toolbarButton))
					{
						expandAnims[i].target = !expandAnims[i].target;
					}
					GUI.color = tempGUIcolor;

					GUILayout.Space(50);

					tempGUIcolor = GUI.color;
					GUI.color = Color.red;
					if (GUILayout.Button("Remove", EditorStyles.toolbarButton, GUILayout.Width(64)))
					{
						expandAnims.RemoveAt(i);
						elementList.DeleteArrayElementAtIndex(i);
						continue;
					}
					GUI.color = tempGUIcolor;

					EditorGUILayout.EndHorizontal();

					if (EditorGUILayout.BeginFadeGroup(expandAnims[i].faded))
					{
						DeterminePropertyDrawerForButtonElement(item, itemState, element);
					}
					EditorGUILayout.EndFadeGroup();

					EditorHelper.DrawFullHorizontalLine(m_viewWidth);	

				}

				GUILayout.Space(16);

				EditorGUILayout.EndScrollView();
			}

#region UPDATE ROUTINES

			private void RegisterUpdateRoutines()
			{
				//EditorUpdateManager.RegisterRoutine(TestRoutine);
			}

			private void UnregisterUpdateRoutines()
			{
				//EditorUpdateManager.UnregisterRoutine(TestRoutine);
			}

			private void TestRoutine()
			{
				//Debug.Log("test updating");
			}

#endregion

			private static ProButtonWindow m_window;
			private static SerializedObject m_serializedObject;
			private static ProButton m_proButton;

	#region constant-members

			private const string BUTTON_ELEMENT_CANVAS_SETTINGS = "canvasSettings";
			private const string BUTTON_ELEMENT_COLOR_SETTINGS = "colorSettings";
			private const string BUTTON_ELEMENT_FILL_SETTINGS = "fillSettings";
			private const string BUTTON_ELEMENT_POSITION_SETTINGS = "positionSettings";
			private const string BUTTON_ELEMENT_ROTATION_SETTINGS = "rotationSettings";
			private const string BUTTON_ELEMENT_SIZE_SETTINGS = "sizeSettings";

	#endregion
			
	#region expand-members

			private bool m_showEvents;

	#endregion

	#region toolbar-members

			private string[] m_menuOptions = new string[] { "Events", "Animations", "Misc" };
			private string[] m_styleOptions = new string[] { "Canvas", "Color", "Fill", "Scale", "Rotation", "Position" };
			private string[] m_buttonStateOptions = new string[] { "Rest", "Hover", "Down", "Up", "Hold" };
			
			private int m_menuSelection;
			private int m_settingSelection;
			private int m_buttonStateSelection;

	#endregion

	#region gui-skin-members

			private GUISkin m_skin;
			private float m_viewWidth;

	#endregion

	#region types

			private enum MenuOptions 
			{
				Events,
				Animations,
				Misc,
			}

			private enum StyleOptions
			{
				Canvas,
				Color,
				Fill,
				Scale,
				Rotation,
				Position
			}

			private enum StateOptions
			{
				Rest,
				Hover,
				Up,
				Down,
				Hold
			}

			private MenuOptions m_currMenuOption;
			private StyleOptions m_currStyleOption;
			private StateOptions m_currStateOption;

	#endregion

		}

	}
}

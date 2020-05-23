using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace ProScripts {

	namespace EditorScripts {

		public class EditorUpdateManager : Editor {

			public static void BeginUpdating()
			{
				if (actions == null)
				{
					actions = new List<Action>();
				}

				EditorApplication.update += EditorUpdate;

				isUpdating = true;
			}

			public static void StopUpdating()
			{
				if (actions != null)
				{
					actions.Clear();
				}

				EditorApplication.update -= EditorUpdate;

				isUpdating = false;
			}

			public static void RegisterRoutine(Action a)
			{
				if (actions.Contains(a)) return;

				actions.Add(a);
			}

			public static void UnregisterRoutine(Action a)
			{
				if (!actions.Contains(a)) return;

				actions.Remove(a);
			}

			private static void EditorUpdate()
			{
				foreach (Action action in actions)
				{
					action.Invoke();
				}
			}

			private static List<Action> actions;

			public static bool isUpdating {get; private set;}
		}
	}
}

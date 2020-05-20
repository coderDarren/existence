using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProScripts {

	namespace Tests {

		/// <summary>
		/// A pattern for getting buttons to do "things".
		/// Link events from the advanced button editor.
		/// </summary>
		public class ProButtonEventTest : MonoBehaviour {

			public void ButtonHoverTest()
			{
				Log("hover");
			}

			public void ButtonRestTest()
			{
				Log("rest");
			}

			public void ButtonDownTest()
			{
				Log("down");
			}

			public void ButtonUpTest()
			{
				Log("up");
			}

			private void Log(string msg)
			{
				Debug.Log("[EVENT CALLED VIA BUTTON] " +msg);
			}

		}
	}
}

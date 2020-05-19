
using UnityEngine;
using UnityEngine.UI;

namespace ProScripts {

	namespace Tests {

		/// <summary>
		/// A pattern for getting buttons to do "things"
		/// Sometimes it is nice to have an entirely new class for buttons which..
		/// ..complete tasks that are dependent on variables such as:
		///
		///		•Scene Types
		///		•Page Types
		///		•Anything that makes sense for your setup!
		///
		/// </summary>
		public class ProButtonOverrideTest : ProButton {
			
			/* 
			//called on OnEnable and Start
			public override void Init()
			{
				base.Init();
			}

			public override void Dispose()
			{
				base.Dispose();
			}

			//also known as 'rest'
			public override void OnButtonExit()
			{
				base.OnButtonExit();
			}

			public override void OnButtonDown()
			{
				base.OnButtonDown();
			}
			
			public override void OnButtonHold()
			{
				base.OnButtonHold();
			}		

			public override void OnButtonUp()
			{
				base.OnButtonUp();
			}*/

			public override void OnButtonEnter()
			{
				base.OnButtonEnter();

				helpBoxTitleLabel.text = title;
				helpBoxDescriptionLabel.text = description;
			}

			public Text helpBoxTitleLabel;
			public Text helpBoxDescriptionLabel;
			public string title;
			public string description;

		}
	}
}

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace ProScripts {

	public class ProButtonData : ScriptableObject {

#region class-members
		
		[Header("Event Effects")]
		[Header("On Rest/Exit")]
		public StateSettings restSettings;
		[Header("On Hover")]
		public StateSettings hoverSettings;
		[Header("On Up")]
		public StateSettings upSettings;
		[Header("On Down")]
		public StateSettings downSettings;
		[Header("On Hold")]
		public HoldSettings holdSettings;

#endregion
	}

#region helper-classes

		[System.Serializable]
		public struct VectorState
		{
			[Tooltip("Delay before attempting to reach this state.")]
			public float delay;
			[Tooltip("To what vector should this element be at during this state?")]
			public Vector3 vec;
			[Tooltip("How long should it take to reach the target vector?")]
			public float timeToVec;
			[Tooltip("Should the vector move to another value after this state is reached?")]
			public bool usePostValues;
			[Tooltip("Delay before attempting to reach the post state.")]
			public float postDelay;
			[Tooltip("The vector to move to after this state is reached.")]
			public Vector3 postVec;
			[Tooltip("The time taken to reach the 'postVec'.")]
			public float postTimeToVec;

			public VectorState(float delay, Vector3 vec, float timeToVec, bool usePostValues, float postDelay, Vector3 postVec, float postTimeToVec)
			{
				this.delay = delay;
				this.vec = vec;
				this.timeToVec = timeToVec;
				this.usePostValues = usePostValues;
				this.postDelay = postDelay;
				this.postVec = postVec;
				this.postTimeToVec = postTimeToVec;
			}
		}

		[System.Serializable]
		public struct ColorState
		{
			[Tooltip("Delay before attempting to reach this state.")]
			public float delay;
			[Tooltip("To what color should this element be at during this state?")]
			public Color color;
			[Tooltip("How long should it take to reach the target color?")]
			public float timeToColor;
			[Tooltip("Should the color move to another value after this state is reached?")]
			public bool usePostValues;
			[Tooltip("Delay before attempting to reach the post state.")]
			public float postDelay;
			[Tooltip("The color to move to after this state is reached.")]
			public Color postColor;
			[Tooltip("The time taken to reach the 'postColor'.")]
			public float postTimeToColor;

			public ColorState(float delay, Color color, float timeToColor, bool usePostValues, float postDelay, Color postColor, float postTimeToColor)
			{
				this.delay = delay;
				this.color = color;
				this.timeToColor = timeToColor;
				this.usePostValues = usePostValues;
				this.postDelay = postDelay;
				this.postColor = postColor;
				this.postTimeToColor = postTimeToColor;
			}
		}

		[System.Serializable]
		public struct FillState
		{
			[Tooltip("Delay before attempting to reach this state.")]
			public float delay;
			[Tooltip("To what fill amount should this element be at during this state?")]
			[Range(0.0f,1.0f)]
			public float fill;
			[Tooltip("How long should it take to reach the target fill?")]
			public float timeToFill;
			[Tooltip("Should the color move to another value after this state is reached?")]
			public bool usePostValues;
			[Tooltip("Delay before attempting to reach the post state.")]
			public float postDelay;
			[Tooltip("The fill amount to move to after this state is reached.")]
			[Range(0.0f,1.0f)]
			public float postFill;
			[Tooltip("The time taken to reach the 'postFill'.")]
			public float postTimeToFill;

			public FillState(float delay, float fill, float timeToFill, bool usePostValues, float postDelay, float postFill, float postTimeToFill)
			{
				this.delay = delay;
				this.fill = fill;
				this.timeToFill = timeToFill;
				this.usePostValues = usePostValues;
				this.postDelay = postDelay;
				this.postFill = postFill;
				this.postTimeToFill = postTimeToFill;
			}
		}

		[System.Serializable]
		public struct CanvasState
		{
			[Tooltip("Delay before attempting to reach this state.")]
			public float delay;
			[Tooltip("To what opacity should this element be at during this state?")]
			[Range(0.0f,1)]
			public float alpha;
			[Tooltip("How long should it take to reach the target opacity?")]
			public float timeToAlpha;
			[Tooltip("Should the opacity move to another value after this state is reached?")]
			public bool usePostValues;
			[Tooltip("Delay before attempting to reach the post state.")]
			public float postDelay;
			[Tooltip("The opacity to move to after this state is reached.")]
			[Range(0.01f,1)]
			public float postAlpha;
			[Tooltip("The time taken to reach the 'postAlpha'.")]
			public float postTimeToAlpha;

			public CanvasState(float delay, float alpha, float timeToAlpha, bool usePostValues, float postDelay, float postAlpha, float postTimeToAlpha)
			{
				this.delay = delay;
				this.alpha = alpha;
				this.timeToAlpha = timeToAlpha;
				this.usePostValues = usePostValues;
				this.postDelay = postDelay;
				this.postAlpha = postAlpha;
				this.postTimeToAlpha = postTimeToAlpha;
			}
		}

		[System.Serializable]
		public class StateSettings
		{
			[Tooltip("If you want to display canvas groups during this state, modify here.")]
			public List<CanvasSettings> canvasSettings;
			[Tooltip("If you want to modify image colors, use these settings.")]
			public List<ColorSettings> colorSettings;
			[Tooltip("If you want to modify image fill amounts, use these settings.")]
			public List<FillSettings> fillSettings;
			[Tooltip("If you want to modify scale of RectTransforms, look here.")]
			public List<SizeSettings> sizeSettings;
			[Tooltip("If you want to change the position of RectTransforms, check this out.")]
			public List<PositionSettings> positionSettings;
			[Tooltip("If you want to manipulate the rotation of RectTransforms, see here.")]
			public List<RotationSettings> rotationSettings;
			public UnityEvent eventAction;
		}

		[System.Serializable]
		public class CanvasSettings
		{
			[Tooltip("The UI elements that appear with this button is interacted with.")]
			public CanvasGroup canvas;
			[Tooltip("The canvas properties for the state of this button.")]
			public CanvasState state;
			[Tooltip("Should the canvas be interactable during this state?")]
			public bool shouldBeActive;
			
			public IEnumerator coroutine;
		}

		[System.Serializable]
		public class ColorSettings
		{
			[Tooltip("The image that follows the rules for these settings.")]
			public Image img;
			[Tooltip("The text that follows the rules for these settings.")]
			public Text txt;
			[Tooltip("The color properties for the state of this button.")]
			public ColorState state;

			public IEnumerator coroutine;
		}

		[System.Serializable]
		public class FillSettings
		{
			[Tooltip("The image that follows the rules for these settings.")]
			public Image img;
			[Tooltip("The fill properties for the state of this button.")]
			public FillState state;

			public IEnumerator coroutine;
		}

		[System.Serializable]
		public class SizeSettings
		{
			[Tooltip("The transform that follows the rules for these settings.")]
			public Transform obj;
			[Tooltip("The size properties for the state of this button.")]
			public VectorState state;

			public IEnumerator coroutine;
		}

		[System.Serializable]
		public class PositionSettings
		{
			[Tooltip("The transform that follows the rules for these settings.")]
			public Transform obj;
			[Tooltip("The position properties for the state of this button.")]
			public VectorState state;

			public IEnumerator coroutine;
		}

		[System.Serializable]
		public class RotationSettings
		{
			[Tooltip("The transform that follows the rules for these settings.")]
			public Transform obj;
			[Tooltip("The rotation properties for the state of this button.")]
			public VectorState state;

			public IEnumerator coroutine;
		}

		[System.Serializable]
		public struct HoldSettings
		{
			[Tooltip("Determines whether these settings will be used or not.")]
			public bool 	enabled;
			[Tooltip("The amount of time from initial activation before hold executes.")]
			public float 	enableDelay;
			[Tooltip("The amount of time between each execution during hold.")]
			public float 	executionInterval;
		}

#endregion
}

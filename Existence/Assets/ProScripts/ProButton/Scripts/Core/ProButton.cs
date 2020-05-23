using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ProScripts {

	/// <summary>
	/// This component was built to make it easy to create complex button event animations.
	/// </summary>
	[RequireComponent(typeof(Image))]
	public class ProButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler  {
	
#region unity-event-functions
		private void Start() {
			Init();
			m_initOnEnable = true;
		}

		private void OnEnable() {
			if (m_initOnEnable && m_enabled)
				Init();
		}

		private void OnDisable() {
			if (m_enabled)
				Dispose();
		}

#endregion

#region private-functions

		private IEnumerator Hold()
		{
			yield return new WaitForSeconds(holdSettings.enableDelay);

			while (true)
			{
				yield return new WaitForSeconds(holdSettings.executionInterval);
				OnButtonDown();
			}
		}

		private IEnumerator SetCanvas(CanvasSettings setting, CanvasState state) {
			float initial = setting.canvas.alpha;
			float curr = initial;
			float target = state.alpha;
			float timePassed = 0;
			float timeToTarget = state.timeToAlpha;
			if (timeToTarget <= 0)
				timeToTarget = 0.01f;

			if (!setting.shouldBeActive)
			{
				setting.canvas.interactable = setting.shouldBeActive;
				setting.canvas.blocksRaycasts = setting.shouldBeActive;
			}

			yield return new WaitForSeconds(state.delay);

			while (curr != target) {
				curr = Mathf.Lerp(initial, target, timePassed / timeToTarget);
				setting.canvas.alpha = curr;
				timePassed += Time.deltaTime;						
				yield return null;
			}

			setting.canvas.interactable = setting.shouldBeActive;
			setting.canvas.blocksRaycasts = setting.shouldBeActive;

			if (state.usePostValues)
			{
				CanvasState nextState = new CanvasState(state.postDelay, state.postAlpha, state.postTimeToAlpha, false, state.postDelay, state.postAlpha, state.postTimeToAlpha);
				setting.coroutine = SetCanvas(setting, nextState);
				StartCoroutine(setting.coroutine);
			}
		}

		private IEnumerator SetColor(ColorSettings setting, ColorState state)
		{
			Color initial = Color.white;

			if (setting.img != null)
			{
				initial = setting.img.color;
			}

			else if (setting.txt != null)
			{
				initial = setting.txt.color;
			}

			else
			{
				LogWarning("A coloring coroutine is attempting to run changes on an image or text component, but no image or text could be found.\nPlease add an image or text component in the color settings for this button.");
			}

			Color curr = initial;
			Color target = state.color;
			float timePassed = 0;
			float timeToTarget = state.timeToColor;
			if (timeToTarget <= 0)
				timeToTarget = 0.01f;

			yield return new WaitForSeconds(state.delay);

			while (curr != target)
			{
				curr = Color.Lerp(initial, target, timePassed / timeToTarget);

				if (setting.img != null)
				{
					setting.img.color = curr;
				}

				else if (setting.txt != null)
				{
					setting.txt.color = curr;
				}

				else 
				{
					curr = target; //immediately bypass this coroutine
				}

				timePassed += Time.deltaTime;
				yield return null;
			}

			if (state.usePostValues)
			{
				ColorState nextState = new ColorState(state.postDelay, state.postColor, state.postTimeToColor, false, state.postDelay, state.postColor, state.postTimeToColor);
				setting.coroutine = SetColor(setting, nextState);
				StartCoroutine(setting.coroutine);
			}
		}

		private IEnumerator SetFill(FillSettings setting, FillState state)
		{
			float initial = 0;

			if (setting.img != null)
			{
				initial = setting.img.fillAmount;
			}

			else
			{
				LogWarning("A fill coroutine is attempting to run changes on an image component that could not be found.\nPlease add the image component to the fill settings of this button.");
			}

			float curr = initial;
			float target = state.fill;
			float timePassed = 0;
			float timeToTarget = state.timeToFill;
			if (timeToTarget <= 0)
				timeToTarget = 0.01f;

			yield return new WaitForSeconds(state.delay);

			while (curr != target)
			{
				curr = Mathf.Lerp(initial, target, timePassed / timeToTarget);

				if (setting.img != null)
				{
					setting.img.fillAmount = curr;
				}

				else
				{
					curr = target; //immediately bypass this coroutine
				}

				timePassed += Time.deltaTime;
				yield return null;
			}

			if (state.usePostValues)
			{
				FillState nextState = new FillState(state.postDelay, state.postFill, state.postTimeToFill, false, state.postDelay, state.postFill, state.postTimeToFill);
				setting.coroutine = SetFill(setting, nextState);
				StartCoroutine(setting.coroutine);
			}
		}

		private IEnumerator SetSize(SizeSettings setting, VectorState state)
		{
			Vector3 initial = Vector3.zero;

			if (setting.obj != null)
			{
				initial = setting.obj.localScale;
			}

			else
			{
				LogWarning("A scaling coroutine is attempting to modify the transform of an object that does could not be found.\nPlease add a transform to the size settings for this button.");
			}

			Vector3 curr = initial;
			Vector3 target = state.vec;
			float timePassed = 0;
			float timeToTarget = state.timeToVec;
			if (timeToTarget <= 0)
				timeToTarget = 0.01f;

			yield return new WaitForSeconds(state.delay);

			while (curr != target)
			{
				curr = Vector3.Lerp(initial, target, timePassed / timeToTarget);

				if (setting.obj != null)
				{
					setting.obj.localScale = curr;
				}

				else
				{
					curr = target;
				}

				timePassed += Time.deltaTime;
				yield return null;
			}

			if (state.usePostValues)
			{
				VectorState nextState = new VectorState(state.postDelay, state.postVec, state.postTimeToVec, false, state.postDelay, state.postVec, state.postTimeToVec);
				setting.coroutine = SetSize(setting, nextState);
				StartCoroutine(setting.coroutine);
			}
		}

		private IEnumerator SetPosition(PositionSettings setting, VectorState state)
		{
			Vector3 initial = Vector3.zero;

			if (setting.obj != null)
			{
				initial = setting.obj.localPosition;
			}

			else
			{
				LogWarning("A positioning coroutine is attempting to modify the transform of an object that does could not be found.\nPlease add a transform to the position settings for this button.");
			}

			Vector3 curr = initial;
			Vector3 target = state.vec;
			float timePassed = 0;
			float timeToTarget = state.timeToVec;
			if (timeToTarget <= 0)
				timeToTarget = 0.01f;

			yield return new WaitForSeconds(state.delay);

			while (curr != target)
			{
				curr = Vector3.Lerp(initial, target, timePassed / timeToTarget);

				if (setting.obj != null)
				{
					setting.obj.localPosition = curr;
				}

				else
				{
					curr = target;
				}

				timePassed += Time.deltaTime;
				yield return null;
			}

			if (state.usePostValues)
			{
				VectorState nextState = new VectorState(state.postDelay, state.postVec, state.postTimeToVec, false, state.postDelay, state.postVec, state.postTimeToVec);
				setting.coroutine = SetPosition(setting, nextState);
				StartCoroutine(setting.coroutine);
			}
		}

		private IEnumerator SetRotation(RotationSettings setting, VectorState state)
		{
			Quaternion initial = Quaternion.identity;

			if (setting.obj != null)
			{
				initial = setting.obj.localRotation;
			}

			else
			{
				LogWarning("A rotation coroutine is attempting to modify the transform of an object that does could not be found.\nPlease add a transform to the rotation settings for this button.");
			}

			Quaternion curr = initial;
			Quaternion target = Quaternion.Euler(state.vec);
			float timePassed = 0;
			float timeToTarget = state.timeToVec;
			if (timeToTarget <= 0)
				timeToTarget = 0.01f;

			yield return new WaitForSeconds(state.delay);

			while (curr != target)
			{
				curr = Quaternion.Slerp(initial, target, timePassed / timeToTarget);

				if (setting.obj != null)
				{
					setting.obj.localRotation = curr;
				}

				else
				{
					curr = target;
				}

				timePassed += Time.deltaTime;
				yield return null;
			}

			if (state.usePostValues)
			{
				VectorState nextState = new VectorState(state.postDelay, state.postVec, state.postTimeToVec, false, state.postDelay, state.postVec, state.postTimeToVec);
				setting.coroutine = SetRotation(setting, nextState);
				StartCoroutine(setting.coroutine);
			}
		}

		private void HandleButtonStateToActive(StateSettings state)
		{
			//ACTIVE COLOR
			for (int i = 0; i < state.colorSettings.Count; i++)
			{
				if (state.colorSettings[i].coroutine != null)
				{
					StopCoroutine(state.colorSettings[i].coroutine);
				}

				//do nothing if a color component was not found
				if (state.colorSettings[i].img == null && state.colorSettings[i].txt == null)
				{
					LogWarning("Attempting to animate color of text or image component, but a null parameter was found. Please check the color settings of this button.");
					continue;
				}
				
				if (state.colorSettings[i].img != null)
				{
					try { OnColorCoroutineStart(state.colorSettings[i].img); } catch (System.Exception) {}
				}

				else if (state.colorSettings[i].txt != null)
				{
					try { OnTextCoroutineStart(state.colorSettings[i].txt); } catch (System.Exception) {}
				}
				

				state.colorSettings[i].coroutine = SetColor(state.colorSettings[i], state.colorSettings[i].state);
				StartCoroutine(state.colorSettings[i].coroutine);
			}

			//ACTIVE FILL
			for (int i = 0; i < state.fillSettings.Count; i++)
			{
				if (state.fillSettings[i].coroutine != null)
				{
					StopCoroutine(state.fillSettings[i].coroutine);
				}

				//do nothing if a fill image was not found
				if (state.fillSettings[i].img == null)
				{
					LogWarning("Attempting to animate an Image fill, but a null parameter was found. Please check the fill settings of this button.");
					continue;
				}

				try { OnFillCoroutineStart(state.fillSettings[i].img); } catch (System.Exception) {}
				state.fillSettings[i].coroutine = SetFill(state.fillSettings[i], state.fillSettings[i].state);
				StartCoroutine(state.fillSettings[i].coroutine);
			}
			
			//ACTIVE SIZE
			for (int i = 0; i < state.sizeSettings.Count; i++)
			{
				if (state.sizeSettings[i].coroutine != null)
				{
					StopCoroutine(state.sizeSettings[i].coroutine);
				}

				//do nothing if an object was not found
				if (state.sizeSettings[i].obj == null)
				{
					LogWarning("Attempting to animate a Transform, but a null parameter was found. Please check the size settings of this button.");
					continue;
				}

				try { OnSizeCoroutineStart(state.sizeSettings[i].obj); } catch (System.Exception) {}
				state.sizeSettings[i].coroutine = SetSize(state.sizeSettings[i], state.sizeSettings[i].state);
				StartCoroutine(state.sizeSettings[i].coroutine);
			}

			//ACTIVE CANVAS GROUP
			for (int i = 0; i < state.canvasSettings.Count; i++)
			{
				if (state.canvasSettings[i].coroutine != null)
				{
					StopCoroutine(state.canvasSettings[i].coroutine);
				}

				//do nothing if a canvas group was not found
				if (state.canvasSettings[i].canvas == null)
				{
					LogWarning("Attempting to animate a CanvasGroup, but a null parameter was found. Please check the canvas settings of this button.");
					continue;
				}

				try { OnCanvasCoroutineStart(state.canvasSettings[i].canvas); } catch (System.Exception) {}

				state.canvasSettings[i].coroutine = SetCanvas(state.canvasSettings[i], state.canvasSettings[i].state);
				StartCoroutine(state.canvasSettings[i].coroutine);
			}

			//ACTIVE POSITION
			for (int i = 0; i < state.positionSettings.Count; i++)
			{
				if (state.positionSettings[i].coroutine != null)
				{
					StopCoroutine(state.positionSettings[i].coroutine);
				}

				//do nothing if an object was not found
				if (state.positionSettings[i].obj == null)
				{
					LogWarning("Attempting to animate a Transform, but a null parameter was found. Please check the position settings of this button.");
					continue;
				}

				try { OnPositionCoroutineStart(state.positionSettings[i].obj); } catch (System.Exception) {}
				state.positionSettings[i].coroutine = SetPosition(state.positionSettings[i], state.positionSettings[i].state);
				StartCoroutine(state.positionSettings[i].coroutine);
			}

			//ACTIVE ROTATION
			for (int i = 0; i < state.rotationSettings.Count; i++)
			{
				if (state.rotationSettings[i].coroutine != null)
				{
					StopCoroutine(state.rotationSettings[i].coroutine);
				}

				//do nothing if an object was not found
				if (state.rotationSettings[i].obj == null)
				{
					LogWarning("Attempting to animate a Transform, but a null parameter was found. Please check the rotation settings of this button.");
					continue;
				}

				try { OnRotationCoroutineStart(state.rotationSettings[i].obj); } catch (System.Exception) {}
				state.rotationSettings[i].coroutine = SetRotation(state.rotationSettings[i], state.rotationSettings[i].state);
				StartCoroutine(state.rotationSettings[i].coroutine);
			}
		}

		private void StopButtonStateTransition(StateSettings state)
		{
			//STOP COLOR
			for (int i = 0; i < state.colorSettings.Count; i++)
			{
				if (state.colorSettings[i].coroutine != null)
				{
					StopCoroutine(state.colorSettings[i].coroutine);
				}
			}
			
			//STOP SIZE
			for (int i = 0; i < state.sizeSettings.Count; i++)
			{
				if (state.sizeSettings[i].coroutine != null)
				{
					StopCoroutine(state.sizeSettings[i].coroutine);
				}
			}

			//STOP CANVAS GROUP
			for (int i = 0; i < state.canvasSettings.Count; i++)
			{
				if (state.canvasSettings[i].coroutine != null)
				{
					StopCoroutine(state.canvasSettings[i].coroutine);
				}
			}

			//STOP POSITION
			for (int i = 0; i < state.positionSettings.Count; i++)
			{
				if (state.positionSettings[i].coroutine != null)
				{
					StopCoroutine(state.positionSettings[i].coroutine);
				}
			}

			//STOP ROTATION
			for (int i = 0; i < state.rotationSettings.Count; i++)
			{
				if (state.rotationSettings[i].coroutine != null)
				{
					StopCoroutine(state.rotationSettings[i].coroutine);
				}
			}
		}

		private void ApplyRestStateForcibly()
		{
			//REST CANVAS GROUP
			foreach (CanvasSettings setting in restSettings.canvasSettings)
			{
				if (setting.canvas == null) continue;

				setting.canvas.alpha = setting.state.usePostValues ? setting.state.postAlpha : setting.state.alpha;
			}

			//REST COLOR
			foreach (ColorSettings setting in restSettings.colorSettings)
			{
				if (setting.img != null)
				{
					setting.img.color = setting.state.usePostValues ? setting.state.postColor : setting.state.color;
				}

				else if (setting.txt != null)
				{
					setting.txt.color = setting.state.usePostValues ? setting.state.postColor : setting.state.color;
				}
			}

			//REST COLOR
			foreach (FillSettings setting in restSettings.fillSettings)
			{
				if (setting.img == null) continue;

				setting.img.fillAmount = setting.state.usePostValues ? setting.state.postFill : setting.state.fill;
			}

			//REST SIZE
			foreach (SizeSettings setting in restSettings.sizeSettings)
			{
				if (setting.obj == null) continue;

				setting.obj.localScale = setting.state.usePostValues ? setting.state.postVec : setting.state.vec;
			}

			//REST POSITION
			foreach (PositionSettings setting in restSettings.positionSettings)
			{
				if (setting.obj == null) continue;

				setting.obj.localPosition = setting.state.usePostValues ? setting.state.postVec : setting.state.vec;
			}

			//REST ROTATION
			foreach (RotationSettings setting in restSettings.rotationSettings)
			{
				if (setting.obj == null) continue;

				setting.obj.localRotation = setting.state.usePostValues ? Quaternion.Euler(setting.state.postVec) : Quaternion.Euler(setting.state.vec);
			}
		}

		private void CancelCanvasCoroutinesOnState(StateSettings state, CanvasGroup c)
		{
			if (c == null) return;
			
			for (int i = 0; i < state.canvasSettings.Count; i++)
			{
				if (state.canvasSettings[i].canvas == null) continue;
				if (state.canvasSettings[i].canvas.GetInstanceID() != c.GetInstanceID()) continue;

				if (state.canvasSettings[i].coroutine != null)
				{
					StopCoroutine(state.canvasSettings[i].coroutine);
				}
			}
		}

		private void CancelColorCoroutinesOnState(StateSettings state, Image img)
		{
			if (img == null) return;

			for (int i = 0; i < state.colorSettings.Count; i++)
			{
				if (state.colorSettings[i].img == null) continue;
				if (state.colorSettings[i].img.GetInstanceID() != img.GetInstanceID()) continue;

				if (state.colorSettings[i].coroutine != null)
				{
					StopCoroutine(state.colorSettings[i].coroutine);
				}
			}
		}

		private void CancelTextCoroutinesOnState(StateSettings state, Text txt)
		{
			if (txt == null) return;

			for (int i = 0; i < state.colorSettings.Count; i++)
			{
				if (state.colorSettings[i].txt == null) continue;
				if (state.colorSettings[i].txt.GetInstanceID() != txt.GetInstanceID()) continue;

				if (state.colorSettings[i].coroutine != null)
				{
					StopCoroutine(state.colorSettings[i].coroutine);
				}
			}
		}

		private void CancelFillCoroutinesOnState(StateSettings state, Image img)
		{
			if (img == null) return;

			for (int i = 0; i < state.fillSettings.Count; i++)
			{
				if (state.fillSettings[i].img == null) continue;
				if (state.fillSettings[i].img.GetInstanceID() != img.GetInstanceID()) continue;

				if (state.fillSettings[i].coroutine != null)
				{
					StopCoroutine(state.fillSettings[i].coroutine);
				}
			}
		}

		private void CancelSizeCoroutinesOnState(StateSettings state, Transform t)
		{
			if (t == null) return;

			for (int i = 0; i < state.sizeSettings.Count; i++)
			{
				if (state.sizeSettings[i].obj == null) continue;
				if (state.sizeSettings[i].obj.GetInstanceID() != t.GetInstanceID()) continue;

				if (state.sizeSettings[i].coroutine != null)
				{
					StopCoroutine(state.sizeSettings[i].coroutine);
				}
			}
		}

		private void CancelRotationCoroutinesOnState(StateSettings state, Transform t)
		{
			if (t == null) return;

			for (int i = 0; i < state.rotationSettings.Count; i++)
			{
				if (state.rotationSettings[i].obj == null) continue;
				if (state.rotationSettings[i].obj.GetInstanceID() != t.GetInstanceID()) continue;

				if (state.rotationSettings[i].coroutine != null)
				{
					StopCoroutine(state.rotationSettings[i].coroutine);
				}
			}
		}

		private void CancelPositionCoroutinesOnState(StateSettings state, Transform t)
		{
			if (t == null) return;

			for (int i = 0; i < state.positionSettings.Count; i++)
			{
				if (state.positionSettings[i].obj == null) continue;
				if (state.positionSettings[i].obj.GetInstanceID() != t.GetInstanceID()) continue;

				if (state.positionSettings[i].coroutine != null)
				{
					StopCoroutine(state.positionSettings[i].coroutine);
				}
			}
		}

		/// <summary>
		/// This called on button events to cancel coroutines on 'c' when a new button event, 'b'..
		/// ..is using the same canvas 'c'
		/// </summary>
		private void ResolveCanvasCoroutines(CanvasGroup c)
		{
			CancelCanvasCoroutinesOnState(restSettings, c);
			CancelCanvasCoroutinesOnState(hoverSettings, c);
			CancelCanvasCoroutinesOnState(downSettings, c);
			CancelCanvasCoroutinesOnState(upSettings, c);
		}

		/// <summary>
		/// This called on button events to cancel coroutines on 'i' when a new button event, 'b'..
		/// ..is using the same Image 'i'
		/// </summary>
		private void ResolveColorCoroutines(Image i)
		{
			CancelColorCoroutinesOnState(restSettings, i);
			CancelColorCoroutinesOnState(hoverSettings, i);
			CancelColorCoroutinesOnState(downSettings, i);
			CancelColorCoroutinesOnState(upSettings, i);
		}

		/// <summary>
		/// This called on button events to cancel coroutines on 't' when a new button event, 'b'..
		/// ..is using the same Text 't'
		/// </summary>
		private void ResolveTextCoroutines(Text t)
		{
			CancelTextCoroutinesOnState(restSettings, t);
			CancelTextCoroutinesOnState(hoverSettings, t);
			CancelTextCoroutinesOnState(downSettings, t);
			CancelTextCoroutinesOnState(upSettings, t);
		}

		/// <summary>
		/// This called on button events to cancel coroutines on 'i' when a new button event, 'b'..
		/// ..is using the same Image 'i'
		/// </summary>
		private void ResolveFillCoroutines(Image i)
		{
			CancelFillCoroutinesOnState(restSettings, i);
			CancelFillCoroutinesOnState(hoverSettings, i);
			CancelFillCoroutinesOnState(downSettings, i);
			CancelFillCoroutinesOnState(upSettings, i);
		}

		/// <summary>
		/// This called on button events to cancel coroutines on 't' when a new button event, 'b'..
		/// ..is using the same transform 't'
		/// </summary>
		private void ResolveSizeCoroutines(Transform t)
		{
			CancelSizeCoroutinesOnState(restSettings, t);
			CancelSizeCoroutinesOnState(hoverSettings, t);
			CancelSizeCoroutinesOnState(downSettings, t);
			CancelSizeCoroutinesOnState(upSettings, t);
		}

		/// <summary>
		/// This called on button events to cancel coroutines on 't' when a new button event, 'b'..
		/// ..is using the same transform 't'
		/// </summary>
		private void ResolveRotationCoroutines(Transform t)
		{
			CancelRotationCoroutinesOnState(restSettings, t);
			CancelRotationCoroutinesOnState(hoverSettings, t);
			CancelRotationCoroutinesOnState(downSettings, t);
			CancelRotationCoroutinesOnState(upSettings, t);
		}

		/// <summary>
		/// This called on button events to cancel coroutines on 't' when a new button event, 'b'..
		/// ..is using the same transform 't'
		/// </summary>
		private void ResolvePositionCoroutines(Transform t)
		{
			CancelPositionCoroutinesOnState(restSettings, t);
			CancelPositionCoroutinesOnState(hoverSettings, t);
			CancelPositionCoroutinesOnState(downSettings, t);
			CancelPositionCoroutinesOnState(upSettings, t);
		}

		protected void Log(string msg)
		{
			if (!debugThisButton) return;
			Debug.Log("[CLASS=ProButton, OBJECT="+gameObject.name+"] " +msg);
		}
		
		protected void LogWarning(string msg)
		{
			//if (!debugThisButton) return;
			Debug.LogWarning("[CLASS=ProButton, OBJECT="+gameObject.name+"] WARNING=" +msg);
		}

#endregion

#region virtual-functions
		public virtual void Init() 
		{
			m_enabled = true;

			ProButton.OnCanvasCoroutineStart += ResolveCanvasCoroutines;
			ProButton.OnColorCoroutineStart += ResolveColorCoroutines;
			ProButton.OnRotationCoroutineStart += ResolveRotationCoroutines;
			ProButton.OnSizeCoroutineStart += ResolveSizeCoroutines;
			ProButton.OnPositionCoroutineStart += ResolvePositionCoroutines;
			ProButton.OnFillCoroutineStart += ResolveFillCoroutines;
			ProButton.OnTextCoroutineStart += ResolveTextCoroutines;

			ApplyRestStateForcibly();
		}

		public virtual void Dispose()
		{
			//m_enabled = false;

			ProButton.OnCanvasCoroutineStart -= ResolveCanvasCoroutines;
			ProButton.OnColorCoroutineStart -= ResolveColorCoroutines;
			ProButton.OnRotationCoroutineStart -= ResolveRotationCoroutines;
			ProButton.OnSizeCoroutineStart -= ResolveSizeCoroutines;
			ProButton.OnPositionCoroutineStart -= ResolvePositionCoroutines;
			ProButton.OnFillCoroutineStart -= ResolveFillCoroutines;
			ProButton.OnTextCoroutineStart -= ResolveTextCoroutines;

			ApplyRestStateForcibly();
			isInteracting = false;
		}

		public virtual void OnButtonEnter()
		{
			//if the parentButton exists, the parent must be activated before this event is activated
			if (parentButton != null)
			{
				if (!parentButton.isInteracting) return;
			}

			if (isInteracting) return;

			Log("EVENT=Button Enter");	
			isInteracting = true;

			HandleButtonStateToActive(hoverSettings);
			hoverSettings.eventAction.Invoke();
		}

		public virtual void OnButtonExit()
		{
			if (!isInteracting) return;

			Log("EVENT=Button Exit");
			isInteracting = false;

			StopCoroutine("Hold");

			HandleButtonStateToActive(restSettings);
			restSettings.eventAction.Invoke();
		}

		public virtual void OnButtonUp()
		{
			Log("EVENT=Button Up");

			StopCoroutine("Hold");

			HandleButtonStateToActive(upSettings);
			upSettings.eventAction.Invoke();
		}

		public virtual void OnButtonDown()
		{

			Log("EVENT=Button Down");

			HandleButtonStateToActive(downSettings);
			downSettings.eventAction.Invoke();
		}

		public virtual void OnButtonHold()
		{
			if (!isInteracting) return;
			if (!m_hovering) return;
			if (!m_enabled) return;

			StartCoroutine("Hold");
		}

#endregion


#region unity-events

		public void OnPointerEnter(PointerEventData ped)
		{
			m_hovering = true;
			if (!m_enabled) return;
			OnButtonEnter();
		}

		public void OnPointerExit(PointerEventData ped)
		{
			m_hovering = false;
			OnButtonExit();
		}

		public void OnPointerUp(PointerEventData ped)
		{
			if (!isInteracting) return;
			if (!m_hovering) return;
			if (!m_enabled) return;

			OnButtonUp();
		}

		public void OnPointerDown(PointerEventData ped)
		{
			if (!isInteracting) return;
			if (!m_enabled) return;

			if (holdSettings.enabled)
			{
				OnButtonDown();
				//OnButtonHold();
			}

			else
			{
				OnButtonDown();
			}
		}


#endregion


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

		//[Header("Group Options")]
		[Tooltip("Does this event depend on another being activated first?")]
		public ProButton parentButton;

		//[Header("Debug")]
		public bool debugThisButton;

		public bool	isInteracting 
		{
			get; 
			private set;
		}

		private bool m_initOnEnable = false;
		private bool m_enabled = false;
		private bool m_hovering = false;
		private bool m_dragging = false;

		private delegate void CanvasCoroutineDelegate(CanvasGroup c);
		private static event CanvasCoroutineDelegate OnCanvasCoroutineStart;

		private delegate void ColorCoroutineDelegate(Image i);
		private static event ColorCoroutineDelegate OnColorCoroutineStart;

		private delegate void TextCoroutineDelegate(Text txt);
		private static event TextCoroutineDelegate OnTextCoroutineStart;

		private delegate void VectorCoroutineDelegate(Transform t);
		private static event VectorCoroutineDelegate OnSizeCoroutineStart;
		private static event VectorCoroutineDelegate OnRotationCoroutineStart;
		private static event VectorCoroutineDelegate OnPositionCoroutineStart;

		private delegate void FillCoroutineDelegate(Image i);
		private static event FillCoroutineDelegate OnFillCoroutineStart;

#endregion


	}
}

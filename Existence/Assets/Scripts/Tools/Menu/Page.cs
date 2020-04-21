using System.Collections;
using UnityEngine;

namespace UnityCore {
    
    namespace Menu {

        public class Page : MonoBehaviour
        {
            
            public static readonly string FLAG_ON = "On";
            public static readonly string FLAG_OFF = "Off";
            public static readonly string FLAG_NONE = "None";

            public PageType type;
            public bool useAnimation;
            public string targetState {get;private set;}

            /*
             * Animaton Requirements...
             *  - This class uses certain controls to determine page state
             *  - Pages have three core states:
             *      1. Resting
             *      2. Turning On
             *      3. Turning Off
             *  - The animator must have a control boolean called 'on'. Otherwise the animator will not work.
             */
            private Animator m_Animator;
            private bool m_IsOn;

            public bool isOn {
                get {
                    return m_IsOn;
                }
                private set {
                    m_IsOn = value;
                }
            }

#region Unity Functions
            private void OnEnable() {
                CheckAnimatorIntegrity();
                OnPageEnabled();
            }

            private void OnDisable() {
                OnPageDisabled();
            }
#endregion

#region Public Functions
            /// <summary>
            /// Call this to turn the page on or off by setting the control '_on'
            /// </summary>
            public void Animate(bool _on) {
                if (useAnimation) {
                    m_Animator.SetBool("on", _on);

                    StopCoroutine("AwaitAnimation");
                    StartCoroutine("AwaitAnimation", _on);
                } else {
                    if (!_on) {
                        OnPageExit();
                    } else {
                        OnPageEnter();
                    }
                }
            }
#endregion

#region Private Functions
            private IEnumerator AwaitAnimation(bool _on) {
                targetState = _on ? FLAG_ON : FLAG_OFF;

                while (!m_Animator.GetCurrentAnimatorStateInfo(0).IsName(targetState)) {
                    yield return null;
                }
                while (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) {
                    yield return null;
                }

                targetState = FLAG_NONE;

                Log("Page ["+type+"] finished transitioning to "+(_on ? "<color=#0f0>on</color>." : "<color=#f00>off</color>."));

                if (!_on) {
                    OnPageExit();
                } else {
                    OnPageEnter();
                }
            }

            private void CheckAnimatorIntegrity() {
                if (useAnimation) {
                    // try to get animator
                    m_Animator = GetComponent<Animator>();
                    if (!m_Animator) {
                        LogWarning("You opted to animate page ["+type+"], but no Animator component exists on the object.");
                    }
                }
            }

            protected void Log(string _msg) {
                Debug.Log("[Page]: "+_msg);
            }

            protected void LogWarning(string _msg) {
                Debug.LogWarning("[Page]: "+_msg);
            }

            protected void LogError(string _msg) {
                Debug.LogError("[Page]: "+_msg);
            }
#endregion

#region Override Functions
            protected virtual void OnPageEnter() {
                isOn = true;
            }

            protected virtual void OnPageExit() {
                isOn = false;
                gameObject.SetActive(false);
            }

            protected virtual void OnPageEnabled() {}
            protected virtual void OnPageDisabled() {}
#endregion
        }
    }
}
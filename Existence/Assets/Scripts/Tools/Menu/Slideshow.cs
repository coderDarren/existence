using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Slideshow : MonoBehaviour
{
    public CanvasGroup[] canvases;
    public float slideSpeed;
    public float fadeSpeed;
    public bool loop;
    public bool automatic;
    public bool openOnEnable=true;
    public bool async=true;
    public float automaticDuration = 5;

    private int m_Index;
    private CanvasGroup m_CurrentCanvas;
    private CanvasGroup m_TargetCanvas;
    private bool m_ValidSlideshow;
    private IEnumerator m_Job;
    private bool m_JobIsRunning;

    public int progress {get;private set;}
    public int stage {get{return m_Index;}}

#region Unity Functions
    private void OnEnable() {
        m_ValidSlideshow = canvases.Length > 0;

        if (m_ValidSlideshow) {
            m_Index = 0;
            if (openOnEnable) {
                ApplyPage(-1);
            }
            if (automatic) {
                StartCoroutine("RunAutomatic");
            }
        }
    }

    private void OnDisable() {
        StopJob();
        StopCoroutine("RunAutomatic");
    }
#endregion

#region Public Functions
    public void GoToNextPage() {
        m_Index++;
        if (m_Index > canvases.Length - 1) {
            if (loop)
                m_Index = 0;
            else {
                m_Index--;
                return;
            }
        }
        ApplyPage(-1);
    }

    public void GoToPreviousPage() {
        m_Index--;
        if (m_Index < 0) {
            if (loop)
                m_Index = canvases.Length - 1;
            else {
                m_Index++;
                return;
            }
        }
        ApplyPage(1);
    }

    public void GoToPage(int _index, int _dir) {
        m_Index = Mathf.Clamp(_index, 0, canvases.Length - 1);
        ApplyPage(_dir);
    }
#endregion

#region Private Functions
    private void StopJob() {
        if (m_Job != null) {
            StopCoroutine(m_Job);
            if (m_TargetCanvas && m_JobIsRunning) {
                m_TargetCanvas.interactable = false;
                m_TargetCanvas.blocksRaycasts = false;
                m_TargetCanvas.alpha = 0;
            }
        }
    }

    private void ApplyPage(int _dir) {
        progress = (int)((m_Index / (float)(canvases.Length - 1)) * 100);
        StopJob();
        m_TargetCanvas = canvases[m_Index];
        m_Job = async ? GoToAsync(_dir) : GoTo(_dir);
        StartCoroutine(m_Job);
    }

    private IEnumerator RunAutomatic() {
        while (true) {
            yield return new WaitForSeconds(automaticDuration);
            GoToNextPage();
        }
    }

    private IEnumerator GoTo(int _dir) {
        m_JobIsRunning = true;
        float _timer = 0;
        float _initX = 0;
        float _targetX = 0;
        float _initAlpha = 0;
        float _targetAlpha = 0;
        float _dist = 0;

        if (m_CurrentCanvas) {
            m_CurrentCanvas.interactable = false;
            m_CurrentCanvas.blocksRaycasts = false;
            _initX = m_CurrentCanvas.transform.localPosition.x;
            _targetX = _dir * _dist;
            _initAlpha = m_CurrentCanvas.alpha;
            _targetAlpha = 0;

            while (_timer <= (slideSpeed > fadeSpeed ? slideSpeed : fadeSpeed)) {
                _timer += Time.deltaTime;
                Vector3 _pos = m_CurrentCanvas.transform.localPosition;
                _pos.x = Mathf.Lerp(_initX, _targetX, _timer / slideSpeed);
                m_CurrentCanvas.transform.localPosition = _pos;
                m_CurrentCanvas.alpha = Mathf.Lerp(_initAlpha, _targetAlpha, _timer / fadeSpeed);
                yield return null;
            }
            m_CurrentCanvas.alpha = 0;
        }

        _initX = _dir * _dist * -1;
        _targetX = 0;
        _initAlpha = m_TargetCanvas.alpha;
        _targetAlpha = 1;
        _timer = 0;

        while (_timer <= (slideSpeed > fadeSpeed ? slideSpeed : fadeSpeed)) {
            _timer += Time.deltaTime;
            Vector3 _pos = m_TargetCanvas.transform.localPosition;
            _pos.x = Mathf.Lerp(_initX, _targetX, _timer / slideSpeed);
            m_TargetCanvas.transform.localPosition = _pos;
            m_TargetCanvas.alpha = Mathf.Lerp(_initAlpha, _targetAlpha, _timer / fadeSpeed);
            yield return null;
        }
        m_TargetCanvas.alpha = 1;

        m_CurrentCanvas = m_TargetCanvas;
        m_CurrentCanvas.interactable = true;
        m_CurrentCanvas.blocksRaycasts = true;
        m_JobIsRunning = false;
    }

    private IEnumerator GoToAsync(int _dir) {
        m_JobIsRunning = true;
        float _timer = 0;
        float _initXOut = 0;
        float _targetXOut = 0;
        float _initAlphaOut = 0;
        float _targetAlphaOut = 0;
        float _initXIn = 0;
        float _targetXIn = 0;
        float _initAlphaIn = 0;
        float _targetAlphaIn = 0;
        float _dist = 25;

        if (m_CurrentCanvas) {
            m_CurrentCanvas.interactable = false;
            m_CurrentCanvas.blocksRaycasts = false;
            _initXOut = m_CurrentCanvas.transform.localPosition.x;
            _targetXOut = _dir * _dist;
            _initAlphaOut = m_CurrentCanvas.alpha;
            _targetAlphaOut = 0;
        }

        _initXIn = _dir * _dist * -1;
        _targetXIn = 0;
        _initAlphaIn = m_TargetCanvas.alpha;
        _targetAlphaIn = 1;

        while (_timer <= (slideSpeed > fadeSpeed ? slideSpeed : fadeSpeed)) {
            _timer += Time.deltaTime;

            if (m_CurrentCanvas) {
                Vector3 _posOut = m_CurrentCanvas.transform.localPosition;
                _posOut.x = Mathf.Lerp(_initXOut, _targetXOut, _timer / slideSpeed);
                m_CurrentCanvas.transform.localPosition = _posOut;
                m_CurrentCanvas.alpha = Mathf.Lerp(_initAlphaOut, _targetAlphaOut, _timer / fadeSpeed);
            }

            Vector3 _posIn = m_TargetCanvas.transform.localPosition;
            _posIn.x = Mathf.Lerp(_initXIn, _targetXIn, _timer / slideSpeed);
            m_TargetCanvas.transform.localPosition = _posIn;
            m_TargetCanvas.alpha = Mathf.Lerp(_initAlphaIn, _targetAlphaIn, _timer / fadeSpeed);

            yield return null;
        }
        
        if (m_CurrentCanvas) {
            m_CurrentCanvas.alpha = 0;
        }

        m_CurrentCanvas = m_TargetCanvas;
        m_CurrentCanvas.interactable = true;
        m_CurrentCanvas.blocksRaycasts = true;
        m_JobIsRunning = false;
    }
#endregion 
}

﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Menu;
using Tween;

/*
 * This page listens for new nearby loot previews on dead mobs.
 * Responsible for managing loot previews with LootPreviewMobSections
 */ 
public class LootPreviewPage : Page
{
    public GameObject LootPreview;
    public RectTransform previewContainer;
    public UIContainer masterContainer;
    public Image header;

    private Session m_Session;
    private Hashtable m_Previews; // mob to gameobject

    // get Session with integrity
    private Session session {
        get {
            if (!m_Session) {
                m_Session = Session.instance;
            }
            if (!m_Session) {
                LogError("Trying to use Session, but no instance could be found.");
            }
            return m_Session;
        }
    }

#region Private Functions
    private void SetContainerHeight(float _height) {
        Vector2 _size = previewContainer.sizeDelta;
        _size.y = _height;
        previewContainer.sizeDelta = _size;
    }

    private IEnumerator FlashNotification() {
        ImageColorTween _tweener = header.GetComponent<ImageColorTween>();
        _tweener.StartTween();
        yield return new WaitForSeconds(2);
        _tweener.StopTween();

        float _timer = 0;
        float _smooth = 0.25f;
        Color _initial = header.color;
        Color _target = _tweener.keys[0].value;

        while (_timer <= _smooth) {
            header.color = Color.Lerp(_initial, _target, _timer / _smooth);
            _timer += Time.deltaTime;
            yield return null;
        }

        header.color = _target;
    }

    private void RestartFlashNotification() {
        //header.GetComponent<ImageColorTween>().StopTween();
        StopCoroutine("FlashNotification");
        StartCoroutine("FlashNotification");
    }
#endregion

#region Public Functions
    public void AddLoot(Mob _mob) {
        if (m_Previews.ContainsKey(_mob)) return;
        GameObject _obj = Instantiate(LootPreview);
        RectTransform _rect = _obj.GetComponent<RectTransform>();
        _rect.SetParent(previewContainer);
        _rect.localScale = Vector3.one;
        LootPreviewMobSection _section = _obj.GetComponent<LootPreviewMobSection>();
        _section.Init(masterContainer, _mob);
        m_Previews.Add(_mob, _obj);
        SetContainerHeight(m_Previews.Count * 100 + m_Previews.Count * 8);
        RestartFlashNotification();
    }

    public void RemoveLoot(Mob _mob) {
        if (!m_Previews.ContainsKey(_mob)) return;
        GameObject _obj = (GameObject)m_Previews[_mob];
        m_Previews.Remove(_mob);
        Destroy(_obj);
        SetContainerHeight(m_Previews.Count * 100 + m_Previews.Count * 8);
    }
#endregion

#region Override Functions
    protected override void OnPageEnabled() {
        base.OnPageEnabled();
        if (!session) return;
        m_Previews = new Hashtable();
    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();
        StopCoroutine("FlashNotification");
    }
#endregion
}

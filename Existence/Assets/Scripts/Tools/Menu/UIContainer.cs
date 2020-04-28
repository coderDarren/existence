
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIContainer is responsible for performing operations on the container.
/// Operations include
///     1. Resize events initiated by UIHandle
///     2. Drag events initiated by UIHandle
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class UIContainer : GameSystem
{
    [System.Serializable]
    public struct FloatThreshold {
        public float min;
        public float max;
    }

    [System.Serializable]
    public struct Constraints {
        public FloatThreshold width;
        public FloatThreshold height;
    }

    public int uniqueId;
    public bool draggable;
    public bool resizable;
    public Constraints constraints;

    [Header("Handles")]
    public UIHandle dragger;
    public UIHandle topLeftResizer;
    public UIHandle topRightResizer;
    public UIHandle bottomLeftResizer;
    public UIHandle bottomRightResizer;
    public UIHandle topResizer;
    public UIHandle leftResizer;
    public UIHandle bottomResizer;
    public UIHandle rightResizer;

    private RectTransform m_Rect;

    public RectTransform rect {
        get {
            if (!m_Rect) {
                m_Rect = GetComponent<RectTransform>();
            }
            return m_Rect;
        }
    }

#region Unity Functions
    private void Awake() {
        // Configure handles
        ConfigureHandle(dragger, UIHandle.HandleLoc.IRRELEVANT, uniqueId);
        ConfigureHandle(topLeftResizer, UIHandle.HandleLoc.TOP_LEFT, uniqueId);
        ConfigureHandle(topRightResizer, UIHandle.HandleLoc.TOP_RIGHT, uniqueId);
        ConfigureHandle(bottomLeftResizer, UIHandle.HandleLoc.BOTTOM_LEFT, uniqueId);
        ConfigureHandle(bottomRightResizer, UIHandle.HandleLoc.BOTTOM_RIGHT, uniqueId);
        ConfigureHandle(topResizer, UIHandle.HandleLoc.TOP, uniqueId);
        ConfigureHandle(leftResizer, UIHandle.HandleLoc.LEFT, uniqueId);
        ConfigureHandle(bottomResizer, UIHandle.HandleLoc.BOTTOM, uniqueId);
        ConfigureHandle(rightResizer, UIHandle.HandleLoc.RIGHT, uniqueId);
    }
#endregion

#region Public Functions
    public void Drag(Vector2 _pos) {
        if (!draggable) return;
        float _maxWidth = Screen.width;
        float _maxHeight = Screen.height;
        float _rectWidth = rect.rect.width;
        float _rectHeight = rect.rect.height;
        Debug.Log("rect width: "+_rectWidth);
        Debug.Log("screen width: "+_maxWidth);

        if (_pos.x < 0) _pos.x = 0;
        if (_pos.x > _maxWidth - _rectWidth) _pos.x = _maxWidth - _rectWidth;
        if (_pos.y < 0) _pos.y = 0;
        if (_pos.y > _maxHeight - _rectHeight) _pos.y = _maxHeight - _rectHeight;
        
        rect.transform.position = _pos;
    }

    public void Resize(Vector2 _pos, UIHandle.HandleLoc _loc) {
        if (!resizable) return;
        switch (_loc) {
            case UIHandle.HandleLoc.TOP_LEFT: ResizeTopLeft(_pos); break;
            case UIHandle.HandleLoc.TOP_RIGHT: ResizeTopRight(_pos); break;
            case UIHandle.HandleLoc.BOTTOM_LEFT: ResizeBottomLeft(_pos); break;
            case UIHandle.HandleLoc.BOTTOM_RIGHT: ResizeBottomRight(_pos); break;
            case UIHandle.HandleLoc.TOP: ResizeTop(_pos); break;
            case UIHandle.HandleLoc.LEFT: ResizeLeft(_pos); break;
            case UIHandle.HandleLoc.BOTTOM: ResizeBottom(_pos); break;
            case UIHandle.HandleLoc.RIGHT: ResizeRight(_pos); break;
        }
    }
#endregion

#region Private Functions
    private void ResizeTopLeft(Vector2 _pos) {
        ResizeTop(_pos);
        ResizeLeft(_pos);
    }
    
    private void ResizeTopRight(Vector2 _pos) {
        ResizeTop(_pos);
        ResizeRight(_pos);
    }
    
    private void ResizeBottomLeft(Vector2 _pos) {
        ResizeBottom(_pos);
        ResizeLeft(_pos);
    }
    
    private void ResizeBottomRight(Vector2 _pos) {
        ResizeBottom(_pos);
        ResizeRight(_pos);
    }
    
    private void ResizeTop(Vector2 _pos) {
        float _width = rect.sizeDelta.x;
        float _height = rect.sizeDelta.y;
        Vector2 _bottomLeft = rect.transform.position;
        Vector2 _topRight = new Vector2(_bottomLeft.x + _width, _bottomLeft.y + _height);
        if (FloatIsBetween(_pos.y, _bottomLeft.y + constraints.height.min, _bottomLeft.y + constraints.height.max)) {
            SetSize(-1, _pos.y - _bottomLeft.y);
        } else {
            if (_pos.y < _bottomLeft.y + constraints.height.min) {
                SetSize(-1, constraints.height.min);
            } else if (_pos.y > _bottomLeft.y + constraints.height.max) {
                SetSize(-1, constraints.height.max);
            } 
        }
    }
    
    private Vector2 _stopPos;
    private bool _constraintHit;
    private void ResizeLeft(Vector2 _pos) {
        float _width = rect.sizeDelta.x;
        float _height = rect.sizeDelta.y;
        Vector2 _bottomLeft = rect.transform.position;
        Vector2 _topRight = new Vector2(_bottomLeft.x + _width, _bottomLeft.y + _height);
        if (FloatIsBetween(_pos.x, _topRight.x - constraints.width.max, _topRight.x - constraints.width.min)) {
            SetSize(_topRight.x - _pos.x, -1);
            SetPos(_pos.x, rect.transform.position.y);
        } else {
            if (_pos.x > _topRight.x - constraints.width.min) {
                SetSize(constraints.width.min, -1);
                SetPos(_topRight.x - constraints.width.min, rect.transform.position.y);
            } else if (_pos.x < _topRight.x - constraints.width.max) {
                SetSize(constraints.width.max, -1);
                SetPos(_topRight.x - constraints.width.max, rect.transform.position.y);
            } 
        }
    }
    
    private void ResizeBottom(Vector2 _pos) {
        float _width = rect.sizeDelta.x;
        float _height = rect.sizeDelta.y;
        Vector2 _bottomLeft = rect.transform.position;
        Vector2 _topRight = new Vector2(_bottomLeft.x + _width, _bottomLeft.y + _height);
        if (FloatIsBetween(_pos.y, _topRight.y - constraints.height.max, _topRight.y - constraints.height.min)) {
            SetSize(-1, _topRight.y - _pos.y);
            SetPos(rect.transform.position.x, _pos.y);
        } else {
            if (_pos.y > _topRight.y - constraints.height.min) {
                SetSize(-1, constraints.height.min);
                SetPos(rect.transform.position.x, _topRight.y - constraints.height.min);
            } else if (_pos.y < _topRight.y - constraints.height.max) {
                SetSize(-1, constraints.height.max);
                SetPos(rect.transform.position.x, _topRight.y - constraints.height.max);
            } 
        }
    }
    
    private void ResizeRight(Vector2 _pos) {
        float _width = rect.sizeDelta.x;
        float _height = rect.sizeDelta.y;
        Vector2 _bottomLeft = rect.transform.position;
        Vector2 _topRight = new Vector2(_bottomLeft.x + _width, _bottomLeft.y + _height);
        if (FloatIsBetween(_pos.x, _bottomLeft.x + constraints.width.min, _bottomLeft.x + constraints.width.max)) {
            SetSize(_pos.x - _bottomLeft.x, -1);
        } else {
            if (_pos.x > _bottomLeft.x + constraints.width.max) {
                SetSize(constraints.width.max, -1);
            } else if (_pos.x < _bottomLeft.x + constraints.width.min) {
                SetSize(constraints.width.min, -1);
            } 
        }
    }

    private void SetSize(float _x, float _y) {
        Vector2 _size = rect.sizeDelta;
        if (_x >= 0)
            _size.x = _x;
        if (_y >= 0)
            _size.y = _y;
        rect.sizeDelta = _size;
    }

    private void SetPos(float _x, float _y) {
        Vector2 _pos = rect.transform.position;
        _pos.x = _x;
        _pos.y = _y;
        rect.transform.position = _pos;
    }

    private bool FloatIsBetween(float _val, float _min, float _max) {
        return _val >= _min && _val <= _max;
    }

    private void ConfigureHandle(UIHandle _handle, UIHandle.HandleLoc _loc, int _uniqueId) {
        if (_handle) {
            _handle.Configure(this, _loc, _uniqueId);
        }
    }
#endregion
}

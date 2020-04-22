
using UnityEngine;

namespace Tween {

    [System.Serializable]
    public class Keyframe<T>
    {
        public T value;
        [Range(0,1)]
        public float nTime;
        public Keyframe(T _value, float _nTime) {
            value = _value;
            nTime = _nTime;
        }
    }

    [System.Serializable]
    public class Vec3Keyframe
    {
        public Vector3 value;
        [Range(0,1)]
        public float nTime;
    }
    
    [System.Serializable]
    public class FloatKeyframe
    {
        public float value;
        [Range(0,1)]
        public float nTime;
    }
    
    [System.Serializable]
    public class ColorKeyframe
    {
        public Color value;
        [Range(0,1)]
        public float nTime;
    }
}

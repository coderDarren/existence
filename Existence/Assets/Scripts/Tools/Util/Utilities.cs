
using UnityEngine;
using Tools.IO;

public class Utilities
{
#region Public Static Functions
    public static void FadeMaterialTo(Material mat, float alpha)
    {
        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);
    }

    public static void SetMaterialBlendMode(Material material, string BLEND_MODE)
    {
        switch (BLEND_MODE)
        {
            case "Opaque":
                material.SetFloat("_Mode", 0);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case "Fade":
                material.SetFloat("_Mode", 2); //set to fade mode
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
        }
    }

    public static Sprite LoadStreamingAssetsSprite(string _path) {
        string _fullPath = Application.streamingAssetsPath + _path;
        if (!Files.FileExists(_fullPath)) {
            Debug.Log("Trying to load sprite from streaming assets path ["+_path+"], but could not find file.");
            return null;
        }

        byte[] _data = Files.GetBytesFromFile(_fullPath);
        Texture2D _tex = new Texture2D(2, 2);
        _tex.LoadImage(_data);
        Sprite _ret = Sprite.Create(_tex, new Rect(0,0,_tex.width, _tex.height), Vector2.one*0.5f, 100);
        return _ret;
    }

    [System.Serializable]
    public class RectBounds {
        public int minX;
        public int maxX;
        public int minY;
        public int maxY;
    }
    public static Texture2D InsertTextureIntoTextureBounds(Texture2D _insertTex, Texture2D _intoTex, RectBounds _bounds) {
        for (int y = _bounds.minY; y <= _bounds.maxY; y++) {
            for (int x = _bounds.minX; x <= _bounds.maxX; x++) {
                Color _c = _insertTex.GetPixel(x - _bounds.maxX, y - _bounds.maxY);
                _intoTex.SetPixel(_intoTex.width - x, _intoTex.height - y, _c);
            }
        }
        _intoTex.Apply();
        return _intoTex;
    }

    public static float ClampAngle(float _a, float _min, float _max) 
    {
        while (_max < _min) _max += 360.0f;
        while (_a > _max) _a -= 360.0f;
        while (_a < _min) _a += 360.0f;
        
        if (_a > _max)
            {
            if (_a - (_max + _min) * 0.5f < 180.0f)
                return _max;
            else
                return _min;
            }
        else
            return _a;
    }
#endregion
}

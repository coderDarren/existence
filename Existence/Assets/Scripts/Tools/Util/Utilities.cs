
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
#endregion
}

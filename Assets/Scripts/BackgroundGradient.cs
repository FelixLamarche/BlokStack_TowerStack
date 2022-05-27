using UnityEngine;

public class BackgroundGradient : MonoBehaviour
{
    [SerializeField]
    Color topColor;

    [SerializeField]
    Color bottomColor;

    [SerializeField]
    MeshRenderer meshRenderer;

    void Awake()
    {
        ChangeColor(topColor, bottomColor);
    }
    
    void OnValidate()
    {
        ChangeColor(topColor, bottomColor);
    }

    public void ScaleToFitOrthographicCameraView(float orthographicSize)
    {
        transform.localScale = new Vector3(orthographicSize * 2.5f / 5f, 1, orthographicSize / 5f);
    }

    void ChangeColor(Color topColor, Color bottomColor)
    {
        Texture2D texture = (Texture2D) meshRenderer.sharedMaterial.mainTexture;
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        for(int i = 0; i < texture.height; i++)
        {
            float t = ((float) i ) / texture.height;
            Color lerped = Color.Lerp(topColor, bottomColor, t);
            texture.SetPixel(0, i, lerped);
        }

        texture.Apply();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }

}

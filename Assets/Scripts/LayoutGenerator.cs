using UnityEngine;

public class LayoutGenerator : MonoBehaviour
{
    public MeshRenderer previewRenderer;
    private void Start()
    {
        Generate();    
    }
    private void Generate()
    {
        int textureWidth = 256;
        int textureHeight = 256;
        Texture2D texture = new Texture2D(textureWidth, textureHeight);
        texture.filterMode = FilterMode.Point;
        // texture.SetPixel(100, 100, Color.red);
        // texture.Apply();
        previewRenderer.material.mainTexture = texture;

    }
}

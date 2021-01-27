using UnityEngine;

public class LayoutGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct LayoutParameters
    {
        public int width;
        public int height;
    }
    
    [System.Serializable]
    public struct RoomParameters
    {
        public int minWidth;
        public int maxWidth;
        public int minHeight;
        public int maxHeight;
        public int minQuantity;
        public int maxQuantity;
    }

    public LayoutParameters layoutParameters;
    public RoomParameters roomParameters;
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

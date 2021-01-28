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

    public void Generate()
    {
        LayoutStructure layout = SetupLayout();
        GenerateRooms(layout);
        ConverToBitmap(layout);
    }

    private LayoutStructure SetupLayout()
    {
        LayoutStructure layout = new LayoutStructure(this.layoutParameters.width, this.layoutParameters.height); 
        return layout; 
    }

    private void GenerateRooms(LayoutStructure layout)
    {
        int quantity = Random.Range(roomParameters.minQuantity, roomParameters.maxQuantity + 1);

        for(int i = 0; i < quantity; i++)
        {
            int width = Random.Range(roomParameters.minWidth, roomParameters.maxWidth + 1);
            int height = Random.Range(roomParameters.minHeight, roomParameters.maxHeight + 1);
            float x = Random.Range(layout.area.min.x, layout.area.max.x);
            float z = Random.Range(layout.area.min.z, layout.area.max.z);
            Vector3 size = new Vector3(width, 0, height);
            Vector3 position = new Vector3(x, 0, z);
            layout.AddRoom(position, size);
    }
    }

    private void ConverToBitmap(LayoutStructure layout)
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

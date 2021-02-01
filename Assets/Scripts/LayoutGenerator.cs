using UnityEngine;

public class LayoutGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct LayoutParameters
    {
        [Range(1,8192)]
        public int width;
        [Range(1,8192)]
        public int height;
    }
    
    [System.Serializable]
    public struct RoomParameters
    {
        public float wallThickness;
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
        ConvertToBitmap(layout);
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

    private void ConvertToBitmap(LayoutStructure layout)
    {
        int textureWidth = layoutParameters.width;
        int textureHeight = layoutParameters.height;
        float wallThickness = roomParameters.wallThickness;
        Texture2D texture = new Texture2D(textureWidth, textureHeight);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        for(int i = 0; i < layout.rooms.Count; i++)
        {
            LayoutStructure.Room room = layout.rooms[i];
            Bounds innerBounds = new Bounds(room.bounds.center ,room.bounds.size);
            innerBounds.Expand(new Vector3(wallThickness * -2.0f, 0, wallThickness * -2.0f));
            
            for(float pointX = room.bounds.min.x; pointX <= room.bounds.max.x; pointX += 1.0f)
            {
                for(float pointZ = room.bounds.min.z; pointZ <= room.bounds.max.z; pointZ += 1.0f)
                {
                    if(!innerBounds.Contains(new Vector3(pointX, 0, pointZ)))
                    {
                        int x = Mathf.CeilToInt(pointX + (textureWidth/2));
                        int y = Mathf.CeilToInt(pointZ + (textureHeight/2));
                        texture.SetPixel(x, y, Color.red);
                    }
                }
            }

            texture.SetPixel(Mathf.CeilToInt(room.anchorPoint.x + (textureWidth/2)), Mathf.CeilToInt(room.anchorPoint.z + (textureHeight/2)), Color.blue);
        }
        texture.Apply();
        previewRenderer.material.mainTexture = texture;

    }
}

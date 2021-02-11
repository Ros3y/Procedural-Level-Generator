using UnityEngine;
public class LayoutOutput2DBitmapPreview : LayoutOutputPreview
{
    public override void Preview(LayoutStructure layout, Parameters parameters)
    {
        Bounds totalBounds = layout.CalculateTotalBounds();
        int textureSize = Mathf.CeilToInt(Mathf.Max(totalBounds.size.x, totalBounds.size.z));
        Texture2D texture = new Texture2D(textureSize, textureSize);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        
        for(int i = 0; i < layout.rooms.Count; i++)
        {
            LayoutStructure.Room room = layout.rooms[i];
            Bounds innerBounds = new Bounds(room.bounds.center ,room.bounds.size);
            innerBounds.Expand(new Vector3(parameters.wallThickness * -2.0f, 0, parameters.wallThickness * -2.0f));
            
            for(float pointX = room.bounds.min.x; pointX <= room.bounds.max.x; pointX += 1.0f)
            {
                for(float pointZ = room.bounds.min.z; pointZ <= room.bounds.max.z; pointZ += 1.0f)
                {
                    if(!innerBounds.Contains(new Vector3(pointX, 0, pointZ)))
                    {
                        int x = Convert3DPointTo2DPixel(pointX, totalBounds.center.x, textureSize);
                        int y = Convert3DPointTo2DPixel(pointZ, totalBounds.center.z, textureSize);
                        texture.SetPixel(x, y, Color.red);
                    }
                }
            }

            
            int centerX = Convert3DPointTo2DPixel(room.anchorPoint.x, totalBounds.center.x, textureSize);
            int centerY = Convert3DPointTo2DPixel(room.anchorPoint.z, totalBounds.center.z, textureSize);
            texture.SetPixel(centerX, centerY, Color.blue);
        }
        texture.Apply();
        parameters.previewRenderer.material.mainTexture = texture;
    }
    private int Convert3DPointTo2DPixel(float point, float centerOffset, int textureSize)
    {
        return Mathf.CeilToInt(point - centerOffset + (textureSize/2));
    }
}

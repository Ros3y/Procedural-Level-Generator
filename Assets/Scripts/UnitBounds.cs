using UnityEngine;

[System.Serializable]
public struct UnitBounds
{
    public Vector2Int position;
    public Vector2Int size;

    public Vector2Int center => new Vector2Int(
        position.x + (size.x / 2),
        position.y + (size.y / 2));

    public UnitBounds(Vector2Int position, Vector2Int size)
    {
        this.position = position;
        this.size = size;
    }

    public UnitBounds(int x, int y, int width, int height)
    {
        this.position = new Vector2Int(x, y);
        this.size = new Vector2Int(width, height);
    }

}

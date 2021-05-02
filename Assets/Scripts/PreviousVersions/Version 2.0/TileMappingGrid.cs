using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using Zigurous.DataStructures;


public class TileMappingGrid : MonoBehaviour
{
    private TileMappingParametersOLD.Parameters parameters;
    private TileBase baseTile;
    private TileMappingGrid grid;
    private Tilemap _tilemap;
    private TileMappingUnit unit;
    private int unitsX;
    private int unitsY;
    private bool[,] unitAvailability;
    private BoundsInt[,] unitGrid;
    public TileMappingGrid(Tilemap _tilemap)
    {
        this._tilemap = _tilemap;
        this.unitsX = parameters.gridUnitWidth/ unit.unitWidth;
        this.unitsY = parameters.gridLength/ unit.unitLength;
        this.unitAvailability = new bool[unitsX, unitsY];
        this.unitGrid = new BoundsInt[unitsX,unitsY];
    }

    private void fillUnit(BoundsInt unitBounds)
        {
            for (int x = unitBounds.min.x; x < unitBounds.max.x; x++)
                {
                    for (int y = unitBounds.min.y; y < unitBounds.max.y; y++)
                    {
                        this._tilemap.SetTile(new Vector3Int(x, y, 0), this.baseTile);
                    }
                }
        }

    public void fillGrid()
    {
        for(int x = 0; x < this.grid.unitsX; x++)
        {
            for(int y = 0; y < this.grid.unitsY; y++)
            {
                if(this.unitAvailability[x,y] != true)
                {
                    this.unitGrid[x,y].position = calculatePosition(new Vector3Int(x, y, 0));
                    fillUnit(unitGrid[x,y]);        
                }
            }
        }
    }

    public Vector3Int calculatePosition(Vector3Int index)
    {
        Vector3Int position = new Vector3Int();
        position.x = index.x * unit.unitWidth;
        position.y = index.y * unit.unitLength;
        return position;
    }
}

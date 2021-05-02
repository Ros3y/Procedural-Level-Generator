using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using Zigurous.DataStructures;

public class TileMappingUnit : MonoBehaviour
{
    public TileMappingParametersOLD.Parameters parameters;
    public int unitWidth; 
    public int unitLength;
    public TileMappingUnit()
    {
        unitWidth = parameters.gridUnitWidth;
        unitWidth = parameters.gridUnitLength;

    }

   
}

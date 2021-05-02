using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zigurous.DataStructures;

public class TileMappingParameters : MonoBehaviour
{
    public enum axisOfSymetry
    {
        [InspectorName("(-x, y)")]
        X,
        [InspectorName("(x, -y)")]
        Y,
        [InspectorName("(-x, -y)")]
        Diagonal1,
        [InspectorName("(y, x)")]
        Diagonal2
    }
     
     
     [System.Serializable]
    public struct Parameters
    {
        [Tooltip("Generates a new layout (will update with any changed parameters)")]
        public bool regenerate;
        [Tooltip("The Dimensions of each unit to be tiled")]
        public Vector2Int unitSize;
        [Tooltip("Minimum size of rooms")]
        public Vector2Int roomSizeMin;
        [Tooltip("Maximum size of rooms")]
        public Vector2Int roomSizeMax;
        [Tooltip("Room size multiplier for central room (this room is always the first room generated)")]
        public int centralRoomScale;
        [Tooltip("Chace for a room to be generated as a focal room (room generated with a multiplier)")]
        [Range(0.0f,100.0f)]
        public float focalRoomChance;
        [Tooltip("Room size multiplier for any room (if a room is generated as a focal room this multiplier will be applied to it)")]
        public int focalRoomScale;
        [Tooltip("Minimum distance between rooms (only applies on a room to room generation basis)")]
        public int roomDistanceMin;
        [Tooltip("Maximum distance between rooms (only applies on a room to room generation basis)")]
        public int roomDistanceMax;
        [Tooltip("Minimum offset from host room")]
        public int roomOffsetMin;
        [Tooltip("Maximum offset from host room")]
        public int roomOffsetMax;
        [Tooltip("Minimum possible rooms to be generated")]
        public int roomQuantityMin;
        [Tooltip("Maximum possible rooms to be generated")]
        public int roomQuantityMax;
        [Tooltip("Width of all connections between rooms")]
        public int corridorWidth;
        [Tooltip("Allows a second pass of corridor generation between random rooms (may cause some seperations)")]
        public bool allowAdditionalConnections;       
        [Tooltip("Number of added connections between rooms (this happeneds after innitial generation has been completed and is applied as a percentage of total rooms)")]
        
        [Range(0.0f,1.0f)]
        public float additionalConnections;
        [Tooltip("Chance to Branch from central room")]

        [Range(0.0f,100.0f)]
        public float centralBranchChance;
        [Tooltip("Determines if branching happends symetricaly (on / off)")]
        public bool symetricalBranching;
        [Tooltip("Determines in what axis to mirror on")]
        public axisOfSymetry axisOfSymetry;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zigurous.DataStructures;

public class TileMappingParametersOLD : MonoBehaviour
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

    public enum CastleSize
    {
        Small,
        Medium,
        Large,
        Custom
    }
     
     
     [System.Serializable]
    public struct Parameters
    {
        public IntRange roomWidth;
        public IntRange roomLength;
        public IntRange roomQuantity;
        public IntRange roomToRoomDistance;
        public IntRange roomOffset;
        public int gridUnitWidth;
        public int gridUnitLength;
        public int gridWidth;
        public int gridLength;
        public bool hasHubRoom;
        
        [ConditionalShow(nameof(hasHubRoom))]
        public int numberOfHubRooms;
        [ConditionalShow(nameof(hasHubRoom))]
        public int hubRoomScale;
        public int proximityScale;
        public int additionalConnections;
        public int corridorWidth;

        public bool symetricalBranching;
        public axisOfSymetry axisOfSymetry;
        public int branchLength;
    
        [Range(0.0f,100.0f)]
        public float centralBranchChance;
        




    }
}

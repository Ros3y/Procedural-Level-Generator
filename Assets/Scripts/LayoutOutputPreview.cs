using UnityEngine;
using Zigurous.DataStructures;

public abstract class LayoutOutputPreview
{
    public enum OutputType
    {
        [InspectorName("2D")]
        TwoD,
        [InspectorName("3D")]
        ThreeD
    }

    [System.Serializable]
    public struct Parameters
    {
        public OutputType outputType;
        public float wallThickness;
        [ConditionalShow(nameof(outputType), (int)OutputType.TwoD)]
        public MeshRenderer previewRenderer;
    }
    public abstract void Preview(LayoutStructure layout, Parameters parameters);
}
using UnityEngine;
public class LayoutOutput3DPreview : LayoutOutputPreview
{
    public override void Preview(LayoutStructure layout, Parameters parameters)
    {
        Bounds totalBounds = layout.CalculateTotalBounds();
        if(parameters.persectiveView)
        {
            Vector3 cameraPersective = new Vector3(45,45,0);
            Vector3 cameraPosition = new Vector3(totalBounds.min.x - 10, 90, totalBounds.min.z - 10);
            Camera.main.transform.position = cameraPosition;
            Camera.main.orthographic = false;
            Camera.main.transform.eulerAngles = cameraPersective;
            Camera.main.backgroundColor = Color.cyan;
        }

        else
        {
            Vector3 cameraPosition = new Vector3(totalBounds.center.x, 90, totalBounds.center.z);
            Camera.main.transform.position = cameraPosition;
            Camera.main.orthographicSize = Mathf.Max(totalBounds.extents.x, totalBounds.extents.z);
        }

        for(int i = 0; i < layout.rooms.Count; i++)
        {
            GameObject room = GameObject.CreatePrimitive(PrimitiveType.Cube);
            room.name = "Room " + i; 
            room.transform.position = layout.rooms[i].bounds.center;
            room.transform.localScale = layout.rooms[i].bounds.size;
            room.GetComponent<Renderer>().material = parameters.previewMaterial;
        }
    }

}
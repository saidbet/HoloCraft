using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;

public class ScanController : Singleton<ScanController>
{
    public bool enableHiRezScan = false;
    public bool scanDone;
    public bool scanInProgress;

    private void Start()
    {
        SetHirezScan(enableHiRezScan);
    }

    public bool StopMapping()
    {
        SpatialMappingManager.Instance.DrawVisualMeshes = false;

        SurfaceMeshesToPlanes.Instance.MakePlanes();

        InfoDisplay.Instance.UpdateText("Mapping stopped.");
        scanDone = true;
        scanInProgress = false;
        return true;
    }

    public bool StartMapping()
    {
        if (!SpatialMappingManager.Instance.IsObserverRunning())
        {
            SpatialMappingManager.Instance.StartObserver();
        }

        SpatialMappingManager.Instance.DrawVisualMeshes = true;
        InfoDisplay.Instance.UpdateText("Mapping started.\nWhen you are satisfied with the placement \nof walls, click \"Scan\" one more time.");
        scanInProgress = true;
        return true;
    }

    public void ResetMapping()
    {
        SpaceManager.Instance.ResetPlanes();
        scanDone = false;
    }

    public void SetHirezScan(bool hiRez)
    {
        int trianglesPerCubicMeter = hiRez ? 2000 : 500;
        SpatialMappingManager.Instance.gameObject.GetComponent<SpatialMappingObserver>().TrianglesPerCubicMeter = trianglesPerCubicMeter;
        SpatialMappingManager.Instance.StartObserver();
    }

}
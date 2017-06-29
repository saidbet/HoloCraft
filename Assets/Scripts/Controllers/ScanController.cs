using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanController : Singleton<ScanController>
{
    public bool enableHiRezScan = false;
    public bool scanDone;

    private void Start()
    {
        SetHirezScan(enableHiRezScan);
    }

    public bool StopMapping()
    {
        SpatialMappingManager.Instance.DrawVisualMeshes = false;
        
        SpaceManager.Instance.CreatePlanes();

        // scan détaillé
        /*
        if (enableHiRezScan) {
            StopHiResMapping();
        }*/

        InfoDisplay.Instance.UpdateText("Mapping stopped.");
        scanDone = true;
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
        return true;
    }

    public void ResetMapping()
    {
        SpaceManager.Instance.ResetPlanes();
        scanDone = false;
    }

    public void StartHiResMapping()
    {
        if (!(SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Scanning &&
!SpatialUnderstanding.Instance.ScanStatsReportStillWorking))
        {
            SpatialUnderstanding.Instance.RequestBeginScanning();
            InfoDisplay.Instance.UpdateText("Hi-res Mapping started");
        }
    }

    public void StopHiResMapping()
    {
        if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Scanning &&
!SpatialUnderstanding.Instance.ScanStatsReportStillWorking)
        {
            SpatialUnderstanding.Instance.UnderstandingCustomMesh.DrawProcessedMesh = false;
            SpatialUnderstanding.Instance.RequestFinishScan();
            InfoDisplay.Instance.UpdateText("Hi-res Mapping stopped");
        }
    }

    public IEnumerator HideUnderstandingMesh()
    {
        int i = 0;
        while(i < 10)
        {
            Renderer[] rs = SpatialUnderstanding.Instance.gameObject.GetComponentsInChildren<Renderer>();
            foreach(Renderer r in rs)
            {
                r.enabled = false;
            }
            yield return new WaitForSeconds(1F);
            i++;
        }
    }

    public void SetHirezScan(bool hiRez)
    {
        int trianglesPerCubicMeter = hiRez ? 2000 : 500;
        SpatialMappingManager.Instance.gameObject.GetComponent<SpatialMappingObserver>().TrianglesPerCubicMeter = trianglesPerCubicMeter;
        SpatialMappingManager.Instance.StartObserver();
    }
    
}
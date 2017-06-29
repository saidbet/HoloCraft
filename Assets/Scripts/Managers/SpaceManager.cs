using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Unity.InputModule;
using System;

/// <summary>
/// The SurfaceManager class allows applications to scan the environment for a specified amount of time 
/// and then process the Spatial Mapping Mesh (find planes, remove vertices) after that time has expired.
/// </summary>
public class SpaceManager : Singleton<SpaceManager>, IInputClickHandler
{
    public GameObject wallPrefab;
    public SpatialMappingManager spatialMappingManager;

    public int minimumWalls = 1;

    public float wallAngleThreshold = 10f;
    public float moveTime = 0.3f;

    private List<GameObject> walls;
    private List<GameObject> floors;
    private List<GameObject> selectedWalls;
    public List<GameObject> floorsFound;

    private bool selectingWalls = false;

    public int count = 0;

    public Material blackMaterial;

    private List<SyncWall> syncWallList;

    private void Start()
    {
        // Update surfaceObserver and storedMeshes to use the same material during scanning.
        //SpatialMappingManager.Instance.SetSurfaceMaterial(defaultMaterial);
        walls = new List<GameObject>();
        floors = new List<GameObject>();
        selectedWalls = new List<GameObject>();
        // Register for the MakePlanesComplete event.
        SurfaceMeshesToPlanes.Instance.MakePlanesComplete += SurfaceMeshesToPlanes_MakePlanesComplete;
        InputManager.Instance.AddGlobalListener(gameObject);
    }

    /// <summary>
    /// Creates planes from the spatial mapping surfaces.
    /// </summary>
    public void CreatePlanes()
    {
        if(walls.Count > 0)
        {
            ResetPlanes();
        }


        // Generate planes based on the spatial map.
        SurfaceMeshesToPlanes surfaceToPlanes = SurfaceMeshesToPlanes.Instance;

        if (surfaceToPlanes != null && surfaceToPlanes.enabled)
        {
            surfaceToPlanes.MakePlanes();
        }

        //StartCoroutine(HideSpatialUnderstandingMesh());
    }

    /// <summary>
    /// Removes triangles from the spatial mapping surfaces.
    /// </summary>
    /// <param name="boundingObjects"></param>
    private void RemoveVertices(IEnumerable<GameObject> boundingObjects)
    {
        RemoveSurfaceVertices removeVerts = RemoveSurfaceVertices.Instance;
        if (removeVerts != null && removeVerts.enabled)
        {
            removeVerts.RemoveSurfaceVerticesWithinBounds(boundingObjects);
        }
    }

    private void SurfaceMeshesToPlanes_MakePlanesComplete(object source, System.EventArgs args)
    {
        //RemoveVertices(SurfaceMeshesToPlanes.Instance.ActivePlanes);

        List<GameObject> tempWalls = new List<GameObject>();
        tempWalls.AddRange(SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Wall));
        List<GameObject> tempFloors = new List<GameObject>();
        tempFloors.AddRange(SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Floor));
        walls = new List<GameObject>();
        floors = new List<GameObject>();
        foreach (GameObject w in tempWalls)
        {
            Debug.Log("found wall pos:" + w.transform.position);
            SyncSpawnedObject obj = new SyncSpawnedObject();
            Vector3 newPos = w.transform.position + ShareManager.Instance.spawnManager.defaultParent.transform.position;
            Quaternion newRot = w.transform.rotation * ShareManager.Instance.spawnManager.defaultParent.transform.rotation;
            GameObject wall = ShareManager.Instance.spawnManager.Spawn(obj, wallPrefab, NetworkSpawnManager.EVERYONE, "", null, newPos, newRot , w.transform.localScale);
            wall.layer = 8;
            wall.tag = "Walls";
            wall.GetComponent<Renderer>().enabled = false;
            walls.Add(wall);
            w.SetActive(false);
            wall.transform.position = w.transform.position;
            wall.transform.localScale = w.transform.localScale;
            wall.transform.localRotation = w.transform.localRotation;
            Debug.Log("spawned wall pos:" + wall.transform.position);
        }
        foreach (GameObject f in tempFloors)
        {
            f.layer = 8;
            f.tag = "Floors";
            f.GetComponent<Renderer>().enabled = false;
        }
        StartCoroutine(HideSpatialUnderstandingMesh());
        DisplayWallSurfaces();
        StartWallSelection();
    }

    public void StartWallSelection()
    {
        selectingWalls = true;
    }

    public void StopWallSelection()
    {
        selectingWalls = false;
    }

    public void ResetPlanes()
    {
        if(walls.Count > 0)
        {
            /*
            foreach(GameObject plane in SurfaceMeshesToPlanes.Instance.ActivePlanes)
            {
                Destroy(plane);
            }*/

            foreach(GameObject wall in walls)
            {
                ShareManager.Instance.spawnManager.Delete(wall);
            }

            foreach(GameObject floor in floors)
            {
                ShareManager.Instance.spawnManager.Delete(floor);
            }

            walls.Clear();
            floors.Clear();
            selectedWalls.Clear();
            selectingWalls = false;
        }
    }

    public List<GameObject> getFloorsConnectedToWall(GameObject wall)
    {
        List<GameObject> listFloors = new List<GameObject>();
        floorsFound = new List<GameObject>();
        BoxCollider wallCollider = wall.GetComponent<BoxCollider>();
        wallCollider.bounds.Expand(0.2F);
        foreach(GameObject f in floors)
        {
            if (f.GetComponent<BoxCollider>().bounds.Intersects(wallCollider.bounds))
            {
                listFloors.Add(f);
                floorsFound.Add(f);
            }
        }

        return listFloors;
    }

    public bool gazePointValid()
    {
        Vector3 camPos = Camera.main.transform.position;
        Vector3 rayVec = Camera.main.transform.forward * 8f;

        SpatialUnderstandingDll.Imports.PlayspaceRaycast(camPos.x, camPos.y, camPos.z, rayVec.x, rayVec.y, rayVec.z, SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticRaycastResultPtr());
        SpatialUnderstandingDll.Imports.RaycastResult result = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticRaycastResult();
        if(result.SurfaceType == SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.WallLike ||
            result.SurfaceType == SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.WallExternal)
        {
            return true;
        } else
        {
            return false;
        }
    }

    private IEnumerator HideSpatialUnderstandingMesh()
    {
        yield return new WaitForSeconds(6f);
        SpatialUnderstanding.Instance.UnderstandingCustomMesh.DrawProcessedMesh = false;
        List<MeshFilter> liste = SpatialUnderstanding.Instance.UnderstandingCustomMesh.GetMeshFilters();
        foreach(MeshFilter m in liste)
        {
            m.gameObject.GetComponent<Renderer>().enabled = false;
        }
    }

    public void DisplayWallSurfaces()
    {
        syncWallList = new List<SyncWall>();
        foreach (GameObject wall in walls)
        {
            /*
            SyncWall syncWall = new SyncWall();
            NetworkSpawnReferrer.Instance.spawnManager.Spawn(syncWall, wall.transform.position, wall.transform.localRotation, null, "wall", true);
            syncWall.Transform.Scale.Value = wall.transform.localScale;*/
            wall.GetComponent<Renderer>().enabled = true;
        }
    }

    public void HideWallSurfaces()
    {
        foreach (GameObject wall in walls)
        {
            wall.GetComponent<Renderer>().material = blackMaterial;
        }
    }

    private void BroadcastSelectedWallList()
    {
        string message = "";
        if(selectedWalls.Count > 0)
        {
            message += selectedWalls[0].name;
            for(int i = 1; i < selectedWalls.Count; i++)
            {
                message += "*" + selectedWalls[i].name;
            }

        }
        //ShareManager.Instance.SendSyncMessage(ShareManager.SELECTED_WALLS, message);
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (selectingWalls && GazeManager.Instance.HitObject != null && walls.Contains(GazeManager.Instance.HitObject))
        {
            GameObject wall = GazeManager.Instance.HitObject;
            if(wall == null)
            {
                return;
            }
            if (selectedWalls.Contains(wall))
            {   
                selectedWalls.Remove(wall);
            } else
            {
                selectedWalls.Add(wall);
            }
            BroadcastSelectedWallList();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;
using HoloToolkit.Unity.SpatialMapping;
using HoloToolkit.Unity;
using System.Runtime.InteropServices;
using UnityEngine.VR.WSA;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Sharing.SyncModel;

/// <summary>
/// AnchorController: Class that enables the placing and positioning of the anchors. Each anchor needs to know
/// its index (to determine its color and the angle of the ruler). The rules are defined in Managers/AnchorSynchronizer.
/// The position has to been done here instead of the synchronizer, because only one person can decide where to put the anchor.
/// </summary>
public class AnchorController : Singleton<AnchorController>, IInputClickHandler
{
    public GameObject anchorPrefab;
    public GameObject originAnchorPrefab;
    public GameObject rulerPrefab;
    public GameObject rulerSizePrefab;
    private GazeManager gazeManager;
    public List<GameObject> anchorsList;
    private List<GameObject> rulersList;
    private List<GameObject> rulerTextList;
    private GameObject currentAnchor;
    private GameObject currentRuler;
    private GameObject currentRulerSize;
    private bool placing;
    private int index;
    //number of anchors to place
    private int nbrToPlace = 3;
    private int nbrPlaced;
    private Vector3 originNormal;
    private bool lockedOnFloor;

    public float originAnchorSnapDistance = 0.2f;
    float distance = 0;


    private void Start()
    {
        placing = false;
        InputManager.Instance.AddGlobalListener(this.gameObject);
        gazeManager = GazeManager.Instance;
        lockedOnFloor = false;
    }

    private void Update()
    {
        if (gazeManager.IsGazingAtObject)
        {
            if (placing && gazeManager.HitObject.CompareTag("Walls") && index < 3)
            {
                PositionAnchor();
            }
            else if (placing && gazeManager.HitObject.CompareTag("Floors") && index > 2)
            {
                PositionAnchor();
            }
        }
    }

    public bool StartAnchorsPlacing()
    {
        if (!placing)
        {
            DestroyAnchors();
            NextAnchor();
            rulerTextList = new List<GameObject>();
            return true;
        }
        return false;
    }

    public bool StopAnchorsPlacing()
    {
        placing = false;
        Destroy(currentAnchor);
        return true;
    }

    private void NextAnchor()
    {
        if (nbrPlaced <= 0)
        {
            index = 0;
            rulersList = new List<GameObject>();
            anchorsList = new List<GameObject>();
            currentAnchor = ShareManager.Instance.spawnManager.Spawn(new SyncSpawnedObject(), originAnchorPrefab, NetworkSpawnManager.EVERYONE, "");
        }
        else if (nbrPlaced > 0)
        {
            index++;
            currentRuler = ShareManager.Instance.spawnManager.Spawn(new SyncSpawnedObject(), rulerPrefab, NetworkSpawnManager.EVERYONE, "");
            currentAnchor = ShareManager.Instance.spawnManager.Spawn(new SyncAnchor(), anchorPrefab, NetworkSpawnManager.EVERYONE, "AnchorSynchronizer");
            currentRulerSize = ShareManager.Instance.spawnManager.Spawn(new SyncText(), rulerSizePrefab, NetworkSpawnManager.EVERYONE, "RulerSynchroniser");
        }

        Color color;
        switch (index)
        {
            case 0:
                color = Color.grey;
                break;

            case 1:
                color = Color.red;
                break;

            case 2:
                color = Color.green;
                break;

            case 3:
                color = Color.blue;
                break;

            default:
                color = Color.grey;
                break;
        }

        currentAnchor.GetComponentInChildren<Renderer>().material.color = color;
        Debug.Log("Placing passé à true");
        placing = true;

    }

    public bool PlaceAnchor()
    {
        if (index == 0)
        {
            originNormal = gazeManager.HitInfo.normal;
        }

        placing = false;

        if (currentRuler)
        {
            rulersList.Add(currentRuler);
            rulerTextList.Add(currentRulerSize);
            currentRulerSize.transform.position = new Vector3(currentRuler.transform.position.x, currentRuler.transform.position.y + 0.01f, currentRuler.transform.position.z);
        }

        anchorsList.Add(currentAnchor);
        nbrPlaced++;

        if (nbrPlaced >= nbrToPlace || (nbrPlaced == nbrToPlace && !lockedOnFloor))
        {
            SpaceManager.Instance.HideWallSurfaces();
            InfoDisplay.Instance.UpdateText("Placing finished.\nDistance between 1 and 2: " + Vector3.Distance(anchorsList[0].transform.position, anchorsList[1].transform.position) +
                        "\nDistance between 1 and 3: " + Vector3.Distance(anchorsList[0].transform.position, gazeManager.HitInfo.point));

            currentAnchor = null;
            currentRuler = null;
            ShareManager.Instance.SendSyncMessage(ShareManager.ANCHORS_LIST, anchorsList[0].name + "*" + anchorsList[1].name + "*" + anchorsList[2].name);
            VerandaController.Instance.ValidateVeranda(SharedController.Instance.currentVerandaId);
            SharedController.Instance.SetBackyard();
            return true;
        }
        NextAnchor();
        return true;
    }

    public void AdjustAnchorsPositions(float xLength, float yLength)
    {
        Vector3 horizontalNormal = anchorsList[1].transform.position - anchorsList[0].transform.position;
        horizontalNormal.Normalize();
        Vector3 verticalNormal = anchorsList[2].transform.position - anchorsList[0].transform.position;
        verticalNormal.Normalize();
        Vector3 newXPos = anchorsList[0].transform.position + (horizontalNormal * xLength);
        Vector3 newYPos = anchorsList[0].transform.position + (verticalNormal * yLength);
        anchorsList[1].transform.position = newXPos;
        anchorsList[2].transform.position = newYPos;
        rulersList[0].transform.position = ((anchorsList[1].transform.position - anchorsList[0].transform.position) * 0.5f) + anchorsList[0].transform.position;
        rulersList[0].transform.localScale = new Vector3(xLength, 0.002f, 0.002f);
        rulerTextList[0].transform.position = new Vector3(rulersList[0].transform.position.x, rulersList[0].transform.position.y + 0.01f, rulersList[0].transform.position.z);
        TextMesh textMesh1 = rulerTextList[0].GetComponent<TextMesh>();
        textMesh1.text = xLength.ToString("F3");
        rulersList[1].transform.position = ((anchorsList[2].transform.position - anchorsList[0].transform.position) * 0.5f) + anchorsList[0].transform.position;
        rulersList[1].transform.localScale = new Vector3(0.002f, yLength, 0.002f);
        rulerTextList[1].transform.position = new Vector3(rulersList[1].transform.position.x, rulersList[1].transform.position.y + 0.01f, rulersList[1].transform.position.z);
        TextMesh textMesh2 = rulerTextList[1].GetComponent<TextMesh>();
        textMesh2.text = yLength.ToString("F3");

        VerandaController.Instance.AdjustVerandaPos();
    }

    private Vector3 getSnapPoint(GameObject wallTouched, Vector3 hitPos)
    {
        if (wallTouched != null)
        {
            List<GameObject> floors = new List<GameObject>();
            floors = SpaceManager.Instance.getFloorsConnectedToWall(wallTouched);

            // Get snap point from floors
            Vector3 posTemp = new Vector3(0, 0, 0);
            foreach (GameObject f in floors)
            {
                Vector3 newPos = f.GetComponent<BoxCollider>().bounds.ClosestPoint(hitPos);
                if (Vector3.Distance(newPos, hitPos) < Vector3.Distance(posTemp, hitPos))
                {
                    posTemp = newPos;
                }
            }
            Vector3 newHitPos = hitPos;
            // Get snap point from wall and check if it is closest
            Vector3 wallPos = new Vector3(hitPos.x, wallTouched.GetComponent<BoxCollider>().bounds.min.y, hitPos.z);
            if (Vector3.Distance(hitPos, wallPos) < Vector3.Distance(hitPos, posTemp))
            {
                newHitPos.y = wallPos.y;
            }
            else
            {
                newHitPos.y = posTemp.y;
            }
            return newHitPos;
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }

    private void PositionAnchor()
    {
        float posX;
        float posY;
        float posZ;

        currentAnchor.transform.rotation = Quaternion.LookRotation(gazeManager.HitInfo.normal);

        switch (index)
        {
            case 0:
                InfoDisplay.Instance.UpdateText("Positioning first anchor (Bottom-left).");
                // Calculer la position au Y du sol le plus proche ou du bord du mur
                GameObject wallTouched = gazeManager.HitObject;
                Vector3 hitPos = gazeManager.HitPosition;
                Vector3 snapPoint = getSnapPoint(wallTouched, hitPos);
                if (Vector3.Distance(snapPoint, hitPos) < originAnchorSnapDistance)
                {
                    currentAnchor.transform.position = snapPoint;
                    lockedOnFloor = true;
                }
                else
                {
                    currentAnchor.transform.position = hitPos;
                    lockedOnFloor = false;
                }
                break;
            case 1:
                posX = gazeManager.HitPosition.x;
                posY = anchorsList[0].transform.position.y;
                posZ = gazeManager.HitPosition.z;
                Vector3 posAnchorX = new Vector3(posX, posY, posZ);
                Vector3 direction = posAnchorX - anchorsList[0].transform.position;
                float angleDir = AngleDir(Camera.main.transform.forward, direction, Camera.main.transform.up);
                if (angleDir != 1f)
                {
                    posAnchorX = anchorsList[0].transform.position;
                }
                currentAnchor.transform.position = posAnchorX;
                InfoDisplay.Instance.UpdateText("Positioning second anchor (Bottom-right).\nDistance between 1 and 2: " + Vector3.Distance(anchorsList[0].transform.position, currentAnchor.transform.position) +
                    "\nAngle between 1 and 2: " + angleDir);
                break;

            case 2:
                posX = anchorsList[0].transform.position.x;
                posY = gazeManager.HitPosition.y;
                posZ = anchorsList[0].transform.position.z;
                currentAnchor.transform.position = new Vector3(posX, posY, posZ);
                InfoDisplay.Instance.UpdateText("Positioning third anchor (Top-left).\nDistance between 1 and 2: " + Vector3.Distance(anchorsList[0].transform.position, anchorsList[1].transform.position) +
                    "\nAngle between 1 and 2: " + Vector3.Angle(anchorsList[0].transform.position, anchorsList[1].transform.position) +
                    "\nDistance between 1 and 3: " + Vector3.Distance(anchorsList[0].transform.position, currentAnchor.transform.position) +
                    "\nAngle between 1 and 3: " + Vector3.Angle(anchorsList[0].transform.position, currentAnchor.transform.position));
                break;

            case 3:
                posX = gazeManager.HitPosition.x;
                posY = anchorsList[0].transform.position.y;
                posZ = gazeManager.HitPosition.z;
                Vector3 posGlobal = new Vector3(posX, posY, posZ);
                Vector3 fw = anchorsList[0].transform.forward;
                float angle = Vector3.Angle(fw * 2f, posGlobal);
                float dist = Vector3.Distance(anchorsList[0].transform.position, posGlobal);
                float newDist = Mathf.Cos(angle) * dist;
                Vector3 newPos = anchorsList[0].transform.position;
                currentAnchor.transform.localRotation = anchorsList[0].transform.localRotation;
                newPos.z = newPos.z + newDist;
                currentAnchor.transform.position = newPos;
                InfoDisplay.Instance.UpdateText("Positioning fourth anchor (Floor).\nDistance between 1 and 2: " + Vector3.Distance(anchorsList[0].transform.position, anchorsList[1].transform.position) +
                "\nAngle between 1 and 2: " + Vector3.Angle(anchorsList[0].transform.position, anchorsList[1].transform.position) +
                "\nDistance between 1 and 3: " + Vector3.Distance(anchorsList[0].transform.position, currentAnchor.transform.position) +
                "\nDistance between 1 and 4: " + Vector3.Distance(anchorsList[0].transform.position, currentAnchor.transform.position));
                break;
        }

        if (index > 0)
        {
            RulerUpdate();
        }
    }

    private float AngleDir(Vector3 forward, Vector3 direction, Vector3 up)
    {
        Vector3 perpendicular = Vector3.Cross(forward, direction);
        float dir = Vector3.Dot(perpendicular, up);
        if (dir > 0f)
        {
            return 1f;
        }
        else if (dir > 0f)
        {
            return -1f;
        }
        else
        {
            return 0f;
        }
    }

    private void RulerUpdate()
    {
        TextMesh textMesh = currentRulerSize.GetComponent<TextMesh>();

        currentRuler.transform.rotation = currentAnchor.transform.rotation;
        textMesh.transform.forward = -currentAnchor.transform.forward;
        switch (index)
        {
            case 1:
                currentRuler.transform.position = ((currentAnchor.transform.position - anchorsList[0].transform.position) * 0.5f) + anchorsList[0].transform.position;
                distance = Vector3.Distance(anchorsList[0].transform.position, currentAnchor.transform.position);
                currentRuler.transform.localScale = new Vector3(distance, 0.002f, 0.002f);
                textMesh.transform.position = new Vector3(currentAnchor.transform.position.x, currentAnchor.transform.position.y + 0.01f, currentAnchor.transform.position.z);
                break;

            case 2:
                currentRuler.transform.position = ((currentAnchor.transform.position - anchorsList[0].transform.position) * 0.5f) + anchorsList[0].transform.position;
                distance = Vector3.Distance(anchorsList[0].transform.position, currentAnchor.transform.position);
                currentRuler.transform.localScale = new Vector3(0.002f, distance, 0.002f);
                textMesh.transform.position = new Vector3(currentRuler.transform.position.x, currentAnchor.transform.position.y - 0.01f, currentRuler.transform.position.z);
                break;

            case 3:
                if (gazeManager.HitObject.CompareTag("Floors"))
                {
                    //currentRuler.transform.localRotation = Quaternion.LookRotation(originNormal);
                    //currentRuler.transform.position = new Vector3(anchorsList[0].transform.position.x, gazeManager.HitPosition.y, (gazeManager.HitPosition.z - anchorsList[0].transform.position.z) * 0.5f + anchorsList[0].transform.position.z);
                    //line.localScale = new Vector3(0.002f, 0.002f, distance);
                    //textMesh.transform.position = new Vector3(line.transform.position.x, currentAnchor.transform.position.y - 0.01f, currentAnchor.transform.position.z - 0.01f);
                }
                break;
        }
        textMesh.text = distance.ToString("F3") + "m";
    }

    public void ResetAnchors()
    {
        placing = false;
        DestroyAnchors();
    }

    public void DestroyAnchors()
    {
        if (anchorsList != null && anchorsList.Count > 0)
        {
            foreach (GameObject anchor in anchorsList)
            {
                ShareManager.Instance.spawnManager.Delete(anchor);
            }
        }

        if (rulersList != null && rulersList.Count > 0)
        {
            foreach (GameObject ruler in rulersList)
            {
                ShareManager.Instance.spawnManager.Delete(ruler);
            }
        }
        if(rulerTextList != null && rulerTextList.Count > 0)
        {
            foreach(GameObject rulerSize in rulerTextList)
            {
                ShareManager.Instance.spawnManager.Delete(rulerSize);
            }
        }

        if (currentRuler)
        {
            ShareManager.Instance.spawnManager.Delete(currentRuler);
            currentRuler = null;
        }
        if (currentAnchor)
        {
            ShareManager.Instance.spawnManager.Delete(currentAnchor);
            currentAnchor = null;
        }
        if (currentRulerSize)
        {
            ShareManager.Instance.spawnManager.Delete(currentRulerSize);
            currentRulerSize = null;
        }

        anchorsList = new List<GameObject>();
        rulersList = new List<GameObject>();
        rulerTextList = new List<GameObject>();
        

        nbrPlaced = 0;
    }

    public void UndoAnchors()
    {
        if (nbrPlaced > 0)
        {
            placing = false;

            if (anchorsList.Count == nbrPlaced && anchorsList.Count > 0)
            {
                ShareManager.Instance.spawnManager.Delete(anchorsList[nbrPlaced - 1]);
                anchorsList.RemoveAt(nbrPlaced - 1);
            }

            if (rulersList.Count == nbrPlaced - 1 && rulersList.Count > 0)
            {
                ShareManager.Instance.spawnManager.Delete(rulersList[nbrPlaced - 2]);
                ShareManager.Instance.spawnManager.Delete(rulerTextList[nbrPlaced - 2]);
                rulersList.RemoveAt(nbrPlaced - 2);
                rulerTextList.RemoveAt(nbrPlaced - 2);
            }

            if (currentRuler)
            {
                ShareManager.Instance.spawnManager.Delete(currentRuler);
            }

            if (currentRulerSize)
                ShareManager.Instance.spawnManager.Delete(currentRulerSize);

            if (currentAnchor)
                ShareManager.Instance.spawnManager.Delete(currentAnchor);

            nbrPlaced--;
            index = nbrPlaced - 1;
            NextAnchor();
        } else
        {
            MainController.Instance.StopPlacingAnchors();
        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        Debug.Log("called from AnchorManager");
        if (placing)
        {
            if (gazeManager.IsGazingAtObject && (gazeManager.HitObject.CompareTag("Walls") || gazeManager.HitObject.CompareTag("Floors")))
            {
                PlaceAnchor();
            }
        }
    }
}

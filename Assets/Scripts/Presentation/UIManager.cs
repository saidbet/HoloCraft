using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour, IRemoveOnSpawn {
    public float moveAngle;
    public MoveStatus moveStatus;
    public float distance;
    public float triggerDistance = 0.06f;
    public float distanceBox;
    private float startTime;
    private float endTime = 0.5f;
    private Vector3 velocity = Vector3.zero;
    private Vector3 targetPos;
    public float delay = 1f;

    public enum MoveStatus
    {
        NO,
        REQ,
        ACK,
        DONE,
        MOVING
    }

	// Use this for initialization
	void Start () {
        moveStatus = MoveStatus.NO;
	}
	
	// Update is called once per frame
	void Update () {
        if (checkMenuProximity() && moveStatus == MoveStatus.NO)
        {
            moveStatus = MoveStatus.REQ;
            StartCoroutine(PrepareToMoveGui());
        }
        if (moveStatus == MoveStatus.DONE)
        {
            moveStatus = MoveStatus.NO;
        }
        if(moveStatus == MoveStatus.MOVING)
        {
            if(startTime > endTime)
            {
                moveStatus = MoveStatus.DONE;
            }
            startTime += Time.deltaTime;
            gameObject.transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, endTime);
        }
        if(moveStatus == MoveStatus.ACK && !checkMenuProximity())
        {
            moveStatus = MoveStatus.NO;
            StopAllCoroutines();
        }
    }

    private bool checkMenuProximity()
    {
        GameObject hitObject = GazeManager.Instance.HitObject;
        //distanceBox = GetComponent<BoxCollider>().bounds.SqrDistance(Camera.main.transform.position + Camera.main.transform.forward * 2);
        float distanceToGui = Vector3.Distance(Camera.main.transform.position, gameObject.transform.position);
        Vector3 facticePoint = Camera.main.transform.position + Camera.main.transform.forward * distanceToGui;
        Vector3 closest = GetComponent<BoxCollider>().ClosestPointOnBounds(facticePoint);
        distanceBox = Vector3.Distance(facticePoint, closest);       
        return (hitObject == null || !(hitObject.CompareTag("Button") || hitObject.CompareTag("Veranda") || hitObject.CompareTag("PreviewInfos"))) && distanceBox < triggerDistance;
    }

    IEnumerator PrepareToMoveGui()
    {
        if(moveStatus == MoveStatus.REQ && checkMenuProximity())
        {
            moveStatus = MoveStatus.ACK;
            yield return new WaitForSeconds(delay);
            if(moveStatus == MoveStatus.ACK && checkMenuProximity())
            {
                MoveGui();
            } else
            {
                moveStatus = MoveStatus.NO;
            }
        }
    }

    void MoveGui()
    {
        startTime = 0;
        var q = Quaternion.AngleAxis(moveAngle, Camera.main.transform.forward);
        targetPos = gameObject.transform.position + q * Camera.main.transform.right * distance;
        GetComponent<Interpolator>().PositionPerSecond = 10f;
        GetComponent<Interpolator>().SetTargetPosition(targetPos);
        moveStatus = MoveStatus.DONE;
    }
}

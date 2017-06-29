using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class FollowCamera : MonoBehaviour {

    public float offsetX;
    public float offsetY;
    public float distance;

    public HoloToolkit.Unity.InputModule.Cursor cursor;
    private Vector3 cursorPosition;
    private Vector3 newPosition;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update ()
    {
        cursorPosition = cursor.transform.position;
        newPosition.x = cursorPosition.x;
        newPosition.y = cursorPosition.y;
        newPosition.z = Camera.main.transform.position.z + distance;
        this.transform.position = newPosition;
    }
}

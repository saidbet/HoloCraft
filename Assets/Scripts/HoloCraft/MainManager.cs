using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : Singleton<MainManager>
{
    public enum Direction
    {
        Up,
        Down,
        Right,
        Left,
        Backward,
        Forward
    }

    public enum Mode
    {
        Building,
        PickerMenu,
        WorkspaceMenu,
        Moving,
        Scaling,
        Playing
    }

    public enum Axis
    {
        X,
        Y,
        Z
    }

    public GameObject workspacePrefab;
    private GameObject workspace;

    public Material validMat;
    public Material invalidMat;

    private bool isValid;
    private bool isOccupied;

    //Info relative to current block to place
    public GameObject objectToPlace;
    private Block currentObject;

    //Reference to other scripts
    public Creation creation;

    //Previous block pos and rot
    private Vector3 previousPos;
    private Quaternion previousRot;

    //Current mode
    public Mode mode;

    bool IsValid
    {
        get { return isValid; }

        set
        {
            if (value == true)
                currentObject.GetComponent<Block>().SetMaterialColor(Color.green);
            else
                currentObject.GetComponent<Block>().SetMaterialColor(Color.red);

            isValid = value;
        }
    }

    private void Start()
    {
        creation = GetComponent<Creation>();
        StartCoroutine(SpawnWorkspace());
        previousPos = new Vector3(creation.maxHeight / 2, creation.maxWidth / 2, creation.maxDepth / 2);
        previousRot = Quaternion.identity;
        InputHandler.Instance.keyPress += Instance_keyPress;
    }

    private IEnumerator SpawnWorkspace()
    {
        int timer = 20;

        do
        {
            if (ShareManager.Instance.spawnManager != null && ShareManager.Instance.spawnManager.SyncSourceReady())
            {
                workspace = ShareManager.Instance.spawnManager.Spawn(new SyncPanel(), workspacePrefab, NetworkSpawnManager.EVERYONE, "");
                workspace.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
            }

            yield return new WaitForSeconds(0.1f);
            timer -= 1;
        }
        while (workspace == null && timer > 0);

        if (workspace == null)
        {
            workspace = ShareManager.Instance.spawnManager.Spawn(new SyncPanel(), workspacePrefab, NetworkSpawnManager.EVERYONE, "");
            workspace.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1;
        }

        StartPlacing();
    }

    private void Instance_keyPress(KeyPress obj)
    {
        if (mode != Mode.Building) return;

        if (obj.button == ControllerConfig.RIGHT)
            Translate(Direction.Right);
        if (obj.button == ControllerConfig.LEFT)
            Translate(Direction.Left);
        if (obj.button == ControllerConfig.UP)
            Translate(Direction.Up);
        if (obj.button == ControllerConfig.DOWN)
            Translate(Direction.Down);
        if (obj.button == ControllerConfig.Y)
            Translate(Direction.Forward);
        if (obj.button == ControllerConfig.X)
            Translate(Direction.Backward);

        if (obj.button == ControllerConfig.LEFTSTICKUP)
            Rotate(Direction.Up);
        if (obj.button == ControllerConfig.LEFTSTICKDOWN)
            Rotate(Direction.Down);
        if (obj.button == ControllerConfig.LEFTSTICKLEFT)
            Rotate(Direction.Left);
        if (obj.button == ControllerConfig.LEFTSTICKRIGHT)
            Rotate(Direction.Right);

        if (obj.button == ControllerConfig.A)
        {
            if (creation.GetBlock(currentObject.position) == null)
                Validate();
        }
        if (obj.button == ControllerConfig.B)
        {
            creation.RemoveBlock(currentObject.transform.position);
            PlaceNext();
        }
    }

    private void StartPlacing()
    {
        workspace = workspace.transform.Find("WorkspaceController").gameObject;
        currentObject = ShareManager.Instance.spawnManager.Spawn(new SyncSpawnedObject(), objectToPlace, 0, "", workspace).GetComponent<Block>();
        currentObject.transform.localPosition = previousPos;
        creation.AddToDict(currentObject);
        currentObject.GetComponent<Block>().DisableSnapPoints();
        currentObject = null;
        mode = Mode.Building;
        PlaceNext();
    }

    private void PlaceNext()
    {
        if (currentObject != null)
        {
            Destroy(currentObject);
        }

        currentObject = ShareManager.Instance.spawnManager.Spawn(new SyncSpawnedObject(), objectToPlace, 0, "", workspace).GetComponent<Block>();
        currentObject.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        currentObject.transform.localPosition = previousPos;
        currentObject.transform.localRotation = previousRot;
        CheckValid();
    }

    public void Validate()
    {
        if (isValid == false) return;

        previousPos = currentObject.transform.localPosition;
        previousRot = currentObject.transform.localRotation;
        currentObject.transform.localScale = Vector3.one;
        creation.AddToDict(currentObject);
        currentObject.GetComponent<Block>().RestoreDefaultColor();
        currentObject.GetComponent<Block>().DisableSnapPoints();
        currentObject = null;
        PlaceNext();
    }

    public void Translate(Direction direction)
    {
        Vector3 translation = Vector3.zero;
        switch (direction)
        {
            case Direction.Left:
                translation = -Camera.main.transform.right;
                break;
            case Direction.Right:
                translation = Camera.main.transform.right;
                break;
            case Direction.Up:
                translation = Camera.main.transform.up;
                break;
            case Direction.Down:
                translation = -Camera.main.transform.up;
                break;
            case Direction.Backward:
                translation = -Camera.main.transform.forward;
                break;
            case Direction.Forward:
                translation = Camera.main.transform.forward;
                break;
        }

        Vector3 newTranslation = workspace.transform.InverseTransformDirection(translation);

        newTranslation.x = Round(newTranslation.x);
        newTranslation.y = Round(newTranslation.y);
        newTranslation.z = Round(newTranslation.z);

        Vector3 newPosition = currentObject.transform.localPosition + newTranslation;


        if (!CheckPosition(newPosition)) return;

        currentObject.transform.localPosition += newTranslation;

        CheckValid();
    }

    public void Rotate(Direction direction)
    {
        Debug.Log("Rotate : " + direction);
        switch (direction)
        {
            case Direction.Left:
                currentObject.transform.Rotate(0, -90, 0, Space.Self);
                break;
            case Direction.Right:
                currentObject.transform.Rotate(0, 90, 0, Space.Self);
                break;
            case Direction.Up:
                currentObject.transform.Rotate(90, 0, 0, Space.Self);
                break;
            case Direction.Down:
                currentObject.transform.Rotate(-90, 0, 0, Space.Self);
                break;
        }
    }

    private bool CheckPosition(Vector3 position)
    {
        if (position.x < 0 || position.x >= creation.maxWidth ||
            position.y < 0 || position.y >= creation.maxHeight ||
            position.z < 0 || position.z >= creation.maxDepth)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static float Round(float nbr)
    {
        if (nbr > 0.5)
        {
            return 1;
        }
        else if (nbr <= 0.5 && nbr >= -0.5)
        {
            return 0;
        }
        else if (nbr < -0.5)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    public void ChangeObject(GameObject newObject)
    {
        previousPos = currentObject.transform.localPosition;
        previousRot = currentObject.transform.localRotation;
        Destroy(currentObject);
        objectToPlace = newObject;
        PlaceNext();
    }

    private void CheckValid()
    {
        IsValid = false;
        isOccupied = creation.CheckKey(currentObject.position);
    }

    public void SnapColliding()
    {
        if (isOccupied == false && IsValid == false)
        {
            IsValid = true;
        }
    }
}

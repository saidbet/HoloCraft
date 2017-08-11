using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Unity;
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
        InMenu,
        Moving,
        Scaling,
        Playing
    }

    public GameObject workspacePrefab;
    private WorkspaceController workspaceController;
    public GameObject workspaceHolder;

    private bool _isValid;

    //Info relative to current block to place
    public GameObject objectToPlace;
    private Block currentObject;
    public Block _hoveredObject;
    public Block firstBlock;

    //Reference to other scripts
    public Creation creation;

    //Position and rotation informations
    private Vector3 currentPosition;
    private Quaternion previousRot;

    private float timer;

    public List<BlockType> listOfBlocks = new List<BlockType>();

    //Current mode
    private Mode _currentMode;

    public Mode CurrentMode
    {
        get { return _currentMode; }

        set
        {
            if (timer <= 0)
            {
                _currentMode = value;
                timer = 0.2f;
            }
            else
                return;
        }
    }

    bool IsValid
    {
        get { return _isValid; }

        set
        {
            if (value == true)
                currentObject.GetComponent<Block>().SetMaterialColor(Color.green);
            else
                currentObject.GetComponent<Block>().SetMaterialColor(Color.red);

            _isValid = value;
        }
    }

    public Block HoveredObject
    {
        get { return _hoveredObject; }

        private set
        {
            if (_hoveredObject != null)
                _hoveredObject.RestoreDefaultColor();

            if (value != null)
            {
                currentObject.Hide();
                value.SetMaterialColor(Color.yellow);
            }
            else
            {
                currentObject.UnHide();
            }

            _hoveredObject = value;
        }
    }

    private void Start()
    {
        creation = GetComponent<Creation>();
        StartCoroutine(SpawnWorkspace());
        previousRot = Quaternion.identity;
        InputHandler.Instance.keyPress += Instance_keyPress;
    }

    private void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
    }

    private IEnumerator SpawnWorkspace()
    {
        int timer = 20;

        do
        {
            if (ShareManager.Instance.spawnManager != null && ShareManager.Instance.spawnManager.SyncSourceReady())
            {
                workspaceController = ShareManager.Instance.spawnManager.Spawn(new SyncPanel(), workspacePrefab, NetworkSpawnManager.EVERYONE, "").GetComponent<WorkspaceController>();
                workspaceController.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
            }

            yield return new WaitForSeconds(0.1f);
            timer -= 1;
        }
        while (workspaceController == null && timer > 0);

        if (workspaceController == null)
        {
            workspaceController = ShareManager.Instance.spawnManager.Spawn(new SyncPanel(), workspacePrefab, NetworkSpawnManager.EVERYONE, "").GetComponent<WorkspaceController>();
            workspaceController.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1;
        }

        StartPlacing();
    }

    private void Instance_keyPress(KeyPress obj)
    {
        if (CurrentMode != Mode.Building) return;

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

        if (obj.button == ControllerConfig.RIGHTSTICKUP)
            Rotate(MainManager.Direction.Up);
        else if (obj.button == ControllerConfig.RIGHTSTICKDOWN)
            Rotate(MainManager.Direction.Down);
        else if (obj.button == ControllerConfig.RIGHTSTICKLEFT)
            Rotate(MainManager.Direction.Left);
        else if (obj.button == ControllerConfig.RIGHTSTICKRIGHT)
            Rotate(MainManager.Direction.Right);
        else if (obj.button == ControllerConfig.RIGHTSTICKDOWN)
            Rotate(MainManager.Direction.Down);


        if (obj.button == ControllerConfig.A)
        {
            if (creation.GetBlock(currentPosition) == null)
                Validate(currentPosition, currentObject.gameObject);
        }
        if (obj.button == ControllerConfig.B)
        {
            if (_hoveredObject != null && _hoveredObject != firstBlock)
            {
                creation.RemoveBlock(_hoveredObject.transform.localPosition);
                PlaceNext();
            }
        }
    }

    private void StartPlacing()
    {
        currentPosition = new Vector3(creation.maxHeight / 2, creation.maxWidth / 2, creation.maxDepth / 2);
        workspaceHolder = workspaceController.workspaceHolder;
        firstBlock = ShareManager.Instance.spawnManager.Spawn(new SyncSpawnedObject(), objectToPlace, 0, "", workspaceHolder).GetComponent<Block>();
        firstBlock.transform.localPosition = currentPosition;
        creation.AddToDict(currentPosition, firstBlock);
        firstBlock.GetComponent<Block>().DisableSnapPoints();
        firstBlock.FindMats();
        CurrentMode = Mode.Building;
        PlaceNext();
        Translate(Direction.Up);
    }

    private void PlaceNext()
    {
        currentObject = ShareManager.Instance.spawnManager.Spawn(new SyncSpawnedObject(), objectToPlace, 0, "", workspaceHolder).GetComponent<Block>();
        currentObject.transform.localPosition = currentPosition;
        currentObject.transform.localRotation = previousRot;
        currentObject.FindMats();
        CheckValid();
    }

    public void Validate(Vector3 position, GameObject block)
    {
        if (_isValid == false) return;

        previousRot = block.transform.localRotation;
        creation.AddToDict(position, block.GetComponent<Block>());
        block.GetComponent<Block>().RestoreDefaultColor();
        block.GetComponent<Block>().DisableSnapPoints();
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

        Vector3 newTranslation = workspaceController.transform.InverseTransformDirection(translation);

        newTranslation.x = Utility.Round(newTranslation.x);
        newTranslation.y = Utility.Round(newTranslation.y);
        newTranslation.z = Utility.Round(newTranslation.z);

        Vector3 newPosition = currentObject.transform.localPosition + newTranslation;


        if (!CheckPosition(newPosition)) return;

        currentPosition += newTranslation;
        currentObject.transform.localPosition = currentPosition;

        CheckValid();
    }

    public void Rotate(Direction direction)
    {
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

    public void ChangeObject(GameObject newObject)
    {
        previousRot = currentObject.transform.localRotation;
        Destroy(currentObject.gameObject);
        objectToPlace = newObject;
        PlaceNext();
    }

    private void CheckValid()
    {
        IsValid = false;
        HoveredObject = creation.GetBlock(currentPosition);
    }

    public void SnapColliding()
    {
        if (_hoveredObject == null && IsValid == false)
        {
            IsValid = true;
        }
    }

    public void StartPlayMode()
    {
        Destroy(currentObject.gameObject);
        CurrentMode = Mode.Playing;

        Transform parent = new GameObject().transform;

        foreach (var blk in creation.creationDict)
        {

            blk.Value.transform.SetParent(parent.transform);
            //blk.Value.transform.localScale = Vector3.one;

            FindAdjacents(blk.Key, blk.Value.gameObject);

            blk.Value.GetComponent<Rigidbody>().isKinematic = false;
            IPlayable playable = blk.Value.GetComponent<IPlayable>();

            if (playable != null)
                playable.Startplay();
        }

        workspaceController.GetComponent<WorkspaceController>().ToggleVisual(false);

    }


    private void FindAdjacents(Vector3 position, GameObject currentBlock)
    {
        Vector3[] adjacentPos = Utility.FindAdjacentPos(position);
        Block foundBlock = null;

        for (int i = 0; i < adjacentPos.Length; i++)
        {
            foundBlock = creation.GetBlock(adjacentPos[i]);
            if (foundBlock != null)
            {
                Join(currentBlock, foundBlock.gameObject);
            }
        }
    }

    private void Join(GameObject first, GameObject second)
    {
        FixedJoint fixedJoint = first.AddComponent<FixedJoint>();
        Rigidbody secondRb = second.EnsureComponent<Rigidbody>();
        fixedJoint.connectedBody = secondRb;
        fixedJoint.breakForce = 9999;
        fixedJoint.breakTorque = 9999;
    }
}

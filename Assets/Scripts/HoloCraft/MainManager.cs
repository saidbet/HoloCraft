using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Unity;
using System.Collections;
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
        PropertiesMenu,
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
                timer = 0.1f;
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
            if (creation.GetBlock(currentPosition) == null)
                Validate();
        }
        if (obj.button == ControllerConfig.B)
        {
            if (_hoveredObject != null && _hoveredObject != firstBlock)
                creation.RemoveBlock(_hoveredObject.transform.localPosition);
            PlaceNext();
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
        if (currentObject != null)
        {
            Destroy(currentObject.gameObject);
        }

        currentObject = ShareManager.Instance.spawnManager.Spawn(new SyncSpawnedObject(), objectToPlace, 0, "", workspaceHolder).GetComponent<Block>();
        currentObject.transform.localPosition = currentPosition;
        currentObject.transform.localRotation = previousRot;
        currentObject.FindMats();
        CheckValid();
    }

    public void Validate()
    {
        if (_isValid == false) return;

        previousRot = currentObject.transform.localRotation;
        creation.AddToDict(currentPosition, currentObject);
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

        GameObject fb = Instantiate(firstBlock.type.playPrefab, firstBlock.transform.parent);
        fb.transform.localPosition = firstBlock.transform.localPosition;
        firstBlock.gameObject.SetActive(false);
        Destroy(fb.GetComponent<FixedJoint>());
        Rigidbody rb = fb.GetComponent<Rigidbody>();

        foreach (var blk in creation.creationDict)
        {
            if (blk.Value == firstBlock) continue;
            GameObject instance = Instantiate(blk.Value.type.playPrefab, blk.Value.transform.parent);
            instance.GetComponent<BlockPropertiesValues>().properties = blk.Value.GetComponent<BlockPropertiesValues>().properties;
            blk.Value.gameObject.SetActive(false);
            instance.transform.localPosition = blk.Key;
            instance.transform.localRotation = blk.Value.transform.localRotation;
            instance.GetComponent<FixedJoint>().connectedBody = rb;
        }

        workspaceController.GetComponent<WorkspaceController>().ToggleVisual(false);

    }
}

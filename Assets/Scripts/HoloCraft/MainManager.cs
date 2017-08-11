using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class MainManager : Singleton<MainManager>
{
    public enum Mode
    {
        Building,
        InMenu,
        Moving,
        Scaling,
        Playing
    }

    public GameObject workspacePrefab;
    public WorkspaceController workspaceController;
    public GameObject workspaceHolder;

    //Info relative to current block to place

    public Block firstBlock;

    //Reference to other scripts
    public Creation creation;
    public Creator creator;
    public CreationsList creationsList;

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
                timer = 0.1f;
            }
            else
                return;
        }
    }

    private void Start()
    {
        creation = GetComponent<Creation>();
        creator = GetComponent<Creator>();
        StartCoroutine(SpawnWorkspace());

        object data = Utility.DeserializeFile("CreationsList");
        if (data != null)
            creationsList = (CreationsList)data;
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
        parent.localScale = workspaceHolder.transform.localScale;

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

        workspaceController.ToggleVisual(false);

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

    public void SaveData()
    {
        creation.AddToCreationsList(creationsList);
        Utility.SerializeFile("CreationList", creationsList);
    }

    public void LoadCreation(string name)
    {
        CreationData creationToLoad = creationsList.creations.Find(data => data.name == name);
        CleanUpWorkspace();
        PopulateWorkspace(creationToLoad);
    }

    private void CleanUpWorkspace()
    {
        foreach (var item in creationDict)
        {
            Destroy(item.Value.gameObject);
        }

        creationDict = new Dictionary<Vector3, Block>();
    }

    private void PopulateWorkspace(CreationData data)
    {
        foreach (var item in data.dataToSave)
        {
            GameObject toInstantiate = MainManager.Instance.listOfBlocks.Find(block => block.blockType == item.type).prefab;
            toInstantiate = Instantiate(toInstantiate, MainManager.Instance.workspaceHolder.transform);
            Vector3 position = new Vector3(item.posX, item.posY, item.posZ);
            toInstantiate.transform.localPosition = position;
            MainManager.Instance.Validate(position, toInstantiate);
        }
        MainManager.Instance.PlaceNext();
    }
}

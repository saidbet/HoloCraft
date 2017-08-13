using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : Singleton<MainManager>
{
    public enum Mode
    {
        Building,
        InMenu,
        Moving,
        Scaling,
        Playing,
        Placing
    }

    public GameObject workspacePrefab;
    public WorkspaceController workspaceController;
    public GameObject workspaceHolder;

    //Reference to other scripts
    private Creation creation;
    public Creator creator;
    public CreationsList creationsList;

    public string CreationsListFilename = "CreationsList";

    private float timer;

    public BlocksArray blockArray;
    private List<BlockType> listOfBlocks = new List<BlockType>();

    public Transform currentPlayingObject;

    //Current mode
    public Mode currentMode;

    private void Start()
    {
        creation = new Creation(20, 20, 20);
        creator = GetComponent<Creator>();
        listOfBlocks.AddRange(blockArray.array);
        InitCreationsList();
        StartCreator();
    }

    private void Update()
    {
        if (currentMode == Mode.Placing)
        {
            currentPlayingObject.transform.position = GazeManager.Instance.HitPosition + new Vector3(0, 0.5f, 0);
            if (CInput.aUp)
                ValidatePosition();
        }
    }

    private void InitCreationsList()
    {
        object data = Utility.DeserializeFile(CreationsListFilename);

        if (data != null)
            creationsList = (CreationsList)data;
        else
            creationsList = new CreationsList();
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

        workspaceHolder = workspaceController.workspaceHolder;
        Vector3 initialPos = new Vector3(creation.maxWidth / 2, creation.maxHeight / 2, creation.maxDepth / 2);
        creator.Initialize(initialPos, creation, workspaceHolder);
    }

    private void StartCreator()
    {
        currentMode = Mode.Building;
        StartCoroutine(SpawnWorkspace());
    }

    public void StartPlayMode()
    {
        currentMode = Mode.Playing;
        creator.StopCreation();

        currentPlayingObject = new GameObject().transform;
        currentPlayingObject.transform.rotation = workspaceHolder.transform.rotation;
        currentPlayingObject.transform.position = workspaceHolder.transform.position;
        currentPlayingObject.transform.localScale = workspaceHolder.transform.localScale;

        foreach (var blk in creation.creationDict)
        {

            blk.Value.transform.SetParent(currentPlayingObject.transform);
            JointToAdjacents(blk.Key, blk.Value.gameObject);
            IPlayable playable = blk.Value.GetComponent<IPlayable>();
            blk.Value.EnablePhysics();

            if (playable != null)
                playable.Startplay();
        }

        workspaceController.gameObject.SetActive(false);
    }

    private void JointToAdjacents(Vector3 position, GameObject currentBlock)
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
        Utility.SerializeFile(CreationsListFilename, creationsList);
    }

    public void LoadCreation(string name)
    {
        CreationData creationToLoad = creationsList.creations.Find(data => data.creationName == name);
        workspaceController.CleanUpWorkspace();
        creator.CleanUpWorkspace();
        creation.SetUpFromLoadData(creationToLoad);
        PopulateWorkspace(creationToLoad.savedBlocks);
        creator.PlaceNext();
    }

    private void PopulateWorkspace(BlockData[] data)
    {
        foreach (var item in data)
        {
            GameObject toInstantiate = listOfBlocks.Find(block => block.blockType == item.type).prefab;
            toInstantiate = Instantiate(toInstantiate, workspaceHolder.transform);
            Vector3 position = new Vector3(item.posX, item.posY, item.posZ);
            toInstantiate.transform.localPosition = position;
            creator.Validate(position, toInstantiate);
        }
    }

    public void RepositionCurrentCreation()
    {
        foreach (var item in creation.creationDict)
        {
            item.Value.GetComponent<Rigidbody>().isKinematic = true;
            item.Value.transform.localPosition = item.Key;
        }
        currentMode = Mode.Placing;
    }

    public void ValidatePosition()
    {
        foreach (var item in creation.creationDict)
        {
            item.Value.GetComponent<Rigidbody>().isKinematic = false;
        }

        currentMode = Mode.Playing;
    }

    public void ReloadCurrentCreation()
    {
        Destroy(currentPlayingObject.gameObject);
        workspaceController.gameObject.SetActive(true);
        PopulateFromDict();
        creator.PlaceNext();
        currentMode = Mode.Building;
    }

    public void PopulateFromDict()
    {
        Dictionary<Vector3, Block> tempDict = new Dictionary<Vector3, Block>();
        foreach (var item in creation.creationDict)
            tempDict.Add(item.Key, item.Value);

        creation.creationDict.Clear();
        foreach (var item in tempDict)
        {
            Block block = Instantiate(item.Value, workspaceHolder.transform);
            block.DisablePhysics();
            block.ResetCreationState();
            creator.Validate(block.position, block.gameObject);
        }
    }
}

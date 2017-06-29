using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Sharing.VoiceChat;

/// <summary>
/// Main controller for the application. Holds the state of the application and references to other controllers.
/// It is also the main hub for messages, dispathcing instructions to the right controller.
/// </summary>
public class MainController : Singleton<MainController>
{
    public enum RunningStates
    {
        scanningState,
        scanningDone,
        anchorState,
        verandaPlaced,
        none
    }

    [HideInInspector]
    private AnchorController anchorController;
    [HideInInspector]
    private ScanController scanController;
    [HideInInspector]
    public AudioManager audioManager;
    public GameObject guiPanel;
    private RunningStates state;
    public SurfaceMeshesToPlanes surfaceMeshToPlane;
    public VerandaController verandaController;

    public GameObject expertPanelPrefab;
    public GameObject hybridePanelPrefab;

    public GameObject crown;

    public GameObject verandaPreviewPrefab;
    // Accessed to adapt behavior to the global scene.
    public bool hybridScene = false;
    public bool voiceChatActive;

    public RunningStates State
    {
        get { return state; }
        set
        {
            state = value;
            if(guiPanel != null)
            {
                guiPanel.GetComponentInChildren<GuiPanel>().UpdateState(value);
            }
        }
    }

    public bool ScanAble
    {
        get
        {
            return state == RunningStates.none || state == RunningStates.scanningDone;
        }
    }
    // Use this for initialization
    void Start () {
        anchorController = AnchorController.Instance;
        scanController = ScanController.Instance;
        audioManager = AudioManager.Instance;
        verandaController = VerandaController.Instance;
        audioManager.InitSounds();
        audioManager.PlaySound(audioManager.welcome);
        ShareManager.Instance.onMessageEvent += OnMessageEvent;

        voiceChatActive = false;
    }

    public void SetPanelsReady()
    {
        StartCoroutine(SpawnGuiPanel());
    }

    private IEnumerator SpawnGuiPanel()
    {
        guiPanel = null;
        GameObject toSpawn = null;
        if (ShareManager.Instance.userType == NetworkSpawnManager.HYBRIDE)
            toSpawn = hybridePanelPrefab;
        else
            toSpawn = expertPanelPrefab;
        int timer = 50;

        do
        {
            if (ShareManager.Instance.spawnManager != null && ShareManager.Instance.spawnManager.SyncSourceReady())
            {
                guiPanel = ShareManager.Instance.spawnManager.Spawn(new SyncPanel(), toSpawn, NetworkSpawnManager.EVERYONE, "");
                guiPanel.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
                ShareManager.Instance.spawnManager.Spawn(new SyncSpawnedObject(), crown, NetworkSpawnManager.EVERYONE, "");
            }

            yield return new WaitForSeconds(0.1f);
            timer -= 1;
        }
        while (guiPanel == null && timer > 0);

        if(guiPanel == null)
        {
            guiPanel = ShareManager.Instance.spawnManager.Spawn(new SyncPanel(), toSpawn, NetworkSpawnManager.EVERYONE, "");
            guiPanel.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1;
        }

        State = RunningStates.none;
    }

    private void OnMessageEvent(MessageSynchronizer syncMessage)
    {
        Debug.Log("received message " + syncMessage.messageType.Value + ", " + syncMessage.stringData.Value);
        switch (syncMessage.messageType.Value)
        {
            case ShareManager.CHANGE_VERANDA:
                VerandaController.Instance.ValidateVeranda(SharedController.Instance.currentVerandaId);   
                break;
            case ShareManager.ADJUST_ANCHORS:
                AnchorController.Instance.AdjustAnchorsPositions(syncMessage.vectorData.Value.x, syncMessage.vectorData.Value.y);
                break;
            case ShareManager.VOICE_CHAT:
                EnableVoiceChat(syncMessage.integerData.Value == 1);
                break;
            case ShareManager.RESET_ALL:
                ResetAll();
                break;
            default:
                break;
        }
    }

    public void EnableVoiceChat(bool enable)
    {
        Debug.Log("enable voice chat: " + enable);
        Camera.main.gameObject.GetComponent<MicrophoneTransmitter>().enabled = enable;
        Camera.main.gameObject.GetComponent<MicrophoneReceiver>().enabled = enable;
        voiceChatActive = enable;
    }

    public void StartMapping()
    {
        if(ScanAble)
        { 
            if(scanController.StartMapping())
            { 
                State = RunningStates.scanningState;
                audioManager.PlaySound(audioManager.scan);
            }
        }
    }

    public void StopMapping()
    {
        if (State == RunningStates.scanningState)
        {
            if (scanController.StopMapping())
            { 
                State = RunningStates.scanningDone;
                audioManager.StopPlayingSound();
            }
        }
    }

    public void StartPlacingAnchors()
    {
        if (State == RunningStates.scanningDone)
        {
            if(anchorController.StartAnchorsPlacing())
            {
                //VuforiaBehaviour.Instance.enabled = false;
                SpaceManager.Instance.StopWallSelection();
                State = RunningStates.anchorState;
                InfoDisplay.Instance.UpdateText("Placing anchors.");
                audioManager.PlaySound(audioManager.anchors);
                SpaceManager.Instance.DisplayWallSurfaces();
            }
        }
    }

    public void StopPlacingAnchors()
    {
        if(State == RunningStates.anchorState)
        {
            if (anchorController.StopAnchorsPlacing())
            { 
                
                State = RunningStates.none;
                audioManager.StopPlayingSound();
            }
        }
    }

    public void Undo()
    {
        if(State == RunningStates.anchorState)
        {
            if(VerandaController.Instance.verandaPlaced)
            {
                VerandaController.Instance.RemoveVeranda();
            }
            anchorController.UndoAnchors();
        }
    }

    public void ResetClick()
    {
        ShareManager.Instance.SendSyncMessage(ShareManager.RESET_ALL);
    }
    
    public void ResetAll()
    {
        if(State == RunningStates.scanningDone)
            scanController.ResetMapping();

        else if (State == RunningStates.anchorState || State == RunningStates.verandaPlaced)
        {
            scanController.ResetMapping();
            anchorController.ResetAnchors();
        }

        if (VerandaController.Instance.verandaPlaced == true)
        {
            VerandaController.Instance.RemoveVeranda();
        }

        State = RunningStates.none;
    }

    public void Exit()
    {
        Application.Quit();
    }
}

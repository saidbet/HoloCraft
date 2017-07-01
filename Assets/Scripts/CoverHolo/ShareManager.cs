using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Sharing.Tests;
using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShareManager : Singleton<ShareManager> {

    public MessageSynchronizer syncMessage;
    //public bool synced = false;
    public bool anchorSynced = false;
    public event Action<MessageSynchronizer> onMessageEvent;
    public int userType;
    public bool readyToSend;

    public List<SyncMessage> messageQueue;

    [HideInInspector]
    public NetworkSpawnManager spawnManager;

    public const int DISPLAY_VERANDA = 0;
    public const int VERANDA_PLACED = 1;
    public const int ANCHORS_LIST = 2;
    public const int ADJUST_ANCHORS = 3;
    public const int CHANGE_VERANDA = 5;
    public const int ADD_TO_CACHE = 6;
    public const int SELECTED_WALLS = 7;
    public const int PREVIEWED_VERANDA_CHANGED = 8;
    public const int SET_ACTIVE = 9;
    public const int HIGHLIGHT = 10;
    public const int NEXT_PAGE = 11;
    public const int PREV_PAGE = 12;
    public const int COLOR_MODE = 13;
    public const int FURNITURES_MODE = 14;
    public const int VOICE_CHAT = 15;
    public const int HIDE_PREVIEW = 16;
    public const int CLICKED = 17;
    public const int CHANGE_STATE = 18;
    public const int CHANGE_COLOR = 19;
    public const int WORLD_ANCHOR = 20;
    public const int KEYDOWN = 31;
    public const int KEYUP = 32;
    public const int RESET_ALL = 99;
    public const int MESSAGE_ACK = 100;

    protected override void Awake()
    {
        base.Awake();
        syncMessage = null;
        spawnManager = GetComponent<NetworkSpawnManager>();
        SetUserType();
        messageQueue = new List<SyncMessage>();
    }

    void Start ()
    {
        if(InfoDisplay.Instance != null)
        {
            InfoDisplay.Instance.UpdateText("Synchronizing...");
        }
    }

    private void Update()
    {
        if(anchorSynced == false)
        {
            if (ImportExportAnchorManager.Instance.AnchorEstablished)
            {
                if (InfoDisplay.Instance != null)
                {
                    InfoDisplay.Instance.UpdateText("Synchronized");
                }
                anchorSynced = true;
                readyToSend = true;
            }
        }
        if(readyToSend && messageQueue.Count > 0)
        {
            SendSyncMessage(messageQueue[0]);
            messageQueue.RemoveAt(0);
        }
    }

    public void UpdateMessage(MessageSynchronizer message)
    {
        // Called from the Message Observer, fire the event for all listeners
        if((syncMessage != message || !message.othersOnly.Value) && onMessageEvent != null)
        {
            onMessageEvent(message);
        }
    }

    private void SendSyncMessage(SyncMessage msg)
    {
        if (syncMessage != null)
        {
            syncMessage.messageType.Value = msg.messageType.Value;
            syncMessage.stringData.Value = msg.stringData.Value;
            syncMessage.vectorData.Value = msg.vectorData.Value;
            syncMessage.quaternionData.Value = msg.quaternionData.Value;
            syncMessage.floatData.Value = msg.floatData.Value;
            syncMessage.updated.Value = true;
            syncMessage.integerData.Value = msg.integerData.Value;
            syncMessage.othersOnly.Value = msg.othersOnly.Value;
            syncMessage.boolData.Value = msg.boolData.Value;
            syncMessage.timeStamp.Value = DateTime.Now.Ticks;
            syncMessage.ackRequired.Value = msg.ackRequired.Value;
            if (msg.ackRequired.Value)
            {
                readyToSend = false;
            }
        }
    }

    public void SendAckMessage(long timeStamp)
    {
        if(syncMessage != null)
        {
            syncMessage.messageType.Value = MESSAGE_ACK;
            syncMessage.timeStamp.Value = timeStamp;
        }
    }

    public void SendSyncMessage(int messageType, string message, Vector3 vectorData, Quaternion quaternionData, float floatData, int integerData, bool othersOnly, bool boolData, bool ackRequired)
    {
        SyncMessage msg = new SyncMessage();
        msg.messageType.Value = messageType;
        msg.stringData.Value = message;
        msg.vectorData.Value = vectorData;
        msg.quaternionData.Value = quaternionData;
        msg.floatData.Value = floatData;
        msg.updated.Value = true;
        msg.integerData.Value = integerData;
        msg.othersOnly.Value = othersOnly;
        msg.boolData.Value = boolData;
        msg.ackRequired.Value = ackRequired;
        messageQueue.Add(msg);
    }
    #region SendSyncMessage overloads
    public void SendSyncMessage(int messageType, string message, Vector3 vectorData, Quaternion quaternionData, float floatData, int integerData, bool othersOnly, bool boolData)
    {
        SendSyncMessage(messageType, message, vectorData, quaternionData, floatData, integerData, othersOnly, boolData, false);
    }

    public void SendSyncMessage(int messageType, int integerData, float floatData)
    {
        SendSyncMessage(messageType, "", Vector3.zero, Quaternion.identity, floatData, integerData, false, false);
    }

    public void SendSyncMessage(int messageType, string message, Vector3 vectorData, float floatData, int integerData, bool othersOnly, bool boolData)
    {
        SendSyncMessage(messageType, message, vectorData, Quaternion.identity, floatData, integerData, othersOnly, boolData);
    }

    public void SendSyncMessage(int messageType, string message, Vector3 vectorData, float floatData, int integerData, bool othersOnly)
    {
        SendSyncMessage(messageType, message, vectorData, floatData, integerData, othersOnly, false);
    }

    public void SendSyncMessage(int messageType, string message, Vector3 vectorData, float floatData, int integerData)
    {
        SendSyncMessage(messageType, message, vectorData, floatData, integerData, false);
    }

    public void SendSyncMessage(int messageType, string message)
    {
        SendSyncMessage(messageType, message, Vector3.zero, 0f, 0, false);
    }

    public void SendSyncMessage(int messageType, Vector3 vectorData, Quaternion quaternionData)
    {
        SendSyncMessage(messageType, "", vectorData, quaternionData, 0f, 0, false, false);
    }

    public void SendSyncMessage(int messageType, string message, bool othersOnly)
    {
        SendSyncMessage(messageType, message, Vector3.zero, 0f, 0, othersOnly);
    }

    public void SendSyncMessage(int messageType)
    {
        SendSyncMessage(messageType, "", Vector3.zero, 0f, 0, false);
    }

    public void SendSyncMessage(int messageType, bool othersOnly)
    {
        SendSyncMessage(messageType, "", Vector3.zero, 0f, 0, othersOnly);
    }

    public void SendSyncMessage(int messageType, int integerData)
    {
        SendSyncMessage(messageType, "", Vector3.zero, 0f, integerData, false);
    }

    public void SendSyncMessage(int messageType, int integerData, bool othersOnly)
    {
        SendSyncMessage(messageType, "", Vector3.zero, 0f, integerData, othersOnly);
    }

    public void SendSyncMessage(int messageType, bool boolData, string stringData, bool ackRequired)
    {
        SendSyncMessage(messageType, stringData, Vector3.zero, Quaternion.identity, 0, 0, false, boolData, ackRequired);
    }

    public void SendSyncMessage(int messageType, Vector3 vectorData)
    {
        SendSyncMessage(messageType, "", vectorData, 0, 0);
    }
    #endregion
    private void SetUserType()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "scene_expert")
        {
            userType = NetworkSpawnManager.EXPERT;
        }
        else if (sceneName == "scene_client")
        {
            userType = NetworkSpawnManager.CLIENT;
        }
        else if(sceneName == "scene_hybride")
        {
            userType = NetworkSpawnManager.HYBRIDE;
        }
        else
        {
            userType = NetworkSpawnManager.EVERYONE;
        }
    }
}

using HoloToolkit.Sharing;
using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageObserver : Singleton<MessageObserver> {

    public Dictionary<MessageSynchronizer, long> messages = new Dictionary<MessageSynchronizer, long>();
    public GameObject syncMessagePrefab;
    public bool offline = false;

    // Use this for initialization
    void Start () {
        // Register on an event to know when we are connected. When we are, create our syncMessage and look for others
        SharingStage.Instance.SessionsTracker.CurrentUserJoined += SessionsTracker_CurrentUserJoined;
	}

    private void SessionsTracker_CurrentUserJoined(Session obj)
    {
        GameObject[] sMessage = GameObject.FindGameObjectsWithTag("SyncMessage");
        if (sMessage != null)
        {
            foreach (GameObject g in sMessage)
            {
                MessageSynchronizer mSync = g.GetComponent<MessageSynchronizer>();
                if (mSync != null && !messages.ContainsKey(mSync))
                {
                    messages.Add(mSync, mSync.timeStamp.Value);
                }
            }
        }
        SyncMessage message = new SyncMessage();
        ShareManager.Instance.syncMessage = ShareManager.Instance.spawnManager.Spawn(message, syncMessagePrefab, NetworkSpawnManager.EVERYONE, "MessageSynchronizer").GetComponent<MessageSynchronizer>();
        if (!messages.ContainsKey(ShareManager.Instance.syncMessage))
        {
            messages.Add(ShareManager.Instance.syncMessage, 0);
            ShareManager.Instance.SendSyncMessage(ShareManager.MESSAGE_ACK);
        }
    }

    public void StartOfflineMode()
    {
        offline = true;
        SyncMessage message = new SyncMessage();
        ShareManager.Instance.syncMessage = ShareManager.Instance.spawnManager.Spawn(message, syncMessagePrefab, NetworkSpawnManager.EVERYONE, "MessageSynchronizer").GetComponent<MessageSynchronizer>();
        if (!messages.ContainsKey(ShareManager.Instance.syncMessage))
        {
            messages.Add(ShareManager.Instance.syncMessage, 0);
            ShareManager.Instance.SendSyncMessage(ShareManager.MESSAGE_ACK);
        }
    }

    // Update is called once per frame
    void Update () {
        // At every frame, check if the timestamp of the messages is updated
        List<MessageSynchronizer> listKeys = new List<MessageSynchronizer>(messages.Keys);
        foreach(MessageSynchronizer m in listKeys)
        {
            if(messages.ContainsKey(m))
            {
                if(m.timeStamp.Value > messages[m] && m.messageType.Value != ShareManager.MESSAGE_ACK)
                {
                    messages[m] = m.timeStamp.Value;
                    ShareManager.Instance.UpdateMessage(m);
                    if(m != ShareManager.Instance.syncMessage && m.ackRequired.Value)
                    {
                        ShareManager.Instance.SendAckMessage(m.timeStamp.Value);
                    }
                } else if(m.messageType.Value == ShareManager.MESSAGE_ACK)
                {
                    if(ShareManager.Instance.syncMessage.timeStamp.Value == m.timeStamp.Value && !ShareManager.Instance.readyToSend)
                    {
                        ShareManager.Instance.readyToSend = true;
                    }
                }
            }
        }
        if(messages.Count == 1)
        {
            ShareManager.Instance.readyToSend = true;
        }
	}
}

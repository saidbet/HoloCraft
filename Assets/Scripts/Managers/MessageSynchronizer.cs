using HoloToolkit.Sharing.SyncModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Sharing;

public class MessageSynchronizer : Synchronizer
{

    public SyncBool updated;
    public SyncBool boolData;
    public SyncString stringData;
    public SyncVector3 vectorData;
    public SyncQuaternion quaternionData;
    public SyncInteger messageType;
    public SyncFloat floatData;
    public SyncInteger integerData;
    public SyncLong timeStamp;
    public SyncBool othersOnly;
    public SyncBool ackRequired;

    public override void LinkData(SyncSpawnedObject dataModel)
    {
        SyncMessage syncMessage = (SyncMessage)dataModel;
        this.floatData = syncMessage.floatData;
        this.messageType = syncMessage.messageType;
        this.stringData = syncMessage.stringData;
        this.updated = syncMessage.updated;
        this.vectorData = syncMessage.vectorData;
        this.quaternionData = syncMessage.quaternionData;
        this.integerData = syncMessage.integerData;
        this.timeStamp = syncMessage.timeStamp;
        this.othersOnly = syncMessage.othersOnly;
        this.boolData = syncMessage.boolData;
        this.ackRequired = syncMessage.ackRequired;
    }

    private void Start()
    {
        RegisterOnObserver();
    }

    public void RegisterOnObserver()
    {
        // Register on the MessageObserver
        // Can be called manually if this message isn't on the observer
        if(!MessageObserver.Instance.messages.ContainsKey(this))
        MessageObserver.Instance.messages.Add(this, 0);
    }

    private void OnDestroy()
    {
        // Remove itself from the observer
        if (MessageObserver.Instance != null && MessageObserver.Instance.messages.ContainsKey(this))
        {
            MessageObserver.Instance.messages.Remove(this);
        }
    }
}

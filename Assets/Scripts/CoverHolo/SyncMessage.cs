using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Sharing.SyncModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Sharing;
using System;

[SyncDataClass]
public class SyncMessage : SyncSpawnedObject
{
    [SyncData]
    public SyncBool updated;

    [SyncData]
    public SyncBool boolData;

    [SyncData]
    public SyncString stringData;

    [SyncData]
    public SyncVector3 vectorData;

    [SyncData]
    public SyncQuaternion quaternionData;

    [SyncData]
    public SyncInteger messageType;

    [SyncData]
    public SyncFloat floatData;

    [SyncData]
    public SyncInteger integerData;

    [SyncData]
    public SyncLong timeStamp;

    [SyncData]
    public SyncBool othersOnly;

    [SyncData]
    public SyncBool ackRequired;

}

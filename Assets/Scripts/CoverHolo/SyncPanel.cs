using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Sharing.SyncModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SyncDataClass]
public class SyncPanel : SyncSpawnedObject
{
    [SyncData]
    public SyncInteger integerData;
    [SyncData]
    public SyncInteger userId;
    [SyncData]
    public SyncInteger nbrUsers;
    [SyncData]
    public SyncInteger message;
    [SyncData]
    public SyncBool updated;
    [SyncData]
    public SyncString buttonName;
} 

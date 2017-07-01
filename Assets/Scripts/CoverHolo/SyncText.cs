using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Sharing.SyncModel;

[SyncDataClass]
public class SyncText : SyncSpawnedObject
{
    [SyncData]
    public SyncString stringData;
}

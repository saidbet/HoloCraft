//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using UnityEngine;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Sharing.Tests
{
    /// <summary>
    /// Class that handles spawning sync objects on keyboard presses, for the SpawningTest scene.
    /// </summary>
    public class SyncObjectSpawner : MonoBehaviour
    {
        public GameObject sphere;

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.I))
            {
                SpawnBasicSyncObject();
            }
        }

        public void SpawnBasicSyncObject()
        {
            Vector3 position = Random.onUnitSphere * 2;
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

            var spawnedObject = new SyncSpawnedObject();

            ShareManager.Instance.spawnManager.Spawn(spawnedObject, sphere, NetworkSpawnManager.EVERYONE, "", null, position, rotation, null);
        }
    }
}

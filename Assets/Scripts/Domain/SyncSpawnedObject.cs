//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using UnityEngine;
using HoloToolkit.Sharing.SyncModel;

namespace HoloToolkit.Sharing.Spawning
{
    /// <summary>
    /// A SpawnedObject contains all the information needed for another device to spawn an object in the same location
    /// as where it was originally created on this device.
    /// </summary>
    [SyncDataClass]
    public class SyncSpawnedObject : SyncObject
    {
        /// <summary>
        /// Transform (position, orientation and scale) for the object.
        /// </summary>
        [SyncData]
        public SyncTransform Transform;

        /// <summary>
        /// Name of the object.
        /// </summary>
        [SyncData]
        public SyncString Name;

        /// <summary>
        /// Path to the parent object in the game object.
        /// </summary>
        [SyncData]
        public SyncString ParentPath;

        /// <summary>
        /// Path to the object
        /// </summary>
        [SyncData]
        public SyncString prefabPath;

        /// <summary>
        /// With which group of users should this be shared
        /// </summary>
        /// 0 = everyone
        /// 1 = experts
        /// 2 = clients
        [SyncData]
        public SyncInteger userGroup;

        /// <summary>
        /// Contains the synchronizer to add to the GameObjects under the form of a string
        /// </summary>
        [SyncData]
        public SyncString synchronizerToAdd;

        /// <summary>
        /// State of the gameobject
        /// </summary>
        [SyncData]
        public SyncBool isEnabled;

        /// <summary>
        /// Key of the object to spawn from cache
        /// </summary>
        [SyncData]
        public SyncInteger id;

        [SyncData]
        public SyncInteger ownerId;

        [SyncData]
        public SyncBool fromCache;

        public GameObject GameObject { get; set; }

        public virtual void Initialize(string name, string parentPath, string objectPath, int userGroup, int ownerId)
        {
            Name.Value = name;

            ParentPath.Value = parentPath;

            prefabPath.Value = objectPath;

            this.userGroup.Value = userGroup;

            id = null;

            this.ownerId.Value = ownerId;
        }

        public virtual void Initialize(string name, string parentPath, int id, int userGroup, int ownerId)
        {
            Name.Value = name;

            ParentPath.Value = parentPath;

            this.id.Value = id;

            this.userGroup.Value = userGroup;

            prefabPath = null;

            this.ownerId.Value = ownerId;
        }
    }
}

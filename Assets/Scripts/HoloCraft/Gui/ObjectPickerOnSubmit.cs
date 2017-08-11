﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectPickerOnSubmit : EventTrigger {

    public BlockType blockType;

    public override void OnSubmit(BaseEventData eventData)
    {
        MainManager.Instance.ChangeObject(blockType.prefab);
    }
}

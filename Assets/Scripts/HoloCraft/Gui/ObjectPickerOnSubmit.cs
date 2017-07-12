using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectPickerOnSubmit : EventTrigger {

    public override void OnSubmit(BaseEventData eventData)
    {
        MainManager.Instance.ChangeObject(eventData.selectedObject.GetComponent<BlockHolder>().blockType.prefab);
    }
}

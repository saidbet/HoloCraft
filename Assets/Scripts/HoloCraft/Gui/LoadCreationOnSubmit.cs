using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoadCreationOnSubmit : EventTrigger {

    public string creationName;

    public override void OnSubmit(BaseEventData eventData)
    {
        MainManager.Instance.creation.LoadCreation(creationName);
		MenuManager.Instance.ToggleMenu (MenuManager.Instance.LoadingMenu);
    }
}

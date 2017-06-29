using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FurnitureController : MonoBehaviour, IInputClickHandler
{

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if(ObjectPlacingController.Instance.CurrentState == ObjectPlacingController.EditState.none)
        {
            if (!ObjectPlacingController.Instance.modelEditingMenu.visible)
            {
                ObjectPlacingController.Instance.modelEditingMenu.DisplayMenu(this);
            }
            else
                ObjectPlacingController.Instance.modelEditingMenu.SetActive(false);
        }
    }
}

using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IpFieldCollider : MonoBehaviour, IInputClickHandler {

    public IpInputPanel ipInputPanel;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if(ipInputPanel != null)
        {
            ipInputPanel.selectIpField(gameObject.GetComponent<TextMesh>());
        }
    }
}

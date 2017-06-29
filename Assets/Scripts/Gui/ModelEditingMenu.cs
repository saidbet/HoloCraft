using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelEditingMenu : BasePanel
{
    public BaseButton move;
    public BaseButton rotate;
    public BaseButton scale;
    public BaseButton delete;
    private FurnitureController currentFurniture;
    public bool visible;

    protected override void Start()
    {
        base.Start();
        ShareManager.Instance.onMessageEvent += Instance_onMessageEvent;
        SetActive(false);
    }

    public void DisplayMenu(FurnitureController furniture)
    {
        //Vector3 position = Vector3.MoveTowards(furniture.GetComponent<BoxCollider>().bounds.center + furniture.GetComponent<BoxCollider>().bounds.center/2, Camera.main.transform.position, 1F);
        transform.position = CalculatePosition(furniture.gameObject);
        currentFurniture = furniture;
        SetActive(true);
    }

    private void Instance_onMessageEvent(MessageSynchronizer obj)
    {
        switch (obj.messageType.Value)
        {
            case ShareManager.SET_ACTIVE:
                if (SharedController.GetPath(gameObject) == obj.stringData.Value)
                    SharedController.SetActive(gameObject, obj.boolData.Value);
                break;
        }
    }

    private Vector3 CalculatePosition(GameObject furniture)
    {
        Vector3 distance = GazeManager.Instance.HitPosition - Camera.main.transform.position;
        Vector3 normalized = distance.normalized;
        Vector3 result = GazeManager.Instance.HitPosition - (0.5f * normalized);
        return result;
    }

    public void SetActive(bool state)
    {
        ShareManager.Instance.SendSyncMessage(ShareManager.SET_ACTIVE, state, SharedController.GetPath(gameObject), true);
    }

    public override void OnClick(BaseButton button)
    {
        if(button == move)
        {
            ObjectPlacingController.Instance.ChangeState(currentFurniture.gameObject, ObjectPlacingController.EditState.placing);
            SetActive(false);
        }

        else if(button == rotate)
        {
            ObjectPlacingController.Instance.ChangeState(currentFurniture.gameObject, ObjectPlacingController.EditState.rotating);
        }

        else if(button == scale)
        {
            ObjectPlacingController.Instance.ChangeState(currentFurniture.gameObject, ObjectPlacingController.EditState.scaling);
        }

        else if(button == delete)
        {
            ObjectPlacingController.Instance.Delete(currentFurniture.gameObject);
        }
    }
}

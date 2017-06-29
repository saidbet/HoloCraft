using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerandasPanel : BasePanel
{
    public BaseButton prevButton;
    public BaseButton selectButton;
    public BaseButton nextButton;

    protected override void Start()
    {
        base.Start();
        ShareManager.Instance.onMessageEvent += Instance_onMessageEvent;
        transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2;
        ShareManager.Instance.SendSyncMessage(ShareManager.PREVIEWED_VERANDA_CHANGED, 1);
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

    private void PreviousPage()
    {
        if (CloudDataManager.Instance.prefabCache.ContainsKey(SharedController.Instance.currentVerandaId - 1))
        {
            ShareManager.Instance.SendSyncMessage(ShareManager.PREVIEWED_VERANDA_CHANGED, SharedController.Instance.currentVerandaId - 1);
        }
        else
        {
            ShareManager.Instance.SendSyncMessage(ShareManager.PREVIEWED_VERANDA_CHANGED, CloudDataManager.Instance.prefabCache.Count);
        }
    }

    private void NextPage()
    {
        if (CloudDataManager.Instance.prefabCache.ContainsKey(SharedController.Instance.currentVerandaId + 1))
            ShareManager.Instance.SendSyncMessage(ShareManager.PREVIEWED_VERANDA_CHANGED, SharedController.Instance.currentVerandaId+1);
        else
        {
            ShareManager.Instance.SendSyncMessage(ShareManager.PREVIEWED_VERANDA_CHANGED, 1);
        }
    }

    public override void OnClick(BaseButton button)
    {
        if(button == prevButton)
        {
            PreviousPage();
        }
        else if(button == nextButton)
        {
            NextPage();
        }
        else if(button == selectButton)
        {
            ChangeVeranda();
        }
    }

    private void ChangeVeranda()
    {
        ShareManager.Instance.SendSyncMessage(ShareManager.CHANGE_VERANDA);
    }

    public void SetActive(bool state)
    {
        if (state == true)
            transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
        ShareManager.Instance.SendSyncMessage(ShareManager.SET_ACTIVE, state, SharedController.GetPath(gameObject), true);
    }


}

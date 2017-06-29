using HoloToolkit.Sharing;
using HoloToolkit.Sharing.SyncModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Sharing.Spawning;

public class PanelSynchronizer : Synchronizer
{
    public SyncInteger integerData;
    public SyncInteger userId;
    public SyncInteger nbrUsers;
    public SyncInteger message;
    public SyncBool updated;
    public SyncString buttonName;

    protected Dictionary<string, BaseButton> buttons;

    public BasePanel panel;
    protected bool localUpdated;

    protected virtual void Start()
    {
        buttons = new Dictionary<string, BaseButton>();
        foreach (BaseButton button in gameObject.GetComponentsInChildren<BaseButton>())
        {
            buttons.Add(button.name, button);
        }
        localUpdated = true;
    }

    private void Update()
    {
        if (updated.Value == true && localUpdated == true && userId.Value != SharingStage.Instance.Manager.GetLocalUser().GetID())
        {
            BaseButton button = buttons[buttonName.Value];
            switch (message.Value)
            {
                case 0:
                    StartCoroutine(ScaleEffect(button));
                    break;
                case 1:
                    StartCoroutine(FocusEnter(button));
                    break;
                case 2:
                    StartCoroutine(FocusEnter(button));
                    break;
                default:
                    break;
            }

            localUpdated = false;
        }

        if (nbrUsers.Value <= 0)
        {
            updated.Value = false;
        }
    }

    public void UpdateButton(string buttonName, int message, int integerData)
    {
        this.integerData.Value = integerData;
        this.userId.Value = SharingStage.Instance.Manager.GetLocalUser().GetID();
        this.message.Value = message;
        nbrUsers.Value = SharingStage.Instance.SessionUsersTracker.CurrentUsers.Count;
        updated.Value = true;
    }

    public override void LinkData(SyncSpawnedObject dataModel)
    {
        SyncPanel syncPanel = (SyncPanel)dataModel;

        integerData = syncPanel.integerData;
        userId = syncPanel.userId;
        nbrUsers = syncPanel.nbrUsers;
        message = syncPanel.message;
        updated = syncPanel.updated;
        buttonName = syncPanel.buttonName;
}

    protected IEnumerator ScaleEffect(BaseButton button)
    {
        button.ScaleEffect();
        yield return new WaitForSeconds(1f);
        localUpdated = true;
    }

    protected IEnumerator FocusEnter(BaseButton button)
    {
        button.OnFocusEnter();
        yield return new WaitForSeconds(1f);
        localUpdated = true;
    }

    protected IEnumerator FocusExit(BaseButton button)
    {
        button.OnFocusExit();
        yield return new WaitForSeconds(1f);
        localUpdated = true;
    }
}

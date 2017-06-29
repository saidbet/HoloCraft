using HoloToolkit.Sharing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiPanelSynchronizer : PanelSynchronizer {

    private GuiPanel guiPanel;

    protected override void Start()
    {
        base.Start();
        guiPanel = GetComponent<GuiPanel>();
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
                    StartCoroutine(FocusExit(button));
                    break;
                case 3:
                    StartCoroutine(ChangeState((MainController.RunningStates)integerData.Value));
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

    private IEnumerator ChangeState(MainController.RunningStates state)
    {
        guiPanel.UpdateState(state);
        yield return new WaitForSeconds(1f);
        localUpdated = true;
    }
}



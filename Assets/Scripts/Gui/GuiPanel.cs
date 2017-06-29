using HoloToolkit.Sharing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiPanel : BasePanel
{

    public BaseButton scanButton;
    public BaseButton undoButton;
    public BaseButton resetButton;
    public BaseButton settingsButton;

    public GameObject settingsGuiPrefab;

    protected override void Start()
    {
        base.Start();
        settingsButton.ChangeState(0);
    }

    public void UpdateState(MainController.RunningStates state)
    {
        if (state == MainController.RunningStates.scanningState)
        {
            scanButton.ChangeState(1);
            undoButton.ChangeState(2);
            resetButton.ChangeState(2);
        }
        else if (state == MainController.RunningStates.anchorState)
        {
            scanButton.ChangeState(2);
            undoButton.ChangeState(0);
            resetButton.ChangeState(0);
        }
        else if (state == MainController.RunningStates.verandaPlaced)
        {
            scanButton.ChangeState(2);
            undoButton.ChangeState(2);
            resetButton.ChangeState(0);
        }
        else if (state == MainController.RunningStates.none || state == MainController.RunningStates.scanningDone)
        {
            if(!VerandaController.Instance.verandaPlaced)
            {
                scanButton.ChangeState(0);
                undoButton.ChangeState(2);
                resetButton.ChangeState(0);
            }
            else if(VerandaController.Instance.verandaPlaced)
            {
                scanButton.ChangeState(2);
                undoButton.ChangeState(2);
                resetButton.ChangeState(0);
            }
        }
        GuiPanelSynchronizer sync = GetComponent<GuiPanelSynchronizer>();
        if(sync != null)
        {
            sync.UpdateButton("", 3, (int)state);
        }
        
    }

    public override void OnClick(BaseButton button)
    {
        if(button == scanButton)
        {
            if (button.state)
                MainController.Instance.StartMapping();
            else
                MainController.Instance.StopMapping();
        }
        else if(button == undoButton)
        {
            if(button.state)
                MainController.Instance.Undo();
        }
        else if(button == resetButton)
        {
            if (button.state)
                MainController.Instance.ResetAll();
        }
        else if(button == settingsButton)
        {
            ShareManager.Instance.spawnManager.Spawn(new SyncPanel(), settingsGuiPrefab, NetworkSpawnManager.EVERYONE, "PanelSynchronizer");
        }
    }
}

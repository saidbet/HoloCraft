using HoloToolkit.Sharing.Spawning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientGuiPanel : BasePanel {

    public BaseButton verandasButton;
    public BaseButton furnituresButton;
    public BaseButton settingsButton;

    public GameObject verandasGuiPrefab;
    public GameObject furnituresGuiPrefab;

    public GameObject verandasGui;
    public GameObject furnituresGui;

	protected override void Start ()
    {
        base.Start();
	}

    public override void OnClick(BaseButton button)
    {
        if(button == verandasButton)
        {
            if (button.state == true)
            {
                if (verandasGui == null)
                {
                    verandasGui = ShareManager.Instance.spawnManager.Spawn(new SyncSpawnedObject(), verandasGuiPrefab, NetworkSpawnManager.EVERYONE, "");
                }
                else
                    verandasGui.GetComponent<VerandasPanel>().SetActive(true);

                button.ChangeState(1);
            }
            else
            {
                verandasGui.GetComponent<VerandasPanel>().SetActive(false);

                button.ChangeState(0);
            }

        }
        else if(button == furnituresButton)
        {
            if(button.state == true)
            {
                if (furnituresGui == null)
                    furnituresGui = ShareManager.Instance.spawnManager.Spawn(new SyncSpawnedObject(), furnituresGuiPrefab, NetworkSpawnManager.EVERYONE, "");
                else
                    furnituresGui.GetComponent<FurnitureMenu>().SetActive(true);

                button.ChangeState(1);
            }
            else
            {
                if (furnituresGui != null)
                    furnituresGui.GetComponent<FurnitureMenu>().SetActive(false);

                button.ChangeState(0);
            }
        }
        else if(button == settingsButton)
        {
            //TODO
        }
    }


}

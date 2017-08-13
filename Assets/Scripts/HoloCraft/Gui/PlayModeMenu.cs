using UnityEngine;
using UnityEngine.UI;

public class PlayModeMenu : Menu
{
    public void RepositionActiveCreation()
    {
        MainManager.Instance.RepositionCurrentCreation();
        GetComponentInParent<Menu>().HideMenu();
    }

    public void ShowScanState(GameObject button)
    {
        if (ScanController.Instance.scanInProgress)
        {
            button.GetComponentInChildren<Text>().text = "Start Scanning";
            ScanController.Instance.StopMapping();
            GetComponentInParent<Menu>().HideMenu();
        }
        else
        {
            ScanController.Instance.StartMapping();
            button.GetComponentInChildren<Text>().text = "Stop Scanning";
            GetComponentInParent<Menu>().HideMenu();
        }
    }

    public void GoBackCreation()
    {
        MainManager.Instance.ReloadCurrentCreation();
        GetComponentInParent<Menu>().HideMenu();
    }
}

using UnityEngine;

public class WorkspaceMenu : MonoBehaviour
{
    public void StartMoveWorkspace()
    {
        MainManager.Instance.CurrentMode = MainManager.Mode.Moving;
        MenuManager.Instance.ToggleMenu(gameObject, false);
    }

    public void StartScaleWorkspace()
    {
        MainManager.Instance.CurrentMode = MainManager.Mode.Scaling;
        MenuManager.Instance.ToggleMenu(gameObject, false);
    }

    public void StartPlay()
    {
        MainManager.Instance.StartPlayMode();
    }
}

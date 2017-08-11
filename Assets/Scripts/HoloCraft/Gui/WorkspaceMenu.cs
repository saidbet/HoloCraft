using UnityEngine;
using UnityEngine.EventSystems;

public class WorkspaceMenu : MonoBehaviour, IMenu
{

    public int currentIndex;

    private void OnEnable()
    {
        currentIndex = 0;
    }

    public void StartMoveWorkspace()
    {
        MainManager.Instance.CurrentMode = MainManager.Mode.Moving;
        MenuManager.Instance.ToggleMenu(gameObject);
    }

    public void StartScaleWorkspace()
    {
        MainManager.Instance.CurrentMode = MainManager.Mode.Scaling;
        MenuManager.Instance.ToggleMenu(gameObject);
    }

    public void StartPlay()
    {
        MainManager.Instance.StartPlayMode();
        MenuManager.Instance.ToggleMenu(gameObject);
    }

    public void SaveCreation()
    {
        MainManager.Instance.creation.AddToCreationsList();
        MenuManager.Instance.ToggleMenu(gameObject);
    }

    public void LoadCreation()
    {
        MenuManager.Instance.ToggleMenu(MenuManager.Instance.LoadingMenu);
        MenuManager.Instance.ToggleMenu(gameObject);
    }

    public void MoveSelection(MainManager.Direction direction)
    {
        switch (direction)
        {
            case MainManager.Direction.Down:
                if (currentIndex < transform.childCount - 1)
                    currentIndex += 1;
                break;

            case MainManager.Direction.Up:
                if (currentIndex > 0)
                    currentIndex -= 1;
                break;

            default:
                break;
        }

        EventSystem.current.SetSelectedGameObject(transform.GetChild(currentIndex).gameObject);
    }
}

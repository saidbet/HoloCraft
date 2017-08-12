using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorkspaceMenu : MonoBehaviour, IMenu
{

    public int currentIndex;

    private void Update()
    {
        if (CInput.aUp)
            Submit();
    }

    private void OnEnable()
    {
        currentIndex = 0;
    }

    public void StartMoveWorkspace()
    {
        MenuManager.Instance.ToggleMenu(gameObject);
        MainManager.Instance.currentMode = MainManager.Mode.Moving;
    }

    public void StartScaleWorkspace()
    {
        MenuManager.Instance.ToggleMenu(gameObject);
        MainManager.Instance.currentMode = MainManager.Mode.Scaling;
    }

    public void StartPlay()
    {
        MainManager.Instance.StartPlayMode();
        MenuManager.Instance.ToggleMenu(gameObject);
    }

    public void SaveCreation()
    {
        MainManager.Instance.SaveData();
        MenuManager.Instance.ToggleMenu(gameObject);
    }

    public void LoadCreation()
    {
        MenuManager.Instance.ToggleMenu(gameObject);
        MenuManager.Instance.ToggleMenu(MenuManager.Instance.LoadingMenu);
    }

    public void MoveSelection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Down:
                if (currentIndex < transform.childCount - 1)
                    currentIndex += 1;
                break;

            case Direction.Up:
                if (currentIndex > 0)
                    currentIndex -= 1;
                break;

            default:
                break;
        }

        EventSystem.current.SetSelectedGameObject(transform.GetChild(currentIndex).gameObject);
    }

    public void Submit()
    {
        EventSystem.current.currentSelectedGameObject.GetComponent<EventTrigger>().OnSubmit(new BaseEventData(EventSystem.current));
    }
}

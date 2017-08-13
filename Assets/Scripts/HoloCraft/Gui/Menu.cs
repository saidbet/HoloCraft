using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private Direction direction;
    public Selectable[] selectables;
    public int nbrElementsPerLine;
    public int nbrSelectablePerline;
    public float elementSizeVert;
    public float elementSizeHoriz;
    public int currentIndex;

    public CInput.Key toggleKey;

    public MainManager.Mode previousMode;

    private void Update()
    {
        direction = CInput.GetDpadDirection();
        if (direction != Direction.None)
            MoveSelection(direction);

        if (CInput.GetSubmitKey())
            Submit();

        if (CInput.GetKeyUp(CInput.Key.B) || CInput.GetKeyUp(toggleKey))
            HideMenu();
    }

    public void HideMenu()
    {
        gameObject.SetActive(false);
    }

    protected virtual void OnEnable()
    {
        selectables = GetComponentsInChildren<Selectable>();
        EventSystem.current.SetSelectedGameObject(null);
        StartCoroutine(SetSelected());
        previousMode = MainManager.Instance.currentMode;
        MainManager.Instance.currentMode = MainManager.Mode.InMenu;
    }

    protected virtual void OnDisable()
    {
        if (MainManager.Instance.currentMode == MainManager.Mode.InMenu)
            MainManager.Instance.currentMode = previousMode;
    }

    protected virtual void MoveSelection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Down:
                if ((currentIndex + nbrSelectablePerline) < selectables.Length)
                    currentIndex += nbrSelectablePerline;
                break;

            case Direction.Up:
                if (currentIndex >= nbrSelectablePerline)
                    currentIndex -= nbrSelectablePerline;
                break;

            case Direction.Right:
                if (currentIndex < selectables.Length - 1)
                    currentIndex += 1;
                break;

            case Direction.Left:
                if (currentIndex > 0)
                    currentIndex -= 1;
                break;
        }

        EventSystem.current.SetSelectedGameObject(selectables[currentIndex].gameObject);
    }

    public Vector2 GetValidPosition(int index)
    {
        int x = index % nbrElementsPerLine;

        int y = index / nbrElementsPerLine;

        return new Vector2(x * elementSizeHoriz, -y * elementSizeVert);
    }

    public void Submit()
    {
        EventSystem.current.currentSelectedGameObject.GetComponent<EventTrigger>().OnSubmit(new BaseEventData(EventSystem.current));
    }

    IEnumerator SetSelected()
    {
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(selectables[currentIndex].gameObject);
    }
}

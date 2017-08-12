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
    public int elementSize;
    public int currentIndex;

    private void Update()
    {
        direction = CInput.GetDpadDirection();
        if (direction != Direction.None)
            MoveSelection(direction);
    }

    protected virtual void OnEnable()
    {
        currentIndex = 0;
        selectables = GetComponentsInChildren<Selectable>();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(selectables[currentIndex].gameObject);
    }

    protected virtual void MoveSelection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Down:
                if ((currentIndex + nbrElementsPerLine) < selectables.Length)
                    currentIndex += nbrElementsPerLine;
                break;

            case Direction.Up:
                if (currentIndex >= nbrElementsPerLine)
                    currentIndex -= nbrElementsPerLine;
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

        return new Vector2(x * elementSize, -y * elementSize);
    }
}

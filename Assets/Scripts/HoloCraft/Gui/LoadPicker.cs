using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadPicker : MonoBehaviour, IMenu
{
    public GameObject elementPrefab;
    public List<GameObject> guiElements;
    public int nbrElementsPerLine;
    public int elementSize;
    public int currentIndex;

    public void OnEnable()
    {
        if (MainManager.Instance.creation.creationsList == null) return;

        currentIndex = 0;

        guiElements = new List<GameObject>();

        for (int i = 0; i < MainManager.Instance.creation.creationsList.creations.Count; i++)
        {
            PlaceElement(i);
            guiElements[i].transform.GetComponentInChildren<Text>().text = MainManager.Instance.creation.creationsList.creations[i].name;
            guiElements[i].transform.GetComponent<LoadCreationOnSubmit>().creationName = MainManager.Instance.creation.creationsList.creations[i].name;
        }

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(guiElements[0]);
    }

    public void OnDisable()
    {
        foreach (GameObject go in guiElements)
        {
            Destroy(go);
        }
    }

    public void PlaceElement(int index)
    {
        guiElements.Add(Instantiate(elementPrefab, transform));
        guiElements[index].GetComponent<RectTransform>().anchoredPosition = GetValidPosition(index);
    }

    public Vector2 GetValidPosition(int index)
    {
        int x = index % nbrElementsPerLine;

        int y = index / nbrElementsPerLine;

        return new Vector2(x * elementSize, -y * elementSize);
    }

    public void MoveSelection(MainManager.Direction direction)
    {
        switch (direction)
        {
            case MainManager.Direction.Down:
                if ((currentIndex + nbrElementsPerLine) < guiElements.Count)
                    currentIndex += nbrElementsPerLine;
                break;

            case MainManager.Direction.Up:
                if (currentIndex >= nbrElementsPerLine)
                    currentIndex -= nbrElementsPerLine;
                break;

            case MainManager.Direction.Right:
                if (currentIndex < guiElements.Count)
                    currentIndex += 1;
                break;

            case MainManager.Direction.Left:
                if (currentIndex > 0)
                    currentIndex -= 1;
                break;
        }

        EventSystem.current.SetSelectedGameObject(guiElements[currentIndex]);
    }
}

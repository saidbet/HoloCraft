using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadPicker : MonoBehaviour
{
    public GameObject elementPrefab;
    public List<GameObject> guiElements;
    public int nbrElementsPerLine;
    public int elementSize;

    public void OnEnable()
    {
        if (MainManager.Instance.creation.creationsList == null) return;

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
        foreach(GameObject go in guiElements)
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
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadPicker : Menu
{
    public GameObject elementPrefab;
    public List<GameObject> guiElements;

    protected override void OnEnable()
    {
        if (MainManager.Instance.creationsList == null) return;

        currentIndex = 0;

        guiElements = new List<GameObject>();

        for (int i = 0; i < MainManager.Instance.creationsList.creations.Count; i++)
        {
            PlaceElement(i);
            guiElements[i].transform.GetComponentInChildren<Text>().text = MainManager.Instance.creationsList.creations[i].creationName;
            guiElements[i].transform.GetComponent<LoadCreationOnSubmit>().creationName = MainManager.Instance.creationsList.creations[i].creationName;
        }

        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

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
}
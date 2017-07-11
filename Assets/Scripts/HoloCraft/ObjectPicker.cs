using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ObjectPicker : MonoBehaviour
{
    public BlocksArray blocksArray;
    public GameObject elementPrefab;
    public GameObject[] guiElements;
    public int nbrElementsPerLine;
    public int elementSize;
    public HighlightManager highlight;

    private void Awake()
    {
        guiElements = new GameObject[blocksArray.array.Length];

        for (int i = 0; i < blocksArray.array.Length; i++)
        {
            PlaceElement(i);
            guiElements[i].transform.GetChild(0).GetComponent<Image>().sprite = blocksArray.array[i].thumbnail;
        }
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(guiElements[0]);
    }

    protected void PlaceElement(int index)
    {
        guiElements[index] = Instantiate(elementPrefab, transform);
        guiElements[index].GetComponent<RectTransform>().anchoredPosition = GetValidPosition(index);
    }

    protected Vector2 GetValidPosition(int index)
    {
        int x = index % nbrElementsPerLine;

        int y = index / nbrElementsPerLine;

        return new Vector2(x * elementSize, -y * elementSize);
    }

}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectPicker : Menu
{
    public BlocksArray blocksArray;
    public GameObject elementPrefab;
    public GameObject[] guiElements;

    private void Awake()
    {
        guiElements = new GameObject[blocksArray.array.Length];

        for (int i = 0; i < blocksArray.array.Length; i++)
        {
            PlaceElement(i);
            guiElements[i].transform.GetChild(0).GetComponent<Image>().sprite = blocksArray.array[i].thumbnail;
        }
    }

    protected void PlaceElement(int index)
    {
        guiElements[index] = Instantiate(elementPrefab, transform);
        guiElements[index].GetComponent<RectTransform>().anchoredPosition = GetValidPosition(index);
        guiElements[index].GetComponent<ObjectPickerOnSubmit>().blockType = blocksArray.array[index];
    }
}
